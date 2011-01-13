using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Trakt.User
{
    [DataContract]
    public class TraktWatchedEpisodeHistory : TraktResponse
    {
        [DataMember(Name="watched")]
        public string WatchedID { get; set; }

        [DataMember(Name="show")]
        public Series Show { get; set; }

        [DataMember(Name="episode")]
        public Episode Episode { get; set; }     
    }

    [DataContract]
    public class Series
    {
        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="url")]
        public string Url { get; set; }

        [DataMember(Name="tvdb_id")]
        public string SeriesID { get; set; }
    }

    [DataContract]
    public class Episode
    {
        [DataMember(Name="season")]
        public string SeasonIndex { get; set; }

        [DataMember(Name="number")]
        public string EpisodeIndex { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="url")]
        public string Url { get; set; }
        
        [DataMember(Name="first_aired")]
        public string FirstAired { get; set; }
    }
}
