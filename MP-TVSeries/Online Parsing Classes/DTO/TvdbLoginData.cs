using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes.DTO
{
  [DataContract]
  class TvdbLoginData
  {
    [DataMember( Name = "token" )]
    public string Token { get; set; }
  }
}
