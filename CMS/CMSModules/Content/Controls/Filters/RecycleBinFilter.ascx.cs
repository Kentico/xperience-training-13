using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Filters_RecycleBinFilter : CMSAbstractRecycleBinFilterControl
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
            return nameFilter.FilterIsSet || pathFilter.FilterIsSet || classFilter.FilterIsSet || ((userId > 0) && UsersPlaceHolder.Visible) || !string.IsNullOrWhiteSpace(txtFilter.Text);
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
    /// Site ID to filter users.
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
    /// Gets user selector control.
    /// </summary>
    public FormEngineUserControl UserSelector
    {
        get
        {
            return userSelector;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        UsersPlaceHolder.Visible = MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);

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

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
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

        DisplayUsersFromAllSites = !(SiteID > 0);
        userSelector.TreatGlobalAdminsAsSiteUsers = !DisplayUsersFromAllSites;
        userSelector.ApplyValueRestrictions = false;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reload control data.
    /// </summary>
    public void ReloadData()
    {
        userSelector.AllowAll = MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
        userSelector.ReloadData();
    }


    private string CreateWhereCondition(string originalWhere)
    {
        string where = originalWhere;
        // Add where conditions from filters
        where = SqlHelper.AddWhereCondition(where, pathFilter.WhereCondition);
        where = SqlHelper.AddWhereCondition(where, nameFilter.WhereCondition);
        if (!string.IsNullOrEmpty(classFilter.WhereCondition))
        {
            where = SqlHelper.AddWhereCondition(where, "VersionClassID IN (SELECT ClassID FROM CMS_Class WHERE " + classFilter.WhereCondition + ")");
        }

        int userId = ValidationHelper.GetInteger(userSelector.Value, 0);
        if (userId > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "VersionDeletedByUserID = " + userId);
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
                    olderThan = olderThan.AddDays(-dateTimeValue*7);
                    break;

                case 2:
                    olderThan = olderThan.AddMonths(-dateTimeValue);
                    break;

                case 3:
                    olderThan = olderThan.AddYears(-dateTimeValue);
                    break;
            }

            where = SqlHelper.AddWhereCondition(where, "VersionDeletedWhen <= '" + olderThan.ToString(TableManager.DatabaseCultureInfo) + "'");
        }
        else
        {
            // Make sure that only deleted documents are selected
            where = SqlHelper.AddWhereCondition(where, "VersionDeletedWhen IS NOT NULL");
        }

        return where;
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


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("RecycleBinUser", SelectedUser);
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

        SelectedUser = state.GetInt32("RecycleBinUser");   
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
        ReloadData();

        nameFilter.ResetFilter();
        pathFilter.ResetFilter();
        classFilter.ResetFilter();
    }

    #endregion
}