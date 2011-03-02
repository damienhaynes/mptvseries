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
    public class Helper
    {
        public static List<T> getFilteredList<T, P>(List<T> inputList, string PropertyName, P ValueOfProperty)
        {
            List<T> resultList = new List<T>();
            foreach (T item in inputList)
            {
                if (ValueOfProperty.Equals(((P)item.GetType().InvokeMember(PropertyName, System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.GetProperty, null, item, null))))
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

        public static List<string> getFieldNameListFromList<T>(string FieldNameToGet, List<T> elements) where T : DBTable
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
            addOperationOnItemBeforeCompare<t> addOp = delegate(t item) { return item; };
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
            List<DBSeason> tmpSeasons = DBSeason.Get(seriesID);
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
            if (path2.Length > 0 && (path2[0] == '\\' || path2[0] == '/')) path2 = path2.Substring(1);
            return System.IO.Path.Combine(path1, path2);
        }

        public static List<T> inverseList<T>(List<T> input)
        {
            List<T> result = new List<T>(input.Count);
            for (int i = 0; i < input.Count; i++)
                result.Add(input[i]);
            return result;
        }

        public class String
        {
            /// <summary>
            /// Fix for the buggy MS implementation
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public static bool IsNullOrEmpty(string value)
            {
                // great, won't be fixed until Orcas
                // http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=113102
                if (value != null) return value.Length == 0;
                return true;
            }
        }
    }

    class perfana
    {
        static DateTime starttime = new DateTime();
        public static void start()
        {
            starttime = DateTime.Now;
        }

        public static string measorFromStart()
        {
            return getTimeFromStart().ToString();
        }

        public static int getTimeFromStart()
        {
            if (starttime.Equals(default(DateTime))) return 0;
            TimeSpan t = DateTime.Now - starttime;
            return (int)t.TotalMilliseconds;
        }

        public static void writeSimple(string entry)
        {
            // get the caller method
            //System.Diagnostics.StackFrame fr = new System.Diagnostics.StackFrame(1, true);
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(fr);
            //string caller = String.Format("{0} {1}", fr.GetMethod().Name,
            //                                       st.ToString());
            DateTime now = DateTime.Now;
            MPTVSeriesLog.Write(" - " + now.Second.ToString() + ":" + now.Millisecond.ToString() + " .... " + entry);
        }

        static Dictionary<string, codeAreaMeasurement> codeAreas = null;
        public static void clearAllCodeMeasurements()
        {
            codeAreas = null;
        }
        public static void measureCodeArea(string name, bool start)
        {
            if (null == codeAreas) codeAreas = new Dictionary<string, codeAreaMeasurement>();
            if (!codeAreas.ContainsKey(name))
            {
                codeAreas.Add(name, new codeAreaMeasurement());
            }
            if (start)
                codeAreas[name].lastStart = DateTime.Now;
            else
            {
                TimeSpan t = DateTime.Now - codeAreas[name].lastStart;
                codeAreas[name].totalTime += t.TotalMilliseconds;
            }
            
        }

        public static string getCodeAreaMeasurement(string name)
        {
            if (codeAreas == null) return string.Empty;
            if (codeAreas.ContainsKey(name))
            {
                return codeAreas[name].totalTime.ToString();
            }
            else return string.Empty;
        }

        public static List<string> getAllAreaResults()
        {
            List<string> results = new List<string>();
            if (codeAreas == null) return results;
            foreach (KeyValuePair<string, codeAreaMeasurement> res in codeAreas)
                results.Add("-> " + res.Key + ": " + res.Value.totalTime.ToString());
            return results;
        }

        public static void writeAllAreaResults()
        {
            foreach (string s in getAllAreaResults())
                MPTVSeriesLog.Write(s);
        }

        public class codeAreaMeasurement
        {
            public double totalTime = default(double);
            public DateTime lastStart = default(DateTime);
        }
    }
}

