using System;

using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Navigation_cmsbreadcrumbs : CMSAbstractWebPart
{
    #region "Document properties"

    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("CacheMinutes"), bcElem.CacheMinutes);
        }
        set
        {
            SetValue("CacheMinutes", value);
            bcElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), bcElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected documents are combined with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), bcElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            bcElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), bcElem.CultureCode), bcElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            bcElem.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the nodes alias path.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), bcElem.Path);
        }
        set
        {
            SetValue("Path", value);
            bcElem.Path = MacroResolver.ResolveCurrentPath(value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected documents must be published.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), bcElem.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            bcElem.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), bcElem.SiteName), bcElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            bcElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the link name should be encoded.
    /// </summary>
    public bool EncodeName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EncodeName"), bcElem.EncodeName);
        }
        set
        {
            SetValue("EncodeName", value);
            bcElem.EncodeName = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), bcElem.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            bcElem.FilterName = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), bcElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            bcElem.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether breadcrumbs is rendered with rtl direction for specific languages.
    /// </summary>
    public bool UseRtlBehaviour
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseRtlBehaviour"), bcElem.UseRtlBehaviour);
        }
        set
        {
            SetValue("UseRtlBehaviour", value);
            bcElem.UseRtlBehaviour = value;
        }
    }


    /// <summary>
    /// Gets or sets the breadcrumb separator.
    /// </summary>
    public string BreadcrumbSeparator
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("BreadCrumbSeparator"), bcElem.BreadCrumbSeparator), bcElem.BreadCrumbSeparator);
        }
        set
        {
            SetValue("BreadCrumbSeparator", value);
            bcElem.BreadCrumbSeparator = value;
        }
    }


    /// <summary>
    /// Gets or sets the default path.
    /// </summary>
    public string DefaultPath
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("DefaultPath"), bcElem.DefaultPath), bcElem.DefaultPath);
        }
        set
        {
            SetValue("DefaultPath", value);
            if (!String.IsNullOrEmpty(value))
            {
                bcElem.DefaultPath = MacroResolver.ResolveCurrentPath(value);
            }
            else
            {
                bcElem.DefaultPath = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the selected nodes starting path.
    /// </summary>
    public string SelectNodesStartPath
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SelectNodesStartPath"), bcElem.StartingPath), bcElem.StartingPath);
        }
        set
        {
            SetValue("SelectNodesStartPath", value);
            if (!String.IsNullOrEmpty(value))
            {
                bcElem.StartingPath = MacroResolver.ResolveCurrentPath(value);
            }
            else
            {
                bcElem.StartingPath = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether current item is displayed.
    /// </summary>
    public bool ShowCurrentItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCurrentItem"), bcElem.ShowCurrentItem);
        }
        set
        {
            SetValue("ShowCurrentItem", value);
            bcElem.ShowCurrentItem = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether current item is link.
    /// </summary>
    public bool ShowCurrentItemAsLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCurrentItemAsLink"), bcElem.ShowCurrentItemAsLink);
        }
        set
        {
            SetValue("ShowCurrentItemAsLink", value);
            bcElem.ShowCurrentItemAsLink = value;
        }
    }


    /// <summary>
    /// Gets or sets the URL target.
    /// </summary>
    public string UrlTarget
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("UrlTarget"), bcElem.UrlTarget), bcElem.UrlTarget);
        }
        set
        {
            SetValue("UrlTarget", value);
            bcElem.UrlTarget = value;
        }
    }


    /// <summary>
    /// Gets or sets the breadcrumb separator for RTL culture.
    /// </summary>
    public string BreadcrumbSeparatorRTL
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("BreadCrumbSeparatorRTL"), bcElem.BreadCrumbSeparatorRTL), bcElem.BreadCrumbSeparatorRTL);
        }
        set
        {
            SetValue("BreadCrumbSeparatorRTL", value);
            bcElem.BreadCrumbSeparatorRTL = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), bcElem.ClassNames), bcElem.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            bcElem.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether document menu item properties are applied.
    /// </summary>
    public bool ApplyMenuDesign
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ApplyMenuDesign"), bcElem.ApplyMenuDesign);
        }
        set
        {
            SetValue("ApplyMenuDesign", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether link title is rendered.
    /// </summary>
    public bool RenderLinkTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderLinkTitle"), bcElem.RenderLinkTitle);
        }
        set
        {
            SetValue("RenderLinkTitle", value);
            bcElem.RenderLinkTitle = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the document value ShowInNavigation is ignored.
    /// </summary>
    public bool IgnoreShowInNavigation
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IgnoreShowInNavigation"), bcElem.IgnoreShowInNavigation);
        }
        set
        {
            SetValue("IgnoreShowInNavigation", value);
            bcElem.IgnoreShowInNavigation = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), bcElem.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            bcElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows results.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), bcElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            bcElem.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to be retrieved from database.
    /// </summary>  
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), bcElem.Columns);
        }
        set
        {
            SetValue("Columns", value);
            bcElem.Columns = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            bcElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item dependencies.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return base.CacheDependencies;
        }
        set
        {
            base.CacheDependencies = value;
            bcElem.CacheDependencies = value;
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
        if (StopProcessing)
        {
            bcElem.StopProcessing = true;
        }
        else
        {
            bcElem.ControlContext = ControlContext;

            // Document properties
            bcElem.CacheItemName = CacheItemName;
            bcElem.CacheDependencies = CacheDependencies;
            bcElem.CacheMinutes = CacheMinutes;
            bcElem.CheckPermissions = CheckPermissions;
            bcElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            bcElem.CultureCode = CultureCode;
            bcElem.Path = Path;
            bcElem.SelectOnlyPublished = SelectOnlyPublished;
            bcElem.SiteName = SiteName;

            bcElem.BreadCrumbSeparator = BreadcrumbSeparator;
            bcElem.DefaultPath = DefaultPath;
            bcElem.StartingPath = SelectNodesStartPath;
            bcElem.ShowCurrentItem = ShowCurrentItem;
            bcElem.ShowCurrentItemAsLink = ShowCurrentItemAsLink;
            bcElem.UrlTarget = UrlTarget;
            bcElem.RenderLinkTitle = RenderLinkTitle;
            bcElem.IgnoreShowInNavigation = IgnoreShowInNavigation;

            bcElem.UseRtlBehaviour = UseRtlBehaviour;
            bcElem.BreadCrumbSeparatorRTL = BreadcrumbSeparatorRTL;
            bcElem.ClassNames = ClassNames;
            bcElem.ApplyMenuDesign = ApplyMenuDesign;
            bcElem.WhereCondition = WhereCondition;
            bcElem.FilterName = FilterName;

            bcElem.EncodeName = EncodeName;

            bcElem.HideControlForZeroRows = HideControlForZeroRows;
            bcElem.ZeroRowsText = ZeroRowsText;

            bcElem.Columns = Columns;
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = bcElem.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(bcElem.DataSource) && bcElem.HideControlForZeroRows)
        {
            Visible = false;
        }
    }
}