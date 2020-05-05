using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_CMSPages_Message_Edit : CMSLiveModalPage
{
    private int mBoardId;
    private int mMessageId;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("messageboardid", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        messageEditElem.AdvancedMode = true;
        messageEditElem.CheckFloodProtection = true;
        messageEditElem.MessageID = mMessageId;
        messageEditElem.MessageBoardID = mBoardId;
        messageEditElem.OnBeforeMessageSaved += messageEditElem_OnBeforeMessageSaved;
        messageEditElem.OnAfterMessageSaved += messageEditElem_OnAfterMessageSaved;

        // initializes page title control		
        PageTitle.TitleText = GetString(mMessageId > 0 ? "Board.MessageEdit.title" : "Board.MessageNew.title");

        if (!RequestHelper.IsPostBack())
        {
            messageEditElem.ReloadData();
        }
    }


    private void messageEditElem_OnAfterMessageSaved(BoardMessageInfo message)
    {
        int queryMarkIndex = Request.RawUrl.IndexOfCSafe('?');
        string filterParams = ScriptHelper.GetString(Request.RawUrl.Substring(queryMarkIndex));

        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBoardList(" + filterParams + "); CloseDialog();");
    }


    private void messageEditElem_OnBeforeMessageSaved()
    {
        bool isOwner = false;

        BoardInfo board = BoardInfoProvider.GetBoardInfo(messageEditElem.MessageBoardID);
        if (board != null)
        {
            // Check if the current user is allowed to modify the message
            isOwner = BoardInfoProvider.IsUserAuthorizedToManageMessages(board);
        }

        if (!isOwner && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.MessageBoards", "Modify"))
        {
            RedirectToAccessDenied(GetString("board.messageedit.notallowed"));
        }
    }
}
