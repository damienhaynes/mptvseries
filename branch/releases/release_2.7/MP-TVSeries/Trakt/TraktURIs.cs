using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trakt
{
    public static class TraktURIs
    {
        public const string ScrobbleShow = @"http://api.trakt.tv/show/{0}/<apiKey>";
        public const string ScrobbleMovie = @"http://api.trakt.tv/movie/{0}/<apiKey>";
        public const string UserMostRecentWatchedEpisodes = @"http://api.trakt.tv/user/watched/episodes.json/<apiKey>/{0}";
        public const string UserWatchedEpisodes = @"http://api.trakt.tv/user/library/shows/watched.json/<apiKey>/{0}";
        public const string UserWatchedMovies = @"http://api.trakt.tv/user/watched/movies.json/<apiKey>/{0}";
        public const string UserProfile = @"http://api.trakt.tv/user/profile.json/<apiKey>/{0}";
        public const string SeriesOverview = @"http://api.trakt.tv/show/summary.json/<apiKey>/{0}";
        public const string UserLibraryShows = @"http://api.trakt.tv/user/library/shows.json/<apiKey>/{0}";
        public const string UserLibraryMovies = @"http://api.trakt.tv/user/library/movies.json/<apiKey>/{0}";
        public const string SyncEpisodeLibrary = @"http://api.trakt.tv/show/episode/{0}/<apiKey>";
        public const string UserCalendarShows = @"http://api.trakt.tv/user/calendar/shows.json/<apiKey>/{0}/{1}/{2}";
        public const string UserFriends = @"http://api.trakt.tv/user/friends.json/<apiKey>/{0}";
        public const string RateItem = @"http://api.trakt.tv/rate/{0}/<apiKey>";        
    }
}
