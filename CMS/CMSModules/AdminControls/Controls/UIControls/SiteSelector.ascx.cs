using System;
using System.Web.UI;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_SiteSelector : CMSAbstractUIWebpart
{
    #region "Variables"

    private int siteId;

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, drop down contains '(global)'
    /// </summary>
    public bool AllowGlobal
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowGlobal"), false);
        }
        set
        {
            SetValue("AllowGlobal", value);
        }
    }


    /// <summary>
    /// If true, drop down contains '(all)'
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowAll"), false);
        }
        set
        {
            SetValue("AllowAll", value);
        }
    }


    /// <summary>
    /// Site selecotor type ('siteorglobal','classic')
    /// </summary>
    public SiteSelectorTypeEnum SelectorType
    {
        get
        {
            String value = ValidationHelper.GetString(GetValue("SelectorType"), String.Empty);
            if (String.IsNullOrEmpty(value))
            {
                return EnumHelper.GetDefaultValue<SiteSelectorTypeEnum>();
            }

            return EnumStringRepresentationExtensions.ToEnum<SiteSelectorTypeEnum>(value);
        }
        set
        {
            SetValue("SelectorType", value.ToStringRepresentation());
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Set visibility
        if (SelectorType == SiteSelectorTypeEnum.SiteOrGlobal)
        {
            SiteSelector.StopProcessing = true;
            SiteOrGlobal.Visible = true;

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(SiteOrGlobal.Selector);

            if (!RequestHelper.IsPostBack())
            {
                siteId = ValidationHelper.GetInteger(UIContext["SiteID"], 0);

                if (siteId != 0)
                {
                    SiteOrGlobal.Reload();
                    SiteOrGlobal.Value = siteId;
                }
            }
        }
        else
        {
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(SiteSelector);

            SiteSelector.Visible = true;
            SiteOrGlobal.StopProcessing = true;

            SiteSelector.DropDownSingleSelect.AutoPostBack = true;

            SiteSelector.AllowGlobal = AllowGlobal;
            SiteSelector.AllowAll = AllowAll;

            if (!RequestHelper.IsPostBack())
            {
                siteId = ValidationHelper.GetInteger(UIContext["SiteID"], 0);

                if (siteId != 0)
                {
                    SiteSelector.Value = siteId;
                }
                else
                {
                    // If site id is not set, select 'all' (if present). Otherwise select current site.
                    SiteSelector.Value = (AllowAll) ? UniSelector.US_ALL_RECORDS.ToString() : SiteContext.CurrentSiteID.ToString();
                }

                // Reload for first time selection
                SiteSelector.Reload(false);
            }
            else if (GetValue("Visible").ToBoolean(false))
            {
                UIContext["SiteID"] = SiteSelector.Value;
            }
        }

        // Register event
        UIContext.OnGetValue += Current_OnGetValue;

        base.OnInit(e);
    }

    
    void Current_OnGetValue(object sender, UIContextEventArgs e)
    {
        if (e.ColumnName.EqualsCSafe("siteid", true))
        {
            siteId = (SelectorType == SiteSelectorTypeEnum.Classic) ? SiteSelector.SiteID : SiteOrGlobal.SiteID;

            if (Visible && (siteId != 0))
            {
                e.Result = siteId;
            }
        }
    }

    #endregion


    #region "Site selector type enum"

    /// <summary>
    /// Enum for selector type
    /// </summary>
    public enum SiteSelectorTypeEnum
    {
        /// <summary>
        /// Classic mode (all sites)
        /// </summary>
        [EnumDefaultValue]
        [EnumStringRepresentation("classic")]
        Classic,

        /// <summary>
        /// Global, current site, or both
        /// </summary>
        [EnumStringRepresentation("siteorglobal")]
        SiteOrGlobal
    }

    #endregion
}