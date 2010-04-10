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

namespace WindowPlugins.GUITVSeries.DataBase
{
    public class SQLWhat
    {
        private String m_sFieldList = String.Empty;
        private String m_sFromTables = String.Empty;

        public SQLWhat()
        {

        }

        public SQLWhat(DBTable table)
        {
            Add(table);
        }

        public void Add(DBTable table)
        {
            AddWhat(table);
            if (m_sFromTables.Length > 0) {
                m_sFromTables += ", ";
            }
            m_sFromTables += table.TableName;
        }

        public void AddWhat(DBTable table)
        {
            foreach (KeyValuePair<string, DBField> field in table.m_fields)
            {
                if (String.IsNullOrEmpty(m_sFieldList)) {
                    m_sFieldList += table.TableName + "." + field.Key;
                } else {
                    m_sFieldList += ", " + table.TableName + "." + field.Key;
                }
            }
        }

        public void Add(String sField)
        {
            if (m_sFieldList.Length > 0)
                m_sFieldList += ", ";
            m_sFieldList += sField;
        }

        public static implicit operator String(SQLWhat what)
        {
            return what.m_sFieldList + " from " + what.m_sFromTables;
        }
    };
}