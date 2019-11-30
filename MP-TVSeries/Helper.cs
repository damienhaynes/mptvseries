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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Ripper;
using MediaPortal.Util;

namespace WindowPlugins.GUITVSeries
{
    #region String Extension Methods
    public static class StringExtensions
    {        
        public static bool IsNumerical(this string number)
        {
            double isNumber = 0;
            return System.Double.TryParse(number, out isNumber);
        }

        /// <summary>
        /// ASCII chars that are considered "special" in the context of CleanStringOfSpecialChars
        /// </summary>
        static int[] specialCharsFromTo = new int[] { 0, 31, 47, 58, 64, 91, 96, 123, 125, 127 };
        /// <summary>
        /// Cleans a string of Punctuation and other Special Characters (removes them)
        /// Leaves ASCII Chars: 0-9, a-z, A-Z, space
        /// Leaves non-ASCII Chars
        /// </summary>
        /// <param name="input">The string to clean</param>
        /// <returns>The cleaned string</returns>
        public static string CleanStringOfSpecialChars(this string input)
        {
            char[] cInput = input.ToCharArray();
            int removed = 0;
            bool isRemoved = false;
            for (int i = 0; i < cInput.Length; i++)
            {
                isRemoved = false;
                for (int j = 0; j < specialCharsFromTo.Length; j += 2)
                {
                    if (cInput[i] >= specialCharsFromTo[j] && cInput[i] <= specialCharsFromTo[j + 1])
                    {
                        removed++;
                        isRemoved = true;
                        break;
                    }
                }
                if (!isRemoved)
                    cInput[i - removed] = cInput[i];
            }

            // shrink the result
            char[] newLengthChars = new char[cInput.Length - removed];
            for (int i = 0; i < newLengthChars.Length; i++)
                newLengthChars[i] = cInput[i];

            return new string( newLengthChars );
        }
        public static string RemoveSpecialCharacters( this string aString )
        {
            StringBuilder lStringBuilder = new StringBuilder( aString.Length);
            foreach ( char c in aString )
            {
                // we want to consider all unicode characters including letters 
                // in the Cyrillic alphabet
                if ( Char.IsLetterOrDigit( c ) )
                {
                    lStringBuilder.Append( c );
                }
            }
            return lStringBuilder.ToString();
        }

        public static string RemapHighOrderChars(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // hack to remap high order unicode characters with a low order equivalents
            // for now, this allows better usage of clipping. This can be removed, once the skin engine can properly render unicode without falling back to sprites
            // as unicode is more widely used, this will hit us more with existing font rendering only allowing cached font textures with clipping
          
            input = input.Replace((char)8211, '-');  //	–
            input = input.Replace((char)8212, '-');  //	—
            input = input.Replace((char)8216, '\''); //	‘
            input = input.Replace((char)8217, '\''); //	’
            input = input.Replace((char)8220, '"');  //	“
            input = input.Replace((char)8221, '"');  //	”
            input = input.Replace((char)8223, '"');  // ‟
            input = input.Replace((char)8226, '*');  //	•
            input = input.Replace(((char)8230).ToString(), "...");  //	…

            return input;
        }

        /// <summary>
        /// TitleCases a string
        /// </summary>
        /// <param name="input">The string to TitleCase</param>
        /// <returns>The TitleCased String</returns>
        public static string ToTitleCase(this string input)
        {
            return textInfo.ToTitleCase(input.ToLower());
        }
        static TextInfo textInfo = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;

        public static string ToSHA1Hash(this string password)
        {
            // don't store the hash if password is empty
            if (string.IsNullOrEmpty(password)) return string.Empty;

            byte[] buffer = Encoding.Default.GetBytes(password);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
        }

    }
    #endregion

    #region Date/Time extension Methods

    public static class DateExtensions
    {
        /// <summary>
        /// Date Time extension method to return a unix epoch
        /// time as a long
        /// </summary>
        /// <returns> A long representing the Date Time as the number
        /// of seconds since 1/1/1970</returns>
        public static long ToEpoch(this DateTime dt)
        {
            return (long)(dt - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Long extension method to convert a Unix epoch
        /// time to a standard C# DateTime object.
        /// </summary>
        /// <returns>A DateTime object representing the unix
        /// time as seconds since 1/1/1970</returns>
        public static DateTime FromEpoch(this long unixTime)
        {
            return new DateTime(1970, 1, 1).AddSeconds(unixTime);
        }
    }

    #endregion

    public enum ThemeType
    {
        File,
        Image
    }

    public class Helper
    {
        #region List<T> Methods

        public static List<T> inverseList<T>(List<T> input)
        {
            List<T> result = new List<T>(input.Count);
            for (int i = input.Count - 1; i >= 0; i--)
                result.Add(input[i]);
            return result;
        }
       
        public static T getElementFromList<T, P>(P currPropertyValue, string PropertyName, int indexOffset, List<T> elements)
        {
            // takes care of "looping"
            if (elements.Count == 0) return default(T);
            int indexToGet = 0;
            P value = default(P);
            for (int i = 0; i < elements.Count; i++)
            {
                try
                {
                    value = (P)elements[i].GetType().InvokeMember(PropertyName, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField, null, elements[i], null);
                    if (value.Equals(currPropertyValue))
                    {
                        indexToGet = i + indexOffset;
                        break;
                    }
                }
                catch (Exception x)
                {
                    MPTVSeriesLog.Write("Wrong call of getElementFromList<T,P>: the Type " + elements[i].GetType().Name + " - " + x.Message);
                    return default(T);
                }
            }
            if (indexToGet < 0) indexToGet = elements.Count + indexToGet;
            if (indexToGet >= elements.Count) indexToGet = indexToGet - elements.Count;
            return elements[indexToGet];
        }

        public static List<string> getFieldNameListFromList<T>(string FieldNameToGet, List<T> elements) where T : DBTable
        {
            MPTVSeriesLog.Write("Elements found: " + elements.Count.ToString());
            List<string> results = new List<string>();
            foreach (T elem in elements)
            {
                try
                {
                    results.Add((string)elem[FieldNameToGet]);
                }
                catch (Exception)
                {
                    MPTVSeriesLog.Write("Wrong call of getPropertyListFromList<T,P>: Type " + elem.GetType().Name);
                }
            }
            return results;
        }

        public static List<P> getPropertyListFromList<T, P>(string PropertyNameToGet, List<T> elements)
        {
            List<P> results = new List<P>();
            foreach (T elem in elements)
            {
                try
                {
                    results.Add((P)elem.GetType().InvokeMember(PropertyNameToGet, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField, null, elem, null));
                }
                catch (Exception)
                {
                    MPTVSeriesLog.Write("Wrong call of getPropertyListFromList<T,P>: Type " + elem.GetType().Name);
                }
            }
            return results;
        }

        public delegate void ForEachOperation<T>(T element, int currIndex);
        /// <summary>
        /// Performs an operation for each element in the list, by starting with a specific index and working its way around it (eg: n, n+1, n-1, n+2, n-2, ...)
        /// </summary>
        /// <typeparam name="T">The Type of elements in the IList</typeparam>
        /// <param name="elements">All elements, this value cannot be null</param>
        /// <param name="startElement">The starting point for the operation (0 operates like a traditional foreach loop)</param>
        /// <param name="operation">The operation to perform on each element</param>
        public static void ProximityForEach<T>(IList<T> elements, int startElement, ForEachOperation<T> operation)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");
            if ((startElement >= elements.Count && elements.Count > 0) || startElement < 0)
                throw new ArgumentOutOfRangeException("startElement", startElement, "startElement must be > 0 and <= elements.Count (" + elements.Count + ")");
            if (elements.Count > 0)                                      // if empty list, nothing to do, but legal, so not an exception
            {
                T item;
                for (int lower = startElement, upper = startElement + 1; // start with the selected, and then go down before going up
                     upper < elements.Count || lower >= 0;               // only exit once both ends have been reached
                     lower--, upper++)
                {
                    if (lower >= 0)                                      // are lower elems left?
                    {
                        item = elements[lower];
                        operation(item, lower);
                        elements[lower] = item;
                    }
                    if (upper < elements.Count)                          // are higher elems left?
                    {
                        item = elements[upper];
                        operation(item, upper);
                        elements[upper] = item;
                    }
                }
            }
        }      

        #endregion

        #region Get Corresponding Series/Season Methods
        public static DBSeries getCorrespondingSeries(int id)
        {
            try
            {
                DBSeries cached = cache.getSeries(id);
                if (cached != null) return cached;
                SQLCondition cond = new SQLCondition();
                cond.Add(new DBSeries(), DBSeries.cID, id, SQLConditionType.Equal);
                List<DBSeries> tmpSeries = DBSeries.Get(cond);
                foreach (DBSeries series in tmpSeries) // should only be one!
                {
                    if (series[DBSeries.cID] == id)
                    {
                        cache.addChangeSeries(series);
                        return series;
                    }
                }
                return null;
            }
            catch (Exception)
            {                
                return null;
            }
            
        }

        public static DBSeason getCorrespondingSeason(int seriesID, int seasonIndex)
        {
            try
            {
                DBSeason cached = cache.getSeason(seriesID, seasonIndex);
                if (cached != null) return cached;
                List<DBSeason> tmpSeasons = DBSeason.Get(seriesID);
                foreach (DBSeason season in tmpSeasons)
                {
                    cache.addChangeSeason(season);
                    if (season[DBSeason.cIndex] == seasonIndex)
                    {
                        return season;
                    }
                }
                return null;
            }
            catch (Exception)
            {                
                return null;
            }
        }
        #endregion

        #region XML File Cache

        public static void SaveXmlCache(string filename, XmlNode node)
        {
            // create cached document
            try
            {
                MPTVSeriesLog.Write("Saving xml to file cache: " + filename, MPTVSeriesLog.LogLevel.Debug);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(node.OuterXml);
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                doc.Save(filename);
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("Failed to save xml to cache: {0}", filename);
                MPTVSeriesLog.Write("Exception: {0}", e.Message);
            }
        }

        public static XmlNode LoadXmlCache(string filename)
        {
            if (!File.Exists(filename)) return null;

            // Load cache
            XmlDocument doc = new XmlDocument();
            try
            {
                MPTVSeriesLog.Write("Loading xml from file cache: " + filename, MPTVSeriesLog.LogLevel.Debug);
                doc.Load(filename);
                return doc.FirstChild;
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("Failed to load xml from file cache: {0}", filename);
                MPTVSeriesLog.Write("Exception: {0}", e.Message);
                return null;
            }
        }

        #endregion

        #region Other Public Methods
    
        /// <summary>
        /// Resolves skin\\ and thumbs\\ relative paths to absolute.
        /// Other relative paths are resolved using MediaPortal installation directory.
        /// Absolute paths are just cleaned.
        /// </summary>
        /// <param name="file">Relative or absolute path to resolve</param>
        /// <returns></returns>
        public static string getCleanAbsolutePath(string file) {
            if (!System.IO.Path.IsPathRooted(file)) {
                // Respect custom skin folders
                if (file.ToLower().StartsWith("skin\\"))
                    file = file.Replace("skin", Settings.GetPath(Settings.Path.skin));
                else if (file.ToLower().StartsWith("thumbs\\"))
                    file = file.Replace("thumbs", Settings.GetPath(Settings.Path.thumbs));
                else
                    file = Helper.PathCombine(Settings.GetPath(Settings.Path.app), file);
            }

            foreach (char c in System.IO.Path.GetInvalidPathChars())
                file = file.Replace(c, '_');

            return file;
        }

        /// <summary>
        /// Removes non-existant files from a list of filenames
        /// </summary>
        /// <param name="filenames"></param>
        /// <returns></returns>
        public static List<string> filterExistingFiles(List<string> filenames)
        {
            for (int f = 0; f < filenames.Count; f++) {
                bool wasCached = false;
                if ((wasCached = nonExistingFiles.Contains(filenames[f])) || !System.IO.File.Exists(filenames[f])) {
                    if (!wasCached) {
                        MPTVSeriesLog.Write("File does not exist: " + filenames[f], MPTVSeriesLog.LogLevel.Debug);
                        nonExistingFiles.Add(filenames[f]);
                    }
                    filenames.RemoveAt(f);
                    f--;
                }
            }
            return filenames;
        } static List<string> nonExistingFiles = new List<string>();

        /// <summary>
        /// Convertes a given amount of Milliseconds into humanly readable MM:SS format
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static System.String MSToMMSS(double milliseconds)
        {
            TimeSpan t = new TimeSpan(0, 0, 0, 0, (int)milliseconds);
            //cs1 anomalies or no disc/data available -> -:- 
            if (milliseconds <= 0)
            { return ("-- : --"); }
            //cs1 playtimes >= 1 hour -> 1:MM:SS 
            else if (milliseconds >= 3600000)
            { return t.Hours.ToString("0") + ":" + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00"); }
            //cs1 playtimes < 1 hour -> MM:SS 
            else { return t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00"); }
        }

        /// <summary>
        /// Joins two parts of a path
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string PathCombine(string path1, string path2) {
            if (path1 == null && path2 == null) return string.Empty;
            if (path1 == null) return path2;
            if (path2 == null) return path1;
            if (path2.Length > 0 && (path2[0] == '\\' || path2[0] == '/')) path2 = path2.Substring(1);
            return System.IO.Path.Combine(path1, path2);
        }
        
        /// <summary>
        /// Cleans the path by removing invalid characters
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string cleanLocalPath(string path) {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) {
                path = path.Replace(c, invalidCharReplacement);                
            }
            // Also remove trailing dots and spaces            
            return path.TrimEnd(new char[] { '.' }).Trim();
        } const char invalidCharReplacement = '_';
        
        /// <summary>
        /// Removes 'the' and other common words from the beginning of a series
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        public static string GetSortByName(string sName)
        {
            string SortBy = sName;
            string SortByStrings = Translation.SortByStrings;

            // loop through and try to remove a preposition            
            string[] prepositions = SortByStrings.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);            
            foreach (string currWord in prepositions)
            {
                string word = currWord.ToLower() + " ";
                if (sName.ToLower().IndexOf(word) == 0)
                {
                    SortBy = sName.Substring(word.Length) + ", " + sName.Substring(0, currWord.Length);
                    break;
                }
            }
            return SortBy;
        }

        /// <summary>
        /// Converts a string of letters to corresponding numbers
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public static string ConvertSMSInputToPinCode(string input) {
			switch (input.ToLower()) {
				case "a":
				case "b":
				case "c":
					return "2";

				case "d":
				case "e":
				case "f":
					return "3";

				case "g":
				case "h":
				case "i":
					return "4";

				case "j":
				case "k":
				case "l":
					return "5";

				case "m":
				case "n":
				case "o":
					return "6";

				case "p":
				case "q":
				case "r":
				case "s":
					return "7";

				case "t":
				case "u":
				case "v":
					return "8";

				case "w":
				case "x":
				case "y":
				case "z":
					return "9";

				default:
					return input;

			}
		}

        /// <summary>
        /// Builds a string of pipe seperated tagged views for a series
        /// </summary>
        /// <param name="series">Series object</param>
        /// <param name="addView">Set to true if adding a view to series</param>
        /// <param name="viewName">Name of view</param>
        /// <returns></returns>
        public static string GetSeriesViewTags(DBSeries series, bool addView, string viewName) {                                   
            // Get Current tags in series
            string newTags = string.Empty;
            string currTags = series[DBOnlineSeries.cViewTags].ToString().Trim();
            string[] splitTags = currTags.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);            

            if (addView) {
                // If no view tags exists, add it
                if (currTags.Length == 0) {
                    newTags = "|" + viewName + "|";
                }
                else {
                    // Check if view tag already exists, ignoring case. If not add it
                    bool tagExists = false;
                    foreach (string tag in splitTags) {
                        if (tag.Equals(viewName, StringComparison.CurrentCultureIgnoreCase)) {
                            tagExists = true;
                            newTags = currTags;
                            break;
                        }
                    }
                    // Add view tag to series if it doesnt exist
                    if (!tagExists)
                        newTags = currTags + viewName + "|";
                }
            }
            else {
                // Remove tag if its exists
                foreach (string tag in splitTags) {
                    if (!tag.Equals(viewName, StringComparison.CurrentCultureIgnoreCase))
                        newTags += "|" + tag;
                }
                if (newTags.Length > 0)
                    newTags += "|";
            }
            return newTags;
        }

        /// <summary>
        /// Removes duplicate items from a list
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns>A list with unique items</returns>
        public static List<string> RemoveDuplicates(List<string> inputList) {
            Dictionary<string, int> uniqueStore = new Dictionary<string, int>();
            List<string> finalList = new List<string>();
            foreach (string currValue in inputList) {
                if (!uniqueStore.ContainsKey(currValue)) {
                    uniqueStore.Add(currValue, 0);
                    finalList.Add(currValue);
                }
            }
            return finalList;
        }

        /// <summary>
        /// Returns a limited list of items
        /// </summary>
        /// <param name="list"></param>
        /// <param name="limit"></param>
        public static void LimitList(ref List<string> list, int limit) {
            if (limit >= list.Count) return;
            list.RemoveRange(list.Count - (list.Count - limit), (list.Count - limit));
        }

        /// <summary>
        /// Checks if a filename is an Image file e.g. ISO
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Returns true if file is a image</returns>
        public static bool IsImageFile(string filename) {
            string extension = System.IO.Path.GetExtension(filename).ToLower();
            return VirtualDirectory.IsImageFile(extension);
        }

        /// <summary>
        /// Checks if Fullscreen video is active
        /// </summary>
        /// <returns></returns>
        public static bool IsFullscreenVideo {
            get {
                bool isFullscreen = false;
                if (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN)
                    isFullscreen = true;
                return isFullscreen;
            }
        }

        public static void disableNativeAutoplay()
        {
            MPTVSeriesLog.Write("Disabling native autoplay.");
            AutoPlay.StopListening();
        }

        public static void enableNativeAutoplay()
        {
            if (GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING)
            {
                MPTVSeriesLog.Write("Re-enabling native autoplay.");
                AutoPlay.StartListening();
            }
        }

        public static string UppercaseFirst(string s) {
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static void GetEpisodeIndexesFromComposite(string compositeID, out int seasonIndex, out int episodeIndex)
        {
            seasonIndex = 0;
            episodeIndex = 0;

            if (string.IsNullOrEmpty(compositeID)) return;
        
            string[] splits = compositeID.Split(new char[] { '_' });
            string[] epComp = splits[1].Split(new char[] { 'x' });

            int.TryParse(epComp[0], out seasonIndex);
            int.TryParse(epComp[1], out episodeIndex);
            
            return;
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            return string.IsNullOrEmpty(value.Trim());
        }

        #endregion

        #region Assembly methods
        public static bool IsAssemblyAvailable(string name, Version ver) {
            bool result = false;

            MPTVSeriesLog.Write(string.Format("Checking whether assembly {0} is available and loaded...", name), MPTVSeriesLog.LogLevel.Debug);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
                try
                {
                    if (a.GetName().Name == name && a.GetName().Version >= ver)
                    {
                        MPTVSeriesLog.Write(string.Format("Assembly {0} is available and loaded.", name), MPTVSeriesLog.LogLevel.Debug);
                        result = true;
                        break;
                    }
                }
                catch
                {
                    MPTVSeriesLog.Write(string.Format("Assembly.GetName() call failed for '{0}'!\n", a.Location));
                }

            if (!result) {
                MPTVSeriesLog.Write(string.Format("Assembly {0} is not loaded (not available?), trying to load it manually...", name), MPTVSeriesLog.LogLevel.Debug);
                try {
                    //Assembly assembly = AppDomain.CurrentDomain.Reflection(new AssemblyName(name));
                    Assembly assembly = Assembly.ReflectionOnlyLoad(name);
                    MPTVSeriesLog.Write(string.Format("Assembly {0} is available and loaded successfully.", name), MPTVSeriesLog.LogLevel.Debug);
                    result = true;
                }
                catch (Exception e) {
                    MPTVSeriesLog.Write(string.Format("Assembly {0} is unavailable, load unsuccessful: {1}:{2}", name, e.GetType(), e.Message), MPTVSeriesLog.LogLevel.Debug);
                }
            }

            return result;
        }

        public static bool IsPluginEnabled(string name) {
            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings()) {
                return xmlreader.GetValueAsBool("plugins", name, false);
            }
        }

        public static bool IsSubCentralAvailableAndEnabled {
            get {
                return Helper.IsAssemblyAvailable("SubCentral", new Version(0, 9, 0, 0)) && IsPluginEnabled("SubCentral");
            }
        }

        public static bool IsTraktAvailableAndEnabled
        {
            get
            {
                return File.Exists(Path.Combine(Config.GetSubFolder(Config.Dir.Plugins, "Windows"), "TraktPlugin.dll")) && IsPluginEnabled("Trakt");
            }
        }

        public static bool IsTVSeriesEnabledInTrakt
        {
            get
            {
                string UserAccessToken = null;

                using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings())
                {
                    UserAccessToken = xmlreader.GetValueAsString("Trakt", "UserAccessToken", string.Empty);
                }

                return TraktPlugin.TraktSettings.TVSeries >= 0 && !string.IsNullOrEmpty(UserAccessToken);
            }
        }

        public static bool IsTrailersAvailableAndEnabled
        {
            get
            {
                return File.Exists(Path.Combine(Config.GetSubFolder(Config.Dir.Plugins, "Windows"), "Trailers.dll")) && IsPluginEnabled("Trailers");
            }
        }

        public static bool IsMyTorrentsAvailableAndEnabled
        {
            get
            {
                return File.Exists(Path.Combine(Config.GetSubFolder(Config.Dir.Plugins, "Windows"), "MyTorrents.dll")) && IsPluginEnabled("MyTorrents");
            }
        }

        public static bool IsMpNZBAvailableAndEnabled
        {
            get
            {
                return File.Exists(Path.Combine(Config.GetSubFolder(Config.Dir.Plugins, "Windows"), "mpNZB.dll")) && IsPluginEnabled("mpNZB");
            }
        }

        public static bool IsMovingPicturesAvailableAndEnabled
        {
            get
            {
                return File.Exists(Path.Combine(Config.GetSubFolder(Config.Dir.Plugins, "Windows"), "MovingPictures.dll")) && IsPluginEnabled("Moving Pictures");
            }
        }
        #endregion

        #region Web Methods
        public static bool DownloadFile(string url, string localFile)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", Settings.UserAgent);

            // .NET 4.0: Use TLS v1.2. Many download sources no longer support the older and now insecure TLS v1.0/1.1 and SSL v3.
            ServicePointManager.SecurityProtocol = ( SecurityProtocolType )0xc00;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localFile));
                if (!File.Exists(localFile) || ImageAllocator.LoadImageFastFromFile(localFile) == null)
                {
                    MPTVSeriesLog.Write("Downloading new file from: " + url, MPTVSeriesLog.LogLevel.Debug);
                    webClient.DownloadFile(url, localFile);
                }
                return true;
            }
            catch (WebException)
            {
                MPTVSeriesLog.Write("File download failed from '{0}' to '{1}'", url, localFile);
                return false;
            }
        }
        #endregion

        #region Skin Themes

        public static string GetThemedSkinFile(ThemeType type, string filename)
        {
            string originalFile = string.Empty;
            string themedFile = string.Empty;

            if (type == ThemeType.Image)
                originalFile = GUIGraphicsContext.Skin + "\\Media\\" + filename;
            else
                originalFile = GUIGraphicsContext.Skin + "\\" + filename;

            if (!Settings.SkinThemesSupported)
                return originalFile;

            string currentTheme = GetCurrrentSkinTheme();

            if (string.IsNullOrEmpty(currentTheme))
                return originalFile;

            if (type == ThemeType.Image)
                themedFile = GUIGraphicsContext.Skin + "\\Themes\\" + currentTheme + "\\Media\\" + filename;
            else
                themedFile = GUIGraphicsContext.Skin + "\\Themes\\" + currentTheme + "\\" + filename;

            // if the theme does not contain file return original
            if (!File.Exists(themedFile))
                return originalFile;

            return themedFile;
        }

        public static string GetCurrrentSkinTheme()
        {
            if (GUIThemeManager.CurrentThemeIsDefault)
                return null;

            return GUIThemeManager.CurrentTheme;
        }

        #endregion
    }
}

