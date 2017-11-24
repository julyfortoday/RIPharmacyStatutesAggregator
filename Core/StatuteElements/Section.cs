﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core.StatuteElements
{
    public class Section
    {
        public string SectionNumber { get; set; }
        public string SectionName { get; set; }
        public string SafeName { get
            {
                var cleaned = SectionNumber + "_" + SectionName.Trim().Replace(" ", "_").Replace("__", "_");
                if(cleaned.Length > 41)
                    return cleaned.Substring(0, 40);
                return cleaned;
            } }
        public List<Line> Lines { get; set; }

        public string HistoryHeader { get; set; }
        public string HistoryList { get; set; }

        public Section()
        {
            Lines = new List<Line>();
        }
    }
}
