using System;

using CMS.Chat;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Chat_Controls_UI_ChatSupportCannedResponse_Edit : CMSAdminEditControl
{
    #region "Properties"

    private int siteID = QueryHelper.GetInteger("siteid", 0);

    /// <summary>
    /// SiteID of a new canned response taken from query string.
    /// 
    /// NULL means global.
    /// </summary>
    public int? SiteID
    {
        get
        {
            // Global
            if (siteID <= 0)
            {
                return null;
            }

            return siteID;
        }
    }

    /// <summary>
    /// Returns edited object info.
    /// </summary>
    public ChatSupportCannedResponseInfo TypedEditedObject
    {
        get
        {
            return (ChatSupportCannedResponseInfo)UIContext.EditedObject;
        }
    }


    public UIForm EditForm
    {
        get
        {
            return editForm;
        }
    }


    /// <summary>
    /// Indicates whether canned response is personal.
    /// </summary>
    public bool Personal
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckPermissions(PermissionsEnum.Read);
        EditForm.OnBeforeSave += UIFormControl_OnBeforeSave;
        EditForm.OnCheckPermissions += UIFormControl_OnCheckPermissions;
    }


    private void UIFormControl_OnCheckPermissions(object sender, EventArgs e)
    {
        CheckPermissions(PermissionsEnum.Modify);
    }


    private void UIFormControl_OnBeforeSave(object sender, EventArgs e)
    {
        // If creating a new canned response
        if (TypedEditedObject == null || TypedEditedObject.ChatSupportCannedResponseID <= 0)
        {
            if (!Personal)
            {
                EditForm.Data["ChatSupportCannedResponseChatUserID"] = null;
                EditForm.Data["ChatSupportCannedResponseSiteID"] = SiteID;
            }
            else
            {
                EditForm.Data["ChatSupportCannedResponseChatUserID"] = ChatUserHelper.GetChatUserFromCMSUser().ChatUserID;
                EditForm.Data["ChatSupportCannedResponseSiteID"] = null;
            }
        }
    }

    #endregion


    #region "Helper methods"

    private void CheckPermissions(PermissionsEnum permission)
    {
        if (TypedEditedObject == null || TypedEditedObject.ChatSupportCannedResponseID <= 0)
        {
            // Creating new
            CheckPermissionsForNewResponse();
            return;
        }

        if (!TypedEditedObject.CheckPermissions(permission, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser, false))
        {
            if (TypedEditedObject.ChatSupportCannedResponseChatUserID.HasValue)
            {
                RedirectToAccessDenied(GetString("chat.error.cannedresponsenotyours"));
            }
            else
            {
                RedirectToAccessDenied(TypedEditedObject.TypeInfo.ModuleName, (SiteID == null ? "Global" : "") + permission.ToStringRepresentation());
            }
        }
    }


    private void CheckPermissionsForNewResponse()
    {
        UserInfo user = MembershipContext.AuthenticatedUser;
        string moduleName = TypedEditedObject.TypeInfo.ModuleName;

        if (Personal)
        {
            // Personal canned response
            if (!user.IsAuthorizedPerResource(moduleName, "EnterSupport"))
            {
                RedirectToAccessDenied(moduleName, "EnterSupport");
            }
        }
        else if (SiteID != null)
        {
            // Site canned response
            if (!user.IsAuthorizedPerResource(moduleName, "Modify"))
            {
                RedirectToAccessDenied(moduleName, "Modify");
            }
        }
        else
        {
            // Global canned response
            if (!user.IsAuthorizedPerResource(moduleName, "GlobalModify"))
            {
                RedirectToAccessDenied(moduleName, "GlobalModify");
            }
        }
    }

    #endregion
}