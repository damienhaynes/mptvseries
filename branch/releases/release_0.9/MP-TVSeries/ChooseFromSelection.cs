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

            if (descriptor.m_sbtnOKLabel == String.Empty)
                button_OK.Visible = false;
            else
            {
                button_OK.Visible = true;
                button_OK.Text = descriptor.m_sbtnOKLabel;
            }

            if (descriptor.m_sbtnCancelLabel == String.Empty)
                button_Cancel.Visible = false;
            else
            {
                button_Cancel.Visible = true;
                button_Cancel.Text = descriptor.m_sbtnCancelLabel;
            }

            if (descriptor.m_sbtnIgnoreLabel == String.Empty)
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