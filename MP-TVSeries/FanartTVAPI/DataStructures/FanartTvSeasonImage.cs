using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures
{
    [DataContract]
    public class FanartTvSeasonImage : FanartTvImage
    {
        [DataMember(Name = "season")]
        public int Season { get; set; }
    }
}
