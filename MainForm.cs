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
        List<Core.Page> pages;
        PageFetcher fetcher;
        StyleSheetProvider styleSheetProvider;
        string AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var styleSheet = "";
            if (File.Exists("style.css"))
                styleSheet = File.ReadAllText("style.css");

            styleSheetProvider = new StyleSheetProvider(styleSheet);

            var addresses = Properties.Settings.Default.Addresses;
            List<string> urls = addresses.Cast<string>().ToList();
            PrintAddressesToListBox(urls);

            fetcher = new PageFetcher(GetAddressesFromListBox(), AppPath);
            fetcher.OverwriteSavedPages = true;
            fetcher.ClearOldFiles = true;

            PrintLastDownloaded();

            if(pages == null)
            {
                saveButton.Enabled = false;
            }

            // DEBUG
            //saveButton.Enabled = true;
            //fetcher.OverwriteSavedPages = false;
            //pages = fetcher.GetPages();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveAppSettings();
        }

        private void SaveAppSettings()
        {
            var sCol = new System.Collections.Specialized.StringCollection();
            var addresses = GetAddressesFromListBox();
            foreach (var address in addresses)
            {
                sCol.Add(address);
            }
            Properties.Settings.Default.Addresses = sCol;
            Properties.Settings.Default.Save();
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            var originalText = downloadButton.Text;
            downloadButton.Text = "Downloading... (this may take a moment)";
            saveButton.Enabled = false;

            fetcher.AddPageUrls(GetAddressesFromListBox());
            pages = fetcher.GetPages();

            Properties.Settings.Default.LastDownloaded = DateTime.Now;
            PrintLastDownloaded();

            downloadButton.Text = originalText;
            saveButton.Enabled = true;
        }

        private void PrintLastDownloaded()
        {
            var label = "Last Downloaded: ";
            var output = "Never";
            var lastDownloaded = Properties.Settings.Default.LastDownloaded;
            if (lastDownloaded != null)
            {
                var result = DateTime.Compare(lastDownloaded, new DateTime(2000, 1, 1));
                var never = result <= 0;
                if(!never)
                    output = lastDownloaded.ToString();
            }
            OutputLabel_LastDownloaded.Text = label + output;
        }

        private void PrintAddressesToListBox(List<string> addresses)
        {
            foreach (var address in addresses)
            {
                addressListBox.Items.Add(address);
            }
        }

        private List<string> GetAddressesFromListBox()
        {
            List<string> addresses = new List<string>();
            foreach (var item in addressListBox.Items)
            {
                addresses.Add(item.ToString());
            }
            return addresses;
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

        private void addButton_Click(object sender, EventArgs e)
        {
            using (var form = new ChangeAddress(""))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string newAddress = form.Address;
                    addressListBox.Items.Add(newAddress);
                }
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            var selected = addressListBox.SelectedIndex;
            addressListBox.Items.RemoveAt(selected);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var selected = addressListBox.SelectedIndex;
            if (selected < 0)
                return;
            var address = addressListBox.Items[selected].ToString();

            using (var form = new ChangeAddress(address))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string newAddress = form.Address;
                    addressListBox.Items[selected] = newAddress;
                }
            }

        }
    }
}
