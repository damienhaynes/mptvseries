using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbCast : TmdbCredit
    {
        [DataMember(Name = "character")]
        public string Character { get; set; }

        [DataMember(Name = "order")]
        public int Order { get; set; }
    }
}
