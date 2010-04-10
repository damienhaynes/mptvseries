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

namespace WindowPlugins.GUITVSeries.DataBase
{
    public enum DBFieldValueType
    {
        Int,
        String
    }

    public struct DBFieldType
    {
        public DBFieldValueType Type;
        public bool Primary;
        public bool AutoIncrement;
    }

    /// <summary>
    /// field class - used to hold information
    /// </summary>
    public class DBField
    {
        private readonly string m_fieldName;
        private DBFieldType m_fieldType;

        public DBField(string fieldName, DBFieldValueType type)
        {
            m_fieldName = fieldName;
            m_fieldType.Type = type;
            m_fieldType.Primary = false;
            m_fieldType.AutoIncrement = false;
        }

        public DBField(string fieldName, DBFieldValueType type, bool primaryKey)
        {
            m_fieldName = fieldName;
            m_fieldType.Type = type;
            m_fieldType.Primary = primaryKey;
            m_fieldType.AutoIncrement = false;
        }

        public DBField(string fieldName, DBFieldValueType type, bool primaryKey, bool autoIncrement)
        {
            m_fieldName = fieldName;
            m_fieldType.Type = type;
            m_fieldType.Primary = primaryKey;
            m_fieldType.AutoIncrement = autoIncrement;
        }

        public DBField(string fieldName, DBFieldType dbFieldT)
        {
            m_fieldName = fieldName;
            m_fieldType.Type = dbFieldT.Type;
            m_fieldType.Primary = dbFieldT.Primary;
            m_fieldType.AutoIncrement = dbFieldT.AutoIncrement;
        }

        public string FieldName
        {
            get { return m_fieldName; }
        }

        /// <summary>
        /// Only works when used on the primary key column
        /// </summary>
        public bool AutoIncrement
        {
            get
            {
                return m_fieldType.AutoIncrement;
            }
        }

        public bool Primary
        {
            get
            {
                return m_fieldType.Primary;
            }
        }

        public DBFieldValueType ValueType
        {
            get
            {
                return m_fieldType.Type;
            }
        }

        /// <summary>
        /// save DB friendly string (doubling singlequotes
        /// </summary>
        public DBValue Value { get; set; }

        public bool WasChanged { get; set; }
    };
}