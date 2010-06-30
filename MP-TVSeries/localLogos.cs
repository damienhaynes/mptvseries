#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion


using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MediaPortal.GUI.Library;
using System.Globalization;

namespace WindowPlugins.GUITVSeries
{
    class localLogos
    {
        const string optionName = "logoConfig";
        const string entriesSplit = "<next>";
        public const string condSplit = ";-;";
        static string pathfortmpfile = Settings.GetPath(Settings.Path.thumbs);
        static string tmpFile = @"tmpLogos.png";
        static List<string> entries = new List<string>();
        static Dictionary<int, List<Level>> entriesValidForInfo = new Dictionary<int, List<Level>>();
        static Dictionary<int, List<string>> splitConditions = new Dictionary<int, List<string>>();
        static DBEpisode tmpEp;
        static DBSeason tmpSeason;
        static DBSeries tmpSeries;
        static string groupedByInfo = string.Empty;
        static string groupedItemType = string.Empty;
        static string groupedField = string.Empty;
        static string groupedSelection = string.Empty;
        static bool entriesInMemory = false;
        static NumberFormatInfo provider = new NumberFormatInfo();
        static public bool appendEpImage = true;
        static string lastResult = string.Empty;
        static bool lastWasCached = false;

        public static string RestrictionSeriesList = "<Restrict:Series>";
        public static string RestrictionSeasonList = "<Restrict:Season>";
        public static string RestrictionEpisodeList = "<Restrict:Episode>";
        public static string RestrictionGroupList = "<Restrict:Group>";

        enum Level
        {
            Series,
            Season,
            Episode,
            Group
        }

        static localLogos()
        {
            provider.NumberDecimalSeparator = "."; // because mediainfo
        }

        private localLogos() { } // prevent instances

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
            entriesValidForInfo.Clear();
            splitConditions.Clear();
            if (all.Length > 0)
                entries = new List<string>(Regex.Split(all, entriesSplit));
            entriesInMemory = true;
            if (entries.Count == 0)
                MPTVSeriesLog.Write("No LogoRules found!");
            else
            {
                // we calculate relevances once here so we can avoid doing it everytime
                // we also split them here so we can avoid doing this later
                for(int i=0; i<entries.Count; i++)
                {
                    List<Level> levels = new List<Level>();
                    if (isRelevant(entries[i], Level.Series)) levels.Add(Level.Series);
                    if (isRelevant(entries[i], Level.Season)) levels.Add(Level.Season);
                    if (isRelevant(entries[i], Level.Episode)) levels.Add(Level.Episode);
                    //if (isRelevant(entries[i], Level.Group)) levels.Add(Level.Group);
                    entriesValidForInfo.Add(i, levels);

                    splitConditions.Add(i, new List<string>(Regex.Split(entries[i], localLogos.condSplit)));
                }
            }
            return entries;
        }

        static string getLogos(Level level, int imgHeight, int imgWidth, ref List<string> logosForBuilding)
        {
            return getLogos(level, imgHeight, imgWidth, false, ref logosForBuilding);
        }

        public static string getFirstEpLogo(DBEpisode ep)
        {
            return getLogos(ref ep, 1, 1, true); // size doesnt matter since it will return the original and not draw the image
        }

        public static string getLogos(string groupedBy, string selection, int imgHeight, int imgWidth)
        {
            // TODO: make possible for groups to request logos
            groupedByInfo = groupedBy;
            groupedItemType = groupedBy.Substring(1, groupedBy.IndexOf(".") - 1);
            groupedField = groupedBy.Substring(groupedItemType.Length + 2);
            groupedField = groupedField.Substring(0, groupedField.Length - 1);
            groupedSelection = selection;
            List<string> cache_not_yet = null;
            return getLogos(Level.Group, imgHeight, imgWidth, true, ref cache_not_yet); // they can logically only have one logo (we don't support hierarchical logos, eg network - genre...genre level will not have network logos)
        }


        public static string getLogos(ref DBEpisode ep, int imgHeight, int imgWidth, bool firstOnly)
        {
            if (ep == null) return null;
            lastWasCached = false;
            DBEpisode inCache = !Settings.isConfig ? cache.getEpisode(ep[DBEpisode.cSeriesID], ep[DBEpisode.cSeasonIndex], ep[DBOnlineEpisode.cEpisodeIndex]) : null;
            tmpEp = inCache == null || inCache[DBEpisode.cFilename] != ep[DBEpisode.cFilename] ? ep : inCache;
            // zeflash: since we use an image allocator we can't use a cache for the episode image anymore - so always call getLogos
            tmpEp.cachedLogoResults = null; // have to redo caching here
            lastResult = getLogos(Level.Episode, imgHeight, imgWidth, firstOnly, ref tmpEp.cachedLogoResults);

            if(!lastWasCached) cache.addChangeEpisode(tmpEp);
            return lastResult;
        }

        public static string getLogos(ref DBEpisode ep, int imgHeight, int imgWidth)
        {
            return getLogos(ref ep, imgHeight, imgWidth, false);           
        }

        public static string getLogos(ref DBSeason season, int imgHeight, int imgWidth)
        {
            if (season == null) return null;
            DBSeason inCache = !Settings.isConfig ? cache.getSeason(season[DBSeason.cSeriesID], season[DBSeason.cIndex]) : null;
            tmpSeason = inCache == null ? season : inCache;
            lastResult = getLogos(Level.Season, imgHeight, imgWidth, ref tmpSeason.cachedLogoResults);
            if (!lastWasCached) cache.addChangeSeason(tmpSeason);
            return lastResult;
        }

        public static string getLogos(ref DBSeries series, int imgHeight, int imgWidth)
        {
            if (series == null) return null;
            DBSeries inCache = !Settings.isConfig ? cache.getSeries(series[DBSeries.cID]) : null;
            tmpSeries = inCache == null ? series : inCache;
            lastResult = getLogos(Level.Series, imgHeight, imgWidth, ref tmpSeries.cachedLogoResults);
            if (!lastWasCached) cache.addChangeSeries(tmpSeries);
            return lastResult;
        }

        static string getLogos(Level level, int imgHeight, int imgWidth, bool firstOnly, ref List<string> logosForBuilding)
        {
            string res = work(level, imgHeight, imgWidth, firstOnly, ref logosForBuilding);
            if (!Settings.isConfig && res != null && res.Length > 0 && res[0] != '[')
            {
                if (tmpEp != null && tmpEp.Image == res)
                    return ImageAllocator.GetOtherImage(res, new Size(), false); // don't resize by allocator
                else
                    return ImageAllocator.GetOtherImage(res, new Size(), true); // don't resize by allocator

            }
            else return res; // we build the memimage already
        }

        static string work(Level level, int imgHeight, int imgWidth, bool firstOnly, ref List<string> logosForBuilding)
        {
            if (!entriesInMemory) getFromDB();
            bool epImgAppended = false;
            // downloaded episodeimage into logos (after getFromDB)
            // also for listview want to get them regardless, so if firstonly is set this is good too
            if ((appendEpImage || firstOnly) && level == Level.Episode && tmpEp.Image.Length > 0 && System.IO.File.Exists(tmpEp.Image))
            {
                if (firstOnly)
                {
                    return tmpEp.Image; // takes precedence (should it?)
                }
                epImgAppended = true;
            }

            if ((!(lastWasCached = logosForBuilding != null)) || Settings.isConfig) // means it was tested before, so we don't even test anymore
            {
                logosForBuilding = new List<string>(); // null means hasn't been tested yet
                if (epImgAppended) logosForBuilding.Add(tmpEp.Image);
                if (entries.Count == 0 && logosForBuilding.Count == 0) return string.Empty; // no rules exist
                MPTVSeriesLog.Write("Testing logos for item of type ", level.ToString(), MPTVSeriesLog.LogLevel.Debug);

                for (int i = 0; i < entries.Count; i++)
                {
                    try
                    {
                        if (entriesValidForInfo[i].Contains(level) || (level == Level.Group && isRelevant(entries[i], level))) // precalculated relevances (can't do for groups though)
                        {
                            MPTVSeriesLog.Write("Logo-Rule is relevant....testing: ", entries[i], MPTVSeriesLog.LogLevel.Debug);
                            List<string> conditions = splitConditions[i];
                            // resolve dnyamic image and check if logo img exists
                            List<string> filenames = Helper.filterExistingFiles(getDynamicFileName(conditions[0], level));

                            // check if the condition is met
                            // each image may only exist once                            
                            if (filenames.Count > 0 && !logosForBuilding.Contains(conditions[0]) && condIsTrue(conditions, entries[i], level))
                            {
                                if (firstOnly)
                                {
                                    return filenames[0]; // if we only need the first then we just return the original here
                                }
                                else
                                {
                                    //logosForBuilding.AddRange(filenames);
                                    // Strange things happen when in playlist view and using the mouse, so dont add duplicates that get generated
                                    // Need to get to the bottom of the issue!
                                    foreach (string filename in filenames)
                                    {
                                        if (!logosForBuilding.Contains(filename))
                                            logosForBuilding.Add(filename);
                                    }                                    
                                }
                            }
                        }
                        //else MPTVSeriesLog.Write("Logo-Rule is not relevant for current item, aborting!", MPTVSeriesLog.LogLevel.Debug);
                    }
                    catch (Exception ex)
                    {
                        MPTVSeriesLog.Write("Logo Rule crashed: " + entries[i] + " - " + ex.Message);
                    }
                }
            }

            return buildLogoImage(logosForBuilding, firstOnly, imgWidth, imgHeight);
        }

        static string buildLogoImage(List<string> logosForBuilding, bool firstOnly, int imgWidth, int imgHeight)
        {
            try
            {
                if (logosForBuilding.Count == 1 || (firstOnly && logosForBuilding.Count > 0)) return logosForBuilding[0];
                else if (logosForBuilding.Count > 1)
                {
                    tmpFile = string.Empty;
                    foreach (string logo in logosForBuilding)
                        tmpFile += System.IO.Path.GetFileNameWithoutExtension(logo);
                    tmpFile = Helper.PathCombine(pathfortmpfile, "TVSeriesDynLogo" + tmpFile + ".png");

                    Bitmap b = new Bitmap(imgWidth, imgHeight);
                    Image img = b;
                    Graphics g = Graphics.FromImage(img);
                    appendLogos(logosForBuilding, ref g, imgHeight, imgWidth);
                    if (Settings.isConfig)
                    {
                        b.Save(tmpFile);
                        return tmpFile;
                    }
                    else
                        return ImageAllocator.GetOtherImage(b, tmpFile, new Size(), true); // don't resize in allocator
                }
                else return string.Empty;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("The Logo Building Engine generated an error: " + ex.Message);
                return string.Empty;
            }
        }

        static void appendLogos(List<string> logosForBuilding, ref Graphics g, int totalHeight, int totalWidth)
        {
            int noImgs = logosForBuilding.Count;
            List<Image> imgs = new List<Image>();
            List<Size> imgSizes = new List<Size>();
            int spacer = 5;
            int checkWidth = 0;
            // step one: get all sizes (not all logos are obviously square) and scale them to fit vertically
            Image single = null;
            float scale = 0, totalHeightf = (float)totalHeight;
            Size tmp = default(Size);
            int x_pos = 0;
            for (int i = 0; i < logosForBuilding.Count; i++)
            {
                try
                {                    
                    single = ImageAllocator.LoadImageFastFromFile(logosForBuilding[i]);
                }
                catch (Exception)
                {
                    MPTVSeriesLog.Write("Could not load Image file... " + logosForBuilding[i]);
                    return;
                }
                scale = totalHeightf / (float)single.Size.Height;
                tmp = new Size((int)(single.Width * scale), (int)(single.Height * scale));
                checkWidth += tmp.Width;
                imgSizes.Add(tmp);
                imgs.Add(single);
            }
            // step two: check if we are too big horizontally and if so scale again
            checkWidth += imgSizes.Count * spacer;
            if (checkWidth > totalWidth)
            {
                scale = (float)checkWidth / (float)totalWidth;
                for (int i = 0; i < imgSizes.Count; i++)
                {
                    imgSizes[i] = new Size((int)(imgSizes[i].Width / scale), (int)(imgSizes[i].Height / scale));
                }
            }
            // step three: finally draw them
            for (int i = 0; i < imgs.Count; i++)
            {
                g.DrawImage(imgs[i], x_pos, totalHeight - imgSizes[i].Height, imgSizes[i].Width, imgSizes[i].Height);
                x_pos += imgSizes[i].Width + spacer;
            }
        }

        static bool condIsTrue(List<string> conditions, string cond, Level level)
        {
            bool[] results = new bool[]{false, false, false};
            bool cancel = false;
            for (int i = 0, j=0; i < 3; i++, j=i<<2)
            {                
                MPTVSeriesLog.Write("Testing Loop:", i, MPTVSeriesLog.LogLevel.Debug);
                string what = conditions[j + 1];
                string compare = conditions[j + 3];
                if (!getFieldValues(what, out what, level)) return false;
                if (!getFieldValues(compare, out compare, level)) return false;

                results[i] = Condition.evaluate(what,
                                 conditions[j + 2],
                                 compare, out cancel, provider);

                MPTVSeriesLog.Write("Test Result: ", results[i].ToString(), MPTVSeriesLog.LogLevel.Debug);
                if (cancel) return true; // the first empty condition (what + value = empty) means no other conds can follow, we exit

                if (i < 2)
                {
                    if (!results[i] && conditions[j + 4] == "AND") // result is false and next link is and -> everything is wrong
                    {
                        MPTVSeriesLog.Write("No addition Test Loop needed, reason: next link = AND and current result was FALSE", MPTVSeriesLog.LogLevel.Debug);
                        return false;
                    }
                    if (results[i] && conditions[j + 4] == "OR") // everything has to be true
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

        static List<string> getDynamicFileName(string dynfilename, Level level)
        {
            int dnyStart = 0;
            List<string> result = new List<string>();
            if (!dynfilename.Contains("<"))
            {
                // not dynamic
                result.Add(Helper.getCleanAbsolutePath(dynfilename));
                return result;
            }
            #pragma warning disable 0642
            else if ((dnyStart = dynfilename.IndexOf("<Episode.")) > -1) ;
            else if ((dnyStart = dynfilename.IndexOf("<Series.")) > -1) ;
            else if ((dnyStart = dynfilename.IndexOf("<Season.")) > -1) ;
            #pragma warning restore 0642
            else
            {
                // no '<' but none of the recognized? that is a wrong entry
                return result;
            }
            //MPTVSeriesLog.Write("dynamic Filename detected..trying to resolve");
            string fieldToGet = string.Empty;
            string value = string.Empty;
            fieldToGet = dynfilename.Substring(dnyStart, dynfilename.IndexOf('>', dnyStart) - dnyStart + 1);
            if (getFieldValues(fieldToGet, out value, level))
            {
                // for genres/actors we need to split dynamic filenames again
                string[] vals = DBOnlineSeries.splitField(value);                                

                for (int i = 0; i < vals.Length; i++)
                {                    
                    // recursive so we support multiple dyn fields in filename
                    if (vals[i] != fieldToGet)
                        result.AddRange(getDynamicFileName(dynfilename.Replace(fieldToGet, vals[i].Trim()), level));
                }
                return result;
            }
            else
            {
                return new List<string>(new string[] { string.Empty }); // something went wrong
            }
        }

        static bool getFieldValues(string what, out string value, Level level)
        {            
            value = string.Empty;
            if (what.Trim().Length == 0) return true; // just skip it
            if (level == Level.Group)
            {
                value = groupedSelection.ToString(); // the only thing we can do
            }
            else
            {
                DBTable item = null;
                switch(level)
                {
                    case Level.Episode: item = tmpEp; break;
                    case Level.Season: item = tmpSeason; break;
                    case Level.Series: item = tmpSeries; break;
                }
                value = FieldGetter.resolveDynString(what, item, false);            
            }             
            return true;
        }

        static bool isRelevant(string field, Level level)
        {
            if(field.Contains("<Restrict:"))
            {
                Level restrLevel = Level.Series;
                if (field.Contains(RestrictionSeriesList)) restrLevel = Level.Series;
                else if (field.Contains(RestrictionSeasonList)) restrLevel = Level.Season;
                else if (field.Contains(RestrictionEpisodeList)) restrLevel = Level.Episode;
                else if (field.Contains(RestrictionGroupList)) restrLevel = Level.Group;

                if (level != restrLevel) return false;
            }

            if (level == Level.Series && (field.Contains("<Season."))) return false;
            if ((level == Level.Series || level == Level.Season) && field.Contains("<Episode.")) return false;
            if (level == Level.Group)
            {
                switch(groupedItemType)
                {
                    case "Series":
                        if (field.Contains("<E") || field.Contains("<Sea")) return false;
                        break;
                    case "Episode":
                        if (field.Contains("<S")) return false;
                        break;
                    case "Season":
                        if (field.Contains("<E") || field.Contains("<Ser")) return false;
                        break;
                }
                // there can be multiple same item entries (eg. <Series.Network> = abc or <Series.Network> = nbc
                return !field.Replace(groupedByInfo, string.Empty).Contains("<" + groupedItemType + ".");
            }
            return true;
        }


        public static void cleanUP() // there is no static destructor in .NET, so this has to be called explicitally
        {
            // clean up all dynLogoFiles
            MPTVSeriesLog.Write("Cleaning up cached, generated Logos");
            foreach (string file in System.IO.Directory.GetFiles(pathfortmpfile, "TVSeriesDynLogo*"))
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("Error: Could not delete temporary Logo File " + file, ex.Message, MPTVSeriesLog.LogLevel.Normal);
                }
            }
        }
    }

    public class Condition
    {
        private Condition() { }
        public static bool evaluate(string what, string type, string value, out bool cancel, NumberFormatInfo provider)
        {
            double testf = 0, test1f = 0;
            cancel = false;
            if (what.Length == 0 && value.Length == 0)
            {
                cancel = true; // on the first empty condition break
                return true;
            }
            if (type.Contains("<") || type.Contains(">"))
            {
                try
                {
                    if (what.Length == 0) what = "0";
                    if (value.Length == 0) value = "0";
                    testf = System.Convert.ToDouble(what, provider);
                    test1f = System.Convert.ToDouble(value, provider);
                }
                catch (Exception)
                {
                    MPTVSeriesLog.Write("Error in LogoDefinition: only numerical values can be compared with modes </<=/>/>=");
                    MPTVSeriesLog.Write("Values were: " + what + " and " + value);
                    return false;
                }
            }

            switch (type)
            {
                case "=":
                    return (what == value);
                case "!=":
                    return (what != value);
                case ">":
                    return (testf > test1f);
                case ">=":
                    return (testf >= test1f);
                case "<":
                    return (testf < test1f);
                case "<=":
                    return (testf <= test1f);
                case "contains":
                    return what.ToLower().Contains(value.ToLower());
                case "!contains":
                    return !what.ToLower().Contains(value.ToLower());
            }
            return false;
        }

    }
}
