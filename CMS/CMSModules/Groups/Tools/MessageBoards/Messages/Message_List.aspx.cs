using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_MessageBoards_Messages_Message_List : CMSGroupMessageBoardsPage
{
    #region "Variables"

    private int mBoardId;
    private int mGroupId;
    private BoardInfo boardObj;

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mGroupId = QueryHelper.GetInteger("groupId", 0);

        boardObj = BoardInfoProvider.GetBoardInfo(mBoardId);
        if (boardObj != null)
        {
            mGroupId = boardObj.BoardGroupID;

            // Check whether edited board belongs to any group
            if (mGroupId == 0)
            {
                EditedObject = null;
            }
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        messageList.BoardID = mBoardId;
        messageList.GroupID = mGroupId;
        messageList.EditPageUrl = "~/CMSModules/Groups/Tools/MessageBoards/Messages/Message_Edit.aspx";
        messageList.OnCheckPermissions += messageList_OnCheckPermissions;
        messageList.OnAction += messageList_OnAction;

        if (mBoardId > 0)
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("Board.MessageList.NewMessage");
            action.OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/Groups/Tools/MessageBoards/Messages/Message_Edit.aspx") + "?boardId=" + mBoardId + "', 'MessageEdit', 360, 490); return false;";
            CurrentMaster.HeaderActions.AddAction(action);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            messageList.ReloadData();
        }
    }


    private void messageList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":

                string[] arguments = e.CommandArgument as string[];
                URLHelper.Redirect(UrlResolver.ResolveUrl("Message_Edit.aspx?boardId=" + mBoardId + "&messageId=" + arguments[1] + arguments[0] + ((mGroupId > 0) ? "&groupid=" + mGroupId : "")));
                break;
        }
    }


    private void messageList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check 'Manage' permission
        CheckGroupPermissions(messageList.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }
}