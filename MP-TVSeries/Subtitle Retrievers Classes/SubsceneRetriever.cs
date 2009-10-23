using System.Collections.Generic;
using SubtitleDownloader.Core;
using SubtitleDownloader.Implementations.Subscene;
using WindowPlugins.GUITVSeries.Feedback;

namespace WindowPlugins.GUITVSeries.Subtitles
{
    public class SubsceneRetriever : BaseSubtitleRetriever
    {
        public SubsceneRetriever(IFeedback feedback) : 
            base(feedback, new SubsceneDownloader())
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
