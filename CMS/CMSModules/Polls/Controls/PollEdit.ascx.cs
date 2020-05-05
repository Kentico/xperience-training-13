using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI.Configuration;
using CMS.Helpers;
using CMS.Polls;
using CMS.UIControls;


public partial class CMSModules_Polls_Controls_PollEdit : CMSAdminEditControl
{
    #region "Private variables"

    private int mSiteID;
    private int mGroupID;

    #endregion


    #region "Public variables"

    /// <summary>
    /// Gets or Sets site ID of the poll.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
            PollProperties.SiteID = mSiteID;
        }
    }


    /// <summary>
    /// Gets or sets the group ID for which the poll should be created.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupID;
        }
        set
        {
            mGroupID = value;
            PollProperties.GroupID = mGroupID;
        }
    }


    /// <summary>
    /// Gets or sets answer edit property.
    /// </summary>
    public bool AnswerEditSelected
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["answeredit"], false);
        }
        set
        {
            ViewState["answeredit"] = value;
        }
    }

    #endregion


    #region "Security handlers"

    /// <summary>
    /// Polls security - check permission event handler.
    /// </summary>
    private void PollSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    /// <summary>
    /// Answer edit - check permission event handler.
    /// </summary>
    private void AnswerEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    /// <summary>
    /// Answer list - check permission event handler.
    /// </summary>
    private void AnswerList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    /// <summary>
    /// Poll properties - check permission event handler.
    /// </summary>
    private void PollProperties_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        PollProperties.DisplayMode = DisplayMode;
        PollProperties.IsLiveSite = IsLiveSite;
        AnswerList.IsLiveSite = IsLiveSite;
        AnswerEdit.IsLiveSite = IsLiveSite;
        PollSecurity.IsLiveSite = IsLiveSite;
        PollView.IsLiveSite = IsLiveSite;
        PollView.CheckOpen = false;

        PollSecurity.OnCheckPermissions += PollSecurity_OnCheckPermissions;
        AnswerEdit.OnCheckPermissions += AnswerEdit_OnCheckPermissions;
        AnswerList.OnCheckPermissions += AnswerList_OnCheckPermissions;
        PollProperties.OnCheckPermissions += PollProperties_OnCheckPermissions;

        // Setup buttons
        btnNewAnswer.ResourceString = "Polls_Answer_List.NewItemCaption";
        imgNewAnswer.AlternateText = GetString("Polls_Answer_List.NewItemCaption");
        imgNewAnswer.ImageUrl = GetImageUrl("Objects/Polls_PollAnswer/add.png");
        btnResetAnswers.ResourceString = "Polls_Answer_List.ResetButton";
        btnResetAnswers.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("Polls_Answer_List.ResetConfirmation")) + ");";
        imgResetAnswers.ImageUrl = GetImageUrl("CMSModules/CMS_Polls/resetanswers.png");
        imgResetAnswers.AlternateText = GetString("Polls_Answer_List.ResetButton");

        // Menu initialization
        tabMenu.UrlTarget = "_self";
        tabMenu.AddTab(new TabItem
        {
            Text = GetString("general.general"),
            RedirectUrl = "#"
        });
        tabMenu.AddTab(new TabItem
        {
            Text = GetString("Polls_Edit.Answers"),
            RedirectUrl = "#"
        });
        tabMenu.AddTab(new TabItem
        {
            Text = GetString("general.security"),
            RedirectUrl = "#"
        });
        tabMenu.AddTab(new TabItem
        {
            Text = GetString("general.view"),
            RedirectUrl = "#"
        });

        tabMenu.UsePostback = true;
        tabMenu.UseClientScript = true;
        tabMenu.RenderLinks = true;
        tabMenu.TabControlIdPrefix = ClientID;

        // BreadCrumbs back link handler
        lnkBackHidden.Click += lnkBackHidden_Click;

        // Register event handlers
        btnNewAnswer.Click += btnNewAnswer_Click;
        btnResetAnswers.Click += btnResetAnswers_Click;
        tabMenu.OnTabClicked += tabMenu_OnTabClicked;
        AnswerList.OnEdit += AnswerList_OnEdit;
        AnswerEdit.OnSaved += AnswerEdit_OnSaved;

        if (!RequestHelper.IsPostBack() && !IsLiveSite)
        {
            ReloadData(false);
        }
    }


    /// <summary>
    /// Reloads data in controls.
    /// </summary>
    /// <param name="forceReload">Forces to reload all data</param>
    public override void ReloadData(bool forceReload)
    {
        base.ReloadData(forceReload);
        DisplayControls(tabMenu.SelectedTab.ToString(), forceReload);
    }


    /// <summary>
    /// Tab menu clicked event handler.
    /// </summary>
    private void tabMenu_OnTabClicked(object sender, EventArgs e)
    {
        DisplayControls(tabMenu.SelectedTab.ToString(), false);
    }


    /// <summary>
    /// Displays appropriate controls regarding set properties.
    /// </summary>
    private void DisplayControls(string selectedPage, bool forceReload)
    {
        PollProperties.Visible = false;
        AnswerList.Visible = false;
        PollSecurity.Visible = false;
        PollView.Visible = false;
        PollView.StopProcessing = true;
        headerLinks.Visible = false;
        pnlPollsBreadcrumbs.Visible = false;
        pnlPollsLinks.Visible = false;
        AnswerEdit.Visible = false;
        btnResetAnswers.Visible = true;
        imgResetAnswers.Visible = true;

        if (forceReload)
        {
            selectedPage = "0";
            tabMenu.SelectedTab = 0;
        }

        // Display appropriate tab
        switch (selectedPage)
        {
            // Answer list
            case "1":
                AnswerList.Visible = true;
                AnswerList.PollId = ItemID;
                AnswerList.ReloadData(true);
                headerLinks.Visible = true;
                pnlPollsLinks.Visible = true;
                break;

            // Answer edit
            case "answersedit":
                headerLinks.Visible = true;
                pnlPollsBreadcrumbs.Visible = true;
                pnlPollsLinks.Visible = true;
                AnswerEdit.Visible = true;
                AnswerEdit.PollId = ItemID;
                AnswerEdit.ReloadData();
                btnResetAnswers.Visible = false;
                imgResetAnswers.Visible = false;
                AnswerEditSelected = true;

                // Initialize breadcrumbs
                InitializeBreadcrumbs();
                break;

            // Poll security
            case "2":
                PollSecurity.Visible = true;
                PollSecurity.ItemID = ItemID;
                PollSecurity.ReloadData();
                break;

            // Poll view
            case "3":
                PollView.Visible = true;
                InitPollView(ItemID);
                PollView.StopProcessing = false;
                PollView.ReloadData(false);
                break;

            // Poll properties
            default:
                PollProperties.Visible = true;
                PollProperties.ItemID = ItemID;

                PollProperties.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Answer list edit action event handler.
    /// </summary>
    private void AnswerList_OnEdit(object sender, EventArgs e)
    {
        // Handle events from visible controls only
        if (AnswerList.Visible)
        {
            AnswerEdit.ItemID = AnswerList.SelectedItemID;
            AnswerEdit.PollId = ItemID;
            DisplayControls("answersedit", false);
        }
    }


    /// <summary>
    /// Answer Edit OnSave handler.
    /// </summary>
    private void AnswerEdit_OnSaved(object sender, EventArgs e)
    {
        // Handle events from visible controls only
        if (AnswerEdit.Visible)
        {
            AnswerEdit.PollId = ItemID;
            DisplayControls("answersedit", false);
            AnswerEdit.LoadData();
        }
    }


    /// <summary>
    /// New answer button click.
    /// </summary>
    private void btnNewAnswer_Click(object sender, EventArgs e)
    {
        if (CheckModifyPermission(ItemID))
        {
            AnswerEdit.ItemID = 0;
            AnswerEdit.PollId = ItemID;
            DisplayControls("answersedit", false);
            AnswerEdit.LoadData();
        }
    }


    /// <summary>
    /// Breadcrumbs click handler.
    /// </summary>
    private void lnkBackHidden_Click(object sender, EventArgs e)
    {
        DisplayControls("1", false);
    }


    /// <summary>
    /// Reset answers button handler.
    /// </summary>
    private void btnResetAnswers_Click(object sender, EventArgs e)
    {
        if (CheckModifyPermission(ItemID))
        {
            PollAnswerInfoProvider.ResetAnswers(ItemID);
            AnswerList.ReloadData();
        }
    }


    /// <summary>
    /// Initializes PollView control.
    /// </summary>
    /// <param name="pollId">ID of current Poll</param>
    protected void InitPollView(int pollId)
    {
        PollInfo pi = PollInfoProvider.GetPollInfo(pollId);

        if (pi != null)
        {
            PollView.PollCodeName = pi.PollCodeName;
            PollView.PollSiteID = pi.PollSiteID;
            PollView.PollGroupID = pi.PollGroupID;
            PollView.CountType = CountTypeEnum.Percentage;
            PollView.ShowGraph = true;
            PollView.ShowResultsAfterVote = true;
            PollView.CheckPermissions = false;
            PollView.CheckVoted = false;
            PollView.HideWhenNotAuthorized = false;
            PollView.CheckOpen = false;
            PollView.Visible = false;
        }
    }


    /// <summary>
    /// Checks modify permission. Returns false if checking failed.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    private bool CheckModifyPermission(int pollId)
    {
        PollInfo pi = PollInfoProvider.GetPollInfo(pollId);
        if (pi != null)
        {
            return (pi.PollSiteID > 0) && CheckPermissions("cms.polls", PERMISSION_MODIFY) ||
                   (pi.PollSiteID <= 0) && CheckPermissions("cms.polls", PERMISSION_GLOBALMODIFY);
        }
        return false;
    }


    /// <summary>
    /// Initializes breadcrumbs items.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("Polls_Answer_Edit.ItemListLink"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        PollAnswerInfo pollAnswerObj = (AnswerEdit.ItemID > 0) ? PollAnswerInfoProvider.GetPollAnswerInfo(AnswerEdit.ItemID) : null;

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem { 
            Text = (pollAnswerObj == null) ? GetString("Polls_Answer_Edit.NewItemCaption") : GetString("Polls_Answer_Edit.AnswerLabel") + " " + pollAnswerObj.AnswerOrder.ToString(),
        });
    }

    #endregion
}