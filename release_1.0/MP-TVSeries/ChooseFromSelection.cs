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
        public String m_sTypedText = String.Empty;
        private Feedback.CDescriptor m_descriptor = null;

        public ChooseFromSelectionDialog(Feedback.CDescriptor descriptor)
        {
            m_descriptor = descriptor;
            InitializeComponent();
            Text = descriptor.m_sTitle;
            label_ToMatch.Text = descriptor.m_sItemToMatchLabel;
            textbox_ToMatch.Text = descriptor.m_sItemToMatch;
            label_Choices.Text = descriptor.m_sListLabel;

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

            foreach (Feedback.CItem item in descriptor.m_List)
            {
                listbox_Choices.Items.Add(item);
            }
            if (listbox_Choices.Items.Count > 0)
                listbox_Choices.SelectedIndex = 0;
        }

        public Feedback.CItem SelectedItem
        {
            get 
            { 
                Feedback.CItem selectedItem =  listbox_Choices.SelectedItem as Feedback.CItem;
                return new Feedback.CItem(m_sTypedText, String.Empty, selectedItem.m_Tag);
            }
        }

        private void listbox_Choices_SelectedIndexChanged(object sender, EventArgs e)
        {
            Feedback.CItem item = listbox_Choices.SelectedItem as Feedback.CItem;
            if (item != null)
            {
                textbox_Description.Text = item.m_sDescription;
                button_OK.Text = m_descriptor.m_sbtnOKLabel;
            }
        }

        private void textbox_ToMatch_TextChanged(object sender, EventArgs e)
        {
            m_sTypedText = textbox_ToMatch.Text;
            button_OK.Text = m_descriptor.m_sbtnOKLabelAlternate;
        }
    }
}