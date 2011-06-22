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
    public enum SQLConditionType
    {
        Equal,
        NotEqual,
        LessThan,
        LessEqualThan,
        GreaterThan,
        GreaterEqualThan,
        Like,
        NotLike,
        In,
        NotIn,
    };

    public class SQLCondition
    {
        private String m_sConditions = String.Empty;
        private String m_sLimit = String.Empty;
        private String m_sOrderstring = String.Empty;
        bool _beginGroup = false;
        public void beginGroup()
        {
            _beginGroup = true;
        }

        public void endGroup()
        {
            m_sConditions += " ) ";
        }

        public bool limitIsSet = false;
        public bool customOrderStringIsSet = false;

        public bool nextIsOr = false;

        // I need this for subqueries
        /// <summary>
        /// Warning: do not set "where", also returns without "where"
        /// </summary>
        public string ConditionsSQLString
        {
            set
            {
                m_sConditions = value;
            }
            get
            {
                return m_sConditions;
            }
        }

        public enum orderType { Ascending, Descending };
        
        public SQLCondition()
        {
        }

        public SQLCondition(DBTable table, String sField, DBValue value, SQLConditionType type)
        {
            Add(table, sField, value, type);
        }

        public void AddSubQuery(string field, DBTable table, SQLCondition innerConditions, DBValue value, SQLConditionType type)
        {
            string sValue;
            if (type == SQLConditionType.Like || type == SQLConditionType.NotLike)
                sValue = "'%" + ((String)value).Replace("'", "''") + "%'";
            else
                sValue = ((String)value).Replace("'", "''");

            AddCustom("( select " + field + " from " + table.m_tableName + innerConditions + innerConditions.orderString + innerConditions.limitString +  " ) ", sValue, type);
        }

        public void Add(DBTable table, String sField, DBValue value, SQLConditionType type)
        {
            if (table.m_fields.ContainsKey(sField))
            {
                String sValue = String.Empty;
                switch (table.m_fields[sField].Type)
                {
                    case DBField.cTypeInt:
                        sValue = value;
                        break;

                    case DBField.cTypeString:
                        if (type == SQLConditionType.Like || type == SQLConditionType.NotLike)
                            sValue = "'%" + ((String)value).Replace("'", "''") + "%'";
                        else
                            sValue = "'" + ((String)value).Replace("'", "''") + "'";
                        break;
                }
                AddCustom(table.m_tableName + "." + sField, sValue, type);
            }
        }

        public void SetLimit(int limit)
        {
            m_sLimit = " limit " + limit.ToString();
            limitIsSet = true;
        }

        public string orderString
        {
            get { return m_sOrderstring; }
        }

        public string limitString
        {
            get { return m_sLimit; }
        }

        public void AddOrderItem(string qualifiedFieldname, orderType type)
        {
            if (m_sOrderstring.Length == 0)
                m_sOrderstring = " order by ";
            else m_sOrderstring += " , ";
            m_sOrderstring += qualifiedFieldname;
            m_sOrderstring += type == orderType.Ascending ? " asc " : " desc ";
            customOrderStringIsSet = true;
        }

        public void AddCustom(string what, string value, SQLConditionType type, bool EncloseIfString)
        {
            if (EncloseIfString)
            {
                if (value.Length > 2 && value[0] != '\'' && !value.IsNumerical())
                    value = "'" + value + "'";
            }
            AddCustom(what, value, type);
        }

        public void AddCustom(string what, string value, SQLConditionType type)
        {
            String sType = String.Empty;
            switch (type)
            {
                case SQLConditionType.Equal:
                    sType = " = ";
                    break;

                case SQLConditionType.NotEqual:
                    sType = " != ";
                    break;
                
                case SQLConditionType.LessThan:
                    sType = " < ";
                    break;
            
                case SQLConditionType.LessEqualThan:
                    sType = " <= ";
                    break;
                
                case SQLConditionType.GreaterThan:
                    sType = " > ";
                    break;
            
                case SQLConditionType.GreaterEqualThan:
                    sType = " >= ";
                    break;
            
                case SQLConditionType.Like:
                    sType = " like ";
                    break;

                case SQLConditionType.NotLike:
                    sType = " not like ";
                    break;

                case SQLConditionType.In:
                    sType = " in ";
                    break;

                case SQLConditionType.NotIn:
                    sType = " not in ";
                    break;
            }

            if (SQLConditionType.In == type || SQLConditionType.NotIn == type) // reverse
                AddCustom(value + sType + "(" + what + ")");
            else 
                AddCustom(what + sType + value);
        }

        public void AddCustom(string SQLString)
        {
            if (m_sConditions.Length > 0 && SQLString.Length > 0)
            {
                if (nextIsOr)
                    m_sConditions += " or ";
                else
                    m_sConditions += " and ";
            }
            if (_beginGroup)
            {
                m_sConditions += " ( ";
                _beginGroup = false;
            }
            m_sConditions += SQLString;
        }

        public static implicit operator String(SQLCondition conditions)
        {
            return  conditions.m_sConditions.Length > 0 ? " where " + conditions.m_sConditions : conditions.m_sConditions;
        }

        public SQLCondition Copy()
        {
            SQLCondition copy = new SQLCondition();
            copy.customOrderStringIsSet = customOrderStringIsSet;
            copy.limitIsSet = limitIsSet;
            
            copy.m_sConditions = m_sConditions;
            copy.m_sLimit = m_sLimit;
            copy.m_sOrderstring = m_sOrderstring;
            return copy;
        }

        public override string ToString()
        {
            return this;
        }
    };
}