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

namespace WindowPlugins.GUITVSeries
{
    public class DBValue
    {
        static System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
        static DBValue()
        {
            provider.NumberDecimalSeparator = ".";
        }
        private String value = String.Empty;

        public override String ToString()
        {
            return value;
        }

        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            return this == obj as DBValue;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return base.GetHashCode();
        }

        public DBValue(String value)
        {
            if (value != null)
                this.value = value;
            else
                this.value = String.Empty;
        }
        public DBValue(Boolean value)
        {
            // booleans are 0/1
            this.value = Convert.ToInt16(value).ToString();
        }

        public DBValue(int value)
        {
            this.value = value.ToString();
        }
        public DBValue(long value)
        {
            this.value = value.ToString();
        }

        static public implicit operator String(DBValue value)
        {
            if (value == null)
                return "";

            return value.value;
        }

        static public implicit operator Boolean(DBValue value)
        {
            if (value == null)
                return false;

            if (value.value.Length > 0 && value.value != "0")
                return true;
            else
                return false;
        }

        static public implicit operator int(DBValue value)
        {
            if (null == value)
                return 0;
            try {
                return Convert.ToInt32(value.value);
            } catch (System.FormatException) {
                return 0;
            }
        }

        static public implicit operator long(DBValue value)
        {
            if (null == value)
                return 0;
            try {
                return Convert.ToInt64(value.value);
            } catch (System.FormatException) {
                return 0;
            }
        }

        /// <summary>
        /// NumberDecimalSeperator needs to be "."
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public implicit operator double(DBValue value)
        {
            if (null == value)
                return 0;
            try {
                return Convert.ToDouble(value.value, provider);
            } catch (System.FormatException) {
                return 0;
            }
        }

        static public implicit operator DBValue(String value)
        {
            return new DBValue(value);
        }

        static public implicit operator DBValue(Boolean value)
        {
            return new DBValue(value);
        }

        static public implicit operator DBValue(int value)
        {
            return new DBValue(value);
        }

        static public implicit operator DBValue(long value)
        {
            return new DBValue(value);
        }

        static public implicit operator DBValue(double value)
        {
            return new DBValue(value.ToString(provider));
        }

        static public bool operator ==(DBValue first, DBValue second)
        {
            if ((object)first == null || (object)second == null) {
                if ((object)first == null && (object)second == null)
                    return true;
                else
                    return false;
            }
            return first.value == second.value;
        }

        static public bool operator !=(DBValue first, DBValue second)
        {
            if ((object)first == null || (object)second == null) {
                if ((object)first == null && (object)second == null)
                    return false;
                else
                    return true;
            }
            return first.value != second.value;
        }

    };
}