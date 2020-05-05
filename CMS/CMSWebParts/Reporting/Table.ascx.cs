using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.Reporting;
using CMS.WebAnalytics;
using CMS.PortalEngine;

public partial class CMSWebParts_Reporting_Table : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets table name.
    /// </summary>
    public string TableName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ReportTable"), "");
        }
        set
        {
            SetValue("ReportTable", value);
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
    /// Enables/disables paging for tables
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), false);
        }
        set
        {
            SetValue("EnablePaging", value);
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
    /// Page size for paged tables
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 0);
        }
        set
        {
            SetValue("PageSize", value);
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
    /// Interval of time range.
    /// </summary>
    public HitsIntervalEnum RangeInterval
    {
        get
        {
            return HitsIntervalEnumFunctions.StringToHitsConversion(ValidationHelper.GetString(GetValue("Range"), "none"));
        }
        set
        {
            SetValue("Range", HitsIntervalEnumFunctions.HitsConversionToString(value));
        }
    }


    /// <summary>
    /// Value of range interval.
    /// </summary>
    public int RangeValue
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RangeValue"), 0);
        }
        set
        {
            SetValue("RangeValue", value);
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
            string[] items = TableName.Split(';');
            if ((items != null) && (items.Length == 2))
            {
                ucTable.Parameter = items[0] + "." + items[1];
                ucTable.ReportItemName = items[0] + ";" + items[1];
                ucTable.CacheItemName = CacheItemName;
                ucTable.CacheMinutes = CacheMinutes;
                ucTable.CacheDependencies = CacheDependencies;
                ucTable.ItemType = ReportItemType.Graph;
                ucTable.LoadDefaultParameters(ParametersXmlData, ParametersXmlSchema);
                ucTable.RangeInterval = RangeInterval;
                ucTable.RangeValue = RangeValue;
                ucTable.EnableExport = EnableExport;
                ucTable.EnablePaging = EnablePaging;
                ucTable.PageSize = PageSize;
                ucTable.IsLiveSite = PortalContext.ViewMode.IsLiveSite();
                ucTable.EnableSubscription = EnableSubscription;
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

        ucTable.ReloadData(true);
    }


    /// <summary>
    /// Prerenders the control.
    /// </summary>    
    protected override void OnPreRender(EventArgs e)
    {
        if (!StopProcessing)
        {
            ucTable.ReloadData(true);
        }

        // Set visibility of current webpart with dependence on graph visibility
        Visible = ucTable.Visible;

        base.OnPreRender(e);
    }

    #endregion
}