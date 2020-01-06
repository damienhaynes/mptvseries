using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbRequestAge
    {
        [DataMember]
        public string RequestAge { get; set; }
    }
}
