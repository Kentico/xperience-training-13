using System;

using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_WebAnalytics_Controls_ConversionReportHeader : CMSAdminControl
{
    /// <summary>
    /// Selected conversion code name
    /// </summary>
    public object SelectedConversion
    {
        get
        {
            return usSelectConversion.Value;
        }
        set
        {
            usSelectConversion.Value = value;
        }
    }


    /// <summary>
    /// Value for all record item
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            return usSelectConversion.AllRecordValue;
        }
    }


    /// <summary>
    /// Indicates whether show conversion selector
    /// </summary>
    public bool ShowConversionSelector
    {
        get
        {
            return pnlConversion.Visible;
        }
        set
        {
            pnlConversion.Visible = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Filter conversions only if not all sites selected
        usSelectConversion.WhereCondition = SqlHelper.AddWhereCondition(usSelectConversion.WhereCondition, "ConversionSiteID =" + SiteContext.CurrentSiteID);

        usSelectConversion.ReloadData(true);
    }
}