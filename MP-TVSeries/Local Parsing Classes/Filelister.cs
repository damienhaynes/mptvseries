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
                        //String[] localFiles = FilesinFolder(importPath[DBImportPath.cPath].ToString());
                        List<string> localFiles = filesInFolder(importPath[DBImportPath.cPath].ToString());

                        // trim the import path root from the filenames (because I don't think it makes sense to add unneeded data
                        foreach (String localFile in localFiles)
                        {
                            PathPair pair = new PathPair(localFile.Substring(importPath[DBImportPath.cPath].ToString().Length).TrimStart('\\'), localFile);
                            outList.Add(pair);
                        }

                        MPTVSeriesLog.Write("Found " + localFiles.Count + " supported video files.");
                    }
                }
            }
            return outList;
        }

        private static List<string> filesInFolder(string folder)
        {
            // this is much faster than calling Directory.Getfiles for every extension (and even about twice as fast as the old recursive way, especially over network paths!
            List<string> files = new List<string>();
            List<string> validExts = new List<string>();
            foreach (string ext in MediaPortal.Util.Utils.VideoExtensions) validExts.Add(ext);
            try
            {
                if (System.IO.Directory.Exists(folder))
                    files.AddRange(System.IO.Directory.GetFiles(folder, "*", System.IO.SearchOption.AllDirectories));
                bool valid = false;
                string ext = string.Empty;
                for (int i = 0; i < validExts.Count; i++)
                    validExts[i] = validExts[i].ToLower();
                for (int i = 0; i < files.Count; i++)
                {
                    valid = false;
                    ext = System.IO.Path.GetExtension(files[i]).ToLower();
                    foreach (string validExt in validExts)
                    {
                        if (ext == validExt)
                        {
                            valid = true;
                            break;
                        }
                    }
                    if (!valid)
                    {
                        files.RemoveAt(i);
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error occured while scanning files in '" + folder + "' (" + ex.Message + ").");
            }
            return files;
        }

        /*
        private static List<string> filesInFolder(string folder)
        {
            MPTVSeriesLog.Write("get files: " + DateTime.Now.Second + ":" + DateTime.Now.Millisecond);
            List<string> files = new List<string>();
            try
            {
                if (System.IO.Directory.Exists(folder))
                    foreach (String extention in MediaPortal.Util.Utils.VideoExtensions)
                        files.AddRange(System.IO.Directory.GetFiles(folder, "*" + extention, System.IO.SearchOption.AllDirectories));
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error occured while scanning files in '" + folder + "' (" + ex.Message + ").");
            }
            MPTVSeriesLog.Write("get files done: " + DateTime.Now.Second + ":" + DateTime.Now.Millisecond);
            return files;
        }
         */
    }
}
