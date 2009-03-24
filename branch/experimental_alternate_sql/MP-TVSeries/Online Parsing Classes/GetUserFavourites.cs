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
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class GetUserFavourites
    {
        # region properties

        public List<string> Series
        {
            get
            {
                return _series;
            }
            set
            {
                _series = value;
            }
        } private List<string> _series = new List<string>();

        # endregion properties

        public GetUserFavourites(string sAccountID)
        {
            doWork(sAccountID);
        }

        public void doWork(string sAccountID)
        {
            XmlNode node = Online_Parsing_Classes.OnlineAPI.GetUserFavourites(sAccountID);          

            if (node != null)
            {
                string id = string.Empty;
                foreach (XmlNode itemNode in node.ChildNodes)
                {
                    if (itemNode.Name == "Series")
                    {
                        id = string.Empty;
                        id = itemNode.InnerText;                            
                        Series.Add(id);
                    }                
                }
            }
        }
    }
}
