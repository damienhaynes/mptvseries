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
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;
using WindowPlugins.GUITVSeries.TmdbAPI;

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
        public List<Language> Languages = new List<Language>();

        public GetLanguages()
        {
            var lLanguages = TmdbAPI.TmdbAPI.GetLanguages();
            var lLanguage = new Language();

            foreach (var language in lLanguages)
            {
                Languages.Add( new Language
                { 
                    Name = language.Name,
                    Abbreviation = language.Code,
                    EnglishName = language.EnglishName
                });
            }
        }
    }
}
