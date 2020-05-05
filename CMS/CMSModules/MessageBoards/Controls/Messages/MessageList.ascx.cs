using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_Messages_MessageList : CMSAdminListControl
{
    #region "Private variables"

    private int mGroupId = 0;
    protected string mPostBackRefference = "";
    private int siteId = 0;


    public CMSModules_MessageBoards_Controls_Messages_MessageList()
    {
        ShowFilter = true;
        EditPageUrl = "";
        IsSpam = "all";
        IsApproved = "no";
        OrderBy = String.Empty;
        BoardID = 0;
        ItemsPerPage = String.Empty;
        AllowMassActions = true;
        SiteName = String.Empty;
        HideWhenGroupIsNotSupplied = false;
        ShowPermissionMessage = false;
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Determines whether to hide the content of the control when group ID is not supplied.
    /// </summary>
    public bool HideWhenGroupIsNotSupplied
    {
        get;
        set;
    }


    /// <summary>
    /// If true show permission error.
    /// </summary>
    public bool ShowPermissionMessage
    {
        get;
        set;
    }


    /// <summary>
    /// Site name of blogs.
    /// </summary>
    public string SiteName
    {
        get;
        set;
    }


    /// <summary>
    /// If true mass actions are allowed.
    /// </summary>
    public bool AllowMassActions
    {
        get;
        set;
    }


    /// <summary>
    /// Items per page.
    /// </summary>
    public string ItemsPerPage
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the current board.
    /// </summary>
    public int BoardID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of board selected in filter UI.
    /// </summary>
    public int SelectedBoardID
    {
        get
        {
            return (BoardID > 0) ? BoardID : ValidationHelper.GetInteger(boardSelector.Value, 0);
        }
    }


    /// <summary>
    /// Order by.
    /// </summary>
    public string OrderBy
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether shown comments are approved.
    /// </summary>
    public string IsApproved
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether show spam marked comments.
    /// </summary>
    public string IsSpam
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the current group.
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
            boardSelector.GroupID = value;
        }
    }


    /// <summary>
    /// Target URL for the modal dialog message is edited in.
    /// </summary>
    public string EditPageUrl
    {
        get;
        set;
    }


    /// <summary>
    /// If true filter is shown.
    /// </summary>
    public bool ShowFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is used on live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            headerActions.IsLiveSite = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns selected site ID.
    /// </summary>
    private int SelectedSiteID
    {
        get
        {
            Control postbackControl = ControlsHelper.GetPostBackControl(Page);
            int val = ValidationHelper.GetInteger((postbackControl == btnFilter) ? siteSelector.Value : ViewState["SelectedSiteID"], SiteContext.CurrentSiteID);

            return val;
        }
        set
        {
            ViewState["SelectedSiteID"] = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    private string WhereCondition
    {
        get
        {
            Control postbackControl = ControlsHelper.GetPostBackControl(Page);
            return DataHelper.GetNotEmpty((postbackControl == btnFilter) ? GetFilterWhereCondition() : ViewState["WhereCondition"], GetFilterWhereCondition());
        }
        set
        {
            ViewState["WhereCondition"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if the group was supplied and hide control if required
        if ((GroupID == 0) && (HideWhenGroupIsNotSupplied))
        {
            Visible = false;
        }

        // If control should be hidden save view state memory
        if (StopProcessing || !Visible)
        {
            EnableViewState = false;
        }

        if (!ShowFilter)
        {
            plcActions.Visible = false;
        }

        SetContext();

        // Initializes the controls
        SetupControls();

        ReleaseContext();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource))
        {
            lblActions.Visible = false;
            drpActions.Visible = false;
            btnOk.Visible = false;
        }
        else
        {
            // Hide column containing board name when reviewing specific board
            if (BoardID > 0)
            {
                gridElem.NamedColumns["BoardName"].Visible = false;
            }

            // Hide column containing site name when specific site filtered
            if (!plcSite.Visible || (SelectedSiteID > 0))
            {
                gridElem.NamedColumns["SiteName"].Visible = false;
            }

            if (ShowPermissionMessage)
            {
                messageElem.Visible = true;
                messageElem.ErrorMessage = GetString("general.nopermission");
            }

            lblActions.Visible = true;
            drpActions.Visible = true;
            btnOk.Visible = true;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControls()
    {
        btnFilter.Text = GetString("general.search");
        btnOk.Text = GetString("general.ok");

        // Mass actions
        gridElem.GridOptions.ShowSelection = AllowMassActions;
        plcActions.Visible = AllowMassActions;

        lblSiteName.AssociatedControlClientID = siteSelector.DropDownSingleSelect.ClientID;
        lblBoardName.AssociatedControlClientID = boardSelector.DropDownSingleSelect.ClientID;

        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.ZeroRowsText = GetString("general.nodatafound");

        btnOk.OnClientClick += "return MassConfirm('" + drpActions.ClientID + "'," + ScriptHelper.GetString(GetString("General.ConfirmGlobalDelete")) + ");";

        ScriptHelper.RegisterDialogScript(Page);

        if (GroupID == 0)
        {
            GroupID = QueryHelper.GetInteger("groupid", 0);
        }

        ReloadFilter();

        if (!RequestHelper.IsPostBack())
        {
            var defaultApproved = "NO";

            if (SelectedBoardID > 0)
            {
                // Check if board is moderated
                var boardInfo = BoardInfoProvider.GetBoardInfo(SelectedBoardID);
                if ((boardInfo != null) && (!boardInfo.BoardModerated))
                {
                    defaultApproved = "";
                }
            }

            // Preselect filter data
            PreselectFilter(String.Format(";;{0};;;{1};;", GroupID, defaultApproved));
        }

        if (GroupID > 0)
        {
            // Hide site selection
            plcSite.Visible = false;
        }

        if (BoardID > 0)
        {
            // Hide board selection
            plcBoard.Visible = false;

            // Hide site selection
            plcSite.Visible = false;

            if ((GroupID > 0) && IsLiveSite)
            {
                InitializeGroupNewMessage();
            }
        }

        siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        // Reload message list script        
        string board = (BoardID > 0 ? BoardID.ToString() : boardSelector.Value.ToString());
        string group = GroupID.ToString();
        string user = HTMLHelper.HTMLEncode(txtUserName.Text);
        string message = HTMLHelper.HTMLEncode(txtMessage.Text);
        string approved = drpApproved.SelectedItem.Value;
        string spam = drpSpam.SelectedItem.Value;
        bool changemaster = QueryHelper.GetBoolean("changemaster", false);

        // Set site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.AllowAll = true;
        siteSelector.IsLiveSite = IsLiveSite;

        boardSelector.IsLiveSite = IsLiveSite;
        boardSelector.GroupID = GroupID;

        if (!ShowFilter)
        {
            SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteName);
            if (si != null)
            {
                siteId = si.SiteID;
            }
            if (SiteName == TreeProvider.ALL_SITES)
            {
                siteId = -1;
            }
        }
        else
        {
            // Collect values from filter controls
            siteId = SelectedSiteID;
            IsApproved = drpApproved.SelectedValue.ToLowerCSafe();
            IsSpam = drpSpam.SelectedValue.ToLowerCSafe();
        }


        if (siteId == 0)
        {
            siteId = SiteContext.CurrentSiteID;
            siteSelector.Value = siteId;
        }

        SelectedSiteID = siteId;

        string cmdArg = siteId + ";" + board + ";" + group + ";" + user.Replace(";", "#sc#") + ";" +
                        message.Replace(";", "#sc#") + ";" + approved + ";" + spam + ";" + changemaster;

        btnRefreshHdn.CommandArgument = cmdArg;
        mPostBackRefference = ControlsHelper.GetPostBackEventReference(btnRefreshHdn, null);
        ScriptHelper.RegisterClientScriptBlock(this, typeof (string), "RefreshBoardList", ScriptHelper.GetScript("function RefreshBoardList(){" +
                                                                                                                 mPostBackRefference + "}"));

        siteSelector.OnlyRunningSites = IsLiveSite;

        gridElem.WhereCondition = GetWhereCondition();

        if ((!RequestHelper.IsPostBack()) && (!string.IsNullOrEmpty(ItemsPerPage)))
        {
            gridElem.Pager.DefaultPageSize = ValidationHelper.GetInteger(ItemsPerPage, -1);
        }

        if (!String.IsNullOrEmpty(OrderBy))
        {
            gridElem.OrderBy = OrderBy;
        }
    }


    /// <summary>
    /// Creates where condition reflecting filter settings.
    /// </summary>
    private string GetFilterWhereCondition()
    {
        string where = String.Empty;

        // Sites dropdown list
        if (SelectedSiteID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "BoardSiteID = " + SelectedSiteID);
        }

        // Approved dropdown list
        switch (IsApproved.ToLowerCSafe())
        {
            case "yes":
                where = SqlHelper.AddWhereCondition(where, "MessageApproved = 1");
                break;

            case "no":
                where = SqlHelper.AddWhereCondition(where, "MessageApproved = 0");
                break;
        }

        // Spam dropdown list
        switch (IsSpam.ToLowerCSafe())
        {
            case "yes":
                where = SqlHelper.AddWhereCondition(where, "MessageIsSpam = 1");
                break;

            case "no":
                where = SqlHelper.AddWhereCondition(where, "MessageIsSpam = 0");
                break;
        }

        if (BoardID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "MessageBoardID = " + BoardID.ToString());
        }
        else
        {
            // Board dropdown list
            if (SelectedBoardID > 0)
            {
                where = SqlHelper.AddWhereCondition(where, "MessageBoardID = " + SelectedBoardID);
            }
            else if (GroupID > 0)
            {
                where = SqlHelper.AddWhereCondition(where, "BoardGroupID =" + GroupID);
            }
            else
            {
                where = SqlHelper.AddWhereCondition(where, "BoardGroupID IS NULL");
            }
        }

        if (txtUserName.Text.Trim() != "")
        {
            where = SqlHelper.AddWhereCondition(where, "MessageUserName LIKE '%" + txtUserName.Text.Trim().Replace("'", "''") + "%'");
        }

        if (txtMessage.Text.Trim() != "")
        {
            where = SqlHelper.AddWhereCondition(where, "MessageText LIKE '%" + txtMessage.Text.Trim().Replace("'", "''") + "%'");
        }

        return where;
    }


    /// <summary>
    /// Creates where condition based on data access permissions.
    /// </summary>
    private string GetWhereCondition()
    {
        string where = String.Empty;

        bool isAuthorized = false;

        if (SelectedBoardID > 0)
        {
            BoardInfo selectedBoard = BoardInfoProvider.GetBoardInfo(SelectedBoardID);
            if (selectedBoard != null)
            {
                isAuthorized = BoardInfoProvider.IsUserAuthorizedToManageMessages(selectedBoard);
            }
        }

        // Show messages to boards only where user is moderator
        if (!isAuthorized && (!(MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.MessageBoards", "Modify") || MembershipContext.AuthenticatedUser.IsGroupAdministrator(GroupID))))
        {
            where = SqlHelper.AddWhereCondition(where, "BoardID IN (SELECT BoardID FROM Board_Moderator WHERE Board_Moderator.UserID = " + MembershipContext.AuthenticatedUser.UserID + " )");
        }

        // Group restriction
        if (GroupID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "BoardGroupID =" + mGroupId);
        }
        else
        {
            where = SqlHelper.AddWhereCondition(where, "(BoardGroupID = 0 OR BoardGroupID IS NULL)");
        }

        // Site restriction
        if (SelectedSiteID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "BoardSiteID = " + SelectedSiteID);
        }

        return where;
    }


    /// <summary>
    /// Initializes New message action for group message board.
    /// </summary>
    private void InitializeGroupNewMessage()
    {
        plcNewMessageGroups.Visible = true;

        HeaderAction action = new HeaderAction();
        action.Text = GetString("Board.MessageList.NewMessage");
        action.OnClientClick = "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/Message_Edit.aspx") + "?boardId=" + BoardID + "&groupid=" + GroupID + "&changemaster=" + QueryHelper.GetBoolean("changemaster", false) + "', 'MessageEdit', 800, 535); return false;";
        headerActions.AddAction(action);
    }


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        WhereCondition = GetFilterWhereCondition();
        SelectedSiteID = ValidationHelper.GetInteger(siteSelector.Value, SiteContext.CurrentSiteID);
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        boardSelector.SiteID = ValidationHelper.GetInteger(siteSelector.Value, 0);
        boardSelector.ReloadData(true);

        ReloadData();
    }


    /// <summary>
    /// Gets the information on last selected filter configuration and pre-selects the actual values.
    /// </summary>
    private void PreselectFilter(string filter)
    {
        string site = "";
        string board = "";
        string username = "";
        string message = "";
        string approved = "no";
        string isspam = "";

        string filterParams = HttpUtility.HtmlDecode(filter);

        if (!string.IsNullOrEmpty(filterParams))
        {
            string[] paramsArr = filterParams.Split(';');
            site = paramsArr[0];
            board = paramsArr[1];
            GroupID = ValidationHelper.GetInteger(paramsArr[2], 0);
            username = paramsArr[3].Replace("#sc#", ";");
            message = paramsArr[4].Replace("#sc#", ";");
            approved = paramsArr[5];
            isspam = paramsArr[6];
        }

        // Get filter values from the query string by default 
        // or ViewState if the control is used as part of GroupProfile on Live site
        site = QueryHelper.GetString("site", site);
        board = QueryHelper.GetString("board", board);
        username = QueryHelper.GetString("username", username);
        message = QueryHelper.GetString("message", message);
        approved = QueryHelper.GetString("approved", approved);
        isspam = QueryHelper.GetString("isspam", isspam);

        // Ensure site id
        if (site != "")
        {
            siteId = ValidationHelper.GetInteger(site, 0);
            siteSelector.Value = siteId;
        }
        else
        {
            if (BoardID == 0)
            {
                siteId = SiteContext.CurrentSiteID;
                siteSelector.Value = siteId;
            }
        }

        if (board != "")
        {
            if (boardSelector.Visible && boardSelector.UniSelector.HasData)
            {
                boardSelector.Value = board;
            }
        }

        // Show user name
        if (username != "")
        {
            txtUserName.Text = username;
        }

        // Show message
        if (message != "")
        {
            txtMessage.Text = message;
        }

        if (approved != "")
        {
            if (drpApproved.Items.Count > 0)
            {
                drpApproved.SelectedValue = approved;
            }
        }

        if (isspam != "")
        {
            if (drpSpam.Items.Count > 0)
            {
                drpSpam.SelectedValue = isspam;
            }
        }
    }


    /// <summary>
    /// Load data according to filter settings.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        gridElem.ReloadData();
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Sets the filter default values.
    /// </summary>
    public void ReloadFilter()
    {
        if (GroupID == 0)
        {
            plcSite.Visible = MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
        }

        if (BoardID == 0)
        {
            // Init board selector
            boardSelector.IsLiveSite = IsLiveSite;
            boardSelector.SiteID = SiteContext.CurrentSiteID;
            boardSelector.GroupID = GroupID;
            boardSelector.ReloadData(false);
        }

        if (drpApproved.Items.Count == 0)
        {
            // Fill is approved DDL
            drpApproved.Items.Add(new ListItem(GetString("general.selectall"), "ALL"));
            drpApproved.Items.Add(new ListItem(GetString("general.yes"), "YES"));
            drpApproved.Items.Add(new ListItem(GetString("general.no"), "NO"));
            drpApproved.SelectedIndex = 0;
        }

        if (drpSpam.Items.Count == 0)
        {
            // Fill is span DDL
            drpSpam.Items.Add(new ListItem(GetString("general.selectall"), "ALL"));
            drpSpam.Items.Add(new ListItem(GetString("general.yes"), "YES"));
            drpSpam.Items.Add(new ListItem(GetString("general.no"), "NO"));
            drpSpam.SelectedIndex = 0;
        }

        if (drpActions.Items.Count == 0)
        {
            // Fill actions DDL
            drpActions.Items.Add(new ListItem(GetString("Board.MessageList.Action.Select"), "SELECT"));
            drpActions.Items.Add(new ListItem(GetString("Board.MessageList.Action.Approve"), "APPROVE"));
            drpActions.Items.Add(new ListItem(GetString("Board.MessageList.Action.Reject"), "REJECT"));
            drpActions.Items.Add(new ListItem(GetString("Board.MessageList.Action.Spam"), "SPAM"));
            drpActions.Items.Add(new ListItem(GetString("Board.MessageList.Action.NoSpam"), "NOSPAM"));
            drpActions.Items.Add(new ListItem(GetString("Board.MessageList.Action.Delete"), "DELETE"));
        }
    }

    #endregion


    #region "Event handlers"

    protected void btnRefreshHdn_Command(object sender, CommandEventArgs e)
    {
        // Preselect filter and reload data
        PreselectFilter(Convert.ToString(e.CommandArgument));
        ReloadData();
    }


    protected void gridElem_OnBeforeDataReload()
    {
        gridElem.WhereCondition = WhereCondition;
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "messageisspam":
                return UniGridFunctions.ColoredSpanYesNoReversed(parameter);

            case "edit":
                CMSGridActionButton editButton = ((CMSGridActionButton)sender);
                int boardID = ValidationHelper.GetInteger(((DataRowView) ((GridViewRow) parameter).DataItem).Row["BoardID"], 0);

                string url = "~/CMSModules/MessageBoards/Tools/Messages/Message_Edit.aspx";
                if (IsLiveSite)
                {
                    url = "~/CMSModules/MessageBoards/CMSPages/Message_Edit.aspx";
                }

                editButton.OnClientClick = "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl(((EditPageUrl == "") ? url : EditPageUrl)) +
                                           "?boardid=" + boardID + "&messageId=" + editButton.CommandArgument + "', 'MessageEdit', 800, 535); return false;";
                break;

            case "approve":
                bool approve = ValidationHelper.GetBoolean(((DataRowView) ((GridViewRow) parameter).DataItem).Row["MessageApproved"], false);
                if (!approve)
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);
                    button.IconCssClass = "icon-check-circle";
                    button.IconStyle = GridIconStyle.Allow;
                    button.ToolTip = GetString("general.approve");
                }
                else
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);
                    button.IconCssClass = "icon-times-circle";
                    button.IconStyle = GridIconStyle.Critical;
                    button.ToolTip = GetString("general.reject");
                }
                break;

            case "messageinserted":
                return TimeZoneUIMethods.ConvertDateTime(ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME), this).ToString();
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        BoardMessageInfo message = BoardMessageInfoProvider.GetBoardMessageInfo(Convert.ToInt32(actionArgument));
        BoardInfo bi = BoardInfoProvider.GetBoardInfo(message.MessageBoardID);

        switch (actionName)
        {
            case "delete":
            case "approve":
                // Check whether user is board moderator first 
                if (!BoardInfoProvider.IsUserAuthorizedToManageMessages(bi))
                {
                    // Then check modify to messageboards
                    if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
                    {
                        return;
                    }
                }
                break;
        }

        switch (actionName)
        {
            case "delete":
                if (message != null)
                {
                    BoardMessageInfoProvider.DeleteBoardMessageInfo(message);
                }
                break;

            case "approve":
                if (message != null)
                {
                    if (message.MessageApproved)
                    {
                        // Reject message
                        message.MessageApproved = false;
                        message.MessageApprovedByUserID = 0;
                    }
                    else
                    {
                        // Approve message
                        message.MessageApproved = true;
                        message.MessageApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                    }
                    BoardMessageInfoProvider.SetBoardMessageInfo(message);
                }
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }


    protected void btnOk_Clicked(object sender, EventArgs e)
    {
        // Check permissions
        if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
        {
            return;
        }

        if (drpActions.SelectedValue != "SELECT")
        {
            var list = gridElem.SelectedItems;
            if (list.Count > 0)
            {
                foreach (string messageId in list)
                {
                    BoardMessageInfo message = BoardMessageInfoProvider.GetBoardMessageInfo(Convert.ToInt32(messageId));
                    switch (drpActions.SelectedValue)
                    {
                        case "DELETE":
                            // Delete board
                            BoardMessageInfoProvider.DeleteBoardMessageInfo(message);
                            break;

                        case "APPROVE":
                            if (!message.MessageApproved)
                            {
                                message.MessageApproved = true;
                                message.MessageApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                                BoardMessageInfoProvider.SetBoardMessageInfo(message);
                            }
                            break;

                        case "REJECT":
                            // Reject message
                            if (message.MessageApproved)
                            {
                                message.MessageApproved = false;
                                message.MessageApprovedByUserID = 0;
                                BoardMessageInfoProvider.SetBoardMessageInfo(message);
                            }
                            break;

                        case "SPAM":
                            if (!message.MessageIsSpam)
                            {
                                message.MessageIsSpam = true;
                                BoardMessageInfoProvider.SetBoardMessageInfo(message);
                            }
                            break;

                        case "NOSPAM":
                            if (message.MessageIsSpam)
                            {
                                message.MessageIsSpam = false;
                                BoardMessageInfoProvider.SetBoardMessageInfo(message);
                            }
                            break;
                    }
                }
            }
            else
            {
                ltlScript.Text += ScriptHelper.GetAlertScript(GetString("general.noitems"));
            }
        }

        gridElem.ResetSelection();

        ReloadData();
    }

    #endregion
}
