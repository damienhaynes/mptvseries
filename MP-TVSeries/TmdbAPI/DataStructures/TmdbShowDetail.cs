using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbShowDetail : TmdbShow
    {
        [DataMember(Name = "credits")]
        public TmdbCredits Credits { get; set; }

        [DataMember(Name = "images")]
        public TmdbShowImages Images { get; set; }

        [DataMember(Name = "external_ids")]
        public TmdbExternalId ExternalIds { get; set; }

        [DataMember(Name = "content_ratings")]
        public TmdbContentRatings ContentRatings { get; set; }
    }
}
