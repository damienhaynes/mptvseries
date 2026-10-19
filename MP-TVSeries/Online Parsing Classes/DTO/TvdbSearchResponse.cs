using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes.DTO
{
  [DataContract]
  public class TvdbSearchResponse : TvdbResponseBase
  {
    [DataMember( Name = "data" )]
    public List<TvdbSeries> Data { get; set; }
  }
}
