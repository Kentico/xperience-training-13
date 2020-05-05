using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_Groups_CMSPages_Message_Edit : CMSLiveModalPage
{
    private int mBoardId;
    private int mMessageId;
    private int mGroupId;
    private CurrentUserInfo cu;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        cu = MembershipContext.AuthenticatedUser;

        // Check 'Manage' permission
        if (!cu.IsGroupAdministrator(mGroupId) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToAccessDenied("cms.groups", CMSAdminControl.PERMISSION_MANAGE);
        }

        messageEditElem.AdvancedMode = true;
        messageEditElem.MessageID = mMessageId;
        messageEditElem.MessageBoardID = mBoardId;

        messageEditElem.OnCheckPermissions += messageEditElem_OnCheckPermissions;

        messageEditElem.OnBeforeMessageSaved += messageEditElem_OnBeforeMessageSaved;
        messageEditElem.OnAfterMessageSaved += messageEditElem_OnAfterMessageSaved;

        // initializes page title control		
        if (mMessageId > 0)
        {
            PageTitle.TitleText = GetString("Board.MessageEdit.title");
        }
        else
        {
            PageTitle.TitleText = GetString("Board.MessageNew.title");
        }

        if (!RequestHelper.IsPostBack())
        {
            messageEditElem.ReloadData();
        }
    }


    private void messageEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check 'Manage' permission
        if (!cu.IsGroupAdministrator(mGroupId) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToAccessDenied("cms.groups", CMSAdminControl.PERMISSION_MANAGE);
        }
    }


    private void messageEditElem_OnAfterMessageSaved(BoardMessageInfo message)
    {
        int queryMarkIndex = Request.RawUrl.IndexOfCSafe('?');
        string filterParams = ScriptHelper.GetString(Request.RawUrl.Substring(queryMarkIndex));

        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBoardList(" + filterParams + ");CloseDialog();");
    }


    private void messageEditElem_OnBeforeMessageSaved()
    {
        bool isOwner = false;

        BoardMessageInfo message = BoardMessageInfoProvider.GetBoardMessageInfo(mMessageId);
        if (message != null)
        {
            // Check if the current user is allowed to modify the message
            isOwner = ((MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)) || cu.IsGroupAdministrator(mGroupId) ||
                       (BoardModeratorInfoProvider.IsUserBoardModerator(MembershipContext.AuthenticatedUser.UserID, message.MessageBoardID)) ||
                       (message.MessageUserID == MembershipContext.AuthenticatedUser.UserID));
        }

        if (!isOwner && !cu.IsGroupAdministrator(mGroupId) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToAccessDenied(GetString("board.messageedit.notallowed"));
        }
    }
}
