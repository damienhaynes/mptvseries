using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    public class UserInputResults
    {
        public enum SeriesAction
        {
            Skip,
            IgnoreAlways,
            Approve
        }

        public IList<parseResult> ParseResults { get; set; }
        public Dictionary<string, UserInputResultSeriesActionPair> UserChosenSeries { get; set; }

        public UserInputResults(IList<parseResult> parseResults, Dictionary<string, UserInputResultSeriesActionPair> UserChosenSeries)
        {
            this.ParseResults = parseResults;
            this.UserChosenSeries = UserChosenSeries;
        }
    }

    public class UserInputResultSeriesActionPair
    {
        public UserInputResults.SeriesAction RequestedAction { get; set; }
        public DBOnlineSeries ChosenSeries { get; set; }

        public UserInputResultSeriesActionPair(UserInputResults.SeriesAction RequestedAction, DBOnlineSeries series)
        {
            this.RequestedAction = RequestedAction;
            this.ChosenSeries = series;
        }
    }
}
