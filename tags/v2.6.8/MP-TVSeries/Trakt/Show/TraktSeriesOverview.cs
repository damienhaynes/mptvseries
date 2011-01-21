using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Trakt.Show
{
    [DataContract]
    public class TraktSeriesOverview : TraktResponse
    {
        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="year")]
        public string Year { get; set; }
                
        [DataMember(Name="url")]
        public string Url { get; set; }

        [DataMember(Name="first_aired")]
        public string FirstAired { get; set; }
        
        [DataMember(Name="tvdb_id")]
        public string SeriesID { get; set; }

        [DataMember(Name="poster")]
        public string Poster { get; set; }

        [DataMember(Name = "top_watchers")]
        public List<TopWatchers> TopWatchers { get; set; }

        [DataMember(Name = "top_episodes")]
        public List<TopEpisodes> TopEpisodes { get; set; }

        [DataMember(Name = "ratings")]
        public Ratings Ratings { get; set; }
    }

    [DataContract]
    public class TopWatchers
    {
        [DataMember(Name = "plays")]
        public string Plays { get; set; }

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
    }

    [DataContract]
    public class TopEpisodes
    {
        [DataMember(Name = "plays")]
        public string Plays { get; set; }

        [DataMember(Name = "season")]
        public string SeasonIndex { get; set; }

        [DataMember(Name = "number")]
        public string EpisodeIndex { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "first_aired")]
        public string FirstAired { get; set; }
    }

    [DataContract]
    public class Ratings
    {
        [DataMember(Name = "percentage")]
        public string Score { get; set; }

        [DataMember(Name = "votes")]
        public string Votes { get; set; }

        [DataMember(Name = "loved")]
        public string Loved { get; set; }

        [DataMember(Name = "hated")]
        public string Hated { get; set; }
    }
}
