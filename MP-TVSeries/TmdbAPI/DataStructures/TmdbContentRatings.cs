using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbContentRatings
    {
        [DataMember(Name = "results")]
        public List<TmdbContentRating> Results { get; set; }

        [DataMember(Name = "id")]
        public int? Id { get; set; }
    }
}
