using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_Syndication_Basic_XMLRepeater : CMSAbstractWebPart
{
    #region "Variables"

    private CMSBaseDataSource mDataSourceControl = null;

    #endregion


    #region "XML Repeater properties"

    /// <summary>
    /// Custom feed header XML which is generated before feed items. If the value is empty default header for Atom feed is generated.
    /// </summary>
    public string HeaderXML
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderXML"), null);
        }
        set
        {
            SetValue("HeaderXML", value);
            xmlRepeater.HeaderXML = value;
        }
    }


    /// <summary>
    /// Custom feed footer XML which is generated after feed items. If the value is empty default footer for Atom feed is generated.
    /// </summary>
    public string FooterXML
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FooterXML"), null);
        }
        set
        {
            SetValue("FooterXML", value);
            xmlRepeater.FooterXML = value;
        }
    }

    #endregion


    #region "Datasource properties"

    /// <summary>
    /// Gets or sets name of source.
    /// </summary>
    public string DataSourceName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataSourceName"), string.Empty);
        }
        set
        {
            SetValue("DataSourceName", value);
            xmlRepeater.DataSourceName = value;
        }
    }


    /// <summary>
    /// Control with data source.
    /// </summary>
    public CMSBaseDataSource DataSourceControl
    {
        get
        {
            return mDataSourceControl;
        }
        set
        {
            mDataSourceControl = value;
            xmlRepeater.DataSourceControl = value;
        }
    }

    #endregion


    #region "Cache properties"

    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return ValidationHelper.GetString(base.CacheItemName, xmlRepeater.CacheItemName);
        }
        set
        {
            base.CacheItemName = value;
            xmlRepeater.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, xmlRepeater.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            xmlRepeater.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            xmlRepeater.CacheMinutes = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), string.Empty);
        }
        set
        {
            SetValue("TransformationName", value);
            xmlRepeater.TransformationName = value;
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            xmlRepeater.StopProcessing = value;
        }
    }

    #endregion


    #region "Overidden methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion


    #region "Setup control"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        var vm = PortalContext.ViewMode;

        if (StopProcessing || PortalContext.IsDesignMode(vm) || vm.IsOneOf(ViewModeEnum.Edit, ViewModeEnum.EditDisabled))
        {
            xmlRepeater.StopProcessing = true;
        }
        else
        {
            // XML feed properties
            xmlRepeater.HeaderXML = HeaderXML;
            xmlRepeater.FooterXML = FooterXML;

            // Cache properties
            xmlRepeater.CacheItemName = CacheItemName;
            xmlRepeater.CacheDependencies = CacheDependencies;
            xmlRepeater.CacheMinutes = CacheMinutes;

            // Transformation properties
            xmlRepeater.TransformationName = TransformationName;

            // Datasource properties
            xmlRepeater.DataSourceName = DataSourceName;
        }
    }

    #endregion
}