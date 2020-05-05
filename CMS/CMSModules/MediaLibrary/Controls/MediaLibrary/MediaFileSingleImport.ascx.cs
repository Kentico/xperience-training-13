using System;

using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileSingleImport : CMSAdminControl
{
    #region "Event & delegates"

    /// <summary>
    /// Delegate used to describe handler of the event required on new file being saved.
    /// </summary>
    /// <param name="file">Info on saved file</param>
    /// <param name="title">New file title</param>
    /// <param name="desc">New file description</param>
    /// <param name="name">New file name</param>
    /// <param name="filePath">Path to the file physical location</param>
    public delegate MediaFileInfo OnSaveRequired(FileInfo file, string title, string desc, string name, string filePath);


    /// <summary>
    /// Event fired when new file should be saved.
    /// </summary>
    public event OnSaveRequired SaveRequired;


    /// <summary>
    /// Event fired after saved succeeded.
    /// </summary>
    public event OnActionEventHandler Action;

    #endregion


    #region "Private variables"

    private SiteInfo mLibrarySiteInfo;
    private MediaLibraryInfo mLibraryInfo;
    private bool mErrorOccurred;
    private FileInfo mCurrentFile;

    #endregion


    #region "Properties"
    
    /// <summary>
    /// Gets or sets library info.
    /// </summary>
    public MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (LibraryID > 0))
            {
                mLibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryID);
            }
            return mLibraryInfo;
        }
        set
        {
            mLibraryInfo = value;
        }
    }


    /// <summary>
    /// ID of the library file is imported to.
    /// </summary>
    public int LibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["LibraryID"], 0);
        }
        set
        {
            ViewState["LibraryID"] = value;
            LibraryInfo = null;
            LibrarySiteInfo = null;
        }
    }


    /// <summary>
    /// Gets or sets library info.
    /// </summary>
    public SiteInfo LibrarySiteInfo
    {
        get
        {
            if ((mLibrarySiteInfo == null) && (LibraryInfo != null))
            {
                mLibrarySiteInfo = SiteInfoProvider.GetSiteInfo(LibraryInfo.LibrarySiteID);
            }
            return mLibrarySiteInfo;
        }
        set
        {
            mLibrarySiteInfo = value;
        }
    }


    /// <summary>
    /// Current file path.
    /// </summary>
    public string FilePath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FilePath"], "");
        }
        set
        {
            ViewState["FilePath"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the error occurred during new file processing.
    /// </summary>
    public bool ErrorOccurred
    {
        get
        {
            return mErrorOccurred;
        }
        private set
        {
            mErrorOccurred = value;
        }
    }


    /// <summary>
    /// Current file info 
    /// </summary>
    private FileInfo CurrentFile
    {
        get
        {
            if ((mCurrentFile == null) && !string.IsNullOrEmpty(FilePath))
            {
                mCurrentFile = FileInfo.New(MediaFileInfoProvider.GetMediaFilePath(LibrarySiteInfo.SiteName, LibraryInfo.LibraryFolder, FilePath));
            }
            return mCurrentFile;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            SetupTexts();
        }
    }


    protected void Page_PreRender()
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            // Reload form if file is set
            if (!String.IsNullOrEmpty(FilePath))
            {
                DisplayForm(CurrentFile);
            }
        }
    }

    /// <summary>
    /// Initializes nested controls.
    /// </summary>
    public void SetupTexts()
    {
        // Initialize labels
        rfvNewFileName.ErrorMessage = GetString("general.requiresvalue");
        tabImport.HeaderText = GetString("media.file.import");

        plcMessInfo.ShowWarning(GetString("media.file.infonew"));
    }


    /// <summary>
    /// Displays new file form.
    /// </summary>
    /// <param name="file">File the form is displayed for</param>
    public void DisplayForm(FileInfo file)
    {
        if (file != null)
        {
            string ext = file.Extension;

            if (!MediaLibraryHelper.IsExtensionAllowed(ext.TrimStart('.')))
            {
                String errorMsg = String.Format(GetString("attach.notallowedextension"), ext, MediaLibraryHelper.GetAllowedExtensions(SiteContext.CurrentSiteName).TrimEnd(';').Replace(";", ", "));

                plcMessInfo.ClearLabels();
                plcMessInfo.ShowError(errorMsg);
                SetFormVisible(false);
            }
            else
            {
                // If is not in DB fill new file form and show it
                SetFormVisible(true);
                txtNewDescripotion.Text = "";
                txtNewFileTitle.Text = Path.GetFileNameWithoutExtension(file.Name);
                txtNewFileName.Text = URLHelper.GetSafeFileName(Path.GetFileNameWithoutExtension(file.Name), SiteContext.CurrentSiteName, false);
            }
        }
    }


    /// <summary>
    /// Sets default values and clear text boxes.
    /// </summary>
    public void SetDefault()
    {
        txtNewFileName.Text = "";
        txtNewFileTitle.Text = "";
        txtNewDescripotion.Text = "";
    }


    /// <summary>
    /// New file event handler.
    /// </summary>
    protected void btnNew_Click(object sender, EventArgs e)
    {
        MediaFileInfo mfi = null;

        try
        {
            string newFileName = txtNewFileName.Text.Trim();
            // Check if the filename is in correct format
            if (!ValidationHelper.IsFileName(newFileName))
            {
                lblErrorNew.Text = GetString("media.rename.wrongformat");
                ErrorOccurred = true;
            }
            else
            {
                // Check 'File create' permission
                if (MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filecreate"))
                {
                    if (LibraryInfo != null)
                    {
                        // Get file and library info
                        if (CurrentFile != null)
                        {
                            if (File.Exists(CurrentFile.FullName))
                            {
                                // Save new file in the DB
                                if (SaveRequired != null)
                                {
                                    mfi = SaveRequired(CurrentFile, txtNewFileTitle.Text.Trim(), txtNewDescripotion.Text.Trim(), URLHelper.GetSafeFileName(newFileName, SiteContext.CurrentSiteName, false), FilePath);
                                    if (mfi != null)
                                    {
                                        mfi.RelatedData = CurrentFile.FullName;
                                    }
                                }
                            }
                            else
                            {
                                lblErrorNew.Text = GetString("media.newfile.notexist");
                                ErrorOccurred = true;
                            }
                        }
                    }
                }
                else
                {
                    lblErrorNew.Text = MediaLibraryHelper.GetAccessDeniedMessage("filecreate");
                    ErrorOccurred = true;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred = true;
            lblErrorNew.Text = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }

        // Display user with error encountered
        if (ErrorOccurred)
        {
            lblErrorNew.Visible = true;
            RaiseOnAction("error");
        }
        else
        {
            RaiseOnAction("save", mfi);
        }
    }


    /// <summary>
    /// BreadCrumbs in new file form.
    /// </summary>
    protected void lnkNewList_Click(object sender, EventArgs e)
    {
        RaiseOnAction("showlist");
    }


    #region "Event methods"

    /// <summary>
    /// Raises action event of specified type.
    /// </summary>
    /// <param name="name">Action name</param>
    /// <param name="argument">Argument</param>
    private void RaiseOnAction(string name, object argument = null)
    {
        if (Action != null)
        {
            // Hide new file form and show unigrid
            Action(name, argument);
        }
    }


    /// <summary>
    /// Sets forms visible property.
    /// </summary>
    /// <param name="visible">Visible flag</param>
    private void SetFormVisible(bool visible)
    {
        txtNewFileName.Visible = visible;
        lblNewFileName.Visible = visible;
        txtNewFileTitle.Visible = visible;
        lnlNewFileTitle.Visible = visible;
        txtNewDescripotion.Visible = visible;
        lblNewDescription.Visible = visible;
        btnNew.Visible = visible;
    }

    #endregion
}