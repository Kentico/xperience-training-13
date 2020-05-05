using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_GroupList : CMSAdminListControl
{
    #region "Variables"

    private int mSiteId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the site ID for which the roles should be displayed.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
            gridElem.WhereCondition = CreateWhereCondition();
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Unigrid
        gridElem.OnAction += gridElem_OnAction;
        gridElem.WhereCondition = CreateWhereCondition();
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.ZeroRowsText = GetString("general.nodatafound");
        gridElem.HideFilterButton = true;
    }


    /// <summary>
    /// Unigrid external databound handler.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "GroupApproved":
                {
                    if (ValidationHelper.GetBoolean(parameter, false))
                    {
                        return "<span class=\"Approved\">" + GetString("general.yes") + "</span>";
                    }
                    else
                    {
                        return "<span class=\"NotApproved\">" + GetString("general.no") + "</span>";
                    }
                }


            case "approve":
                bool approve = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["GroupApproved"], false);
                if (!approve)
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
                break;

            case "reject":
                bool reject = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["GroupApproved"], false);
                if (reject)
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
                break;
        }

        return parameter.ToString();
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        GroupInfo gi = null;

        // Handle event
        switch (actionName)
        {
            case "approve":
                CheckPermissions();
                gi = GroupInfoProvider.GetGroupInfo(ValidationHelper.GetInteger(actionArgument, 0));
                gi.GroupApproved = true;
                gi.GroupApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                GroupInfoProvider.SetGroupInfo(gi);
                break;

            case "reject":
                CheckPermissions();
                gi = GroupInfoProvider.GetGroupInfo(ValidationHelper.GetInteger(actionArgument, 0));
                gi.GroupApproved = false;
                gi.GroupApprovedByUserID = 0;
                GroupInfoProvider.SetGroupInfo(gi);
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }


    /// <summary>
    /// Check manage permission.
    /// </summary>
    private void CheckPermissions()
    {
        if (!CheckPermissions("cms.groups", PERMISSION_MANAGE))
        {
        }
    }


    /// <summary>
    /// Creates where condition for unigrid according to the parameters.
    /// </summary>
    private string CreateWhereCondition()
    {
        string where = null;

        if (SiteID > 0)
        {
            where = "(GroupSiteID = " + SiteID + ")";
        }

        return where;
    }
}