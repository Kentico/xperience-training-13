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

public partial class CMSWebParts_Community_Profile_GroupProfile : CMSAbstractWebPart
{
    #region "Private variables"

    private bool mDisplayMessage = false;

    #endregion


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
            groupProfileElem.AllowChangeGroupDisplayName = value;
            SetValue("AllowChangeGroupDisplayName", value);
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
    /// If true, changing theme for group page is enabled.
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
            groupProfileElem.AllowSelectTheme = value;
        }
    }


    /// <summary>
    /// If true, the general tab is enabled.
    /// </summary>
    public bool DisplayGeneral
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayGeneral"), groupProfileElem.ShowGeneralTab);
        }
        set
        {
            SetValue("DisplayGeneral", value);
            groupProfileElem.ShowGeneralTab = value;
        }
    }


    /// <summary>
    /// If true, the security tab is enabled.
    /// </summary>
    public bool DisplaySecurity
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplaySecurity"), groupProfileElem.ShowSecurityTab);
        }
        set
        {
            SetValue("DisplaySecurity", value);
            groupProfileElem.ShowSecurityTab = value;
        }
    }


    /// <summary>
    /// If true, the members tab is enabled.
    /// </summary>
    public bool DisplayMembers
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMembers"), groupProfileElem.ShowMembersTab);
        }
        set
        {
            SetValue("DisplayMembers", value);
            groupProfileElem.ShowMembersTab = value;
        }
    }


    /// <summary>
    /// If true, the roles tab is enabled.
    /// </summary>
    public bool DisplayRoles
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayRoles"), groupProfileElem.ShowRolesTab);
        }
        set
        {
            SetValue("DisplayRoles", value);
            groupProfileElem.ShowRolesTab = value;
        }
    }


    /// <summary>
    /// If true, the forums tab is enabled.
    /// </summary>
    public bool DisplayForums
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayForums"), groupProfileElem.ShowForumsTab);
        }
        set
        {
            SetValue("DisplayForums", value);
            groupProfileElem.ShowForumsTab = value;
        }
    }


    /// <summary>
    /// If true, the media library tab is enabled.
    /// </summary>
    public bool DisplayMediaLibrary
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMediaLibrary"), groupProfileElem.ShowMediaTab);
        }
        set
        {
            SetValue("DisplayMediaLibrary", value);
            groupProfileElem.ShowMediaTab = value;
        }
    }


    /// <summary>
    /// If true, the message boards tab is enabled.
    /// </summary>
    public bool DisplayMessageBoards
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMessageBoards"), groupProfileElem.ShowMessageBoardsTab);
        }
        set
        {
            SetValue("DisplayMessageBoards", value);
            groupProfileElem.ShowMessageBoardsTab = value;
        }
    }


    /// <summary>
    /// If true, the polls tab is enabled.
    /// </summary>
    public bool DisplayPolls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPolls"), groupProfileElem.ShowPollsTab);
        }
        set
        {
            SetValue("DisplayPolls", value);
            groupProfileElem.ShowPollsTab = value;
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
            SetContext();

            messageElem.ErrorMessage = NoPermissionMessage;
            messageElem.IsLiveSite = true;
            groupProfileElem.IsLiveSite = true;
            groupProfileElem.RedirectToAccessDeniedPage = false;
            groupProfileElem.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(groupProfileElem_OnCheckPermissions);

            groupProfileElem.HideWhenGroupIsNotSupplied = true;

            GroupInfo gi = GroupInfoProvider.GetGroupInfo(GroupName, SiteContext.CurrentSiteName);

            if (gi != null)
            {
                groupProfileElem.GroupID = gi.GroupID;
                groupProfileElem.AllowChangeGroupDisplayName = AllowChangeGroupDisplayName;
                groupProfileElem.AllowSelectTheme = AllowSelectTheme;
                groupProfileElem.ShowGeneralTab = DisplayGeneral;
                groupProfileElem.ShowSecurityTab = DisplaySecurity;
                groupProfileElem.ShowMembersTab = DisplayMembers;
                groupProfileElem.ShowRolesTab = DisplayRoles;
                groupProfileElem.ShowForumsTab = DisplayForums;
                groupProfileElem.ShowMediaTab = DisplayMediaLibrary;
                groupProfileElem.ShowMessageBoardsTab = DisplayMessageBoards;
                groupProfileElem.ShowPollsTab = DisplayPolls;
            }
            else
            {
                groupProfileElem.StopProcessing = true;
                groupProfileElem.Visible = false;
                mDisplayMessage = true;
            }

            ReleaseContext();
        }
    }


    /// <summary>
    /// Group profile - check permissions.
    /// </summary>
    private void groupProfileElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!(MembershipContext.AuthenticatedUser.IsGroupAdministrator(groupProfileElem.GroupID) || MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", permissionType)))
        {
            if (sender != null)
            {
                sender.StopProcessing = true;
            }
            groupProfileElem.StopProcessing = true;
            groupProfileElem.Visible = false;
            messageElem.ErrorMessage = NoPermissionMessage;
            mDisplayMessage = true;
        }
    }


    /// <summary>
    /// Render override.
    /// </summary>
    /// <param name="writer">Writer</param>
    protected override void Render(HtmlTextWriter writer)
    {
        if (!mDisplayMessage)
        {
            messageElem.Visible = false;
        }

        base.Render(writer);
    }
}