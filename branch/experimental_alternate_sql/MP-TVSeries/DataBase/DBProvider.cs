using System;

namespace WindowPlugins.GUITVSeries
{
    public abstract class DBProvider
    {
        protected String m_sConnectionString = string.Empty;

        public abstract String sProviderName
        {
            get;
        }

        public abstract bool bUseLimit
        {
            get;
        }

        public String sConnectionString
        {
            get
            {
                return m_sConnectionString;
            }
        }

        public abstract string sGetLastIdCommand
        {
            get;
        }


        public abstract char cIdentifierStart
        {
            get;
        }

        public abstract char cIdentifierFinish
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public abstract void InitDB();

        public abstract string Clean(string command);

        public abstract void AddColumn(string tableName, string fieldName, DBField field);

        public abstract void CreateTable(string tableName, string fieldName, DBField field);
    }
}