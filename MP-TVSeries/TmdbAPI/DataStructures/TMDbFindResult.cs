using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbFindResult
    {
        [DataMember( Name = "tv_results" )]
        public List<TmdbSearchResultShow> Shows { get; set; }
    }
}
