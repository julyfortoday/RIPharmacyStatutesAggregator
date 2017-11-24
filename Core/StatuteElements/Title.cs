using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core.StatuteElements
{
    public class Title
    {
        public string TitleNumber { get; set; }
        public string TitleName { get; set; }
        public List<Chapter> Chapters { get; set; }

        public Title()
        {
            Chapters = new List<Chapter>();
        }
    }
}
