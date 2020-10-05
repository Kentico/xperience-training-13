using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Objects_Controls_ViewObjectDataSet : CMSUserControl
{
    #region "Variables"

    private bool mEncodeDisplayedData = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// DataSet containing data to display.
    /// </summary>
    public DataSet DataSet
    {
        get;
        set;
    }


    /// <summary>
    /// DataSet containgind data to be compared.
    /// </summary>
    public DataSet CompareDataSet
    {
        get;
        set;
    }


    /// <summary>
    /// Additional HTML content to display.
    /// </summary>
    public string AdditionalContent
    {
        get
        {
            return ltlAdditionalContent.Text;
        }
        set
        {
            ltlAdditionalContent.Text = value;
        }
    }


    /// <summary>
    /// Gets table control displaying DataSets.
    /// </summary>
    public Table Table
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if all fields should be displayed(even if value is empty).
    /// </summary>
    public bool ShowAllFields
    {
        get;
        set;
    }


    /// <summary>
    /// List of excluded table names separated by semicolon (;)
    /// </summary>
    public string ExcludedTableNames
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if link to Metafile preview should be displayed instead of simple metafile name
    /// </summary>
    public bool ShowLinksForMetafiles
    {
        get;
        set;
    }


    /// <summary>
    /// Force display tables content in rows instead of separated tables
    /// </summary>
    public bool ForceRowDisplayFormat
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if displayed data should be encoded
    /// </summary>
    public bool EncodeDisplayedData
    {
        get
        {
            return mEncodeDisplayedData;
        }
        set
        {
            mEncodeDisplayedData = value;
        }
    }


    /// <summary>
    /// Object type of given data (optional)
    /// </summary>
    public string ObjectType
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Display DataSet content.
    /// </summary>
    /// <param name="ds">DataSet to display</param>
    private void DisplayDataSet(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Prepare list of tables
            string excludedTableNames = (ExcludedTableNames != null) ? ";" + ExcludedTableNames.Trim(';').ToLowerCSafe() + ";" : "";
            List<string> tables = new List<string>();

            foreach (DataTable dt in ds.Tables)
            {
                if (!DataHelper.DataSourceIsEmpty(dt))
                {
                    string tableName = dt.TableName;
                    if (!excludedTableNames.Contains(";" + tableName.ToLowerCSafe() + ";"))
                    {
                        tables.Add(tableName);
                    }
                }
            }

            // Generate the tables ordered by object type display name
            foreach (string tableName in tables.OrderBy(x => GetString("ObjectType." + x)))
            {
                DataTable dt = ds.Tables[tableName];
                if (!DataHelper.DataSourceIsEmpty(dt))
                {
                    if (ds.Tables.Count > 1)
                    {
                        plcContent.Controls.Add(new LiteralControl(GetTableHeaderText(dt)));
                    }

                    Table contentTable;

                    if (!ForceRowDisplayFormat && (dt.Columns.Count >= 6) && !dt.TableName.EqualsCSafe(TranslationHelper.TRANSLATION_TABLE, true))
                    {
                        // Write all rows
                        foreach (DataRow dr in dt.Rows)
                        {
                            contentTable = new Table();
                            SetTable(contentTable);

                            // Set first table as table property
                            if (Table == null)
                            {
                                Table = contentTable;
                            }

                            // Create table header
                            TableCell labelCell = new TableHeaderCell();
                            labelCell.Text = GetString("General.FieldName");

                            TableCell valueCell = new TableHeaderCell();
                            valueCell.Text = GetString("General.Value");

                            AddRow(contentTable, labelCell, valueCell, null, "unigrid-head", true);

                            // Add values
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string content = GetRowColumnContent(dr, dc, false);

                                if (ShowAllFields || !String.IsNullOrEmpty(content))
                                {
                                    labelCell = new TableCell();
                                    labelCell.Text = "<strong>" + dc.ColumnName + "</strong>";

                                    valueCell = new TableCell();
                                    valueCell.Text = content;
                                    AddRow(contentTable, labelCell, valueCell, null);
                                }
                            }

                            plcContent.Controls.Add(contentTable);
                            plcContent.Controls.Add(new LiteralControl("<br />"));
                        }
                    }
                    else
                    {
                        contentTable = new Table();
                        SetTable(contentTable);

                        // Add header
                        TableRow tr = new TableHeaderRow { TableSection = TableRowSection.TableHeader };
                        tr.CssClass = "unigrid-head";

                        int h = 1;
                        foreach (DataColumn column in dt.Columns)
                        {
                            TableHeaderCell th = new TableHeaderCell();
                            th.Text = ValidationHelper.GetString(column.Caption, column.ColumnName);

                            if (!ForceRowDisplayFormat && (h == dt.Columns.Count))
                            {
                                th.CssClass = "main-column-100";
                            }
                            tr.Cells.Add(th);
                            h++;
                        }
                        contentTable.Rows.Add(tr);

                        // Write all rows
                        foreach (DataRow dr in dt.Rows)
                        {
                            tr = new TableRow();

                            // Add values
                            foreach (DataColumn dc in dt.Columns)
                            {
                                TableCell tc = new TableCell();
                                object value = dr[dc.ColumnName];

                                // Possible DataTime columns
                                if ((dc.DataType == typeof(DateTime)) && (value != DBNull.Value))
                                {
                                    DateTime dateTime = Convert.ToDateTime(value);
                                    CultureInfo cultureInfo = CultureHelper.GetCultureInfo(MembershipContext.AuthenticatedUser.PreferredUICultureCode);
                                    value = dateTime.ToString(cultureInfo);
                                }

                                string content = ValidationHelper.GetString(value, String.Empty);
                                tc.Text = EncodeDisplayedData ? HTMLHelper.HTMLEncode(content) : content;

                                if (!ForceRowDisplayFormat)
                                {
                                    tc.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
                                }
                                tr.Cells.Add(tc);
                            }
                            contentTable.Rows.Add(tr);
                        }
                        plcContent.Controls.Add(contentTable);
                        plcContent.Controls.Add(new LiteralControl("<br />"));
                    }
                }
            }
        }
        else
        {
            Label lblError = new Label();
            lblError.CssClass = "InfoLabel";
            lblError.Text = GetString("general.nodatafound");
            plcContent.Controls.Add(lblError);
        }
    }


    /// <summary>
    /// Gets header text for given table
    /// </summary>
    /// <param name="table">Table to get the header for</param>
    /// <returns>HTML representing header text</returns>
    private string GetTableHeaderText(DataTable table)
    {
        string tableName = table.TableName;

        string defaultString = null;
        // ### Special cases
        if (!string.IsNullOrEmpty(ObjectType))
        {
            if (CustomTableItemProvider.IsCustomTableItemObjectType(ObjectType))
            {
                defaultString = "CustomTableData";
            }
            else if (ObjectType == TreeNode.OBJECT_TYPE)
            {
                defaultString = "DocumentData";
            }
            else
            {
                defaultString = tableName;
            }
        }
        return "<h4 class=\"listing-title\">" + ResHelper.GetAPIString("ObjectType." + tableName, Thread.CurrentThread.CurrentUICulture.ToString(), GetString("ObjectType." + defaultString)) + "</h4>";
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public void ReloadData()
    {
        if (!DataHelper.DataSourceIsEmpty(DataSet) && !DataHelper.DataSourceIsEmpty(CompareDataSet))
        {
            CompareDataSets(DataSet, CompareDataSet);
        }
        else
        {
            DisplayDataSet(DataSet);
        }
    }


    /// <summary>
    /// Compare DataSets.
    /// </summary>
    /// <param name="ds">Original DataSet</param>
    /// <param name="compareDs">DataSet to compare</param>
    private void CompareDataSets(DataSet ds, DataSet compareDs)
    {
        Table = new Table();
        SetTable(Table);
        Table.CssClass += " NoSideBorders";

        // Ensure same tables in DataSets
        EnsureSameTables(ds, compareDs);
        EnsureSameTables(compareDs, ds);

        // Prepare list of tables
        SortedDictionary<string, string> tables = new SortedDictionary<string, string>();
        foreach (DataTable dt in ds.Tables)
        {
            string excludedTableNames = (ExcludedTableNames != null) ? ";" + ExcludedTableNames.Trim(';').ToLowerCSafe() + ";" : "";
            string tableName = dt.TableName;
            if (!DataHelper.DataSourceIsEmpty(ds.Tables[tableName]) || !DataHelper.DataSourceIsEmpty(CompareDataSet.Tables[tableName]))
            {
                if (!excludedTableNames.Contains(";" + tableName.ToLowerCSafe() + ";"))
                {
                    tables.Add(GetString("ObjectType." + tableName), tableName);
                }
            }
        }

        // Generate the tables
        foreach (string tableName in tables.Values)
        {
            DataTable dt = ds.Tables[tableName].Copy();
            DataTable dtCompare = CompareDataSet.Tables[tableName].Copy();

            if (dt.PrimaryKey.Length <= 0)
            {
                continue;
            }

            // Add table heading
            if ((tables.Count > 1) || (ds.Tables.Count > 1))
            {
                AddTableHeaderRow(Table, GetTableHeaderText(dt), null);
            }

            while (dt.Rows.Count > 0 || dtCompare.Rows.Count > 0)
            {
                // Add table header row
                TableCell labelCell = new TableHeaderCell();
                labelCell.Text = GetString("General.FieldName");
                TableCell valueCell = new TableHeaderCell();
                valueCell.Text = GetString("General.Value");
                TableCell valueCompare = new TableHeaderCell();
                valueCompare.Text = GetString("General.Value");

                AddRow(Table, labelCell, valueCell, valueCompare, "unigrid-head", true);

                DataRow srcDr;
                DataRow dstDr;

                if ((tables.Count == 1) && (dt.Rows.Count == 1) && (dtCompare.Rows.Count == 1))
                {
                    srcDr = dt.Rows[0];
                    dstDr = dtCompare.Rows[0];
                }
                else
                {
                    if (!DataHelper.DataSourceIsEmpty(dt))
                    {
                        srcDr = dt.Rows[0];
                        dstDr = dtCompare.Rows.Find(GetPrimaryColumnsValue(dt, srcDr));
                    }
                    else
                    {
                        dstDr = dtCompare.Rows[0];
                        srcDr = dt.Rows.Find(GetPrimaryColumnsValue(dtCompare, dstDr));
                    }

                    // If match not find, try to find in guid column
                    if ((srcDr == null) || (dstDr == null))
                    {
                        DataTable dtToSearch;
                        DataRow drTocheck;

                        if (srcDr == null)
                        {
                            dtToSearch = dt;
                            drTocheck = dstDr;
                        }
                        else
                        {
                            dtToSearch = dtCompare;
                            drTocheck = srcDr;
                        }


                        GeneralizedInfo infoObj = ModuleManager.GetObject(drTocheck, dt.TableName.Replace("_", "."));
                        if ((infoObj != null) && ((infoObj.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN) || (infoObj.TypeInfo.GUIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)))
                        {
                            DataRow[] rows = dtToSearch.Select(infoObj.CodeNameColumn + "='" + drTocheck[infoObj.CodeNameColumn] + "'");
                            if (rows.Length > 0)
                            {
                                if (srcDr == null)
                                {
                                    srcDr = rows[0];
                                }
                                else
                                {
                                    dstDr = rows[0];
                                }
                            }
                            else
                            {
                                rows = dtToSearch.Select(infoObj.TypeInfo.GUIDColumn + "='" + drTocheck[infoObj.TypeInfo.GUIDColumn] + "'");
                                if (rows.Length > 0)
                                {
                                    if (srcDr == null)
                                    {
                                        srcDr = rows[0];
                                    }
                                    else
                                    {
                                        dstDr = rows[0];
                                    }
                                }
                            }
                        }
                    }
                }

                // Add values
                foreach (DataColumn dc in dt.Columns)
                {
                    // Get content values
                    string fieldContent = GetRowColumnContent(srcDr, dc, true);
                    string fieldCompareContent = GetRowColumnContent(dstDr, dc, true);

                    if (ShowAllFields || !String.IsNullOrEmpty(fieldContent) || !String.IsNullOrEmpty(fieldCompareContent))
                    {
                        // Initialize comparators
                        TextComparison comparefirst = new TextComparison();
                        comparefirst.SynchronizedScrolling = false;
                        comparefirst.ComparisonMode = TextComparisonModeEnum.PlainTextWithoutFormating;
                        comparefirst.EnsureHTMLLineEndings = true;

                        TextComparison comparesecond = new TextComparison();
                        comparesecond.SynchronizedScrolling = false;
                        comparesecond.RenderingMode = TextComparisonTypeEnum.DestinationText;
                        comparesecond.EnsureHTMLLineEndings = true;

                        comparefirst.PairedControl = comparesecond;

                        // Set comparator content 
                        comparefirst.SourceText = fieldContent;
                        comparefirst.DestinationText = fieldCompareContent;

                        // Create set of cells
                        labelCell = new TableCell();
                        labelCell.Text = "<strong>" + dc.ColumnName + "</strong>";
                        valueCell = new TableCell();
                        valueCell.Controls.Add(comparefirst);
                        valueCompare = new TableCell();
                        valueCompare.Controls.Add(comparesecond);

                        // Add comparison row
                        AddRow(Table, labelCell, valueCell, valueCompare);
                    }
                }

                // Remove rows from tables
                if (srcDr != null)
                {
                    dt.Rows.Remove(srcDr);
                }
                if (dstDr != null)
                {
                    dtCompare.Rows.Remove(dstDr);
                }

                if (dt.Rows.Count > 0 || dtCompare.Rows.Count > 0)
                {
                    TableCell emptyCell = new TableCell();
                    emptyCell.Text = "&nbsp;";
                    AddRow(Table, emptyCell, null, null, "TableSeparator");
                }
            }
        }
        plcContent.Controls.Add(Table);
    }


    /// <summary>
    /// Creates 3 column table row.
    /// </summary>
    /// <param name="table">Table element</param>
    /// <param name="labelCell">Cell with label</param>
    /// <param name="valueCell">Cell with content</param>
    /// <param name="compareCell">Cell with compare content</param>
    /// <param name="cssClass">CSS class</param>
    /// <param name="isHeader">Indicates if created row is header row (False by default)</param>
    /// <returns>Returns TableRow object</returns>
    private void AddRow(Table table, TableCell labelCell, TableCell valueCell, TableCell compareCell, string cssClass = null, bool isHeader = false)
    {
        TableRow newRow = new TableRow();

        if (!String.IsNullOrEmpty(cssClass))
        {
            newRow.CssClass = cssClass + (isHeader ? " UniGridHead" : "");
        }

        labelCell.Width = new Unit(20, UnitType.Percentage);
        newRow.Cells.Add(labelCell);

        int cellWidth = (compareCell != null) ? 40 : 80;

        if (valueCell != null)
        {
            valueCell.Width = new Unit(cellWidth, UnitType.Percentage);
            newRow.Cells.Add(valueCell);
        }

        if (compareCell != null)
        {
            compareCell.Width = new Unit(cellWidth, UnitType.Percentage);
            newRow.Cells.Add(compareCell);
        }

        table.Rows.Add(newRow);
    }


    /// <summary>
    /// Sets table properties to ensure same design.
    /// </summary>
    /// <param name="table">Table to adjust</param>
    private void SetTable(Table table)
    {
        table.CellPadding = -1;
        table.CellSpacing = -1;
        table.CssClass = "table table-hover";
    }


    /// <summary>
    /// Ensure that compared DataSet will contain same tables as source DataSet.
    /// </summary>
    /// <param name="ds">Source DataSet</param>
    /// <param name="dsCompare">Compared DataSet</param>
    private void EnsureSameTables(DataSet ds, DataSet dsCompare)
    {
        for (int i = 0; i < ds.Tables.Count; i++)
        {
            DataTable procTable = ds.Tables[i];

            if (!dsCompare.Tables.Contains(procTable.TableName))
            {
                dsCompare.Tables.Add(procTable.Clone());
            }
        }
    }


    /// <summary>
    /// Adds table header.
    /// </summary>
    /// <param name="table">Table to which header will be added</param>
    /// <param name="text">Header text</param>
    /// <param name="cssClass">Css class of header</param>
    /// <returns>TableRow with header text</returns>
    private void AddTableHeaderRow(Table table, string text, string cssClass)
    {
        TableRow newRow = new TableRow();

        if (!String.IsNullOrEmpty(cssClass))
        {
            newRow.CssClass = cssClass;
        }

        TableCell cell = new TableCell();
        cell.ColumnSpan = 3;
        cell.Text = text;

        newRow.Cells.Add(cell);
        table.Rows.Add(newRow);
    }


    /// <summary>
    /// Gets row column content.
    /// </summary>
    /// <param name="dr">DataRow</param>
    /// <param name="dc">DataColumn</param>
    /// <param name="toCompare">Indicates if comparison will be used for content</param>
    /// <returns>String with column content</returns>
    private string GetRowColumnContent(DataRow dr, DataColumn dc, bool toCompare)
    {
        if (dr == null)
        {
            // Data row was not specified
            return string.Empty;
        }

        if (!dr.Table.Columns.Contains(dc.ColumnName))
        {
            // Data row does not contain the required column
            return string.Empty;
        }

        var value = dr[dc.ColumnName];

        if (DataHelper.IsEmpty(value))
        {
            // Column is empty
            return string.Empty;
        }

        var content = ValidationHelper.GetString(value, "");

        Func<string> render = () =>
        {
            if (toCompare)
            {
                return content;
            }

            content = HTMLHelper.EnsureHtmlLineEndings(content);

            return content;
        };

        Func<string> standardRender = () =>
        {
            if (toCompare)
            {
                return content;
            }

            if (EncodeDisplayedData)
            {
                content = HTMLHelper.HTMLEncode(content);
            }
            content = content.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            content = "<div style=\"max-height: 300px; overflow: auto;\">" + content + "</div>";
            content = HTMLHelper.EnsureHtmlLineEndings(content);

            return content;
        };

        // Binary columns
        if (dc.DataType == typeof(byte[]))
        {
            var data = (byte[])dr[dc.ColumnName];
            content = string.Format("<{0}: {1}>", GetString("General.BinaryData"), DataHelper.GetSizeString(data.Length));

            return standardRender();
        }

        // DataTime columns
        if (dc.DataType == typeof(DateTime))
        {
            var dateTime = Convert.ToDateTime(content);
            var cultureInfo = CultureHelper.GetCultureInfo(MembershipContext.AuthenticatedUser.PreferredUICultureCode);
            content = dateTime.ToString(cultureInfo);

            return standardRender();
        }

        switch (dc.ColumnName.ToLowerCSafe())
        {
            // Document content
            case "documentcontent":
                var sb = new StringBuilder();

                Action<MultiKeyDictionary<string>, string, string> addItems = (dictionary, titleClass, textClass) =>
                {
                    foreach (DictionaryEntry item in dictionary)
                    {
                        var regionContent = HTMLHelper.ResolveUrls((string)item.Value, SystemContext.ApplicationPath);

                        if (toCompare)
                        {
                            sb.AppendLine((string)item.Key);
                            sb.AppendLine(regionContent);
                        }
                        else
                        {
                            sb.AppendFormat("<span class=\"{0}\">{1}</span>", titleClass, item.Key);
                            sb.AppendFormat("<span class=\"{0}\">{1}</span>", textClass, HTMLHelper.HTMLEncode(regionContent));
                        }
                    }
                };

                var items = new EditableItems();
                items.LoadContentXml(ValidationHelper.GetString(value, ""));

                // Add regions
                addItems(items.EditableRegions, "VersionEditableRegionTitle", "VersionEditableRegionText");

                // Add web parts
                addItems(items.EditableWebParts, "VersionEditableWebPartTitle", "VersionEditableWebPartText");

                content = sb.ToString();
                return render();

            // XML columns
            case "pagetemplatewebparts":
            case "webpartproperties":
            case "reportparameters":
            case "classformdefinition":
            case "classxmlschema":
            case "classformlayout":
            case "userdialogsconfiguration":
            case "siteinvoicetemplate":
            case "userlastlogoninfo":
            case "formdefinition":
            case "formlayout":
            case "classsearchsettings":
            case "graphsettings":
            case "tablesettings":
            case "issuetext":
            case "issuewidgets":
            case "savedreportparameters":
            case "emailwidgetproperties":

            // HTML columns
            case "emailtemplatetext":
            case "emailwidgetcode":
            case "templatebody":
            case "templateheader":
            case "templatefooter":
            case "containertextbefore":
            case "containertextafter":
            case "savedreporthtml":
            case "layoutcode":
            case "webpartlayoutcode":
            case "transformationcode":
            case "reportlayout":
#pragma warning disable CS0618 // Type or member is obsolete
                if (BrowserHelper.IsIE())
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    content = HTMLHelper.ReformatHTML(content, " ");
                }
                else
                {
                    content = HTMLHelper.ReformatHTML(content);
                }
                break;

            // File columns
            case "metafilename":
                var metaFileName = HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["MetaFileName"], ""));

                if (ShowLinksForMetafiles)
                {
                    var metaFileGuid = ValidationHelper.GetGuid(dr["MetaFileGuid"], Guid.Empty);
                    if (metaFileGuid != Guid.Empty)
                    {
                        var metaFileUrl = ResolveUrl(MetaFileInfoProvider.GetMetaFileUrl(metaFileGuid, metaFileName));
                        content = string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", metaFileUrl, metaFileName);
                    }
                }
                else
                {
                    content = metaFileName;
                }

                return render();
        }

        return standardRender();
    }


    private object[] GetPrimaryColumnsValue(DataTable dt, DataRow dr)
    {
        if (dt.PrimaryKey.Length > 0)
        {
            object[] columns = new object[dt.PrimaryKey.Length];
            for (int i = 0; i < dt.PrimaryKey.Length; i++)
            {
                columns[i] = dr[dt.PrimaryKey[i].ColumnName];
            }
            return columns;
        }
        return null;
    }

    #endregion
}