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

        public DBField(cType type)
        {
            Type = type;
            Primary = false;
            AutoIncrement = false;
        }
        public DBField(cType type, bool primaryKey)
        {
            Type = type;
            Primary = primaryKey;
            AutoIncrement = false;
        }

        public DBField(cType type, bool primaryKey, bool autoIncrement)
        {
            Type = type;
            Primary = primaryKey;
            AutoIncrement = autoIncrement;
        }

        public DBField(DBFieldType dbFieldT)
        {
            Type = dbFieldT.Type;
            Primary = dbFieldT.Primary;
            AutoIncrement = dbFieldT.AutoIncrement;
        }

        /// <summary>
        /// Only works when used on the primary key column
        /// </summary>
        public bool AutoIncrement { get; set; }

        public bool Primary { get; set; }

        public cType Type { get; set; }

        /// <summary>
        /// save DB friendly string (doubling singlequotes
        /// </summary>
        public DBValue Value { get; set; }

        public bool WasChanged { get; set; }
    };
}