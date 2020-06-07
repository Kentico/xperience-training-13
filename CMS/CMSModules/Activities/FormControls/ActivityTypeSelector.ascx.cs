using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Activities_FormControls_ActivityTypeSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mShowAll = true;
    private SiteInfo mCurrentSite = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Return null if "(all)" item is selected
            if (ValidationHelper.GetInteger(ucType.Value, Int32.MinValue) == UniSelector.US_ALL_RECORDS)
            {
                return null;
            }
            return ucType.Value;
        }
        set
        {
            ucType.Value = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Gets selected value from dropdown list.
    /// </summary>
    public string SelectedValue
    {
        get
        {
            return ValidationHelper.GetString(Value, null);
        }
    }


    /// <summary>
    /// Inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector 
    {
        get
        {
            return ucType;
        }
    }


    /// <summary>
    /// Current site.
    /// </summary>
    /// <remarks>Override that handles situations, where activity types are used in global context.</remarks>
    public override SiteInfo CurrentSite
    {
        get
        {
            if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && mCurrentSite == null)
            {
                var siteId = ValidationHelper.GetInteger(QueryHelper.GetString("siteid", string.Empty), SiteContext.CurrentSiteID);
                mCurrentSite = SiteInfo.Provider.Get(siteId);
            }
            return mCurrentSite ?? base.CurrentSite;
        }
    }


    /// <summary>
    /// If set to true, only custom activities will be shown.
    /// </summary>
    public bool ShowCustomActivitiesOnly
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCustomActivitiesOnly"), false);
        }
        set
        {
            SetValue("ShowCustomActivitiesOnly", value);
        }
    }


    /// <summary>
    /// If set to true, only enabled activities will be shown.
    /// </summary>
    public bool ShowEnabledActivitiesOnly
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEnabledActivitiesOnly"), false);
        }
        set
        {
            SetValue("ShowEnabledActivitiesOnly", value);
        }
    }


    /// <summary>
    /// If set to true, only manually creatable activities are shown.
    /// </summary>
    public bool ShowManuallyCreatableActivities
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowManuallyCreatableActivities"), false);
        }
        set
        {
            SetValue("ShowManuallyCreatableActivities", value);
        }
    }


    /// <summary>
    /// If set to true, "(all)" item will be included in the list.
    /// </summary>
    public bool ShowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAll"), mShowAll);
        }
        set
        {
            SetValue("ShowAll", value);
        }
    }


    /// <summary>
    /// Gets or sets AutoPostBack property of internal dropdown list.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return ucType.DropDownSingleSelect.AutoPostBack;
        }
        set
        {
            ucType.DropDownSingleSelect.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Additional CSS class for drop down list control.
    /// </summary>
    public String AdditionalDropDownCSSClass
    {
        get
        {
            return ucType.AdditionalDropDownCSSClass;
        }
        set
        {
            ucType.AdditionalDropDownCSSClass = value;
        }
    }

    #endregion


    #region "Events"

    public event EventHandler OnSelectedIndexChanged;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            ucType.Visible = false;
            return;
        }

        if (!String.IsNullOrEmpty(CssClass))
        {
            ucType.AdditionalDropDownCSSClass = CssClass;
        }
        ucType.AllowAll = ShowAll;

        var condition = new WhereCondition();        
        if (ShowCustomActivitiesOnly)
        {
            condition.WhereTrue("ActivityTypeIsCustom");
        }
        if (ShowManuallyCreatableActivities)
        {
            condition.WhereTrue("ActivityTypeManualCreationAllowed");
        }
        if (ShowEnabledActivitiesOnly)
        {
            condition.WhereTrue("ActivityTypeEnabled");
        }

        ucType.WhereCondition = condition.ToString(true);
        ucType.OnSelectionChanged += new EventHandler(DropDownSingleSelect_SelectedIndexChanged);
    }


    protected void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (OnSelectedIndexChanged != null)
        {
            OnSelectedIndexChanged(this, EventArgs.Empty);
        }
    }

    #endregion
}