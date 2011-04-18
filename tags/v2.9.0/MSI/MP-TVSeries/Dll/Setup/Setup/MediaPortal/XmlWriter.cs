using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace MediaPortal
{
    public class XmlWriter
    {
        public XmlDocument Document = new XmlDocument();

        public void CreateXmlConfigFile(string file)
        {
            try
            {
                XmlTextWriter textWriter = new XmlTextWriter(file, Encoding.UTF8);

                textWriter.WriteStartDocument();
                textWriter.WriteStartElement("profile");
                textWriter.WriteEndElement();
                textWriter.WriteEndDocument();
                
                textWriter.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public bool Load(string file)
        {
            if (!File.Exists(file)) return false;

            try
            {
                Document.Load(file);
            }
            catch (XmlException)
            {
                Document = null;
                return false;
            }
            return true;
        }

        public bool Save(string file)
        {
            if (!File.Exists(file)) return false;

            try
            {
                Document.Save(file);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SetEntry(string section, string name, string value)
        {
            if (Document == null) return false;

            try
            {
                XmlNode node = null;
                node = Document.SelectSingleNode(string.Format("/profile//section[@name='{0}']", section));
                if (node == null)
                {
                    // select root node
                    node = Document.SelectSingleNode("/profile");

                    // create new section node
                    XmlNode newNode = Document.CreateElement("section");
                    XmlAttribute newAttribute = Document.CreateAttribute("name");
                    newAttribute.Value = section.Trim('\0');
                    newNode.Attributes.Append(newAttribute);
                    node.AppendChild(newNode);
                }

                node = Document.SelectSingleNode(string.Format("/profile//section[@name='{0}']//entry[@name='{1}']", section, name));
                if (node == null)
                {
                    // select root node
                    node = Document.SelectSingleNode(string.Format("/profile//section[@name='{0}']", section));

                    // create new entry node
                    XmlNode newNode = Document.CreateElement("entry");
                    XmlAttribute newAttribute = Document.CreateAttribute("name");
                    newAttribute.Value = name.Trim('\0');
                    newNode.Attributes.Append(newAttribute);
                    node.AppendChild(newNode);
                    node = Document.SelectSingleNode(string.Format("/profile//section[@name='{0}']//entry[@name='{1}']", section, name));
                }

                // set the value
                if (!string.IsNullOrEmpty(value))
                    value = value.Trim('\0').Trim();

                node.InnerText = string.IsNullOrEmpty(value) ? string.Empty : value;

                return true;
            }
            catch { return false; }
        }
    }
}
