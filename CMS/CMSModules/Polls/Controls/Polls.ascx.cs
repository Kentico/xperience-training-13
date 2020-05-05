using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Polls;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Polls_Controls_Polls : CMSAdminEditControl
{
    #region "Variables"

    private int mGroupID = 0;
    private int mSiteID = 0;
    private Guid mGroupGUID = Guid.Empty;
    private bool mHideWhenGroupIsNotSupplied = false;
    private bool dataLoaded = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets ID of site.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID <= 0)
            {
                mSiteID = ValidationHelper.GetInteger(GetValue("SiteID"), SiteContext.CurrentSiteID);
            }
            return mSiteID;
        }
        set
        {
            mSiteID = value;
            PollNew.SiteID = mSiteID;
            PollEdit.SiteID = mSiteID;
        }
    }


    /// <summary>
    /// Gets or sets ID of group.
    /// </summary>
    public int GroupID
    {
        get
        {
            if (mGroupID <= 0)
            {
                mGroupID = ValidationHelper.GetInteger(GetValue("GroupID"), 0);
            }

            return mGroupID;
        }
        set
        {
            mGroupID = value;
        }
    }


    /// <summary>
    /// Gets or sets GUID of group.
    /// </summary>
    public Guid GroupGUID
    {
        get
        {
            if (mGroupGUID == Guid.Empty)
            {
                mGroupGUID = ValidationHelper.GetGuid(GetValue("GroupGUID"), Guid.Empty);
            }

            return mGroupGUID;
        }
        set
        {
            mGroupGUID = value;
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


    /// <summary>
    /// Gets or sets switch to display appropriate controls.
    /// </summary>
    public string SelectedControl
    {
        get
        {
            return ValidationHelper.GetString(ViewState["selectedcontrol" + ClientID], "list");
        }
        set
        {
            ViewState["selectedcontrol" + ClientID] = (object)value;
        }
    }


    public bool DelayedReload
    {
        get
        {
            return PollsList.DelayedReload;
        }
        set
        {
            PollsList.DelayedReload = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        #region "Security"

        if (!CheckPermissions("cms.polls", PERMISSION_READ))
        {
            return;
        }

        PollEdit.OnCheckPermissions += new CheckPermissionsEventHandler(PollEdit_OnCheckPermissions);
        PollNew.OnCheckPermissions += new CheckPermissionsEventHandler(PollNew_OnCheckPermissions);
        PollsList.OnCheckPermissions += new CheckPermissionsEventHandler(PollsList_OnCheckPermissions);

        #endregion

        PollsList.IsLiveSite = IsLiveSite;
        PollEdit.IsLiveSite = IsLiveSite;
        PollNew.IsLiveSite = IsLiveSite;

        PollNew.SiteID = SiteID;
        PollEdit.SiteID = SiteID;

        PollsList.GroupId = GroupID;
        PollNew.GroupID = GroupID;
        PollNew.GroupGUID = GroupGUID;
        PollEdit.GroupID = GroupID;

        // Set display mode
        PollNew.DisplayMode = DisplayMode;
        PollEdit.DisplayMode = DisplayMode;

        if ((GroupID == 0) && HideWhenGroupIsNotSupplied)
        {
            Visible = false;
            return;
        }

        PollsList.OnEdit += new EventHandler(PollsList_OnEdit);
        PollNew.OnSaved += new EventHandler(PollNew_OnSaved);
        btnNewPoll.Click += new EventHandler(btnNewPoll_Click);
        lnkBackHidden.Click += new EventHandler(lnkBackHidden_Click);

        if (!RequestHelper.IsPostBack() && (!IsLiveSite))
        {
            ReloadData(false);
        }

        InitializeBreadcrumbs();
    }


    #region "Security handlers"

    private void PollsList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void PollNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void PollEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    /// <summary>
    /// Displays controls in dependence on properties.
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        base.ReloadData(forceReload);

        // Setup button
        imgNewPoll.ImageUrl = GetImageUrl("Objects/Polls_Poll/add.png");
        imgNewPoll.AlternateText = GetString("polls_new.newitemcaption");
        btnNewPoll.ResourceString = "Polls_List.NewItemCaption";

        // Setup panels
        pnlPollsHeaderLinks.Visible = false;
        pnlPollsHeaderBreadCrumbs.Visible = false;
        pnlList.Visible = false;
        pnlEdit.Visible = false;
        pnlPollNew.Visible = false;

        // Display appropriate poll controls
        switch (SelectedControl)
        {
            case "new":
                {
                    pnlPollsHeaderBreadCrumbs.Visible = true;
                    pnlPollNew.Visible = true;
                    PollNew.ReloadData();
                    break;
                }
            case "edit":
                {
                    pnlPollsHeaderBreadCrumbs.Visible = true;
                    pnlEdit.Visible = true;
                    PollEdit.ReloadData(true);
                    break;
                }
            case "list":
            default:
                {
                    if (!dataLoaded || forceReload)
                    {
                        pnlPollsHeaderLinks.Visible = true;
                        pnlList.Visible = true;
                        PollsList.GroupId = GroupID;
                        PollsList.ReloadData();
                        dataLoaded = true;
                    }
                    break;
                }
        }

        InitializeBreadcrumbs();
    }


    /// <summary>
    /// New poll saved event handler.
    /// </summary>
    private void PollNew_OnSaved(object sender, EventArgs e)
    {
        // Handle events only from visible controls
        if (PollNew.Visible)
        {
            ItemID = PollNew.ItemID;
            PollEdit.ItemID = PollNew.ItemID;
            PollEdit.ReloadData(true);
            SelectedControl = "edit";
            ReloadData();
        }
    }


    /// <summary>
    /// Edit poll click event handler.
    /// </summary>
    private void PollsList_OnEdit(object sender, EventArgs e)
    {
        // Handle events only from visible controls
        if (PollsList.Visible)
        {
            ItemID = PollsList.SelectedItemID;
            PollEdit.ItemID = PollsList.SelectedItemID;
            SelectedControl = "edit";
            ReloadData();
        }
    }


    /// <summary>
    /// New poll click handler.
    /// </summary>
    private void btnNewPoll_Click(object sender, EventArgs e)
    {
        ItemID = 0;
        PollEdit.ItemID = 0;
        PollEdit.ReloadData(true);
        PollNew.ItemID = 0;
        PollNew.ClearForm();
        SelectedControl = "new";
        ReloadData();
    }


    /// <summary>
    /// Breadcrumbs click event handler.
    /// </summary>
    private void lnkBackHidden_Click(object sender, EventArgs e)
    {
        SelectedControl = "list";
        ReloadData(true);
    }


    /// <summary>
    /// Initializes breadcrumbs items.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("Polls_Edit.itemlistlink"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        PollInfo pi = PollInfoProvider.GetPollInfo(this.ItemID);

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = (pi == null) ? GetString("polls_new.newitemcaption") : pi.PollDisplayName,
        });

    }
}