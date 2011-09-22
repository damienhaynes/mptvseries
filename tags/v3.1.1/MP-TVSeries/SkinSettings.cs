using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using MediaPortal.GUI.Library;

namespace WindowPlugins.GUITVSeries {
    static class SkinSettings {

        #region Public Properties
        public static string CurrentSkin { get { return GUIGraphicsContext.Skin; } }
        public static string PreviousSkin { get; set; }

        public static bool ImportGraphics { get; set; }
        public static bool ImportFormatting { get; set; }
        public static bool ImportLogos { get; set; }
        public static bool ImportViews { get; set; }
        public static bool ImportVideoOSDImages { get; set; }
        public static bool ImportVideoPlayImages { get; set; }

        public static int WideBannerNewStampPosX { get; set; }
        public static int WideBannerNewStampPosY { get; set; }
        public static int PosterNewStampPosX { get; set; }
        public static int PosterNewStampPosY { get; set; }

        public static List<string> SupportedLayouts {
            get {
                return _supportedLayouts;
            }
            set {
                _supportedLayouts = value;
            }
        } private static List<string> _supportedLayouts = new List<string>();

        public static Dictionary<string, List<string>> SkinProperties {
            get {
                return _skinProperties;
            }
            set {
                _skinProperties = value;
            }
        } private static Dictionary<string, List<string>> _skinProperties = new Dictionary<string, List<string>>();

        public static Dictionary<string, string> VideoOSDImages {
            get {
                return _videoOSDImages;
            }
            set {
                _videoOSDImages = value;
            }
        } private static Dictionary<string, string> _videoOSDImages = new Dictionary<string, string>();

        public static Dictionary<string, string> VideoPlayImages {
            get {
                return _videoPlayImages;
            }
            set {
                _videoPlayImages = value;
            }
        } private static Dictionary<string, string> _videoPlayImages = new Dictionary<string, string>();

        public static Dictionary<string, string> Defines
        {
            get { return _defines; }
            set { _defines = value; }
        } 
        private static Dictionary<string,string> _defines = new Dictionary<string, string>();

        #endregion

        #region Public Constants

        public const string cfanartlistlayout = "fanart.listlayout";
        public const string cfanarticonslayout = "fanart.iconslayout";
        public const string cfanartfilmstriplayout = "fanart.filmstriplayout";
        public const string cfanartcoverflowlayout = "fanart.coverflowlayout";
        public const string cfanartseriesview = "fanart.seriesview";
        public const string cfanartseasonview = "fanart.seasonview";
        public const string cfanartepisodeview = "fanart.episodeview";

        #endregion

        #region Public Methods
        public static void Init()
        {
            // Import Skin Settings
            string xmlSkinSettings = GUIGraphicsContext.Skin + @"\TVSeries.SkinSettings.xml";
            Load(xmlSkinSettings);

            #region Display Format Strings
            TVSeriesPlugin.m_sFormatSeriesCol1 = DBOption.GetOptions(DBOption.cView_Series_Col1);
            TVSeriesPlugin.m_sFormatSeriesCol2 = DBOption.GetOptions(DBOption.cView_Series_Col2);
            TVSeriesPlugin.m_sFormatSeriesCol3 = DBOption.GetOptions(DBOption.cView_Series_Col3);
            TVSeriesPlugin.m_sFormatSeriesTitle = DBOption.GetOptions(DBOption.cView_Series_Title);
            TVSeriesPlugin.m_sFormatSeriesSubtitle = DBOption.GetOptions(DBOption.cView_Series_Subtitle);
            TVSeriesPlugin.m_sFormatSeriesMain = DBOption.GetOptions(DBOption.cView_Series_Main);

            TVSeriesPlugin.m_sFormatSeasonCol1 = DBOption.GetOptions(DBOption.cView_Season_Col1);
            TVSeriesPlugin.m_sFormatSeasonCol2 = DBOption.GetOptions(DBOption.cView_Season_Col2);
            TVSeriesPlugin.m_sFormatSeasonCol3 = DBOption.GetOptions(DBOption.cView_Season_Col3);
            TVSeriesPlugin.m_sFormatSeasonTitle = DBOption.GetOptions(DBOption.cView_Season_Title);
            TVSeriesPlugin.m_sFormatSeasonSubtitle = DBOption.GetOptions(DBOption.cView_Season_Subtitle);
            TVSeriesPlugin.m_sFormatSeasonMain = DBOption.GetOptions(DBOption.cView_Season_Main);

            TVSeriesPlugin.m_sFormatEpisodeCol1 = DBOption.GetOptions(DBOption.cView_Episode_Col1);
            TVSeriesPlugin.m_sFormatEpisodeCol2 = DBOption.GetOptions(DBOption.cView_Episode_Col2);
            TVSeriesPlugin.m_sFormatEpisodeCol3 = DBOption.GetOptions(DBOption.cView_Episode_Col3);
            TVSeriesPlugin.m_sFormatEpisodeTitle = DBOption.GetOptions(DBOption.cView_Episode_Title);
            TVSeriesPlugin.m_sFormatEpisodeSubtitle = DBOption.GetOptions(DBOption.cView_Episode_Subtitle);
            TVSeriesPlugin.m_sFormatEpisodeMain = DBOption.GetOptions(DBOption.cView_Episode_Main);
            #endregion

            // Load all Skin Fields being used
            string[] skinFiles = Directory.GetFiles(GUIGraphicsContext.Skin, "TVSeries*.xml");
            foreach (string skinFile in skinFiles)
            {
                MPTVSeriesLog.Write("Loading Skin Properties in: " + skinFile);
                GetSkinProperties(skinFile);
            }
            LogSkinProperties();

            // Remember last skin used incase we need to reload
            PreviousSkin = CurrentSkin;
        }

        /// <summary>
        /// Reads all Skin Settings and Imports into Database
        /// </summary>
        /// <param name="filename"></param>
        public static void Load(string filename) {          
            // Check if File Exist
            if (!System.IO.File.Exists(filename))
                return;

            XmlDocument doc = new XmlDocument();
            try {
                doc.Load(filename);
            }
            catch (XmlException e) {
                MPTVSeriesLog.Write("Error: Cannot Load skin settings xml file: ", MPTVSeriesLog.LogLevel.Normal);
                MPTVSeriesLog.Write(e.Message);
                return;
            }

            SkinProperties.Clear();

            // Read and Import Skin Settings into database
            GetVersion(doc);
            GetSupportedLayouts(doc);
            GetViews(doc);
            GetGraphicsQuality(doc);
            GetFormattingRules(doc);
            GetLogoRules(doc);
            GetVideoOSDImages(doc);
            GetVideoPlayImages(doc);
            GetThumbNewStampPositions(doc);
            GetDefines(doc);
        }

        /// <summary>
        /// Gets the number of supported layouts at a particular list level
        /// ListLevel = "Group", "Series", "Season", "Episode"        
        /// </summary>
        /// <param name="listLevel"></param>
        /// <returns></returns>
        public static int GetLayoutCount(string listLevel) {
            int count = 0;
            foreach (string layout in SupportedLayouts) {
                switch (listLevel) {
                    case "Group":
                        if (layout.StartsWith("Group"))
                            count++;                    
                        break;

                    case "Series":
                        if (layout.StartsWith("Series"))
                            count++;                        
                        break;

                    case "Season":
                        if (layout.StartsWith("Season"))
                            count++;                        
                        break;

                    case "Episode":
                        if (layout.StartsWith("Episode"))
                            count++;                        
                        break;
                }
            }
            return count;
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets all properties used by this plugin     
        /// </summary>
        /// <param name="skinfile"></param>
        static void GetSkinProperties(string filename)
        {
            string content = string.Empty;

            StreamReader r = new StreamReader(filename);
            content = r.ReadToEnd();

            Regex reg = new Regex(@"(#TVSeries\.\w+(?:\.\w+)*)");
            MatchCollection matches = reg.Matches(content);
            MPTVSeriesLog.Write("Skin uses " + matches.Count.ToString() + " fields", MPTVSeriesLog.LogLevel.Debug);

            for (int i = 0; i < matches.Count; i++)
            {
                string pre = string.Empty;
                string remove = string.Empty;

                if (matches[i].Value.Contains((remove = "#TVSeries.Episode.")))
                    pre = "Episode";
                else if (matches[i].Value.Contains((remove = "#TVSeries.Season.")))
                    pre = "Season";
                else if (matches[i].Value.Contains((remove = "#TVSeries.Series.")))
                    pre = "Series";

                string value = matches[i].Value.Trim().Replace(remove, string.Empty);
                if (pre.Length > 0)
                {
                    if (SkinProperties.ContainsKey(pre))
                    {
                        if (!SkinProperties[pre].Contains(value))
                        {
                            SkinProperties[pre].Add(value);
                        }
                    }
                    else
                    {
                        List<string> v = new List<string>();
                        v.Add(value);
                        SkinProperties.Add(pre, v);
                    }
                }
            }
        }

        /// <summary>
        /// Logs all Skin Properties used
        /// </summary>
        static void LogSkinProperties()
        {
            MPTVSeriesLog.Write("Skin uses the following properties:");

            if (SkinSettings.SkinProperties.ContainsKey("Series"))
            {
                List<string> seriesProperties = SkinSettings.SkinProperties["Series"];
                foreach (string property in seriesProperties)
                {
                    MPTVSeriesLog.Write("#TVSeries.Series." + property);
                }
            }

            if (SkinSettings.SkinProperties.ContainsKey("Season"))
            {
                List<string> seasonProperties = SkinSettings.SkinProperties["Season"];
                foreach (string property in seasonProperties)
                {
                    MPTVSeriesLog.Write("#TVSeries.Season." + property);
                }
            }

            if (SkinSettings.SkinProperties.ContainsKey("Episode"))
            {
                List<string> episodeProperties = SkinSettings.SkinProperties["Episode"];
                foreach (string property in episodeProperties)
                {
                    MPTVSeriesLog.Write("#TVSeries.Episode." + property);
                }
            }
        }

        /// <summary>
        /// Get Skin Settings Version
        /// </summary>
        /// <param name="doc">XML Document</param>
        private static void GetVersion(XmlDocument doc) {
            XmlNode node = null;          

            // Read Version if defined
            node = doc.DocumentElement.SelectSingleNode("/settings/version");
            if (node != null)
                MPTVSeriesLog.Write("Loading Skin Settings: v" + node.InnerText);
        }

        /// <summary>
        /// Get a List of supported layouts
        /// Skin Designers can choose to add these settings so their users can change layouts
        /// from with-in MediaPortals GUI
        /// </summary>
        /// <param name="doc">XML Document</param>
        private static void GetSupportedLayouts(XmlDocument doc) {
            XmlNode node = null;
            XmlNode innerNode = null;

            node = doc.DocumentElement.SelectSingleNode("/settings/layouts");
            if (node == null)
                return;

            MPTVSeriesLog.Write("Loading Supported Layouts Skin Settings");

            SupportedLayouts.Clear();

            #region Group Layouts
            innerNode = node.SelectSingleNode("group");
            if (innerNode != null) {
                // Possible type are List and SmallIcons
                if (innerNode.Attributes.GetNamedItem("List") != null)
                    if (innerNode.Attributes.GetNamedItem("List").Value.ToLower() == "true") SupportedLayouts.Add("GroupList");

                if (innerNode.Attributes.GetNamedItem("SmallIcons") != null)
                    if (innerNode.Attributes.GetNamedItem("SmallIcons").Value.ToLower() == "true") SupportedLayouts.Add("GroupSmallIcons");
            }
            #endregion

            #region Series Layouts
            innerNode = node.SelectSingleNode("series");
            if (innerNode != null) {
                // Possible type are ListPosters, ListBanners, WideBanners and Filmstrip
                if (innerNode.Attributes.GetNamedItem("ListPosters") != null)
                    if (innerNode.Attributes.GetNamedItem("ListPosters").Value.ToLower() == "true") SupportedLayouts.Add("SeriesListPosters");

                if (innerNode.Attributes.GetNamedItem("ListBanners") != null)
                    if (innerNode.Attributes.GetNamedItem("ListBanners").Value.ToLower() == "true") SupportedLayouts.Add("SeriesListBanners");

                if (innerNode.Attributes.GetNamedItem("WideBanners") != null)
                    if (innerNode.Attributes.GetNamedItem("WideBanners").Value.ToLower() == "true") SupportedLayouts.Add("SeriesWideBanners");

                if (innerNode.Attributes.GetNamedItem("Filmstrip") != null)
                    if (innerNode.Attributes.GetNamedItem("Filmstrip").Value.ToLower() == "true") SupportedLayouts.Add("SeriesFilmstrip");

                if (innerNode.Attributes.GetNamedItem("Coverflow") != null)
                  if (innerNode.Attributes.GetNamedItem("Coverflow").Value.ToLower() == "true") SupportedLayouts.Add("SeriesCoverflow");

            }
            #endregion

            #region Season Layouts
            innerNode = node.SelectSingleNode("season");
            if (innerNode != null) {
                // Possible type are List and Filmstrip
                if (innerNode.Attributes.GetNamedItem("List") != null)
                    if (innerNode.Attributes.GetNamedItem("List").Value.ToLower() == "true") SupportedLayouts.Add("SeasonList");

                if (innerNode.Attributes.GetNamedItem("Filmstrip") != null)
                    if (innerNode.Attributes.GetNamedItem("Filmstrip").Value.ToLower() == "true") SupportedLayouts.Add("SeasonFilmstrip");

                if (innerNode.Attributes.GetNamedItem("Coverflow") != null)
                  if (innerNode.Attributes.GetNamedItem("Coverflow").Value.ToLower() == "true") SupportedLayouts.Add("SeasonCoverflow");
            }
            #endregion

            #region Episode Layouts
            innerNode = node.SelectSingleNode("episode");
            if (innerNode != null) {
                // Possible type are List
                if (innerNode.Attributes.GetNamedItem("List") != null)
                    if (innerNode.Attributes.GetNamedItem("List").Value.ToLower() == "true") SupportedLayouts.Add("EpisodeList");
            }
            #endregion

        }        

        /// <summary>
        /// Read View Settings and Import into Database
        /// </summary>
        /// <param name="doc">XML Document</param>
        private static void GetViews(XmlDocument doc) {
            XmlNode node = null;
            XmlNode innerNode = null;
         
            node = doc.DocumentElement.SelectSingleNode("/settings/views");
            if (node != null && node.Attributes.GetNamedItem("import").Value.ToLower() == "true") {                
                List<string> layouts = null;

                // Append First Logo/Image to List
                try {
                    if (node.Attributes.GetNamedItem("AppendlmageToList").Value.ToLower() == "true")
                        DBOption.SetOptions(DBOption.cAppendFirstLogoToList, "1");
                    else
                        DBOption.SetOptions(DBOption.cAppendFirstLogoToList, "0");
                }
                catch {
                    MPTVSeriesLog.Write("Error reading AppendlmageToList skin setting");
                }

                string layout = string.Empty;

                #region Group Views
                innerNode = node.SelectSingleNode("group");
                if (innerNode != null) {
                    MPTVSeriesLog.Write("Loading Skin Group View Settings", MPTVSeriesLog.LogLevel.Normal);

                    // Dont override default Layout if skin can change from GUI
                    if (GetLayoutCount("Group") == 0) {
                        layout = innerNode.Attributes.GetNamedItem("layout").Value;
                        switch (layout.ToLower()) {
                            case "list":
                                DBOption.SetOptions(DBOption.cGraphicalGroupView, "0");
                                break;

                            case "smallicons":
                            case "bigicons":
                                DBOption.SetOptions(DBOption.cGraphicalGroupView, "1");
                                break;

                            default:
                                DBOption.SetOptions(DBOption.cGraphicalGroupView, "0");
                                break;
                        }
                    }
                    // Confirm that the current layout is really supported
                    // May have come from another skin that used an unsupported layout for this skin
                    if (GetLayoutCount("Group") > 0 && !IsLayoutSupported("Group")) {
                        // Set First Supported Type
                        layouts = GetViewLayouts("Group");
                        if (layouts == null || layouts.Count == 0)
                            DBOption.SetOptions(DBOption.cGraphicalGroupView, "0");
                        else
                            DBOption.SetOptions(DBOption.cGraphicalGroupView, layouts[0]);

                    }

                }
                #endregion

                #region Series Views
                innerNode = node.SelectSingleNode("series");
                if (innerNode != null) {
                    MPTVSeriesLog.Write("Loading Skin Series View Settings", MPTVSeriesLog.LogLevel.Normal);

                    if (GetLayoutCount("Series") == 0) {
                        layout = innerNode.Attributes.GetNamedItem("layout").Value;
                        switch (layout.ToLower()) {
                            case "listposters":
                                DBOption.SetOptions(DBOption.cView_Series_ListFormat, "ListPosters");
                                break;

                            case "listbanners":
                                DBOption.SetOptions(DBOption.cView_Series_ListFormat, "ListBanners");
                                break;

                            case "filmstrip":
                                DBOption.SetOptions(DBOption.cView_Series_ListFormat, "Filmstrip");
                                break;

                            case "widebanners":
                                DBOption.SetOptions(DBOption.cView_Series_ListFormat, "WideBanners");
                                break;

                            case "coverflow":
                                DBOption.SetOptions(DBOption.cView_Series_ListFormat, "Coverflow");
                                break;

                            default:
                                DBOption.SetOptions(DBOption.cView_Series_ListFormat, "WideBanners");
                                break;
                        }
                    }
                    // Confirm that the current layout is really supported
                    // May have come from another skin that used an unsupported layout for this skin
                    if (GetLayoutCount("Series") > 0 && !IsLayoutSupported("Series")) {
                        // Set First Supported Type                        
                        layouts = GetViewLayouts("Series");
                        if (layouts == null || layouts.Count == 0)
                            DBOption.SetOptions(DBOption.cView_Series_ListFormat, "WideBanners");
                        else
                            DBOption.SetOptions(DBOption.cView_Series_ListFormat, layouts[0]);

                    }
                }

                innerNode = node.SelectSingleNode("series/item1");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Series_Col1, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("series/item2");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Series_Col2, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("series/item3");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Series_Col3, innerNode.InnerText.Trim());
                #endregion

                #region Season Views
                innerNode = node.SelectSingleNode("season");
                if (innerNode != null) {
                    MPTVSeriesLog.Write("Loading Skin Season View Settings", MPTVSeriesLog.LogLevel.Normal);

                    if (GetLayoutCount("Season") == 0) {
                        layout = innerNode.Attributes.GetNamedItem("layout").Value;
                        switch (layout.ToLower()) {
                            case "list":
                                DBOption.SetOptions(DBOption.cView_Season_ListFormat, "0");
                                break;

                            case "smallicons":
                            case "bigicons":
                            case "filmstrip":                            
                                DBOption.SetOptions(DBOption.cView_Season_ListFormat, "1");
                                break;

                            case "coverflow":
                                DBOption.SetOptions(DBOption.cView_Season_ListFormat, "2");
                                break;

                            default:
                                DBOption.SetOptions(DBOption.cView_Season_ListFormat, "0");
                                break;
                        }
                    }
                    // Confirm that the current layout is really supported
                    // May have come from another skin that used an unsupported layout for this skin
                    if (GetLayoutCount("Season") > 0 && !IsLayoutSupported("Season")) {
                        // Set First Supported Type
                        layouts = GetViewLayouts("Season");
                        if (layouts == null || layouts.Count == 0)
                            DBOption.SetOptions(DBOption.cView_Season_ListFormat, "0");
                        else
                            DBOption.SetOptions(DBOption.cView_Season_ListFormat, layouts[0]);

                    }
                }

                innerNode = node.SelectSingleNode("season/item1");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Season_Col1, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("season/item2");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Season_Col2, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("season/item3");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Season_Col3, innerNode.InnerText.Trim());
                #endregion

                #region Episode Views
                MPTVSeriesLog.Write("Loading Skin Episode View Settings", MPTVSeriesLog.LogLevel.Normal);

                innerNode = node.SelectSingleNode("episode/item1");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Episode_Col1, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("episode/item2");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Episode_Col2, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("episode/item3");
                if (innerNode != null) DBOption.SetOptions(DBOption.cView_Episode_Col3, innerNode.InnerText.Trim());
                #endregion

                ImportViews = true;
            }            
        }

        /// <summary>
        /// Read Formatting Rules and Import into Database
        /// </summary>
        /// <param name="doc"></param>
        private static void GetFormattingRules(XmlDocument doc) {
            XmlNode node = null;

            node = doc.DocumentElement.SelectSingleNode("/settings/formatting");
            if (node != null && node.Attributes.GetNamedItem("import").Value.ToLower() == "true") {
                MPTVSeriesLog.Write("Loading Skin Formatting Rules", MPTVSeriesLog.LogLevel.Normal);

                DBFormatting.ClearAll();
                long id = 0;
                foreach (string rule in node.InnerText.Split('\n')) {
                    string[] seperators = new string[] { "<Enabled>", "<Format>", "<FormatAs>" };
                    string[] properties = rule.Trim('\r').Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                    if (properties.Length == 3) {
                        DBFormatting dbf = new DBFormatting(id);
                        dbf[DBFormatting.cEnabled] = properties[0];
                        dbf[DBFormatting.cReplace] = properties[1];
                        dbf[DBFormatting.cWith] = properties[2];

                        dbf.Commit();
                        id++;
                    }
                }
                ImportFormatting = true;
            }
        }

        /// <summary>
        /// Read Logo Rules and Import into Database
        /// </summary>
        /// <param name="doc"></param>
        private static void GetLogoRules(XmlDocument doc) {
            XmlNode node = null;

            node = doc.DocumentElement.SelectSingleNode("/settings/logos");
            if (node != null && node.Attributes.GetNamedItem("import").Value.ToLower() == "true") {
                MPTVSeriesLog.Write("Loading Skin Logo Rules", MPTVSeriesLog.LogLevel.Normal);

                DBOption.SetOptions("logoConfig", "");
                List<string> logos = new List<string>();
                foreach (string rule in node.InnerText.Split('\n')) {
                    logos.Add(rule.Trim());
                }
                localLogos.saveToDB(logos);
                ImportLogos = true;
            }           
        }

        /// <summary>
        /// Read Graphics Quality Settings and Import into Database
        /// </summary>
        /// <param name="doc"></param>
        private static void GetGraphicsQuality(XmlDocument doc) {
            XmlNode node = null;
            XmlNode innerNode = null;

            node = doc.DocumentElement.SelectSingleNode("/settings/graphicsquality");
            if (node != null && node.Attributes.GetNamedItem("import").Value.ToLower() == "true") {
                MPTVSeriesLog.Write("Loading Skin Thumbnail Graphics Quality", MPTVSeriesLog.LogLevel.Normal);

                innerNode = node.SelectSingleNode("seriesbanners");
                if (innerNode != null) DBOption.SetOptions(DBOption.cQualitySeriesBanners, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("seriesposters");
                if (innerNode != null) DBOption.SetOptions(DBOption.cQualitySeriesPosters, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("seriescoverflow");
                if (innerNode != null) DBOption.SetOptions(DBOption.cQualitySeriesPosters, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("seasonbanners");
                if (innerNode != null) DBOption.SetOptions(DBOption.cQualitySeasonBanners, innerNode.InnerText.Trim());
                innerNode = node.SelectSingleNode("episodethumbs");
                if (innerNode != null) DBOption.SetOptions(DBOption.cQualityEpisodeImages, innerNode.InnerText.Trim());

                ImportGraphics = true;
            }
        }
        
        /// <summary>
        /// Gets a list of supported layouts for view
        /// </summary>
        /// <param name="listLevel"></param>
        /// <returns>List of supported layouts</returns>
        private static List<string> GetViewLayouts(string listLevel) {
            List<String> layouts = new List<string>();
            foreach (string layout in SupportedLayouts) {
                switch (listLevel) {
                    case "Group":
                        if (layout.StartsWith("Group"))
                            layouts.Add(layout.Substring(5));
                        break;

                    case "Series":
                        if (layout.StartsWith("Series"))
                            layouts.Add(layout.Substring(6));
                        break;

                    case "Season":
                        if (layout.StartsWith("Season"))
                            layouts.Add(layout.Substring(6));
                        break;

                    case "Episode":
                        if (layout.StartsWith("Episode"))
                            layouts.Add(layout.Substring(7));
                        break;
                }
            }
            return layouts;
        }

        /// <summary>
        /// Checks if the specified layout is supported by skin
        /// </summary>
        /// <param name="listLevel"></param>
        /// <returns></returns>
        private static bool IsLayoutSupported(string listLevel) {
            bool supported = false;
            string currentLayout = string.Empty;
            List<String> layouts = null;

            // Get List of layouts for View
            layouts = GetViewLayouts(listLevel);
            if (layouts == null || layouts.Count == 0)
                return false;

            // Check if Current Layout is supported
            switch (listLevel) {
                case "Group":
                    if (DBOption.GetOptions(DBOption.cGraphicalGroupView))
                        currentLayout = "SmallIcons";
                    else
                        currentLayout = "List";

                    if (layouts.Contains(currentLayout))
                        supported = true;

                    break;

                case "Series":
                    if (layouts.Contains(DBOption.GetOptions(DBOption.cView_Series_ListFormat)))
                        supported = true;
                    break;

                case "Season":
                    if (DBOption.GetOptions(DBOption.cView_Season_ListFormat) =="1")
                        currentLayout = "Filmstrip";
                    else if (DBOption.GetOptions(DBOption.cView_Season_ListFormat) =="2")
                        currentLayout = "Coverflow";
                    else
                        currentLayout = "List";

                    if (layouts.Contains(currentLayout))
                        supported = true;

                    break;

                case "Episode":
                    if (layouts.Contains("List"))
                        supported = true;
                    break;
            }
            return supported;
        }

        /// <summary>
        /// Read the images to be loaded in Video OSD
        /// </summary>
        private static void GetVideoOSDImages(XmlDocument doc) {
            XmlNode node = null;
            List<string> supportedValues = new List<string> { "episode", "season", "series", "custom" };

            ImportVideoOSDImages = false;
            VideoOSDImages.Clear();

            node = doc.DocumentElement.SelectSingleNode("/settings/videoosdimages");
            if (node != null && node.Attributes.GetNamedItem("import").Value.ToLower() == "true") {
                MPTVSeriesLog.Write("Loading images to be loaded in video OSD", MPTVSeriesLog.LogLevel.Normal);

                foreach (XmlNode innerNode in node.ChildNodes) {
                    if (supportedValues.Contains(innerNode.Name) && innerNode.Attributes.GetNamedItem("use").Value.ToLower() == "true") {
                        VideoOSDImages.Add(innerNode.Name, innerNode.InnerText);
                    }
                }

                ImportVideoOSDImages = true;
            }

            // maintain compatibility with old skins
            if (node == null || !ImportVideoOSDImages /*|| (node.ChildNodes.Count == 0 && ImportVideoOSDImages)*/) {
                VideoOSDImages.Add("episode", string.Empty);
                VideoOSDImages.Add("series", string.Empty);
            }
        }

        private static void GetVideoPlayImages(XmlDocument doc) {
            XmlNode node = null;
            node = doc.DocumentElement.SelectSingleNode("/settings/videoplayimages");

            VideoPlayImages.Clear();          

            if (node != null && node.Attributes.GetNamedItem("import").Value.ToLower() == "true") {
                XmlNodeList propertyList = node.SelectNodes("property");

                if (propertyList != null && propertyList.Count > 0) {
                    MPTVSeriesLog.Write("Loading images to be loaded during video play", MPTVSeriesLog.LogLevel.Normal);

                    foreach (XmlNode oneProperty in propertyList) {
                        XmlNode propertyNameNode = oneProperty.SelectSingleNode("name");
                        XmlNode propertyValueNode = oneProperty.SelectSingleNode("value");
                        string propertyName = string.Empty;
                        string propertyValue = string.Empty;
                        if (propertyNameNode != null) propertyName = propertyNameNode.InnerText;
                        if (propertyValueNode != null) propertyValue = propertyValueNode.InnerText;
                        if (string.IsNullOrEmpty(propertyName)) propertyName = string.Empty;
                        if (string.IsNullOrEmpty(propertyValue)) propertyValue = string.Empty;
                        if (!propertyName.StartsWith("#")) propertyName = "#" + propertyName;
                        if (propertyName.Length > 1 && propertyValue.Length > 0) {
                            MPTVSeriesLog.Write(string.Format("Adding play image {0} for property {1}", propertyValue, propertyName), MPTVSeriesLog.LogLevel.Debug);
                            VideoPlayImages.Add(propertyName, propertyValue);
                        }
                    }
                }

                ImportVideoPlayImages = true;
            }
        }

        /// <summary>
        /// Get Position of 'NEW' Stamp for RecentlyAdded/Unwatched Items in Thumbs view
        /// </summary>
        /// <param name="doc"></param>
        private static void GetThumbNewStampPositions(XmlDocument doc)
        {
            MPTVSeriesLog.Write("Loading Settings for Thumbs NewStamp positions", MPTVSeriesLog.LogLevel.Normal);

            int posx = 0;
            int posy = 0;

            XmlNode node = null;
            node = doc.DocumentElement.SelectSingleNode("/settings/thumbstamp/widebanners/posx");
            if (node != null)
            {
                int.TryParse(node.InnerText, out posx);                
                WideBannerNewStampPosX = posx;
            }
            node = doc.DocumentElement.SelectSingleNode("/settings/thumbstamp/widebanners/posy");
            if (node != null)
            {
                int.TryParse(node.InnerText, out posy);
                WideBannerNewStampPosY = posy;
            }

            node = doc.DocumentElement.SelectSingleNode("/settings/thumbstamp/posters/posx");
            if (node != null)
            {
                int.TryParse(node.InnerText, out posx);
                PosterNewStampPosX = posx;
            }
            node = doc.DocumentElement.SelectSingleNode("/settings/thumbstamp/posters/posy");
            if (node != null)
            {
                int.TryParse(node.InnerText, out posy);
                PosterNewStampPosY = posy;
            }           
        }

        private static void GetDefines(XmlDocument doc)
        {
            MPTVSeriesLog.Write("Loading Skin Defines", MPTVSeriesLog.LogLevel.Normal);

            // clear defines
            Defines.Clear();

            XmlNode node = null;
            node = doc.DocumentElement.SelectSingleNode(string.Format("/settings/defines//property[@key='{0}']", cfanartlistlayout));
            if (node != null) Defines.Add(cfanartlistlayout, node.InnerText);

            node = doc.DocumentElement.SelectSingleNode(string.Format("/settings/defines//property[@key='{0}']", cfanarticonslayout));
            if (node != null) Defines.Add(cfanarticonslayout, node.InnerText.ToLowerInvariant());

            node = doc.DocumentElement.SelectSingleNode(string.Format("/settings/defines//property[@key='{0}']", cfanartfilmstriplayout));
            if (node != null) Defines.Add(cfanartfilmstriplayout, node.InnerText.ToLowerInvariant());

            node = doc.DocumentElement.SelectSingleNode(string.Format("/settings/defines//property[@key='{0}']", cfanartcoverflowlayout));
            if (node != null) Defines.Add(cfanartcoverflowlayout, node.InnerText.ToLowerInvariant());

            node = doc.DocumentElement.SelectSingleNode(string.Format("/settings/defines//property[@key='{0}']", cfanartseriesview));
            if (node != null) Defines.Add(cfanartseriesview, node.InnerText.ToLowerInvariant());

            node = doc.DocumentElement.SelectSingleNode(string.Format("/settings/defines//property[@key='{0}']", cfanartseasonview));
            if (node != null) Defines.Add(cfanartseasonview, node.InnerText.ToLowerInvariant());

            node = doc.DocumentElement.SelectSingleNode(string.Format("/settings/defines//property[@key='{0}']", cfanartepisodeview));
            if (node != null) Defines.Add(cfanartepisodeview, node.InnerText.ToLowerInvariant());

        }
        #endregion
    }

}
