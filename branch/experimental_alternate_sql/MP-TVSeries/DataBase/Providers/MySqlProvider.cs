using System;
using System.Data.Common;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    public class MySqlProvider : DBProvider
    {
        //MySQL limitation of 1000 bytes (333 utf8 characters) for primary key
        private int cMaxKeyLength = 333;

        public MySqlProvider(string connectionString)
        {
            m_sConnectionString = connectionString;
        }

        public override string sProviderName
        {
            get 
            { 
                return "MySql.Data.MySqlClient"; 
            }
        }

        public override bool bUseLimit
        {
            get 
            {
                return false;
            }
        }

        public override string sGetLastIdCommand
        {
            get
            {
                return "select LAST_INSERT_ID() as ID";
            }
        }

        public override char cIdentifierStart
        {
            get { return '`'; }
        }

        public override char cIdentifierFinish
        {
            get { return '`'; }
        }

        public override string Description
        {
            get
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                builder.ConnectionString = m_sConnectionString;
                return string.Format("MySQL: Server={0}, Database={1}", builder["Server"], builder["Database"]);
            }
        }

        public override string Clean(String command)
        {
            //MySql uses \ as escape charaters
            return command.Replace(@"\", @"\\");
        }

        public static void TestConnection(string ConnectionString)
        {
            DbProviderFactory factory = MySql.Data.MySqlClient.MySqlClientFactory.Instance;
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

            //initial mysql commands (use GO not ; so that Replace commands below work
            string createScript = "USE mysql GO\r\n DROP DATABASE IF EXISTS %MpTvSeriesDb4% GO\r\n CREATE DATABASE %MpTvSeriesDb4% GO\r\n use %MpTvSeriesDb4% GO\r\n";
            //+ " SET GLOBAL SQL_MODE=ANSI_QUOTES GO\r\n";
            createScript = createScript.Replace("%MpTvSeriesDb4%", database);

            using (StreamReader reader = new StreamReader(stream)) {
                createScript += reader.ReadToEnd();
            }

            createScript = createScript.Replace("GO\r\n", ";!");
            createScript = createScript.Replace("\r\n", " ");
            createScript = createScript.Replace("\t", " ");

            //convert the script to MySQL syntax
            createScript = createScript.Replace("[dbo].", "");
            createScript = createScript.Replace("NONCLUSTERED INDEX", "INDEX");
            createScript = createScript.Replace('[', '`');
            createScript = createScript.Replace(']', '`');
            createScript = createScript.Replace("IDENTITY(1,1) NOT NULL", "NOT NULL AUTO_INCREMENT");

            //MySQL limits keys to 1000 bytes so default utf8 encoding only allows a length of 333 (if not enough  need to change the character encoding)
            createScript = createScript.Replace("`EpisodeFilename` varchar(1024) NOT NULL,--MySqlReplace", "`EpisodeFilename` varchar(333) NOT NULL,");
            createScript = createScript.Replace("`filename` varchar(1024) NOT NULL,--MySqlReplace", "`filename` varchar(333) NOT NULL,");
            
            string[] Commands = createScript.Split('!');

            DbProviderFactory factory = MySql.Data.MySqlClient.MySqlClientFactory.Instance;
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
                if (field.Primary && (field.MaxLength <= DBField.cMaxLength || field.MaxLength > cMaxKeyLength)) {
                    type = string.Format("varchar({0})", cMaxKeyLength);
                } else if (field.MaxLength <= DBField.cMaxLength) {
                    type = "varchar(2048)";
                } else {
                    type = string.Format("varchar({0})", field.MaxLength);
                }
            } else {
                type = "int";
            }

            string sQuery = "ALTER TABLE `" + tableName + "` ADD `" + fieldName + "` " + type;
            DbProviderFactory factory = MySql.Data.MySqlClient.MySqlClientFactory.Instance;

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
                if (field.Primary && (field.MaxLength <= DBField.cMaxLength || field.MaxLength > cMaxKeyLength)) {
                    type = string.Format("varchar({0})", cMaxKeyLength);
                }else if (field.MaxLength <= DBField.cMaxLength) {
                    type = "varchar(2048)";
                } else {
                    type = string.Format("varchar({0})", field.MaxLength);
                }
            } else {
                type = "int";
            }

            String sQuery = "CREATE TABLE `" + tableName + "` (`" + fieldName + "` " + type + (field.Primary ? " primary key)" : ")");
            DbProviderFactory factory = MySql.Data.MySqlClient.MySqlClientFactory.Instance;

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