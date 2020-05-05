using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_GroupFilter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Where condition built using the filter options.
    /// </summary>
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


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!RequestHelper.IsPostBack())
        {
            InitializeForm();
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
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }
    #endregion


    #region "Private methods"


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtGroupName.Text = String.Empty;
        drpGroupStatus.SelectedIndex = 0;
        drpGroupName.SelectedIndex = 0;
    }

    /// <summary>
    /// Initializes filter controls.
    /// </summary>
    private void InitializeForm()
    {
        // Initialize first dropdown lists
        ControlsHelper.FillListWithNumberedSqlOperators(drpGroupName);

        drpGroupStatus.Items.Add(new ListItem(GetString("general.selectall"), "0"));
        drpGroupStatus.Items.Add(new ListItem(GetString("groups.status.waitingforapproval"), "1"));
        drpGroupStatus.Items.Add(new ListItem(GetString("general.approved"), "2"));
        drpGroupStatus.Items.Add(new ListItem(GetString("general.rejected"), "3"));

        // Preselect all
        drpGroupStatus.SelectedIndex = 0;
    }


    /// <summary>
    /// Builds where condition.
    /// </summary>
    /// <returns>Filter where condition</returns>
    private string BuildWhereCondition()
    {
        string whereCondition = String.Empty;
        // Group name
        string groupName = txtGroupName.Text.Trim().Replace("'", "''");
        if (!String.IsNullOrEmpty(groupName))
        {
            // Get proper operator name
            int sqlOperatorNumber = ValidationHelper.GetInteger(drpGroupName.SelectedValue, 0);
            string sqlOperatorName;
            switch (sqlOperatorNumber)
            {
                case 1:
                    sqlOperatorName = WhereBuilder.NOT_LIKE;
                    break;
                case 2:
                    sqlOperatorName = WhereBuilder.EQUAL;
                    break;
                case 3:
                    sqlOperatorName = WhereBuilder.NOT_EQUAL;
                    break;
                default:
                    sqlOperatorName = WhereBuilder.LIKE;
                    break;
            }

            if ((sqlOperatorName == WhereBuilder.LIKE) || (sqlOperatorName == WhereBuilder.NOT_LIKE))
            {
                groupName = "%" + groupName + "%";
            }

            whereCondition = "(GroupDisplayName " + sqlOperatorName + " N'" + groupName + "')";
        }

        // Group status
        int sqlStatusNumber = ValidationHelper.GetInteger(drpGroupStatus.SelectedValue, 0);
        string sqlStatusCode;
        switch (sqlStatusNumber)
        {
            case 1:
                sqlStatusCode = "GroupApproved IS NULL";
                break;
            case 2:
                sqlStatusCode = "GroupApproved = 1";
                break;
            case 3:
                sqlStatusCode = "GroupApproved = 0";
                break;
            default:
                sqlStatusCode = "";
                break;
        }

        if (!String.IsNullOrEmpty(sqlStatusCode))
        {
            if (!String.IsNullOrEmpty(whereCondition))
            {
                whereCondition += " AND ";
            }

            whereCondition += "(" + sqlStatusCode + ")";
        }

        return whereCondition;
    }

    #endregion
}