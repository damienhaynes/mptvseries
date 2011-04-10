using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace TVSeries
{
    public class Database
    {
        public SQLiteConnection sqlite_connection = null;
        public SQLiteCommand sqlite_command = null;

        public bool Open(string file)
        {
            if (!File.Exists(file)) return false;

            try
            {
                sqlite_connection = new SQLiteConnection(string.Format("Data Source={0};", file));
                sqlite_command = sqlite_connection.CreateCommand();
                sqlite_connection.Open();
                return true;
            }
            catch
            {
                return false;
            }            
        }

        public string GetOption(string property)
        {
            try
            {
                sqlite_command.CommandText = string.Format("SELECT value FROM options WHERE property = '{0}'", property);
                SQLiteDataReader sqlite_datareader = sqlite_command.ExecuteReader();

                if (sqlite_datareader.HasRows)
                {
                    sqlite_datareader.Read();
                    string result = sqlite_datareader["value"].ToString();
                    sqlite_datareader.Close();

                    return result;
                }
            }
            catch { }
            
            return string.Empty;
        }

        public void SetOption(string property, string value)
        {
            try
            {
                // set the value
                if (!string.IsNullOrEmpty(value))
                    value = value.Trim('\0').Trim();

                value = string.IsNullOrEmpty(value) ? string.Empty : value;

                sqlite_command.CommandText = string.Format("UPDATE options SET value = '{0}' WHERE property = '{1}'", value, property);
                sqlite_command.ExecuteNonQuery();                
            }
            catch { }
        }
    }
}
