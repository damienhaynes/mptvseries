using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Trakt.Common;

namespace Trakt.User
{
    [DataContract]
    public class TraktUserProfile : TraktResponse
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "protected")]
        public string Protected { get; set; }

        [DataMember(Name = "full_name")]
        public string FullName { get; set; }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "age")]
        public string Age { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "about")]
        public string About { get; set; }

        [DataMember(Name = "joined")]
        public string JoinDate { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "stats")]
        public Stats Statistics { get; set; }

        [DataMember(Name = "watching")]
        public TraktEpisode Watching { get; set; }

        [DataMember(Name = "watched")]
        public List<TraktEpisode> WatchedHistory { get; set; }
    }

    [DataContract]
    public class Stats
    {
        [DataMember(Name = "friends")]
        public string FriendCount { get; set; }

        [DataMember(Name = "shows")]
        public Shows Shows  { get; set; }

        [DataMember(Name = "episodes")]
        public Episodes Episodes { get; set; }

        [DataMember(Name = "movies")]
        public Movies Movies { get; set; }
    }

    [DataContract]
    public class Shows
    {
        [DataMember(Name = "library")]
        public string Count { get; set; }
    }

    [DataContract]
    public class Episodes
    {
        [DataMember(Name = "watched")]
        public string WatchedCount { get; set; }

        [DataMember(Name = "watched_unique")]
        public string WatchedUniqueCount { get; set; }

        [DataMember(Name = "watched_trakt")]
        public string WatchedTraktCount { get; set; }

        [DataMember(Name = "watched_trakt_unique")]
        public string WatchedTraktUniqueCount { get; set; }

        [DataMember(Name = "watched_elsewhere")]
        public string WatchedElseWhereCount { get; set; }

        [DataMember(Name = "unwatched")]
        public string UnWatchedCount { get; set; }
    }

    [DataContract]
    public class Movies
    {
        [DataMember(Name = "watched")]
        public string WatchedCount { get; set; }

        [DataMember(Name = "watched_trakt")]
        public string WatchedTraktCount { get; set; }

        [DataMember(Name = "watched_elsewhere")]
        public string WatchedElseWhereCount { get; set; }

        [DataMember(Name = "library")]
        public string MovieCount { get; set; }

        [DataMember(Name = "unwatched")]
        public string UnWatchedCount { get; set; }
    }
}
