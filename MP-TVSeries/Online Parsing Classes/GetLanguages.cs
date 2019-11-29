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
        public int id = default(int);
        public string abbreviation = string.Empty;
        public string englishName = string.Empty;

        public override string ToString()
        {
            return $"{englishName} ({abbreviation}) - {Name}";
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
                        if (node.Name == "id") int.TryParse(node.InnerText, out lang.id);
                        if (node.Name == "name") lang.Name = node.InnerText;
                        if ( node.Name == "abbreviation" )
                        {
                            lang.abbreviation = node.InnerText;
                            lang.englishName = new System.Globalization.CultureInfo( lang.abbreviation ).EnglishName;
                        }
                    }
                    if (lang.id != default(int) && lang.Name.Length > 0 && !lang.englishName.StartsWith("Unknown"))
                        languages.Add(lang);
                }
            }
        }
    }
}
