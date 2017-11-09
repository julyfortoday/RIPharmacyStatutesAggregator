using RIPharmStatutesAggregator.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RIPharmStatutesAggregator
{
    public partial class MainForm : Form
    {
        List<string> pageUrls;
        List<Core.Page> pages;
        PageFetcher fetcher;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pageUrls = new List<string>();
            pageUrls.Add("http://webserver.rilin.state.ri.us/Statutes/TITLE5/5-19.1/INDEX.HTM");
            pageUrls.Add("http://webserver.rilin.state.ri.us/Statutes/TITLE5/5-19.2/INDEX.HTM");
            pageUrls.Add("http://webserver.rilin.state.ri.us/Statutes/TITLE21/21-28/INDEX.HTM");
            pageUrls.Add("http://webserver.rilin.state.ri.us/Statutes/TITLE21/21-31/INDEX.HTM");

            fetcher = new PageFetcher(pageUrls,
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            pages = fetcher.GetPages();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (pages == null)
                return;
            var aggregator = new PageAggregator();
            var aggregatedPage = aggregator.Aggregate(pages);

            File.WriteAllText("RI_Pharm_Statutes.html",aggregatedPage);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            fetcher.OverwriteSavedPages = checkBox1.Checked;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
