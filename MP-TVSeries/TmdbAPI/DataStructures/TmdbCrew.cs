using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbCrew : TmdbCredit
    {
        [DataMember(Name = "department")]
        public string Department { get; set; }

        [DataMember(Name = "job")]
        public string Job { get; set; }
    }
}
