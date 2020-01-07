using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures
{
    [DataContract]
    public class FanartTvImage
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "lang")]
        public string Language { get; set; }

        [DataMember(Name = "likes")]
        public int Likes { get; set; }
    }
}
