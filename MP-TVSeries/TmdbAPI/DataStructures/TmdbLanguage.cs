using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbLanguage
    {
        [DataMember(Name = "iso_639_1")]
        public string Code { get; set; }

        [DataMember(Name = "english_name")]
        public string EnglishName { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

    }
}
