using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.Reporting;
using CMS.PortalEngine;

public partial class CMSWebParts_Reporting_Value : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Value set by report name and value.
    /// </summary>
    public string ReportValue
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ReportValue"), String.Empty);
        }
        set
        {
            SetValue("ReportValue", value);
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

    #endregion


    #region "Methods"

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
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            string[] items = ReportValue.Split(';');
            if ((items != null) && (items.Length == 2))
            {
                ucValue.Parameter = items[0] + "." + items[1];
                ucValue.ReportItemName = items[0] + ";" + items[1];
                ucValue.CacheItemName = CacheItemName;
                ucValue.CacheMinutes = CacheMinutes;
                ucValue.CacheDependencies = CacheDependencies;
                ucValue.ItemType = ReportItemType.Graph;
                ucValue.LoadDefaultParameters(ParametersXmlData, ParametersXmlSchema);
                ucValue.EnableSubscription = EnableSubscription;
                ucValue.IsLiveSite = PortalContext.ViewMode.IsLiveSite(); ;

                ucValue.ReloadData(true);
            }
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    /// <summary>
    /// OnPreRender override - set visibility.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Set visibility of current webpart with dependence on graph visibility
        Visible = ucValue.Visible;

        base.OnPreRender(e);
    }

    #endregion
}