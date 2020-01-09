
namespace WindowPlugins.GUITVSeries.TmdbAPI
{
    public static class TmdbURIs
    {
        public const string ApiKey = "e636af47bb9604b7fe591847a98ca408";
        private const string ApiUrl = "http://api.themoviedb.org/3/";

        public static string ApiConfig = string.Concat( ApiUrl, "configuration?api_key=", ApiKey );
        public static string ApiGetShowImages = string.Concat( ApiUrl, "tv/{0}/images?api_key=", ApiKey, "&include_image_language={1}" );
        public static string ApiGetSeasonImages = string.Concat( ApiUrl, "tv/{0}/season/{1}/images?api_key=", ApiKey, "&include_image_language={2}" );
        public static string ApiGetEpisodeImages = string.Concat( ApiUrl, "tv/{0}/season/{1}/episode/{2}/images?api_key=", ApiKey, "&include_image_language={2}");

        public static string apiFind = string.Concat( ApiUrl, "find/{0}?&external_source={1}&api_key=", ApiKey );
    }
}
