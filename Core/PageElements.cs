using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core
{
    public class PageElements
    {
        public string TitleNumber { get; set; }
        public string TitleName { get; set; }

        public string ChapterNumber { get; set; }
        public string ChapterName { get; set; }

        public string ArticleNumber { get; set; }
        public string ArticleName { get; set; }

        public string SectionTitle { get; set; }

        public List<Tuple<string,string>> SectionContents { get; set; }

        public string HistoryHeader { get; set; }
        public string HistoryList { get; set; }
    }
}
