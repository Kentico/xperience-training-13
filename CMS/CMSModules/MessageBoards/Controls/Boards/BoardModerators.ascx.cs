using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_Boards_BoardModerators : CMSAdminEditControl
{
    #region "Variables"

    protected int mBoardID;
    protected BoardInfo mBoard;
    private string currentValues = String.Empty;
    private bool canModify;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// ID of the current message board.
    /// </summary>
    public int BoardID
    {
        get
        {
            if (mBoard != null)
            {
                return mBoard.BoardID;
            }

            return mBoardID;
        }
        set
        {
            mBoardID = value;

            mBoard = null;
        }
    }


    /// <summary>
    /// Current message board info object.
    /// </summary>
    public BoardInfo Board
    {
        get
        {
            return mBoard ?? (mBoard = BoardInfoProvider.GetBoardInfo(BoardID));
        }
        set
        {
            mBoard = value;

            mBoardID = 0;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether the data should be reloaded on PreRender.
    /// </summary>
    private bool ShouldReloadData
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Initializes the controls
        SetupControls();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Reload data if necessary
        if (ShouldReloadData || (!RequestHelper.IsPostBack() && !IsLiveSite))
        {
            currentValues = "";
            userSelector.CurrentValues = GetModerators();

            ReloadData();
        }

        if (Board != null)
        {
            userSelector.Enabled = Board.BoardModerated && canModify;
            chkBoardModerated.Enabled = canModify;
        }
    }


    private void SetupControls()
    {
        // Get resource strings
        userSelector.CurrentSelector.OnSelectionChanged += CurrentSelector_OnSelectionChanged;

        if (BoardID > 0)
        {
            EditedObject = Board;
        }

        if (Board == null)
        {
            return;
        }

        // Check permissions
        if (Board.BoardGroupID > 0)
        {
            canModify = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", PERMISSION_MANAGE);
        }
        else
        {
            canModify = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.messageboards", PERMISSION_MODIFY);
        }
            
        userSelector.BoardID = BoardID;
        userSelector.GroupID = Board.BoardGroupID;
        userSelector.CurrentSelector.SelectionMode = SelectionModeEnum.Multiple;
        userSelector.ShowSiteFilter = false;
        userSelector.SiteID = SiteContext.CurrentSiteID;
        userSelector.CurrentValues = GetModerators();
        userSelector.IsLiveSite = IsLiveSite;
    }


    /// <summary>
    /// Reloads form data.
    /// </summary>
    public override void ReloadData()
    {
        ReloadData(true);
    }


    /// <summary>
    /// Reloads form data.
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        base.ReloadData(forceReload);

        // Get board info
        if (Board == null)
        {
            return;
        }

        chkBoardModerated.Checked = Board.BoardModerated;
        if (forceReload)
        {
            if (!String.IsNullOrEmpty(currentValues))
            {
                string where = SqlHelper.AddWhereCondition(userSelector.CurrentSelector.WhereCondition, "UserID IN (" + currentValues.Replace(';', ',') + ")", "OR");
                userSelector.CurrentSelector.WhereCondition = @where;
            }

            userSelector.CurrentSelector.Value = GetModerators();
            userSelector.ReloadData();
        }
    }


    /// <summary>
    /// Returns ID of users who are moderators to this board.
    /// </summary>
    protected string GetModerators()
    {
        if (!String.IsNullOrEmpty(currentValues))
        {
            return currentValues;
        }
        
        var userIds = UserInfoProvider.GetUsers()
                    .Column("UserID")
                    .WhereIn("UserID", new IDQuery<BoardModeratorInfo>("UserID").WhereEquals("BoardID", BoardID))
                    .GetListResult<int>();

        if (userIds.Any())
        {
            currentValues = String.Join(";", userIds);
        }

        return currentValues;
    }


    /// <summary>
    /// Board moderated checkbox change.
    /// </summary>
    protected void chkBoardModerated_CheckedChanged(object sender, EventArgs e)
    {
        if (!canModify || (Board == null))
        {
            return;
        }
        
        Board.BoardModerated = chkBoardModerated.Checked;
        BoardInfoProvider.SetBoardInfo(Board);

        ShowChangesSaved();

        ShouldReloadData = true;
    }


    private void CurrentSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        ShouldReloadData = true;
    }
}
