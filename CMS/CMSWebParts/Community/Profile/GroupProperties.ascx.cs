using System;
using System.Web;
using System.Web.UI;

using CMS.Community;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_Community_Profile_GroupProperties : CMSAbstractWebPart
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


    /// <summary>
    /// If true group display name change allowed on live site.
    /// </summary>
    public bool AllowChangeGroupDisplayName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowChangeGroupDisplayName"), false);
        }
        set
        {
            groupEditElem.AllowChangeGroupDisplayName = value;
            SetValue("AllowChangeGroupDisplayName", value);
        }
    }


    /// <summary>
    /// If true changing theme for group page is enabled.
    /// </summary>
    public bool AllowSelectTheme
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowSelectTheme"), false);
        }
        set
        {
            SetValue("AllowSelectTheme", value);
            groupEditElem.AllowSelectTheme = value;
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
    /// Reloads data.
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
            groupEditElem.HideWhenGroupIsNotSupplied = true;
            groupEditElem.OnCheckPermissions += groupEditElem_OnCheckPermissions;
            GroupInfo gi = GroupInfoProvider.GetGroupInfo(GroupName, SiteContext.CurrentSiteName);
            if (gi != null)
            {
                groupEditElem.AllowChangeGroupDisplayName = AllowChangeGroupDisplayName;
                groupEditElem.GroupID = gi.GroupID;
                groupEditElem.SiteID = gi.GroupSiteID;
                groupEditElem.AllowSelectTheme = AllowSelectTheme;
            }
            else
            {
                Visible = false;
            }
        }
    }


    /// <summary>
    /// Group properties - check permissions.
    /// </summary>
    private void groupEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!(MembershipContext.AuthenticatedUser.IsGroupAdministrator(groupEditElem.GroupID) || MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", "Manage")))
        {
            if (sender != null)
            {
                sender.StopProcessing = true;
            }
            groupEditElem.StopProcessing = true;
            groupEditElem.Visible = false;
            messageElem.ErrorMessage = NoPermissionMessage;
            messageElem.Visible = true;
        }
    }
}