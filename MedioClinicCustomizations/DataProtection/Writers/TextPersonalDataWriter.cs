using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using CMS.DataEngine;

namespace MedioClinicCustomizations.DataProtection.Writers
{
    public class TextPersonalDataWriter
    {
        private readonly StringBuilder stringBuilder;
        private int indentationLevel;

        public TextPersonalDataWriter()
        {
            stringBuilder = new StringBuilder();
            indentationLevel = 0;
        }

        // Writes horizontal tabs based on the current indentation level
        private void Indent()
        {
            stringBuilder.Append('\t', indentationLevel);
        }

        // Writes text representing a new section of data, and increases the indentation level
        public void WriteStartSection(string sectionName)
        {
            Indent();

            stringBuilder.AppendLine(sectionName + ": ");
            indentationLevel++;
        }

        // Writes the specified columns of an Xperience object (BaseInfo) and their values
        public void WriteObject(BaseInfo baseInfo, List<Tuple<string, string>> columns)
        {
            foreach (var column in columns)
            {
                // Gets the name of the current column
                string columnName = column.Item1;
                // Gets a user-friendly name for the current column
                string columnDisplayName = column.Item2;

                // Filters out identifier columns from the human-readable text data
                if (columnName.Equals(baseInfo.TypeInfo.IDColumn, StringComparison.Ordinal) ||
                    columnName.Equals(baseInfo.TypeInfo.GUIDColumn, StringComparison.Ordinal))
                {
                    continue;
                }

                // Gets the value of the current column for the given object
                object value = baseInfo.GetValue(columnName);

                if (value != null)
                {
                    Indent();
                    stringBuilder.AppendFormat("{0}: ", columnDisplayName);
                    stringBuilder.Append(value);
                    stringBuilder.AppendLine();
                }
            }
        }

        // "Closes" a text section by reducing the indentation level
        public void WriteEndSection()
        {
            indentationLevel--;
        }

        // Gets a string containing the writer's overall text
        public string GetResult()
        {
            return stringBuilder.ToString();
        }
    }
}
