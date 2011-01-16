using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trakt
{
    public static class TraktURIs
    {
        public const string ScrobbleShow = @"http://api.trakt.tv/show/{0}/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4";
        public const string ScrobbleMovie = @"http://api.trakt.tv/movie/{0}/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4";
        public const string UserWatchedEpisodes = @"http://api.trakt.tv/user/watched/episodes.json/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4/{0}";
        public const string UserWatchedMovies = @"http://api.trakt.tv/user/watched/movies.json/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4/{0}";
        public const string UserProfile = @"http://api.trakt.tv/user/profile.json/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4/{0}";
        public const string SeriesOverview = @"http://api.trakt.tv/show/summary.json/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4/{0}";
        public const string UserLibraryShows = @"http://api.trakt.tv/user/library/shows.json/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4/{0}";
        public const string UserLibraryMovies = @"http://api.trakt.tv/user/library/movies.json/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4/{0}";
        public const string SyncEpisodeLibrary = @"http://api.trakt.tv/show/episode/{0}/5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4";        
    }
}
