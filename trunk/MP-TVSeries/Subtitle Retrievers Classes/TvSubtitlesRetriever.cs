using System.Collections.Generic;
using SubtitleDownloader.Core;
using SubtitleDownloader.Implementations.Sublight;
using SubtitleDownloader.Implementations.TVSubtitles;
using WindowPlugins.GUITVSeries.Feedback;

namespace WindowPlugins.GUITVSeries.Subtitles
{
    public class TvSubtitlesRetriever : BaseSubtitleRetriever
    {
        public TvSubtitlesRetriever(IFeedback feedback) : 
            base(feedback, new TvSubtitlesDownloader())
        {

        }

        public override List<Subtitle> PerformSearch(string languageCode)
        {
            EpisodeSearchQuery query = new EpisodeSearchQuery(SeriesName, SeasonIndex, EpisodeIndex);
            query.LanguageCode = languageCode;
            
            return Downloader.SearchSubtitles(query);
        }
    }
}
