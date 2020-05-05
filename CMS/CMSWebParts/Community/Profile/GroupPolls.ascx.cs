using System;
using System.Web;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_Community_Profile_GroupPolls : CMSAbstractWebPart
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
            return ValidationHelper.GetString(GetValue("NoPermissionMessage"), "");
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
    /// Reloads data for partial caching.
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
            ScriptHelper.RegisterBootstrapScripts(this.Page);
            pollsElem.IsLiveSite = true;
            pollsElem.HideWhenGroupIsNotSupplied = true;
            pollsElem.OnCheckPermissions += pollsElem_OnCheckPermissions;
            GroupInfo gi = GroupInfoProvider.GetGroupInfo(GroupName, SiteContext.CurrentSiteName);
            if (gi != null)
            {
                pollsElem.GroupID = gi.GroupID;
                pollsElem.SiteID = gi.GroupSiteID;
            }
            else
            {
                Visible = false;
            }
            pollsElem.ReloadData();
        }
    }


    /// <summary>
    /// Group members - check permissions.
    /// </summary>
    protected void pollsElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!(MembershipContext.AuthenticatedUser.IsGroupAdministrator(pollsElem.GroupID) || MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", "Manage")))
        {
            if (sender != null)
            {
                sender.StopProcessing = true;
            }
            pollsElem.StopProcessing = true;
            pollsElem.Visible = false;
            messageElem.Visible = true;
            messageElem.ErrorMessage = NoPermissionMessage;
        }
    }
}
