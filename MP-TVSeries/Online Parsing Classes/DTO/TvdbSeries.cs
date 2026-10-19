using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes.DTO
{
  [DataContract]
  public class TvdbSeries
  {
    [DataMember( Name = "tvdb_id" )]
    public string TvdbId { get; set; }

    [DataMember( Name = "name" )]
    public string Name { get; set; }

    [DataMember( Name = "slug" )]
    public string Slug { get; set; }

    [DataMember( Name = "country" )]
    public string Country { get; set; }

    [DataMember( Name = "first_air_time" )]
    public string FirstAirTime { get; set; }

    [DataMember( Name = "overview" )]
    public string Overview { get; set; }

    [DataMember( Name = "overviews" )]
    public Dictionary<string, string> Overviews { get; set; }

    [DataMember( Name = "translations" )]
    public Dictionary<string, string> Translations { get; set; }

    [DataMember( Name = "image_url" )]
    public string ImageUrl { get; set; }

    [DataMember( Name = "thumbnail" )]
    public string Thumbnail { get; set; }

    [DataMember( Name = "aliases" )]
    public List<string> Aliases { get; set; }

    [DataMember( Name = "genres" )]
    public List<string> Genres { get; set; }

    [DataMember( Name = "network" )]
    public string Network { get; set; }

    [DataMember( Name = "status" )]
    public string Status { get; set; }

    [DataMember( Name = "year" )]
    public string Year { get; set; }

    [DataMember( Name = "primary_language" )]
    public string PrimaryLanguage { get; set; }

    [DataMember( Name = "type" )]
    public string Type { get; set; }

    [DataMember( Name = "remote_ids" )]
    public List<TvdbRemoteId> RemoteIds { get; set; }
  }
}
