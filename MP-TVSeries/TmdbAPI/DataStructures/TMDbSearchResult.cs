using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TMDbSearchResultBase
    {
        [DataMember( Name = "backdrop_path" )]
        public string BackdropPath { get; set; }

        [DataMember( Name = "id" )]
        public int Id { get; set; }

        [DataMember( Name = "popularity" )]
        public decimal Popularity { get; set; }

        [DataMember( Name = "poster_path" )]
        public string PosterPath { get; set; }

        [DataMember( Name = "vote_average" )]
        public decimal VoteAverage { get; set; }

        [DataMember( Name = "vote_count" )]
        public uint VoteCount { get; set; }

        [DataMember( Name = "genre_ids" )]
        public List<string> Genres { get; set; }

        [DataMember( Name = "origin_country" )]
        public List<string> Countries { get; set; }

        [DataMember( Name = "overview" )]
        public string Overview { get; set; }

        [DataMember( Name = "original_language" )]
        public string OriginalLanguage { get; set; }
    }

    [DataContract]
    public class TMDbSearchResultShow : TMDbSearchResultBase
    {
        [DataMember( Name = "first_air_date" )]
        public string FirstAirDate { get; set; }

        [DataMember( Name = "original_name" )]
        public string OriginalName { get; set; }

        [DataMember( Name = "name" )]
        public string Name { get; set; }
    }
}