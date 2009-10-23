using System.Collections.Generic;
using SubtitleDownloader.Core;
using SubtitleDownloader.Implementations.OpenSubtitles;
using WindowPlugins.GUITVSeries.Feedback;

namespace WindowPlugins.GUITVSeries.Subtitles
{
    public class OpenSubtitlesRetriever : BaseSubtitleRetriever
    {
        public OpenSubtitlesRetriever(IFeedback feedback) : 
            base(feedback, new OpenSubtitlesDownloader())
        {

        }

        public override List<Subtitle> PerformSearch(string languageCode)
        {
            SearchQuery query = new SearchQuery(EpisodeFileNameWithoutExtension);
            query.LanguageCode = languageCode;

            return Downloader.SearchSubtitles(query);
        }
    }
}
