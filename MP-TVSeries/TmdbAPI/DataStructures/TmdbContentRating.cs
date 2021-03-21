using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbContentRating
    {
        [DataMember(Name = "iso_3166_1")]
        public string Code { get; set; }

        [DataMember(Name = "rating")]
        public string Rating { get; set; }
    }
}
