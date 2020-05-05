using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_Boards_BoardList : CMSAdminListControl
{
    #region "Variables"

    private int mGroupId;
    private string mGridName = "";

    #endregion


    #region "Public properties"

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
    /// Current group ID.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }


    /// <summary>
    /// UniGrid XML definition.
    /// </summary>
    public string GridName
    {
        get
        {
            return mGridName;
        }
        set
        {
            mGridName = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Initialize the controls
        SetupControls();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            return;
        }

        // Hide document link column when placed on LiveSite
        if (IsLiveSite)
        {
            gridBoards.GridView.Columns[7].Visible = false;
        }

        // Display info message
        if (GroupID != 0)
        {
            if (!RequestHelper.IsPostBack() && gridBoards.IsEmpty)
            {
                ShowInformation(GetString("messageboards.board_list.groupinfo"));
                txtBoardName.Enabled = false;
                btnFilter.Enabled = false;
            }
        }
        else
        {
            ShowInformation(GetString("messageboards.board_list.info"));
        }

        ScriptHelper.RegisterModule(this, "CMS.MessageBoards/MessageBoardsGrid", new
        {
            GridSelector = "#" + gridBoards.ClientID,
            PagesApplicationHash = ApplicationUrlHelper.GetApplicationHash("cms.content", "content"),
        });
    }


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        // Filter and reload data
        ReloadData();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        // Clear and reload data
        ClearFilter();
        ReloadData();
    }

    #endregion


    #region "UniGrid handling"

    protected object gridBoards_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "enabled":
                bool enabled = ValidationHelper.GetBoolean(parameter, false);

                return UniGridFunctions.ColoredSpanYesNo(enabled);

            case "opened":
                bool opened = IsBoardOpened((DataRowView)parameter);

                return UniGridFunctions.ColoredSpanYesNo(opened);

            case "moderated":
                bool moderated = ValidationHelper.GetBoolean(parameter, false);

                return UniGridFunctions.ColoredSpanYesNo(moderated);

            case "document":
                DataRowView dr = parameter as DataRowView;
                if (dr != null)
                {
                    string documentName = ValidationHelper.GetString(dr["DocumentName"], "");
                    int siteId = ValidationHelper.GetInteger(dr["NodeSiteID"], 0);
                    SiteInfo site = SiteInfoProvider.GetSiteInfo(siteId);

                    if (string.IsNullOrEmpty(documentName))
                    {
                        documentName = GetString("general.root");
                    }

                    if (site.Status == SiteStatusEnum.Running)
                    {
                        int nodeID = ValidationHelper.GetInteger(dr["NodeID"], 0);
                        string culture = ValidationHelper.GetString(dr["DocumentCulture"], "");

                        return "<a class=\"js-unigrid-action js-viewDocument\"" +
                               "href=\"javascript:void(0)\" " +
                               "data-node-id=\"" + nodeID + "\" " +
                               "data-document-culture=\"" + culture + "\">" + HTMLHelper.HTMLEncode(documentName) + "</a>";
                    }
                }
                return "";

            case "lastpost":
                DateTime lastPost = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);

                return lastPost == DateTimeHelper.ZERO_TIME ? GetString("general.na") : TimeZoneUIMethods.ConvertDateTime(lastPost, this).ToString();

            default:
                return "";
        }
    }


    protected void gridBoards_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
                {
                    return;
                }

                int boardId = ValidationHelper.GetInteger(actionArgument, 0);

                // If no Document-Category relationship exist concerning current category
                if (boardId > 0)
                {
                    BoardInfoProvider.DeleteBoardInfo(boardId);
                }

                if (IsLiveSite)
                {
                    ReloadData();
                }
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }


    protected void gridBoards_OnBeforeDataReload()
    {
        string boardName = txtBoardName.Text.Replace("'", "''").Trim();
        gridBoards.WhereClause = string.IsNullOrEmpty(boardName) ? null : string.Format("(BoardDisplayName LIKE '%{0}%')", boardName);
    }


    protected DataSet gridBoards_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        // Prepare where condition
        completeWhere = SqlHelper.AddWhereCondition(completeWhere, "(BoardSiteID = " + SiteContext.CurrentSite.SiteID + ")");
        if (mGroupId != 0)
        {
            completeWhere = SqlHelper.AddWhereCondition(completeWhere, "(BoardGroupID =" + mGroupId + ")");
        }
        else
        {
            completeWhere = SqlHelper.AddWhereCondition(completeWhere, "(BoardGroupID IS NULL OR BoardGroupID = 0)");
        }


        // Get boards
        DataSet ds = BoardInfoProvider.GetMessageBoards(completeWhere, currentOrder, currentTopN, columns, true);

        totalRecords = DataHelper.GetItemsCount(ds);
        return ds;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControls()
    {
        lblBoardName.Text = GetString("board.boardlist.boardname");
        btnFilter.Text = GetString("general.search");
        btnReset.Text = GetString("general.reset");

        gridBoards.IsLiveSite = IsLiveSite;
        gridBoards.GridName = (!string.IsNullOrEmpty(GridName)) ? GridName : "~/CMSModules/MessageBoards/Tools/Boards/Board_List.xml";
        gridBoards.OrderBy = "BoardDisplayName ASC";
        gridBoards.IsLiveSite = IsLiveSite;
        gridBoards.OnAction += gridBoards_OnAction;
        gridBoards.OnExternalDataBound += gridBoards_OnExternalDataBound;
        gridBoards.OnDataReload += gridBoards_OnDataReload;
        gridBoards.OnBeforeDataReload += gridBoards_OnBeforeDataReload;
        gridBoards.ZeroRowsText = GetString("general.nodatafound");
        gridBoards.ShowActionsMenu = true;
        gridBoards.Columns = "BoardID, BoardDisplayName, BoardEnabled, BoardModerated, BoardMessages, BoardLastMessageTime, BoardDocumentID, BoardOpened, NodeID, NodeSiteID, DocumentName, DocumentCulture, ClassName";

        // Get all possible column names.
        gridBoards.AllColumns = SqlHelper.JoinColumnList(ObjectTypeManager.GetColumnNames(BoardInfo.OBJECT_TYPE, PredefinedObjectType.NODE, PredefinedObjectType.DOCUMENTLOCALIZATION));
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        gridBoards.ReloadData();
    }


    /// <summary>
    /// Checks if the board is currently opened.
    /// </summary>
    /// <param name="drv">Data row view holding information on current board data</param>
    private bool IsBoardOpened(DataRowView drv)
    {
        bool opened = ValidationHelper.GetBoolean(drv["BoardOpened"], false);
        DateTime from = ValidationHelper.GetDateTime(drv["BoardOpened"], DateTimeHelper.ZERO_TIME);
        DateTime to = ValidationHelper.GetDateTime(drv["BoardOpened"], DateTimeHelper.ZERO_TIME);
        return BoardInfoProvider.IsBoardOpened(opened, from, to);
    }


    /// <summary>
    /// Clears the filter up.
    /// </summary>
    public void ClearFilter()
    {
        txtBoardName.Text = "";
    }

    #endregion
}
