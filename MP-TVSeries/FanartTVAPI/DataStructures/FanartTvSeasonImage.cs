using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures
{
    [DataContract]
    public class FanartTvSeasonImage : FanartTvImage
    {
        // data type as string to cater for "all" season
        [DataMember(Name = "season")]
        public string Season { get; set; }
    }
}
