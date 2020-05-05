using System;
using System.Data;
using System.Web.UI;

using CMS.Core;
using CMS.Helpers;
using CMS.Reporting;
using CMS.Reporting.Web.UI;


public partial class CMSModules_Reporting_Controls_ReportValue : AbstractReportControl
{
    #region "Variables"

    private ReportValueInfo mValueInfo;
    private string mParameter = string.Empty;
    private ReportInfo ri;

    #endregion


    #region "Properties"

    /// <summary>
    /// Value info for direct input (no load from DB).
    /// </summary>
    public ReportValueInfo ValueInfo
    {
        get
        {
            return mValueInfo ?? (mValueInfo = ReportValueInfoProvider.GetReportValueInfo(Parameter));
        }
        set
        {
            mValueInfo = value;
        }
    }


    /// <summary>
    /// Report value connection string
    /// </summary>
    public override string ConnectionString
    {
        get
        {
            String valueConn = (ValueInfo == null) ? String.Empty : ValueInfo.ValueConnectionString;
            if (String.IsNullOrEmpty(valueConn))
            {
                return (ri == null) ? String.Empty : ri.ReportConnectionString;
            }

            return valueConn;
        }
    }


    /// <summary>
    /// Value name - prevent using viewstate  (problems with displayreportcontrol and postback).
    /// </summary>
    public override string Parameter
    {
        get
        {
            return mParameter;
        }
        set
        {
            mParameter = value;
        }
    }

    #endregion


    #region "Control events"

    protected override void OnLoad(EventArgs e)
    {
        ItemType = ReportItemType.Value;
        base.OnLoad(e);
    }


    /// <summary>
    /// OnPreRender handler - register progress script.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (ri != null)
        {
            // Disable export context menu for report value
            EnableExport = false;

            if (ValueInfo != null)
            {
                menuCont.RenderAsTag = HtmlTextWriterTag.Span;
                EnableSubscription = (EnableSubscription && ValidationHelper.GetBoolean(ValueInfo.ValueSettings["SubscriptionEnabled"], true) && ri.ReportEnableSubscription);
                RegisterSubscriptionScript(ValueInfo.ValueReportID, "valueid", ValueInfo.ValueID, menuCont);
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        // Load value info object
        ReportValueInfo rvi = ValueInfo;
        if (rvi == null)
        {
            return;
        }

        ri = ReportInfoProvider.GetReportInfo(rvi.ValueReportID);
        if (ri == null)
        {
            return;
        }

        // Check security settings
        if (!(CheckReportAccess(ri) && CheckEmailModeSubscription(ri, ValidationHelper.GetBoolean(ValueInfo.ValueSettings["SubscriptionEnabled"], true))))
        {
            Visible = false;
            return;
        }

        // Prepare query attributes
        QueryIsStoredProcedure = rvi.ValueQueryIsStoredProcedure;
        QueryText = rvi.ValueQuery;

        // Init parameters
        InitParameters(ri.ReportParameters);

        // Init macro resolver
        InitResolver();

        DataSet ds = null;

        // Ensure report item name for caching
        if (String.IsNullOrEmpty(ReportItemName))
        {
            ReportItemName = String.Format("{0};{1}", ri.ReportName, rvi.ValueName);
        }

        try
        {
            // Load data
            ds = LoadData();
        }
        catch (Exception ex)
        {
            // Display error message, if data load fail
            lblError.Visible = true;
            lblError.Text = "Error loading the data: " + ex.Message;
            Service.Resolve<IEventLogService>().LogException("Report value", "E", ex);
        }

        string value;

        // If data source or result is empty, send an empty dataset
        if (DataHelper.DataSourceIsEmpty(ds) || string.IsNullOrEmpty(value = GetReportValue(ds, rvi)))
        {
            if (EmailMode && SendOnlyNonEmptyDataSource)
            {
                Visible = false;
            }

            return;
        }

        if (EmailMode)
        {
            ltlEmail.Text = HTMLHelper.HTMLEncode(ResolveMacros(value));
            ltlEmail.Visible = true;
            menuCont.Visible = false;
        }
        else
        {
            lblValue.Text = HTMLHelper.HTMLEncode(ResolveMacros(value));
        }

    }


    /// <summary>
    /// Returns true if graph belongs to report.
    /// </summary>
    /// <param name="report">Report to validate</param>
    public override bool IsValid(ReportInfo report)
    {
        ReportValueInfo rvi = ValueInfo;

        if ((report != null) && (rvi != null) && (report.ReportID == rvi.ValueReportID))
        {
            return true;
        }

        return false;
    }


    private string GetReportValue(DataSet ds, ReportValueInfo rvi)
    {
        var value = "";
        var row = ds.Tables[0].Rows[0];

        if (string.IsNullOrEmpty(rvi.ValueFormatString))
        {
            value = ValidationHelper.GetString(row[0], string.Empty);
        }
        else
        {
            try
            {
                value = string.Format(rvi.ValueFormatString, row.ItemArray);
            }
            catch (FormatException ex)
            {
                lblError.Visible = true;
                lblError.Text = "Invalid format string: " + ex.Message;
                Service.Resolve<IEventLogService>().LogException("Report value", "E", ex);
            }
        }

        return value;
    }

    #endregion
}