#region Copyright (C) 2005-2008 Team MediaPortal

/* 
 *	Copyright (C) 2005-2008 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using MediaPortal.Util;
using MediaPortal.GUI.Library;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries
{
    public class PlayListIO : IPlayListIO
    { 
        private PlayList playlist;        
        private string basePath;

        public PlayListIO()
        {
        }

        public bool Load(PlayList incomingPlaylist, string playlistFileName)
        {
            if (playlistFileName == null)
                return false;
            
            playlist = incomingPlaylist;
            playlist.Clear();

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(playlistFileName);
            }
            catch (XmlException e)
            {
                MPTVSeriesLog.Write(string.Format("Cannot Load Playlist file: {0}",playlistFileName));
                MPTVSeriesLog.Write(e.Message);
                return false;
            }

            try
            {
                playlist.Name = Path.GetFileName(playlistFileName);
                basePath = Path.GetDirectoryName(Path.GetFullPath(playlistFileName));

                XmlNodeList nodeList = doc.DocumentElement.SelectNodes("/Playlist");
                if (nodeList == null)
                    return false;

                foreach (XmlNode node in nodeList)
                {                
                    foreach (XmlNode itemNode in node.ChildNodes)
                    {
                        if (itemNode.Name == "Episode")
                        {
                            foreach (XmlNode propertyNode in itemNode.ChildNodes)
                            {
                                if (propertyNode.Name == "ID")
                                {
                                    if (!AddItem(propertyNode.InnerText))
                                        return false;
                                }
                            }
                        }
                    }                   
                }        
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write(string.Format("exception loading playlist {0} err:{1} stack:{2}", playlistFileName, ex.Message, ex.StackTrace));
                return false;
            }
            return true;
        }               

        private bool AddItem(string episodeID)
        {
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, episodeID, SQLConditionType.Equal);

            List<DBEpisode> ep = DBEpisode.Get(condition, false);

            if (ep.Count != 1)
                return false;
            
            PlayListItem newItem = new PlayListItem(ep[0]);
            playlist.Add(newItem);
            return true;
        }

        public void Save(PlayList playlist, string fileName)
        {
            try
            {
                XmlTextWriter textWriter = new XmlTextWriter(fileName, null);

                textWriter.Formatting = Formatting.Indented;
                textWriter.Indentation = 4;

                textWriter.WriteStartDocument();
                textWriter.WriteComment("MP-TVSeries playlist");
                
                // Create a <Playlist> element, to contain a list of episodes in playlist
                textWriter.WriteStartElement("Playlist");

                foreach (PlayListItem item in playlist)
                {
                    // Create an <Episode> element for each episode
                    textWriter.WriteStartElement("Episode");
                    
                    // Store Episode ID, this is all that is required
                    textWriter.WriteStartElement("ID");
                    textWriter.WriteString(item.EpisodeID);
                    textWriter.WriteEndElement();

                    // Close <Episode> element
                    textWriter.WriteEndElement();                    
                }

                textWriter.WriteEndElement();
                textWriter.WriteEndDocument();
                textWriter.Close();
                                              
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write(string.Format("failed to save a playlist {0}. err: {1} stack: {2}", fileName, e.Message, e.StackTrace));
            }
        }
    }
}