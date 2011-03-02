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
using System.Runtime.InteropServices;

namespace WindowPlugins.GUITVSeries
{

    class Unrar
    {
        public const int ERAR_END_ARCHIVE = 10;
        public const int ERAR_NO_MEMORY = 11;
        public const int ERAR_BAD_DATA = 12;
        public const int ERAR_BAD_ARCHIVE = 13;
        public const int ERAR_UNKNOWN_FORMAT = 14;
        public const int ERAR_EOPEN = 15;
        public const int ERAR_ECREATE = 16;
        public const int ERAR_ECLOSE = 17;
        public const int ERAR_EREAD = 18;
        public const int ERAR_EWRITE = 19;
        public const int ERAR_SMALL_BUF = 20;

        public const int RAR_OM_LIST = 0;
        public const int RAR_OM_EXTRACT = 1;

        public const int RAR_SKIP = 0;
        public const int RAR_TEST = 1;
        public const int RAR_EXTRACT = 2;

        public const int RAR_VOL_ASK = 0;
        public const int RAR_VOL_NOTIFY = 1;

        public enum RarOperations
        {
            OP_EXTRACT = 0,
            OP_TEST = 1,
            OP_LIST = 2
        }

        public struct RARHeaderData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string ArcName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string FileName;

            public uint Flags;
            public uint PackSize;
            public uint UnpSize;
            public uint HostOS;
            public uint FileCRC;
            public uint FileTime;
            public uint UnpVer;
            public uint Method;
            public uint FileAttr;
            public string CmtBuf;
            public uint CmtBufSize;
            public uint CmtSize;
            public uint CmtState;
        }
        
        public struct RAROpenArchiveData
        {
            public string ArcName;
            public uint OpenMode;
            public uint OpenResult;
            public string CmtBuf;
            public uint CmtBufSize;
            public uint CmtSize;
            public uint CmtState;
        }

#pragma warning disable 0649 // disable never assigned to warning
        public struct RAROpenArchiveDataEx
        {
            public string ArcName;
            public string ArcNameW;
            public uint OpenMode;
            public uint OpenResult;
            public string CmtBuf;
            public uint CmtBufSize;
            public uint CmtSize;
            public uint CmtState;
            public uint Flags;
            public uint Reserved;
        }
        public struct RARHeaderDataEx
        {
            public string ArcName;
            public string ArcNameW;
            public string FileName;
            public string FileNameW;
            public uint Flags;
            public uint PackSize;
            public uint PackSizeHigh;
            public uint UnpSize;
            public uint UnpSizeHigh;
            public uint HostOS;
            public uint FileCRC;
            public uint FileTime;
            public uint UnpVer;
            public uint Method;
            public uint FileAttr;
            public string CmtBuf;
            public uint CmtBufSize;
            public uint CmtSize;
            public uint CmtState;
            public uint Reserved;
        };
#pragma warning restore 0649

        [DllImportAttribute("unrar.dll")]
        private static extern IntPtr RAROpenArchive(ref RAROpenArchiveData ArchiveData);
        [DllImportAttribute("unrar.dll")]
        private static extern int RARCloseArchive(IntPtr hArcData);
        [DllImportAttribute("unrar.dll")]
        private static extern int RARReadHeader(IntPtr hArcData, ref RARHeaderData HeaderData);
        [DllImportAttribute("unrar.dll")]
        private static extern IntPtr RAROpenArchiveEx(ref RAROpenArchiveDataEx ArchiveData);
        [DllImportAttribute("unrar.dll")]
        private static extern int RARReadHeaderEx(IntPtr hArcData, ref RARHeaderDataEx HeaderData);
        [DllImportAttribute("unrar.dll")]
        private static extern int RARProcessFile(IntPtr hArcData, int Operation, string DestPath, string DestName);
        [DllImportAttribute("unrar.dll")]
        private static extern int RARGetDllVersion();

        private String m_sArchiveFileName;
        private IntPtr m_lHandle;

        private bool Open(String ArchiveFileName)
        {
            RAROpenArchiveData uRAR = new RAROpenArchiveData();

            uRAR.ArcName = ArchiveFileName;
            uRAR.CmtBuf = string.Empty.PadLeft(16384, ' ');
            uRAR.CmtBufSize = 16384;
            uRAR.OpenMode = RAR_OM_EXTRACT;

            m_lHandle = RAROpenArchive(ref uRAR);
            return uRAR.OpenResult != 0 ? false : true;
        }

        private bool Close()
        {
            int iStatus = RARCloseArchive(m_lHandle);
            return iStatus != 0 ? false : true;
        }

        public String ArchiveName
        {
            get { return m_sArchiveFileName;}
            set { m_sArchiveFileName = value;}
        }

        public List<String> FileNameList
        {
            get
            {
                List<String> outList = new List<string>();
                if (Open(m_sArchiveFileName))
                {
                    int iStatus;
                    if (m_lHandle != null)
                    {
                        RARHeaderData uHeader = new RARHeaderData();
                        while ((iStatus = RARReadHeader(m_lHandle, ref uHeader)) == 0)
                        {
                            outList.Add(uHeader.FileName);
                            RARProcessFile(m_lHandle, RAR_SKIP, String.Empty, String.Empty);
                        }
                    }
                    Close();
                }
                return outList;
            }
        }

        public bool ExtractAll(String outPath)
        {
            if (Open(m_sArchiveFileName))
            {
                int iStatus = -1;
                if (m_lHandle != null)
                {
                    RARHeaderData uHeader = new RARHeaderData();
                    while ((iStatus = RARReadHeader(m_lHandle, ref uHeader)) == 0)
                    {
                        RARProcessFile(m_lHandle, RAR_EXTRACT, outPath, uHeader.FileName);
                    }
                }
                Close();
                return iStatus != 0 ? false : true;
            }
            return false;
        }

        public bool Extract(String compressedFileName, String outPath)
        {
            if (Open(m_sArchiveFileName))
            {
                int iStatus = - 1;
                if (m_lHandle != null)
                {
                    RARHeaderData uHeader = new RARHeaderData();
                    while ((iStatus = RARReadHeader(m_lHandle, ref uHeader)) == 0)
                    {
                        if (uHeader.FileName == compressedFileName)
                            iStatus = RARProcessFile(m_lHandle, RAR_EXTRACT, outPath, null);
                        else
                            RARProcessFile(m_lHandle, RAR_SKIP, String.Empty, String.Empty);
                    }
                }
                Close();
                return iStatus != ERAR_END_ARCHIVE ? false : true;
            }
            return false;
        }
    }
}
