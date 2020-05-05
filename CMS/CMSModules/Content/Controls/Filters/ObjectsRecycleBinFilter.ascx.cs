using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Filters_ObjectsRecycleBinFilter : CMSAbstractRecycleBinFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Returns selected user identifier.
    /// </summary>
    public int SelectedUser
    {
        get
        {
            Control postbackControl = ControlsHelper.GetPostBackControl(Page);
            return ValidationHelper.GetInteger((postbackControl == btnShow) ? userSelector.Value : ViewState["SelectedUser"], MembershipContext.AuthenticatedUser.UserID);
        }
        private set
        {
            userSelector.Value = value;
            ViewState["SelectedUser"] = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return CreateWhereCondition(base.WhereCondition);
        }
    }


    /// <summary>
    /// Determines whether filter is set.
    /// </summary>
    public override bool FilterIsSet
    {
        get
        {
            int userId = ValidationHelper.GetInteger(userSelector.Value, 0);
            string objType = ValidationHelper.GetString(objTypeSelector.Value, "");
            return nameFilter.FilterIsSet || !String.IsNullOrEmpty(objType) || ((userId > 0) && UsersPlaceHolder.Visible);
        }
    }


    /// <summary>
    /// Gets place holder with user selector.
    /// </summary>
    public PlaceHolder UsersPlaceHolder
    {
        get
        {
            return plcUsers;
        }
    }


    /// <summary>
    /// Indicates if date time filter should be displayed.
    /// </summary>
    public override bool DisplayDateTimeFilter
    {
        get
        {
            return plcDateTime.Visible;
        }
        set
        {
            plcDateTime.Visible = value;
        }
    }


    /// <summary>
    /// Site ID to filter usable values.
    /// </summary>
    public override int SiteID
    {
        get
        {
            return userSelector.SiteID;
        }
        set
        {
            userSelector.SiteID = value;
        }
    }


    /// <summary>
    /// Indicates if all available users should be displayed.
    /// </summary>
    public override bool DisplayUsersFromAllSites
    {
        get
        {
            return userSelector.DisplayUsersFromAllSites;
        }
        set
        {
            userSelector.DisplayUsersFromAllSites = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page Init event.
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            // Preselect default value
            SelectedUser = MembershipContext.AuthenticatedUser.UserID;
        }

        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null && grid.RememberState)
        {
            btnReset.Text = GetString("general.reset");
            btnReset.Click += btnReset_Click;
        }
        else
        {
            btnReset.Visible = false;
        }

        UsersPlaceHolder.Visible = MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        userSelector.DropDownSingleSelect.AutoPostBack = true;
        userSelector.DropDownSingleSelect.SelectedIndexChanged += userSelector_SelectedIndexChanged;

        if (!RequestHelper.IsPostBack())
        {
            if (DisplayDateTimeFilter)
            {
                // Fill the dropdown list
                EnsureDropDownListFilterItems();

                // Load default value
                if (String.IsNullOrEmpty(txtFilter.Text))
                {
                    txtFilter.Text = "0";
                }
            }
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Reload control data.
    /// </summary>
    private void ReloadData()
    {
        SetObjTypeSelector();
        objTypeSelector.Reload(true);
        WhereCondition = null;

        pnlObjType.Update();
    }


    private string CreateWhereCondition(string originalWhere)
    {
        var where = new WhereCondition();
        where.Where(new WhereCondition(originalWhere){ WhereIsComplex = true });

        // Add where conditions from filters
        where.Where(new WhereCondition(nameFilter.WhereCondition) { WhereIsComplex = true });

        string objType = ValidationHelper.GetString(objTypeSelector.Value, "");
        if (!String.IsNullOrEmpty(objType))
        {
#pragma warning disable BH2000 // Method 'WhereLike()' or 'WhereNotLike()' should not be used used.
            where.WhereLike("VersionObjectType", objType);
#pragma warning restore BH2000 // Method 'WhereLike()' or 'WhereNotLike()' should not be used used.
        }

        int userId = ValidationHelper.GetInteger(userSelector.Value, 0);
        if (userId > 0)
        {
            where.WhereEquals("VersionDeletedByUserID", userId);
        }

        // Get older than value
        if (DisplayDateTimeFilter)
        {
            DateTime olderThan = DateTime.Now.Date.AddDays(1);
            int dateTimeValue = ValidationHelper.GetInteger(txtFilter.Text, 0);

            switch (drpFilter.SelectedIndex)
            {
                case 0:
                    olderThan = olderThan.AddDays(-dateTimeValue);
                    break;

                case 1:
                    olderThan = olderThan.AddDays(-dateTimeValue * 7);
                    break;

                case 2:
                    olderThan = olderThan.AddMonths(-dateTimeValue);
                    break;

                case 3:
                    olderThan = olderThan.AddYears(-dateTimeValue);
                    break;
            }

            where.WhereLessOrEquals("VersionDeletedWhen", olderThan);
        }

        return where.ToString(true);
    }



    /// <summary>
    /// Set object type selector properties
    /// </summary>
    private void SetObjTypeSelector()
    {
        objTypeSelector.UserID = ValidationHelper.GetInteger(userSelector.Value, 0);
        if (SiteID > 0)
        {
            objTypeSelector.WhereCondition = "VersionObjectSiteID = " + SiteID;
            if (IsSingleSite)
            {
                objTypeSelector.WhereCondition = SqlHelper.AddWhereCondition(objTypeSelector.WhereCondition, "VersionObjectSiteID IS NULL", "OR");
            }
        }
        else if ((SiteID == 0) && !IsSingleSite)
        {
            objTypeSelector.WhereCondition = SqlHelper.AddWhereCondition(objTypeSelector.WhereCondition, "VersionObjectSiteID IS NULL");
        }
    }


    private void EnsureDropDownListFilterItems()
    {
        if (drpFilter.Items.Count == 0)
        {
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Days"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Weeks"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Months"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Years"));
        }
    }

    #endregion


    #region "Control events"

    protected void btnShow_Click(object sender, EventArgs e)
    {
        SelectedUser = ValidationHelper.GetInteger(userSelector.Value, MembershipContext.AuthenticatedUser.UserID);

        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    private void userSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        ReloadData();
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("ObjectRecycleBinUser", SelectedUser);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        // Before retrieving selected value from saved state is necessary to ensure items in DropDownList (delete before)
        EnsureDropDownListFilterItems();

        base.RestoreFilterState(state);

        SelectedUser = state.GetInt32("ObjectRecycleBinUser");

    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        if (DisplayDateTimeFilter)
        {
            txtFilter.Text = "0";
            drpFilter.SelectedIndex = 0;
        }

        SelectedUser = MembershipContext.AuthenticatedUser.UserID;
        userSelector.ReloadData();

        nameFilter.ResetFilter();

        objTypeSelector.Value = null;
        objTypeSelector.Reload(true);
    }

    #endregion
}