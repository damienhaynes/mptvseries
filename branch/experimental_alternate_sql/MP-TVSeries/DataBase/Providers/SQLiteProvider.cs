using System;
using System.Data.Common;

namespace WindowPlugins.GUITVSeries
{
    public class SQLiteProvider : DBProvider
    {
        public SQLiteProvider(string databaseFile)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.Add("Data Source", databaseFile);
            builder.Add("Pooling", true);   //may not be enalbed by default

            m_sConnectionString = builder.ConnectionString;
        }

        override public String sProviderName
        {
            get
            {
                return "System.Data.SQLite";
            }
        }

        public override bool bUseLimit
        {
            get
            {
                return true;
            }
        }

        public override string sGetLastIdCommand
        {
            get
            {
                return "SELECT last_insert_rowid() AS ID";
            }
        }

        public override char cIdentifierStart
        {
            get { return '['; }
        }

        public override char cIdentifierFinish
        {
            get { return ']'; }
        }

        public override string Description
        {
            get
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                builder.ConnectionString = m_sConnectionString;
                return string.Format("SQLite: {0}", builder["Data Source"]);
            }
        }

        public override string Clean(String command)
        {
            return command;
        }

        public override void InitDB()
        {
            DbProviderFactory factory = System.Data.SQLite.SQLiteFactory.Instance;
            using (DbConnection connection = factory.CreateConnection()) {
                connection.ConnectionString = sConnectionString;
                try {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA cache_size=5000;";        // Each page uses about 1.5K of memory
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA synchronous='OFF';";
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA count_changes=1;";
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA full_column_names=0;";
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA short_column_names=0;";
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA temp_store = MEMORY;";
                        command.ExecuteNonQuery();
                    }
                } finally {
                    connection.Close();
                }
            }
        }

        public override void AddColumn(string tableName, string fieldName, DBField field)
        {
            string sQuery = "ALTER TABLE " + tableName + " ADD " + fieldName + " " + field.Type;
            DbProviderFactory factory = System.Data.SQLite.SQLiteFactory.Instance;

            using (DbConnection connection = factory.CreateConnection()) {
                connection.ConnectionString = sConnectionString;
                try {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = sQuery;
                        command.ExecuteNonQuery();
                    }
                } finally {
                    connection.Close();
                }
            }
        }
    
        public override void  CreateTable(string tableName, string fieldName, DBField field)
        {
            String sQuery = "CREATE TABLE " + tableName + " (" + fieldName + " " + field.Type + (field.Primary ? " primary key)" : ")");
            DbProviderFactory factory = System.Data.SQLite.SQLiteFactory.Instance;

            using (DbConnection connection = factory.CreateConnection()) {
                connection.ConnectionString = sConnectionString;
                try {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = sQuery;
                        command.ExecuteNonQuery();
                    }
                } finally {
                    connection.Close();
                }
            }
        }
    }
}