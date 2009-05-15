using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration {
    public partial class ViewsConfiguration : Form {

		public enum ViewType {
			SIMPLE,
			ADVANCED
		}

		public string ViewName { get; set; }
		public ViewType Type { get; set; }

		public ViewsConfiguration() {
            InitializeComponent();

			txtViewName.Text = ViewName;
			radioAdvanced.Checked = (Type == ViewType.ADVANCED);
        }

		private void btnOK_Click(object sender, EventArgs e) {
			ViewName = txtViewName.Text;

			if (ViewName.Length > 0) {
				DialogResult = DialogResult.OK;
				Close();
			}
			else {
				MessageBox.Show("You must enter a valid name for this view", "Views", MessageBoxButtons.OK,MessageBoxIcon.Information);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void radioSimple_Click(object sender, EventArgs e) {
			Type = ViewType.SIMPLE;
			groupBoxAdvanced.Enabled = false;
		}

		private void radioAdvanced_Click(object sender, EventArgs e) {
			Type = ViewType.ADVANCED;
			groupBoxAdvanced.Enabled = true;
		}
    }
}
