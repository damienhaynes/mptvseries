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
#if inclDownloaders
using System;
using System.Collections.Generic;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    interface IDownloaders
    {
    }

    // used to do websearches
    // input: DBEpisode
    // ouput: DownloadObject
    interface IWebSources
    {
        bool Search();
        bool SetEpisode(DBEpisode episode);
        DownloadObject Result();
    }

    class DownloadObject
    {
        public enum Type
        {
            URL,
            Torrent,
            NZB
        }

        private Type m_type;
        private String m_Resource;
        

        public DownloadObject(Type type, String sResource)
        {
            m_type = type;
            m_Resource = sResource;
        }

        public Type ResourceType
        {
            get { return m_type;}
        }

        public String ResourceString
        {
            get { return m_Resource; }
        }
    };
}
#endif