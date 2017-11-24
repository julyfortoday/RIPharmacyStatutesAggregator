using RIPharmStatutesAggregator.Core;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RIPharmStatutesAggregator.Services
{
    public class PageFetcher
    {
        public bool OverwriteSavedPages {get; set;}

        List<string> pageUrls;
        string dataPath;

        public PageFetcher(List<string> pageUrls, string appPath)
        {
            this.pageUrls = pageUrls;
            this.dataPath = Path.Combine(appPath,"Data");
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
        }

        public List<Page> GetPages()
        {
            var pages = new List<Page>();
            AddPagesFromUrlList(pageUrls, pages);

            GetDataForPages(pages);
            ParseAllPagesForLinks(pages);
            AddPagesFromLinks(pages);

            GetDataForPages(pages);
            return pages;
        }

        private void AddPagesFromUrlList(List<string> pageUrls, List<Page> pages)
        {
            foreach (var address in pageUrls)
                pages.Add(new Page() { Address = address});
        }

        public void GetDataForPages(List<Page> pages)
        {
            foreach (var page in pages)
            {
                page.Path = ConvertAddressToPath(page.Address);
                if (File.Exists(page.Path) && (!OverwriteSavedPages))
                {
                    page.Html = File.ReadAllText(page.Path);
                }
                else
                {
                    page.Html = GetHtmlForAddress(page.Address);
                    File.WriteAllText(page.Path, page.Html);
                }
            }
        }

        public string ConvertAddressToPath(string Address)
        {
            var fileName = Address.ToUpper()
                .Replace("HTTP://", "")
                .Replace("WEBSERVER.RILIN.STATE.RI.US", "")
                .Replace("/STATUTES/", "")
                .Replace("/", "_");
            return Path.Combine(dataPath, fileName);
        }

        private void ParseAllPagesForLinks(List<Page> pages)
        {
            foreach(var page in pages)
                page.Links = ParsePageForLinks(page);
        }

        private List<string> ParsePageForLinks(Page page)
        {
            var subpages = new List<string>();
            var baseUrl = page.Address.ToUpper().Replace("INDEX.HTM", "");

            const string linkStartPattern = "<A HREF=\"";
            const string wildCardPattern = ".*";
            const string linkEndPattern = "\">";
            const string linkFullPattern = linkStartPattern + wildCardPattern + linkEndPattern;

            // find all links in the index page
            MatchCollection matches = Regex.Matches(page.Html, linkFullPattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                var pageLink = match.Value.Replace(linkStartPattern,"").Replace(linkEndPattern,"");
                var fullLink = baseUrl + pageLink;
                subpages.Add(fullLink);
            }

            return subpages;
        }

        private void AddPagesFromLinks(List<Page> pages)
        {
            var currentPages = pages.GetRange(0, pages.Count);
            foreach (var page in currentPages)
            {
                foreach(var link in page.Links)
                {
                    pages.Add(new Page() { Address = link });
                }
            }
        }

        public string GetHtmlForAddress(string address)
        {
            string html;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                string s = webClient.DownloadString(address);
                html = s;
            }
            return html;
        }
    }
}
