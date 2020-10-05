using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_Dialogs_AdvancedMediaLibrarySelector : CMSUserControl
{
    #region "Events & delegates"

    /// <summary>
    /// Event fired when library selection changed.
    /// </summary>
    public event EventHandler LibraryChanged;

    #endregion


    #region "Private variables"

    private AvailableLibrariesEnum mGlobalLibraries = AvailableLibrariesEnum.None; 
    private AvailableSitesEnum mSites = AvailableSitesEnum.All;
    private string mGlobalLibraryName = "";
    private int mSelectedLibraryID;
    private string mSiteToSelect = "";

    private bool mLibraryDataLoaded;

    #endregion
    #region "Public properties"

    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            siteSelector.IsLiveSite = value;
            librarySelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates what libraries should be available.
    /// </summary>
    public AvailableLibrariesEnum GlobalLibraries
    {
        get
        {
            return mGlobalLibraries;
        }
        set
        {
            mGlobalLibraries = value;
        }
    }


    /// <summary>
    /// Available sites.
    /// </summary>
    public AvailableSitesEnum Sites
    {
        get
        {
            return mSites;
        }
        set
        {
            mSites = value;
        }
    }


    /// <summary>
    /// Name of the global library.
    /// </summary>
    public string GlobalLibraryName
    {
        get
        {
            return mGlobalLibraryName;
        }
        set
        {
            mGlobalLibraryName = value;
        }
    }


    /// <summary>
    /// ID of the library to select.
    /// </summary>
    public int SelectedLibraryID
    {
        get
        {
            return mSelectedLibraryID;
        }
        set
        {
            mSelectedLibraryID = value;
            LibraryID = value;
        }
    }


    /// <summary>
    /// Current library ID.
    /// </summary>
    public int LibraryID
    {
        get
        {
            return librarySelector.MediaLibraryID;
        }
        set
        {
            librarySelector.MediaLibraryID = value;
        }
    }


    /// <summary>
    /// Name of the library to pre-select.
    /// </summary>
    public string LibraryName
    {
        get
        {
            return librarySelector.MediaLibraryName;
        }
        set
        {
            librarySelector.MediaLibraryName = value;
        }
    }


    /// <summary>
    /// Currently selected site name.
    /// </summary>
    public string SelectedSiteName
    {
        get
        {
            if (String.IsNullOrEmpty(siteSelector.SiteName))
            {
                return mSiteToSelect;
            }
            return siteSelector.SiteName;
        }
        set
        {
            siteSelector.SiteName = value;
            mSiteToSelect = value;
        }
    }


    /// <summary>
    /// ID of the currently selected site.
    /// </summary>
    public int SiteID
    {
        get
        {
            int siteId = ValidationHelper.GetInteger(siteSelector.DropDownSingleSelect.SelectedValue, 0);
            return (siteId > 0 ? siteId : -1);
        }
        set
        {
            siteSelector.SiteID = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates if advanced library selector was loaded before
    /// </summary>
    private bool IsSelectorLoaded
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["MediaLibraryASelectorLoaded"], false);
        }
        set
        {
            ViewState["MediaLibraryASelectorLoaded"] = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        siteSelector.StopProcessing = false;
        siteSelector.UniSelector.AllowEmpty = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            siteSelector.StopProcessing = true;
            librarySelector.StopProcessing = true;
            Visible = false;
        }
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            LoadLibraryData();
        }

        HandleSiteEmpty();

        if (librarySelector.MediaLibraryID != 0)
        {
            SetLibraries();
        }
        else
        {
            SetLibrariesEmpty();
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Loads selector data.
    /// </summary>
    public void LoadData()
    {
        // Initialize controls
        SetupControls();
    }


    /// <summary>
    /// Loads media library data
    /// </summary>
    public void LoadLibraryData()
    {
        if (mLibraryDataLoaded)
        {
            return;
        }

        // Reload libraries
        LoadLibrarySelection();

        // Pre-select library
        PreselectLibrary();

        // OnLibraryChanged will be fired only when control wasn't loaded before
        if (!IsSelectorLoaded)
        {
            IsSelectorLoaded = true;
            RaiseOnLibraryChanged();
        }

        mLibraryDataLoaded = true;
    }


    /// <summary>
    /// Reloads content of site selector.
    /// </summary>
    public void ReloadSites()
    {
        siteSelector.Reload(true);
    }


    #region "Private methods"

    /// <summary>
    /// Initializes all the inner controls.
    /// </summary>
    private void SetupControls()
    {
        // Initialize site selector
        siteSelector.IsLiveSite = IsLiveSite;
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.UniSelector.WhereCondition = GetSitesWhere();
        siteSelector.Reload(true);

        librarySelector.IsLiveSite = IsLiveSite;
    }


    /// <summary>
    /// Gets WHERE condition to retrieve available sites according specified type.
    /// </summary>
    private string GetSitesWhere()
    {
        WhereCondition condition = new WhereCondition().WhereEquals("SiteStatus", SiteStatusEnum.Running.ToStringRepresentation());

        // If not global admin display only related sites
        if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            condition.WhereIn("SiteID", new IDQuery<UserSiteInfo>(UserSiteInfo.TYPEINFO.SiteIDColumn)
                                                    .WhereEquals("UserID", MembershipContext.AuthenticatedUser.UserID));
        }

        switch (Sites)
        {
            case AvailableSitesEnum.OnlySingleSite:
                if (!string.IsNullOrEmpty(SelectedSiteName))
                {
                    condition.WhereEquals("SiteName", SelectedSiteName);
                }
                break;

            case AvailableSitesEnum.OnlyCurrentSite:
                condition.WhereEquals("SiteName", SiteContext.CurrentSiteName);
                break;
        }

        return condition.ToString(true);
    }


    /// <summary>
    /// Disables libraries drop-down list when empty.
    /// </summary>
    private void SetLibrariesEmpty()
    {
        LibraryID = 0;
        librarySelector.Enabled = false;

        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "DialogsDisableMenuActions", ScriptHelper.GetScript("if(window.DisableNewFileBtn){ window.DisableNewFileBtn(); } if(window.DisableNewFolderBtn){ window.DisableNewFolderBtn(); }"));
    }


    /// <summary>
    /// Enables libraries drop-down list.
    /// </summary>
    private void SetLibraries()
    {
        librarySelector.Enabled = true;
    }


    /// <summary>
    /// Disables sites drop-down list when empty.
    /// </summary>
    private void HandleSiteEmpty()
    {
        if (!siteSelector.UniSelector.HasData || siteSelector.SiteID == 0)
        {
            siteSelector.Enabled = false;
        }
    }

    #endregion


    #region "Selection handler methods"

    /// <summary>
    /// Initializes library selector.
    /// </summary>
    public void LoadLibrarySelection()
    {
        librarySelector.Where = GetDisplayedLibrariesCondition();

        librarySelector.SiteID = SiteID;
        librarySelector.ReloadData();
    }


    /// <summary>
    /// Returns WHERE condition when libraries are being displayed. Sets also group identifier if specified
    /// </summary>
    private string GetDisplayedLibrariesCondition()
    {
        AvailableLibrariesEnum availableLibrariesEnum;
        
        // Get correct libraries
        availableLibrariesEnum = GlobalLibraries;
        string libraryName = GlobalLibraryName;

        var condition = new WhereCondition();

        switch (availableLibrariesEnum)
        {
            case AvailableLibrariesEnum.OnlySingleLibrary:
                librarySelector.SiteID = SiteID;
                return condition.WhereEquals("LibraryName", libraryName).ToString(true);

            case AvailableLibrariesEnum.OnlyCurrentLibrary:
                int libraryId = (MediaLibraryContext.CurrentMediaLibrary != null) ? MediaLibraryContext.CurrentMediaLibrary.LibraryID : 0;
                return condition.WhereEquals("LibraryID", libraryId).ToString(true);

            case AvailableLibrariesEnum.None:
                return condition.NoResults().ToString(true);

            default:
                librarySelector.SiteID = SiteID;
                return String.Empty;
        }
    }


    /// <summary>
    /// Ensures right library is selected in the list.
    /// </summary>
    private void PreselectLibrary()
    {
        if ((SelectedLibraryID > 0) && siteSelector.UniSelector.HasData)
        {
            librarySelector.MediaLibraryID = SelectedLibraryID;
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Handler of the event occurring when the site in selector has changed.
    /// </summary>
    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SiteID = (int)siteSelector.Value;

        // Pre-select library
        PreselectLibrary();

        // Reload libraries
        LoadLibrarySelection();

        RaiseOnLibraryChanged();
    }


    protected void librarySelector_SelectedLibraryChanged()
    {
        // Let the parent now about library selection change
        RaiseOnLibraryChanged();
    }


    /// <summary>
    /// Fires event when library changed.
    /// </summary>
    private void RaiseOnLibraryChanged()
    {
        // Fire event
        if (LibraryChanged != null)
        {
            LibraryChanged(this, null);
        }
    }   

    #endregion
}