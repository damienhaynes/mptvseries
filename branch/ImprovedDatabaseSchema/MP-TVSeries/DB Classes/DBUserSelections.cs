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
using SQLite.NET;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.DataClass
{
    class DBUserSelection : DBTable
    {
        public enum SelectionLevel
        {
            series,
            season,
            episode
        }

        public enum SelectionType
        {
            emule,
            subtitles
        }

        public const string cTableName = "user_selection";

		#region Local DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
		public const string cIndex = "ID";
        public const string cSelectionLevel = "selectionLevel";
        public const string cSelectionType = "selectionType";
        public const string cInternalKey = "internalKey";
        public const string cUserKey = "userKey";
        public const string cTags = "tags";
        public const string cContextType = "contextType";
        public const string cEnabled = "enabled";

		// all mandatory fields. Place the primary key first - it's just good manners
		public static readonly DBFieldDefList TableFields = new DBFieldDefList {
            {cIndex,			new DBFieldDef{FieldName = cIndex,			Type = DBFieldType.Int,			Primary = true}},
            {cSelectionLevel,	new DBFieldDef{FieldName = cSelectionLevel, Type = DBFieldType.String}},
            {cSelectionType,	new DBFieldDef{FieldName = cSelectionType,	Type = DBFieldType.String}},
            {cInternalKey,		new DBFieldDef{FieldName = cInternalKey,	Type = DBFieldType.String}},
            {cUserKey,			new DBFieldDef{FieldName = cUserKey,		Type = DBFieldType.String}},
            {cTags,				new DBFieldDef{FieldName = cTags,			Type = DBFieldType.String}},
            {cContextType,		new DBFieldDef{FieldName = cContextType,	Type = DBFieldType.String}},
            {cEnabled,			new DBFieldDef{FieldName = cEnabled,		Type = DBFieldType.Int}}
		};
		#endregion

		public const SelectionLevel SelectionLevelEpisode = SelectionLevel.episode;
        public const SelectionLevel SelectionLevelSeason = SelectionLevel.season;
        public const SelectionLevel SelectionLevelSeries = SelectionLevel.series;
        public const SelectionType SelectionTypeEmule = SelectionType.emule;
        public const SelectionType SelectionTypeSubtiles = SelectionType.subtitles;

        #region Constructors
		//static DBUserSelection()
		//{
		//}

		internal static void MaintainDatabaseTable(Version lastVersion)
		{
			try {
				//test for table existance
				if (!DatabaseHelper.TableExists(cTableName)) {
					DatabaseHelper.CreateTable(cTableName, TableFields.Values);
				}
			} catch (Exception) {
				MPTVSeriesLog.Write("Unable to Correctly Maintain the " + cTableName + " Table");
			}
		}
		
		public DBUserSelection()
            : base(cTableName)
        {
        }

        public DBUserSelection(long ID)
            : base(cTableName)
        {
            ReadPrimary(ID.ToString());
        }

        public DBUserSelection(SelectionLevel level, SelectionType type, string internalkey)
            : base(cTableName)
        {
            if (!this.ReadAKInternal(new DBValue(level.ToString()), new DBValue(type.ToString()), new DBValue(internalkey)))
            {
                this[cSelectionLevel] = level.ToString();
                this[cSelectionType] = type.ToString();
                this[cInternalKey] = internalkey;
            }
        }
        
        #endregion Constructors

        #region Private Methods

        protected override void InitColumns()
        {
        	AddColumns(TableFields.Values);
        }

        #endregion Private Methods 

        #region Public Methods

        public override bool Commit()
        {
            if (this[cIndex] == 0)
            {
                string sqlMax = "select max("+ cIndex + ") from " + cTableName;
                SQLiteResultSet set = DBTVSeries.Execute(sqlMax);
                if (set.Rows.Count > 0)
                {
                    string maxID = set.GetRow(0).fields[0];
                    try
                    {
                        this[cIndex] = Convert.ToInt32(maxID) + 1;
                    }
                    catch (Exception)
                    {
                        this[cIndex] = 1;
                    }
                }
            }
            return base.Commit();
        }

        public bool matchTags(List<string> userTags)
        {
            if ((userTags == null) || (userTags.Count == 0))
            {
                return false;
            }
            List<string> tags = this.Tags;
            foreach (string tag in userTags)
            {
                if (!tags.Contains(tag.ToLower()))
                {
                    return false;
                }
            }
            return (tags.Count == userTags.Count);
        }

        public bool ReadAKInternal(DBValue level, DBValue type, DBValue internalkey)
        {
            try
            {
                SQLCondition condition = new SQLCondition();
                condition.Add(new DBUserSelection(), cSelectionLevel, new DBValue(level.ToString()), SQLConditionType.Equal);
                condition.Add(new DBUserSelection(), cSelectionType, new DBValue(type.ToString()), SQLConditionType.Equal);
                condition.Add(new DBUserSelection(), cInternalKey, new DBValue(internalkey.ToString()), SQLConditionType.Equal);

                SQLiteResultSet records = DBTVSeries.Execute("select * from " + base.TableName + condition);
                return base.Read(ref records, 0);
            }
            catch (Exception exception)
            {
                MPTVSeriesLog.Write("An Error Occurred (" + exception.Message + ").");
            }
            return false;
        }

        #endregion Public Methods

        #region Properties

        // Properties
        public bool Enabled
        {
            get
            {
                return (this[cEnabled] == 1 && this[cUserKey] != "");
            }
            set
            {
                this[cEnabled] = (value ? 1 : 0);
            }
        }

        public List<string> Tags
        {
            get
            {
                List<string> tagList = new List<string>();
                if (this[cTags] != null && this[cTags] != "")
                {
                    string[] tagArray = this[cTags].ToString().Split(new char[] { ',' });
                    for (int i = 0; i < tagArray.Length; i++)
                    {
                        tagList.Add(tagArray[i]);
                    }
                }
                return tagList;
            }
            set
            {
                StringBuilder tagCommaSepList = new StringBuilder();
                foreach (string tag in value)
                {
                    tagCommaSepList.Append(tag).Append(",");
                }
                this[cTags] = tagCommaSepList.ToString().Substring(0, tagCommaSepList.Length - 1);
            }
        }

        #endregion Properties

        #region Static Methods

        public static void Clear(DBEpisode episode)
        {
            SQLCondition cond = new SQLCondition();
            cond.Add(new DBUserSelection(), cInternalKey, episode[DBEpisode.cCompositeID], SQLConditionType.Equal);
            cond.Add(new DBUserSelection(), cSelectionType, new DBValue(SelectionLevelEpisode.ToString()), SQLConditionType.Equal);
            Clear(cond);
        }

        public static void Clear(DBSeason season)
        {
            SQLCondition cond = new SQLCondition();
            cond.Add(new DBUserSelection(), cInternalKey, season[DBSeason.cID], SQLConditionType.Equal);
            cond.Add(new DBUserSelection(), cSelectionLevel, new DBValue(SelectionLevelSeason.ToString()), SQLConditionType.Equal);
            Clear(cond);
        }

        public static void Clear(DBSeries series)
        {
            SQLCondition cond = new SQLCondition();
            cond.Add(new DBUserSelection(), cInternalKey, series[DBSeries.cID], SQLConditionType.Equal);
            cond.Add(new DBUserSelection(), cSelectionLevel, new DBValue(SelectionLevelSeries.ToString()), SQLConditionType.Equal);
            Clear(cond);
            cond = new SQLCondition();
            cond.Add(new DBUserSelection(), cSelectionLevel, new DBValue(SelectionLevelSeries.ToString()), SQLConditionType.NotEqual);
            cond.Add(new DBUserSelection(), cInternalKey, series[DBSeries.cID] + "_", SQLConditionType.Like);
            Clear(cond);
        }

        public static void Clear(SQLCondition cond)
        {
            DBTVSeries.Execute("delete from " + cTableName + cond);
        }

        public static void ClearAll()
        {
            SQLCondition cond = new SQLCondition();
            Clear(cond);
        }

        public static DBUserSelection[] Get(SQLCondition conditions)
        {
            try
            {
                SQLWhat what = new SQLWhat(new DBUserSelection());
                SQLiteResultSet records = DBTVSeries.Execute("select " + what + " where " + conditions);
                if (records.Rows.Count > 0)
                {
                    DBUserSelection[] selectionArray = new DBUserSelection[records.Rows.Count];
                    for (int i = 0; i < records.Rows.Count; i++)
                    {
                        selectionArray[i] = new DBUserSelection();
                        selectionArray[i].Read(ref records, i);
                    }
                    return selectionArray;
                }
            }
            catch (Exception exception)
            {
                MPTVSeriesLog.Write("Error in DBUserSelection.Get (" + exception.Message + ").");
            }
            return null;
        }

        public static DBUserSelection[] Get(SelectionLevel level, SelectionType type, string internalKey)
        {
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBUserSelection(), cSelectionLevel, new DBValue(level.ToString()), SQLConditionType.Equal);
            conditions.Add(new DBUserSelection(), cSelectionType, new DBValue(type.ToString()), SQLConditionType.Equal);
            conditions.Add(new DBUserSelection(), cInternalKey, new DBValue(internalKey), SQLConditionType.Equal);
            return Get(conditions);
        }

        public static DBUserSelection[] GetAll()
        {
            SQLCondition conditions = new SQLCondition();
            return Get(conditions);
        }

  
        public static string Q(string sField)
        {
            return (cTableName + "." + sField);
        }

        #endregion Static Methods
    }
}