using System;

using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_MessageBoards_Boards_Board_Edit_Security : CMSGroupMessageBoardsPage
{
    #region "Variables"

    protected int boardId = 0;
    private int mGroupId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get parametr from query string
        boardId = QueryHelper.GetInteger("boardid", 0);
        BoardInfo bi = BoardInfoProvider.GetBoardInfo(boardId);
        if (bi != null)
        {
            mGroupId = bi.BoardGroupID;

            // Check whether edited board belongs to any group
            if (mGroupId == 0)
            {
                EditedObject = null;
            }
        }

        boardSecurity.BoardID = boardId;
        boardSecurity.GroupID = mGroupId;

        boardSecurity.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(boardSecurity_OnCheckPermissions);
    }

    #endregion


    #region "Events"

    private void boardSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check 'Manage' permission
        int groupId = 0;
        BoardInfo bi = BoardInfoProvider.GetBoardInfo(boardId);
        if (bi != null)
        {
            groupId = bi.BoardGroupID;
        }

        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }

    #endregion
}