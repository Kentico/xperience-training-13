using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Tools_Messages_Message_Edit : CMSModalPage
{
    private int mBoardId;
    private int mMessageId;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Check permissions for CMS Desk -> Tools -> MessageBoards
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MessageBoards", "MessageBoards"))
        {
            RedirectToUIElementAccessDenied("CMS.MessageBoards", "MessageBoards");
        }

        // Check permissions for MessageBoards -> Messages
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MessageBoards", "Messages"))
        {
            RedirectToUIElementAccessDenied("CMS.MessageBoards", "Messages");
        }

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);

        if ((BoardInfoProvider.GetBoardInfo(mBoardId) == null) || ((mMessageId > 0) && (BoardMessageInfoProvider.GetBoardMessageInfo(mMessageId) == null)))
        {
            RedirectToInformation("editedobject.notexists");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'Read' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.messageboards", CMSAdminControl.PERMISSION_READ))
        {
            RedirectToAccessDenied("cms.messageboards", CMSAdminControl.PERMISSION_READ);
        }

        messageEditElem.IsLiveSite = false;
        messageEditElem.AdvancedMode = true;
        messageEditElem.MessageID = mMessageId;
        messageEditElem.MessageBoardID = mBoardId;
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
    }


    private void messageEditElem_OnAfterMessageSaved(BoardMessageInfo message)
    {
        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBoardList(); CloseDialog();");
    }


    private void messageEditElem_OnBeforeMessageSaved()
    {
        bool isOwner = false;

        BoardInfo board = BoardInfoProvider.GetBoardInfo(mBoardId);
        if (board != null)
        {
            // Check if the current user is allowed to modify the message
            isOwner = BoardInfoProvider.IsUserAuthorizedToManageMessages(board);
        }

        if (!isOwner && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.MessageBoards", CMSAdminControl.PERMISSION_MODIFY))
        {
            RedirectToAccessDenied("cms.messageboards", CMSAdminControl.PERMISSION_MODIFY);
        }
    }
}
