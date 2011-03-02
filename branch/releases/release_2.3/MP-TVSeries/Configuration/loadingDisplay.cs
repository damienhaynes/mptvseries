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
    public partial class loadingDisplay : Form
    {
        public loadingDisplay()
        {
            InitializeComponent();
            ShowWaiting();
        }

        public new void Close()
        {
            this.Dispose();
            base.Close();
        }

        void ShowWaiting()
        {
            this.series.Text = "0 " + Translation.Series_Plural;
            this.season.Text = "0 " + Translation.Seasons;
            this.episodes.Text = "0 " + Translation.Episodes;
            this.Show();
            this.Refresh();
        }

        public void updateStats(int series, int seasons, int episodes)
        {
            this.series.Text = series.ToString() + " " + Translation.Series_Plural;
            this.season.Text = seasons.ToString() + " " + Translation.Seasons;
            this.episodes.Text = episodes.ToString() + " " + Translation.Episodes;
            this.Refresh();
        }

    }
}