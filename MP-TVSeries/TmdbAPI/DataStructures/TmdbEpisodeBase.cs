using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbEpisodeBase
    {
        [DataMember(Name = "air_date")]
        public string AirDate { get; set; }

        [DataMember(Name = "episode_number")]
        public int EpisodeNumber { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "overview")]
        public string Overview { get; set; }

        [DataMember(Name = "production_code")]
        public string ProductionCode { get; set; }

        [DataMember(Name = "season_number")]
        public int SeasonNumber { get; set; }

        [DataMember(Name = "still_path")]
        public string StillPath { get; set; }

        [DataMember(Name = "vote_average")]
        public double Score { get; set; }

        [DataMember(Name = "vote_count")]
        public int Votes { get; set; }
    }
}
