using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbCredits
    {
        [DataMember(Name = "cast")]
        public List<TmdbCast> Cast { get; set; }

        [DataMember(Name = "crew")]
        public List<TmdbCrew> Crew { get; set; }
    }
}
