using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Helpers;

using System.Linq;
using System.Web.UI;

using CMS.Base;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.IO;
using CMS.Reporting;


public partial class CMSModules_Reporting_FormControls_ReportItemSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mDisplay = true;
    private DataSet mCurrentDataSet;
    private bool mKeepDataInWindowsHelper;
    private string mFirstItemText = String.Empty;
    private ReportInfo mReportInfo;
    private bool mSetValues;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value that indicates whether id value should be used in selector
    /// </summary>
    public bool UseIDValue
    {
        get;
        set;
    }


    /// <summary>
    /// If set <c>false</c> control shows only report selector.
    /// </summary>
    public bool ShowItemSelector
    {
        get
        {
            return GetValue("ShowItemSelector", false);
        }
        set
        {
            SetValue("ShowItemSelector", value);
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();

            string usReportsValue = GetString(usReports.Value.ToString());
            string usItemsValue = GetString(usItems.Value.ToString());

            if (!ShowItemSelector)
            {
                if (usReportsValue == "0")
                {
                    return String.Empty;
                }
                return usReportsValue;
            }

            if ((usReportsValue == "0") || (usItemsValue == "0"))
            {
                return String.Empty;
            }
            return String.Format("{0};{1}", usReportsValue, usItemsValue);
        }
        set
        {
            EnsureChildControls();

            // Convert input value to string
            string values = Convert.ToString(value);

            // Check whether value is defined
            if (!String.IsNullOrEmpty(values))
            {
                if (ShowItemSelector)
                {
                    // Get report name and item name
                    string[] items = values.Split(';');

                    // Check whether all required items are defined
                    if (items.Length == 2)
                    {
                        // Set report and item values
                        usReports.Value = items[0];
                        usItems.Value = items[1];
                    }
                }
                else
                {
                    usReports.Value = values;
                }

                if (!RequestHelper.IsPostBack())
                {
                    LoadOtherValues();
                }
            }
        }
    }


    /// <summary>
    /// Type of report (graph,table,value).
    /// </summary>
    public ReportItemType ReportType
    {
        get
        {
            return (ReportItemType)GetValue("ReportType", (int)ReportItemType.All);
        }
        set
        {
            SetValue("ReportType", (int)value);
        }
    }


    /// <summary>
    /// Report Id.
    /// </summary>
    public int ReportID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the item uniselector drop down list client id for javascript use.
    /// </summary>
    public string UniSelectorClientID
    {
        get
        {
            EnsureChildControls();
            return usItems.UseUniSelectorAutocomplete ? usItems.AutocompleteValueClientID : usItems.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Indicates weather display report drop down list.
    /// </summary>
    public bool Display
    {
        get
        {
            return mDisplay;
        }
        set
        {
            mDisplay = value;
        }
    }


    /// <summary>
    /// Gets the current data set.
    /// </summary>
    private DataSet CurrentDataSet
    {
        get
        {
            DataSet ds = WindowHelper.GetItem(CurrentGuid()) as DataSet;
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                if (DataHelper.DataSourceIsEmpty(mCurrentDataSet))
                {
                    mCurrentDataSet = LoadFromXML(Convert.ToString(ViewState["ParametersXmlData"]), Convert.ToString(ViewState["ParametersXmlSchema"]));
                }
                ds = mCurrentDataSet;
            }
            return ds;
        }
    }

    #endregion


    #region "Control events"

    protected override void OnLoad(EventArgs e)
    {
        // First item as "please select .." - not default "none"
        usReports.SpecialFields.Add(new SpecialField { Text = "(" + GetString("rep.pleaseselect") + ")", Value = "0" });
        usReports.DropDownSingleSelect.AutoPostBack = true;
        usReports.IsLiveSite = IsLiveSite;
        usReports.OnSelectionChanged += usReports_OnSelectionChanged;

        // Disable 'please select' for items selector
        usItems.MaxDisplayedItems = usItems.MaxDisplayedTotalItems = 1000;
        usItems.IsLiveSite = IsLiveSite;

        BuildReportCondition();

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        string reportName = ValidationHelper.GetString(usReports.Value, String.Empty);
        mReportInfo = ReportInfoProvider.GetReportInfo(reportName);
        if (mReportInfo != null)
        {
            ReportID = mReportInfo.ReportID;

            usItems.Enabled = true;

            // Test if there is any item visible in report parameters
            FormInfo fi = new FormInfo(mReportInfo.ReportParameters);

            // Hide if there are no visible parameters
            pnlParametersButtons.Visible = fi.GetFields(true, false).Any();
        }
        else
        {
            if (ReportID == 0)
            {
                pnlParametersButtons.Visible = false;
                usItems.Enabled = false;
            }
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(String), "ReportItemSelector_Refresh", "function refresh () {" + ControlsHelper.GetPostBackEventReference(pnlUpdate, String.Empty) + "}", true);

        if (!mDisplay)
        {
            pnlReports.Visible = false;
            pnlParametersButtons.Visible = false;
            usItems.Enabled = true;
        }

        if (!ShowItemSelector)
        {
            pnlItems.Visible = false;
        }

        BuildConditions();

        if (mReportInfo == null)
        {
            usItems.SpecialFields.Add(new SpecialField { Text = "(" + mFirstItemText + ")", Value = "0" });
        }

        if (ShowItemSelector)
        {
            ReloadItems();
        }

        var currentGuid = CurrentGuid();

        if (mSetValues)
        {
            WindowHelper.Add(currentGuid, CurrentDataSet);
	        ScriptHelper.RegisterDialogScript(Page);
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "OpenModalWindowReportItem", ScriptHelper.GetScript("modalDialog('" + ResolveUrl("~/CMSModules/Reporting/Dialogs/ReportParametersSelector.aspx?ReportID=" + ReportID + "&guid=" + currentGuid) + "','ReportParametersDialog', 700, 500);"));
            mKeepDataInWindowsHelper = true;
        }

        // Apply reportid condition if report was selected via uniselector
        if (mReportInfo != null)
        {
            DataSet ds = CurrentDataSet;

            ViewState["ParametersXmlData"] = null;
            ViewState["ParametersXmlSchema"] = null;

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                ViewState["ParametersXmlData"] = ds.GetXml();
                ViewState["ParametersXmlSchema"] = ds.GetXmlSchema();
            }

            if (!mKeepDataInWindowsHelper)
            {
                WindowHelper.Remove(currentGuid);
            }

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                ds = DataHelper.DataSetPivot(ds, new [] { "ParameterName", "ParameterValue" });
                ugParameters.DataSource = ds;
                ugParameters.ReloadData();
                pnlParameters.Visible = true;
            }
            else
            {
                pnlParameters.Visible = false;
            }
        }
        else
        {
            pnlParameters.Visible = false;
        }

        if (pnlParameters.Visible || pnlParametersButtons.Visible)
        {
            pnlItems.AddCssClass("form-group");
        }
        else
        {
            pnlItems.RemoveCssClass("form-group");
        }

        base.OnPreRender(e);
    }


    protected void btnSet_Click(object sender, EventArgs e)
    {
        mSetValues = true;
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearData();
    }


    protected void usReports_OnSelectionChanged(object sender, EventArgs ea)
    {
        ClearData();

        // Try to set first item
        if (usItems.DropDownSingleSelect.Items.Count > 0)
        {
            usItems.DropDownSingleSelect.SelectedIndex = 0;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if ((Form != null) && (Form.Data != null))
        {
            // Check if the schema information is available
            IDataContainer data = Form.Data;

            if (data.ContainsColumn("ParametersXmlSchema") && data.ContainsColumn("ParametersXmlData"))
            {
                // Get xml schema and data                    
                string schema = Convert.ToString(Form.GetFieldValue("ParametersXmlSchema"));
                string xml = Convert.ToString(Form.GetFieldValue("ParametersXmlData"));

                LoadFromXML(xml, schema);
            }
        }
    }


    /// <summary>
    /// Returns an array of values of any other fields returned by the control.
    /// </summary>
    /// <remarks>It returns an array where first dimension is attribute name and the second dimension is its value.</remarks>
    public override object[,] GetOtherValues()
    {
        // Get current dataset
        DataSet ds = CurrentDataSet;

        // Set properties names
        object[,] values = new object[2, 2];
        values[0, 0] = "ParametersXmlSchema";
        values[1, 0] = "ParametersXmlData";

        // Check whether dataset is defined
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Set dataset values
            values[0, 1] = ds.GetXmlSchema();
            values[1, 1] = ds.GetXml();
        }

        return values;
    }


    /// <summary>
    /// Loads dataset to windows helper from schema and data xml definitions.
    /// </summary>
    /// <param name="xml">XML data</param>
    /// <param name="schema">XML schema</param>
    protected DataSet LoadFromXML(string xml, string schema)
    {
        // Check whether schema and data are defined
        if (!String.IsNullOrEmpty(schema) && !String.IsNullOrEmpty(xml))
        {
            //Create data set from xml
            DataSet ds = new DataSet();

            // Load schema
            StringReader sr = StringReader.New(schema);
            ds.ReadXmlSchema(sr);

            // Load data
            ds.TryReadXml(xml);

            // Set current dataset
            WindowHelper.Add(CurrentGuid(), ds);

            return ds;
        }

        return null;
    }


    /// <summary>
    /// Builds condition for report selector
    /// </summary>
    private void BuildReportCondition()
    {
        switch (ReportType)
        {
            case ReportItemType.Graph:
                usReports.WhereCondition = "EXISTS (SELECT GraphID FROM Reporting_ReportGraph as graph WHERE (graph.GraphIsHtml IS NULL OR graph.GraphIsHtml = 0) AND graph.GraphReportID = ReportID)";
                break;

            case ReportItemType.HtmlGraph:
                usReports.WhereCondition = "EXISTS (SELECT GraphID FROM Reporting_ReportGraph as graph WHERE (graph.GraphIsHtml = 1) AND graph.GraphReportID = ReportID)";
                break;

            case ReportItemType.Table:
                usReports.WhereCondition = "EXISTS (SELECT TableID FROM Reporting_ReportTable as reporttable WHERE reporttable.TableReportID = ReportID)";
                break;

            case ReportItemType.Value:
                usReports.WhereCondition = "EXISTS (SELECT ValueID FROM Reporting_ReportValue as value WHERE value.ValueReportID = ReportID)";
                break;

            // By default do nothing
        }
    }


    /// <summary>
    /// Builds conditions for particular type of selector.
    /// </summary>
    protected void BuildConditions()
    {
        switch (ReportType)
        {
            case ReportItemType.Graph:
                usItems.WhereCondition = "GraphReportID = " + ReportID + " AND (GraphIsHtml IS NULL OR GraphIsHtml = 0)";
                usItems.DisplayNameFormat = "{%GraphDisplayName%}";
                usItems.ReturnColumnName = UseIDValue ? "GraphID" : "GraphName";
                usItems.ObjectType = "reporting.reportgraph";
                mFirstItemText = GetString("rep.graph.pleaseselect");
                break;

            case ReportItemType.HtmlGraph:
                usItems.WhereCondition = "GraphReportID = " + ReportID + " AND (GraphIsHtml = 1)";
                usItems.DisplayNameFormat = "{%GraphDisplayName%}";
                usItems.ReturnColumnName = UseIDValue ? "GraphID" : "GraphName";
                usItems.ObjectType = "reporting.reportgraph";
                mFirstItemText = GetString("rep.graph.pleaseselect");
                break;

            case ReportItemType.Table:
                usItems.WhereCondition = "TableReportID = " + ReportID;
                usItems.ObjectType = "reporting.reporttable";
                usItems.DisplayNameFormat = "{%TableDisplayName%}";
                usItems.ReturnColumnName = UseIDValue ? "TableID" : "TableName";
                mFirstItemText = GetString("rep.table.pleaseselect");
                break;

            case ReportItemType.Value:
                usItems.WhereCondition = "ValueReportID = " + ReportID;
                usItems.ObjectType = "reporting.reportvalue";
                usItems.DisplayNameFormat = "{%ValueDisplayName%}";
                usItems.ReturnColumnName = UseIDValue ? "ValueID" : "ValueName";
                mFirstItemText = GetString("rep.value.pleaseselect");
                break;

            // By default do nothing
        }
    }


    /// <summary>
    /// Forces reload of the selector with report items.
    /// </summary>
    public void ReloadItems()
    {
        string selected = usItems.DropDownSingleSelect.SelectedValue;

        usItems.Reload(true);

        var item = usItems.DropDownSingleSelect.Items.FindByValue(selected);
        if (item != null)
        {
            item.Selected = true;
        }
    }


    /// <summary>
    /// Gets GUID from hidden field .. if not there create new one.
    /// </summary>
    private String CurrentGuid()
    {
        // For ReloadData (f.e. webpart save) store guid also in request helper, because hidden field is empty after control reload
        Guid guid = ValidationHelper.GetGuid(RequestStockHelper.GetItem("wppreportselector"), Guid.Empty);
        if (hdnGuid.Value == String.Empty)
        {
            hdnGuid.Value = (guid == Guid.Empty) ? Guid.NewGuid().ToString() : guid.ToString();
        }

        if (guid == Guid.Empty)
        {
            RequestStockHelper.Add("wppreportselector", hdnGuid.Value);
        }

        return hdnGuid.Value;
    }


    private void ClearData()
    {
        WindowHelper.Remove(CurrentGuid());
        ViewState["ParametersXmlData"] = null;
        ViewState["ParametersXmlSchema"] = null;
    }

    #endregion
}