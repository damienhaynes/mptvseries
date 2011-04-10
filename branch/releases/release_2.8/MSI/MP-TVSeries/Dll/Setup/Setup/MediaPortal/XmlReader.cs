using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace MediaPortal
{
    public class XmlReader
    {
        public XmlDocument Document = new XmlDocument();

        public string GetEntry(string section, string name)
        {
            if (Document == null) return string.Empty;
         
            XmlNode node = null;
            node = Document.DocumentElement.SelectSingleNode(string.Format("/profile//section[@name='{0}']//entry[@name='{1}']", section, name));
            if (node == null) return string.Empty;
           
            return node.InnerText;
        }

        public bool Load(string file)
        {
            if (!File.Exists(file)) return false;

            try
            {
                Document.Load(file);
            }
            catch (Exception)
            {
                Document = null;
                return false;
            }
            return true;
        }
    }
}
