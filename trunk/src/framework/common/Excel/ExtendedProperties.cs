using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DL.Framework.Common
{
    [Serializable]
    public class ExtendedProperties : IXmlSerializable, IEquatable<ExtendedProperties>
    {
        private readonly static XmlSerializer _serializer = new XmlSerializer(typeof(ExtendedProperties));

        public ExtendedProperties()
        {
            Properties = new NameValueCollection();
        }

        public ExtendedProperties(NameValueCollection properties)
        {
            Properties = properties;
        }

        [XmlIgnore]
        public string Server
        {
            get { return Properties["svr"] ?? "QPServer"; }
            set { Properties["svr"] = value; }
        }

        [XmlIgnore]
        public NameValueCollection Properties { get; set; }

        [XmlIgnore]
        public string PromptString { get; set; }

        public static ExtendedProperties Deserialize(string serializedString)
        {
            using (var sr = new StringReader(serializedString))
            {
                return (ExtendedProperties)_serializer.Deserialize(sr);
            }
        }

        public string SerializeToString()
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                _serializer.Serialize(sw, this);
            }
            return sb.ToString().Substring(sb.ToString().IndexOf("<Extended")).Replace("\r\n", "");
        }

        /// <summary>
        /// Build columns from user input
        /// </summary>
        /// <param name="lcNames"></param>
        /// <returns></returns>
        public IList<string> BuildColumnProperty(List<string> lcNames)
        {
            var requiredColumns = new List<string>();

            /*string colsProperty = ",";
            string cols = Properties["cols"];
            lcNames.ForEach(delegate(string colName)
            {
                if (!string.IsNullOrEmpty(colName))
                {
                    if (cols == null || cols.Contains("," + colName + "=1"))
                        colsProperty = String.Format("{0},{1}=1", colsProperty, colName);
                    else
                        colsProperty = String.Format("{0},{1}=2", colsProperty, colName);
                    requiredColumns.Add(colName);
                }
            });
            if (!string.IsNullOrEmpty(cols))
            {
                // col format cols=ColName=1,ColName2=0.......
                string[] colsDisplayed = cols.Split(',');
                Array.ForEach(colsDisplayed,
                delegate(string col)
                {
                    if (!string.IsNullOrEmpty(col))
                    {
                        string[] s = col.Split('=');
                        if (!colsProperty.Contains(String.Format(",{0}=", s[0])))
                            colsProperty = String.Format("{0},{1}", colsProperty, col.Replace("=1", "=-1").Replace("=2", "=-1"));
                    }
                });
            }
            if (requiredColumns.Count > 1)
            {
                requiredColumns.Remove("NoColumnsDefined");
                colsProperty = colsProperty.Replace(",NoColumnsDefined=1", "");
                requiredColumns.Remove("Column1");
                colsProperty = colsProperty.Replace(",Column1=1", "");
            }
            colsProperty = colsProperty.Replace(",,", ",");
            Properties["cols"] = colsProperty;*/

            return requiredColumns;
        }

        /// <summary>
        /// Return list of required columns
        /// cols property visible/locked = 2, visible = 1, not visible/locked = -1, not visible = 0
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        /*public List<string> BuildColumnProperty(BasicList bl)
        {
            string[] blColumnNames = bl == null ? new string[] { } : bl.ColumnNames;

            string colsProperty = "";
            List<string> requiredColumns = new List<string>();
            if (bl == null || bl.QPType != "matrix")
            {
                string cols = Properties["cols"];
                if (!string.IsNullOrEmpty(cols))
                {
                    // col format cols=ColName=1,ColName2=0.......
                    string[] colsDisplayed = cols.Split(',');
                    Array.ForEach(colsDisplayed,
                    delegate(string col)
                    {
                        if (!string.IsNullOrEmpty(col))
                        {
                            string[] s = col.Split('=');
                            if (s[1] == "0" || s[1] == "-1")
                            {
                                requiredColumns.Remove(s[0]);
                                if (colsProperty.Contains("," + s[0] + "="))
                                    colsProperty = colsProperty.Replace("," + s[0] + "=1", "," + s[0] + "=0");
                                else
                                    colsProperty = colsProperty + "," + col.Replace("=1", "=0");
                            }
                            else
                            {
                                if (colsProperty.Contains("," + s[0] + "="))
                                    colsProperty = colsProperty.Replace("," + s[0] + "=0", "," + s[0] + "=1");
                                else
                                    colsProperty = colsProperty + "," + col;
                                requiredColumns.Add(s[0]);
                            }
                        }
                    });
                }
            }
            int c = 0;
            // Add any columns from basicList not already in colsProperty
            bool addCols = requiredColumns.Count > 0 && !requiredColumns.Contains("NoColumnsDefined") ? false : true;
            foreach (string col in blColumnNames)
            {
                if (!(colsProperty.StartsWith(String.Format("{0}=", col)) || colsProperty.Contains("," + col + "=")))
                {
                    if (c == 0 && col == "XObjectMethod")
                    {
                        c++;
                        continue;
                    }
                    if (colsProperty.Length > 0)
                    {
                        colsProperty = colsProperty + "," + col + (addCols ? "=1" : "=0");
                        if (addCols)
                            requiredColumns.Add(col);
                    }
                    else
                    {
                        colsProperty = col + (addCols ? "=1" : "=0");
                        if (addCols)
                            requiredColumns.Add(col);
                    }
                }
                c++;
            }
            if (requiredColumns.Count > 1)
            {
                requiredColumns.Remove("NoColumnsDefined");
                colsProperty = colsProperty.Replace("NoColumnsDefined=1,", "");
                colsProperty = colsProperty.Replace(",NoColumnsDefined=1", "");
            }
            Properties["cols"] = colsProperty;
            return requiredColumns;
        }*/

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            /*reader.ReadStartElement();
            if (reader.IsStartElement("PromptString"))
                promptString = reader.ReadElementString("PromptString");

            properties = new NameValueCollection();

            while (reader.IsStartElement("p"))
            {
                while (reader.MoveToNextAttribute())
                {
                    properties.Add(reader.Name, reader.Value);
                }
                reader.Read();
            }*/
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            /*writer.WriteElementString("PromptString", promptString);
            writer.WriteStartElement("p");
            foreach (string key in properties.Keys)
            {
                string value = properties[key];
                writer.WriteAttributeString(key, value);
            }
            writer.WriteEndElement();*/
        }

        public bool Equals(ExtendedProperties other)
        {
            return false;
            /*if (other == null)
                return false;
            if (PromptString != other.PromptString)
                return false;
            foreach (string key in properties.Keys)
            {
                if (key != "lastRefreshed" && key != "formats" && key != "nm")
                {
                    if (other.properties[key] != properties[key])
                        return false;
                }
            }
            foreach (string key in other.Properties.Keys)
            {
                if (key != "lastRefreshed" && key != "formats" && key != "nm")
                {
                    if (other.properties[key] != properties[key])
                        return false;
                }
            }
            return true;*/
        }
    }
}
