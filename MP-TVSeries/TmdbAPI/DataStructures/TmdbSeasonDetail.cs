using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbSeasonDetail : TmdbSeasonBase
    {
        [DataMember(Name = "_id")]
        public string _Id { get; set; }

        [DataMember(Name = "episodes")]
        public List<TmdbEpisodeDetail> Episodes { get; set; }

        [DataMember(Name = "images")]
        public TmdbSeasonImages Images { get; set; }
    }
}
