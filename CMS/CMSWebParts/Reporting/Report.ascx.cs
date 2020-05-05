using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_Reporting_Report : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the code name of the report, which should be displayed.
    /// </summary>
    public string ReportName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ReportName"), viewReport.ReportName);
        }
        set
        {
            SetValue("ReportName", value);
        }
    }


    /// <summary>
    /// Determines whether to show parameter filter or not.
    /// </summary>
    public bool DisplayFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFilter"), false);
        }
        set
        {
            SetValue("DisplayFilter", value);
        }
    }


    /// <summary>
    /// Gets or sets the XML schema of parameters dataset.
    /// </summary>
    public string ParametersXmlSchema
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ParametersXmlSchema"), String.Empty);
        }
        set
        {
            SetValue("ParametersXmlSchema", value);
        }
    }


    /// <summary>
    /// Gets or sets chart name in format reportname;itemname.
    /// </summary>
    public string ParametersXmlData
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ParametersXmlData"), String.Empty);
        }
        set
        {
            SetValue("ParametersXmlData", value);
        }
    }


    /// <summary>
    /// Indicates whether enable export
    /// </summary>
    public bool EnableExport
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableExport"), true);
        }
        set
        {
            SetValue("EnableExport", value);
        }
    }


    /// <summary>
    /// If true, chart subscription is enabled
    /// </summary>
    public bool EnableSubscription
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableSubscription"), false);
        }
        set
        {
            SetValue("EnableSubscription", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (!StopProcessing)
        {
            SetContext();
            SetupBasicData();

            ReleaseContext();
            Visible = viewReport.Visible;
        }
    }


    /// <summary>
    /// Setup basit settings for view report.
    /// </summary>
    private void SetupBasicData()
    {
        viewReport.DisplayFilter = DisplayFilter;
        viewReport.ReportName = ReportName;
        viewReport.EnableExport = EnableExport;
        viewReport.EnableSubscription = EnableSubscription;
        viewReport.IsLiveSite = PortalContext.ViewMode.IsLiveSite();

        if (!DisplayFilter && !String.IsNullOrEmpty(ParametersXmlData))
        {
            viewReport.LoadDefaultParameters(ParametersXmlData, ParametersXmlSchema);
            viewReport.LoadFormParameters = false;
        }
        viewReport.ReloadData(true);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupBasicData();

        viewReport.ForceLoadDefaultValues = true;
        viewReport.IgnoreWasInit = true;

        viewReport.ReloadData(true);
    }


    protected override void OnPreRender(EventArgs e)
    {
        //Security check
        Visible = viewReport.Visible;
        base.OnPreRender(e);
    }
}