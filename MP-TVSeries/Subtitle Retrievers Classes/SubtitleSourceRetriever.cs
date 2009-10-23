using System.Collections.Generic;
using SubtitleDownloader.Core;
using SubtitleDownloader.Implementations.SubtitleSource;
using WindowPlugins.GUITVSeries.Feedback;

namespace WindowPlugins.GUITVSeries.Subtitles
{
    public class SubtitleSourceRetriever : BaseSubtitleRetriever
    {
        public SubtitleSourceRetriever(IFeedback feedback) :
            base(feedback, new SubtitleSourceDownloaderV2())
        {

        }

        public override List<Subtitle> PerformSearch(string[] languageCodes)
        {
            EpisodeSearchQuery query = new EpisodeSearchQuery(SeriesName, SeasonIndex, EpisodeIndex);
            query.LanguageCodes = languageCodes;
                
            return Downloader.SearchSubtitles(query);
        }
    }
}
