using System;
using System.Data.Common;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    public class SQLClientProvider : DBProvider
    {
        public SQLClientProvider(string connectionString)
        {
            m_sConnectionString = connectionString;
        }

        public override string sProviderName
        {
            get
            {
                return "System.Data.SqlClient";
            }
        }

        public override bool bUseLimit
        {
            get
            {
                return false;
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

        public override string sGetLastIdCommand
        {
            get
            {
                return "SELECT @@IDENTITY As ID";
            }
        }

        public override string Description
        {
            get
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                builder.ConnectionString = m_sConnectionString;
                return string.Format("Sql Express: Server={0}, Database={1}", builder["Server"], builder["Database"]);
            }
        }

        public override string Clean(String command)
        {
            return command;
        }

        public static void TestConnection(string ConnectionString)
        {
            DbProviderFactory factory = System.Data.SqlClient.SqlClientFactory.Instance;
            using (DbConnection connection = factory.CreateConnection()) {
                //try and open the database
                try {
                    connection.ConnectionString = ConnectionString;
                    connection.Open();
                } finally {
                    connection.Close();
                }
            }
        }

        public static void CreateDatabase(string ConnectionString)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;

            string database = builder["Database"].ToString();

            //were going to make the database, so remove the database from the connectionstring
            builder.Remove("Initial Catalog");
            builder.Remove("Database");

            System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();
            Stream stream = assm.GetManifestResourceStream("WindowPlugins.GUITVSeries.DB_Classes.create_sqlserver_database.sql");

            string createScript = "USE [master] GO\r\n IF EXISTS (SELECT name FROM sysdatabases WHERE name = N'%MpTvSeriesDb4%') DROP DATABASE [%MpTvSeriesDb4%] GO\r\n CREATE DATABASE [%MpTvSeriesDb4%] GO\r\n use [%MpTvSeriesDb4%] GO\r\n";
            createScript = createScript.Replace("%MpTvSeriesDb4%", database);

            using (StreamReader reader = new StreamReader(stream)) {
                createScript += reader.ReadToEnd();
            }

            createScript = createScript.Replace("GO\r\n", "!");
            createScript = createScript.Replace("\r\n", " ");
            createScript = createScript.Replace("\t", " ");
            string[] Commands = createScript.Split('!');

            DbProviderFactory factory = System.Data.SqlClient.SqlClientFactory.Instance;
            using (DbConnection connection = factory.CreateConnection()) {
                try {
                    connection.ConnectionString = builder.ConnectionString;
                    connection.Open();

                    foreach (string commandText in Commands) {
                        string Sql = commandText.Trim();
                        if (!string.IsNullOrEmpty(Sql) && !Sql.StartsWith("--") && !Sql.StartsWith("/*")) {
                            using (DbCommand command = connection.CreateCommand()) {
                                command.CommandText = commandText;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                } finally {
                    connection.Close();
                }
            }
        }

        public override void InitDB()
        {
            TestConnection(sConnectionString);
        }

        public override void AddColumn(string tableName, string fieldName, DBField field)
        {
            string type = string.Empty;
            if (field.Type == DBField.cType.String) {
                if (field.MaxLength <= DBField.cMaxLength) {
                    type = "varchar(max)";
                } else {
                    type = string.Format("varchar({0})", field.MaxLength);
                }
            } else {
                type = "int";
            }

            string sQuery = "ALTER TABLE [" + tableName + "] ADD [" + fieldName + "] " + type;
            DbProviderFactory factory = System.Data.SqlClient.SqlClientFactory.Instance;

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

        public override void CreateTable(string tableName, string fieldName, DBField field)
        {
            string type = string.Empty;
            if (field.Type == DBField.cType.String) {
                if (field.MaxLength <= DBField.cMaxLength) {
                    type = "varchar(max)";
                } else {
                    type = string.Format("varchar({0})", field.MaxLength);
                }
            } else {
                type = "int";
            }

            String sQuery = "CREATE TABLE [" + tableName + "] ([" + fieldName + "] " + type + (field.Primary ? " primary key)" : ")");
            DbProviderFactory factory = System.Data.SqlClient.SqlClientFactory.Instance;

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