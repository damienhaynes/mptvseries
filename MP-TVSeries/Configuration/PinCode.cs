using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration {
    public partial class PinCode : Form {

        public string Pin {
            get { return _pin; }
            set { _pin = value; }
        } string _pin = string.Empty;
        
        public PinCode() {
            InitializeComponent();

            pincodeTextBox.Text = Pin;
        }

        private void okButton_Click(object sender, EventArgs e) {
            Pin = pincodeTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

    }
}
