using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbExternalId
    {
        [DataMember(Name = "imdb_id")]
        public string ImdbId { get; set; }

        [DataMember(Name = "freebase_mid")]
        public string FreebaseId { get; set; }

        [DataMember(Name = "tvdb_id")]
        public int? TvdbId { get; set; }

        [DataMember(Name = "tvrage_id")]
        public int? TvRageId { get; set; }

        [DataMember(Name = "facebook_id")]
        public string FacebookId { get; set; }

        [DataMember(Name = "instagram_id")]
        public string InstagramId { get; set; }

        [DataMember(Name = "twitter_id")]
        public string TwitterId { get; set; }

        [DataMember(Name = "id")]
        public int? Id { get; set; }
    }
}
