using System;
using System.Collections.Generic;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    interface IDownloaders
    {
    }

    // used to do websearches
    // input: DBEpisode
    // ouput: DownloadObject
    interface IWebSources
    {
        bool Search();
        bool SetEpisode(DBEpisode episode);
        DownloadObject Result();
    }

    class DownloadObject
    {
        public enum Type
        {
            URL,
            Torrent,
            NZB
        }

        private Type m_type;
        private String m_Resource;
        

        public DownloadObject(Type type, String sResource)
        {
            m_type = type;
            m_Resource = sResource;
        }

        public Type ResourceType
        {
            get { return m_type;}
        }

        public String ResourceString
        {
            get { return m_Resource; }
        }
    };
}
