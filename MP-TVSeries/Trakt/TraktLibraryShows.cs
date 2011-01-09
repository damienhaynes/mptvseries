using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.Trakt
{
    [DataContract]
    public class TraktLibraryShows : TraktResponse
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public string Year { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "tvdb_id")]
        public string SeriesID { get; set; }
    }
}
