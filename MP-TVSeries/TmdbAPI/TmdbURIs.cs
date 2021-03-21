
namespace WindowPlugins.GUITVSeries.TmdbAPI
{
    public static class TmdbURIs
    {
        public const string ApiKey = "e636af47bb9604b7fe591847a98ca408";
        private const string ApiUrl = "http://api.themoviedb.org/3/";

        public static string ApiConfig = string.Concat( ApiUrl, "configuration?api_key=", ApiKey );
        public static string ApiGetLanguages = string.Concat(ApiUrl, "configuration/languages?api_key=", ApiKey);
        public static string ApiGetShowImages = string.Concat( ApiUrl, "tv/{0}/images?api_key=", ApiKey, "&include_image_language={1}" );
        public static string ApiGetSeasonImages = string.Concat( ApiUrl, "tv/{0}/season/{1}/images?api_key=", ApiKey, "&include_image_language={2}" );
        public static string ApiGetEpisodeImages = string.Concat( ApiUrl, "tv/{0}/season/{1}/episode/{2}/images?api_key=", ApiKey, "&include_image_language={3}");
        public static string ApiSearchTvShow = string.Concat(ApiUrl, "search/tv?api_key=", ApiKey, "&language={0}&page={1}&query={2}&include_adult={3}");
        public static string ApiTvShowDetail = string.Concat(ApiUrl, "tv/{0}?api_key=", ApiKey, "&language={0}&append_to_response=credits,images,external_ids,content_ratings&include_image_language=en,null");
        public static string ApiTvSeasonDetail = string.Concat(ApiUrl, "tv/{0}/season/{1}?api_key=", ApiKey, "&language={2}&append_to_response=images&include_image_language=en,null");
        public static string ApiFind = string.Concat( ApiUrl, "find/{0}?&external_source={1}&api_key=", ApiKey );
        public static string ApiChanges = string.Concat(ApiUrl, "/tv/changes?api_key=", ApiKey, "&start_date={0}&page={1}");
    }
}
