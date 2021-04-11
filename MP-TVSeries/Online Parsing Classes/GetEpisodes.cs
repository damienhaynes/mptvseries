#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2021
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
using System.Linq;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;

namespace WindowPlugins.GUITVSeries
{
    public class GetEpisodes
    {
        private readonly List<DBOnlineEpisode> mListEpisodes = new List<DBOnlineEpisode>();

        public List<DBOnlineEpisode> Results
        {
            get { return mListEpisodes; }
        }

        public GetEpisodes(string aSeriesID, int[] aSeasons)
        {
            if (Int32.TryParse(aSeriesID, out int lSeriesId) && lSeriesId > 0)
            {
                if (aSeasons == null)
                {
                    aSeasons = DBSeason.Get(lSeriesId).Select(s => (int)s[DBSeason.cIndex]).ToArray();
                }

                foreach (int season in aSeasons)
                {
                    DoWork(lSeriesId, season);
                }
            }
        }        
        
        public void DoWork(int aSeriesID, int aSeason)
        {
            // we can only query for episodes per season at TMDb
            TmdbSeasonDetail lSeason = TmdbAPI.TmdbAPI.GetSeasonDetail(aSeriesID, aSeason);
            if (lSeason == null) return;

            foreach (TmdbEpisodeDetail epsiode in lSeason.Episodes)
            {
                var lEpisode = new DBOnlineEpisode();

                lEpisode[DBOnlineEpisode.cID] = epsiode.Id;
                lEpisode[DBOnlineEpisode.cDirector] = string.Join("|", epsiode.Crew.Where(c => c.Job == "Director").Select(d => d.Name));
                lEpisode[DBOnlineEpisode.cEpisodeIndex] = epsiode.EpisodeNumber;
                lEpisode[DBOnlineEpisode.cEpisodeName] = epsiode.Name;
                lEpisode[DBOnlineEpisode.cEpisodeSummary] = epsiode.Overview;
                lEpisode[DBOnlineEpisode.cEpisodeThumbnailUrl] = epsiode.StillPath != null ? "original" + epsiode.StillPath : string.Empty;
                lEpisode[DBOnlineEpisode.cTMDbEpisodeThumbnailUrl] = epsiode.StillPath != null ? "original" + epsiode.StillPath : string.Empty;
                lEpisode[DBOnlineEpisode.cFirstAired] = epsiode.AirDate;
                lEpisode[DBOnlineEpisode.cGuestStars] = string.Join("|", epsiode.GuestStars.Select(g => g.Name));
                lEpisode[DBOnlineEpisode.cProductionCode] = epsiode.ProductionCode;
                lEpisode[DBOnlineEpisode.cRating] = epsiode.Score;
                lEpisode[DBOnlineEpisode.cRatingCount] = epsiode.Votes;
                lEpisode[DBOnlineEpisode.cSeasonIndex] = epsiode.SeasonNumber;
                lEpisode[DBOnlineEpisode.cSeasonID] = lSeason.Id;
                lEpisode[DBOnlineEpisode.cSeriesID] = aSeriesID;
                lEpisode[DBOnlineEpisode.cWriter] = string.Join("|", epsiode.Crew.Where(c => c.Job == "Writer").Select(d => d.Name));
                lEpisode[DBOnlineEpisode.cEpisodeThumbnailSource] = (int)GUI.ArtworkDataProvider.TMDb;

                //lEpisode[DBOnlineEpisode.cAbsoluteNumber] = string.Empty;
                //lEpisode[DBOnlineEpisode.cAirsAfterSeason] = string.Empty;
                //lEpisode[DBOnlineEpisode.cAirsBeforeSeason] = string.Empty;
                //lEpisode[DBOnlineEpisode.cAirsBeforeEpisode] = string.Empty;
                lEpisode[DBOnlineEpisode.cDVDEpisodeNumber] = string.Empty;
                lEpisode[DBOnlineEpisode.cDVDSeasonNumber] = string.Empty;
                lEpisode[DBOnlineEpisode.cCombinedEpisodeNumber] = epsiode.EpisodeNumber; // TODO: review if we still need this, but for now keep for queries
                lEpisode[DBOnlineEpisode.cCombinedSeason] = epsiode.SeasonNumber; // TODO: review if we still need this, but for now keep for queries

                mListEpisodes.Add(lEpisode);
            }
        }
    }
}
