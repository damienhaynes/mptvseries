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

namespace WindowPlugins.GUITVSeries
{
    public struct DBFieldType
    {
        public DBField.cType Type;
        public bool Primary;
        public bool AutoIncrement;
        public int MaxLength;
    }
    
    /// <summary>
    /// field class - used to hold information
    /// </summary>
    public class DBField
    {
        public enum cType
        {
            Int,
            String
        }

        // the following are remainders to easily change the type (because it was a string! comparision before)
        public const cType cTypeInt = cType.Int;
        public const cType cTypeString = cType.String;

        // private access
        private cType m_type;
        private bool m_primaryKey;
        private bool m_autoIncrement;
        private DBValue m_value;

        //the maximum lenght for string fields (only use in making database columns)
        // - 1024 is a reasonable default - primary keys can't be any larger
        private int m_maxLength = 1024;

        //use this lenght only when you really need the absolute max field length
        public const int cMaxLength = -1;

        private bool wasChanged = false;

        public DBField(cType type)
        {
            m_type = type;
            m_primaryKey = false;
            m_autoIncrement = false;
        }
        public DBField(cType type, bool primaryKey)
        {
            m_type = type;
            m_primaryKey = primaryKey;
            m_autoIncrement = false;
        }
        public DBField(cType type, bool primaryKey, int maxLength)
        {
            m_type = type;
            m_primaryKey = primaryKey;
            m_maxLength = maxLength;
        }
        public DBField(cType type, int maxLength)
        {
            m_type = type;
            m_maxLength = maxLength;
        }

        public DBField(cType type, bool primaryKey, bool autoIncrement)
        {
            m_type = type;
            m_primaryKey = primaryKey;
            m_autoIncrement = autoIncrement;
        }

        public DBField(DBFieldType dbFieldT)
        {
            m_type = dbFieldT.Type;
            m_primaryKey = dbFieldT.Primary;
            m_autoIncrement = dbFieldT.AutoIncrement;
            m_maxLength = dbFieldT.MaxLength;
        }

        public bool AutoIncrement
        {
            get
            {
                return this.m_autoIncrement;
            }
            set
            {
                this.m_autoIncrement = value;
            }
        }

        public bool Primary
        {
            get
            {
                return this.m_primaryKey;
            }
            set
            {
                this.m_primaryKey = value;
            }
        }

        public cType Type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
            }
        }

        public DBValue Value
        {
            // save DB friendly string (doubling singlequotes
            get
            {
                return this.m_value;
            }
            set
            {
                this.m_value = value;
            }
        }

        public int MaxLength
        {
            get
            {
                return this.m_maxLength;
            }
            set
            {
                this.m_maxLength = value;
            }
        }

        public bool WasChanged
        {
            get
            {
                return wasChanged;
            }
            set
            {
                wasChanged = value;
            }
        }
    };
}