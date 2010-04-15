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

namespace WindowPlugins.GUITVSeries.DataBase
{
    public enum DBFieldType
    {
        Int,
        String
    }

    public class DBFieldDef
    {
        public string FieldName { get; set; }
        public DBFieldType Type { get; set; }
        public bool Primary { get; set; }
        public bool AutoIncrement{ get; set; }
		public bool Indexed { get; set; }
		public DBValue Default { get; set; }
		public string TableName { get; set; }

		//TODO: Add Virtual (bool), OnlineField (string), and Split (bool)

		private string prettyName;
        public string PrettyName
        {
            get
            {
                if (string.IsNullOrEmpty(prettyName)) {
                    return FieldName;
                }
                return prettyName;
            }
            set { prettyName = value; }
        }

        public string ColumnDefinition
        {
            get
            {
                string def = FieldName + " " + Type;
                if (Primary && Type == DBFieldType.Int && AutoIncrement) {
                    //in SQLite for the automatic creation of an auto incremental primary key you must specify the full "Integer" not just "int"
                    def = FieldName + " " + "Integer";
                }
                if (Primary) {
                    def += " primary key";
                }
                return def;
            }
        }

    	/// <summary>
    	/// returns the fully qualified field name ie. "TableName.FieldName"
    	/// </summary>
		public string Q
    	{
			get
			{
				if (string.IsNullOrEmpty(TableName)) {
					throw new Exception("Table Name not set!");
				}
				return TableName + "." + FieldName;
			}
    	}

        static public implicit operator string(DBFieldDef value)
        {
            return value.FieldName;
        }
    }

    public class DBFieldDefList : Dictionary<string, DBFieldDef>
    {
		public DBFieldDefList() : base(StringComparer.InvariantCultureIgnoreCase)
		{
			
		}
    }

    /// <summary>
    /// A collection of DBFields
    /// </summary>
    public class DBFieldList : Dictionary<string, DBField>
    {
        public DBFieldList() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

		public DBField PrimaryKey
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a copy of DBFieldList that has seperate instances of DBValues
        /// </summary>
        /// <returns></returns>
        public DBFieldList Copy()
        {
            DBFieldList fieldList = new DBFieldList();
            foreach (KeyValuePair<string, DBField> pair in this) {
                //use DBField.Copy as ee need a deep copy of the DBValues so that changes to values in one table are seperate from 
                //another so until we find a better way
                DBField field = pair.Value.Copy();
                fieldList.Add(pair.Key, field);
                if (pair.Value.Primary) {
                    fieldList.PrimaryKey = field;
                }
            }
            return fieldList;
        }
    }

    /// <summary>
    /// field class - used to hold information
    /// </summary>
    public class DBField
    {
        private readonly DBFieldDef m_fieldDef;

        public DBField(DBFieldDef fieldDef)
        {
            m_fieldDef = fieldDef;
        	Value = m_fieldDef.Default;
        }

        public string FieldName
        {
            get { return m_fieldDef.FieldName; }
        }

        public string PrettyName
        {
            get
            {
                return m_fieldDef.PrettyName;
            }
        }
        /// <summary>
        /// Only works when used on the primary key column
        /// </summary>
        public bool AutoIncrement
        {
            get
            {
                return m_fieldDef.AutoIncrement;
            }
        }

        public bool Primary
        {
            get
            {
                return m_fieldDef.Primary;
            }
        }

        public DBFieldType Type
        {
            get
            {
                return m_fieldDef.Type;
            }
        }

        public DBValue Value { get; set; }

    	public string TableName
    	{
    		get
    		{
    			return m_fieldDef.TableName;
    		}
    	}

		/// <summary>
		/// returns the fully qualified field name ie. "TableName.FieldName"
		/// </summary>
		public string Q
    	{
    		get
    		{
    			return m_fieldDef.Q;
    		}
    	}

        /// <summary>
        /// save DB friendly string (ie. escaping singlequotes into double singlequotes)
        /// </summary>
        public string SQLSafeValue
        {
            get
            {
                if (Type == DBFieldType.String) {
                    return Value.SQLSafeValue;
                } else {
                    return Value;
                }
            }
        }

        public bool WasChanged { get; set; }

        /// <summary>
        /// Creates an new instance of DBField that has a instance of DBValue
        /// </summary>
        /// <returns></returns>
        public DBField Copy()
        {
            return new DBField(m_fieldDef) { Value = string.Copy(Value) };
        }
    };
}