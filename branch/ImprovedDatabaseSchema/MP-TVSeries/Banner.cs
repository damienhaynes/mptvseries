using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes;

namespace WindowPlugins.GUITVSeries
{
    class Banner
    {
        public static string getRandomBanner(List<string> BannerList)
        {
            const string graphicalBannerRecognizerSubstring = "-g";
            string langIdentifier = "-lang" + OnlineAPI.SelLanguageAsString + "-";

            // random banners are prefered in the following order
            // 1) own lang + graphical
            // 2) own lang but not graphical
            // 3) english + graphical (english really is any other language banners that are in db)
            // 4) english but not graphical

            string randImage = null;
            if (BannerList == null || BannerList.Count == 0) return String.Empty;
            if (BannerList.Count == 1) randImage = BannerList[0];

            if (randImage == null)
            {
                List<string> langG = new List<string>();
                List<string> lang = new List<string>();
                List<string> engG = new List<string>();
                List<string> eng = new List<string>();
                for (int i = 0; i < BannerList.Count; i++)
                {
                    if (File.Exists(BannerList[i]))
                    {
                        if (BannerList[i].Contains(graphicalBannerRecognizerSubstring))
                        {
                            if (BannerList[i].Contains(langIdentifier))
                                langG.Add(BannerList[i]);
                            else
                                engG.Add(BannerList[i]);
                        }
                        else
                        {
                            if (BannerList[i].Contains(langIdentifier))
                                lang.Add(BannerList[i]);
                            else
                                eng.Add(BannerList[i]);
                        }
                    }
                }

                try
                {
                    if (langG.Count > 0) randImage = langG[new Random().Next(0, langG.Count)];
                    else if (lang.Count > 0) randImage = lang[new Random().Next(0, lang.Count)];
                    else if (engG.Count > 0) randImage = engG[new Random().Next(0, engG.Count)];
                    else if (eng.Count > 0) randImage = eng[new Random().Next(0, eng.Count)];
                    else return String.Empty;
                }
                catch
                {
                    MPTVSeriesLog.Write("Error getting random Image", MPTVSeriesLog.LogLevel.Normal);
                    return String.Empty;
                }
            }
            return File.Exists(randImage) ? randImage : String.Empty;
        }
    }
}
