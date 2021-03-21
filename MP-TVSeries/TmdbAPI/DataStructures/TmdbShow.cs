using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbShow
    {
        [DataMember(Name = "backdrop_path")]
        public string BackdropPath { get; set; }

        [DataMember(Name = "created_by")]
        public List<TmdbPerson> Creators { get; set; }

        [DataMember(Name = "episode_run_time")]
        public List<int> EpisodeRuntimes { get; set; }

        [DataMember(Name = "first_air_date")]
        public string FirstAirDate { get; set; }

        [DataMember(Name = "genres")]
        public List<TmdbGenre> Genres { get; set; }

        [DataMember(Name = "homepage")]
        public string Homepage { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "in_production")]
        public bool InProduction { get; set; }

        [DataMember(Name = "languages")]
        public List<string> Languages { get; set; }

        [DataMember(Name = "last_air_date")]
        public string LastAirDate { get; set; }

        [DataMember(Name = "last_episode_to_air")]
        public TmdbEpisodeBase LastEpisodeToAir { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "next_episode_to_air")]
        public TmdbEpisodeBase NextEpisodeToAir { get; set; }

        [DataMember(Name = "networks")]
        public List<TmdbNetwork> Networks { get; set; }

        [DataMember(Name = "number_of_episodes")]
        public int EpisodeCount { get; set; }

        [DataMember(Name = "number_of_seasons")]
        public int SeasonCount { get; set; }

        [DataMember(Name = "origin_country")]
        public List<string> OriginCountries { get; set; }

        [DataMember(Name = "original_language")]
        public string OriginalLanguage { get; set; }

        [DataMember(Name = "original_name")]
        public string OriginalName { get; set; }

        [DataMember(Name = "overview")]
        public string Overview { get; set; }

        [DataMember(Name = "popularity")]
        public double Popularity { get; set; }

        [DataMember(Name = "poster_path")]
        public string PosterPath { get; set; }

        [DataMember(Name = "production_companies")]
        public List<TmdbProductionCompany> ProductionCompanies { get; set; }

        [DataMember(Name = "production_countries")]
        public List<TmdbProductionCountry> ProductionCountries { get; set; }

        [DataMember(Name = "seasons")]
        public List<TmdbSeasonBase> Seasons { get; set; }

        [DataMember(Name = "spoken_languages")]
        public List<TmdbLanguage> SpokenLanaguages { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "tagline")]
        public string Tagline { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "vote_average")]
        public double Score { get; set; }

        [DataMember(Name = "vote_count")]
        public int Votes { get; set; }
    }
}
