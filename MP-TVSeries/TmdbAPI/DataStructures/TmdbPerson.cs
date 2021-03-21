using System.Runtime.Serialization;

namespace WindowPlugins.GUITVSeries.TmdbAPI.DataStructures
{
    [DataContract]
    public class TmdbPerson
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "credit_id")]
        public string CreditId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "gender")]
        public int? Gender { get; set; }

        [DataMember(Name = "profile_path")]
        public string ProfilePath { get; set; }
    }
}
