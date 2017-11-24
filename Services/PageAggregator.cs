using RIPharmStatutesAggregator.Core;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace RIPharmStatutesAggregator.Services
{
    public class PageAggregator
    {
        private ListingFormatter formatter;
        public PageAggregator(StyleSheetProvider styleSheetProvider)
        {
            formatter = new ListingFormatter(styleSheetProvider);
        }

        //<a href="#Section1.2">Section 1.2</a>
        //<a name="Section1.2"></a> 

        internal string Aggregate(List<Page> pages)
        {
            var listing = ListingBuilder.BuildListing(pages);
            var formatted = formatter.Format(listing);
            return formatted;
        }

        //internal string Aggregate_Old(List<Page> pages)
        //{
        //    var indexPages = pages.Where(x => x.Path.Contains("INDEX.HTM"));

        //    var aggregatedPage = new StringBuilder();
        //    foreach (var indexPage in indexPages)
        //    {
        //        var chapter = FormatChapter(indexPage, pages);
        //        aggregatedPage.Append(chapter);
        //    }

        //    var pageIndex = CreatePageIndex(pages);

        //    var html = AddTopLevelHTML(pageIndex, aggregatedPage.ToString());
        //    return html;
        //}

        //private string CreatePageIndex(List<Page> pages)
        //{
        //    var indexPages = pages.Where(x => x.Path.Contains("INDEX.HTM"));
        //    var pageIndex = new StringBuilder();
        //    pageIndex.AppendLine("<ul>");
        //    foreach (var indexPage in indexPages)
        //    {
        //        var chapterName = Path.GetFileName(indexPage.Path).Replace("_INDEX.HTM", "");
        //        var subPages = pages.Where(x => x.Path.Contains(chapterName)).ToList();
        //        subPages.Remove(indexPage);
        //        var first = subPages.FirstOrDefault();

        //        var chapTitle = GetChapterTitle(first.Html, false);
        //        var cleaned = chapTitle.Replace("<H1>", "").Replace("</H1>", "").Replace("<BR>", " ").Replace("\r\n", "");

        //        pageIndex.AppendLine("<li><a href=\"#"+ chapTitle.Replace(" ", "_") + "\">"+ cleaned + "</a></li>");
        //    }
        //    pageIndex.AppendLine("</ul>");
        //    return pageIndex.ToString();
        //}
    }
}
