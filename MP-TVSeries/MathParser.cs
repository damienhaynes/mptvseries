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
    // "Eval(Round(PrettyBytes(3400+53555)*100)/100-> Bytes| KiloBytes| MegaBytes| GigaBytes)"
    sealed class mathParser
    {
        const string floatNumberRegex = @"[+|-]?[0-9]+\.?[0-9]*";
        const string evalFormat = @"Eval(";
        const string optionFormat = "->";
        //const string ifForm = "if(";
        //const string thenForm = "then(";
        //const string elseForm = "else(";

        static NumberFormatInfo provider = new NumberFormatInfo();

        static List<mathFunction> functions = new List<mathFunction>();
        static List<mathConstant> constants = new List<mathConstant>();
        static List<Regex> atomics = new List<Regex>();

        static Regex modPow = new Regex(floatNumberRegex + @"[%|^]" + floatNumberRegex, RegexOptions.Compiled);
        static Regex multDiv = new Regex(floatNumberRegex + @"[\*|/]" + floatNumberRegex, RegexOptions.Compiled);
        static Regex addSub = new Regex(floatNumberRegex + @"[-|+]" + floatNumberRegex, RegexOptions.Compiled);

        static Regex singleOp = new Regex(@"(?<no1>" + floatNumberRegex + @")(?<type>[-|+|*|/|%|^])(?<no2>" + floatNumberRegex + ")", RegexOptions.Compiled);

        static int prevResult = -1;

        static public List<mathFunction> SupportedFunctions
        {
            get { return functions; }
        }

        static public List<mathConstant> SupportedConstants
        {
            get { return constants; }
        }

        static mathParser()
        {
            provider.NumberDecimalSeparator = ".";

            addFunction("Round", delegate(double number) { return (int)Math.Round(number); });
            addFunction("Sqrt", delegate(double number) { return Math.Sqrt(number); });
            addFunction("Floor", delegate(double number) { return Math.Floor(number); });
            addFunction("Ceil", delegate(double number) { return Math.Ceiling(number); });
            addFunction("Abs", delegate(double number) { return Math.Abs(number); });
            addFunction("Log10", delegate(double number) { return Math.Log10(number); });

            addFunction("PrettyBytes1024", delegate(double number, out int pow)
            {
                double result = number;
                pow = 0;
                while (result > 1024)
                {
                    result /= 1024;
                    pow++;
                }
                return result;
            });

            addFunction("PrettyBytes1000", delegate(double number, out int pow)
            {
                double result = number;
                pow = 0;
                while (result > 1000)
                {
                    result /= 1000;
                    pow++;
                }
                return result;
            });

            addFunction(string.Empty, delegate(double number) { return number; }); // simple parenthesis, needs to be last in list

            addConstant("PI", delegate() { return Math.PI; });
            addConstant("Euler", delegate() { return Math.E; });

            atomics.Add(modPow); // % and ^ first
            atomics.Add(multDiv); // then * /
            atomics.Add(addSub); // and finally + -
        }

        static void addFunction(string format, mathFunction.functionDel function)
        {
            addFunction(format, function, null);
        }

        static void addFunction(string format, mathFunction.functionDelWithResult function)
        {
            addFunction(format, null, function);
        }

        static void addFunction(string format, mathFunction.functionDel function, mathFunction.functionDelWithResult functionWRes)
        {
            mathFunction m = null;
            m = function == null ? new mathFunction(format, functionWRes) : new mathFunction(format, function);
            functions.Add(m);
            subLog(string.Format("Added MathFunction: {0}double value)", m.form));
        }

        public static void addConstant(string format, mathConstant.functionDel function)
        {
            mathConstant m = new mathConstant(format, function);
            constants.Add(m);
            subLog(string.Format("Added MathConstant: {0} -> {1}", m.form, m.Value));
        }

        /// <summary>
        /// Tries to parse a given mathematical expression within a string "Eval(expression)" and evaluates it
        /// </summary>
        /// <param name="expression">The expression to parse, eg: "Eval(5) m" -> 5 m</param>
        /// <returns>returns the original expression if it cannot be parsed, otherwise returns the result of the expression as a string</returns>
        public static string TryParse(string expression)
        {
            string replace, with, stringResult;
            while (parenthesisFinder(expression, evalFormat, out replace, out with))
            {
                double? res = Parse(with, out stringResult);
                if (res != null) expression = expression.Replace(replace, res.Value.ToString());
                else if (stringResult != null) expression = expression.Replace(replace, stringResult);
                else expression = expression.Replace(replace, with);
            }
            return expression;
        }

        /// <summary>
        /// Parses a given mathematical expression and evaluates it
        /// </summary>
        /// <param name="expression">The expression to parse</param>
        /// <returns>returns the result of the mathematical expression, or null if it cannot be evaluated</returns>
        public static double? Parse(string expression, out string stringResult)
        {
            subLog("Mathparser: Trying " + expression);
            double? result = null;
            stringResult = null;
            try
            {
                StringBuilder builder = new StringBuilder(expression);
                //StringBuilder builder = new StringBuilder(expression.Replace(" ", ""));
                foreach (mathConstant c in constants)
                    builder.Replace(c.form, c.Value.ToString(provider));
                result = breakdDown(builder.ToString(), out stringResult);
            }
            catch (Exception e)
            {
                subLog("Mathparser: Critical Error " + e.Message);
            }
            if (null != result)
                MPTVSeriesLog.Write(String.Format("Mathparser: Total Result: {0} = {1}", expression, result.ToString()));
            else if (stringResult != null)
                MPTVSeriesLog.Write(String.Format("Mathparser: Total Result as String: {0} = {1}", expression, result.ToString()));
            return result;
        }

        static bool parenthesisFinder(string expression, string form, out string resultWith, out string resultWithout)
        {
            int index = -1;
            index = expression.IndexOf(form);
            resultWith = null;
            resultWithout = null;
            if (index != -1)
            {
                // ok match, lets fast forward to the next ), offsetting +1 for every additional ( we find
                int offset = -1;
                int endPos = index;
                for (int j = index; j < expression.Length; j++)
                {
                    char c = expression[j];
                    if (offset == 0 && c == ')') // yahoo
                    {
                        endPos = j;
                        break;
                    }
                    else if (c == '(') offset++;
                    else if (c == ')') offset--;
                }
                if (endPos != index)
                {
                    // we had a succesful match
                    int len = index + form.Length;
                    resultWith = expression.Substring(index, endPos - index + 1);
                    resultWithout = expression.Substring(len, endPos - len);
                    return true;
                }
            }
            return false;
        }

        static double? breakdDown(string expression, out string stringResult)
        {
            stringResult = null;
            // find parenthesis and apply the desired delegate on the result described in functions
            double result;
            for (int i = 0; i < functions.Count; i++)
            {
                string toReplace;
                string replaceWith;
                if (parenthesisFinder(expression, functions[i].form, out toReplace, out replaceWith))
                {
                    subLog("Processing now: " + replaceWith);

                    double? subresult = breakdDown(replaceWith, out stringResult);

                    subLog("Subresult: " + (subresult == null ? " ERROR" : ((double)(subresult)).ToString(provider)));

                    if (subresult == null) return null; // can't process it
                    double funcRes = functions[i].Perform((double)subresult);
                    if (functions[i].setsResult) prevResult = functions[i].Result;

                    subLog("Function: " + functions[i].form + subresult + ") = " + funcRes.ToString(provider));
                    if (Double.IsNaN(funcRes)) return null; // function not possible
                    expression = expression.Replace(toReplace, funcRes.ToString(provider));
                    i--; //because each function may exist more than once per "level" we have to try again
                }

            }

            // now process atomic operations
            foreach (Regex reg in atomics)
                if (!processAtomics(ref expression, reg)) return null;

            if (prevResult > -1 && expression.Contains(optionFormat)) stringResult = stringOption(expression);

            if (!double.TryParse(expression, NumberStyles.Float, provider, out result)) return null;

            // check if we have any options we have to use with the prevResult


            return result;
        }

        static bool processAtomics(ref string expression, Regex operations)
        {
            Match m = null;
            while ((m = operations.Match(expression)).Value.Length > 0)
            {
                decimal? atomRes = atomicOperation(m.Value);
                subLog("Atomic operation: " + m.Value + " = " + (atomRes == null ? "unable" : atomRes.ToString()));
                if (atomRes == null) return false; // unsovlable
                expression = expression.Replace(m.Value,
                                               (atomRes.Value >= 0 ? provider.PositiveSign : provider.NegativeSign)
                                               + atomRes.Value.ToString(provider));
            }
            return true;
        }

        enum atomicOperationType
        {
            addition,
            substraction,
            multiplication,
            division,
            mod,
            pow,
            unknown
        }

        static decimal? atomicOperation(string expression)
        {
            decimal no1, no2;
            Match m = singleOp.Match(expression);

            if (m == null) return null;
            if (!decimal.TryParse(m.Groups["no1"].Value, NumberStyles.Number, provider, out no1)) return null;
            if (!decimal.TryParse(m.Groups["no2"].Value, NumberStyles.Number, provider, out no2)) return null;

            switch (MatchAtomOperation(m.Groups["type"].Value))
            {
                case atomicOperationType.addition:
                    return no1 + no2;
                case atomicOperationType.substraction:
                    return no1 - no2;
                case atomicOperationType.multiplication:
                    return no1 * no2;
                case atomicOperationType.division:
                    return no1 / no2;
                case atomicOperationType.mod:
                    return no1 % no2;
                case atomicOperationType.pow:
                    //return Math.Pow((double)no1, no2);
                default: MPTVSeriesLog.Write("MathParser: Unknown operand: " + m.Groups["type"].Value);
                    break;
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
            else if (expression.Contains("%")) a = atomicOperationType.mod;
            else if (expression.Contains("^")) a = atomicOperationType.pow;
            return a;
        }

        public static void subLog(string entry)
        {
            MPTVSeriesLog.Write(entry, MPTVSeriesLog.LogLevel.Debug);
        }

        static string stringOption(string input)
        {
            string[] parameters = input.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
            string[] options = parameters[1].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            subLog("MathParser: StrinOptions -> prev Result was set to " + prevResult.ToString());
            if (options.Length >= prevResult) return double.Parse(parameters[0], provider).ToString() + options[prevResult];
            return string.Empty;

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
        public delegate double functionDelWithResult(double number, out int Result);

        public string form;
        public bool setsResult = false;
        public int Result = -1;
        functionDel _perform;
        functionDelWithResult _performWRes;

        public double Perform(double number)
        {
            if (setsResult) return _performWRes(number, out Result);
            else return _perform(number);
        }

        public mathFunction(string form, functionDel function)
        {
            _perform = function;
            init(form);
        }

        public mathFunction(string form, functionDelWithResult function)
        {
            _performWRes = function;
            setsResult = true;
            init(form);
        }

        void init(string form)
        {
            this.form = form + "(";
        }
    }
}
