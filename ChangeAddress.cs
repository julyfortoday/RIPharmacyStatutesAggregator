using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RIPharmStatutesAggregator
{
    public partial class ChangeAddress : Form
    {
        public string Address { get; set; }

        public ChangeAddress(string address)
        {
            InitializeComponent();
            addressInput.Text = address;
            this.DialogResult = DialogResult.Cancel;
        }

        private void saveAddress_Click(object sender, EventArgs e)
        {
            this.Address = addressInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
