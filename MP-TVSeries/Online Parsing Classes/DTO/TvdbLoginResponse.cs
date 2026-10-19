using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes.DTO
{
  [DataContract]
  class TvdbLoginResponse : TvdbResponseBase
  {
    [DataMember( Name = "data" )]
    public TvdbLoginData Data { get; set; }
  }
}
