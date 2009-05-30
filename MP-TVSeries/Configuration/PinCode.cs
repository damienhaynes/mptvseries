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
        }

        private void okButton_Click(object sender, EventArgs e) {            			
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

		private void pincodeTextBox_KeyPress(object sender, KeyPressEventArgs e) {
			// Only allow numbers
			if ((!Char.IsDigit(e.KeyChar)) && (e.KeyChar != Convert.ToChar(Keys.Back)))
				e.Handled = true;
		}

		private void EnableOKButton() {
			if (Pin.Length < 4)
				okButton.Enabled = false;
			else
				okButton.Enabled = true;
		}

		private void pincodeTextBox_TextChanged(object sender, EventArgs e) {
			Pin = pincodeTextBox.Text;
			EnableOKButton();
		}

		private void PinCode_Load(object sender, EventArgs e) {
			if (Owner == null)
				return;

			pincodeTextBox.Text = Pin;
			EnableOKButton();
		}

    }
}
