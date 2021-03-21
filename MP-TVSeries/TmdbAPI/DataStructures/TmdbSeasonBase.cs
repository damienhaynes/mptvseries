using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbSeasonBase
    {
        [DataMember(Name = "air_date")]
        public string AirDate { get; set; }

        [DataMember(Name = "episode_count")]
        public int EpisodeCount { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "overview")]
        public string Overview { get; set; }

        [DataMember(Name = "season_number")]
        public int SeasonNumber { get; set; }

        [DataMember(Name = "poster_path")]
        public string PosterPath { get; set; }
    }
}
