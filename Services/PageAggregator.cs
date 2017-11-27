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

        internal string Aggregate(List<Page> pages)
        {
            var listing = ListingBuilder.BuildListing(pages);
            var formatted = formatter.Format(listing);
            return formatted;
        }
    }
}
