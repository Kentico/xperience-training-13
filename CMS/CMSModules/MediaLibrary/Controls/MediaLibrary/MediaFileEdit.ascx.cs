using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

using IOExceptions = System.IO;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileEdit : CMSAdminControl
{
    #region "Event & delegates"

    /// <summary>
    /// Event fired after saved succeeded.
    /// </summary>
    public event OnActionEventHandler Action;

    #endregion


    #region "Private variables"

    private MediaFileInfo mFileInfo;
    private MediaLibraryInfo mLibraryInfo;
    private SiteInfo mLibrarySiteInfo;
    private bool mForceReload;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether the custom fields tab is displayed.
    /// </summary>
    private bool HasCustomFields
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the current file has a preview.
    /// </summary>
    private bool HasPreview
    {
        get
        {
            return MediaLibraryHelper.HasPreview(LibrarySiteInfo.SiteName, MediaLibraryID, FileInfo.FilePath);
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
            if (pnlTabs.SelectedTab == tabEdit)
            {
                return plcMessEdit;
            }
            if (pnlTabs.SelectedTab == tabCustomFields)
            {
                return plcMessCustom;
            }
            return base.MessagesPlaceHolder;
        }
    }


    /// <summary>
    /// Currently edited file info.
    /// </summary>
    public MediaFileInfo FileInfo
    {
        get
        {
            if ((mFileInfo == null) && (FileID > 0))
            {
                mFileInfo = MediaFileInfo.Provider.Get(FileID);
            }
            return mFileInfo;
        }
        set
        {
            mFileInfo = value;
        }
    }


    /// <summary>
    /// File ID.
    /// </summary>
    public int FileID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["FileID"], 0);
        }
        set
        {
            ViewState["FileID"] = value;
            FileInfo = null;
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
    /// Current folder path.
    /// </summary>
    public string FolderPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FolderPath"], "");
        }
        set
        {
            ViewState["FolderPath"] = value;
        }
    }


    /// <summary>
    /// Media library ID.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["MediaLibraryID"], 0);
        }
        set
        {
            ViewState["MediaLibraryID"] = value;
        }
    }


    /// <summary>
    /// Gets library info object.
    /// </summary>
    public MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (MediaLibraryID > 0))
            {
                LibraryInfo = MediaLibraryInfo.Provider.Get(MediaLibraryID);
            }
            return mLibraryInfo;
        }
        set
        {
            mLibraryInfo = value;
        }
    }


    /// <summary>
    /// Info on the site related to the current library.
    /// </summary>
    public SiteInfo LibrarySiteInfo
    {
        get
        {
            if (mLibrarySiteInfo == null)
            {
                mLibrarySiteInfo = SiteInfo.Provider.Get(LibraryInfo.LibrarySiteID);
            }
            return mLibrarySiteInfo;
        }
        set
        {
            mLibrarySiteInfo = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site
    /// Required for versions tab to ensure that control IsLiveSite property presits postback
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return false;
        }
        set
        {
        }
    }

    #endregion


    /// <summary>
    /// Page init event
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set IsLiveSite
        imagePreview.IsLiveSite = IsLiveSite;
        mediaPreview.IsLiveSite = IsLiveSite;
        fileUplPreview.IsLiveSite = IsLiveSite;

        formMediaFileCustomFields.IsLiveSite = IsLiveSite;
        formMediaFileCustomFields.StopProcessing = true;
    }


    /// <summary>
    /// Page pre render event
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if ((FileInfo != null) && (LibraryInfo != null) && HasPreview)
        {
            plcPreview.Visible = true;

            string fileName = AttachmentHelper.GetFullFileName(FileInfo.FileName, FileInfo.FileExtension);
            string permanentUrl = MediaFileURLProvider.GetMediaFileUrl(FileInfo.FileGUID, fileName);
            permanentUrl = URLHelper.UpdateParameterInUrl(permanentUrl, "preview", "1");            

            lblPreviewPermaLink.Text = GetFileLinkHtml(permanentUrl, LibraryInfo.LibrarySiteID);

            if (MediaLibraryHelper.IsExternalLibrary(SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder))
            {
                plcPrevDirPath.Visible = false;
            }
            else
            {
                plcPrevDirPath.Visible = true;
                var directUrl = GetPreviewDirectPath();
                lblPrevDirectLinkVal.Text = GetFileLinkHtml(directUrl, LibraryInfo.LibrarySiteID);
            }
        }
        else
        {
            lblNoPreview.Text = GetString("media.file.nothumb");

            plcNoPreview.Visible = true;
            plcPreview.Visible = false;
        }
        pnlUpdatePreviewDetails.Update();

        // Refresh versions tab if selected and reload was forced
        if (mForceReload && (pnlTabs.SelectedTabIndex == tabVersions.Index))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "ReloadVersionsTab", "$cmsj(\"#" + objectVersionList.RefreshButton.ClientID + "\").click();", true);
        }
    }


    /// <summary>
    /// Page load
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        objectVersionList.StopProcessing = StopProcessing;
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            ReLoadUserControl(false);
        }

        plcMessEdit.IsLiveSite = plcMessCustom.IsLiveSite = IsLiveSite;

        // Initialize header actions on edit tab
        btnEdit.HeaderActions = headerActionsEdit;
        btnEdit.ComponentName = headerActionsEdit.ComponentName = "MediaFile_TabEdit";

        // Register event handler to provide refresh file information
        ComponentEvents.RequestEvents.RegisterForComponentEvent(headerActionsEdit.ComponentName, "refresh", (s, args) => RefreshFileInformation());

        // Initialize header actions on custom fields tab
        BasicForm customFields = formMediaFileCustomFields;
        if (customFields != null)
        {
            FormSubmitButton btnCustom = customFields.SubmitButton;
            btnCustom.HeaderActions = headerActionsCustom;
            btnCustom.ComponentName = headerActionsCustom.ComponentName = "MediaFile_TabCustomFields";
        }

        fileUplPreview.StopProcessing = StopProcessing;
    }


    #region "Public methods"

    /// <summary>
    /// Reloads controls content.
    /// </summary>
    public void ReLoadUserControl()
    {
        ReLoadUserControl(true);
    }


    /// <summary>
    /// Reloads controls content.
    /// </summary>
    /// <param name="forceReload">Indicates whether the content should be reloaded as well</param>
    public void ReLoadUserControl(bool forceReload)
    {
        if (!StopProcessing)
        {
            Visible = true;
            SetupControls(forceReload);
            mForceReload = forceReload;
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Ensures required actions when the file was saved recently.
    /// </summary>
    public void AfterSave()
    {
        SetupFile();
        SetupVersions();
        pnlUpdateVersions.Update();
    }


    /// <summary>
    /// Sets default values and clear textboxes.
    /// </summary>
    public void SetDefault()
    {
        txtEditDescription.Text = "";
        txtEditName.Text = "";
        txtEditTitle.Text = "";
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    /// <param name="forceReload">Indicates whether the content should be reloaded as well</param>
    private void SetupControls(bool forceReload)
    {
        ShowProperTabs(forceReload);
    }


    private static string GetFileLinkHtml(string relativeUrl, SiteInfoIdentifier site)
    {
        var absoluteUrl = DocumentURLProvider.GetAbsoluteUrl(relativeUrl, site);
        var path = new Uri(absoluteUrl).PathAndQuery;
        return String.Format("<span class=\"form-control-text\"><a href=\"{0}\" target=\"_blank\">{1}</a></span>", absoluteUrl, path);
    }


    /// <summary>
    /// Setup general values.
    /// </summary>
    private void SetupFile()
    {
        // Get file and library info
        if ((FileInfo != null) && (LibraryInfo != null))
        {
            formMediaFileCustomFields.IsLiveSite = IsLiveSite;

            if (MediaLibraryHelper.IsExternalLibrary(SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder))
            {
                plcDirPath.Visible = false;
            }
            else
            {
                string directUrl = MediaFileURLProvider.GetMediaFileUrl(LibrarySiteInfo.SiteName, LibraryInfo.LibraryFolder, FileInfo.FilePath);
                ltrDirPathValue.Text = GetFileLinkHtml(directUrl, LibraryInfo.LibrarySiteID);
            }

            var permanentUrl = MediaFileURLProvider.GetMediaFileUrl(FileInfo.FileGUID, AttachmentHelper.GetFullFileName(FileInfo.FileName, FileInfo.FileExtension));
            ltrPermaLinkValue.Text = GetFileLinkHtml(permanentUrl, LibraryInfo.LibrarySiteID);
            if (ImageHelper.IsImage(FileInfo.FileExtension))
            {
                // Ensure max side size 200
                int[] maxsize = ImageHelper.EnsureImageDimensions(0, 0, 200, FileInfo.FileImageWidth, FileInfo.FileImageHeight);
                imagePreview.Width = maxsize[0];
                imagePreview.Height = maxsize[1];

                // If is Image show image properties
                imagePreview.URL = URLHelper.AddParameterToUrl(permanentUrl, "maxsidesize", "200");
                imagePreview.URL = URLHelper.AddParameterToUrl(imagePreview.URL, "chset", Guid.NewGuid().ToString());
                plcImagePreview.Visible = true;
                plcMediaPreview.Visible = false;

                pnlPrew.Visible = true;
            }
            else if (MediaHelper.IsAudioVideo(FileInfo.FileExtension))
            {
                mediaPreview.Height = MediaHelper.IsAudio(FileInfo.FileExtension) ? 45 : 180;
                mediaPreview.Width = 270;

                mediaPreview.AutoPlay = false;
                mediaPreview.AVControls = true;
                mediaPreview.Loop = false;
                mediaPreview.Type = FileInfo.FileExtension;

                // If is Image show image properties
                mediaPreview.Url = permanentUrl;
                plcMediaPreview.Visible = true;
                plcImagePreview.Visible = false;

                pnlPrew.Visible = true;
            }
            else
            {
                pnlPrew.Visible = false;
            }
        }
        else
        {
            pnlPrew.Visible = false;
        }
    }


    /// <summary>
    /// Setup preview values.
    /// </summary>
    private void SetupPreview()
    {
        if ((FileInfo != null) && (LibraryInfo != null))
        {
            fileUplPreview.EnableUpdate = MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify");
            fileUplPreview.StopProcessing = false;
            fileUplPreview.IsLiveSite = IsLiveSite;
            fileUplPreview.LibraryFolderPath = FolderPath;
            fileUplPreview.LibraryID = LibraryInfo.LibraryID;
            fileUplPreview.MediaFileID = FileID;
            fileUplPreview.FileInfo = FileInfo;
            fileUplPreview.ReloadData();
        }
        else
        {
            plcPreview.Visible = false;
        }
    }


    /// <summary>
    /// Setup versions tab values.
    /// </summary>
    private void SetupVersions()
    {
        if (!IsLiveSite && (FileInfo != null) && ObjectVersionManager.DisplayVersionsTab(FileInfo))
        {
            tabVersions.Visible = true;
            tabVersions.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
            objectVersionList.Visible = true;
            objectVersionList.Object = FileInfo;

            // Bind refresh tab script to tab click event
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TabVersionsOnClick", ScriptHelper.GetScript("$cmsj(document).ready(function () {$cmsj(\"#" + tabVersions.ClientID + "_head\").children().click( function() { $cmsj(\"#" + objectVersionList.RefreshButton.ClientID + "\").click();});})"));

            // Register script to refresh content
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ReloadMediaFileEdit", ScriptHelper.GetScript("function RefreshContent() { var button = document.getElementById('" + btnHidden.ClientID + "'); if (button){button.click();}}"));
        }
        else
        {
            tabVersions.Visible = false;
            objectVersionList.Visible = false;
            objectVersionList.StopProcessing = true;
        }
    }


    /// <summary>
    /// Setup edit values.
    /// </summary>
    private void SetupEdit()
    {
        if (FileInfo != null)
        {
            // Indicates if form is editable
            bool formEnabled = true;

            // Get system file info
            string filePath = MediaFileInfoProvider.GetMediaFilePath(FileInfo.FileLibraryID, FileInfo.FilePath);

            // Disable edit form if can't modify file
            if (!DirectoryHelper.CheckPermissions(filePath, false, true, true, true))
            {
                DisableEditForm();
                formEnabled = false;
            }

            UserInfo currentUserInfo = MembershipContext.AuthenticatedUser;
            SiteInfo currentSiteInfo = SiteContext.CurrentSite;

            FillFieldsOnBasisOfFileInfo(currentUserInfo, currentSiteInfo);

            if (File.Exists(filePath))
            {
                // Fill size and modified when on file system
                bool refreshVisible = FillFileSystemDetails(filePath, currentUserInfo, currentSiteInfo);

                // If file has different size or modified date on disk than in database and form is editable
                if (refreshVisible && formEnabled)
                {
                    AddRefreshButton();
                }
            }
        }
        else
        {
            ClearBasicFields();
        }
    }


    /// <summary>
    /// Method fills basic and other text fields.
    /// </summary>
    /// <remarks>
    /// <see cref="FileInfo"/> is used as a source for: filename, title, description, created by, created when, modified when, size, file extension and file dimension.
    /// </remarks>
    private void FillFieldsOnBasisOfFileInfo(UserInfo currentUserInfo, SiteInfo currentSiteInfo)
    {
        // Fill filename, title and description
        FillBasicFields();

        FillCreatedByField();

        FillCreatedWhenField(currentUserInfo, currentSiteInfo);

        FillModifiedWhenField(currentUserInfo, currentSiteInfo);

        FillSizeField();

        FillExtensionsField();

        FillDimensionsField();
    }


    private void DisableEditForm()
    {
        btnEdit.Enabled = false;
        pnlTabEdit.Enabled = false;
        plcMessEdit.ShowError(GetString("media.error.filesystempermissions"));
    }


    /// <summary>
    /// Method fills textboxes from file properties (modified when and size). Uses information from file system.
    /// </summary>
    /// <returns><c>True</c> if <see cref="MediaFileInfo.FileSize"/> or <see cref="MediaFileInfo.FileModifiedWhen"/> are different.</returns>
    private bool FillFileSystemDetails(string filePath, UserInfo currentUserInfo, SiteInfo currentSiteInfo)
    {
        FileInfo sysFileInfo = CMS.IO.FileInfo.New(filePath);

        // File modified when
        var hasChanged = FillFileSystemModifiedWhenField(currentUserInfo, currentSiteInfo, sysFileInfo);

        // File size
        hasChanged |= FillFileSizeField(sysFileInfo);

        return hasChanged;
    }


    private void FillSizeField()
    {
        lblSizeVal.Text = DataHelper.GetSizeString(FileInfo.FileSize);
    }


    private void FillExtensionsField()
    {
        lblExtensionVal.Text = FileInfo.FileExtension.TrimStart('.').ToLowerCSafe();
    }


    /// <summary>
    /// Fills dimensions textbox - uses format "width x height". Together with that hides textbox if file isn't image.
    /// </summary>
    private void FillDimensionsField()
    {
        if (ImageHelper.IsImage(FileInfo.FileExtension))
        {
            lblDimensionsVal.Text = FileInfo.FileImageWidth + " x " + FileInfo.FileImageHeight;
            plcDimensions.Visible = true;
        }
        else
        {
            plcDimensions.Visible = false;
        }
    }


    private void AddRefreshButton()
    {
        headerActionsEdit.AddAction(new HeaderAction
        {
            Text = GetString("general.refresh"),
            CommandName = "refresh",
            ButtonStyle = ButtonStyle.Default
        });
    }


    /// <summary>
    /// Fills file system modified when field when file on disk has different value (modified when) than value stored in database.
    /// </summary>
    /// <param name="modified">Value of modified date stored in database</param>
    /// <returns><c>True</c> if values are different</returns>
    private bool FillFileSystemModifiedWhenField(UserInfo currentUserInfo, SiteInfo currentSiteInfo, FileInfo sysFileInfo)
    {
        DateTime modified = ValidationHelper.GetDateTime(FileInfo.FileModifiedWhen, DateTimeHelper.ZERO_TIME);

        bool hasModifiedDateChanged;

        DateTime fileModified = ValidationHelper.GetDateTime(sysFileInfo.LastWriteTime, DateTimeHelper.ZERO_TIME);
        // Display only if system time is 
        if ((fileModified - modified).TotalSeconds > 5)
        {
            lblFileModifiedVal.Text = TimeZoneHelper.ConvertToUserTimeZone(fileModified, true, currentUserInfo, currentSiteInfo);

            plcFileModified.Visible = true;
            hasModifiedDateChanged = true;
        }
        else
        {
            plcFileModified.Visible = false;
            hasModifiedDateChanged = false;
        }

        return hasModifiedDateChanged;
    }


    /// <summary>
    /// Fill file size field if size on disk is different than value in database
    /// </summary>
    /// <returns><c>True</c> if sizes are different</returns>
    private bool FillFileSizeField(FileInfo sysFileInfo)
    {
        bool hasModifiedFileSizeChanged;

        if (sysFileInfo.Length != FileInfo.FileSize)
        {
            lblFileSizeVal.Text = DataHelper.GetSizeString(sysFileInfo.Length);
            plcFileSize.Visible = true;
            hasModifiedFileSizeChanged = true;
        }
        else
        {
            plcFileSize.Visible = false;
            hasModifiedFileSizeChanged = false;
        }

        return hasModifiedFileSizeChanged;
    }


    private void FillModifiedWhenField(UserInfo currentUserInfo, SiteInfo currentSiteInfo)
    {
        lblModifiedVal.Text = ValidateAndConvertToUserTimeZone(FileInfo.FileModifiedWhen, currentUserInfo, currentSiteInfo);
    }


    private void FillCreatedWhenField(UserInfo currentUserInfo, SiteInfo currentSiteInfo)
    {
        lblCreatedWhenVal.Text = ValidateAndConvertToUserTimeZone(FileInfo.FileCreatedWhen, currentUserInfo, currentSiteInfo);
    }


    /// <summary>
    /// Fills created by textbox with user <see cref="UserInfo.FullName"/>.
    /// If <see cref="UserInfo.FullName"/> isn't available, general "not available" text from resources is showed instead. 
    /// </summary>
    private void FillCreatedByField()
    {
        string userName;

        UserInfo userInfo = UserInfoProvider.GetFullUserInfo(FileInfo.FileCreatedByUserID);
        if ((userInfo == null) || userInfo.IsPublic())
        {
            userName = GetString("general.na");
        }
        else
        {
            userName = HTMLHelper.HTMLEncode(userInfo.FullName);
        }

        lblCreatedByVal.Text = userName;
    }


    private void FillBasicFields()
    {
        txtEditName.Text = FileInfo.FileName;
        txtEditDescription.Text = FileInfo.FileDescription;
        txtEditTitle.Text = FileInfo.FileTitle;
    }


    private string ValidateAndConvertToUserTimeZone(DateTime date, UserInfo currentUserInfo, SiteInfo currentSiteInfo)
    {
        var validatedDate = ValidationHelper.GetDateTime(date, DateTimeHelper.ZERO_TIME);
        return TimeZoneHelper.ConvertToUserTimeZone(validatedDate, true, currentUserInfo, currentSiteInfo);
    }


    private void ClearBasicFields()
    {
        txtEditName.Text = "";
        txtEditDescription.Text = "";
        txtEditTitle.Text = "";
    }


    /// <summary>
    /// Header action click event handler.
    /// </summary>
    protected void headerActionsEdit_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "refresh":
                RefreshFileInformation();
                break;
        }
    }


    /// <summary>
    /// Raises action event.
    /// </summary>
    /// <param name="actionName">Name of the action occurring</param>
    /// <param name="actionArgument">Argument related to the action</param>
    private void RaiseOnAction(string actionName, object actionArgument)
    {
        if (Action != null)
        {
            Action(actionName, actionArgument);
        }
    }


    /// <summary>
    /// Display or hides the tabs according to the ViewMode setting.
    /// </summary>
    /// <param name="forceReload">Indicates whether the content should be reloaded as well</param> 
    private void ShowProperTabs(bool forceReload)
    {
        ScriptHelper.RegisterJQuery(Page);

        // We need to remove the header text for unused tabs, because of bug
        // in AjaxToolkit Tab control (when hiding the tab text is still visible)
        tabGeneral.HeaderText = GetString("general.file");
        tabPreview.HeaderText = GetString("general.thumbnail");
        tabEdit.HeaderText = GetString("general.edit");
        tabCustomFields.HeaderText = GetString("general.customfields");
        tabVersions.HeaderText = GetString("objectversioning.tabtitle");

        DisplayCustomFields();

        if (forceReload)
        {
            SetupEdit();
        }
        SetupFile();
        SetupPreview();
        SetupVersions();
    }


    /// <summary>
    /// Handles custom fields tab displaying.
    /// </summary>
    private void DisplayCustomFields()
    {
        // Initialize DataForm
        if ((FileID > 0) && Visible)
        {
            formMediaFileCustomFields.OnBeforeSave += formMediaFileCustomFields_OnBeforeSave;
            formMediaFileCustomFields.OnAfterSave += formMediaFileCustomFields_OnAfterSave;
            formMediaFileCustomFields.OnValidationFailed += formMediaFileCustomFields_OnValidationFailed;
            formMediaFileCustomFields.OnBeforeDataLoad += formMediaFileCustomFields_OnBeforeDataLoad;

            formMediaFileCustomFields.IsLiveSite = IsLiveSite;
            formMediaFileCustomFields.StopProcessing = false;
            formMediaFileCustomFields.Info = FileInfo;
            formMediaFileCustomFields.ID = "formMediaFileCustomFields" + FileID;

            formMediaFileCustomFields.HideSystemFields = true;

            formMediaFileCustomFields.ReloadData();
        }

        pnlUpdateCustomFields.Update();
    }


    /// <summary>
    /// Gets direct path for preview image of currently edited media file.
    /// </summary>
    private string GetPreviewDirectPath()
    {
        string prevUrl = "";

        string hiddenFolder = String.Empty;
        if (!Path.GetDirectoryName(FileInfo.FilePath).EndsWithCSafe(MediaLibraryHelper.GetMediaFileHiddenFolder(SiteContext.CurrentSiteName)))
        {
            hiddenFolder = MediaLibraryHelper.GetMediaFileHiddenFolder(SiteContext.CurrentSiteName);
        }

        // Get relative folder under media library when file is located
        string dirName = Path.GetDirectoryName(FileInfo.FilePath);
        // Get preview file searching pattern
        string previewFilePattern = MediaLibraryHelper.GetPreviewFileName(FileInfo.FileName, FileInfo.FileExtension, ".*", SiteContext.CurrentSiteName);

        // Create path for thumbnails searching
        string previewFolder = String.Format("{0}{1}{2}", String.IsNullOrEmpty(dirName) ? String.Empty : dirName + "\\",
            String.IsNullOrEmpty(hiddenFolder) ? String.Empty : hiddenFolder + "\\", previewFilePattern);

        // Get absolute path
        string previewPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(FileInfo.FileLibraryID) + "\\" + previewFolder;

        if (Directory.Exists(Path.GetDirectoryName(previewPath)))
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(previewPath), Path.GetFileName(previewPath));
            if (files.Length > 0)
            {
                previewFolder = Path.EnsureForwardSlashes(Path.GetDirectoryName(previewFolder), true);
                string prevFileName = Path.GetFileName(files[0]);

                prevUrl = MediaFileURLProvider.GetMediaFileUrl(SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder, previewFolder + '/' + prevFileName);
            }
        }

        return prevUrl;
    }

    #endregion


    #region "Edit tab"

    /// <summary>
    /// Edit file event handler.
    /// </summary>
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        // Check 'File modify' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filemodify"));

            SetupEdit();

            // Update form
            pnlUpdateFileInfo.Update();
            return;
        }

        FileInfo fi = CMS.IO.FileInfo.New(MediaFileInfoProvider.GetMediaFilePath(SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder, FilePath));
        if ((fi != null) && (LibraryInfo != null))
        {
            // Check if the file exists
            if (!fi.Exists)
            {
                ShowError(GetString("media.error.FileDoesNotExist"));
                return;
            }

            string path = Path.EnsureForwardSlashes(FilePath);
            string fileName = URLHelper.GetSafeFileName(txtEditName.Text.Trim(), SiteContext.CurrentSiteName, false);
            string origFileName = Path.GetFileNameWithoutExtension(fi.FullName);

            Validator fileNameValidator = new Validator()
                .NotEmpty(fileName, GetString("media.error.FileNameIsEmpty"))
                .IsFileName(fileName, GetString("media.error.FileNameIsNotValid"))
                .MatchesCondition(fileName, x => x != "." && x != "..", GetString("media.error.FileNameIsRelative"));

            if (!fileNameValidator.IsValid)
            {
                ShowError(HTMLHelper.HTMLEncode(fileNameValidator.Result));
                return;
            }

            if (FileInfo != null)
            {
                if ((MembershipContext.AuthenticatedUser != null) && (!MembershipContext.AuthenticatedUser.IsPublic()))
                {
                    FileInfo.FileModifiedByUserID = MembershipContext.AuthenticatedUser.UserID;
                }

                FileInfo.FileModifiedWhen = DateTime.Now;

                // Check if filename is changed ad move file if necessary
                if (fileName != origFileName)
                {
                    try
                    {
                        // Check if file with new file name exists
                        string newFilePath = Path.GetDirectoryName(fi.FullName) + "\\" + fileName + fi.Extension;
                        if (!File.Exists(newFilePath))
                        {
                            string newPath = (string.IsNullOrEmpty(Path.GetDirectoryName(path)) ? "" : Path.GetDirectoryName(path) + "/") + fileName + FileInfo.FileExtension;
                            MediaFileInfoProvider.MoveMediaFile(SiteContext.CurrentSiteName, FileInfo.FileLibraryID, path, newPath);
                            FileInfo.FilePath = Path.EnsureForwardSlashes(newPath);
                            FileInfo.FileExtension = Path.GetExtension(newPath);
                            FileInfo.FileMimeType = MimeTypeHelper.GetMimetype(FileInfo.FileExtension);
                        }
                        else
                        {
                            ShowError(GetString("media.error.FileExists"));
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(GetString("media.error.RenameFileException"), ex.Message);
                        return;
                    }
                }
                // Set media file info
                FileInfo.FileName = fileName;
                FileInfo.FileTitle = txtEditTitle.Text;
                FileInfo.FileDescription = txtEditDescription.Text;

                // Save
                MediaFileInfo.Provider.Set(FileInfo);
                FilePath = FileInfo.FilePath;

                UpdateLastWriteTime(fileName, origFileName, fi);

                // Inform user on success
                ShowChangesSaved();

                SetupEdit();
                pnlUpdateFileInfo.Update();

                SetupFile();
                pnlUpdateGeneral.Update();

                SetupPreview();
                pnlUpdatePreviewDetails.Update();

                SetupVersions();
                pnlUpdateVersions.Update();

                RaiseOnAction("rehighlightitem", Path.GetFileName(FileInfo.FilePath));
            }
        }
    }


    private void UpdateLastWriteTime(string fileName, string origFileName, FileInfo fi)
    {
        try
        {
            if (fileName == origFileName)
            {
                fi.LastWriteTime = FileInfo.FileModifiedWhen;
            }
        }
        catch (IOExceptions.IOException exception)
        {
            Service.Resolve<IEventLogService>().LogWarning("Media File Edit", "UPDATEFILE", exception, CurrentSite.SiteID,
                "Cannot update the 'Modified' time stamp, because the file is currently being used by another process.");
            ShowWarning(ResHelper.GetString("mediafile.cantupdatelockedfile"));
        }
    }


    /// <summary>
    /// Refreshes file information.
    /// </summary>
    private void RefreshFileInformation()
    {
        // Check 'File modify' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filemodify"));

            SetupFile();
            return;
        }

        FileInfo fi = CMS.IO.FileInfo.New(MediaFileInfoProvider.GetMediaFilePath(SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder, FilePath));
        if ((fi != null) && (LibraryInfo != null))
        {
            if (FileInfo != null)
            {
                FileInfo.FileModifiedWhen = DateTime.Now;
                // Set media file info
                FileInfo.FileSize = fi.Length;
                if (ImageHelper.IsImage(FileInfo.FileExtension))
                {
                    ImageHelper ih = new ImageHelper();
                    ih.LoadImage(File.ReadAllBytes(fi.FullName));
                    FileInfo.FileImageWidth = ih.ImageWidth;
                    FileInfo.FileImageHeight = ih.ImageHeight;
                }
                FileInfo.FileTitle = txtEditTitle.Text.Trim();
                FileInfo.FileDescription = txtEditDescription.Text.Trim();

                // Save
                MediaFileInfo.Provider.Set(FileInfo);

                // Remove old thumbnails
                MediaFileInfoProvider.DeleteMediaFileThumbnails(FileInfo);

                // Inform user on success
                ShowConfirmation(GetString("media.refresh.success"));

                SetupFile();
                pnlUpdateGeneral.Update();

                SetupPreview();
                pnlUpdatePreviewDetails.Update();

                SetupEdit();
                pnlUpdateFileInfo.Update();

                SetupVersions();
                pnlUpdateVersions.Update();

                RaiseOnAction("rehighlightitem", Path.GetFileName(FileInfo.FilePath));
            }
        }
    }


    /// <summary>
    /// BreadCrumbs in edit file form.
    /// </summary>
    protected void lnkEditList_Click(object sender, EventArgs e)
    {
        // Hide preview/edit form and show unigrid
        RaiseOnAction("showlist", null);
    }


    /// <summary>
    /// Stores new media file info into the DB.
    /// </summary>
    /// <param name="fi">Info on file to be stored</param>
    /// <param name="title">Title of new media file</param>
    /// <param name="description">Description of new media file</param>
    /// <param name="name">Name of new media file</param>
    /// <param name="filePath">File path</param>
    public MediaFileInfo SaveNewFile(FileInfo fi, string title, string description, string name, string filePath)
    {
        string path = Path.EnsureForwardSlashes(filePath);
        string fileName = name;

        string fullPath = fi.FullName;
        string extension = URLHelper.GetSafeFileName(fi.Extension, SiteContext.CurrentSiteName);

        // Check if filename is changed ad move file if necessary
        if (fileName + extension != fi.Name)
        {
            string oldPath = path;
            fullPath = MediaLibraryHelper.EnsureUniqueFileName(Path.GetDirectoryName(fullPath) + "\\" + fileName + extension);
            path = Path.EnsureForwardSlashes(Path.GetDirectoryName(path) + "/" + Path.GetFileName(fullPath)).TrimStart('/');
            MediaFileInfoProvider.MoveMediaFile(SiteContext.CurrentSiteName, MediaLibraryID, oldPath, path, true);
        }

        // Create media file info
        MediaFileInfo fileInfo = new MediaFileInfo(fullPath, LibraryInfo.LibraryID, Path.EnsureForwardSlashes(Path.GetDirectoryName(path)), 0, 0, 0);

        fileInfo.FileTitle = title;
        fileInfo.FileDescription = description;

        // Save media file info
        MediaFileInfoProvider.ImportMediaFileInfo(fileInfo);

        // Save FileID in ViewState
        FileID = fileInfo.FileID;
        FilePath = fileInfo.FilePath;

        return fileInfo;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Button hidden click
    /// </summary>
    protected void btnHidden_Click(object sender, EventArgs e)
    {
        // Refresh tree structure after rollback
        RaiseOnAction("reloadmedialibrary", FileID + "|" + Path.GetDirectoryName(FileInfo.FilePath));

        // Reload data and refresh update panels
        ReLoadUserControl();
        pnlUpdateGeneral.Update();
        pnlUpdatePreviewDetails.Update();
        pnlUpdateFileInfo.Update();
    }


    private void formMediaFileCustomFields_OnValidationFailed(object sender, EventArgs e)
    {
        pnlUpdateCustomFields.Update();
    }


    private void formMediaFileCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        ShowChangesSaved();

        SetupEdit();

        SetupVersions();
        pnlUpdateVersions.Update();
    }


    private void formMediaFileCustomFields_OnBeforeSave(object sender, EventArgs e)
    {
        // Check 'File modify' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filemodify"));

            DisplayCustomFields();
            formMediaFileCustomFields.StopProcessing = true;

            // Update form
            SetupEdit();
        }
    }


    void formMediaFileCustomFields_OnBeforeDataLoad(object sender, EventArgs e)
    {
        // Initialize custom fields tab if visible
        HasCustomFields = formMediaFileCustomFields.FormInformation.GetFormElements(true, false, true).Any();
        if (HasCustomFields)
        {
            if ((formMediaFileCustomFields != null) && formMediaFileCustomFields.SubmitButton.Visible)
            {
                // Register the postback control
                ScriptManager manager = ScriptManager.GetCurrent(Page);
                if (manager != null)
                {
                    manager.RegisterPostBackControl(formMediaFileCustomFields.SubmitButton);
                }
            }

            tabCustomFields.Visible = true;
            plcMediaFileCustomFields.Visible = true;
        }
        else
        {
            formMediaFileCustomFields.StopProcessing = true;
            formMediaFileCustomFields.Enabled = false;
            formMediaFileCustomFields.Visible = false;
            tabCustomFields.Visible = false;
            tabCustomFields.HeaderText = "";
            plcMediaFileCustomFields.Visible = false;
        }
    }

    #endregion
}