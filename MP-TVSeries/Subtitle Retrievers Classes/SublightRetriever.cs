using System.Collections.Generic;
using SubtitleDownloader.Core;
using SubtitleDownloader.Implementations.Sublight;
using WindowPlugins.GUITVSeries.Feedback;

namespace WindowPlugins.GUITVSeries.Subtitles
{
    public class SublightRetriever : BaseSubtitleRetriever
    {
        public SublightRetriever(IFeedback feedback) : 
            base(feedback, new SublightDownloader())
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
