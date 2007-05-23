using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    class PathMigration
    {
        public static bool migrateBanners()
        {
            try
            {
                String oldBannersBasePath = oldBannersPath;
                String newBannersBasePath = Settings.GetPath(Settings.Path.banners);
                if (oldBannersBasePath != newBannersBasePath && Directory.Exists(oldBannersBasePath))
                {
                    List<string> files = new List<string>(Directory.GetFiles(oldBannersBasePath,"*", SearchOption.AllDirectories));
                    if (files.Count > 0)
                    {
                        foreach (string file in files)
                        {
                            string newLocation = Helper.PathCombine(newBannersBasePath, file.Replace(oldBannersBasePath, ""));
                            if (!Directory.Exists(Path.GetDirectoryName(newLocation))) Directory.CreateDirectory(Path.GetDirectoryName(newLocation));
                            File.Move(file, newLocation);
                        }
                        if (files.Count > 0)
                        {
                            MPTVSeriesLog.Write("Sucessfully moved " + files.Count.ToString() + " banners to the their new location (" + newBannersBasePath + ")");
                            MPTVSeriesLog.Write("Note: if you had any other files besides banners in the old directory (" + oldBannersBasePath + "), they were also moved!!!");
                        }
                        
                    }
                }
               SQLCondition cond = new SQLCondition();
               
               // we delete all the banner references (I'm sorry, but you'll need to redownload
               DBOnlineEpisode.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cBannerFileNames, string.Empty, cond);
               DBOnlineEpisode.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cCurrentBannerFileName, string.Empty, cond);
               DBOnlineEpisode.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 0, cond);

               DBSeason.GlobalSet(DBSeason.cBannerFileNames, string.Empty, cond);
               DBSeason.GlobalSet(DBSeason.cCurrentBannerFileName, string.Empty, cond);

               DBOption.SetOptions(DBOption.cUpdateBannersTimeStamp, 0);

               // now get all the banners again
               OnlineParsing parser = new OnlineParsing(null);
    
               DBOption.SetOptions(DBOption.cUsesNewPathFormat, true);
               MPTVSeriesLog.Write("Banner references removed...next Online Refresh will get them again");
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error trying to move your banners to the new default location: " + ex.Message);
            }
            return false;
        }
        public static bool migrateDB()
        {
            string databaseFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string oldDBPath = string.Empty;
            oldDBPath = databaseFile.Remove(databaseFile.LastIndexOf('\\')); // Get out of Windows folder
            oldDBPath = databaseFile.Remove(databaseFile.LastIndexOf('\\')); // Get out of plugin folder
            oldDBPath += @"\Database\TVSeriesDatabase4.db3";
            string newDBPath = Settings.GetPath(Settings.Path.database);
            if(oldDBPath != newDBPath) // else no need to do anything
            {
                if (File.Exists(oldDBPath))
                {
                    MPTVSeriesLog.Write("Found old dbFile at " + oldDBPath);
                    if (!File.Exists(newDBPath))
                    {
                        MPTVSeriesLog.Write("Moving Database to new location: " + newDBPath);
                        try
                        {
                            File.Move(oldDBPath, newDBPath);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            MPTVSeriesLog.Write("Error moving db File: " + ex.Message);
                        }
                    }
                    else
                    {
                        MPTVSeriesLog.Write("Not moving db because there is already one at the new location");
                    }
                }
            }
            return false;
        }

        static string replaceOldPath(string input)
        {
            return input.Replace(PathMigration.oldBannersPath, "");
        }
        
        public static string oldBannersPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\";
    }
}
