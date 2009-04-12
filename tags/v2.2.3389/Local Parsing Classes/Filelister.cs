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
        public String m_sMatch_FileName;
        public String m_sFull_FileName;

        public PathPair(String match, String full)
        {
            m_sMatch_FileName = match;
            m_sFull_FileName = full;
        }
    };

    public class Filelister
    {

        public static List<PathPair> GetFiles(List<String> listFolders)
        {
            List<PathPair> outList = new List<PathPair>();
            int prev = 0;
            foreach (String path in listFolders)
            {
                MPTVSeriesLog.Write("Searching for all supported videos files within " + path + " and it's subfolders");
                filesInFolder(path, ref outList, path.Length);
                MPTVSeriesLog.Write("Found " + (outList.Count - prev).ToString() + " supported video files");
                prev = outList.Count;
            }
            return outList;
        }

        static System.Text.RegularExpressions.Regex s_reg = null;
        static void buildExtRegex()
        {
            string extPattern = string.Empty;
            foreach (string ext in MediaPortal.Util.Utils.VideoExtensions)
            {
                if (extPattern.Length > 0)
                    extPattern += '|';
                extPattern += ext.Replace(".", "\\.");
            }
            s_reg = new System.Text.RegularExpressions.Regex(extPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        private static bool filesInFolder(string folder, ref List<PathPair> outList, int importPathLength)
        {
            // this is much faster than calling Directory.Getfiles for every extension (and even about twice as fast as the old recursive way, especially over network paths!
            if (null == s_reg) buildExtRegex();
            try
            {
                if (System.IO.Directory.Exists(folder))
                {
                    string[] sfiles = System.IO.Directory.GetFiles(folder, "*", System.IO.SearchOption.AllDirectories);
                    for (int i = 0; i < sfiles.Length; i++)
                    {
                        if (s_reg.IsMatch(System.IO.Path.GetExtension(sfiles[i])))
                        {
                            // trim the import path root from the filenames (because I don't think it makes sense to add unneeded data
                            outList.Add(new PathPair(sfiles[i].Substring(importPathLength).TrimStart('\\'), sfiles[i]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while scanning files in '" + folder + "' (" + ex.Message + ") - Going folder by folder.");
                // an error occured, lets be more conservative and scan folder by folder by going recursive
                try
                {
                    foreach (string subDir in System.IO.Directory.GetDirectories(folder))
                        filesInFolder(subDir, ref outList, importPathLength);
                }
                catch (Exception)
                {
                    // if we crash here it means the current folder itself is inacessable
                    Console.WriteLine("Inacessable folder: " + folder);
                    return false;
                }
                return false;
            }
            return true;
        }
    }
}
