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

    public enum InfoFileOptions
    {
        FileOption_Nothing = 0x00,
        FileOption_Recursive = 0x01,
        FileOption_CloseAll = 0x02,
        FileOption_Max = 0x04
    };


    public class MediaInfo
    {
        //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)  
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_New();
        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, Int64 File_Size, Int64 File_Offset);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open(IntPtr Handle, Int64 File_Size, Int64 File_Offset);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, Int64 File_Size, byte[] Buffer, IntPtr Buffer_Size);
        [DllImport("MediaInfo.dll")]
        private static extern Int64 MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern Int64 MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Close(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_State_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);
 
        static MediaInfo _instance;
        public static MediaInfo GetInstance()
        {
            if (_instance == null)
                return _instance = new MediaInfo();
            else return _instance;
        }

        //MediaInfo class
        public MediaInfo()
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
                MediaInfo_Delete(Handle);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error deleting the MediaInfo Object: ", ex.Message, MPTVSeriesLog.LogLevel.Normal);
            }
        }

        public int Open(String FileName)
        {        
            return (int)MediaInfo_Open(Handle, FileName);
        }
        public int Open_Buffer_Init(Int64 File_Size, Int64 File_Offset)
        {
            return (int)MediaInfo_Open_Buffer_Init(Handle, File_Size, File_Offset);
        }
        public int Open_Buffer_Continue(IntPtr Buffer, IntPtr Buffer_Size)
        {
            return (int)MediaInfo_Open_Buffer_Continue(Handle, Buffer, Buffer_Size);
        }
        public Int64 Open_Buffer_Continue_GoTo_Get()
        {
            return (int)MediaInfo_Open_Buffer_Continue_GoTo_Get(Handle);
        }
        public int Open_Buffer_Finalize()
        {
            return (int)MediaInfo_Open_Buffer_Finalize(Handle);
        }
        public void Close() 
        { 
            MediaInfo_Close(Handle); 
        }
        public String Inform()
        {        
            return Marshal.PtrToStringUni(MediaInfo_Inform(Handle, (IntPtr)0));
        }
        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch)
        { 
            return Marshal.PtrToStringUni(MediaInfo_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch));
        }
        public String Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
        {            
            return Marshal.PtrToStringUni(MediaInfo_GetI(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));
        }
        public String Option(String Option, String Value)
        {
            return Marshal.PtrToStringUni(MediaInfo_Option(Handle, Option, Value));
        }
        public int State_Get() { return (int)MediaInfo_State_Get(Handle); }
        public int Count_Get(StreamKind StreamKind, int StreamNumber) { return (int)MediaInfo_Count_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber); }
        private IntPtr Handle;

        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter, InfoKind KindOfInfo) { return Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name); }
        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter) { return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name); }
        public String Get(StreamKind StreamKind, int StreamNumber, int Parameter) { return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text); }
        public String Option(String Option_) { return Option(Option_, ""); }
        public int Count_Get(StreamKind StreamKind) { return Count_Get(StreamKind, -1); }

        #region Video Properties

        public string VideoCodec
        {
            get {
                string result = this.Get(StreamKind.Video, 0, "CodecID");
                MPTVSeriesLog.Write("Video Codec ID: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string VideoCodecFormat
        {
            get
            {
                string result = this.Get(StreamKind.Video, 0, "Format");
                MPTVSeriesLog.Write("Video Format: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string VideoFormatProfile
        {
            get
            {
                string result = this.Get(StreamKind.Video, 0, "Format_Profile");
                MPTVSeriesLog.Write("Video Format Profile: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string VideoBitrate{
            get {
                string result = this.Get(StreamKind.Video, 0, "BitRate");
                MPTVSeriesLog.Write("Video Bit Rate: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string VideoWidth {
            get {
                string result = this.Get(StreamKind.Video, 0, "Width");
                MPTVSeriesLog.Write("Video Width: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string VideoHeight {
            get {
                string result = this.Get(StreamKind.Video, 0, "Height");
                MPTVSeriesLog.Write("Video Height: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string VideoAspectRatio {
            get {
                string result = this.Get(StreamKind.Video, 0, "DisplayAspectRatio");
                MPTVSeriesLog.Write("Video Aspect Ratio: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string VideoPlaytime {
            get {
                string result = this.Get(StreamKind.Video, 0, "Duration");
                MPTVSeriesLog.Write("Video Duration: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string VideoFramesPerSecond {
            get {
                string result = this.Get(StreamKind.Video, 0, "FrameRate");
                MPTVSeriesLog.Write("Video Framerate: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        #endregion

        #region Audio Properties

        public string AudioCodec {
            get {
                string result = this.Get(StreamKind.Audio, 0, "CodecID");
                MPTVSeriesLog.Write("Audio Codec ID: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string AudioCodecFormat
        {
            get
            {
                string result = this.Get(StreamKind.Audio, 0, "Format");
                MPTVSeriesLog.Write("Audio Format: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string AudioFormatProfile
        {
            get
            {
                string result = this.Get(StreamKind.Audio, 0, "Format_Profile");
                MPTVSeriesLog.Write("Audio Format Profile: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string AudioBitrate {
            get {
                string result = this.Get(StreamKind.Audio, 0, "BitRate");
                MPTVSeriesLog.Write("Audio Bit Rate: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        public string AudioStreamCount {
            get {
                string result = this.Get(StreamKind.Audio, 0, "StreamCount");
                MPTVSeriesLog.Write("Audio Stream Count: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        /// <summary>
        /// Returns the number of Audio channels in the 1st Audio stream
        /// </summary>
        public string AudioChannelCount {
            get {
                string result = GetAudioChannelCount(0);                
                return result.Length > 0 ? result : "-1";
            }
        }

        #endregion

        #region Subtitle properties

        public string SubtitleCount {
            get {
                string result = this.Get(StreamKind.General, 0, "TextCount");
                MPTVSeriesLog.Write("Subtitle Count: ", result, MPTVSeriesLog.LogLevel.Debug);
                return result.Length > 0 ? result : "-1";
            }
        }

        #endregion

        #region Audio Methods

        public string GetAudioChannelCount(int stream) {
            string result = this.Get(StreamKind.Audio, (int)stream, "Channel(s)");
            MPTVSeriesLog.Write(string.Format("Audio Channel Count [{0}]: {1}", stream.ToString(), result), MPTVSeriesLog.LogLevel.Debug);
            return result;
        }

        #endregion

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
