using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Trakt.Common
{
    [DataContract]
    public class TraktEpisode
    {
        [DataMember(Name = "show")]
        public Series Show { get; set; }

        [DataMember(Name = "episode")]
        public Episode Episode { get; set; }
    }

    [DataContract]
    public class Series
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "tvdb_id")]
        public string SeriesID { get; set; }

        [DataMember(Name = "imdb_id")]
        public string IMDBID { get; set; }
    }

    [DataContract]
    public class Episode
    {
        [DataMember(Name = "season")]
        public string SeasonIndex { get; set; }

        [DataMember(Name = "number")]
        public string EpisodeIndex { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "overview")]
        public string Overview { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "first_aired")]
        public string FirstAired { get; set; }
    }
}
