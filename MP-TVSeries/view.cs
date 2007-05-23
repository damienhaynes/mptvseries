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
        string _prettyName = null;
        public static bool cachePrettyName = true;
        //public bool isGroupType = false;
        public string groupedInfo(int step)
        {
            return steps[step].groupedBy.PrettyName;
        }
        public string Name  { get { return this._name; } }
        public string[] loadQuickSettings()
        {
            String optionsSaveString = DBOption.GetOptions("viewsQuickConfig");
            string split = "<;>";
            List<string> viewsConfigs = new List<string>();
            viewsConfigs.AddRange(optionsSaveString.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries));
            for (int i = 0; i < viewsConfigs.Count; i++)
            {
                try
                {
                    string[] Current = viewsConfigs[i].Split(new char[] { ';' }, StringSplitOptions.None);
                    if (Current[0] == Name)
                    {
                        return Current;
                    }
                }
                catch (Exception) { }
            }
            return null;
        }
        public string prettyName
        {
            get 
            {
                if (_prettyName != null && cachePrettyName) return _prettyName;
                else
                {
                    try
                    {
                        string[] quickSettings = loadQuickSettings();
                        if (quickSettings != null)
                        {
                            _prettyName = quickSettings[1];
                            return quickSettings[1];
                        }
                    }
                    catch (Exception){}
                    return Name;
                }
            }
        }

        public bool Enabled
        {
            get
            {
                try 
	            {	        
		            string[] quickSettings = loadQuickSettings();
                    if (quickSettings != null) return quickSettings[2] == "1";
	            }
	            catch (Exception){}
                return true; // by default enabled
            }
        }

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
                        string[] split = DBOnlineEpisode.splitField(tmpItem);
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
        public static List<logicalView> getStaticViews(bool includeDisabled)
        {
            return getAllFromString(DBOptionFake.Get(string.Empty), includeDisabled);
        }

        public static List<logicalView> getAllFromDB(bool includeDisabled)
        {
            return getAllFromString(null, includeDisabled);
        }
        public static List<logicalView> getAllFromString(string fake, bool includeDisabled)
        {
            string[] viewStrings = null;
            if(fake == null)
                viewStrings = System.Text.RegularExpressions.Regex.Split(DBOptionFake.Get("logicalViews"), viewSeperator);
            else
                viewStrings = System.Text.RegularExpressions.Regex.Split(fake, viewSeperator);
            List<logicalView> views = new List<logicalView>();
            foreach (string viewString in viewStrings)
            {
                logicalView view = new logicalView(viewString);
                if (includeDisabled || view.Enabled) views.Add(view);
            }
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
                groupedBy.attempSplit = DBSeries.FieldsRequiringSplit.Contains(groupedBy.rawFieldname);// RequiringSplit; //groupedBy.tableField.ToLower() == "online_series.genre" || groupedBy.tableField.ToLower() == "online_series.actors";
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
                        join = DBEpisode.cSeasonIndex + " = " + DBSeason.Q(DBSeason.cIndex);
                        join += "{local_files}";
                        SubQueryDynInsert_localFilesOnly = " and exists(select filename from local_episodes where seriesid = online_series.id and seasonindex = season.index and episodefilename != '') ";
                        //goto case logicalViewStep.type.series;
                        break;
                    case logicalViewStep.type.series:
                        join += DBEpisode.cSeriesID + " = " + DBOnlineSeries.Q(DBOnlineSeries.cID);
                        join += "{local_files}";
                        SubQueryDynInsert_localFilesOnly = " and exists(select filename from local_episodes where seriesid = online_series.id and episodefilename != '') ";
                        break;
                        //goto default;
                    case logicalViewStep.type.group:
                        join += "{local_files}";
                        SubQueryDynInsert_localFilesOnly = " online_series.haslocalfiles = 1  and exists(select episodefilename from local_episodes where seriesid = online_series.id and episodefilename != '') ";
                        break;
                    default:
                        SubQueryDynInsert_localFilesOnly = " and online_series.haslocalfiles = 1  and exists(select episodefilename from local_episodes where seriesid = online_series.id and compositeid = online_episode.compositeid and episodefilename != '') ";
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
                if(thisView.groupedBy.table.GetType() == typeof(DBSeries))
                    thisView.conds.Add(new DBSeries(), DBSeries.cHidden, 1, SQLConditionType.NotEqual);
                else if(thisView.groupedBy.table.GetType() == typeof(DBOnlineSeries))
                    thisView.conds.AddCustom(" not exists ( select * from " + DBSeries.cTableName + " where id = " + DBOnlineSeries.Q(DBOnlineSeries.cID) + " and " + DBSeries.Q(DBSeries.cHidden) + " = 1)");
                else if (thisView.groupedBy.table.GetType() == typeof(DBSeason))
                    thisView.conds.Add(new DBSeason(), DBSeason.cHidden, 1, SQLConditionType.NotEqual);
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
