using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core
{
    public class Page
    {
        public string Address { get; set; }
        public string Path { get; set; }
        public string Html { get; set; }
        public List<string> Links { get; set; }
        public Page()
        {
            Links = new List<string>();
        }
    }
}
