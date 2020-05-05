using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI.Configuration;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_LiveControls_Boards : CMSAdminItemsControl
{
    #region "Selected tab enumeration"

    private enum SelectedControlEnum : int
    {
        Messages = 0,
        General = 1,
        Moderators = 2,
        Security = 3,
        Subscriptions = 4,
        Listing = 5
    };

    #endregion


    #region "Private fields"

    private BoardInfo board = null;

    private int mGroupID = 0;
    private bool mHideWhenGroupIsNotSupplied = false;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns currently selected tab.
    /// </summary>
    private SelectedControlEnum SelectedControl
    {
        get
        {
            int selectedTab = ValidationHelper.GetInteger(ViewState["SelectedControl"], 0);
            return Enum.IsDefined(typeof(SelectedControlEnum), selectedTab) ? (SelectedControlEnum)selectedTab : SelectedControlEnum.Messages;
        }
        set
        {
            tabElem.SelectedTab = (int)value;
        }
    }


    /// <summary>
    /// Current board ID for internal use.
    /// </summary>
    private int BoardID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["BoardID"], 0);
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current group ID.
    /// </summary>
    public int GroupID
    {
        get
        {
            if (mGroupID == 0)
            {
                mGroupID = ValidationHelper.GetInteger(GetValue("GroupID"), 0);
            }

            if (mGroupID == 0)
            {
                mGroupID = QueryHelper.GetInteger("groupid", 0);
            }
            return mGroupID;
        }
        set
        {
            mGroupID = value;
        }
    }


    /// <summary>
    /// Determines whether to hide the content of the control when GroupID is not supplied.
    /// </summary>
    public bool HideWhenGroupIsNotSupplied
    {
        get
        {
            return mHideWhenGroupIsNotSupplied;
        }
        set
        {
            mHideWhenGroupIsNotSupplied = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if the group was supplied and hide control if necessary
        if ((GroupID == 0) && (HideWhenGroupIsNotSupplied))
        {
            Visible = false;
        }

        if (StopProcessing || !Visible)
        {
            EnableViewState = false;
            boardMessages.StopProcessing = true;
            boardList.StopProcessing = true;
            boardEdit.StopProcessing = true;
            boardModerators.StopProcessing = true;
            boardSecurity.StopProcessing = true;
            boardSubscriptions.StopProcessing = true;
            return;
        }

        // Initializes the controls
        SetupControls();

        // Display current control
        if (ViewState["SelectedControl"] != null)
        {
            DisplayControl(SelectedControl, false);
        }

        // Reload data if necessary
        if (!RequestHelper.IsPostBack() && !IsLiveSite)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        tabElem.TabControlIdPrefix = "boards";
        tabElem.OnTabClicked += new EventHandler(tabElem_OnTabChanged);
        lnkBackHidden.Click += new EventHandler(lnkBackHidden_Click);

        // Register for the security events
        boardList.OnCheckPermissions += new CheckPermissionsEventHandler(boardList_OnCheckPermissions);
        boardEdit.OnCheckPermissions += new CheckPermissionsEventHandler(boardEdit_OnCheckPermissions);
        boardModerators.OnCheckPermissions += new CheckPermissionsEventHandler(boardModerators_OnCheckPermissions);
        boardSecurity.OnCheckPermissions += new CheckPermissionsEventHandler(boardSecurity_OnCheckPermissions);

        // Setup controls        
        boardList.IsLiveSite = IsLiveSite;
        boardList.GroupID = GroupID;
        boardList.OnAction += new CommandEventHandler(boardList_OnAction);

        boardMessages.IsLiveSite = IsLiveSite;
        boardMessages.OnCheckPermissions += new CheckPermissionsEventHandler(boardMessages_OnCheckPermissions);
        boardMessages.BoardID = BoardID;
        boardMessages.GroupID = GroupID;
        boardMessages.EditPageUrl = (GroupID > 0) ? "~/CMSModules/Groups/CMSPages/Message_Edit.aspx" : "~/CMSModules/MessageBoards/CMSPages/Message_Edit.aspx";

        boardEdit.IsLiveSite = IsLiveSite;
        boardEdit.BoardID = BoardID;
        boardEdit.DisplayMode = DisplayMode;

        boardModerators.IsLiveSite = IsLiveSite;
        boardModerators.BoardID = BoardID;

        boardSecurity.IsLiveSite = IsLiveSite;
        boardSecurity.BoardID = BoardID;
        boardSecurity.GroupID = GroupID;

        boardSubscriptions.IsLiveSite = IsLiveSite;
        boardSubscriptions.BoardID = BoardID;
        boardSubscriptions.GroupID = GroupID;

        // Initialize tab control
        tabElem.TabItems.Add(new TabItem { Text = GetString("Group_General.Boards.Boards.Messages") });
        tabElem.TabItems.Add(new TabItem { Text = GetString("Group_General.Boards.Boards.Edit") });
        tabElem.TabItems.Add(new TabItem { Text = GetString("Group_General.Boards.Boards.Moderators") });
        tabElem.TabItems.Add(new TabItem { Text = GetString("Group_General.Boards.Boards.Security") });
        tabElem.TabItems.Add(new TabItem { Text = GetString("Group_General.Boards.Boards.SubsList") });

        InitializeBreadcrumbs();
    }


    /// <summary>
    /// Initializes breadcrumb items
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("Group_General.Boards.Boards.BackToList"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        if (BoardID > 0)
        {
            board = BoardInfoProvider.GetBoardInfo(BoardID);
            if (board != null)
            {
                ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
                {
                    Text = board.BoardDisplayName,
                });
            }
        }
    }


    /// <summary>
    /// Displays specified control.
    /// </summary>
    /// <param name="selectedControl">Control to be displayed</param>
    /// <param name="reload">If True, ReloadData on child control is called</param>
    private void DisplayControl(SelectedControlEnum selectedControl, bool reload)
    {
        // First hide and stop all elements
        plcList.Visible = false;
        boardList.StopProcessing = true;

        plcTabs.Visible = true;
        plcTabsHeader.Visible = true;

        tabEdit.Visible = false;
        boardEdit.StopProcessing = true;

        tabMessages.Visible = false;
        boardMessages.StopProcessing = true;

        tabModerators.Visible = false;
        boardModerators.StopProcessing = true;

        tabSecurity.Visible = false;
        boardSecurity.StopProcessing = true;

        tabSubscriptions.Visible = false;
        boardSubscriptions.StopProcessing = true;

        // Set correct tab
        SelectedControl = selectedControl;
        pnlContent.CssClass = "TabBody";

        InitializeBreadcrumbs();

        // Enable currently selected element
        switch (selectedControl)
        {
            case SelectedControlEnum.Listing:
                pnlContent.CssClass = "";
                plcTabs.Visible = false;
                plcTabsHeader.Visible = false;
                plcList.Visible = true;
                boardList.StopProcessing = false;
                if (reload)
                {
                    // Relaod data
                    boardList.ReloadData();
                }
                break;

            case SelectedControlEnum.General:
                tabEdit.Visible = true;
                boardEdit.StopProcessing = false;
                if (reload)
                {
                    // Relaod data
                    boardEdit.ReloadData();
                }
                break;

            case SelectedControlEnum.Messages:
                tabMessages.Visible = true;
                boardMessages.StopProcessing = false;
                if (reload)
                {
                    boardMessages.IsLiveSite = IsLiveSite;
                    boardMessages.BoardID = BoardID;
                    boardMessages.ReloadData();
                }
                break;

            case SelectedControlEnum.Moderators:
                tabModerators.Visible = true;
                boardModerators.StopProcessing = false;
                if (reload)
                {
                    // Reload data
                    boardModerators.ReloadData(true);
                }
                break;

            case SelectedControlEnum.Security:
                tabSecurity.Visible = true;
                boardSecurity.StopProcessing = false;
                if (reload)
                {
                    // Reload data
                    boardSecurity.ReloadData();
                }
                break;

            case SelectedControlEnum.Subscriptions:
                tabSubscriptions.Visible = true;
                boardSubscriptions.StopProcessing = false;
                if (reload)
                {
                    boardSubscriptions.BoardID = BoardID;
                    // Reload data
                    boardSubscriptions.ReloadData();
                }

                break;
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControls();

        DisplayControl(SelectedControlEnum.Listing, true);
    }


    /// <summary>
    /// Displays the default control.
    /// </summary>
    public void SetDefault()
    {
        DisplayControl(SelectedControlEnum.Listing, true);
    }


    /// <summary>
    /// Clears the boards filter up.
    /// </summary>
    public void ClearFilter()
    {
        boardList.ClearFilter();
    }


    #region "Event handlers"

    protected void tabElem_OnTabChanged(object sender, EventArgs e)
    {
        ViewState.Add("SelectedControl", tabElem.SelectedTab);
        DisplayControl(SelectedControl, true);
    }


    protected void boardList_OnAction(object sender, CommandEventArgs e)
    {
        if ((e.CommandName.ToLowerCSafe() == "edit") && plcList.Visible)
        {
            ViewState["BoardID"] = e.CommandArgument;
            DisplayControl(SelectedControlEnum.Messages, true);
            ViewState.Add("SelectedControl", (int)SelectedControlEnum.Messages);
        }
    }


    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        ViewState.Add("SelectedControl", null);
        DisplayControl(SelectedControlEnum.Listing, true);
    }

    #endregion


    #region "Security event handlers"

    protected void boardList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardMessages_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardModerators_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion
}