using RIPharmStatutesAggregator.Core;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RIPharmStatutesAggregator.Services
{
    public class PageAggregator
    {
        private string styleSheet =
@"<style>
   html {
        font-family: Calibri,Candara,Segoe,Segoe UI,Optima,Arial,sans-serif;
        background-color: #e3f1ff;
        Color: #424242;
    }
   #title{
        font-size: 300%;
        Color: #92b8d5;
        padding-left:70px;
        margin-top: 20px;
        margin-bottom: 20px;
    }
   #status{
        font-size: 100%;
        padding-left:70px;
    }
   #content{
        background-color: white;
        margin-left: 50px;
        margin-right: 50px;
    }
   h1 {
        text-decoration: underline;
        Color: #d09837;
        background-color: #496d89;
        padding-left:40px;
        padding-top:20px;
        padding-bottom:30px;
        margin-top: 30px;
        margin-bottom: 0px;
    }
    h2{
        Color: #424242;
        background-color: #92b8d5;
        padding-left:50px;
        padding-top:15px;
        padding-bottom:15px;
        margin: 0px;
}
   h3 {
        margin-left: 30px;
        text-decoration: underline;
    }
   h4 {
        margin-left: 70px;
    }
    p {
		margin-left: 70px;
		margin-right: 70px;
	}
    ul {
		margin-left: 70px;
	}
</style>";

        internal string Aggregate(List<Page> pages)
        {
            var indexPages = pages.Where(x => x.Path.Contains("INDEX.HTM"));

            var aggregatedPage = new StringBuilder();
            foreach (var indexPage in indexPages)
            {
                var chapter = FormatChapter(indexPage, pages);
                aggregatedPage.Append(chapter);
            }

            var html = AddTopLevelHTML(aggregatedPage.ToString());
            return html;
        }

        private string AddTopLevelHTML(string body)
        {
            var html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine(styleSheet);
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div id=\"title\">");
            html.AppendLine("Rhode Island Pharmacy Statutes");
            html.AppendLine("</div>");
            html.AppendLine("<div id=\"status\">");
            html.AppendLine("Last Updated: " + DateTime.Now);
            html.AppendLine("</div>");
            html.AppendLine("<div id=\"content\">");
            html.AppendLine(body);
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        private string FormatChapter(Page indexPage, List<Page> pages)
        {          
            var chapterName = Path.GetFileName(indexPage.Path).Replace("_INDEX.HTM", "");
            var subPages = pages.Where(x => x.Path.Contains(chapterName)).ToList();
            subPages.Remove(indexPage);

            var first = subPages.FirstOrDefault();
            var chapTitle = GetChapterTitle(first.Html);

            var sb = new StringBuilder();

            foreach(var page in subPages)
            {
                sb.Append(AdjustPage(page.Html));
            }
            return chapTitle + sb.ToString();
        }

        private string GetChapterTitle(string html)
        {
            var chapterTitleStartIndex = html.IndexOf("<H1>");
            var chapterTitleEndIndex = html.IndexOf("<H3>");
            return html.Substring(chapterTitleStartIndex, chapterTitleEndIndex - chapterTitleStartIndex);
        }

        private string AdjustPage(string html)
        {
            var header = FormatHeader(GetHeader(html));
            var body = "";
            var footer = "";
            if (!html.Contains("Repealed") && !html.Contains("Reserved"))
            {
                body = FormatBody(GetBody(html));
                footer = FormatFooter(GetFooter(html));
            }

            var adjustedPage = "<div id=\"section\">\r\n" + header + body + footer + "</div>\r\n";

            return adjustedPage;
        }

        private string GetHeader(string html)
        {
            var headerStartIndex = html.IndexOf("<B>") + 3;
            var headerEndIndex = html.IndexOf("</B>");
            var headerText = html.Substring(headerStartIndex, headerEndIndex - headerStartIndex);
            return headerText.Trim();
        }

        private string FormatHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
                return "";

            return "\t<h3 id=\"section-title\">" + header + "</h3>\r\n";
        }

        private string GetBody(string html)
        {
            var headerEndIndex = html.IndexOf("</B>");
            var bodyStartIndex = headerEndIndex + 4;
            var bodyEndIndex = html.IndexOf("<HISTORY>");
            var bodyText = html.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex);
            bodyText = bodyText.Replace("<BR>", "");
            bodyText = bodyText.Replace("<P>", "</P><P>");
            bodyText = bodyText.Replace("<P>\r\n", "\r\n<P>");
            return bodyText.Trim();
        }

        private string FormatBody(string body)
        {
            if (string.IsNullOrEmpty(body))
                return "";

            return "\t<div id=\"section-body\"><P>" + body + "</div>\r\n";
        }

        private string GetFooter(string html)
        {
            var footerStartIndex = html.IndexOf("<HISTORY>") + 9;
            var footerEndIndex = html.IndexOf("</HISTORY>");
            var footerText = html.Substring(footerStartIndex, footerEndIndex - footerStartIndex);
            footerText = footerText.Replace("<BR>", "").Replace("History of Section.", "");
            return footerText.Trim();
        }

        private string FormatFooter(string footer)
        {
            if (string.IsNullOrEmpty(footer))
                return "";

            var historyList = new StringBuilder();
            var historyEntries = footer.Replace("(", "").Replace(")", "").Replace("&sect;", "[SECT_SYMBOL]").Split(';');

            if(historyEntries.Count() > 0)
            {
                historyList.Append("\r\n\t\t<ul>\r\n");
                foreach (var entry in historyEntries)
                {
                    historyList.Append("\t\t\t<li>");
                    historyList.Append(entry.Replace("[SECT_SYMBOL]", "&sect;"));
                    historyList.Append("</li>\r\n");
                }
                historyList.Append("\t\t</ul>\r\n");
            }

            return "\t<div id=\"section-history\"><h4>History of Section</h4>" + historyList.ToString() + "\t</div>\r\n";
        }
    }
}
