using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Community;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_FormControls_CommunityGroupSelector : FormEngineUserControl, IDataUserControl
{
    #region "Private variables"

    private bool mDisplayCurrentGroup = true;
    private bool mDisplayNoneWhenEmpty = false;
    private bool mDisplayNoneValue = false;
    private bool mUseDisplayNames = false;
    private string mWhereCondition = "";
    private string mNoneValueText = "";

    private bool specialFieldsAdded = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the current site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["SiteID"], SiteContext.CurrentSiteID);
        }
        set
        {
            ViewState["SiteID"] = value;
        }
    }


    /// <summary>
    /// ID of the currently selected group.
    /// </summary>
    public int GroupID
    {
        get
        {
            return ValidationHelper.GetInteger(usGroups.Value, 0);
        }
        set
        {
            usGroups.Value = value.ToString();
        }
    }


    /// <summary>
    /// Indicates if '(global)' value should be displayed.
    /// </summary>
    public bool DisplayGlobalValue
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DisplayGlobalValue"], false);
        }
        set
        {
            ViewState["DisplayGlobalValue"] = value;
        }
    }


    /// <summary>
    /// Gets or sets text of 'none' item in the selector.
    /// </summary>
    public string NoneValueText
    {
        get
        {
            if (mNoneValueText == "")
            {
                mNoneValueText = GetString("general.empty");
            }
            return mNoneValueText;
        }
        set
        {
            mNoneValueText = value;
        }
    }


    /// <summary>
    /// Gets or sets text of global item in the selector.
    /// </summary>
    public string GlobalValueText
    {
        get
        {
            if (string.IsNullOrEmpty(ValidationHelper.GetString(GetValue("GlobalValueText"), String.Empty)))
            {
                SetValue("GlobalValueText", GetString("general.global"));
            }

            return ValidationHelper.GetString(GetValue("GlobalValueText"), String.Empty);
        }
        set
        {
            SetValue("GlobalValueText", value);
        }
    }


    /// <summary>
    /// Indicates whether show display or code names of groups.
    /// </summary>
    public bool UseDisplayNames
    {
        get
        {
            return mUseDisplayNames;
        }
        set
        {
            mUseDisplayNames = value;
        }
    }


    /// <summary>
    /// Gets or sets if groups drop-down list should be enabled.
    /// </summary>
    public bool EnabledGroups
    {
        get
        {
            return usGroups.Enabled;
        }
        set
        {
            usGroups.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates whether the '(none)' value should be displayed.
    /// </summary>
    public bool DisplayNoneValue
    {
        get
        {
            return mDisplayNoneValue;
        }
        set
        {
            mDisplayNoneValue = value;
        }
    }


    /// <summary>
    /// Indicates whether 'none' item should be added automatically when no groups exist for selector.
    /// </summary>
    public bool DisplayNoneWhenEmpty
    {
        get
        {
            return mDisplayNoneWhenEmpty;
        }
        set
        {
            mDisplayNoneWhenEmpty = value;
        }
    }


    /// <summary>
    /// Gets or sets WHERE condition to filter data.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();
            base.Enabled = value;
            usGroups.Enabled = value;
        }
    }

    /// <summary>
    /// Gets the currene UniSelector instance.
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            EnsureChildControls();
            return usGroups;
        }
    }


    /// <summary>
    /// Group code name.
    /// </summary>
    public string GroupCodeName
    {
        get
        {
            EnsureChildControls();
            return Convert.ToString(usGroups.Value);
        }
        set
        {
            EnsureChildControls();
            usGroups.Value = value;
        }
    }


    /// <summary>
    /// Gets the current drop down control.
    /// </summary>
    public CMSDropDownList CurrentDropDown
    {
        get
        {
            EnsureChildControls();
            return usGroups.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets value name - default groupname.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return Convert.ToString(usGroups.Value);
        }
        set
        {
            EnsureChildControls();
            usGroups.Value = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the CMSDropDownList with stylesheets.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            EnsureChildControls();
            return usGroups.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets if live iste property.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            EnsureChildControls();
            return ValidationHelper.GetBoolean(ViewState["IsLiveSite"], true);
        }
        set
        {
            EnsureChildControls();
            ViewState["IsLiveSite"] = value;
            usGroups.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates whether the current group option should be added to the list.
    /// </summary>
    public bool DisplayCurrentGroup
    {
        get
        {
            return mDisplayCurrentGroup;
        }
        set
        {
            mDisplayCurrentGroup = value;
        }
    }

    #endregion


    #region "SetValue/GetValue overrides"

    /// <summary>
    /// Sets the property value of the control, setting the value affects only local property value.
    /// </summary>
    /// <param name="propertyName">Property name to set</param>
    /// <param name="value">New property value</param>
    public override bool SetValue(string propertyName, object value)
    {
        switch (propertyName.ToLowerCSafe())
        {
            case "groupid":
                EnsureChildControls();
                usGroups.Value = value;
                break;
            case "reloaddata":
                ReloadData();
                break;
            case "enabledgroups":
                EnabledGroups = ValidationHelper.GetBoolean(value, false);
                break;
            case "usedisplaynames":
                UseDisplayNames = ValidationHelper.GetBoolean(value, false);
                break;
            case "wherecondition":
                WhereCondition = ValidationHelper.GetString(value, "");
                break;
            case "displaynonevalue":
                DisplayNoneValue = ValidationHelper.GetBoolean(value, false);
                break;
            case "nonevaluetext":
                NoneValueText = ValidationHelper.GetString(value, "");
                break;
            case "displaynonewhenempty":
                DisplayNoneWhenEmpty = ValidationHelper.GetBoolean(value, false);
                break;
            case "displayglobalvalue":
                DisplayGlobalValue = ValidationHelper.GetBoolean(value, false);
                break;
            case "displaycurrentgroup":
                DisplayCurrentGroup = ValidationHelper.GetBoolean(value, false);
                break;
            case "siteid":
                SiteID = ValidationHelper.GetInteger(value, 0);
                break;
        }

        return base.SetValue(propertyName, value);
    }


    /// <summary>
    /// Returns the value of the given webpart property property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        switch (propertyName.ToLowerCSafe())
        {
            case "groupid":
                return ValidationHelper.GetInteger(usGroups.Value, 0);
            case "enabledgroups":
                return EnabledGroups;
            case "usedisplaynames":
                return UseDisplayNames;
            case "wherecondition":
                return WhereCondition;
            case "displaynonevalue":
                return DisplayNoneValue;
            case "nonevaluetext":
                return NoneValueText;
            case "displaynonewhenempty":
                return DisplayNoneWhenEmpty;
            case "displayglobalvalue":
                return DisplayGlobalValue;
            case "currentselector":
                return CurrentSelector;
            case "currentdropdown":
                return CurrentDropDown;
            case "displaycurrentgroup":
                return DisplayCurrentGroup;
            case "siteid":
                return SiteID;
        }

        return base.GetValue(propertyName);
    }

    #endregion


    #region "Other methods/events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetValue("CurrentSelector", CurrentSelector);
    }


    protected override void OnPreRender(EventArgs e)
    {
        EnsureSpecialFields();
        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Return GroupID if field type is integer
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            usGroups.ReturnColumnName = "GroupID";
            usGroups.AllowEmpty = true;
            DisplayCurrentGroup = DisplayNoneValue = DisplayNoneWhenEmpty = false;
        }

        if (StopProcessing)
        {
            usGroups.StopProcessing = true;
        }
        else
        {
            EnsureChildControls();

            // Ensure correct livesite setting for uniselector
            IsLiveSite = IsLiveSite;

            usGroups.WhereCondition = BuildWhereCondition();
            usGroups.OnSelectionChanged += new EventHandler(usGroups_OnSelectionChanged);
        }
    }


    private void usGroups_OnSelectionChanged(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load updat panel container
        if (usGroups == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }


    /// <summary>
    /// Reloads groups for the specified site.
    /// </summary>
    /// <param name="forceLoad">Indicates whether the load should be forced</param>
    public void ReloadData(bool forceLoad)
    {
        ReloadData();
    }


    /// <summary>
    /// Reloads groups for the specified site.
    /// </summary>
    public void ReloadData()
    {
        usGroups.DropDownSingleSelect.ClearSelection();
        usGroups.DropDownSingleSelect.Items.Clear();

        EnsureSpecialFields();
        usGroups.WhereCondition = BuildWhereCondition();
        usGroups.Reload(true);

        if (!RequestHelper.IsPostBack())
        {
            RaiseOnChanged();
        }
    }


    /// <summary>
    /// Builds wehre condition.
    /// </summary>
    public string BuildWhereCondition()
    {
        string where = string.Empty;
        string siteName = null;

        if (SiteID > 0)
        {
            where = "GroupSiteID=" + SiteID + " AND GroupApproved = 1";
            siteName = SiteInfoProvider.GetSiteName(SiteID);
        }

        // If the user is global admin display all the site groups otherwise display just user's groups
        if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Groups", "read", siteName))
        {
            if (where != null)
            {
                if (!string.IsNullOrEmpty(where))
                {
                    where += " AND ";
                }
                where += "(GroupID IN (SELECT MemberGroupID FROM Community_GroupMember WHERE MemberUserID=" +
                         MembershipContext.AuthenticatedUser.UserID + "))";
            }
        }

        // Include custom WHERE condition if specified
        if (!string.IsNullOrEmpty(WhereCondition))
        {
            if (!string.IsNullOrEmpty(where))
            {
                where += " AND ";
            }
            where += "(" + WhereCondition + ")";
        }

        return where;
    }


    /// <summary>
    /// Adds special fields to selector.
    /// </summary>
    private void EnsureSpecialFields()
    {
        if (!specialFieldsAdded)
        {
            // Add "current group" option
            if (DisplayCurrentGroup)
            {
                usGroups.SpecialFields.Add(new SpecialField() { Text = GetString("group.current"), Value = GroupInfoProvider.CURRENT_GROUP });
            }

            // Add default item
            if (DisplayGlobalValue)
            {
                usGroups.SpecialFields.Add(new SpecialField() { Text = GlobalValueText, Value = "0" });
            }

            // Add none item if required
            if (DisplayNoneValue || (DisplayNoneWhenEmpty && !usGroups.HasData))
            {
                usGroups.SpecialFields.Add(new SpecialField() { Text = NoneValueText, Value = "-1" });
            }

            specialFieldsAdded = true;
        }
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            int id = ValidationHelper.GetInteger(usGroups.Value, 0);
            if (id > 0)
            {
                return base.GetWhereCondition();
            }
        }
        return null;
    }

    #endregion
}