using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core
{
    public class Tag
    {
        public string TagName {get; private set;}
        public string Start { get { return "<" + TagName.ToUpper() + ">"; } }
        public string End { get { return "</" + TagName.ToUpper() + ">"; } }

        public Tag(string name)
        {
            TagName = name;
        }
    }
}
