using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbProductionCountry
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "iso_3166_1")]
        public string Code { get; set; }
    }
}
