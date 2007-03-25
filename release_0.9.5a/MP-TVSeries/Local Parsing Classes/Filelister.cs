using System;
using System.Collections.Generic;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    public class PathPair
    {
        public String sMatch_FileName;
        public String sFull_FileName;

        public PathPair(String match, String full)
        {
            sMatch_FileName = match;
            sFull_FileName = full;
        }
    };

    public class Filelister
    {

        public static List<PathPair> GetFiles()
        {
            List<PathPair> outList = new List<PathPair>();
            DBImportPath[] importPathes = DBImportPath.GetAll();

            if (importPathes != null)
            {
                foreach (DBImportPath importPath in importPathes)
                {
                    if (importPath[DBImportPath.cEnabled] != 0)
                    {
                        MPTVSeriesLog.Write("Searching for all supported videos files within " + importPath[DBImportPath.cPath] + " and it's subfolders.");
                        String[] localFiles = FilesinFolder(importPath[DBImportPath.cPath].ToString());

                        // trim the import path root from the filenames (because I don't think it makes sense to add unneeded data
                        foreach (String localFile in localFiles)
                        {
                            PathPair pair = new PathPair(localFile.Substring(importPath[DBImportPath.cPath].ToString().Length).TrimStart('\\'), localFile);
                            outList.Add(pair);
                        }

                        MPTVSeriesLog.Write("Found " + localFiles.Length + " supported video files.");
                    }
                }
            }
            return outList;
        }

        private static String[] JoinArrays(String[] arrayA, String[] arrayB)
        {
            String[] newArray = new String[arrayA.Length + arrayB.Length];
            if (arrayA.Length > 0)
                arrayA.CopyTo(newArray, 0);
            if (arrayB.Length > 0)
                arrayB.CopyTo(newArray, arrayA.Length);
            return newArray;
        }
        private static String[] FilesinFolder(String folder)
        {
            try
            {
                String[] fileList = new String[0];
                if (System.IO.Directory.Exists(folder))
                    foreach (String extention in MediaPortal.Util.Utils.VideoExtensions)
                        fileList = JoinArrays(fileList, System.IO.Directory.GetFiles(folder, "*" + extention));
                foreach (String subfolder in System.IO.Directory.GetDirectories(folder))
                {
                    String[] subFolderFiles = FilesinFolder(subfolder);
                    String[] newArray = new String[fileList.Length + subFolderFiles.Length];
                    if (fileList.Length > 0)
                        fileList.CopyTo(newArray, 0);
                    if (subFolderFiles.Length > 0)
                        subFolderFiles.CopyTo(newArray, fileList.Length);
                    fileList = newArray;
                }
                return fileList;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error occured while scanning files in '" + folder + "' (" + ex.Message + ").");
            }
            return (new String[0]);
        }
    }
}
