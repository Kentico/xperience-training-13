using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using CMS.DataEngine;
using CMS.Helpers;

namespace MedioClinicCustomizations.DataProtection.Writers
{
    public class XmlPersonalDataWriter : IDisposable
    {
        private readonly StringBuilder stringBuilder;
        private readonly XmlWriter xmlWriter;

        public XmlPersonalDataWriter()
        {
            stringBuilder = new StringBuilder();
            xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true });
        }

        // Writes an opening XML tag with a specified name
        public void WriteStartSection(string sectionName)
        {
            // Replaces period characters in object names with underscores
            sectionName = sectionName.Replace('.', '_');

            xmlWriter.WriteStartElement(sectionName);
        }

        // Writes XML tags representing the specified columns of an Xperience object (BaseInfo) and their values
        public void WriteObject(BaseInfo baseInfo, List<string> columns)
        {
            foreach (string column in columns)
            {
                object value = baseInfo.GetValue(column);

                if (value != null)
                {
                    xmlWriter.WriteStartElement(column);
                    xmlWriter.WriteValue(XmlHelper.ConvertToString(value));
                    xmlWriter.WriteEndElement();
                }
            }
        }

        // Writes a closing XML tag for the most recent open tag
        public void WriteEndSection()
        {
            xmlWriter.WriteEndElement();
        }

        // Gets a string containing the writer's overall XML data
        public string GetResult()
        {
            xmlWriter.Flush();

            return stringBuilder.ToString();
        }

        // Releases all resources used by the current XmlPersonalDataWriter instance.
        public void Dispose()
        {
            xmlWriter.Dispose();
        }
    }
}
