using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core.StatuteElements
{
    public class Article
    {
        public string ArticleNumber { get; set; }
        public string ArticleName { get; set; }
        public List<Section> Sections { get; set; }

        public Article()
        {
            Sections = new List<Section>();
        }
    }
}
