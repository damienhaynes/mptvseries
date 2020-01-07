using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures
{
    [DataContract]
    public class FanartTvImages
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "thetvdb_id")]
        public string TvdbId { get; set; }

        [DataMember(Name = "clearlogo")]
        public List<FanartTvImage> ClearLogos { get; set; }

        [DataMember(Name = "hdtvlogo")]
        public List<FanartTvImage> HdtvLogos { get; set; }

        [DataMember(Name = "clearart")]
        public List<FanartTvImage> ClearArts { get; set; }

        [DataMember(Name = "showbackground")]
        public List<FanartTvImage> TvShowBackgrounds { get; set; }

        [DataMember(Name = "tvthumb")]
        public List<FanartTvImage> TvThumbs { get; set; }

        [DataMember(Name = "seasonposter")]
        public List<FanartTvSeasonImage> TvSeasonPosters { get; set; }

        [DataMember(Name = "seasonthumb")]
        public List<FanartTvSeasonImage> TvSeasonThumbs { get; set; }

        [DataMember(Name = "hdclearart")]
        public List<FanartTvImage> HdClearArts { get; set; }

        [DataMember(Name = "tvbanner")]
        public List<FanartTvImage> TvBanners { get; set; }

        [DataMember(Name = "characterart")]
        public List<FanartTvImage> CharacterArts { get; set; }

        [DataMember(Name = "tvposter")]
        public List<FanartTvImage> TvPosters { get; set; }

        [DataMember(Name = "seasonbanner")]
        public List<FanartTvSeasonImage> TvSeasonBanners { get; set; }
    }
}
