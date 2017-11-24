using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core.StatuteElements
{
    public class Chapter
    {
        public string ChapterNumber { get; set; }
        public string ChapterName { get; set; }
        public List<Article> Articles { get; set; }
        public List<Section> Sections { get; set; }

        public Chapter()
        {
            Articles = new List<Article>();
            Sections = new List<Section>();
        }
    }
}
