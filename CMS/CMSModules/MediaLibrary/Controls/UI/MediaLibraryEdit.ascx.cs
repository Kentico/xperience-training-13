using System;
using System.Linq;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_UI_MediaLibraryEdit : CMSAdminEditControl
{
    #region "Private variables"

    private int mMediaLibraryID;
    private bool mEnable = true;
    private MediaLibraryInfo mLibraryInfo;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current library info.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (MediaLibraryID > 0))
            {
                // Get data
                mLibraryInfo = MediaLibraryInfo.Provider.Get(MediaLibraryID);

                // Check whether library belongs to current site when not global admin
                if ((mLibraryInfo != null) && (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)) && (mLibraryInfo.LibrarySiteID != SiteContext.CurrentSiteID))
                {
                    mLibraryInfo = null;
                }
            }

            return mLibraryInfo;
        }
    }

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
    /// Gets or sets media library ID.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return mMediaLibraryID;
        }
        set
        {
            mMediaLibraryID = value;
            mLibraryInfo = null;
        }
    }


    /// <summary>
    /// Indicates whether editing form is enabled.
    /// </summary>
    public bool Enable
    {
        get
        {
            return mEnable;
        }
        set
        {
            mEnable = value;
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
            base.IsLiveSite = value;
            plcMess.IsLiveSite = value;
            editElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the editing form.
    /// </summary>
    public UIForm EditingForm
    {
        get
        {
            return editElem;
        }
    }


    /// <summary>
    /// Indicates if control has stopped processing.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return editElem.StopProcessing;
        }
        set
        {
            editElem.StopProcessing = value;
        }
    }

    #endregion


    #region "Life-cycle events"


    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize only when visible
            if (Visible)
            {
                InitializeControl();

                // Initialize controls
                editElem.OnBeforeSave += editElem_OnBeforeSave;
                editElem.OnAfterSave += editElem_OnAfterSave;
                editElem.OnBeforeValidate += editElem_OnBeforeValidate;
                editElem.OnAfterValidate += editElem_OnAfterValidate;
            }

            if (!RequestHelper.IsPostBack() && !IsLiveSite)
            {
                CheckLibraryID();
            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads control's content.
    /// </summary>
    public override void ReloadData()
    {
        InitializeControl();
        if (CheckLibraryID())
        {
            editElem.ReloadData();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes controls
    /// </summary>
    private void InitializeControl()
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        if (!CheckPermissions())
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("Manage"));
            return;
        }

        // Hide code name edit for simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            editElem.FieldsToHide.Add("LibraryName");
        }

        if (!Enable)
        {
            editElem.Enabled = false;
        }
    }



    /// <summary>
    /// Check library identifier if correct.
    /// </summary>
    private bool CheckLibraryID()
    {
        // Get info and load controls            
        if ((MediaLibraryID > 0) && (LibraryInfo == null))
        {
            plcProperties.Visible = false;
            ShowError(GetString("general.invalidid"));
            return false;
        }
        else
        {
            plcProperties.Visible = true;
            return true;
        }
    }


    /// <summary>
    /// Returns a value indicating whether the specified media library root folder name is unique.
    /// </summary>
    /// <param name="folderName">A name of the media library root folder.</param>
    private bool IsFolderNameUnique(string folderName)
    {
        MediaLibraryInfo library = MediaLibraryInfo.Provider.Get()
                                                            .TopN(1)
                                                            .Column("LibraryID")
                                                            .WhereEquals("LibraryFolder", folderName)
                                                            .WhereEquals("LibrarySiteID", SiteContext.CurrentSiteID)
                                                            .FirstOrDefault();

        return (library == null) || (MediaLibraryID == library.LibraryID);
    }


    /// <summary>
    /// Returns a value indicating whether the current user is authorized to manage the edited media library.
    /// </summary>
    private bool CheckPermissions()
    {
        if (LibraryInfo != null)
        {
            return MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "Manage");
        }
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.MediaLibrary", "Manage"))
        {
            return true;
        }

        return false;
    }

    #endregion


    #region "Form handlers"

    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        if (MediaLibraryID == 0)
        {
            MediaLibraryID = editElem.EditedObject.Generalized.ObjectID;
        }
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    protected void editElem_OnBeforeSave(object sender, EventArgs e)
    {
        if (editElem.RedirectUrlAfterCreate == null)
        {
            editElem.RedirectUrlAfterCreate = String.Empty;
        }
    }


    /// <summary>
    /// OnAfterValidate event handler.
    /// </summary>
    protected void editElem_OnAfterValidate(object sender, EventArgs e)
    {
        bool error = false;

        FormEngineUserControl folderControl = editElem.FieldControls["LibraryFolder"];
        if (folderControl.Enabled)
        {
            folderControl.Text = URLHelper.GetSafeFileName(folderControl.Text.Trim(), SiteContext.CurrentSiteName);

            // Emptiness is validated in Basic form
            if (!String.IsNullOrEmpty(folderControl.Text))
            {
                Validator validator = new Validator().IsFolderName(folderControl.Text, GetString("media.error.FolderNameIsNotValid")).MatchesCondition(folderControl.Text, x => x != "." && x != "..", GetString("media.error.FolderNameIsRelative")).MatchesCondition(folderControl.Text, IsFolderNameUnique, GetString("media.error.FolderExists"));

                if (!validator.IsValid)
                {
                    editElem.DisplayErrorLabel("LibraryFolder", HTMLHelper.HTMLEncode(validator.Result));
                    error = true;
                }
            }
        }

        editElem.StopProcessing = error;
    }


    /// <summary>
    /// OnBeforeValidate event handler.
    /// </summary>
    protected void editElem_OnBeforeValidate(object sender, EventArgs e)
    {
        // Check permission before save        
        if (!CheckPermissions())
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("Manage"));
            editElem.StopProcessing = true;
            return;
        }

        // Get the code name of the edited media library
        if ((MediaLibraryID == 0) && (DisplayMode == ControlDisplayModeEnum.Simple))
        {
            editElem.FieldControls["LibraryName"].Text = ValidationHelper.GetCodeName(editElem.FieldControls["LibraryDisplayName"].Text, null, "_group_" + Guid.NewGuid());
        }
    }

    #endregion
}