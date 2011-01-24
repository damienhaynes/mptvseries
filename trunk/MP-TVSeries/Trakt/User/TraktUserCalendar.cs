using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Trakt.Common;

namespace Trakt.User
{
    [DataContract]
    public class TraktUserCalendar : TraktResponse
    {
        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "episodes")]
        public List<TraktEpisode> Episodes { get; set; }
    }
}
