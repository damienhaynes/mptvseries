// MediaInfoDLL - All info about media files, for DLL
// Copyright (C) 2002-2006 Jerome Martinez, Zen@MediaArea.net
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
// MediaInfoDLL - All info about media files, for DLL
// Copyright (C) 2002-2006 Jerome Martinez, Zen@MediaArea.net
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
//
// Microsoft Visual C# wrapper for MediaInfo Library
// See MediaInfo.h for help
//
// To make it working, you must put MediaInfo.Dll
// in the executable folder
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Runtime.InteropServices;
using WindowPlugins.GUITVSeries;

namespace WindowPlugins.GUITVSeries.MediaInfoLib
{
    public enum StreamKind
    {
        General,
        Video,
        Audio,
        Text,
        Chapters,
        Image
    }

    public enum InfoKind
    {
        Name,
        Text,
        Measure,
        Options,
        NameText,
        MeasureText,
        Info,
        HowTo
    }

    public enum InfoOptions
    {
        ShowInInform,
        Support,
        ShowInSupported,
        TypeOfValue
    }


    public class MediaInfo : IDisposable
    {
        //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)  
        [DllImport("MediaInfo.dll")]
        static extern IntPtr MediaInfo_New();
        [DllImport("MediaInfo.dll")]
        static extern void MediaInfo_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        static extern int MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
        [DllImport("MediaInfo.dll")]
        static extern void MediaInfo_Close(IntPtr Handle);
        //[DllImport("MediaInfo.dll")]
        //static extern IntPtr MediaInfo_Inform(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] uint Reserved);
        //[DllImport("MediaInfo.dll")]
        //static extern IntPtr MediaInfo_GetI(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, uint StreamNumber, uint Parameter, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfInfo);
        [DllImport("MediaInfo.dll")]
        static extern IntPtr MediaInfo_Get(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, uint StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfInfo, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfSearch);
        //[DllImport("MediaInfo.dll")]
        //static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
        //[DllImport("MediaInfo.dll")]
        //static extern int MediaInfo_State_Get(IntPtr Handle);
        //[DllImport("MediaInfo.dll")]
        //static extern int MediaInfo_Count_Get(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, int StreamNumber);

        IntPtr Handle;

        static MediaInfo _instance;
        public static MediaInfo GetInstance()
        {
            if (_instance == null)
                return _instance = new MediaInfo();
            else return _instance;
        }

        private MediaInfo() 
        {
            try
            {
                Handle = MediaInfo_New();
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error creating the MediaInfo Object, check that MediaInfo.dll is in the windows plugins directory: ", ex.Message, MPTVSeriesLog.LogLevel.Normal);
            }
        }
        ~MediaInfo() 
        {
            try
            {
                if (Handle == IntPtr.Zero) return;
                MediaInfo_Delete(Handle);
                Handle = IntPtr.Zero;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error deleting the MediaInfo Object: ", ex.Message, MPTVSeriesLog.LogLevel.Normal);
            } 
        }
                
        //String Inform() { return Marshal.PtrToStringUni(MediaInfo_Inform(Handle, 0)); }
        String Get(StreamKind StreamKind, uint StreamNumber, String Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch) { return Marshal.PtrToStringUni(MediaInfo_Get(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo, KindOfSearch)); }
        //String Get(StreamKind StreamKind, uint StreamNumber, uint Parameter, InfoKind KindOfInfo) { return Marshal.PtrToStringUni(MediaInfo_GetI(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo)); }
        //String Option(String Option, String Value) { return Marshal.PtrToStringUni(MediaInfo_Option(Handle, Option, Value)); }
        //int State_Get() { return MediaInfo_State_Get(Handle); }
        //int Count_Get(StreamKind StreamKind, int StreamNumber) { return MediaInfo_Count_Get(Handle, StreamKind, StreamNumber); }

        public String Get(StreamKind StreamKind, uint StreamNumber, String Parameter, InfoKind KindOfInfo) { return Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name); }
        public String Get(StreamKind StreamKind, uint StreamNumber, String Parameter) { return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name); }
        //public String Get(StreamKind StreamKind, uint StreamNumber, uint Parameter) { return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text); }
        //public String Option(String Option_) { return Option(Option_, ""); }
        //public int Count_Get(StreamKind StreamKind) { return Count_Get(StreamKind, -1); }

        public int Open(String FileName) { return MediaInfo_Open(Handle, FileName); }
        public void Close() { MediaInfo_Close(Handle); }
        public string getVidCodec() { return this.Get(StreamKind.Video, 0, "Codec"); }
        public string getVidBitrate(){ return this.Get(StreamKind.Video, 0, "BitRate"); }
        public string getWidth() { return this.Get(StreamKind.Video, 0, "Width"); }
        public string getHeight() { return this.Get(StreamKind.Video, 0, "Height"); }
        public string getAR() { return this.Get(StreamKind.Video, 0, "AspectRatio"); }
        public string getPlaytime() { return this.Get(StreamKind.Video, 0, "PlayTime"); }
        public string getFPS() { return this.Get(StreamKind.Video, 0, "FrameRate"); }
        public string getAudioCount() { return this.Get(StreamKind.Audio, 0, "StreamCount");}
        public string getAudioCodec() { return this.Get(StreamKind.Audio, 0, "Codec");  }
        public string getAudioBitrate() { return this.Get(StreamKind.Audio, 0, "BitRate"); }
        public string getAudioStreamCount() { return this.Get(StreamKind.Audio, 0, "StreamCount"); }
        public string getNoChannels() { return getNoChannels(0); }
        public string getNoChannels(int stream) { return this.Get(StreamKind.Audio, (uint)stream, "Channel(s)"); }
        public string getTextCount() { return this.Get(StreamKind.General, 0, "TextCount"); }


        #region IDisposable Members

        public void Dispose()
        {
            if (Handle == IntPtr.Zero) return;
            MediaInfo_Delete(Handle);
            Handle = IntPtr.Zero;
        }

        #endregion
    }

}
