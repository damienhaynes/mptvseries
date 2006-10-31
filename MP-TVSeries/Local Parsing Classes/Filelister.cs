using System;
using System.Collections.Generic;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    class Filelister
    {
        public static String[] GetFiles()
        {
            String[] files = new String[0];
            DBImportPath[] importPathes = DBImportPath.GetAll();

            if (importPathes != null)
            {
                foreach (DBImportPath importPath in importPathes)
                {
                    if (importPath[DBImportPath.cEnabled] != 0)
                    {
                        DBTVSeries.Log("Searching for all supported videos files within " + importPath[DBImportPath.cPath] + " and it's subfolders.");
                        String[] localFiles = FilesinFolder(importPath[DBImportPath.cPath].ToString());
                        String[] trimmedLocalFiles = new String[localFiles.Length];
                        // trim the import path root from the filenames (because I don't think it makes sense to add unneeded data
                        int i = 0;
                        foreach (String localFile in localFiles)
                        {
                            trimmedLocalFiles[i] = localFile.Substring(importPath[DBImportPath.cPath].ToString().Length + 1);
                            i++;
                        }

                        String[] newArray = new String[files.Length + trimmedLocalFiles.Length];
                        if (files.Length > 0)
                            files.CopyTo(newArray, 0);
                        trimmedLocalFiles.CopyTo(newArray, files.Length);
                        files = newArray;
                        DBTVSeries.Log("Found " + trimmedLocalFiles.Length + " supported video files.");
                    }
                }
            }
            return files;
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
                DBTVSeries.Log("Error occured while scanning files in '" + folder + "' (" + ex.Message + ").");
            }
            return (new String[0]);
        }
    }
}
