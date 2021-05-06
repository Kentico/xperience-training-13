using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Helpers.Internal;
using CMS.Membership;
using CMS.UIControls;

using Newtonsoft.Json.Linq;

public partial class CMSAdminControls_UI_System_ViewObject : CMSAdminEditControl
{
    #region "Constants"

    private const int MAX_HEADER_LEVEL = 6;
    private const string JSON_TYPE_PROPERTY = "$type";

    #endregion


    #region "Variables"

    protected object mObject = null;

    // List of tables [DataTable, UniGridPager]
    protected ArrayList tables = new ArrayList();

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the object to display.
    /// </summary>
    public object Object
    {
        get
        {
            return mObject;
        }
        set
        {
            mObject = value;
            ReloadData();
        }
    }

    #endregion"


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        WriteTablesContent();
    }


    public override void ReloadData()
    {
        tables.Clear();
        pnlContent.Controls.Clear();

        WriteObject(Object, 3);
    }


    protected void WriteObject(object obj, int headerLevel)
    {
        // Write the objects
        if (obj != null)
        {
            if (obj == DBNull.Value)
            {
                WriteHeader("DBNull.Value", headerLevel);
            }
            if (obj is DataView)
            {
                WriteDataTable(((DataView)obj).Table, headerLevel);
            }
            else if (obj is DataTable)
            {
                WriteDataTable((DataTable)obj, headerLevel);
            }
            else
            {
                var typeName = obj.GetType().ToString();

                WriteObjectTypeHeader(typeName, headerLevel);

                if (obj is DataSet)
                {
                    WriteDataSet((DataSet)obj, headerLevel);
                }
                else if (obj is IDataContainer)
                {
                    WriteDataContainer((IDataContainer)obj);
                }
                else if (obj is string)
                {
                    var lines = ((string)obj).Count(c => c == '\n');
                    pnlContent.Controls.Add(new CMSTextArea()
                    {
                        Text = HttpUtility.HtmlEncode((string)obj),
                        Rows = lines,
                        ReadOnly = true
                    });
                }
                else if (obj is UnknownTypeDictionary<string, object>)
                {
                    WriteDictionary(headerLevel, "IDictionary (unknown type)", (IDictionary)obj);
                }
                else if (obj is IDictionary)
                {
                    WriteDictionary(headerLevel, "IDictionary", (IDictionary)obj);
                }
                else if (headerLevel < MAX_HEADER_LEVEL && obj is IEnumerable)
                {
                    // Write all objects in enumeration (array, collection, etc.)
                    foreach (object child in (IEnumerable)obj)
                    {
                        WriteObject(child, headerLevel + 1);
                    }
                }
                else
                {
                    WriteObject(obj, headerLevel, typeName);
                }
            }
        }

        pnlContent.Controls.Add(new LiteralControl("<br /><br />"));
    }


    private bool TryGetObject(JObject sourceObj, out object targetObj)
    {
        var objectTypeName = sourceObj[JSON_TYPE_PROPERTY].ToString();
        if (objectTypeName != null)
        {
            var objectType = Type.GetType(objectTypeName);

            if (objectType != null)
            {
                targetObj = sourceObj.ToObject(objectType);
                return true;
            }
        }

        targetObj = sourceObj;
        return false;
    }


    protected void WriteTablesContent()
    {
        foreach (object[] table in tables)
        {
            // Prepare the components
            DataTable dt = (DataTable)table[0];
            LiteralControl ltlContent = (LiteralControl)table[1];
            UIPager pagerElem = (UIPager)table[2];
            UniPagerConnector connectorElem = (UniPagerConnector)table[3];

            // Handle the different types of direct page selector
            int currentPageSize = pagerElem.CurrentPageSize;
            if (currentPageSize > 0)
            {
                if (connectorElem.PagerForceNumberOfResults / (float)currentPageSize > 20.0f)
                {
                    pagerElem.DirectPageControlID = "txtPage";
                }
                else
                {
                    pagerElem.DirectPageControlID = "drpPage";
                }
            }

            // Bind the pager first
            connectorElem.RaiseOnPageBinding(null, null);

            // Prepare the string builder
            StringBuilder sb = new StringBuilder();

            // Prepare the indexes for paging
            int pageSize = pagerElem.CurrentPageSize;

            int startIndex = (pagerElem.CurrentPage - 1) * pageSize + 1;
            int endIndex = startIndex + pageSize;

            // Process all items
            int index = 0;
            bool all = (endIndex <= startIndex);

            if (dt.Columns.Count > 6)
            {
                // Write all rows
                foreach (DataRow dr in dt.Rows)
                {
                    index++;
                    if (all || (index >= startIndex) && (index < endIndex))
                    {
                        sb.Append("<table class=\"table table-hover\">");

                        // Add header
                        sb.AppendFormat("<thead><tr class=\"unigrid-head\"><th>{0}</th><th class=\"main-column-100\">{1}</th></tr></thead><tbody>", GetString("General.FieldName"), GetString("General.Value"));

                        // Add values
                        foreach (DataColumn dc in dt.Columns)
                        {
                            object value = dr[dc.ColumnName];

                            // Binary columns
                            string content;
                            if ((dc.DataType == typeof(byte[])) && (value != DBNull.Value))
                            {
                                byte[] data = (byte[])value;
                                content = "<" + GetString("General.BinaryData") + ", " + DataHelper.GetSizeString(data.Length) + ">";
                            }
                            else
                            {
                                content = ValidationHelper.GetString(value, String.Empty);
                            }

                            if (!String.IsNullOrEmpty(content))
                            {
                                sb.AppendFormat("<tr><td><strong>{0}</strong></td><td class=\"wrap-normal\">", dc.ColumnName);

                                // Possible DataTime columns
                                if ((dc.DataType == typeof(DateTime)) && (value != DBNull.Value))
                                {
                                    DateTime dateTime = Convert.ToDateTime(content);
                                    CultureInfo cultureInfo = CultureHelper.GetCultureInfo(MembershipContext.AuthenticatedUser.PreferredUICultureCode);
                                    content = dateTime.ToString(cultureInfo);
                                }

                                // Process content
                                ProcessContent(sb, dr, dc.ColumnName, ref content);

                                sb.Append("</td></tr>");
                            }
                        }

                        sb.Append("</tbody></table>\n");
                    }
                }
            }
            else
            {
                sb.Append("<table class=\"table table-hover\">");

                // Add header
                sb.Append("<thead><tr class=\"unigrid-head\">");
                int h = 1;
                foreach (DataColumn column in dt.Columns)
                {
                    sb.AppendFormat("<th{0}>{1}</th>", (h == dt.Columns.Count) ? " class=\"main-column-100\"" : String.Empty, column.ColumnName);
                    h++;
                }
                sb.Append("</tr></thead><tbody>");

                // Write all rows
                foreach (DataRow dr in dt.Rows)
                {
                    index++;
                    if (all || (index >= startIndex) && (index < endIndex))
                    {
                        sb.Append("<tr>");

                        // Add values
                        foreach (DataColumn dc in dt.Columns)
                        {
                            object value = dr[dc.ColumnName];
                            // Possible DataTime columns
                            if ((dc.DataType == typeof(DateTime)) && (value != DBNull.Value))
                            {
                                DateTime dateTime = Convert.ToDateTime(value);
                                CultureInfo cultureInfo = CultureHelper.GetCultureInfo(MembershipContext.AuthenticatedUser.PreferredUICultureCode);
                                value = dateTime.ToString(cultureInfo);
                            }

                            string content = ValidationHelper.GetString(value, String.Empty);
                            content = HTMLHelper.HTMLEncode(content);

                            sb.AppendFormat("<td{0}>{1}</td>", (content.Length >= 100) ? " class=\"wrap-normal\"" : String.Empty, content);
                        }
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</tbody></table>\n");
            }

            ltlContent.Text = sb.ToString();
        }
    }


    /// <summary>
    /// Writes the header to output.
    /// </summary>
    /// <param name="text">Header text</param>
    /// <param name="level">Header level</param>
    private void WriteHeader(string text, int level)
    {
        if (level > MAX_HEADER_LEVEL)
        {
            level = MAX_HEADER_LEVEL;
        }

        var h = new LocalizedHeading { Text = text, Level = level };

        pnlContent.Controls.Add(h);
    }


    /// <summary>
    /// Writes the object type to the output.
    /// </summary>
    /// <param name="objectType">Object type</param>
    /// <param name="level">Header level</param>
    private void WriteObjectTypeHeader(string objectType, int level)
    {
        WriteHeader(GetString("general.objecttype") + ": " + objectType, level);
    }


    /// <summary>
    /// Writes regular object to the output.
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="headerLevel">Header level</param>
    /// <param name="typeName">Object type name</param>
    private void WriteObject(object obj, int headerLevel, string typeName)
    {
        // Write regular object
        var objString = DataHelper.GetObjectString(obj);
        if (objString != typeName)
        {
            pnlContent.Controls.Add(new LiteralControl("<div>" + objString + "</div>"));
        }
        else
        {
            var objDict = new Dictionary<string, object>();
            var objType = obj.GetType();

            // struct
            if (objType.IsValueType && !objType.IsEnum)
            {
                objDict = objType.GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj));
            }
            // object
            else
            {
                objDict = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop =>
                    {
                        var valTemp = prop.GetValue(obj, null);
                        if (valTemp != null && valTemp is string)
                        {
                            return (string)valTemp;
                        }
                        return valTemp;
                    });
            }

            WriteDictionary(headerLevel, String.Empty, objDict);
        }
    }


    /// <summary>
    /// Writes the object as dictionary to the output.
    /// </summary>
    /// <param name="headerLevel">Header level</param>
    /// <param name="typeName">Object type name</param>
    /// <param name="objDict">Object to write</param>
    private void WriteDictionary(int headerLevel, string typeName, IDictionary objDict)
    {
        var table = new DataTable(typeName);
        table.Columns.Add("Key");
        table.Columns.Add("Value");

        foreach (DictionaryEntry item in objDict)
        {
            var keyString = item.Key?.ToString() ?? "null";
            var valueString = item.Value?.ToString() ?? "null";

            table.Rows.Add(keyString, valueString);
        }

        WriteDataTable(table, headerLevel);
    }


    /// <summary>
    /// Writes the table content to the output.
    /// </summary>
    /// <param name="dt">Table to write</param>
    /// <param name="headerLevel">Header level</param>
    private void WriteDataTable(DataTable dt, int headerLevel)
    {
        WriteHeader(dt.TableName, headerLevel);

        if (!DataHelper.DataSourceIsEmpty(dt))
        {
            // Add literal for content
            LiteralControl ltlContent = new LiteralControl();
            pnlContent.Controls.Add(ltlContent);

            // Add control for pager
            UIPager pagerElem = (UIPager)LoadUserControl("~/CMSAdminControls/UI/Pager/UIPager.ascx");
            pagerElem.ID = $"{dt.TableName}_{pnlContent.Controls.OfType<UIPager>().Count() + 1}_pager";
            pagerElem.PageSizeOptions = "1,2,5,10,25,50,100";
            if (dt.Columns.Count > 10)
            {
                pagerElem.DefaultPageSize = 1;
            }
            else if (dt.Columns.Count > 6)
            {
                pagerElem.DefaultPageSize = 2;
            }
            pnlContent.Controls.Add(pagerElem);
            pagerElem.ShowPageSize = false;

            // Add pager connector
            UniPagerConnector connectorElem = new UniPagerConnector();
            connectorElem.PagerForceNumberOfResults = dt.Rows.Count;
            pagerElem.PagedControl = connectorElem;
            pnlContent.Controls.Add(connectorElem);

            tables.Add(new object[] { dt, ltlContent, pagerElem, connectorElem });
        }
    }


    /// <summary>
    /// Writes the table content to the output.
    /// </summary>
    /// <param name="ds">DataSet to write</param>
    /// <param name="headerLevel">Header level</param>
    protected void WriteDataSet(DataSet ds, int headerLevel)
    {
        // Write all tables
        foreach (DataTable dt in ds.Tables)
        {
            WriteDataTable(dt, headerLevel + 1);
        }
    }


    /// <summary>
    /// Writes the data container to the output.
    /// </summary>
    /// <param name="dc">Container to write</param>
    protected void WriteDataContainer(IDataContainer dc)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<table class=\"table table-hover\" style=\"margin-top: 16px;\">");

        // Add header
        sb.AppendFormat("<thead><tr class=\"unigrid-head\"><th>{0}</th><th class=\"main-column-100\">{1}</th></tr></thead><tbody>", GetString("General.FieldName"), GetString("General.Value"));

        // Add values
        foreach (string column in dc.ColumnNames)
        {
            try
            {
                object value = dc.GetValue(column);

                // Binary columns
                string content;
                if (value is byte[])
                {
                    byte[] data = (byte[])value;
                    content = "<" + GetString("General.BinaryData") + ", " + DataHelper.GetSizeString(data.Length) + ">";
                }
                else
                {
                    content = ValidationHelper.GetString(value, String.Empty);
                }

                if (!String.IsNullOrEmpty(content))
                {
                    sb.AppendFormat("<tr><td><strong>{0}</strong></td><td class=\"wrap-normal\">", column);

                    // Possible DataTime columns
                    if (value is DateTime)
                    {
                        DateTime dateTime = Convert.ToDateTime(content);
                        CultureInfo cultureInfo = CultureHelper.GetCultureInfo(MembershipContext.AuthenticatedUser.PreferredUICultureCode);
                        content = dateTime.ToString(cultureInfo);
                    }

                    // Process content
                    ProcessContent(sb, dc, column, ref content);

                    sb.Append("</td></tr>");
                }
            }
            catch
            {
            }
        }

        sb.Append("</tbody></table><br />\n");

        pnlContent.Controls.Add(new LiteralControl(sb.ToString()));
    }


    /// <summary>
    /// Processes the content.
    /// </summary>
    /// <param name="sb">StringBuilder to write</param>
    /// <param name="source">Source object</param>
    /// <param name="column">Column</param>
    /// <param name="content">Content</param>
    protected void ProcessContent(StringBuilder sb, object source, string column, ref string content)
    {
        bool standard = true;
        switch (column.ToLowerCSafe())
        {
                // Document content
            case "documentcontent":
                EditableItems items = new EditableItems();
                items.LoadContentXml(content);

                // Add regions
                foreach (DictionaryEntry region in items.EditableRegions)
                {
                    sb.Append("<span class=\"VersionEditableRegionTitle\">" + (string)region.Key + "</span>");

                    string regionContent = HTMLHelper.ResolveUrls((string)region.Value, SystemContext.ApplicationPath);

                    sb.Append("<span class=\"VersionEditableRegionText\">" + regionContent + "</span>");
                }

                // Add web parts
                foreach (DictionaryEntry part in items.EditableWebParts)
                {
                    sb.Append("<span class=\"VersionEditableWebPartTitle\">" + (string)part.Key + "</span>");

                    string regionContent = HTMLHelper.ResolveUrls((string)part.Value, SystemContext.ApplicationPath);
                    sb.Append("<span class=\"VersionEditableWebPartText\">" + regionContent + "</span>");
                }

                standard = false;
                break;

                // XML columns
            case "pagetemplatewebparts":
            case "webpartproperties":
            case "reportparameters":
            case "classformdefinition":
            case "classxmlschema":
            case "classformlayout":
            case "userdialogsconfiguration":
                content = HTMLHelper.ReformatHTML(content);
                break;

                // File columns
            case "metafilename":
                {
                    Guid metaFileGuid = ValidationHelper.GetGuid(GetValueFromSource(source, "MetaFileGuid"), Guid.Empty);
                    if (metaFileGuid != Guid.Empty)
                    {
                        string metaFileName = ValidationHelper.GetString(GetValueFromSource(source, "MetaFileName"), "");

                        content = "<a href=\"" + ResolveUrl(MetaFileInfoProvider.GetMetaFileUrl(metaFileGuid, metaFileName)) + "\" target=\"_blank\" >" + HTMLHelper.HTMLEncode(metaFileName) + "</a>";
                        sb.Append(content);

                        standard = false;
                    }
                }
                break;
        }

        // Standard rendering
        if (standard)
        {
            if (content.Length > 500)
            {
                content = TextHelper.EnsureMaximumLineLength(content, 50, "&#x200B;", true);
            }
            else
            {
                content = HTMLHelper.HTMLEncode(content);
            }

            content = HTMLHelper.EnsureHtmlLineEndings(content);
            content = content.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            sb.Append("<div style=\"max-height: 300px; overflow: auto;\">" + content + "</div>");
        }
    }


    /// <summary>
    /// Gets the value from given source.
    /// </summary>
    /// <param name="source">Source object</param>
    /// <param name="column">Column name</param>
    protected object GetValueFromSource(object source, string column)
    {
        if (source is DataRow)
        {
            DataRow dr = (DataRow)source;
            return dr[column];
        }
        else if (source is IDataContainer)
        {
            IDataContainer dc = (IDataContainer)source;
            return dc.GetValue(column);
        }

        return null;
    }
}
