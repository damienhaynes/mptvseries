#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
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

namespace WindowPlugins.GUITVSeries
{
    public partial class ChooseFromSelectionDialog : Form
    {
        Feedback.CItem SelectedItemRadOption = null;
        public String m_sTypedText = String.Empty;
        private Feedback.CDescriptor m_descriptor = null;
        string origTitle = string.Empty;
        bool useRadioMode = false;
        const int maxItemsForRadioMode = 6; // doesn't scale to above 6 items

        public ChooseFromSelectionDialog(Feedback.CDescriptor descriptor)
        {
            m_descriptor = descriptor;
            InitializeComponent();
            useRadioMode = useRadioMethod(descriptor);
            Text = descriptor.m_sTitle;
            label_ToMatch.Text = descriptor.m_sItemToMatchLabel;
            textbox_ToMatch.Text = descriptor.m_sItemToMatch;
            textbox_ToMatch.Enabled = descriptor.m_allowAlter;
            label_Choices.Text = descriptor.m_sListLabel;
            origTitle = this.Text;


            if (descriptor.m_sbtnOKLabel.Length == 0)
                button_OK.Visible = false;
            else
            {
                button_OK.Visible = true;
                button_OK.Text = descriptor.m_sbtnOKLabel;
            }

            if (descriptor.m_sbtnCancelLabel.Length == 0)
                button_Cancel.Visible = false;
            else
            {
                button_Cancel.Visible = true;
                button_Cancel.Text = descriptor.m_sbtnCancelLabel;
            }

            if (descriptor.m_sbtnIgnoreLabel.Length == 0)
                button_Ignore.Visible = false;
            else
            {
                button_Ignore.Visible = true;
                button_Ignore.Text = descriptor.m_sbtnIgnoreLabel;
            }

            if (!useRadioMode)
            {
                setRadiosVisibility(false);
                listbox_Choices.Visible = true;
                foreach (Feedback.CItem item in descriptor.m_List)
                {
                    listbox_Choices.Items.Add(item);
                }
                if (listbox_Choices.Items.Count > 0)
                    listbox_Choices.SelectedIndex = 0;
            }
            else
            {
                setRadiosVisibility(true);
                listbox_Choices.Visible = false;

                if (descriptor.m_List.Count > 0)
                {
                    radOption1.Text = descriptor.m_List[0].ToString();
                    radOption1.Checked = true; // default
                }
                else radOption1.Visible = false;
                if (descriptor.m_List.Count > 1) radOption2.Text = descriptor.m_List[1].ToString();
                else radOption2.Visible = false;
                if (descriptor.m_List.Count > 2) radOption3.Text = descriptor.m_List[2].ToString();
                else radOption3.Visible = false;
                if (descriptor.m_List.Count > 3) radOption4.Text = descriptor.m_List[3].ToString();
                else radOption4.Visible = false;
                if (descriptor.m_List.Count > 4) radOption5.Text = descriptor.m_List[4].ToString();
                else radOption5.Visible = false;
                if (descriptor.m_List.Count > 5) radOption6.Text = descriptor.m_List[5].ToString();
                else radOption6.Visible = false;
            }
        }

        public Feedback.CItem SelectedItem
        {
            get 
            {
                if (useRadioMode) return SelectedItemRadOption;
                Feedback.CItem selectedItem =  listbox_Choices.SelectedItem as Feedback.CItem;
                return new Feedback.CItem(m_sTypedText, String.Empty, selectedItem.m_Tag);
            }
        }

        private void listbox_Choices_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Comment by Inker:
            // ahmm, this change would have needed communication and TESTING
            // the thing is you now can't see anymore what its searching for, not even before you select something
            // and another thing resulted from this....(chosen series was not accepted and instead always the first!!)
            // please be careful with changes that change what is returned from the interfaces!

            Feedback.CItem item = listbox_Choices.SelectedItem as Feedback.CItem;
            if (item != null)
            {
                if (item.m_sDescription != "")
                {
                    this.Text = origTitle + " (" + item.m_sName + ")";
                    this.textbox_Description.Text = item.m_sDescription;
                }
                button_OK.Text = m_descriptor.m_sbtnOKLabel;
            }
        }

        private void textbox_ToMatch_TextChanged(object sender, EventArgs e)
        {
            m_sTypedText = textbox_ToMatch.Text;
            button_OK.Text = m_descriptor.m_sbtnOKLabelAlternate;
        }

        private void listbox_Choices_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Return OK for Item Double Clicked
            Feedback.CItem item = listbox_Choices.SelectedItem as Feedback.CItem;
            if (item != null && item.m_sDescription != "")
                this.DialogResult = DialogResult.OK;       
        }

        private bool useRadioMethod(Feedback.CDescriptor descriptor)
        {
            return descriptor.m_useRadioToSelect && descriptor.m_List.Count <= maxItemsForRadioMode;
        }

        private void setRadiosVisibility(bool visible)
        {
            radOption1.Visible = visible;
            radOption2.Visible = visible;
            radOption3.Visible = visible;
            radOption4.Visible = visible;
            radOption5.Visible = visible;
            radOption6.Visible = visible;
        }

        private void radOptionSelected(int selectedOption)
        {
            if (selectedOption > m_descriptor.m_List.Count)
            {
                MPTVSeriesLog.Write("Selected an unavailable Option....");
                return;
            }
            Feedback.CItem item = m_descriptor.m_List[selectedOption];
            if (item != null)
            {
                if (item.m_sDescription != "")
                {
                    this.Text = origTitle + " (" + item.m_sName + ")";
                    this.textbox_Description.Text = item.m_sDescription;
                }
                SelectedItemRadOption = item;
                button_OK.Text = m_descriptor.m_sbtnOKLabel;
            }
        }

        private void radOption1_CheckedChanged(object sender, EventArgs e)
        {
            radOptionSelected(0);
        }

        private void radOption2_CheckedChanged(object sender, EventArgs e)
        {
            radOptionSelected(1);
        }

        private void radOption3_CheckedChanged(object sender, EventArgs e)
        {
            radOptionSelected(2);
        }

        private void radOption4_CheckedChanged(object sender, EventArgs e)
        {
            radOptionSelected(3);
        }

        private void radOption5_CheckedChanged(object sender, EventArgs e)
        {
            radOptionSelected(4);
        }

        private void radOption6_CheckedChanged(object sender, EventArgs e)
        {
            radOptionSelected(5);
        }

    }
}