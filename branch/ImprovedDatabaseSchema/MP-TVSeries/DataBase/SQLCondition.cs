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

namespace WindowPlugins.GUITVSeries.DataBase
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

    public enum SQLConditionOrder
    {
        Ascending,
        Descending
    };

    public class SQLCondition
    {
        public bool limitIsSet = false;
        public bool customOrderStringIsSet = false;
        public bool nextIsOr = false;

        private bool _beginGroup = false;
        
        public void beginGroup()
        {
            _beginGroup = true;
        }

        public void endGroup()
        {
            ConditionsSQLString += " ) ";
        }

        // I need this for subqueries
        /// <summary>
        /// Warning: do not set "where", also returns without "where"
        /// </summary>
        public string ConditionsSQLString { get; set; }

        public SQLCondition()
        {
            orderString = String.Empty;
            limitString = String.Empty;
            ConditionsSQLString = String.Empty;
        }

        public SQLCondition(DBTable table, String sField, DBValue value, SQLConditionType type)
        {
            orderString = String.Empty;
            limitString = String.Empty;
            ConditionsSQLString = String.Empty;
            Add(table, sField, value, type);
        }

        public void AddSubQuery(string field, DBTable table, SQLCondition innerConditions, DBValue value, SQLConditionType type)
        {
            string sValue;
            if (type == SQLConditionType.Like || type == SQLConditionType.NotLike)
                sValue = "'%" + value.SQLSafeValue + "%'";
            else
                sValue = value.SQLSafeValue;

            AddCustom("( select " + field + " from " + table.TableName + innerConditions + innerConditions.orderString + innerConditions.limitString +  " ) ", sValue, type);
        }

        public void Add(DBTable table, String sField, DBValue value, SQLConditionType type)
        {
            if (table.m_fields.ContainsKey(sField))
            {
                String sValue = String.Empty;
                switch (table.m_fields[sField].Type)
                {
                    case DBFieldType.Int:
                        sValue = value;
                        break;

                    case DBFieldType.String:
                        if (type == SQLConditionType.Like || type == SQLConditionType.NotLike) {
                            sValue = "'%" + value.SQLSafeValue + "%'";
                        } else {
                            sValue = "'" + value.SQLSafeValue + "'";
                        }
                        break;
                }
                AddCustom(table.TableName + "." + sField, sValue, type);
            }
        }

        public void SetLimit(int limit)
        {
            limitString = " limit " + limit.ToString();
            limitIsSet = true;
        }

        public string orderString { get; private set; }

        public string limitString { get; private set; }

        public void AddOrderItem(string qualifiedFieldname, SQLConditionOrder type)
        {
            if (orderString.Length == 0) {
                orderString = " order by ";
            } else {
                orderString += " , ";
            }
            orderString += qualifiedFieldname;
            orderString += type == SQLConditionOrder.Ascending ? " asc " : " desc ";
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
            if (ConditionsSQLString.Length > 0 && SQLString.Length > 0)
            {
                if (nextIsOr)
                    ConditionsSQLString += " or ";
                else
                    ConditionsSQLString += " and ";
            }
            if (_beginGroup)
            {
                ConditionsSQLString += " ( ";
                _beginGroup = false;
            }
            ConditionsSQLString += SQLString;
        }

        public SQLCondition Copy()
        {
            SQLCondition copy = new SQLCondition {
                                                     customOrderStringIsSet = customOrderStringIsSet,
                                                     limitIsSet = limitIsSet,
                                                     ConditionsSQLString = ConditionsSQLString,
                                                     limitString = limitString,
                                                     orderString = orderString
                                                 };

            return copy;
        }

        #region toString
        public override string ToString()
        {
            return this;
        }

        public static implicit operator String(SQLCondition conditions)
        {
            return conditions.ConditionsSQLString.Length > 0 ? " where " + conditions.ConditionsSQLString : conditions.ConditionsSQLString;
        }
        #endregion
    };
}