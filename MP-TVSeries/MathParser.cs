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
using System.Text.RegularExpressions;
using System.Globalization;

namespace WindowPlugins.GUITVSeries.MathParser
{

    sealed class mathParser
    {
        const string floatNumberRegex = @"[-]?[0-9]*\.?[0-9]*";
        static NumberFormatInfo provider = new NumberFormatInfo();

        static List<mathFunction> functions = new List<mathFunction>();
        static List<mathConstant> constants = new List<mathConstant>();

        static Regex multDiv = new Regex(floatNumberRegex + @"\n*[\*|/]\n*" + floatNumberRegex, RegexOptions.Compiled);
        static Regex addSub = new Regex(floatNumberRegex + @"\n*[-|+]\n*" + floatNumberRegex, RegexOptions.Compiled);
        static Regex number = new Regex(floatNumberRegex, RegexOptions.Compiled);


        static public List SupportedFunctions
        {
            get { return functions; }
        }

        static public List SupportedConstants
        {
            get { return constants; }
        }

        static mathParser()
        {
            provider.NumberDecimalSeparator = ".";

            addFunction("Round", delegate(double number) { return Math.Round(number); });
            addFunction("Sqrt", delegate(double number) { return Math.Sqrt(number); });
            addFunction("Floor", delegate(double number) { return Math.Floor(number); });
            addFunction("Ceil", delegate(double number) { return Math.Ceiling(number); });
            addFunction("Abs", delegate(double number) { return Math.Abs(number); });
            addFunction("Log10", delegate(double number) { return Math.Log10(number); });
            addFunction(string.Empty, delegate(double number) { return number; }); // simple parenthesis, needs to be last in list

            addConstant("PI", delegate() { return Math.PI; });
            addConstant("Euler", delegate() { return Math.E; });
        }

        public static void addFunction(string format, mathFunction.functionDel function)
        {
            mathFunction m = new mathFunction(format, function);
            functions.Add(m);
            MPTVSeriesLog.Write(string.Format("Added MathFunction: {0}double value)", m.form), MPTVSeriesLog.LogLevel.Debug);
        }

        public static void addConstant(string format, mathConstant.functionDel function)
        {
            mathConstant m = new mathConstant(format, function);
            constants.Add(m);
            MPTVSeriesLog.Write(string.Format("Added MathConstant: {0} -> {1}", m.form, m.Value), MPTVSeriesLog.LogLevel.Debug);
        }

        /// <summary>
        /// Tries to parse a given mathematical expression and evaluates it
        /// </summary>
        /// <param name="expression">The expression to parse</param>
        /// <returns>returns the original expression if it cannot be parsed, otherwise returns the result of the expression as a string</returns>
        public static string TryParse(string expression)
        {
            double? res = Parse(expression);
            return res == null ? expression : res.Value.ToString();
        }

        /// <summary>
        /// Parses a given mathematical expression and evaluates it
        /// </summary>
        /// <param name="expression">The expression to parse</param>
        /// <returns>returns the result of the mathematical expression, or null if it cannot be evaluated</returns>
        public static double? Parse(string expression)
        {
            MPTVSeriesLog.Write("Mathparser: Trying " + expression, MPTVSeriesLog.LogLevel.Normal);
            try
            {
                StringBuilder builder = new StringBuilder(expression.Replace(" ", ""));
                foreach (mathConstant c in constants)
                    builder.Replace(c.form, c.Value.ToString(provider));
                double? result = breakdDown(builder.ToString());
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("Mathparser: Critical Error " + e.Message, MPTVSeriesLog.LogLevel.Normal);
            }
            if(null != result)
                MPTVSeriesLog.Write("Mathparser: Total Result: " +  result.ToString(), MPTVSeriesLog.LogLevel.Normal);
            return result;
        }

        static double? breakdDown(string expression)
        {
            // find parenthesis and apply the desired delegate on the result described in functions
            double result;
            for (int i = 0; i < functions.Count; i++)
            {
                int index = -1;
                index = expression.IndexOf(functions[i].form);
                if (index != -1)
                {
                    // ok match, lets fast forward to the next ), offsetting +1 for every additional ( we find
                    int offset = -1;
                    int endPos = index;
                    for (int j = index; j < expression.Length; j++)
                    {
                        if (offset == 0 && expression[j] == ')') // yahoo
                        {
                            endPos = j;
                            break;
                        }
                        else if (expression[j] == '(') offset++;
                        else if (expression[j] == ')') offset--;
                    }
                    if (endPos != index)
                    {
                        // we had a succesful match
                        int len = index + functions[i].form.Length;
                        string toReplace = expression.Substring(index, endPos - index + 1);
                        string replaceWith = expression.Substring(len, endPos - len);

                        MPTVSeriesLog.Write("Processing now: " + replaceWith, MPTVSeriesLog.LogLevel.Debug);

                        double? subresult = breakdDown(replaceWith);

                        MPTVSeriesLog.Write("Subresult: " + (subresult == null ? " ERROR" : ((double)(subresult)).ToString(provider)), MPTVSeriesLog.LogLevel.Debug);

                        if (subresult == null) return null; // can't process it
                        double funcRes = functions[i].perform((double)subresult);

                        MPTVSeriesLog.Write("Function: " + functions[i].form + subresult + ") = " + funcRes.ToString(provider), MPTVSeriesLog.LogLevel.Debug);

                        expression = expression.Replace(toReplace, funcRes.ToString(provider));
                        i = 0;
                    }
                }
            }
            // mult/div first
            if (!processAtomics(ref expression, multDiv)) return null;
            if (!processAtomics(ref expression, addSub)) return null;

            if (!double.TryParse(expression, NumberStyles.Float, provider, out result)) return null;
            return result;
        }

        static bool processAtomics(ref string expression, Regex operations)
        {
            Match m = null;
            while ((m = operations.Match(expression)).Value.Length > 0)
            {
                double? atomRes = atomicOperation(m.Value);
                MPTVSeriesLog.Write("Atomic operation: " + m.Value + " = " + (atomRes == null ? "unable" : atomRes.ToString()), MPTVSeriesLog.LogLevel.Debug);
                if (atomRes == null) return false; // unsovlable
                expression = expression.Replace(m.Value, ((double)atomRes).ToString(provider));
            }
            return true;
        }

        enum atomicOperationType
        {
            addition,
            substraction,
            multiplication,
            division,
            unknown
        }
        static double? atomicOperation(string expression)
        {
            double no1, no2;
            MatchCollection m = number.Matches(expression);
            if (m.Count != 4 && m.Count != 3) return null;
            no1 = double.Parse(m[0].Value, provider);
            no2 = double.Parse(m[2].Value, provider);

            switch (MatchAtomOperation(expression))
            {
                case atomicOperationType.addition:
                    return no1 + no2;
                case atomicOperationType.substraction:
                    return no1 - no2;
                case atomicOperationType.multiplication:
                    return no1 * no2;
                case atomicOperationType.division:
                    return no1 / no2;
            }

            return null;
        }

        static atomicOperationType MatchAtomOperation(string expression)
        {
            atomicOperationType a = atomicOperationType.unknown;
            if (expression.Contains("*")) a = atomicOperationType.multiplication;
            else if (expression.Contains("/")) a = atomicOperationType.division;
            else if (expression.Contains("+")) a = atomicOperationType.addition;
            else if (expression.Contains("-")) a = atomicOperationType.substraction;
            return a;
        }

    }

    class mathConstant
    {
        public delegate double functionDel();
        public string form;
        functionDel perform;

        public double Value
        {
            get { return perform(); }
        }

        public mathConstant(string form, functionDel function)
        {
            this.form = form;
            perform = function;
        }
    }

    class mathFunction
    {
        public delegate double functionDel(double number);

        public string form;
        public functionDel perform;

        public mathFunction(string form, functionDel function)
        {
            this.form = form + "(";
            perform = function;
        }
    }
}
