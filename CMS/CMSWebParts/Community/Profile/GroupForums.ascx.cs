using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Community;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_Community_Profile_GroupForums : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets group name to specify group members.
    /// </summary>
    public string GroupName
    {
        get
        {
            string groupName = ValidationHelper.GetString(GetValue("GroupName"), "");
            if ((string.IsNullOrEmpty(groupName) || groupName == GroupInfoProvider.CURRENT_GROUP) && (CommunityContext.CurrentGroup != null))
            {
                return CommunityContext.CurrentGroup.GroupName;
            }
            return groupName;
        }
        set
        {
            SetValue("GroupName", value);
        }
    }


    /// <summary>
    /// Gets or sets message which should be displayed if user hasn't permissions.
    /// </summary>
    public string NoPermissionMessage
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NoPermissionMessage"), messageElem.ErrorMessage);
        }
        set
        {
            SetValue("NoPermissionMessage", value);
            messageElem.ErrorMessage = value;
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
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            forumsElem.HideWhenGroupIsNotSupplied = true;
            forumsElem.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumsElem_OnCheckPermissions);

            GroupInfo gi = GroupInfoProvider.GetGroupInfo(GroupName, SiteContext.CurrentSiteName);
            if (gi != null)
            {
                forumsElem.GroupID = gi.GroupID;
            }
        }
    }


    /// <summary>
    /// Group forums - check permissions.
    /// </summary>
    private void forumsElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!(MembershipContext.AuthenticatedUser.IsGroupAdministrator(forumsElem.GroupID) || MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", "Manage")))
        {
            if (sender != null)
            {
                sender.StopProcessing = true;
            }
            forumsElem.StopProcessing = true;
            forumsElem.Visible = false;
            messageElem.Visible = true;
            messageElem.ErrorMessage = String.Format(NoPermissionMessage, permissionType, "CMS.Groups");
        }
    }
}