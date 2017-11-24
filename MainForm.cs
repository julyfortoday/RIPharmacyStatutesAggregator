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
        StyleSheetProvider styleSheetProvider;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var styleSheet = File.ReadAllText("style.css") ?? "";
            styleSheetProvider = new StyleSheetProvider(styleSheet);

            var addresses = Properties.Settings.Default.Addresses;
            pageUrls = addresses.Cast<string>().ToList();
            PrintAddressesToListBox();


            fetcher = new PageFetcher(pageUrls,
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            fetcher.OverwriteSavedPages = true;

            PrintLastDownloaded();

            if(pages == null)
            {
                saveButton.Enabled = false;
            }

            // DEBUG
            saveButton.Enabled = true;
            fetcher.OverwriteSavedPages = false;
            pages = fetcher.GetPages();
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            var originalText = downloadButton.Text;
            downloadButton.Text = "Downloading... (this may take a moment)";
            saveButton.Enabled = false;

            pages = fetcher.GetPages();

            Properties.Settings.Default.LastDownloaded = DateTime.Now;
            PrintLastDownloaded();

            downloadButton.Text = originalText;
            saveButton.Enabled = true;
        }

        private void PrintLastDownloaded()
        {
            var label = "Last Downloaded: ";
            var output = Properties.Settings.Default.LastDownloaded.ToString();
            var result = DateTime.Compare(Properties.Settings.Default.LastDownloaded, new DateTime(2000, 1, 1));
            bool never = result <= 0;

            if(never)
            {
                output = "Never";
            }
            OutputLabel_LastDownloaded.Text = label + output;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (pages == null)
                return;
            var aggregator = new PageAggregator(styleSheetProvider);
            var aggregatedPage = aggregator.Aggregate(pages);

            var dir = Properties.Settings.Default.SaveLocation;
            if (!string.IsNullOrEmpty(dir))
            {
                dir = Path.GetDirectoryName(dir);
                if (Directory.Exists(dir))
                    saveFileDialog.InitialDirectory = dir;
            }

            saveFileDialog.FileName = "RI_Pharmacy_Statutes_" + DateTime.Now.ToString("MM_dd_yyyy") +".html";
            saveFileDialog.Title = "Save File";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, aggregatedPage);
                Properties.Settings.Default.SaveLocation = saveFileDialog.FileName;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void PrintAddressesToListBox()
        {
            foreach(var address in pageUrls)
            {
                listBox1.Items.Add(address);
            }
        }
    }
}
