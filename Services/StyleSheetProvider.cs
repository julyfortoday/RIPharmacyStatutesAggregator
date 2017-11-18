using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Services
{
    public class StyleSheetProvider
    {
        private string styleSheet { get; set; }

        public StyleSheetProvider(string styleSheet)
        {
            this.styleSheet = styleSheet;
        }

        public string GetStyleSheet()
        {
            return styleSheet;
        }
    }
}
