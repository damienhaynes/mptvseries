using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class Translation
    {
        static string path = string.Empty;
        static Translation()
        {
            path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Plugins\Windows\MP-TVSeries_lang\";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            loadTranslations(DBOption.GetOptions(DBOption.cLanguage));
        }

        #region Constants Originals
        public static string Series = "Series";
        public static string Series_Plural = "Series (Plr)";
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

        #endregion


        static Dictionary<string, string> TranslatedStrings = new Dictionary<string, string>();
        
        static void loadTranslations(string lang)
        {
            XmlDocument doc = new XmlDocument();
            TranslatedStrings = new Dictionary<string, string>();
            Type TransType = typeof(Translation);
            FieldInfo[] fieldInfos = TransType.GetFields(BindingFlags.Public | BindingFlags.Static);

            try
            {
                doc.Load(path + lang + ".xml");
            }
            catch (Exception)
            {
                MPTVSeriesLog.Write("Cannot find Translation File: ", lang, MPTVSeriesLog.LogLevel.Normal);
                return;
            }
            foreach (XmlNode stringEntry in doc.DocumentElement.ChildNodes)
                if (stringEntry.NodeType == XmlNodeType.Element)
                    try
                    {
                        TranslatedStrings.Add(stringEntry.Attributes.GetNamedItem("Field").Value, stringEntry.InnerText);
                    }
                    catch (Exception ex)
                    {

                    }
            foreach (FieldInfo fi in fieldInfos)
            {
                TransType.InvokeMember(fi.Name, BindingFlags.SetField, null, TransType,
                    new object[] { Get(fi.Name)});
                
            }
            TranslatedStrings = null; // free up
        }

        static string Get(string Field)
        {
            if (TranslatedStrings.ContainsKey(Field)) return TranslatedStrings[Field];
            else
            {
                return (string)(typeof(Translation).InvokeMember(Field, BindingFlags.GetField, null, typeof(Translation), null));
            }
        }

        public static List<string> getSupportedLangs()
        {
            List<string> langs = new List<string>();
            foreach (string file in System.IO.Directory.GetFiles(path, "*.xml"))
            {
                langs.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }
            return langs;
        }
    }
}
