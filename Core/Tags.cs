using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Core
{
    public static class Tags
    {
        public class Tag
        {
            public string TagName { get; private set; }
            public string Start { get { return "<" + TagName.ToUpper() + ">"; } }
            public string End { get { return "</" + TagName.ToUpper() + ">"; } }

            public Tag(string name)
            {
                TagName = name;
            }
        }

        // HTML
        public static Tag html = new Tag("HTML");
        public static Tag head = new Tag("HEAD");
        public static Tag style = new Tag("STYLE");
        public static Tag Body = new Tag("BODY");
        public static Tag Bold = new Tag("B");
        public static Tag Paragraph = new Tag("P");
        public static Tag Italic = new Tag("I");
        public static Tag Break = new Tag("BR");
        public static Tag H1 = new Tag("H1");
        public static Tag H2 = new Tag("H2");
        public static Tag H3 = new Tag("H3");
        public static Tag H4 = new Tag("H4");
        public static Tag H5 = new Tag("H5");
        public static Tag List = new Tag("UL");
        public static Tag ListItem = new Tag("LI");
        public static Tag Div = new Tag("DIV");

        // CUSTOM TAGS
        public static Tag HistoryTag = new Tag("HISTORY");
        public static Tag CenterTag = new Tag("CENTER");

        public static string MakeParagraph(string contents = "")
        {
            return Paragraph.Start + contents + Paragraph.End;
        }


        public static string OpenDivWithId(string id = "")
        {
            return "<DIV id=\"" + id + "\">";
        }
        public static string OpenDivWithClass(string cssClass = "")
        {
            return "<DIV class=\"" + cssClass + "\">";
        }
        public static string MakeDiv(string text = "", string id = "")
        {
            return "<DIV id=\"" + id + "\">" + text + "</DIV>";
        }

        //<a href="#Section1.2">Section 1.2</a>
        //<a name="Section1.2"></a> 

        public static string AnchorLink(string name, string text)
        {
            return "<A href=\"#" + name + "\">" + text + "</A>";
        }

        public static string Anchor(string name = "")
        {
            return "<A name=\"" + name + "\"></A>";
        }

        public static string MakeLink(string href, string text, string cssClass)
        {
            return "<A class=\"" + cssClass + "\" href=\"" + href + "\">" + text + "</A>";
        }
    }
}
