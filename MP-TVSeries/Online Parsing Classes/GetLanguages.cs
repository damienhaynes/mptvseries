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

using System.Collections.Generic;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    public class Language
    {
        public string Name = string.Empty;
        public int Id = default(int);
        public string Abbreviation = string.Empty;
        public string EnglishName = string.Empty;

        public override string ToString()
        {
            return $"{EnglishName} ({Abbreviation}) - {Name}";
        }
    }

    public class GetLanguages
    {
        public List<Language> languages = new List<Language>();

        public GetLanguages()
        {
            XmlNode rootNode = Online_Parsing_Classes.OnlineAPI.GetLanguages();
            if (rootNode != null)
            {
                Language lang = null;
                foreach (XmlNode itemNode in rootNode.ChildNodes)
                {
                    lang = new Language();
                    foreach (XmlNode node in itemNode)
                    {
                        if (node.Name == "id") int.TryParse(node.InnerText, out lang.Id);
                        if (node.Name == "name") lang.Name = node.InnerText;
                        if (node.Name == "abbreviation")
                        {
                            lang.Abbreviation = node.InnerText;
                            try
                            {
                                lang.EnglishName = new System.Globalization.CultureInfo( lang.Abbreviation ).EnglishName;
                            }
                            catch
                            {
                                MPTVSeriesLog.Write( $"Unable to get English name for language code '{lang.Abbreviation}'", MPTVSeriesLog.LogLevel.Debug );
                            }
                        }
                    }
                    if (lang.Id != default(int) && lang.EnglishName.Length > 0 && !lang.EnglishName.StartsWith("Unknown"))
                        languages.Add(lang);
                }
            }
        }
    }
}
