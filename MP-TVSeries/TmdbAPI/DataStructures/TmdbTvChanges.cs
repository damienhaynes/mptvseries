using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbTvChanges
    {
        [DataMember(Name = "results")]
        public List<TmdbChange> Results { get; set; }

        [DataMember(Name = "page")]
        public int Page { get; set; }

        [DataMember(Name = "total_pages")]
        public int TotalPages { get; set; }

        [DataMember(Name = "total_results")]
        public int TotalResults { get; set; }
    }

    [DataContract]
    public class TmdbChange
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "adult")]
        public bool? IsAdult { get; set; }
    }
}
