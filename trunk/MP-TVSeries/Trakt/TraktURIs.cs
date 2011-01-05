using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowPlugins.GUITVSeries.Trakt
{
    public static class TraktURIs
    {
        public const string APIPost = @"http://api.trakt.tv/post";
        public const string UserWatched = @"http://api.trakt.tv/user/watched/episodes.json/{0}/{1}";
        public const string UserProfile = @"http://api.trakt.tv/user/profile.json/{0}/{1}";
        public const string SeriesOverview = @"http://api.trakt.tv/show/summary.json/{0}/{1}";
        public const string UserLibraryShows = @"http://api.trakt.tv/user/library/shows.json/{0}/{1}";        
    }
}
