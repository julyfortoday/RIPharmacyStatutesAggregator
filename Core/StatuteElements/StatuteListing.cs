using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core.StatuteElements
{
    public class StatuteListing
    {
        public List<Title> Titles { get; set; }

        public StatuteListing()
        {
            Titles = new List<Title>();
        }
    }
}
