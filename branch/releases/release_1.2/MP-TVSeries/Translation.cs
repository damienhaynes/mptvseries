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
using System.Reflection;
using System.Text;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    static class Translation
    {
        /// <summary>
        /// These will be loaded with the language files content
        /// if the selected lang file is not found, it will first try to load en(us).xml as a backup
        /// if that also fails it will use the hardcoded strings as a last resort.
        /// </summary>
        #region Translatable Fields
        public static string Series = "Series";
        public static string Series_Plural = "Series";
        public static string Season = "Season";
        public static string Seasons = "Seasons";
        public static string Episode = "Episode";
        public static string Episodes = "Episodes";
        public static string Toggle_watched_flag = "Toggle watched flag";
        public static string Retrieve_Subtitle = "Retrieve Subtitle";
        public static string Load_via_Torrent = "Load via Torrent";
        public static string Mark_all_as_watched = "Mark all as watched";
        public static string Mark_all_as_unwatched = "Mark all as unwatched";
        public static string Hide = "Hide";
        public static string Delete = "Delete";
        public static string Remove_series_from_Favourites = "Remove series from Favourites";
        public static string Add_series_to_Favourites = "Add series to Favourites";
        public static string Force_Local_Scan = "Force Local Scan ";
        public static string Force_Online_Refresh = "Force Online Refresh ";
        public static string In_Progress_with_Barracks = "(In Progress)";
        public static string Only_show_episodes_with_a_local_file = "Only show episodes with a local file";
        public static string Hide_summary_on_unwatched = "Hide the episode's summary on unwatched episodes";
        public static string on = "on";
        public static string off = "off";
        public static string Play_Random_Episode = "Play Random Episode";
        public static string Play_Random_First_Unwatched_Episode = "Play Random First Unwatched Episode";
        public static string Delete_that_series = "Delete that series?";
        public static string Delete_that_season = "Delete that season?";
        public static string Delete_that_episode = "Delete that episode?";
        public static string Confirm = "Confirm";
        public static string Completed = "Completed";
        public static string No_subtitles_found = "No subtitles found";
        public static string skip_Never_ask_again = "skip / Never ask again";
        public static string no_results_found = "no results found!";
        public static string _Hidden_to_prevent_spoilers_ = " * Hidden to prevent spoilers *";
        public static string Yes = "Yes";
        public static string No = "No";
        public static string Error = "#Error";
        public static string No_items = "No items!";
        public static string Unknown = "Unknown";
        public static string wrongSkin = "Wrong Skin file";
        public static string special = "Special";
        public static string specials = "Specials";
        public static string delPhyiscalWarning = "You are about to permanently delete {0} physical file(s).\nWould you like to proceed?";
        public static string Cycle_Banner = "Cycle Banner";
        public static string Force_Online_Match = "Force Online Match";
        public static string Load_via_NewsLeecher = "Load via NewsLeecher";
        public static string Download = "Download";
        public static string Actions = "Actions";
        public static string Options = "Options";
        public static string Genres = "Genres";
        public static string All = "All";
        public static string Latest = "Latest";
        public static string Channels = "Channels";
        public static string Unwatched = "Unwatched";
        public static string Favourites = "Favourites";
        public static string updateMI = "Update Mediainfo of local file(s)";


        #endregion

        static string path = string.Empty;
        static public void Init()
        {
            string lang = DBOption.GetOptions(DBOption.cLanguage);
            if (lang.Length == 0)
            {
                MPTVSeriesLog.Write("No Translation selected, using fall back English");
                lang = "en(us)";
                DBOption.SetOptions(DBOption.cLanguage, lang);
            }
            //path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Plugins\Windows\MP-TVSeries_lang\";
            path = Settings.GetPath(Settings.Path.lang);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            MPTVSeriesLog.Write("Loading Translations for ", lang, MPTVSeriesLog.LogLevel.Normal);
            MPTVSeriesLog.Write(loadTranslations(lang).ToString() + " translated Strings found");
        }
        static Dictionary<string, string> TranslatedStrings = new Dictionary<string, string>();
        
        public static int loadTranslations(string lang)
        {
            XmlDocument doc = new XmlDocument();
            TranslatedStrings = new Dictionary<string, string>();
            Type TransType = typeof(Translation);
            FieldInfo[] fieldInfos = TransType.GetFields(BindingFlags.Public | BindingFlags.Static);

            try
            {
                doc.Load(path + "\\" + lang + ".xml");
            }
            catch (Exception e)
            {
                if (lang == "en(us)")
                    return 0; // othwerise we are in an endless loop!
                MPTVSeriesLog.Write("Cannot find Translation File (or error in xml): ", lang, MPTVSeriesLog.LogLevel.Normal);
                MPTVSeriesLog.Write(e.Message);
                MPTVSeriesLog.Write("Falling back to English", MPTVSeriesLog.LogLevel.Normal);
                DBOption.SetOptions(DBOption.cLanguage, "en(us)");
                return loadTranslations("en(us)");
            }
            foreach (XmlNode stringEntry in doc.DocumentElement.ChildNodes)
                if (stringEntry.NodeType == XmlNodeType.Element)
                    try
                    {
                        TranslatedStrings.Add(stringEntry.Attributes.GetNamedItem("Field").Value, stringEntry.InnerText);
                    }
                    catch (Exception ex)
                    {
                        MPTVSeriesLog.Write("Error in Translation Engine: " + ex.Message);
                    }
            foreach (FieldInfo fi in fieldInfos)
            {
                TransType.InvokeMember(fi.Name, BindingFlags.SetField, null, TransType,
                    new object[] { Get(fi.Name)});
                
            }
            int count = TranslatedStrings.Count;
            TranslatedStrings = null; // free up
            return count;
        }

        public static string Get(string Field)
        {
            if (TranslatedStrings != null && TranslatedStrings.ContainsKey(Field)) return TranslatedStrings[Field];
            else
            {
                return (string)(typeof(Translation).InvokeMember(Field, BindingFlags.GetField, null, typeof(Translation), null));
            }
        }

        public static List<string> getSupportedLangs()
        {
            List<string> langs = new List<string>();
            path = Settings.GetPath(Settings.Path.lang);
            foreach (string file in System.IO.Directory.GetFiles(path, "*.xml"))
            {
                langs.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }
            return langs;
        }
    }
}

