using System.Collections.Generic;
using System.IO;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    class GetFanart
    {
        private readonly List<DBFanart> mFanart = new List<DBFanart>();

        public List<DBFanart> Fanart
        {
            get
            {
                return mFanart;
            }
        }

        private List<DBFanart> CreateFanartDBList(List<TmdbImage> aImages, int aSeriesID)
        {
            var lFanarts = new List<DBFanart>();

            foreach (TmdbImage image in aImages)
            {
                var lFanart = new DBFanart();

                lFanart[DBFanart.cIndex] = image.GetHashCode().ToString();
                lFanart[DBFanart.cSeriesID] = aSeriesID;
                lFanart[DBFanart.cTmdbID] = aSeriesID;
                lFanart[DBFanart.cLanguage] = image.LanguageCode;
                lFanart[DBFanart.cDataSource] = "tmdb";
                lFanart[DBFanart.cRating] = image.Score;
                lFanart[DBFanart.cRatingCount] = image.Votes;
                lFanart[DBFanart.cResolution] = $"{image.Width}x{image.Height}";
                lFanart[DBFanart.cLanguage] = image.LanguageCode;
                lFanart[DBFanart.cSeriesName] = image.LanguageCode != null;
                lFanart[DBFanart.cBannerPath] = "original" + image.FilePath; /* make configurable */
                lFanart[DBFanart.cThumbnailPath] = "w780" + image.FilePath;  /* make configurable */

                // Sync local files with database
                // create the local filename path for the online image e.g. fanart\original\<seriesId>-*.jpg
                string lPath = GUITVSeries.Fanart.GetLocalPath(image.FilePath, aSeriesID.ToString());
                string lFile = Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), lPath);

                if (File.Exists(lFile))
                {
                    // set it, in case it was cleared e.g. new database
                    lFanart[DBFanart.cLocalPath] = lPath;
                }
                else
                {
                    // remove it, in case it was set
                    lFanart[DBFanart.cLocalPath] = string.Empty;
                }

                lFanarts.Add(lFanart);
            }

            return lFanarts;
        }

        public GetFanart(int aSeriesID)
        {
            string lLanguage = OnlineAPI.GetSeriesLanguage(aSeriesID);

            TmdbShowDetail lShowDetail = TmdbAPI.TmdbCache.LoadSeriesFromCache(aSeriesID, lLanguage);
            if (lShowDetail == null) return;

            mFanart = CreateFanartDBList(lShowDetail.Images?.Backdrops, aSeriesID);
        }
    }
}
