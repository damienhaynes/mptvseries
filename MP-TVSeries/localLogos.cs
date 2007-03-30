using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MediaPortal.GUI.Library;

namespace WindowPlugins.GUITVSeries
{
    class localLogos
    {
        const string optionName = "logoConfig";
        const string entriesSplit = "<next>";
        public const string condSplit = ";-;";
        static string pathfortmpfile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        static string tmpFile = @"tmpLogos.png";
        static List<string> entries = new List<string>();
        static DBEpisode tmpEp;
        static DBSeason tmpSeason;
        static DBSeries tmpSeries;
        static string groupedByInfo = string.Empty;
        static bool entriesInMemory = false;
        enum Level
        {
            Series,
            Season,
            Episode,
            Group
        }

        static localLogos()
        {
           pathfortmpfile += @"\thumbs\";
        }

        public static void saveToDB(List<string> entries)
        {
            StringBuilder logoB = new StringBuilder();
            foreach (string entry in entries)
                logoB.Append(entry).Append(entriesSplit);
            if(logoB.Length > entriesSplit.Length)
                logoB.Remove(logoB.Length - entriesSplit.Length, entriesSplit.Length);
            DBOption.SetOptions(optionName, logoB.ToString());
            entriesInMemory = false;
        }

        public static List<string> getFromDB()
        {
            string all = DBOption.GetOptions(optionName);
            entries.Clear();
            if(all != string.Empty)
                entries = new List<string>(Regex.Split(all, entriesSplit));
            entriesInMemory = true;
            if (entries.Count == 0)
                MPTVSeriesLog.Write("No LogoRules found!");
            return entries;
        }

        static string getLogos(Level level, int imgHeight, int imgWidth)
        {
            return getLogos(level, imgHeight, imgWidth, false);
        }

        public static string getFirstEpLogo(DBEpisode ep)
        {
            return getLogos(ref ep, 1, 1, true); // size doesnt matter since it will return the original and not draw the image
        }

        public static string getLogos(string groupedBy, string selection, int imgHeight, int imgWidth)
        {
            // TODO: make possible for groups to request logos
            groupedByInfo = groupedBy;
            return string.Empty;
        }

        public static string getLogos(ref DBEpisode ep, int imgHeight, int imgWidth, bool firstOnly)
        {
            perfana.start();
            tmpEp = ep;
            if (tmpEp == null) return null;
            return getLogos(Level.Episode, imgHeight, imgWidth, firstOnly);
        }

        public static string getLogos(ref DBEpisode ep, int imgHeight, int imgWidth)
        {
            return getLogos(ref ep, imgHeight, imgWidth, false);
        }

        public static string getLogos(ref DBSeason season, int imgHeight, int imgWidth)
        {
            perfana.start();
            tmpSeason = season;
            if (tmpSeason == null) return null;
            return getLogos(Level.Season, imgHeight, imgWidth);
        }

        public static string getLogos(ref DBSeries series, int imgHeight, int imgWidth)
        {
            tmpSeries = series;
            if (tmpSeries == null) return null;
            return getLogos(Level.Series, imgHeight, imgWidth);
        }

        static string getLogos(Level level, int imgHeight, int imgWidth, bool firstOnly)
        {
            if (!entriesInMemory) getFromDB();
            if (entries.Count == 0) return string.Empty; // no rules exist
            MPTVSeriesLog.Write("Testing logos for item of type " + level.ToString(), MPTVSeriesLog.LogLevel.Debug);
            List<string> logosForBuilding = new List<string>();
            bool debugResult = false;
            bool debugResult1 = false;

            for (int i = 0; i < entries.Count; i++)
            {
                if (isRelevant(entries[i], level))
                {
                    MPTVSeriesLog.Write("Logo-Rule is relevant....testing: ", entries[i], MPTVSeriesLog.LogLevel.Debug);
                    List<string> conditions = new List<string>(Regex.Split(entries[i], localLogos.condSplit));

                    // resolve dnyamic image and check if logo img exists
                    string filename = getDynamicFileName(conditions[0], level);
                    if (!System.IO.File.Exists(filename))
                    {
                        MPTVSeriesLog.Write("This Logofile does not exist..skipping: " + filename, MPTVSeriesLog.LogLevel.Normal);

                    }

                    // check if the condition is met
                    // each image may only exist once
                    else if (!(debugResult1 = logosForBuilding.Contains(conditions[0])) && (debugResult = condIsTrue(conditions, entries[i], level)))
                    {
                        if (firstOnly) return filename; // if we only need the first then we just return the original here
                        else logosForBuilding.Add(filename);
                    }
                }
                else MPTVSeriesLog.Write("Logo-Rule is not relevant, aborting (you cannot go \"down\" in hierarchy (Series - Season - Episode)!", MPTVSeriesLog.LogLevel.Debug);

                MPTVSeriesLog.Write("Image needs to be displayed: " + (!debugResult1 && debugResult).ToString(), MPTVSeriesLog.LogLevel.Debug);
            }

            if (logosForBuilding.Count == 1) return logosForBuilding[0];
            else if (logosForBuilding.Count > 1)
            {
                tmpFile = string.Empty;
                foreach (string logo in logosForBuilding)
                    tmpFile += System.IO.Path.GetFileNameWithoutExtension(logo);
                tmpFile = pathfortmpfile + "TVSeriesDynLogo" + tmpFile + ".png";
                if (System.IO.File.Exists(tmpFile))
                    return tmpFile;

                Bitmap b = new Bitmap(imgWidth, imgHeight);
                Image img = b;
                Graphics g = Graphics.FromImage(img);
                appendLogos(logosForBuilding, ref g, imgHeight, imgWidth);
                try
                {
                    b.Save(tmpFile, System.Drawing.Imaging.ImageFormat.Png);
                    return tmpFile;
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(tmpFile)) return tmpFile; // if the tmpfile exists return it regardless
                    return string.Empty;
                }
            }
            else return string.Empty;
        }

        static DBSeries getCorrespondingSeries(int id)
        {
            SQLCondition cond = new SQLCondition();
            cond.Add(new DBSeries(), DBSeries.cID, id, SQLConditionType.Equal);
            List<DBSeries> tmpSeries = DBSeries.Get(cond);

            foreach (DBSeries series in tmpSeries) // should only be one!
                if (series[DBSeries.cID] == id)
                {
                    return series;
                }
            return null;
        }

        static DBSeason getCorrespondingSeason(int seriesID, int seasonIndex)
        {
            List<DBSeason> tmpSeasons = DBSeason.Get(seriesID, false, false, false);
            foreach (DBSeason season in tmpSeasons)
                if (season[DBSeason.cIndex] == seasonIndex)
                {
                    return season;
                }
            return null;
        }

        static void appendLogos(List<string> logosForBuilding, ref Graphics g, int totalHeight, int totalWidth)
        {
            int noImgs = logosForBuilding.Count;
            List<Image> imgs = new List<Image>();
            List<Size> imgSizes = new List<Size>();
            int spacer = 5;
            int checkWidth = 0;

            // step one: get all sizes (not all logos are obviously square) and scale them to fit vertically
            for (int i = 0; i < logosForBuilding.Count; i++)
            {
                Image single = null;
                try
                {
                     single = Image.FromFile(logosForBuilding[i]);
                }
                catch (Exception)
                {
                    MPTVSeriesLog.Write("Could not load Image file... " + logosForBuilding[i]);
                    return;
                }
                float scale = (float)totalHeight / (float)single.Size.Height;
                Size tmp = new Size((int)(single.Width * scale), (int)(single.Height * scale));
                checkWidth += tmp.Width;
                imgSizes.Add(tmp);
                imgs.Add(single);
            }

            // step two: check if we are too big horizontally and if so scale again
            checkWidth += imgSizes.Count * spacer;
            if (checkWidth > totalWidth)
            {
                float scale = (float)checkWidth / (float)totalWidth;
                for (int i = 0; i < imgSizes.Count; i++)
                {
                    imgSizes[i] = new Size((int)(imgSizes[i].Width / scale), (int)(imgSizes[i].Height / scale));
                }
            }
            int x_pos = 0;
            // step three: finally draw them
            for (int i = 0; i < imgs.Count; i++)
            {
                g.DrawImage(imgs[i], x_pos, totalHeight - imgSizes[i].Height, imgSizes[i].Width, imgSizes[i].Height);
                x_pos += imgSizes[i].Width + spacer;
            }

        }

        static bool condIsTrue(List<string> conditions, string cond, Level level)
        {
            conditions.Remove(conditions[0]); // have to get rid of the filename
            bool[] results = new bool[]{false, false, false};
            for (int i = 0; i < 3; i++)
            {
                MPTVSeriesLog.Write("Testing Loop:" + i.ToString(), MPTVSeriesLog.LogLevel.Debug);
                string what = conditions[i * 4];
                if (!getFieldValues(what, out what, level)) return false;

                if (what == string.Empty && conditions[i * 4 + 2].Trim() != string.Empty) return false;
                results[i] = singleCondIsTrue(what,
                                 conditions[i * 4 + 1],
                                 conditions[i * 4 + 2].Trim());

                MPTVSeriesLog.Write("Test Result: " + results[i].ToString(), MPTVSeriesLog.LogLevel.Debug);

                if (i < 2)
                {
                    if (!results[i] && conditions[i * 4 + 3] == "AND") // result is false and next link is and -> everything is wrong
                    {
                        MPTVSeriesLog.Write("No addition Test Loop needed, reason: next link = AND and current result was FALSE", MPTVSeriesLog.LogLevel.Debug);
                        return false;
                    }
                    if (results[i] && conditions[i * 4 + 3] == "OR") // everything has to be true
                    {
                        MPTVSeriesLog.Write("No addition Test Loop needed, reason: next link = OR and current result was TRUE", MPTVSeriesLog.LogLevel.Debug);
                        return true;
                    }
                    // otherwise we have to keep checking
                }
                else
                {
                    // we're at the last one, which means this one has to be true or we would have already exited out
                    return results[i];
                }
                                 
            }
            return false;
        }

        static string getDynamicFileName(string dynfilename, Level level)
        {
            int dnyStart = 0;
            if (dynfilename.Contains("<Series."))
            {
                dnyStart = dynfilename.IndexOf("<Series.");
            }
            else if (dynfilename.Contains("<Season."))
            {
                dnyStart = dynfilename.IndexOf("<Season.");
            }
            else if (dynfilename.Contains("<Episode."))
            {
                dnyStart = dynfilename.IndexOf("<Episode.");
            }
            else
            {
                // not dynamic
                return dynfilename;
            }
            //MPTVSeriesLog.Write("dynamic Filename detected..trying to resolve");
            string fieldToGet = string.Empty;
            string value = string.Empty;
            fieldToGet = dynfilename.Substring(dnyStart, dynfilename.IndexOf('>', dnyStart) - dnyStart + 1);
            if (getFieldValues(fieldToGet, out value, level))
                return getDynamicFileName(dynfilename.Replace(fieldToGet, value),level); // recursive so we support multiple dyn fields in filename
            else
                return dynfilename; // something went wrong
        }

        static bool getFieldValues(string what, out string value, Level level)
        {
            value = string.Empty;
            if (what == string.Empty) return true; // just skip it
            try
            {
                if (what.Contains("Episode"))
                {
                    // tmpEP always has to exists or the isrelevant check would have already failed
                    value = tmpEp[what.Replace("<Episode.", "").Replace(">", "").Trim()];
                }
                else if (what.Contains("Season"))
                {
                    if (level == Level.Episode) // means we might have to get the season object for this episode
                    {
                        // get the season object if needed (either null, or not the one we need), otherwise dont get it again
                        if (tmpSeason == null ||
                            tmpSeason[DBSeason.cSeriesID] != tmpEp[DBEpisode.cSeriesID] ||
                            tmpSeason[DBSeason.cIndex] != tmpEp[DBEpisode.cSeasonIndex])
                        {
                            tmpSeason = getCorrespondingSeason(tmpEp[DBEpisode.cSeriesID], tmpEp[DBEpisode.cSeasonIndex]);
                        }
                        //else MPTVSeriesLog.Write("SeasonObject was cached - optimisation was good!");
                    }
                    value = tmpSeason[what.Replace("<Season.", "").Replace(">", "").Trim()];
                }
                else if (what.Contains("Series"))
                {
                    if (level != Level.Series) // means we might have to get the series object for this episode/season
                    {
                        int seriesID = level == Level.Episode ? tmpEp[DBEpisode.cSeriesID] : tmpSeason[DBSeason.cSeriesID];
                        if (tmpSeries == null || tmpSeries[DBSeries.cID] != seriesID)
                            tmpSeries = getCorrespondingSeries(seriesID);
                        //else MPTVSeriesLog.Write("SeriesObject was cached - optimisation was good!");
                    }
                    value = tmpSeries[what.Replace("<Series.", "").Replace(">", "").Trim()];
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        static bool isRelevant(string field, Level level)
        {
            if (level == Level.Series && field.Contains("<Season.")) return false;
            if ((level == Level.Series || level == Level.Season) && field.Contains("<Episode.")) return false;
            return true;
        }

        static bool singleCondIsTrue(string what, string type, string value)
        {
            double testf = 0, test1f = 0;
            if (type.Contains("<") || type.Contains(">"))
            {
                try
                {
                    testf = System.Convert.ToDouble(what);
                    test1f = System.Convert.ToDouble(value);
                }
                catch (Exception)
                {
                    MPTVSeriesLog.Write("Error in LogoDefinition: only numerical values can be compared with modes </<=/>/>=");
                    return false;
                }
            }

            switch (type)
            {
                case "=":
                    return (what == value);
                    break;
                case "!=":
                    return (what != value);
                    break;
                case ">":
                    return (testf > test1f);
                    break;
                case ">=":
                    return (testf >= test1f);
                    break;
                case "<":
                    return (testf < test1f);
                    break;
                case "<=":
                    return (testf <= test1f);
                    break;
                case "contains":
                    return what.ToLower().Contains(value.ToLower());
                    break;
                case "!contains":
                    return !what.ToLower().Contains(value.ToLower());
                    break;
            }
            return false;
        }

        ~localLogos()
        {
            // clean up all dynLogoFiles
            MPTVSeriesLog.Write("Cleaning up cached, generated Logos");
            foreach (string file in System.IO.Directory.GetFiles(pathfortmpfile, "TVSeriesDynLogo*.png"))
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("Error: Could not delete temporary Logo File " + file, e.Message, MPTVSeriesLog.LogLevel.Normal);
                }
            }
        }
    }
    class perfana
    {
        static DateTime starttime = new DateTime();
        public static void start()
        {
            starttime = DateTime.Now;
        }

        public static string measorFromStart()
        {
            if (starttime.Equals(default(DateTime))) return " - cannot calculate, perfana.start() was not called!";
            TimeSpan t = DateTime.Now - starttime;
            starttime = new DateTime();
            return t.TotalMilliseconds.ToString();
        }
    }
}
