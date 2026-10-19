using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes.DTO
{
  [DataContract]
  public class TvdbRemoteId
  {
    [DataMember( Name = "id" )]
    public string Id { get; set; }

    [DataMember( Name = "type" )]
    public int Type { get; set; }

    [DataMember( Name = "sourceName" )]
    public string SourceName { get; set; }
  }
}
