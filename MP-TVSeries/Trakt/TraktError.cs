using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.Trakt
{
    [DataContract]
    public class TraktError
    {
        [DataMember]
        public string status { get; set;}

        [DataMember]
        public string message { get; set; }
    }
}
