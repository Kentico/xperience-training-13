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

public partial class CMSWebParts_Reporting_Chart : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets chart name in format reportname;itemname.
    /// </summary>
    public string ChartName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ReportChart"), String.Empty);
        }
        set
        {
            SetValue("ReportChart", value);
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
    /// Gets or sets width of graph.
    /// </summary>
    public String Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Width"), String.Empty);
        }
        set
        {
            SetValue("Width", value);
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


    /// <summary>
    /// Gets or sets height of graph.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 0);
        }
        set
        {
            SetValue("Height", value);
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


    /// <summary>
    /// Graph possible width of control.
    /// </summary>
    public int AreaMaxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["AreaMaxWidth"], 0);
        }
        set
        {
            ViewState["AreaMaxWidth"] = value;
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
            string[] items = ChartName.Split(';');
            if ((items != null) && (items.Length == 2))
            {
                ucGraph.Parameter = items[0] + "." + items[1];
                ucGraph.ReportItemName = items[0] + ";" + items[1];
                ucGraph.CacheItemName = CacheItemName;
                ucGraph.CacheMinutes = CacheMinutes;
                ucGraph.CacheDependencies = CacheDependencies;
                ucGraph.Width = Width;
                ucGraph.Height = Height;
                ucGraph.ItemType = ReportItemType.Graph;
                ucGraph.LoadDefaultParameters(ParametersXmlData, ParametersXmlSchema);
                ucGraph.RangeInterval = RangeInterval;
                ucGraph.RangeValue = RangeValue;
                ucGraph.EnableExport = EnableExport;
                ucGraph.EnableSubscription = EnableSubscription;
                ucGraph.IsLiveSite = PortalContext.ViewMode.IsLiveSite();
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
        Visible = ucGraph.Visible;

        if (AreaMaxWidth != 0)
        {
            ucGraph.ComputedWidth = AreaMaxWidth;
        }

        ucGraph.ReloadData(true);

        if (ucGraph.ComputedWidth != 0)
        {
            AreaMaxWidth = ucGraph.ComputedWidth;
        }

        base.OnPreRender(e);
    }

    #endregion
}