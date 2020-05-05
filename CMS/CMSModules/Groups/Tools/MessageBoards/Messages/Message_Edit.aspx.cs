using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_MessageBoards_Messages_Message_Edit : CMSModalPage
{
    private int mBoardId;
    private int mMessageId;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Check permissions for CMS Desk -> Tools -> Groups
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Groups", "Groups"))
        {
            RedirectToUIElementAccessDenied("CMS.Groups", "Groups");
        }

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'Read' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_READ))
        {
            RedirectToAccessDenied("cms.groups", CMSAdminControl.PERMISSION_READ);
        }

        messageEditElem.IsLiveSite = false;
        messageEditElem.AdvancedMode = true;
        messageEditElem.MessageID = mMessageId;
        messageEditElem.MessageBoardID = mBoardId;

        messageEditElem.OnCheckPermissions += messageEditElem_OnCheckPermissions;

        messageEditElem.OnBeforeMessageSaved += messageEditElem_OnBeforeMessageSaved;
        messageEditElem.OnAfterMessageSaved += messageEditElem_OnAfterMessageSaved;

        // initializes page title control		
        PageTitle.TitleText = GetString(mMessageId > 0 ? "Board.MessageEdit.title" : "Board.MessageNew.title");
    }


    private void messageEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckLocalPermissions();
    }


    protected void CheckLocalPermissions()
    {
        int groupId = 0;
        int boardId = mBoardId;

        BoardMessageInfo bmi = BoardMessageInfoProvider.GetBoardMessageInfo(mMessageId);
        if (bmi != null)
        {
            boardId = bmi.MessageBoardID;
        }

        BoardInfo bi = BoardInfoProvider.GetBoardInfo(boardId);
        if (bi != null)
        {
            groupId = bi.BoardGroupID;
        }

        // Check 'Manage' permission
        if (MembershipContext.AuthenticatedUser.IsGroupAdministrator(groupId))
        {
            return;
        }

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
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
        CheckLocalPermissions();
    }
}
