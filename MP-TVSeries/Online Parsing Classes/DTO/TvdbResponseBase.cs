using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes.DTO
{
  [DataContract]
  public class TvdbResponseBase
  {
    [DataMember( Name = "status" )]
    public string Status { get; set; }
  }
}
