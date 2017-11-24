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
        public string LinkID
        {
            get
            {
                var cleaned = TitleNumber + "_" + TitleName.Trim().Replace(" ", "_").Replace("__", "_");
                if (cleaned.Length > 41)
                    return cleaned.Substring(0, 40);
                return cleaned;
            }
        }

        public List<Chapter> Chapters { get; set; }

        public Title()
        {
            Chapters = new List<Chapter>();
            TitleNumber = "";
            TitleName = "";
        }
    }
}
