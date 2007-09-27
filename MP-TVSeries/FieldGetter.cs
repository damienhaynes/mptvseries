using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WindowPlugins.GUITVSeries
{
    sealed class FieldGetter
    {
        const string Episode = "Episode";
        const string Season = "Season";
        const string Series = "Series";
        const char openTag = '<';
        const char closeTag = '>';
        const char typeFieldSeperator = '.';

        static string generalRegex = @"(?<=\" + openTag + @"{0}\.)(?<fieldtoParse>.*?)(?=\" + closeTag + ")";

        static Regex epParse = new Regex(String.Format(generalRegex, Episode), RegexOptions.Compiled | RegexOptions.Singleline);
        static Regex seasonParse = new Regex(String.Format(generalRegex, Season), RegexOptions.Compiled | RegexOptions.Singleline);
        static Regex seriesParse = new Regex(String.Format(generalRegex, Series), RegexOptions.Compiled | RegexOptions.Singleline);

        static string epIdentifier = String.Format("{0}{1}{2}", openTag, Episode, typeFieldSeperator);
        static string seasonIdentifier = String.Format("{0}{1}{2}", openTag, Season, typeFieldSeperator);
        static string seriesIdentifier = String.Format("{0}{1}{2}", openTag, Series, typeFieldSeperator);

        static List<formatingRule> formatingRules = new List<formatingRule>();
        static List<string> nonFormattingFields = new List<string>();

        static bool _splitFields = true; // not thread safe

        private FieldGetter() { }
        static FieldGetter()
        {
            formatingRule fr = null;

            // hide the episodesummary if option is set
            fr = new formatingRule(delegate(string value, string field, DBTable item)
                                   {
                                       DBEpisode e = item as DBEpisode;
                                       if (null == e) return false;
                                       return field == "<Episode.Summary>" &&
                                               DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary)
                                               && !e[DBOnlineEpisode.cWatched];
                                   },
                                   delegate(string value, string field, DBTable item)
                                   {
                                       return Translation._Hidden_to_prevent_spoilers_;
                                   });
            formatingRules.Add(fr);

            fr = new formatingRule("\\n", Environment.NewLine);
            formatingRules.Add(fr);

            fr = new formatingRule("''", "'");
            formatingRules.Add(fr);

            fr = new formatingRule("\\\"", "\"");
            formatingRules.Add(fr);

            fr = new formatingRule("\\'", "'");
            formatingRules.Add(fr);

            fr = new formatingRule(Translation.Season + @"\:{0,1}\s{0,2}0", Translation.specials, true);
            formatingRules.Add(fr);

            fr = new formatingRule(@"Season\:{0,1}\s{0,2}0", Translation.specials, true);
            formatingRules.Add(fr);

            fr = new formatingRule(@"(?<!\d)0x\d{1,3}", Translation.special + " ", true);
            formatingRules.Add(fr);

            // enable user to show 0/1 as Boolean (will show up as Yes/No)
            fr = new formatingRule("asBool(0)", Translation.No);
            formatingRules.Add(fr);

            fr = new formatingRule("asBool(1)", Translation.Yes);
            formatingRules.Add(fr);

            // want to ensure these show as is
            nonFormattingFields.Add("<Episode.EpisodeFilename>");
        }
        public enum Level
        {
            Series,
            Season,
            Episode,
        }

        static List<Level> getLevel(string what)
        {
            List<Level> levels = new List<Level>();
            if (what.Contains(epIdentifier)) levels.Add(Level.Episode);
            if (what.Contains(seasonIdentifier)) levels.Add(Level.Season);
            if (what.Contains(seriesIdentifier)) levels.Add(Level.Series);
            return levels;
        }

        static Level levelOfItem(DBTable item)
        {
            Type p = item.GetType();
            if (p == typeof(DBSeries)) return Level.Series;
            if (p == typeof(DBSeason)) return Level.Season;
            if (p == typeof(DBEpisode)) return Level.Episode;
            return Level.Series;
        }

        public static string resolveDynString(string what, DBTable item)
        {
            return resolveDynString(what, item, true);
        }
        public static string resolveDynString(string what, DBTable item, bool splitFields)
        {
            Level level = levelOfItem(item);
            string value = what;
            List<Level> whatLevels = getLevel(what);
            _splitFields = splitFields;
            // the item needs to be the type corresponding to the level (we require the item to match the indicated level)
            if (level == Level.Episode) // we can do everything
            {
                if (whatLevels.Contains(Level.Episode))
                    value = replaceEpisodeTags(item as DBEpisode, value);
                if (whatLevels.Contains(Level.Season))
                    value = replaceSeriesTags(item[DBEpisode.cSeriesID], value);
                if (whatLevels.Contains(Level.Series))
                    value = replaceSeriesTags(item[DBEpisode.cSeriesID], value);
            }
            else if (level == Level.Season && !whatLevels.Contains(Level.Episode)) // we can do season/series
            {
                if (whatLevels.Contains(Level.Season))
                    value = replaceSeasonTags(item as DBSeason, value);
                if (whatLevels.Contains(Level.Series))
                    value = replaceSeriesTags(item[DBSeason.cSeriesID], value);
            }
            else if (level == Level.Series && !whatLevels.Contains(Level.Episode) && !whatLevels.Contains(Level.Season)) // we can only do series
            {
                value = replaceSeriesTags(item as DBSeries, value);
            }

            if (nonFormattingFields.Contains(what)) return value;
            value = doFormatting(value, what, item);

            value = MathParser.mathParser.TryParse(value);
            return value;
        }

        static string replaceSeriesTags(int seriesID, string what)
        {
            // get the series (tries cache first and then the db)
            return replaceSeriesTags(Helper.getCorrespondingSeries(seriesID), what);
        }
        static string replaceSeriesTags(DBSeries s, string what)
        {
            if (s == null || what.Length < seriesIdentifier.Length) return what;
            return getValuesOfType(s, what, seriesParse, seriesIdentifier);
        }

        static string replaceSeasonTags(int seriesID, int seasonIndex, string what)
        {
            // get the series (tries cache first and then the db)
            return replaceSeasonTags(Helper.getCorrespondingSeason(seriesID, seasonIndex), what);
        }
        static string replaceSeasonTags(DBSeason s, string what)
        {
            if (s == null || what.Length < seasonIdentifier.Length) return what;
            return getValuesOfType(s, what, seasonParse, seasonIdentifier);
        }

        static string replaceEpisodeTags(DBEpisode s, string what)
        {
            if (s == null || what.Length < epIdentifier.Length) return what;
            return getValuesOfType(s, what, epParse, epIdentifier);
        }

        static string getValuesOfType(DBTable item, string what, Regex matchRegex, string Identifier)
        {
            string value = what;
            foreach (Match m in matchRegex.Matches(what))
            {
                string result = item[m.Value];
                if (_splitFields) result = result.Trim('|').Replace("|", ", ");
                value = value.Replace(Identifier + m.Value + ">", result);
            }
            return value;
        }

        static string doFormatting(string input, string fieldname, DBTable item)
        {
            foreach (formatingRule fr in formatingRules)
                if (fr.Format(input, fieldname, item))
                    input = fr.Result;
            return input;
        }
    }

    class formatingRule
    {
        public enum Type
        {
            simpleReplace,
            Regex,
            Custom
            //others ??
        }

        public delegate bool tester(string value, string field, DBTable item);
        public tester applies;
        public delegate string action(string value, string field, DBTable item);
        public action doAction;
        private Type _type = Type.Custom;
        string input = null;
        string field = null;
        string result = null;
        DBTable item = null;
        Regex regEx = null;

        public bool Applies { get { return applies(input, field, item); } }
        public Type TypeOf { get { return _type; } }
        public String Result { get { return result; } }

        public formatingRule(string replaceWhat, string replaceWith)
        {
            setUp(replaceWhat, replaceWith, false);
        }

        public formatingRule(string replaceWhat, string replaceWith, bool isRegex)
        {
            setUp(replaceWhat, replaceWith, isRegex);
        }

        public formatingRule(tester Tester, action Action)
        {
            this.applies = Tester;
            this.doAction = Action;
        }

        void setUp(string replaceWhat, string replaceWith, bool isRegex)
        {
            if (!isRegex)
            {
                _type = Type.simpleReplace;
                applies = delegate { return this.input.Contains(replaceWhat); };
                doAction = delegate { return input.Replace(replaceWhat, replaceWith); };
            }
            else
            {
                _type = Type.Regex;
                regEx = new Regex(replaceWhat, RegexOptions.Compiled);
                if (regEx != null)
                {
                    applies = delegate { return regEx.IsMatch(this.input); };
                    doAction = delegate
                               {
                                   string res = input;
                                   foreach (Match m in regEx.Matches(this.input))
                                       res = res.Replace(m.Value, replaceWith);
                                   return res;
                               };
                }
                else applies = delegate { return false; };
            }
        }

        public bool Format(string input, string field, DBTable item)
        {
            this.input = input;
            this.field = field;
            this.item = item;
            if (Applies)
            {
                result = doAction(input, field, item);
                return true;
            }
            return false;
        }
    }
}