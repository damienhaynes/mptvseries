using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Trakt.Common;

namespace Trakt.User
{
    [DataContract]
    public class TraktWatchedShows : TraktResponse
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "tvdb_id")]
        public string SeriesId { get; set; }

        [DataMember(Name = "seasons")]
        public List<Seasons> Seasons { get; set; }
    }

    [DataContract]
    public class Seasons
    {
        [DataMember(Name = "season")]
        public int Season { get; set; }

        [DataMember(Name = "episodes")]
        public List<int> Episodes { get; set; }
    }
}
