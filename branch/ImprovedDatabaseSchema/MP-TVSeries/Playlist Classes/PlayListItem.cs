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
using MediaPortal.TagReader;
using WindowPlugins.GUITVSeries.DataBase;
using WindowPlugins.GUITVSeries.DataClass;

namespace WindowPlugins.GUITVSeries
{
    [Serializable()]
    public class PlayListItem
    {
        protected string _fileName = "";
        protected string _description = "";
        protected double _duration = 0;
        protected string _episodeThumb = "-";
        protected string _episodeID = "";
        protected string _summary = "";
        protected string _firstAired = "";
        bool _isPlayed = false;       
        protected DBEpisode _episode = null;

        public PlayListItem()
        {
        }

        public PlayListItem(DBEpisode episode)
        {
            Episode = episode;
        }

        public virtual DBEpisode Episode
        {
            get { return _episode; }
            set
            {
                if (value == null)
                    return;

                _episode = value;

                FileName = value[DBEpisode.cFilename];
                Description = value[DBEpisode.cEpisodeName];
                Duration = value["localPlaytime"];
                EpisodeID = value[DBOnlineEpisode.cID];                
                Summary = value[DBOnlineEpisode.cEpisodeSummary];
                FirstAired = value[DBOnlineEpisode.cFirstAired];
                EpisodeThumb = ImageAllocator.GetEpisodeImage(value);                               
            }
        }

        public virtual string Summary
        {
            get { return _summary; }
            set
            {
                if (value == null)
                    return;
                _summary = value;
            }
        }

        public virtual string EpisodeID
        {
            get { return _episodeID; }
            set
            {
                if (value == null)
                    return;
                _episodeID = value;
            }
        }

        public virtual string FileName
        {
            get { return _fileName; }
            set
            {
                if (value == null)
                    return;
                _fileName = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == null)
                    return;
                _description = value;
            }
        }

        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public string FirstAired
        {
            get { return _firstAired; }
            set { _firstAired = value; }
        }

        public string EpisodeThumb
        {
            get { return _episodeThumb; }
            set { _episodeThumb = value; }
        }

        public bool Watched
        {            
            set
            {
                if (!value)
                    return;

                SQLCondition condition = new SQLCondition();
                condition.Add(new DBEpisode(), DBEpisode.cFilename, _episode[DBEpisode.cFilename], SQLConditionType.Equal);
                List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                foreach (DBEpisode ep in episodes)
                {
                    ep[DBOnlineEpisode.cWatched] = "1";
                    ep.Commit();
                    //DBSeason.UpdateUnWatched(ep);
                    //DBSeries.UpdateUnWatched(ep);
                }
                // Update Episode Counts
                DBSeries series = Helper.getCorrespondingSeries(_episode[DBEpisode.cSeriesID]);
                DBSeason season = Helper.getCorrespondingSeason(_episode[DBEpisode.cSeriesID], _episode[DBEpisode.cSeasonIndex]);
                DBSeason.UpdatedEpisodeCounts(series, season);           
            }
        }

        public bool Played
        {
            get { return _isPlayed; }
            set { _isPlayed = value; }
        }
    }
}