using System;

using CMS.Helpers;
using CMS.MessageBoards;
using CMS.MessageBoards.Web.UI;


public partial class CMSModules_MessageBoards_Tools_Boards_Board_Edit_General : CMSMessageBoardBoardsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get board ID
        int boardId = QueryHelper.GetInteger("boardId", 0);

        // Get board info and check whether it belongs to current site
        BoardInfo board = BoardInfoProvider.GetBoardInfo(boardId);
        if (board != null)
        {
            CheckMessageBoardSiteID(board.BoardSiteID);
        }

        // Set board info to editing control
        boardEdit.Board = board;
    }
}