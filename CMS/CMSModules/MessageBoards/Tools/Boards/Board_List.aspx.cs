using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MessageBoards.Web.UI;


public partial class CMSModules_MessageBoards_Tools_Boards_Board_List : CMSMessageBoardBoardsPage
{
    private int mGroupId;


    protected override void OnPreInit(EventArgs e)
    {
        if (mGroupId > 0)
        {
            Page.MasterPageFile = "~/CMSMasterPages/UI/SimplePage.master";
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        mGroupId = QueryHelper.GetInteger("groupid", 0);

        boardList.IsLiveSite = false;
        boardList.GroupID = mGroupId;
        boardList.OnAction += boardList_OnAction;
    }


    private void boardList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":
                int boardId = ValidationHelper.GetInteger(e.CommandArgument, 0);

                // Create a target site URL and pass the category ID as a parameter
                string editUrl = UIContextHelper.GetElementUrl("CMS.MessageBoards", "EditBoards");
                editUrl = URLHelper.AddParameterToUrl(editUrl, "objectid", boardId.ToString());
                editUrl = URLHelper.AddParameterToUrl(editUrl, "displaytitle", "false");
                URLHelper.Redirect(editUrl);
                break;
        }
    }
}