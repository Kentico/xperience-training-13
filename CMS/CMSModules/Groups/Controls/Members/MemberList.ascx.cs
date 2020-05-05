using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_Members_MemberList : CMSAdminListControl
{
    #region "Variables"

    private int mGroupId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the group ID for which the members should be displayed.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
            gridElem.WhereCondition = CreateWhereCondition();
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize unigrid
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.WhereCondition = CreateWhereCondition();
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnBeforeDataReload += new OnBeforeDataReload(gridElem_OnBeforeDataReload);
        gridElem.ZeroRowsText = GetString("general.nodatafound");
    }

    #endregion


    #region "GridView actions handling"

    /// <summary>
    /// On before data reyload action.
    /// </summary>
    private void gridElem_OnBeforeDataReload()
    {
        string where = CreateWhereCondition();

        // Prepare where condition
        if (!string.IsNullOrEmpty(filterMembers.WhereCondition))
        {
            where = where + " AND (" + filterMembers.WhereCondition + ")";
        }

        gridElem.WhereCondition = where;
    }


    /// <summary>
    /// Unigrid OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        GroupMemberStatus status = GroupMemberStatus.Approved;
        DataRowView drv = null;
        GridViewRow gvr = null;
        bool current = false;

        switch (sourceName.ToLowerCSafe())
        {
            case "memberapprovedwhen":
            case "memberrejectedwhen":
                if (parameter != DBNull.Value)
                {
                    // Get current dateTime
                    return TimeZoneUIMethods.ConvertDateTime(Convert.ToDateTime(parameter), this);
                }
                break;

            case "approve":
                gvr = parameter as GridViewRow;
                if (gvr != null)
                {
                    drv = gvr.DataItem as DataRowView;
                    if (drv != null)
                    {
                        // Check for current user
                        if (IsLiveSite && (MembershipContext.AuthenticatedUser.UserID == ValidationHelper.GetInteger(drv["MemberUserID"], 0)))
                        {
                            current = true;
                        }

                        // Do not allow approve hidden or disabled users
                        bool hiddenOrDisabled = IsUserHiddenOrDisabled(drv);

                        status = (GroupMemberStatus)ValidationHelper.GetInteger(drv["MemberStatus"], 0);

                        // Enable or disable Approve button
                        if (!current && (status != GroupMemberStatus.Approved) && !hiddenOrDisabled)
                        {
                            CMSGridActionButton button = ((CMSGridActionButton)sender);
                            button.IconCssClass = "icon-check-circle";
                            button.IconStyle = GridIconStyle.Allow;
                            button.ToolTip = GetString("general.approve");
                            button.Enabled = true;
                        }
                        else
                        {
                            CMSGridActionButton button = ((CMSGridActionButton)sender);
                            button.IconCssClass = "icon-check-circle";
                            button.IconStyle = GridIconStyle.Allow;
                            button.ToolTip = GetString("general.approve");
                            button.Enabled = false;
                        }
                    }
                }

                break;

            case "reject":
                gvr = parameter as GridViewRow;
                if (gvr != null)
                {
                    drv = gvr.DataItem as DataRowView;
                    if (drv != null)
                    {
                        // Check for current user
                        if (IsLiveSite && (MembershipContext.AuthenticatedUser.UserID == ValidationHelper.GetInteger(drv.Row["MemberUserID"], 0)))
                        {
                            current = true;
                        }

                        status = (GroupMemberStatus)ValidationHelper.GetInteger(drv["MemberStatus"], 0);

                        // Enable or disable Reject button
                        if (!current && (status != GroupMemberStatus.Rejected))
                        {
                            CMSGridActionButton button = ((CMSGridActionButton)sender);
                            button.IconCssClass = "icon-times-circle";
                            button.IconStyle = GridIconStyle.Critical;
                            button.ToolTip = GetString("general.reject");
                            button.Enabled = true;
                        }
                        else
                        {
                            CMSGridActionButton button = ((CMSGridActionButton)sender);
                            button.IconCssClass = "icon-times-circle";
                            button.IconStyle = GridIconStyle.Critical;
                            button.ToolTip = GetString("general.reject");
                            button.Enabled = false;
                        }
                    }
                }
                break;

            case "formattedusername":
                // Format username
                return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(Convert.ToString(parameter), IsLiveSite));

            case "edit":
                gvr = parameter as GridViewRow;
                if (gvr != null)
                {
                    drv = gvr.DataItem as DataRowView;
                    if (drv != null)
                    {
                        // Do not allow approve hidden or disabled users
                        bool hiddenOrDisabled = IsUserHiddenOrDisabled(drv);

                        // Enable or disable Edit button
                        if (!hiddenOrDisabled)
                        {
                            CMSGridActionButton button = ((CMSGridActionButton)sender);
                            button.IconCssClass = "icon-edit";
                            button.IconStyle = GridIconStyle.Allow;
                            button.ToolTip = GetString("general.edit");
                            button.Enabled = true;
                        }
                        else
                        {
                            CMSGridActionButton button = ((CMSGridActionButton)sender);
                            button.IconCssClass = "icon-edit";
                            button.IconStyle = GridIconStyle.Allow;
                            button.ToolTip = GetString("general.edit");
                            button.Enabled = false;
                        }
                    }
                }
                break;
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
            case "approve":
            case "reject":

                // Check MANAGE permission for groups module
                if (!CheckPermissions("cms.groups", PERMISSION_MANAGE, GroupID))
                {
                    return;
                }

                break;
        }

        if (actionName == "delete")
        {
            // Delete member
            GroupMemberInfoProvider.DeleteGroupMemberInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
        else if (actionName == "approve")
        {
            // Approve member
            GroupMemberInfo gmi = GroupMemberInfoProvider.GetGroupMemberInfo(ValidationHelper.GetInteger(actionArgument, 0));
            if (gmi != null)
            {
                gmi.MemberApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                gmi.MemberStatus = GroupMemberStatus.Approved;
                gmi.MemberApprovedWhen = DateTime.Now;
                gmi.MemberRejectedWhen = DateTimeHelper.ZERO_TIME;
                GroupMemberInfoProvider.SetGroupMemberInfo(gmi);
                GroupInfo group = GroupInfoProvider.GetGroupInfo(GroupID);
                if ((group != null) && (group.GroupSendWaitingForApprovalNotification))
                {
                    // Send notification email
                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberApproved", SiteContext.CurrentSiteName, gmi, false);
                }
            }
        }
        else if (actionName == "reject")
        {
            // Reject member
            GroupMemberInfo gmi = GroupMemberInfoProvider.GetGroupMemberInfo(ValidationHelper.GetInteger(actionArgument, 0));
            if (gmi != null)
            {
                gmi.MemberApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                gmi.MemberStatus = GroupMemberStatus.Rejected;
                gmi.MemberApprovedWhen = DateTimeHelper.ZERO_TIME;
                gmi.MemberRejectedWhen = DateTime.Now;
                GroupMemberInfoProvider.SetGroupMemberInfo(gmi);
                GroupInfo group = GroupInfoProvider.GetGroupInfo(GroupID);
                if ((group != null) && (group.GroupSendWaitingForApprovalNotification))
                {
                    // Send notification email
                    GroupMemberInfoProvider.SendNotificationMail("Groups.MemberRejected", SiteContext.CurrentSiteName, gmi, false);
                }
            }
        }
        RaiseOnAction(actionName, actionArgument);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the grid data.
    /// </summary>
    public void ReloadGrid()
    {
        gridElem.ReloadData();
    }


    /// <summary>
    /// Creates where condition for unigrid according to the parameters.
    /// </summary>
    private string CreateWhereCondition()
    {
        // Prepare where condition
        string where = String.Format("(MemberGroupID = {0} AND SiteID = {1})", mGroupId, SiteContext.CurrentSiteID); 

        if (IsLiveSite)
        {
            where += " AND (UserIsHidden = 0 OR UserIsHidden IS NULL) AND " + UserInfoProvider.UserEnabledWhereCondition;
        }

        return where;
    }


    /// <summary>
    /// Returns if user is hidden or disabled
    /// </summary>
    /// <param name="drv">DataRowView with user data to inspect</param>
    private bool IsUserHiddenOrDisabled(DataRowView drv)
    {
        if (drv != null)
        {
            return (ValidationHelper.GetBoolean(drv["UserIsHidden"], false) || (!ValidationHelper.GetBoolean(drv["UserEnabled"], true) || (ValidationHelper.GetInteger(drv["UserAccountLockReason"], 0) != 0)));
        }

        return true;
    }

    #endregion
}