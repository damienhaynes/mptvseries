using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Trakt.Common;

namespace Trakt.User
{
    [DataContract]
    public class TraktWatchedEpisodeHistory : TraktResponse
    {
        [DataMember(Name="watched")]
        public string WatchedID { get; set; }

        [DataMember(Name="show")]
        public Series Show { get; set; }

        [DataMember(Name="episode")]
        public Episode Episode { get; set; }
    }
}
