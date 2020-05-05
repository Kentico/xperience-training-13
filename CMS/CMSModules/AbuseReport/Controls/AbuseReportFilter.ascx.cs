using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AbuseReport_Controls_AbuseReportFilter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    public override string WhereCondition
    {
        get
        {
            return BuildWhereCondition();
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        siteSelector.DropDownSingleSelect.AutoPostBack = false;

        // Load dropdown list
        if (!RequestHelper.IsPostBack())
        {
            InitializeComponents();
            siteSelector.Value = SiteContext.CurrentSiteID;
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
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads status from enumeration to dropdown list.
    /// </summary>
    private void InitializeComponents()
    {
        drpStatus.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        drpStatus.Items.Add(new ListItem(GetString("general.new"), "0"));
        drpStatus.Items.Add(new ListItem(GetString("general.solved"), "1"));
        drpStatus.Items.Add(new ListItem(GetString("general.rejected"), "2"));

        // Status preselection by URL
        string preselectedStatus = QueryHelper.GetString("status", null);
        if (!String.IsNullOrEmpty(preselectedStatus))
        {
            foreach (ListItem item in drpStatus.Items)
            {
                if (item.Value == preselectedStatus)
                {
                    drpStatus.SelectedIndex = drpStatus.Items.IndexOf(item);
                }
            }
        }

        // Show site selector only for global admin
        if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            // Set site selector
            siteSelector.AllowAll = true;
        }
        else
        {
            plcSites.Visible = false;
        }
    }


    /// <summary>
    /// Builds complete where condition to filtering
    /// </summary>
    private string BuildWhereCondition()
    {
        // Create WHERE condition with ReportStatus
        string completeWhere = "";
        if (!String.IsNullOrEmpty(drpStatus.SelectedValue) && (drpStatus.SelectedValue != "-1"))
        {
            completeWhere = SqlHelper.AddWhereCondition(completeWhere, "(ReportStatus = " + ValidationHelper.GetInteger(drpStatus.SelectedValue, 0) + ")");
        }

        // Create WHERE condition with ReportTitle
        string txt = txtTitle.Text.Trim().Replace("'", "''");
        if (!string.IsNullOrEmpty(txt))
        {
            completeWhere = SqlHelper.AddWhereCondition(completeWhere, "(ReportTitle LIKE '%" + txt + "%')");
        }

        // Create WHERE condition with ReportObjectType
        int siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        if (siteId == 0)
        {
            siteId = SiteContext.CurrentSiteID;
        }
        if (siteId > 0)
        {
            completeWhere = SqlHelper.AddWhereCondition(completeWhere, "(ReportSiteID = " + siteId + ")");
        }

        return completeWhere;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("ReportStatus", drpStatus.SelectedValue);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        drpStatus.SelectedValue = state.GetString("ReportStatus");
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpStatus.SelectedValue = "-1";
        txtTitle.Text = String.Empty;
        siteSelector.Value = SiteContext.CurrentSiteID;
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
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnShow_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }

    #endregion
}
