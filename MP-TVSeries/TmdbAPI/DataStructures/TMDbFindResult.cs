using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TMDbFindResult
    {
        [DataMember( Name = "tv_results" )]
        public List<TMDbSearchResultShow> Shows { get; set; }
    }
}
