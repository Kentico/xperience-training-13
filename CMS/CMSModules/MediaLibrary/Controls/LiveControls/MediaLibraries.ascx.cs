using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DocumentEngine.Web.UI.Configuration;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_LiveControls_MediaLibraries : CMSAdminItemsControl, IPostBackEventHandler
{
    #region "Private variables"

    protected int mGroupID = 0;
    protected Guid mGroupGUID = Guid.Empty;
    protected bool mHideWhenGroupIsNotSupplied = false;
    protected bool isNewLibrary = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the group library belongs to.
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
    /// GUID of the group library belongs to.
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
    /// ID of the media library.
    /// </summary>
    protected int LibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["LibraryID"], 0);
        }
        set
        {
            ViewState.Add("LibraryID", value);
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


    /// <summary>
    /// OnInit event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        libraryEdit.StopProcessing = true;
    }


    /// <summary>
    /// Page_Load event handler
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        #region "Security"

        RaiseOnCheckPermissions(PERMISSION_READ, this);

        #endregion

        if (!Visible)
        {
            EnableViewState = false;
        }

        if (StopProcessing)
        {
            libraryFiles.StopProcessing = true;
            librarySecurity.StopProcessing = true;
        }
        else
        {
            // Check if the group was supplied and hide control if necessary
            if ((GroupID == 0) && (HideWhenGroupIsNotSupplied))
            {
                Visible = false;
            }
            else
            {
                // Initialize controls
                SetupControls();
            }
        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes all the controls used on live control.
    /// </summary>
    private void SetupControls()
    {
        // Set display mode
        libraryList.DisplayMode = DisplayMode;
        libraryFiles.DisplayMode = DisplayMode;
        libraryEdit.DisplayMode = DisplayMode;
        librarySecurity.DisplayMode = DisplayMode;

        // Initialize tabs & header actions
        InitializeTabs();
        InitializeHeaderActions();

        tabElem.TabControlIdPrefix = "libraries";
        tabElem.OnTabClicked += tabElem_OnTabChanged;
        lnkBackHidden.Click += lnkBackHidden_Click;

        libraryList.GroupID = GroupID;
        libraryList.OnAction += libraryList_OnAction;


        libraryEdit.MediaLibraryGroupID = GroupID;
        libraryEdit.MediaLibraryGroupGUID = GroupGUID;
        libraryEdit.EditingForm.OnAfterSave += EditingForm_OnAfterSave;

        if (LibraryID > 0)
        {
            // Initialize library security tab controls
            librarySecurity.MediaLibraryID = LibraryID;

            // Initialize library files list tab controls
            libraryFiles.LibraryID = LibraryID;

            libraryEdit.MediaLibraryID = LibraryID;
            pnlContext.UIContext.EditedObject = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryID);
        }

        libraryEdit.ReloadData();
        InitializeBreadcrumbs();
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void EditingForm_OnAfterSave(object sender, EventArgs e)
    {
        isNewLibrary = (LibraryID == 0);

        LibraryID = libraryEdit.EditingForm.EditedObject.Generalized.ObjectID;

        InitializeBreadcrumbs();

        // If brand new library created
        if (isNewLibrary)
        {
            // Reload library data
            DisplayLibraryEdit();
        }
    }


    /// <summary>
    /// Initializes used tab menu control.
    /// </summary>
    private void InitializeTabs()
    {
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("general.files")
        });
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("general.general")
        });
        tabElem.AddTab(new TabItem()
        {
            Text = GetString("general.security")
        });
    }


    /// <summary>
    /// Initializes the header action element.
    /// </summary>
    private void InitializeHeaderActions()
    {
        // New subscription link
        var ha = new HeaderAction
        {
            Text = GetString("Group_General.MediaLibrary.NewLibrary"),
            CommandName = "newmedialibrary",
        };
        ha.CssClass += " new-medialibrary-button";
        newLibrary.AddAction(ha);
        newLibrary.ActionPerformed += newLibrary_ActionPerformed;
    }


    /// <summary>
    /// Initializes the breadcrumbs controls.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("Group_General.MediaLibrary.BackToList"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        string itemText = "";

        if (LibraryID > 0)
        {
            MediaLibraryInfo library = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryID);
            if (library != null)
            {
                itemText = HTMLHelper.HTMLEncode(library.LibraryDisplayName);
            }
        }
        else
        {
            itemText = GetString ("Group_General.MediaLibrary.NewLibrary");
        }

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = itemText,
        });
    }


    /// <summary>
    /// Handles displaying of library edit form.
    /// </summary>
    private void DisplayLibraryNew()
    {
        isNewLibrary = true;

        plcTabsHeader.Visible = true;
        pnlTabsMain.Visible = false;
        plcHeaderActions.Visible = false;

        plcList.Visible = false;

        plcTabs.Visible = true;
        tabEdit.Visible = true;
        tabFiles.Visible = false;
        tabSecurity.Visible = false;

        // Reset library info in view state and refresh breadcrumbs info
        LibraryID = 0;

        InitializeBreadcrumbs();

        libraryEdit.MediaLibraryGroupID = GroupID;
        libraryEdit.MediaLibraryGroupGUID = GroupGUID;
        libraryEdit.MediaLibraryID = LibraryID;

        libraryEdit.ReloadData();
    }


    /// <summary>
    /// Handles displaying of library edit form.
    /// </summary>
    private void DisplayLibraryEdit()
    {
        plcTabsHeader.Visible = true;
        pnlTabsMain.Visible = true;

        plcHeaderActions.Visible = false;

        plcList.Visible = false;

        plcTabs.Visible = true;
        tabFiles.Visible = true;
        tabEdit.Visible = false;
        tabSecurity.Visible = false;

        tabElem.SelectedTab = 0;

        libraryFiles.StopProcessing = false;
        libraryFiles.LibraryID = LibraryID;
        libraryFiles.ReloadData();
    }


    /// <summary>
    /// Displays default media library tab content.
    /// </summary>
    private void SetDefault()
    {
        plcTabsHeader.Visible = false;
        plcHeaderActions.Visible = true;
        plcList.Visible = true;
        plcTabs.Visible = false;
        LibraryID = 0;
    }

    #endregion


    #region "Event handlers"

    protected void libraryList_OnAction(object sender, CommandEventArgs e)
    {
        string commandName = e.CommandName.ToLowerCSafe();
        switch (commandName)
        {
            case "edit":
                LibraryID = ValidationHelper.GetInteger(e.CommandArgument, 0);

                plcTabs.Visible = true;

                InitializeBreadcrumbs();

                // Load library data
                DisplayLibraryEdit();
                break;

            case "delete":
                LibraryID = 0;
                break;

            default:
                break;
        }
    }


    private void lnkBackHidden_Click(object sender, EventArgs e)
    {
        libraryFiles.ShouldProcess = false;

        SetDefault();
    }


    protected void newLibrary_ActionPerformed(object sender, CommandEventArgs e)
    {
        libraryFiles.ShouldProcess = false;

        DisplayLibraryNew();
    }


    private void tabElem_OnTabChanged(object sender, EventArgs e)
    {
        plcList.Visible = false;
        plcHeaderActions.Visible = false;

        int tabIndex = tabElem.SelectedTab;

        tabFiles.Visible = (tabIndex == 0);
        libraryFiles.ShouldProcess = tabFiles.Visible;
        if (tabFiles.Visible)
        {
            plcTabs.Visible = true;
            libraryFiles.LibraryID = LibraryID;
            libraryFiles.ReloadData();
        }

        tabEdit.Visible = (tabIndex == 1);
        tabSecurity.Visible = (tabIndex == 2);
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument.ToLowerCSafe() == "reloadtree")
        {
            libraryFiles.ReLoadUserControl();
        }
    }

    #endregion
}