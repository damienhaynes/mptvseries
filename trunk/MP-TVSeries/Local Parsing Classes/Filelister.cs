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
