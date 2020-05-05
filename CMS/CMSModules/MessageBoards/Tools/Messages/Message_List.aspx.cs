using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.MessageBoards.Web.UI;
using CMS.Modules;


public partial class CMSModules_MessageBoards_Tools_Messages_Message_List : CMSMessageBoardPage
{
    private int mBoardId;
    private int mGroupId;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);

        // Check correct UI element based on specified board identifier or not
        string uiElement = (mBoardId > 0) ? "Board.Messages" : "Messages";
        if (!CurrentUser.IsAuthorizedPerUIElement("CMS.MessageBoards", uiElement))
        {
            RedirectToUIElementAccessDenied("CMS.MessageBoards", uiElement);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        messageList.IsLiveSite = false;
        messageList.BoardID = mBoardId;
        messageList.GroupID = mGroupId;
        messageList.OnAction += messageList_OnAction;

        if (mBoardId > 0)
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("Board.MessageList.NewMessage");
            action.OnClientClick = "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/MessageBoards/Tools/Messages/Message_Edit.aspx") + "?boardId=" + mBoardId + "&changemaster=" + QueryHelper.GetBoolean("changemaster", false) + "', 'MessageEdit', 360, 490); return false;";
            CurrentMaster.HeaderActions.AddAction(action);
        }
    }


    private void messageList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":

                string[] arguments = e.CommandArgument as string[];
                if (arguments != null)
                {
                    URLHelper.Redirect(UrlResolver.ResolveUrl("Message_Edit.aspx?boardId=" + mBoardId + "&messageId=" + arguments[1] + arguments[0] + ((mGroupId > 0) ? "&groupid=" + mGroupId : "")));
                }
                break;
        }
    }
}