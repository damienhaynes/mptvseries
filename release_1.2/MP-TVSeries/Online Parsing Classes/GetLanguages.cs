using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    public class Language
    {
        public string language = string.Empty;
        public int id = default(int);
    }
    public class GetLanguages
    {
        public List<Language> languages = new List<Language>();

        public GetLanguages()
        {
            XmlNodeList nodeList = ZsoriParser.GetLanguages();
            if (nodeList != null)
            {
                Language lang = null;
                foreach (XmlNode itemNode in nodeList)
                {
                    lang = new Language();
                    foreach (XmlNode node in itemNode)
                    {
                        if (node.Name == "id") int.TryParse(node.InnerText, out lang.id);
                        if (node.Name == "language") lang.language = node.InnerText;
                    }
                    if (lang.id != default(int) && lang.language.Length > 0) languages.Add(lang);
                }
            }
        }
    }
}
