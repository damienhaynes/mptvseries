using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbNetwork
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "logo_path")]
        public string LogoPath { get; set; }

        [DataMember(Name = "origin_country")]
        public string OriginCountry { get; set; }

    }
}
