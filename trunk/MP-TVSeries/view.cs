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

namespace WindowPlugins.GUITVSeries
{
    public class logicalView
    {
        const string viewSeperator = "<nextView>";
        string _name = string.Empty;
        public bool isGroupType = false;
        public string groupedInfo(int step)
        {
            return steps[step].groupedBy.PrettyName;
        }
        public string Name { get { return this._name; } }
        
        public List<logicalViewStep> steps = new List<logicalViewStep>();

        public List<DBSeries> getSeriesItems(int stepIndex, string[] currentStepSelection)
        {
            MPTVSeriesLog.Write("View: GetSeriesItems: Begin", MPTVSeriesLog.LogLevel.Debug);
            SQLCondition conditions = null;
            if (stepIndex >= steps.Count) return null; // wrong index specified!!
            addHierarchyConditions(ref stepIndex, ref currentStepSelection, ref conditions);
            conditions.Add(new DBSeries(), DBSeries.cHidden, false, SQLConditionType.Equal);
            if(DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cHasLocalFiles, 1, SQLConditionType.Equal);
            MPTVSeriesLog.Write("View: GetSeriesItems: BeginSQL", MPTVSeriesLog.LogLevel.Debug);
            return DBSeries.Get(conditions);
        }

        public logicalViewStep.type gettypeOfStep(int step)
        {
           return steps[step].Type;
        }
        public bool stepHasSeriesBeforeIt(int step)
        {
            if (step >= steps.Count) return false; // wrong index!
            return steps[step].hasSeriesBeforeIt;
        }
        
        public List<DBSeason> getSeasonItems(int stepIndex, string[] currentStepSelection)
        {
            MPTVSeriesLog.Write("View: GetSeason: Begin", MPTVSeriesLog.LogLevel.Debug);
            SQLCondition conditions = null;
            if (stepIndex >= steps.Count) return null; // wrong index specified!!
            addHierarchyConditions(ref stepIndex, ref currentStepSelection, ref conditions);
            MPTVSeriesLog.Write("View: GetSeason: BeginSQL", MPTVSeriesLog.LogLevel.Debug);
            return DBSeason.Get(default(int), DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles), true, false, conditions);
        }

        public List<DBEpisode> getEpisodeItems(int stepIndex, string[] currentStepSelection)
        {
            MPTVSeriesLog.Write("View: GetEps: Begin", MPTVSeriesLog.LogLevel.Debug);
            SQLCondition conditions = null;
            if (stepIndex >= steps.Count) return null; // wrong index specified!!
            addHierarchyConditions(ref stepIndex, ref currentStepSelection, ref conditions);
            if(DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                conditions.Add(new DBEpisode(), DBEpisode.cFilename, string.Empty, SQLConditionType.NotEqual);
            MPTVSeriesLog.Write("View: GetEps: BeginSQL", MPTVSeriesLog.LogLevel.Debug);
            List<DBEpisode> eps = DBEpisode.Get(conditions, true);
            /*
            if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
            {
                List<DBEpisode> goodEps = new List<DBEpisode>();
                foreach (DBEpisode ep in eps)
                    try
                    {
                        if (System.IO.File.Exists(ep[DBEpisode.cFilename]))
                            goodEps.Add(ep);
                    }
                    catch (Exception){}
                return goodEps;
            }
            else 
             */ return eps;
        }

        public List<string> getGroupItems(int stepIndex, string[] currentStepSelection) // in nested groups, eg. Networks-Genres-.. we also need selections
        {
            SQLCondition conditions = null;
            MPTVSeriesLog.Write("View: GetGroupItems: Begin", MPTVSeriesLog.LogLevel.Debug);
            if (stepIndex >= steps.Count) return null; // wrong index specified!!
            addHierarchyConditions(ref stepIndex, ref currentStepSelection, ref conditions);
            logicalViewStep step = steps[stepIndex];
            List<string> items = new List<string>();
            // to ensure we respect on the fly filter settings
            if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) && (typeof(DBOnlineEpisode) != step.groupedBy.table.GetType() && typeof(DBEpisode) != step.groupedBy.table.GetType()))
                conditions.Add(step.groupedBy.table, DBOnlineSeries.cHasLocalFiles, true, SQLConditionType.Equal);
            else if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
            {
                // has to be grouped by something episode
                conditions.Add(new DBEpisode(), DBEpisode.cFilename, "", SQLConditionType.NotEqual);
            }
            
            string sql = "select distinct " + step.groupedBy.tableField + // tablefield includes table name itself!
                                 " from " + step.groupedBy.table.m_tableName + conditions +
                                 step.conds.orderString; // orderstring pointless if actors/genres, so is limitstring (so is limitstring)
            SQLite.NET.SQLiteResultSet results = DBTVSeries.Execute(sql);
            MPTVSeriesLog.Write("View: GetGroupItems: SQL complete", MPTVSeriesLog.LogLevel.Debug);
            //SQLite.NET.SQLiteResultSet results = SQLiteResultSet.Fake();
            if (results.Rows.Count > 0)
            {
                
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    string tmpItem = results.Rows[index].fields[0];
                    // assume we now have a list of all distinct ones
                    if (step.groupedBy.attempSplit)
                    {
                        // we want to try to split by "|" eg. for actors/genres
                        if (tmpItem.Length > 0 && tmpItem[0] == '|') tmpItem = tmpItem.Remove(0, 1);
                        if (tmpItem.Length > 0 && tmpItem[tmpItem.Length - 1] == '|') tmpItem = tmpItem.Remove(tmpItem.Length - 1, 1);
                        tmpItem = tmpItem.Replace(";", "|").Replace(",", "|").Replace("/", "|");
                        string[] split = tmpItem.Split('|');
                        foreach (string item in split)
                            if (item.Trim() == string.Empty)
                                items.Add(Translation.Unknown);
                            else
                                items.Add(item.Trim());
                    }
                    else
                        if (tmpItem.Trim() == string.Empty)
                            items.Add(Translation.Unknown);
                        else
                            items.Add(tmpItem.Trim());
                }
                if (step.groupedBy.attempSplit)
                {
                    // have to check for dups (because we split eg. Drama|Action so "Action" might be in twice
                    items = removeDuplicates(items);
                }
                // now we have to sort them again (Unknown/splitting above)
                items.Sort();
                if (step.groupedBy.attempSplit)
                {
                    // and limit in memory here (agains because those splits are hard to deal with)
                    if (step.limitItems > 0)
                        limitList(ref items, step.limitItems);
                }
            }
            MPTVSeriesLog.Write("View: GetGroupItems: Complete", MPTVSeriesLog.LogLevel.Debug);
            return items;
        }

        public void addHierarchyConditions(ref int stepIndex, ref string[] currentStepSelection, ref SQLCondition conditions)
        {
            logicalViewStep step = steps[stepIndex];
            conditions = step.conds.Copy(); // important, don't change the steps themselves

            // we need to add one additional condition to reflect the selection one hierarchy up
            if (currentStepSelection != null && currentStepSelection.Length > 0 && stepIndex > 0)
            {
                switch (steps[stepIndex - 1].Type)
                {
                    case logicalViewStep.type.group:
                        // we expect to get the selected group's label
                        if (currentStepSelection[0] == Translation.Unknown) // Unknown really is "" so get all with null values here
                            conditions.Add(steps[stepIndex - 1].groupedBy.table, steps[stepIndex - 1].groupedBy.rawFieldname, "", SQLConditionType.Equal);
                        else 
                            if(steps[stepIndex - 1].groupedBy.attempSplit) // because we split distinct group values such as Drama|Action we can't do an equal compare, use like instead
                                conditions.Add(steps[stepIndex - 1].groupedBy.table, steps[stepIndex - 1].groupedBy.rawFieldname, currentStepSelection[0], SQLConditionType.Like);
                            else
                                conditions.Add(steps[stepIndex - 1].groupedBy.table, steps[stepIndex - 1].groupedBy.rawFieldname, currentStepSelection[0], SQLConditionType.Equal);
                        break;
                    case logicalViewStep.type.series:
                        // we expect to get the seriesID as stepSel
                        conditions.Add(new DBSeason(), DBSeason.cSeriesID, currentStepSelection[0], SQLConditionType.Equal);
                        break;
                    case logicalViewStep.type.season:
                        // we expect to get the seriesID/seasonIndex as stepSel
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, currentStepSelection[0], SQLConditionType.Equal);
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, currentStepSelection[1], SQLConditionType.Equal);
                        break;
                }
            }
            if (step.hasSubQuery && step.SubQueryDynInsert_localFilesOnly != string.Empty)
            {
                if(DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                {
                    conditions.ConditionsSQLString = conditions.ConditionsSQLString.Replace("{local_files}", step.SubQueryDynInsert_localFilesOnly);
                }
                else
                {
                    conditions.ConditionsSQLString = conditions.ConditionsSQLString.Replace("{local_files}", " ");
                }

            }
        }

        public static List<string> removeDuplicates(List<string> inputList)
        {
            Dictionary<string, int> uniqueStore = new Dictionary<string, int>();
            List<string> finalList = new List<string>();
            foreach (string currValue in inputList)
            {
                if (!uniqueStore.ContainsKey(currValue))
                {
                    uniqueStore.Add(currValue, 0);
                    finalList.Add(currValue);
                }
            }
            return finalList;
        }

        static void limitList(ref List<string> list, int limit)
        {
            if(limit >= list.Count) return;
            list.RemoveRange(list.Count - (list.Count - limit), (list.Count - limit));
        }

        public logicalView(string fromDB)
        {
            string[] steps = System.Text.RegularExpressions.Regex.Split(fromDB, logicalViewStep.stepSeperator);
            bool hasSeriesBeforeIt = false;
            this._name = steps[0].Split(new string[] { "<name>" }, StringSplitOptions.RemoveEmptyEntries)[0];
            steps[0] = steps[0].Split(new string[] { "<name>" }, StringSplitOptions.RemoveEmptyEntries)[1];
            for (int i = 0; i < steps.Length; i++)
            {
                this.steps.Add(logicalViewStep.parseFromDB(steps[i], hasSeriesBeforeIt));
                //if (this.steps[i].Type == logicalViewStep.type.group) isGroupType = true;
                // inherit the conditions, so each step will always have all the conditions from steps before it!
                if (i > 0)
                {
                    foreach (string condsToInh in this.steps[i - 1].conditionsToInherit)
                    {
                        this.steps[i].addInheritedConditions(condsToInh);
                    }
                }
                // so lists can query if they'll have to append the seriesname in episode view (when no series was selected, eg. Flat View
                if (this.steps[i].Type == logicalViewStep.type.series)
                    hasSeriesBeforeIt = true;
            }
        }
        /// <summary>
        /// Fakes user defined views with hardcoded ones, to be removed once user configuration has been set upt!!!
        /// </summary>
        /// <returns></returns>
        public static List<logicalView> getStaticViews()
        {
            return getAllFromDB(DBOptionFake.Get(string.Empty));
        }

        public static List<logicalView> getAllFromDB()
        {
            return getAllFromDB(null);
        }
        public static List<logicalView> getAllFromDB(string fake)
        {
            string[] viewStrings = null;
            if(fake == null)
                viewStrings = System.Text.RegularExpressions.Regex.Split(DBOptionFake.Get("logicalViews"), viewSeperator);
            else
                viewStrings = System.Text.RegularExpressions.Regex.Split(fake, viewSeperator);
            List<logicalView> views = new List<logicalView>();
            foreach (string viewString in viewStrings)
                views.Add(new logicalView(viewString));
            return views;
        }
    }

    public class logicalViewStep
    {
        // steps are in db as: type<;>condition;=720<cond>condition;=520<;>orderField;desc<;>limit
        // groups look so: "group:<Series.Network><;><Series.isFavourite>;=;1<cond>condition2;=;520<;>orderField;desc;orderField2;asc<;>15";
        public const string stepSeperator = "<nextStep>";
        const string intSeperator = "<;>";
        const string condSeperator = "<cond>";
        public enum type
        {
            group,
            series,
            season,
            episode
        }
        public string Name
        {
            get
            {
                return Type.ToString();
            }
        }
        public class grouped
        {
            public DBTable table;
            public string tableField;
            public string PrettyName;
            public string rawFieldname;

            public bool attempSplit = false;
        }
        public List<string> conditionsToInherit = new List<string>();
        public type Type;
        public int limitItems = 0;
        public bool hasSubQuery = false;
        public string SubQueryDynInsert_localFilesOnly = string.Empty;

        public SQLCondition conds = new SQLCondition();
        public grouped groupedBy = null;
        public bool hasSeriesBeforeIt = false;
        
        string getQTableNameFromUnknownType(DBTable table, string field)
        {
            return (string)table.GetType().InvokeMember("Q", System.Reflection.BindingFlags.InvokeMethod, null, table, new object[1] { field });
        }
        void setType(string typeString)
        {
            if (typeString.Contains(type.group.ToString()))
            {
                this.Type = type.group;
                groupedBy = new grouped();
                groupedBy.PrettyName = typeString.Split(':')[1];
                getTableFieldname(typeString.Split(':')[1], out groupedBy.table, out groupedBy.rawFieldname);
                groupedBy.tableField = getQTableNameFromUnknownType(groupedBy.table, groupedBy.rawFieldname);
                groupedBy.attempSplit = groupedBy.tableField.ToLower() == "online_series.genre" || groupedBy.tableField.ToLower() == "online_series.actors";
            }
            else if (typeString == type.series.ToString())
                this.Type = type.series;
            else if (typeString == type.season.ToString())
                this.Type = type.season;
            else if (typeString == type.episode.ToString())
                this.Type = type.episode;
            else this.Type = type.series; // this should never happen!
        }

        public void addSQLCondition(string what, string type, string condition)
        {
            if ((!what.Contains("<") || !what.Contains(".")) && !what.Contains("custom:")) return;


            SQLConditionType condtype;
            switch (type)
            {
                case "=":
                    condtype = SQLConditionType.Equal;
                    break;
                case ">":
                    condtype = SQLConditionType.GreaterThan;
                    break;
                case ">=":
                    condtype = SQLConditionType.GreaterThan;
                    break;
                case "<":
                    condtype = SQLConditionType.LessThan;
                    break;
                case "<=":
                    condtype = SQLConditionType.LessThan;
                    break;
                case "!=":
                    condtype = SQLConditionType.NotEqual;
                    break;
                default:
                    condtype = SQLConditionType.Equal;
                    break;
            }
            DBTable table = null;
            string tableField = string.Empty;
            getTableFieldname(what, out table, out tableField);
            Type lType = table.GetType();
            bool cust = false;
            string join = string.Empty;
            if ((lType == typeof(DBEpisode) || lType == typeof(DBOnlineEpisode)) && Type != logicalViewStep.type.episode)
            {
                cust = true;
                // we also need to ensure that the user selected only_local_files is respected (subquery otherwise screws up)
                // however this cannot be done here as the steps are only parsed at the start of the plugin and the user may
                // very well change this setting later on
                switch (this.Type)
                {
                    case logicalViewStep.type.season:
                        join = DBEpisode.cSeasonIndex + " = " + DBSeason.Q(DBSeason.cIndex) + " and ";
                        goto case logicalViewStep.type.series;
                    case logicalViewStep.type.series:
                        join += DBEpisode.cSeriesID + " = " + DBOnlineSeries.Q(DBOnlineSeries.cID);
                        join += "{local_files}";
                        goto default;
                    case logicalViewStep.type.group:
                        join += "{local_files}";
                        SubQueryDynInsert_localFilesOnly = " online_series.haslocalfiles = 1 "; // and exists(select episodefilename from local_episodes where seriesid = online_series.id and episodefilename != '') ";
                        break;
                    default:
                        SubQueryDynInsert_localFilesOnly = " and online_series.haslocalfiles = 1 "; // and exists(select episodefilename from local_episodes where seriesid = online_series.id and episodefilename != '') ";
                        //SubQueryDynInsert_localFilesOnly = " and exists ( select * from local_episodes where compositeid = online_episodes.compositeid and episodefilename != '')";
                        break;
                }
                
            }
            else if (lType.Equals(typeof(DBSeason)) && ( Type != logicalViewStep.type.season && Type != logicalViewStep.type.episode))
            {
                cust = true;
                if (this.Type == logicalViewStep.type.series)
                {
                    join =  DBSeason.cSeriesID + " = " + DBOnlineSeries.Q(DBOnlineSeries.cID); 
                }
            }

            
            if (what.Contains("custom:"))
            {
                conds.AddCustom(what.Split(new string[] { "custom:" }, StringSplitOptions.None)[1], condition.Trim(), condtype);
            }
            else if (cust)
            {
                SQLCondition iCond = new SQLCondition();
                iCond.AddCustom(join);
                iCond.Add(table, tableField, condition.Trim(), condtype);
                
                conds.AddSubQuery("count(" + tableField + ")", table, iCond, 0, SQLConditionType.GreaterThan);
                hasSubQuery = true;
                
            }
            else
            {
                conds.Add(table, tableField, condition.Trim(), condtype);
            }
        }

        void getTableFieldname(string what, out DBTable table, out string fieldname)
        {
            string sTable = string.Empty;
            fieldname = string.Empty;
            table = null;
            what = what.Replace("<", "").Replace(">", "").Trim();
            sTable = what.Split('.')[0];
            switch (sTable)
            {
                case "Series":
                    if(new DBOnlineSeries().FieldNames.Contains(what.Split('.')[1]))
                    {
                        table = new DBOnlineSeries();
                        fieldname = what.Split('.')[1];
                    }
                    else
                    {
                        table = new DBSeries();
                        fieldname = what.Split('.')[1];
                    }
                    break;
                case "Season":
                    table = new DBSeason();
                    fieldname = what.Split('.')[1];
                    break;
                case "Episode":
                    if (new DBOnlineEpisode().FieldNames.Contains(what.Split('.')[1]))
                    {
                        table = new DBOnlineEpisode();
                        fieldname = what.Split('.')[1];
                    }
                    else
                    {
                        table = new DBEpisode();
                        fieldname = what.Split('.')[1];
                    }
                    break;
            }
        }

        public void addInheritedConditions(string conditions)
        {
            addConditionsFromString(conditions);
        }

        void addConditionsFromString(string allConditionsAsString)
        {
            if (allConditionsAsString.Length == 0) return;
            string[] allConditions = System.Text.RegularExpressions.Regex.Split(allConditionsAsString, condSeperator);
            for (int i = 0; i < allConditions.Length; i++)
            {
                string[] condSplit = System.Text.RegularExpressions.Regex.Split(allConditions[i], ";");
                addSQLCondition(condSplit[0], condSplit[1], condSplit[2].Replace("\"", "").Replace("'", ""));
            }
            conditionsToInherit.Add(allConditionsAsString);
        }

        public static logicalViewStep parseFromDB(string viewStep, bool hasSeriesBeforeIt)
        {
            logicalViewStep thisView = new logicalViewStep();
            thisView.hasSeriesBeforeIt = hasSeriesBeforeIt;
            string[] viewSteps = System.Text.RegularExpressions.Regex.Split(viewStep, intSeperator);
            thisView.setType(viewSteps[0]);
            thisView.addConditionsFromString(viewSteps[1]);
            if (viewSteps[2].Length > 0)
            {
                string[] orderFields = System.Text.RegularExpressions.Regex.Split(viewSteps[2], ";");
                for (int i = 0; i < orderFields.Length; i += 2)
                {
                    if(thisView.Type != type.group)
                    {
                        DBTable table = null;
                        string tableField = string.Empty;
                        thisView.getTableFieldname(orderFields[i], out table, out tableField);
                        tableField = thisView.getQTableNameFromUnknownType(table, tableField);
                        if (thisView.Type == type.episode && ( table.GetType() == typeof(DBEpisode) || table.GetType() == typeof(DBOnlineEpisode)))
                        {
                            // for perf reason a subquery is build, otherwise custom orders and the nessesary join really slow down sqllite!
                            SQLCondition subQueryConditions = thisView.conds.Copy(); // have to have all conds too
                            subQueryConditions.AddOrderItem(tableField, (orderFields[i + 1] == "asc" ? SQLCondition.orderType.Ascending : SQLCondition.orderType.Descending));
                            if (viewSteps[3].Length > 0) // set the limit too
                            {
                                try
                                {
                                    subQueryConditions.SetLimit(System.Convert.ToInt32(viewSteps[3]));
                                }
                                catch (Exception)
                                {
                                }
                            }
                            thisView.conds.AddSubQuery("compositeid", table, subQueryConditions, table.m_tableName + "." + DBEpisode.cCompositeID, SQLConditionType.In);
                            
                        }
                        thisView.conds.AddOrderItem(tableField, (orderFields[i + 1] == "asc" ? SQLCondition.orderType.Ascending : SQLCondition.orderType.Descending));
                    }
                }
            }
            if (thisView.Type == type.group && thisView.groupedBy != null) // for groups always by their values (ignore user setting!)
            {
                thisView.conds.AddOrderItem(thisView.groupedBy.tableField, SQLCondition.orderType.Descending); // tablefield includes tablename itself!
            }
            try
            {
                if (viewSteps[3].Length > 0)
                {
                    thisView.limitItems = System.Convert.ToInt32(viewSteps[3]);
                    thisView.conds.SetLimit(thisView.limitItems);
                }
            }
            catch (Exception)
            {
                MPTVSeriesLog.Write("Cannot interpret limit in logicalview, limit was: " + viewSteps[3]);
            }
            return thisView;
        }
    }

    public class Helper
    {
        public static List<T> getFilteredList<T, P>(List<T> inputList, string PropertyName, P ValueOfProperty)
        {
            List<T> resultList = new List<T>();
            foreach (T item in inputList)
            {
                if( ValueOfProperty.Equals(((P)item.GetType().InvokeMember(PropertyName, System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.GetProperty, null, item, null))))
                    resultList.Add(item);
            }
            return resultList;
        }

        public static T getElementFromList<T, P>(P currPropertyValue, string PropertyName, int indexOffset, List<T> elements)
        {
            // takes care of "looping"
            if (elements.Count == 0) return default(T);
            int indexToGet = 0;
            P value = default(P);
            for (int i = 0; i < elements.Count; i++)
            {
                try
                {
                    value = (P)elements[i].GetType().InvokeMember(PropertyName, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField, null, elements[i], null);
                    if (value.Equals(currPropertyValue))
                    {
                        indexToGet = i + indexOffset;
                        break;
                    }
                }
                catch (Exception x)
                {
                    MPTVSeriesLog.Write("Wrong call of getElementFromList<T,P>: the Type " + elements[i].GetType().Name + " - " + x.Message);
                    return default(T);
                }
            }
            if (indexToGet < 0) indexToGet = elements.Count + indexToGet;
            if (indexToGet >= elements.Count) indexToGet = indexToGet - elements.Count;
            return elements[indexToGet];
        }

        public static List<string> getFieldNameListFromList<T>(string FieldNameToGet, List<T> elements) where T:DBTable
        {
            MPTVSeriesLog.Write(elements.Count.ToString());
            List<string> results = new List<string>();
            foreach (T elem in elements)
            {
                try
                {
                    results.Add((string)elem[FieldNameToGet]);
                }
                catch (Exception)
                {
                    MPTVSeriesLog.Write("Wrong call of getPropertyListFromList<T,P>: Type " + elem.GetType().Name);
                }
            }
            return results;
        }

        public static List<P> getPropertyListFromList<T, P>(string PropertyNameToGet, List<T> elements)
        {
            List<P> results = new List<P>();
            foreach (T elem in elements)
            {
                try
                {
                    results.Add((P)elem.GetType().InvokeMember(PropertyNameToGet, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField, null, elem, null));
                }
                catch (Exception)
                {
                    MPTVSeriesLog.Write("Wrong call of getPropertyListFromList<T,P>: Type " + elem.GetType().Name);
                }
            }
            return results;
        }

        # region compareAndAdaptList
        // needed for compareAndAdaptList because we cant pass parameters into predicates??
        class classify<t>
        {
            List<t> compareCollection = null;
            addOperationOnRemovedItemsDelegate<t> AdditionOperation = null;
            addOperationOnItemBeforeCompare<t> addOperationOnItemBeforeCompare = null;
            bool inverse = false;

            public classify(List<t> compareCollection, addOperationOnItemBeforeCompare<t> addOperationOnItemBeforeCompare, addOperationOnRemovedItemsDelegate<t> AdditionOperation, bool inverse)
            {
                this.compareCollection = compareCollection;
                this.AdditionOperation = AdditionOperation;
                this.addOperationOnItemBeforeCompare = addOperationOnItemBeforeCompare;
                this.inverse = inverse;
            }

            public bool isNotInCompareList(t item)
            {
                if (!inverse ? !compareCollection.Contains(addOperationOnItemBeforeCompare(item))
                    : compareCollection.Contains(addOperationOnItemBeforeCompare(item)))
                    return AdditionOperation(item);
                else return false;
            }
        }
        public delegate bool addOperationOnRemovedItemsDelegate<t>(t item);
        public delegate t addOperationOnItemBeforeCompare<t>(t item);
        public static int compareAndAdaptList<t>(ref List<t> ANDAdaptList, List<t> compareList, addOperationOnItemBeforeCompare<t> addOperationOnItemBeforeCompare, addOperationOnRemovedItemsDelegate<t> addOperationOnRemovedItems, bool inverse)
        {
            classify<t> classif = new classify<t>(compareList, addOperationOnItemBeforeCompare, addOperationOnRemovedItems, inverse);
            return ANDAdaptList.RemoveAll(classif.isNotInCompareList);
        }
        public static int compareAndAdaptList<t>(ref List<t> ANDAdaptList, List<t> compareList, addOperationOnItemBeforeCompare<t> addOperationOnItemBeforeCompare, addOperationOnRemovedItemsDelegate<t> addOperationOnRemovedItems)
        {
            return compareAndAdaptList<t>(ref ANDAdaptList, compareList, addOperationOnItemBeforeCompare, addOperationOnRemovedItems, false);
        }
        public static int compareAndAdaptList<t>(ref List<t> ANDAdaptList, List<t> compareList, bool inverse)
        {
            addOperationOnItemBeforeCompare<t> addOp = delegate(t item){return item;};
            addOperationOnRemovedItemsDelegate<t> onRemove = delegate(t item) { return true; };
            return compareAndAdaptList<t>(ref ANDAdaptList, compareList, addOp, onRemove, inverse);
        }
        #endregion

        public static DBSeries getCorrespondingSeries(int id)
        {
            SQLCondition cond = new SQLCondition();
            cond.Add(new DBSeries(), DBSeries.cID, id, SQLConditionType.Equal);
            List<DBSeries> tmpSeries = DBSeries.Get(cond);
            foreach (DBSeries series in tmpSeries) // should only be one!
                if (series[DBSeries.cID] == id)
                {
                    return series;
                }
            return null;
        }

        public static DBSeason getCorrespondingSeason(int seriesID, int seasonIndex)
        {
            List<DBSeason> tmpSeasons = DBSeason.Get(seriesID, false, false, false);
            foreach (DBSeason season in tmpSeasons)
                if (season[DBSeason.cIndex] == seasonIndex)
                {
                    return season;
                }
            return null;
        }

        public static string PathCombine(string path1, string path2)
        {
            if (path1 == null && path2 == null) return string.Empty;
            if (path1 == null) return path2;
            if (path2 == null) return path1;
            if(path2.Length > 0 && ( path2[0] == '\\' || path2[0] == '/')) path2 = path2.Substring(1);
            return System.IO.Path.Combine(path1, path2);
        }
    }

    public class DBOptionFake
    {
        public static string Get(string jkld)
        {
            return "Genres<name>group:<Series.Genre><;><;><;>15" +
                "<nextStep>series<;><;><Series.Pretty_Name>;asc<;>" +
                "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>"
                + "<nextView>" +
                "All<name>series<;><;><Series.Pretty_Name>;asc<;>" +
                "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>"
                + "<nextView>" +
                "Latest<name>episode<;><;><Episode.FirstAired>;desc<;>25"
                + "<nextView>" +
                "Channels<name>group:<Series.Network><;><;><;>15" +
                "<nextStep>series<;><;><Series.Pretty_Name>;asc<;>" +
                "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>"
                + "<nextView>" +
                "Unwatched<name>series<;><Episode.Watched>;=;0<;><Series.Pretty_Name>;asc<;>" +
                //"Unwatched<name>series<;>custom:(select count(watched) from online_episodes where seriesID = online_series.ID and watched = 0);>;0<;><Series.Pretty_Name>;asc<;>" +
                "<nextStep>season<;>;;<;><Season.seasonIndex>;asc<;>" +
                //"<nextStep>season<;>custom:(select count(watched) from online_episodes where seriesID = season.seriesID and seasonindex =  season.seasonindex and watched = 0);>;0<;><Season.seasonIndex>;asc<;>" +
                "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>"
                + "<nextView>" +
                "Favourites<name>series<;><Series.isFavourite>;=;1<;><Series.Pretty_Name>;asc<;>" +
                "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>"
                ;
        }
    }

}
