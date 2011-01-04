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
        public const string SendUpdate = @"""type"":""{0}"",""status"":""{1}"",""title"":""{2}"",""year"":""{3}"",""season"":""{4}"",""episode"":""{5}"",""tvdbid"":""{6}"",""progress"":""{7}"",""plugin_version"":""{8}"",""media_center"":""{9}"",""media_center_version"":""{10}"",""media_center_date"":""{11}"",""duration"":""{12}"",""username"":""{13}"",""password"":""{14}""";
    }
}
