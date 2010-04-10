using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.Configuration {
	public partial class SeriesSelect : Form {

        public List<DBSeries> m_SeriesChecked = new List<DBSeries>();
        public List<DBSeries> m_SeriesUnChecked = new List<DBSeries>();

		public List<DBSeries> CheckedItems {
			get {
				return _checkedItems;
			}
			set {
				_checkedItems = value;
			}
		} private List<DBSeries> _checkedItems = new List<DBSeries>();

		public List<DBSeries> UnCheckedItems {
			get {
				return _uncheckedItems;
			}
			set {
				_uncheckedItems = value;
			}
		} private List<DBSeries> _uncheckedItems = new List<DBSeries>();

        public string ViewTag { get; set; }
        public int CheckedCount { get; set; }

		public SeriesSelect() {
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e) {            
            // Get list of series in view
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cViewTags, ViewTag, SQLConditionType.Like);
            m_SeriesChecked = DBSeries.Get(conditions);
            
            // Get list of series not in view                    
            conditions = new SQLCondition();
            conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cViewTags, ViewTag, SQLConditionType.NotLike);
            m_SeriesUnChecked = DBSeries.Get(conditions);
        
			// Populate series list, if view already 
			// has series added mark as checked at top of list
            foreach (DBSeries series in m_SeriesChecked) {
				checkedListBoxSeries.Items.Add(series, true);
			}

            foreach (DBSeries series in m_SeriesUnChecked) {				
				checkedListBoxSeries.Items.Add(series, false);				
			}

            CheckedCount = m_SeriesChecked.Count;
            labelSeriesSelected.Text = CheckedCount.ToString() + " Series Selected";

			base.OnLoad(e);
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		private void checkedListBoxSeries_ItemCheck(object sender, ItemCheckEventArgs e) {
			DBSeries item = (DBSeries)checkedListBoxSeries.SelectedItem;

            if (item == null) return;

            int index = checkedListBoxSeries.SelectedIndex;    

            // Add/Remove items from list
            if (item != null) {                
                // Item state before item was clicked 
                if (checkedListBoxSeries.GetItemChecked(index)) {

                    // Store items changes
                    if (!UnCheckedItems.Contains(item)) {
                        UnCheckedItems.Add(item);
                    }
                    if (CheckedItems.Contains(item)) {
                        CheckedItems.Remove(item);
                    }
                                        
                    CheckedCount -= 1;
                    labelSeriesSelected.Text = (CheckedCount).ToString() + " Series Selected";
                }
                else {
                    // Store items changes
                    if (!CheckedItems.Contains(item)) {
                        CheckedItems.Add(item);
                    }
                    if (UnCheckedItems.Contains(item)) {
                        UnCheckedItems.Remove(item);
                    }
                    
                    CheckedCount += 1;
                    labelSeriesSelected.Text = (CheckedCount).ToString() + " Series Selected";
                }
            }
				
		}
	}
}
