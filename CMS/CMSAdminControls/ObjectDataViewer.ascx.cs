using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_ObjectDataViewer : CMSAdminEditControl
{
    #region "Variables"

    private string mObjectType = null;
    private int mObjectID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return mObjectType;
        }
        set
        {
            mObjectType = value;
        }
    }


    /// <summary>
    /// Gets or sets object ID.
    /// </summary>
    public int ObjectID
    {
        get
        {
            return mObjectID;
        }
        set
        {
            mObjectID = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do not load the data
        }
        else
        {
            // Get the base object
            GeneralizedInfo info = ModuleManager.GetReadOnlyObject(ObjectType);
            if (info != null)
            {
                var ti = info.TypeInfo;

                DataSet ds = info.GetData(null, ti.IDColumn + " = " + ObjectID, null, 0, null, false);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    ds.Tables[0].TableName = ValidationHelper.GetIdentifier(ti.ObjectClassName);
                    DisplayData(ds);
                }
            }
        }
    }

    #endregion


    #region "Other methods

    /// <summary>
    /// Displays data in table.
    /// </summary>
    /// <param name="ds">Dataset with data</param>
    protected void DisplayData(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Prepare list of tables
            SortedDictionary<string, DataTable> tables = new SortedDictionary<string, DataTable>();
            foreach (DataTable dt in ds.Tables)
            {
                if (!DataHelper.DataSourceIsEmpty(dt))
                {
                    tables.Add(GetString("ObjectType." + dt.TableName), dt);
                }
            }

            // Generate the tables
            foreach (DataTable dt in tables.Values)
            {
                pnlContent.Controls.Add(new LiteralControl("<h3>" + GetString("ObjectType." + dt.TableName) + "</h3>"));

                if (dt.Columns.Count >= 6)
                {
                    StringBuilder sb;

                    // Write all rows
                    foreach (DataRow dr in dt.Rows)
                    {
                        sb = new StringBuilder();

                        sb.Append("<table class=\"table table-hover\">");

                        // Add header
                        sb.AppendFormat("<thead><tr class=\"unigrid-head\"><th>{0}</th><th class=\"main-column-100\">{1}</th></tr></thead><tbody>", GetString("General.FieldName"), GetString("General.Value"));

                        // Add values
                        foreach (DataColumn dc in dt.Columns)
                        {
                            sb.AppendFormat("<tr><td><strong>{0}</strong></td><td class=\"wrap-normal\">", dc.ColumnName);

                            string content = null;

                            // Binary columns
                            if ((dc.DataType == typeof(byte[])) && (dr[dc.ColumnName] != DBNull.Value))
                            {
                                content = "<binary data>";
                            }
                            else
                            {
                                content = ValidationHelper.GetString(dr[dc.ColumnName], String.Empty);
                            }

                            bool standard = true;
                            switch (dc.ColumnName.ToLowerCSafe())
                            {
                                    // Document content
                                case "documentcontent":
                                    EditableItems items = new EditableItems();
                                    items.LoadContentXml(ValidationHelper.GetString(dr[dc.ColumnName], String.Empty));

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
                                    content = HTMLHelper.ReformatHTML(content);
                                    break;
                            }

                            // Standard rendering
                            if (standard)
                            {
                                content = HTMLHelper.HTMLEncodeLineBreaks(content);
                                content = content.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                                sb.Append("<div style=\"max-height: 300px; overflow: auto;\">" + content + "</div>");
                            }

                            sb.Append("</td></tr>");
                        }

                        sb.Append("</tbody></table><br />\n");

                        pnlContent.Controls.Add(new LiteralControl(sb.ToString()));
                    }
                }
                else
                {
                    GridView newGrid = new GridView();
                    newGrid.ID = "grid" + dt.TableName;
                    newGrid.EnableViewState = false;
                    newGrid.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    newGrid.CellPadding = 3;
                    newGrid.GridLines = GridLines.Horizontal;

                    pnlContent.Controls.Add(newGrid);

                    newGrid.DataSource = ds;
                    newGrid.DataMember = dt.TableName;

                    newGrid.DataBind();
                }
            }
        }
    }

    #endregion
}