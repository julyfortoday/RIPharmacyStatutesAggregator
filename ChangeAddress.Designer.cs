namespace RIPharmStatutesAggregator
{
    partial class ChangeAddress
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addressInput = new System.Windows.Forms.TextBox();
            this.saveAddress = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // addressInput
            // 
            this.addressInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addressInput.Location = new System.Drawing.Point(13, 13);
            this.addressInput.Name = "addressInput";
            this.addressInput.Size = new System.Drawing.Size(598, 20);
            this.addressInput.TabIndex = 0;
            // 
            // saveAddress
            // 
            this.saveAddress.Location = new System.Drawing.Point(617, 11);
            this.saveAddress.Name = "saveAddress";
            this.saveAddress.Size = new System.Drawing.Size(75, 23);
            this.saveAddress.TabIndex = 1;
            this.saveAddress.Text = "Save";
            this.saveAddress.UseVisualStyleBackColor = true;
            this.saveAddress.Click += new System.EventHandler(this.saveAddress_Click);
            // 
            // ChangeAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 44);
            this.Controls.Add(this.saveAddress);
            this.Controls.Add(this.addressInput);
            this.Name = "ChangeAddress";
            this.Text = "Save Address";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox addressInput;
        private System.Windows.Forms.Button saveAddress;
    }
}