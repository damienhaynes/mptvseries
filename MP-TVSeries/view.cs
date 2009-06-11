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
        const string m_viewSeperator = "<nextView>";
        string m_name = string.Empty;
        string m_prettyName = null;
        public static bool s_cachePrettyName = true;

        public static bool IsLocked { get; set; }
        
        //public bool isGroupType = false;
        public string groupedInfo(int step)
        {
            return m_steps[step].groupedBy.PrettyName;
        }
        public string Name  { get { return this.m_name; } }
        public string prettyName
        {
            get 
            {
                return m_prettyName;
            }
            set
            {
                this.m_prettyName = value;
            }
        }
        public string m_uniqueID = string.Empty;
        public bool m_Enabled = true;
        public bool IsTaggedView { get; set; }
        public bool ParentalControl { get; set; }        

        DBView m_toUpdateForConfig = null;

        public List<logicalViewStep> m_steps = new List<logicalViewStep>();

        public List<DBSeries> getSeriesItems(int stepIndex, string[] currentStepSelection)
        {
            MPTVSeriesLog.Write("View: GetSeriesItems: Begin", MPTVSeriesLog.LogLevel.Debug);
            SQLCondition conditions = null;
            if (stepIndex >= m_steps.Count) return null; // wrong index specified!!
            addHierarchyConditions(ref stepIndex, ref currentStepSelection, ref conditions);
            MPTVSeriesLog.Write("View: GetSeriesItems: BeginSQL", MPTVSeriesLog.LogLevel.Debug);
            return DBSeries.Get(conditions);
        }

        public logicalViewStep.type gettypeOfStep(int step)
        {
           return m_steps[step].Type;
        }
        public bool stepHasSeriesBeforeIt(int step)
        {
            if (step >= m_steps.Count) return false; // wrong index!
            return m_steps[step].hasSeriesBeforeIt;
        }
        
        public List<DBSeason> getSeasonItems(int stepIndex, string[] currentStepSelection)
        {
            MPTVSeriesLog.Write("View: GetSeason: Begin", MPTVSeriesLog.LogLevel.Debug);
            SQLCondition conditions = null;
            if (stepIndex >= m_steps.Count) return null; // wrong index specified!!
            addHierarchyConditions(ref stepIndex, ref currentStepSelection, ref conditions);
            MPTVSeriesLog.Write("View: GetSeason: BeginSQL", MPTVSeriesLog.LogLevel.Debug);
            //return DBSeason.Get(default(int), DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles), true, false, conditions);
            return DBSeason.Get(conditions);
        }

        public List<DBEpisode> getEpisodeItems(int stepIndex, string[] currentStepSelection)
        {
            MPTVSeriesLog.Write("View: GetEps: Begin", MPTVSeriesLog.LogLevel.Debug);
            SQLCondition conditions = null;
            if (stepIndex >= m_steps.Count) return null; // wrong index specified!!
            addHierarchyConditions(ref stepIndex, ref currentStepSelection, ref conditions);
            
            MPTVSeriesLog.Write("View: GetEps: BeginSQL", MPTVSeriesLog.LogLevel.Debug);
            List<DBEpisode> eps = DBEpisode.Get(conditions);

            // WARNING: this naturally only works if the ordering is by season/episodeOrder
            // inline the special episodes to there relevant positions (Season == 0 by airsbefore_episode)
            if (m_steps[stepIndex].inLineSpecials && currentStepSelection[currentStepSelection.Length - 1] != "0")
            {
                if (m_steps[stepIndex].inLineSpecialsAsc) eps = Helper.inverseList<DBEpisode>(eps);
                Comparison<DBEpisode> inlineSorting = delegate(DBEpisode e1, DBEpisode e2)
                    {
                        return getRelSortingIndexOfEp(e1).CompareTo(getRelSortingIndexOfEp(e2));
                    };
                eps.Sort(inlineSorting);
            }
            return eps;
        }

        double getRelSortingIndexOfEp(DBEpisode ep)
        {
            if (ep[DBEpisode.cSeasonIndex] == 0)
            {
                if (ep[DBOnlineEpisode.cAirsAfterSeason] != string.Empty && ep[DBOnlineEpisode.cAirsBeforeEpisode] == string.Empty)
                {
                    return 9999 + ep[DBOnlineEpisode.cEpisodeIndex];
                }
                else
                    return ((int)ep[DBOnlineEpisode.cAirsBeforeEpisode]) - 0.9 + (((int)ep[DBOnlineEpisode.cEpisodeIndex]) / 100f) + (ep[DBOnlineEpisode.cAirsBeforeSeason] * 100);
            }
            else
                return ((int)ep[DBEpisode.cEpisodeIndex] + ep[DBOnlineEpisode.cSeasonIndex]*100);
        }

        /*
         * Returns a list of all episodes that are available from the specified level with the
         * given currentStepSelection (the selection from the previous level).
         */
        public List<DBEpisode> getAllEpisodesForStep(int stepIndex, string[] currentStepSelection)
        {
            // make sure we have been given a valid step number
            if ((stepIndex >= m_steps.Count) || (stepIndex < 0))
                return null;

            // our return object
            List<DBEpisode> episodeList = new List<DBEpisode>();

            // determine which level we are at, and if we are not at the episode level
            // make recursive calls to step down to the next level for each possible selection
            // adding the return results (a list of episodes) to the episodeList
            logicalViewStep.type stepType = gettypeOfStep(stepIndex);
            switch (stepType)
            {

                // if we are at the group level we need to go deeper, so just request all
                // episodes for each visible group and add them all to the list
                case logicalViewStep.type.group:
                    List<String> groupList = getGroupItems(stepIndex, currentStepSelection);
                    foreach (String currGroupName in groupList)
                    {
                        string[] stepSelection = new string[] { currGroupName };
                        List<DBEpisode> episodeSubset = getAllEpisodesForStep(stepIndex + 1, stepSelection);
                        if (episodeSubset != null)
                            episodeList.AddRange(episodeSubset);
                    }
                    return episodeList;

                // if we are at the series level, we need to go deeper
                case logicalViewStep.type.series:
                    List<DBSeries> seriesList = getSeriesItems(stepIndex, currentStepSelection);
                    foreach (DBSeries currSeries in seriesList)
                    {
                        string[] stepSelection = new string[] { currSeries[DBSeries.cID].ToString() };
                        List<DBEpisode> episodeSubset = getAllEpisodesForStep(stepIndex + 1, stepSelection);
                        if (episodeSubset != null)
                            episodeList.AddRange(episodeSubset);
                    }
                    return episodeList;

                // keep digging!
                case logicalViewStep.type.season:
                    List<DBSeason> seasonList = getSeasonItems(stepIndex, currentStepSelection);
                    foreach (DBSeason currSeason in seasonList)
                    {
                        string[] stepSelection = new string[] { currSeason[DBSeason.cSeriesID].ToString(), currSeason[DBSeason.cIndex].ToString() };
                        List<DBEpisode> episodeSubset = getAllEpisodesForStep(stepIndex + 1, stepSelection);
                        if (episodeSubset != null)
                            episodeList.AddRange(episodeSubset);
                    }
                    return episodeList;

                // something we know. return the list of episodes visible at the given level
                case logicalViewStep.type.episode:
                    episodeList = getEpisodeItems(stepIndex, currentStepSelection);
                    return episodeList;
            }

            // we should never get to this point. this indicates there 
            // is a new, unaccounted for step type.
            MPTVSeriesLog.Write("WARNING: Unhandled logicalViewStep.type enum in logicalView.getAllEpisodesForStep()");
            return null;
        }


        public List<string> getGroupItems(int stepIndex, string[] currentStepSelection) // in nested groups, eg. Networks-Genres-.. we also need selections
        {
            SQLCondition conditions = null;
            MPTVSeriesLog.Write("View: GetGroupItems: Begin", MPTVSeriesLog.LogLevel.Debug);
            if (stepIndex >= m_steps.Count) return null; // wrong index specified!!
            addHierarchyConditions(ref stepIndex, ref currentStepSelection, ref conditions);
            logicalViewStep step = m_steps[stepIndex];
            List<string> items = new List<string>();
            // to ensure we respect on the fly filter settings
            if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) && (typeof(DBOnlineEpisode) != step.groupedBy.table.GetType() && typeof(DBEpisode) != step.groupedBy.table.GetType()))
            {
                // not generic
                SQLCondition fullSubCond = new SQLCondition();
                fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBOnlineSeries.Q(DBOnlineSeries.cID), SQLConditionType.Equal);
                conditions.AddCustom(" exists( " + DBEpisode.stdGetSQL(fullSubCond, false) + " )");
            }
            else if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
            {
                // has to be grouped by something episode
                conditions.Add(new DBEpisode(), DBEpisode.cFilename, "", SQLConditionType.NotEqual);
            }
            
            string sql = "select distinct " + step.groupedBy.tableField + // tablefield includes table name itself!
                                 " , count(*) " +
                                 " from " + step.groupedBy.table.m_tableName + conditions +
                                 " group by " + step.groupedBy.tableField +
                                 step.conds.orderString; // orderstring pointless if actors/genres, so is limitstring (so is limitstring)
            SQLite.NET.SQLiteResultSet results = DBTVSeries.Execute(sql);
            MPTVSeriesLog.Write("View: GetGroupItems: SQL complete", MPTVSeriesLog.LogLevel.Debug);
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
                            if (item.Trim().Length == 0)
                                items.Add(Translation.Unknown);
                            else
                                items.Add(item.Trim());
                    }
                    else
                        if (tmpItem.Trim().Length == 0)
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
                    // and limit in memory here (again because those splits are hard to deal with)
                    if (step.limitItems > 0)
                        limitList(ref items, step.limitItems);
                }
            }
            MPTVSeriesLog.Write("View: GetGroupItems: Complete", MPTVSeriesLog.LogLevel.Debug);
            return items;
        }

        public void addHierarchyConditions(ref int stepIndex, ref string[] currentStepSelection, ref SQLCondition conditions)
        {
            logicalViewStep step = m_steps[stepIndex];
            conditions = step.conds.Copy(); // important, don't change the steps themselves

            // we need to add one additional condition to reflect the selection one hierarchy up
            if (currentStepSelection != null && currentStepSelection.Length > 0 && stepIndex > 0)
            {
                switch (m_steps[stepIndex - 1].Type)
                {
                    case logicalViewStep.type.group:
                        // we expect to get the selected group's label
                        if (currentStepSelection[0] == Translation.Unknown) // Unknown really is "" so get all with null values here
                            conditions.Add(m_steps[stepIndex - 1].groupedBy.table, m_steps[stepIndex - 1].groupedBy.rawFieldname, "", SQLConditionType.Equal);
                        else 
                            if(m_steps[stepIndex - 1].groupedBy.attempSplit) // because we split distinct group values such as Drama|Action we can't do an equal compare, use like instead
                                conditions.Add(m_steps[stepIndex - 1].groupedBy.table, m_steps[stepIndex - 1].groupedBy.rawFieldname, currentStepSelection[0], SQLConditionType.Like);
                            else
                                conditions.Add(m_steps[stepIndex - 1].groupedBy.table, m_steps[stepIndex - 1].groupedBy.rawFieldname, currentStepSelection[0], SQLConditionType.Equal);
                        break;
                    case logicalViewStep.type.series:
                        // we expect to get the seriesID as stepSel
                        conditions.Add(new DBSeason(), DBSeason.cSeriesID, currentStepSelection[0], SQLConditionType.Equal);
                        break;
                    case logicalViewStep.type.season:
                        // we expect to get the seriesID/seasonIndex as stepSel
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, currentStepSelection[0], SQLConditionType.Equal);
                        conditions.beginGroup();
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, currentStepSelection[1], SQLConditionType.Equal);
                        if (DBOption.GetOptions(DBOption.cSortSpecials)) {
                            conditions.nextIsOr = true;
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cAirsBeforeSeason, currentStepSelection[1], SQLConditionType.Equal);
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cAirsAfterSeason, currentStepSelection[1], SQLConditionType.Equal);
                            conditions.nextIsOr = false;
                        }
                        conditions.endGroup();
                        break;
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

        public logicalView(DBView fromDB)
        {
            string[] steps = System.Text.RegularExpressions.Regex.Split(fromDB[DBView.cViewConfig], logicalViewStep.s_stepSeperator);
            bool hasSeriesBeforeIt = false;

            this.m_name = fromDB[DBView.cTransToken];
            this.m_prettyName = fromDB[DBView.cPrettyName].ToString().Length == 0 ? Translation.Get(this.m_name) : (String)fromDB[DBView.cPrettyName];
            this.m_uniqueID = fromDB[DBView.cIndex];
            this.m_Enabled = fromDB[DBView.cEnabled];
            this.IsTaggedView = fromDB[DBView.cTaggedView];
            this.ParentalControl = fromDB[DBView.cParentalControl];

            if (Settings.isConfig) m_toUpdateForConfig = fromDB;

            //steps[0] = steps[0].Split(new string[] { "<name>" }, StringSplitOptions.RemoveEmptyEntries)[1];
            for (int i = 0; i < steps.Length; i++)
            {
                this.m_steps.Add(logicalViewStep.parseFromDB(steps[i], hasSeriesBeforeIt));
                //if (this.steps[i].Type == logicalViewStep.type.group) isGroupType = true;
                // inherit the conditions, so each step will always have all the conditions from steps before it!
                if (i > 0)
                {
                    foreach (string condsToInh in this.m_steps[i - 1].conditionsToInherit)
                    {
						// Don't inherit series conditions in season/episode view
						// 50 Times slower getting episode list if you do!!
						// There is no need to inherit at the Episode step if Series step exists
						if (!(condsToInh.Contains("<Series") && hasSeriesBeforeIt)) {
							this.m_steps[i].addInheritedConditions(condsToInh);
						}
                    }
                }

                // so lists can query if they'll have to append the seriesname in episode view (when no series was selected, eg. Flat View
                if (this.m_steps[i].Type == logicalViewStep.type.series)
                    hasSeriesBeforeIt = true;
            }
        }

        public static List<logicalView> getAll(bool includeDisabled)
        {
            List<logicalView> views = new List<logicalView>();
            foreach (DBView view in DBView.getAll(includeDisabled))
                views.Add(new logicalView(view));
            return views;
        }

        public void saveToDB()
        {
            m_toUpdateForConfig[DBView.cIndex] = m_uniqueID;
            m_toUpdateForConfig[DBView.cEnabled] = m_Enabled;
            m_toUpdateForConfig[DBView.cTransToken] = m_name;
            m_toUpdateForConfig[DBView.cPrettyName] = m_prettyName != Translation.Get(m_name) ? prettyName : string.Empty;
            m_toUpdateForConfig[DBView.cParentalControl] = ParentalControl;
            m_toUpdateForConfig.Commit();
        }
    }

    public class logicalViewStep
    {
        // steps are in db as: type<;>condition;=720<cond>condition;=520<;>orderField;desc<;>limit
        // groups look so: "group:<Series.Network><;><Series.isFavourite>;=;1<cond>condition2;=;520<;>orderField;desc;orderField2;asc<;>15";
        public const string s_stepSeperator = "<nextStep>";
        const string s_intSeperator = "<;>";
        const string s_condSeperator = "<cond>";
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
        public bool inLineSpecials = false;
        public bool inLineSpecialsAsc = false;
        public string SubQueryDynInsert_localFilesOnly = string.Empty;

        public SQLCondition conds = new SQLCondition();
        public grouped groupedBy = null;
        public bool hasSeriesBeforeIt = false;
        
        static string getQTableNameFromUnknownType(DBTable table, string field)
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
                    condtype = SQLConditionType.GreaterEqualThan;
                    break;
                case "<":
                    condtype = SQLConditionType.LessThan;
                    break;
                case "<=":
                    condtype = SQLConditionType.LessEqualThan;
                    break;
                case "!=":
                    condtype = SQLConditionType.NotEqual;
                    break;
				case "like":
					condtype = SQLConditionType.Like;
					break;
                default:
                    condtype = SQLConditionType.Equal;
                    break;
            }

            DBTable table = null;
            string tableField = string.Empty;
            getTableFieldname(what, out table, out tableField);
            Type lType = table.GetType();

            SQLCondition fullSubCond = new SQLCondition();
                        
            if (logicalViewStep.type.series == Type && (lType != typeof(DBSeries) && lType != typeof(DBOnlineSeries)))
            {

                if (lType == typeof(DBSeason))
                {
                    fullSubCond.AddCustom(DBSeason.Q(DBSeason.cSeriesID), DBOnlineSeries.Q(DBOnlineSeries.cID), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBSeason.Q(tableField), condition, condtype, true);

                    conds.AddCustom(" exists( " + DBSeason.stdGetSQL(fullSubCond, false) + " )");
                }
				else if (lType == typeof(DBOnlineEpisode))
                {
                    //fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBOnlineSeries.Q(DBOnlineSeries.cID), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBOnlineEpisode.Q(tableField), condition, condtype, true);
                    //conds.AddCustom(" exists( " + DBEpisode.stdGetSQL(fullSubCond, false) + " )");
                    conds.AddCustom(" online_series.id in ( " + DBEpisode.stdGetSQL(fullSubCond, false, true, DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID)) + " )");
                }
				else if (lType == typeof(DBEpisode)) {
					fullSubCond.AddCustom(DBEpisode.Q(tableField), condition, condtype, true);
					conds.AddCustom(" online_series.id in ( " + DBEpisode.stdGetSQL(fullSubCond, false, true, DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID)) + " )");
				}
            }
            else if (logicalViewStep.type.season == Type && lType != typeof(DBSeason))
            {

                if (lType == typeof(DBOnlineSeries) || lType == typeof(DBSeries))
                {
                    fullSubCond.AddCustom(DBOnlineSeries.Q(DBOnlineSeries.cID), DBSeason.Q(DBSeason.cSeriesID), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBOnlineSeries.Q(tableField), condition, condtype, true);
                    conds.AddCustom(" exists( " + DBSeries.stdGetSQL(fullSubCond, false) + " )");
                }
                else if (lType == typeof(DBOnlineEpisode))
                {
                    //fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBSeason.Q(DBSeason.cSeriesID), SQLConditionType.Equal);
                    //fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex), DBSeason.Q(DBSeason.cIndex), SQLConditionType.Equal);
                    //fullSubCond.AddCustom(DBOnlineEpisode.Q(tableField), condition, condtype);
                    //conds.AddCustom(" exists( " + DBEpisode.stdGetSQL(fullSubCond, false) + " )");
                    // we rely on the join in dbseason for this (much, much faster)
                    conds.AddCustom(DBOnlineEpisode.Q(tableField), condition, condtype, true);
                }
				else if (lType == typeof(DBEpisode)) {
					fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBSeason.Q(DBSeason.cSeriesID), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex), DBSeason.Q(DBSeason.cIndex), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBEpisode.Q(tableField), condition, condtype);
                    conds.AddCustom(" exists( " + DBEpisode.stdGetSQL(fullSubCond, false) + " )");
				}
            }
            else if (logicalViewStep.type.episode == Type && (lType != typeof(DBEpisode) && lType != typeof(DBOnlineEpisode)))
            {

                if (lType == typeof(DBOnlineSeries) || lType == typeof(DBSeries))
                {
                    fullSubCond.AddCustom(DBOnlineSeries.Q(DBOnlineSeries.cID), DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBOnlineSeries.Q(tableField), condition, condtype, true);
                    conds.AddCustom(" exists( " + DBSeries.stdGetSQL(fullSubCond, false) + " )");
                }
                if (lType == typeof(DBSeason))
                {
                    fullSubCond.AddCustom(DBSeason.Q(DBSeason.cSeriesID), DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBSeason.Q(DBSeason.cIndex), DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBSeason.Q(tableField), condition, condtype, true);
                    conds.AddCustom(" exists( " + DBSeason.stdGetSQL(fullSubCond, false) + " )");
                }
            }
            else
            {
                // condition is on current table itself
                conds.Add(table, tableField, condition.Trim(), condtype);
            }

        }

        static void getTableFieldname(string what, out DBTable table, out string fieldname)
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
            string[] allConditions = System.Text.RegularExpressions.Regex.Split(allConditionsAsString, s_condSeperator);
            for (int i = 0; i < allConditions.Length; i++)
            {
                string[] condSplit = System.Text.RegularExpressions.Regex.Split(allConditions[i], ";");
                string condSplit2 = condSplit[2].Replace("\"", "").Replace("'", "");
                if (condSplit2.StartsWith("<today"))
                {
                    // relative date value
                    if (condSplit2 == "<today>")
                        condSplit2 = DateTime.Now.ToString("yyyy-MM-dd");
                    else 
                    {
                        // figure out how long before/after we're going
                        int nDayOffset = System.Convert.ToInt32(condSplit2.Substring("<today".Length).TrimEnd('>'));
                        DateTime offsetDate = DateTime.Now;
                        offsetDate = offsetDate.AddDays(nDayOffset);
                        condSplit2 = offsetDate.ToString("yyyy-MM-dd");
                    }
                }
                addSQLCondition(condSplit[0], condSplit[1], condSplit2);
            }
            conditionsToInherit.Add(allConditionsAsString);
        }

        public static logicalViewStep parseFromDB(string viewStep, bool hasSeriesBeforeIt)
        {
            logicalViewStep thisView = new logicalViewStep();
            thisView.hasSeriesBeforeIt = hasSeriesBeforeIt;
            string[] viewSteps = System.Text.RegularExpressions.Regex.Split(viewStep, s_intSeperator);
            thisView.setType(viewSteps[0]);
            thisView.addConditionsFromString(viewSteps[1]);
            if (viewSteps[2].Length > 0)
            {
                string[] orderFields = System.Text.RegularExpressions.Regex.Split(viewSteps[2], ";");
                thisView.inLineSpecials = orderFields[0] == "<Episode.EpisodeIndex>";
                thisView.inLineSpecialsAsc = orderFields[0] != "desc";
                for (int i = 0; i < orderFields.Length; i += 2)
                {
                    if(thisView.Type != type.group)
                    {
                        DBTable table = null;
                        string tableField = string.Empty;
                        getTableFieldname(orderFields[i], out table, out tableField);
                        tableField = getQTableNameFromUnknownType(table, tableField);

                        // example of how the user can order by a different table
                        // needs to be enabled once definable views are ready
                        /*
                        if (thisView.Type == type.season && table.GetType() != typeof(DBSeason))
                        {
                            Type lType = table.GetType();
                            if (lType == typeof(DBOnlineSeries))
                                tableField = "( select " + tableField + " from " + DBOnlineSeries.cTableName
                                                + " where " + DBOnlineSeries.Q(DBOnlineSeries.cID) + " = " + DBSeason.Q(DBSeason.cSeriesID) + ")";
                        }*/
                        
                        //if (thisView.Type == type.episode && ( table.GetType() == typeof(DBEpisode) || table.GetType() == typeof(DBOnlineEpisode)))
                        //{
                        //    // for perf reason a subquery is build, otherwise custom orders and the nessesary join really slow down sqllite!
                        //    SQLCondition subQueryConditions = thisView.conds.Copy(); // have to have all conds too
                        //    subQueryConditions.AddOrderItem(tableField, (orderFields[i + 1] == "asc" ? SQLCondition.orderType.Ascending : SQLCondition.orderType.Descending));
                        //    if (viewSteps[3].Length > 0) // set the limit too
                        //    {
                        //        try
                        //        {
                        //            subQueryConditions.SetLimit(System.Convert.ToInt32(viewSteps[3]));
                        //        }
                        //        catch (Exception)
                        //        {
                        //        }
                        //    }
                        //    thisView.conds.AddSubQuery("compositeid", table, subQueryConditions, table.m_tableName + "." + DBEpisode.cCompositeID, SQLConditionType.In);
                            
                        //}

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
}
