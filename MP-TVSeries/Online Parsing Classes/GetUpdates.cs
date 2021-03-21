using System;
using System.Collections.Generic;
using System.Linq;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    class GetUpdates
    {
        private readonly long mTimestamp;
        private readonly List<int> mSeries = null;

        public long OnlineTimeStamp
        { 
            get
            { 
                return mTimestamp;
            }
        }

        public List<int> UpdatedSeries
        { 
            get
            { 
                return mSeries;
            }
        }

        public GetUpdates(long aDaysSinceLastUpdate, long aCurrentEpoch)
        {
            mSeries = new List<int>();

            // TMDb only supports maximum 14 days
            int lDays = (int)Math.Min(aDaysSinceLastUpdate, 14);

            string lStartDate = DateTime.UtcNow.Subtract(new TimeSpan(lDays, 0, 0, 0)).ToString("yyyy-MM-dd");

            MPTVSeriesLog.Write($"Downloading online updates since {lStartDate}");

            TmdbTvChanges lTmdbChanges = TmdbAPI.TmdbAPI.GetTvChanges(lStartDate);

            if (lTmdbChanges == null || lTmdbChanges.Results == null)
                return;

            mSeries.AddRange(lTmdbChanges.Results.Select(r => r.Id));

            this.mTimestamp = aCurrentEpoch;
        }
    }
}
