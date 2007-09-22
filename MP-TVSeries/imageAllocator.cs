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
using MediaPortal.Util;
using MediaPortal.GUI.Library;

namespace WindowPlugins.GUITVSeries
{
    public class ImageAllocator
    {
        private static String s_sFontName;
        private static List<String> s_SeriesImageList = new List<string>();
        private static List<String> s_SeasonsImageList = new List<string>();
        private static List<String> s_OtherPersistentImageList = new List<string>();
        private static List<String> s_OtherDiscardableImageList = new List<string>();
        private static Size reqSeriesBannerSize = new Size(758, 140);
        private static Size reqSeasonBannerSize = new Size(400, 578);

        static ImageAllocator()
        {
        }

        #region Helpers
        /// <summary>
        /// Create a banner image of the specified size, outputting the input text on it
        /// </summary>
        /// <param name="sizeImage">Size of the image to be generated</param>
        /// <param name="label">Text to be output on the image</param>
        /// <returns>a bitmap object</returns>
        private static Bitmap drawSimpleBanner(Size sizeImage, string label)
        {
            Bitmap image = new Bitmap(sizeImage.Width, sizeImage.Height);
            Graphics gph = Graphics.FromImage(image);
            //gph.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.White)), new Rectangle(0, 0, sizeImage.Width, sizeImage.Height));
            GUIFont fontList = GUIFontManager.GetFont(s_sFontName);
            Font font = new Font(fontList.FontName, 36);
            gph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            gph.DrawString(label, font, new SolidBrush(Color.FromArgb(200, Color.White)), 5, (sizeImage.Height - font.GetHeight()) / 2);
            gph.Dispose();
            return image;
        }

        /// <summary>
        /// Takes an Image sFileName and tries to load it into MP' graphics memory
        /// If the sFileName was already in memory, it will not be reloaded (basically it caches)
        /// </summary>
        /// <param name="sFileName">The sFileName of the image to load, fails silently if it cannot be loaded</param>
        /// <returns>memory identifier</returns>
        public static string buildMemoryImageFromFile(string sFileName, System.Drawing.Size size)
        {
            try
            {
                if (String.IsNullOrEmpty(sFileName) || !System.IO.File.Exists(sFileName)) return string.Empty;
                return buildMemoryImage(new System.Drawing.Bitmap(sFileName), sFileName, size);
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("Unable to add to MP's Graphics memory: " + sFileName + " Error: " + e.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Takes an Image and tries to load it into MP' graphics memory
        /// If the sFileName was already in memory, it will not be reloaded (basically it caches)
        /// </summary>
        /// <param name="image">The System.Drawing.Bitmap to be loaded</param>
        /// <param name="identifier">A unique identifier for the image so it can be retrieved later on</param>
        /// <returns>memory identifier</returns>
        public static string buildMemoryImage(System.Drawing.Bitmap image, string identifier, System.Drawing.Size size)
        {
            string name = "[TVSeries:" + identifier + "]";
            try
            {
                if (GUITextureManager.LoadFromMemory(null, name, 0, 0, 0) == 0)
                {
                    GUITextureManager.LoadFromMemory(image, name, 0, size.Width, size.Height);
                }
            }
            catch (Exception)
            {
                MPTVSeriesLog.Write("Unable to add to MP's Graphics memory: " + identifier);
                return string.Empty;
            }
            return name;
        }

        public static void Flush(List<String> toFlush)
        {
            foreach (String sTextureName in toFlush)
            {
                Flush(sTextureName);
            }
            toFlush.Clear();
        }

        public static void Flush(string sTextureName)
        {
            GUITextureManager.ReleaseTexture(sTextureName);
        }
        #endregion

        /// <summary>
        /// Set the font name to be used to create dummy banners
        /// </summary>
        /// <param name="sFontName">Size of the image to be generated</param>
        /// <returns>nothing</returns>
        public static void SetFontName(String sFontName)
        {
            s_sFontName = sFontName;
        }

        /// <summary>
        /// Sets or gets the default Series banner size with which banners will be loaded into memory
        /// </summary>
        public static Size SetSeriesBannerSize { set { reqSeriesBannerSize = value; } get { return reqSeriesBannerSize;} }

        /// <summary>
        /// Sets or gets the default Season banner size with which banners will be loaded into memory
        /// </summary>
        public static Size SetSeasonBannerSize { set { reqSeasonBannerSize = value; } get { return reqSeasonBannerSize; } }

        public static String GetSeriesBanner(DBSeries series)
        {
            String sFileName = series.Banner;
            String sTextureName;
            if (sFileName.Length > 0 && System.IO.File.Exists(sFileName))
                sTextureName = buildMemoryImageFromFile(sFileName, reqSeriesBannerSize);  
            else
            {
                return string.Empty;
                // no image, use text, create our own
                string ident = "series_" + series[DBSeries.cID];
                sTextureName = buildMemoryImage(drawSimpleBanner(reqSeriesBannerSize, series[DBOnlineSeries.cPrettyName]), ident, reqSeriesBannerSize);
            }
            if(sTextureName.Length > 0) s_SeriesImageList.Add(sTextureName);
            return sTextureName;
        }

        public static String GetSeasonBanner(DBSeason season, bool createIfNotExist)
        {
            String sFileName = season.Banner;
            String sTextureName;
            if (sFileName.Length > 0)
                sTextureName = buildMemoryImageFromFile(sFileName, reqSeasonBannerSize);
            else if (createIfNotExist)
            {
                // no image, use text, create our own
                string text = (season[DBSeason.cIndex] == 0) ? Translation.specials : Translation.Season + season[DBSeason.cIndex];
                string ident = season[DBSeason.cSeriesID] + "S" + season[DBSeason.cIndex];
                sTextureName = buildMemoryImage(drawSimpleBanner(reqSeasonBannerSize, text), ident, reqSeasonBannerSize);
            }
            else return string.Empty;

            s_SeasonsImageList.Add(sTextureName);
            return sTextureName;
        }

        public static String GetOtherImage(string sFileName, System.Drawing.Size size, bool bPersistent)
        {
            String sTextureName;
            sTextureName = buildMemoryImageFromFile(sFileName, size);
            if (bPersistent)
                s_OtherPersistentImageList.Add(sTextureName);
            else
                s_OtherDiscardableImageList.Add(sTextureName);
            return sTextureName;
        }

        public static String GetOtherImage(Bitmap b, string sFileName, System.Drawing.Size size, bool bPersistent)
        {
            String sTextureName;
            sTextureName = buildMemoryImage(b, sFileName, size);
            if (bPersistent)
                s_OtherPersistentImageList.Add(sTextureName);
            else
                s_OtherDiscardableImageList.Add(sTextureName);
            return sTextureName;
        }

        public static void FlushAll()
        {
            FlushOthers(true);
            FlushSeasons();
            FlushSeries();
        }
        public static void FlushSeries()
        {
            Flush(s_SeriesImageList);
        }
        public static void FlushSeasons()
        {
            Flush(s_SeasonsImageList);
        }
        public static void FlushOthers(bool bFlushPersistents)
        {
            Flush(s_OtherDiscardableImageList);
            if (bFlushPersistents)
                Flush(s_OtherPersistentImageList);
        }

    }
}
