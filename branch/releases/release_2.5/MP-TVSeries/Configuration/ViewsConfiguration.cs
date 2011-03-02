#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2009
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

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
        public bool Edit { get; set; }

        public List<DBSeries> SeriesToAdd {
            get {
                return _seriesToAdd;
            }
            set {
                _seriesToAdd = value;
            }
        } private List<DBSeries> _seriesToAdd = null;

        public List<DBSeries> SeriesToRemove {
            get {
                return _seriesToRemove;
            }
            set {
                _seriesToRemove = value;
            }
        } private List<DBSeries> _seriesToRemove = null;

		public ViewsConfiguration() {
            InitializeComponent();			
        }

		protected override void OnLoad(EventArgs e) {
            txtViewName.Text = ViewName;
            radioAdvanced.Checked = (Type == ViewType.ADVANCED);

			if (ViewName.Length == 0 && Type == ViewType.SIMPLE)
				buttonSeriesSelect.Enabled = false;

            if (Edit) {
                // we can edit the name outside in the main form                
                txtViewName.Enabled = false;                
            }

			base.OnLoad(e);
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
			if (ViewName.Length > 0) buttonSeriesSelect.Enabled = true;
		}

		private void radioAdvanced_Click(object sender, EventArgs e) {
			Type = ViewType.ADVANCED;
			groupBoxAdvanced.Enabled = true;
			buttonSeriesSelect.Enabled = false;
		}

		private void buttonSeriesSelect_Click(object sender, EventArgs e) {												
			SeriesSelect SeriesSelectDlg = new SeriesSelect();
			
            // Set Current View
            SeriesSelectDlg.ViewTag = "|" + txtViewName.Text + "|"; ;

            // Show series list dialog
			DialogResult result = SeriesSelectDlg.ShowDialog(this);

            if (result == DialogResult.OK) {
                SeriesToAdd = SeriesSelectDlg.CheckedItems;
                SeriesToRemove = SeriesSelectDlg.UnCheckedItems;
            }            
		}

		private void txtViewName_TextChanged(object sender, EventArgs e) {
			if (txtViewName.Text.Length > 0)
				buttonSeriesSelect.Enabled = true;
			else
				buttonSeriesSelect.Enabled = false;

            // Clear Series List so we are forced to re-query
            SeriesToAdd = null;
            SeriesToRemove = null;
		}
    }
}
