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
//
// Copyright 2007 Twin Rose Software
//
// You are free to use this code and form in any way that you like, 
// personally or professionally.  If you use it in a published application,
// we ask that you link to us from your website to:
// http://www.twinrose.net/
//
// Any redistrubition of this code must contain this entire comment text.
//
// We would also appreciate a thank you!
//  Chris Johanson
//  twinrose@twinrose.net
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public enum MemoryBoxResult { Yes, YesToAll, No, NoToAll }

    public partial class MemoryBox : Form
    {
        // Internal values
        MemoryBoxResult lastResult = MemoryBoxResult.No;
        MemoryBoxResult result = MemoryBoxResult.No;
        
        // Enums
        // Results

        /// <summary>
        /// The default constructor for MemoryBox.
        /// </summary>
        public MemoryBox()
        {
            InitializeComponent();
        }

        #region Properties
        public String LabelText
        {
            get { return this.labelBody.Text; }
            set
            {
                this.labelBody.Text = value;
                UpdateSize();
            }
        }

        public MemoryBoxResult Result
        {
            get { return this.result; }
            set { this.result = value; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Call this function instead of ShowDialog, to check for remembered
        /// result.
        /// </summary>
        /// <returns></returns>
        public MemoryBoxResult ShowMemoryDialog()
        {
            result = MemoryBoxResult.No;
            if (lastResult == MemoryBoxResult.NoToAll)
            {
                result = MemoryBoxResult.No;
            }
            else if (lastResult == MemoryBoxResult.YesToAll)
            {
                result = MemoryBoxResult.Yes;
            }
            else
            {
                base.ShowDialog();
            }
            return result;
        }

        public MemoryBoxResult ShowMemoryDialog(String label, string title)
        {
            this.Text = title;
            LabelText = label;
            return ShowMemoryDialog();
        }
		
		public void DisableButton(MemoryBoxResult button, bool state)
		{		
			if (button == MemoryBoxResult.NoToAll)
				this.buttonNotoAll.Enabled = state;
			if (button == MemoryBoxResult.YesToAll)
				this.buttonYestoAll.Enabled = state; 	
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// This call updates the size of the window based on certain factors,
        /// such as if an icon is present, and the size of label.
        /// </summary>
        private void UpdateSize()
        {
            int newWidth = labelBody.Size.Width + 40;

            // Add the width of the icon, and some padding.
            if (pictureBoxIcon.Image != null)
            {
                newWidth += pictureBoxIcon.Width + 20;
                labelBody.Location = new Point(118, labelBody.Location.Y);
            }
            else
            {
                labelBody.Location = new Point(12, labelBody.Location.Y);
            }
            if (newWidth >= 440)
            {
                this.Width = newWidth;
            }
            else
            {
                this.Width = 440;
            }

            int newHeight = labelBody.Size.Height + 100;
            if (newHeight >= 200)
            {
                this.Height = newHeight;
            }
            else
            {
                this.Height = 200;
            }
        }

        #endregion

        private void buttonYes_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.Yes;
            lastResult = MemoryBoxResult.Yes;
            DialogResult = DialogResult.Yes;
        }

        private void buttonYestoAll_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.Yes;
            lastResult = MemoryBoxResult.YesToAll;
            DialogResult = DialogResult.Yes;
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.No;
            lastResult = MemoryBoxResult.No;
            DialogResult = DialogResult.No;
        }

        private void buttonNotoAll_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.No;
            lastResult = MemoryBoxResult.NoToAll;
            DialogResult = DialogResult.No;
        }

    }
}