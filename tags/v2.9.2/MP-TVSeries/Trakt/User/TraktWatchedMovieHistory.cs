using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Trakt.User
{
    [DataContract]
    public class TraktWatchedMovieHistory : TraktResponse
    {
        [DataMember(Name = "watched")]
        public string WatchedID { get; set; }

        [DataMember(Name = "movie")]
        public Movie Movie { get; set; }
    }

    [DataContract]
    public class Movie
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public string Year { get; set; }

        [DataMember(Name = "imdb_id")]
        public string IMDBID { get; set; }

        [DataMember(Name = "tmdb_id")]
        public string TMDBID { get; set; }
    }
}