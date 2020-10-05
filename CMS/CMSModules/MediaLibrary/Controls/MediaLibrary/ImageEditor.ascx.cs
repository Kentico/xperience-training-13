using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

using IOExceptions = System.IO;


/// <summary>
/// Image editor for media files.
/// </summary>
public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_ImageEditor_Control : CMSUserControl
{
    #region "Variables"

    private Guid mediafileGuid = Guid.Empty;
    private MediaFileInfo mfi;
    private string mCurrentSiteName;
    private int siteId;
    private bool isPreview;
    private byte[] previewFile;
    private bool mEnabled = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the GUID of the instance of the ImageEditor.
    /// </summary>
    public Guid InstanceGUID
    {
        get
        {
            return baseImageEditor.InstanceGUID;
        }
    }


    /// <summary>
    /// Returns the site name from query string 'sitename' or 'siteid' if present, otherwise SiteContext.CurrentSiteName.
    /// </summary>
    private string CurrentSiteName
    {
        get
        {
            if (mCurrentSiteName == null)
            {
                mCurrentSiteName = QueryHelper.GetString("sitename", SiteContext.CurrentSiteName);

                siteId = QueryHelper.GetInteger("siteid", 0);

                SiteInfo site = SiteInfo.Provider.Get(siteId);
                if (site != null)
                {
                    mCurrentSiteName = site.SiteName;
                }
            }
            return mCurrentSiteName;
        }
    }


    /// <summary>
    /// Preview file path.
    /// </summary>
    private string PreviewPath
    {
        get
        {
            return ViewState["PreviewPath"] as String;
        }
        set
        {
            ViewState["PreviewPath"] = value;
        }
    }


    /// <summary>
    /// Preview file path.
    /// </summary>
    private string OldPreviewExt
    {
        get
        {
            return ViewState["OldPreviewExt"] as String;
        }
        set
        {
            ViewState["OldPreviewExt"] = value;
        }
    }


    /// <summary>
    /// Indicates if saving failed.
    /// </summary>
    public bool SavingFailed
    {
        get
        {
            return baseImageEditor.SavingFailed;
        }
        set
        {
            baseImageEditor.SavingFailed = value;
        }
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Loads image type from querystring.
    /// </summary>
    private void baseImageEditor_LoadImageType()
    {
        if (mediafileGuid != Guid.Empty)
        {
            // Load preview image if preview is edited
            if (isPreview)
            {
                baseImageEditor.IsPreview = true;
            }
            baseImageEditor.ImageType = ImageHelper.ImageTypeEnum.MediaFile;
        }
    }


    /// <summary>
    /// Initializes common properties used for processing image.
    /// </summary>
    private void baseImageEditor_InitializeProperties()
    {
        // Process media file
        if (baseImageEditor.ImageType == ImageHelper.ImageTypeEnum.MediaFile)
        {
            // Get mediafile
            mfi = MediaFileInfo.Provider.Get(mediafileGuid, SiteInfoProvider.GetSiteID(CurrentSiteName));
            // If file is not null 
            if (mfi != null)
            {
                MediaLibraryInfo mli = MediaLibraryInfo.Provider.Get(mfi.FileLibraryID);

                if ((mli != null) && (MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "filemodify")))
                {
                    // Load media file thumbnail
                    if (isPreview)
                    {
                        PreviewPath = MediaFileInfoProvider.GetPreviewFilePath(mfi);
                        if (PreviewPath != null)
                        {
                            OldPreviewExt = Path.GetExtension(PreviewPath);
                            try
                            {
                                // Get file contents from file system
                                previewFile = File.ReadAllBytes(PreviewPath);
                            }
                            catch (Exception ex)
                            {
                                Service.Resolve<IEventLogService>().LogException("ImageEditor", "GetPreviewFile", ex);
                            }
                            if (previewFile != null)
                            {
                                baseImageEditor.ImgHelper = new ImageHelper(previewFile);
                            }
                            else
                            {
                                baseImageEditor.LoadingFailed = true;
                                baseImageEditor.ShowError(GetString("img.errors.loading"));
                            }
                        }
                        else
                        {
                            baseImageEditor.LoadingFailed = true;
                            baseImageEditor.ShowError(GetString("img.errors.loading"));
                        }
                    }
                    // Load media file
                    else
                    {
                        mfi.FileBinary = MediaFileInfoProvider.GetFile(mfi, mli.LibraryFolder, CurrentSiteName);
                        // Ensure metafile binary data
                        if (mfi.FileBinary != null)
                        {
                            baseImageEditor.ImgHelper = new ImageHelper(mfi.FileBinary);
                        }
                        else
                        {
                            baseImageEditor.LoadingFailed = true;
                            baseImageEditor.ShowError(GetString("img.errors.loading"));
                        }
                    }
                }
                else
                {
                    baseImageEditor.LoadingFailed = true;
                    baseImageEditor.ShowError(GetString("img.errors.filemodify"));
                }
            }
            else
            {
                baseImageEditor.LoadingFailed = true;
                baseImageEditor.ShowError(GetString("img.errors.loading"));
            }
        }

        // Check that image is in supported formats
        if ((!baseImageEditor.LoadingFailed) && (baseImageEditor.ImgHelper.ImageFormatToString() == null))
        {
            baseImageEditor.LoadingFailed = true;
            baseImageEditor.ShowError(GetString("img.errors.format"));
        }

        // Disable editor if loading failed
        if (baseImageEditor.LoadingFailed)
        {
            Enabled = false;
        }
    }


    /// <summary>
    /// Initialize labels according to current image type.
    /// </summary>
    private void baseImageEditor_InitializeLabels(bool reloadName)
    {
        //Initialize strings depending on image type
        if (baseImageEditor.ImageType == ImageHelper.ImageTypeEnum.MediaFile)
        {
            // Initialize media file thumbnail
            if (isPreview && (previewFile != null) && !String.IsNullOrEmpty(PreviewPath))
            {
                baseImageEditor.TxtFileName.Text = Path.GetFileName(PreviewPath);
                baseImageEditor.LblExtensionValue.Text = Path.GetExtension(PreviewPath);
                baseImageEditor.LblImageSizeValue.Text = DataHelper.GetSizeString(previewFile.Length);
                baseImageEditor.LblWidthValue.Text = baseImageEditor.ImgHelper.ImageWidth.ToString();
                baseImageEditor.LblHeightValue.Text = baseImageEditor.ImgHelper.ImageHeight.ToString();
            }
            // Initialize regular media file
            else if ((mfi != null) && (mfi.FileBinary != null))
            {
                if (!RequestHelper.IsPostBack() || reloadName)
                {
                    baseImageEditor.TxtFileName.Text = mfi.FileName;
                }
                baseImageEditor.LblExtensionValue.Text = mfi.FileExtension;
                baseImageEditor.LblImageSizeValue.Text = DataHelper.GetSizeString(mfi.FileSize);
                baseImageEditor.LblWidthValue.Text = mfi.FileImageWidth.ToString();
                baseImageEditor.LblHeightValue.Text = mfi.FileImageHeight.ToString();
                baseImageEditor.SetTitleAndDescription(mfi.FileTitle, mfi.FileDescription);
                // Set media file info object
                baseImageEditor.SetMetaDataInfoObject(mfi);
            }
        }
    }


    /// <summary>
    /// Saves modified image data.
    /// </summary>
    /// <param name="name">Image name</param>
    /// <param name="extension">Image extension</param>
    /// <param name="mimetype">Image mimetype</param>
    /// <param name="title">Image title</param>
    /// <param name="description">Image description</param>
    /// <param name="binary">Image binary data</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    private void baseImageEditor_SaveImage(string name, string extension, string mimetype, string title, string description, byte[] binary, int width, int height)
    {
        SaveImage(name, extension, mimetype, title, description, binary, width, height);
    }


    /// <summary>
    /// Returns image name, title and description according to image type.
    /// </summary>
    /// <returns>Image name, title and description</returns>
    private void baseImageEditor_GetMetaData()
    {
        if (mfi == null)
        {
            mfi = MediaFileInfo.Provider.Get(mediafileGuid, SiteInfoProvider.GetSiteID(CurrentSiteName));
        }

        if (mfi != null)
        {
            string name = mfi.FileName;
            baseImageEditor.GetNameResult = name;
            baseImageEditor.GetTitleResult = mfi.FileTitle;
            baseImageEditor.GetDescriptionResult = mfi.FileDescription;
        }
    }


    private void baseImageEditor_LoadImageUrl()
    {
        // Use appropriate parameter from URL
        string url = null;
        if (mediafileGuid != Guid.Empty)
        {
            url = "~/CMSPages/GetMediaFile.aspx?fileguid=" + mediafileGuid;
        }
        baseImageEditor.MediaUrl = URLHelper.UpdateParameterInUrl(url, "chset", Guid.NewGuid().ToString());
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        mediafileGuid = QueryHelper.GetGuid("mediafileguid", Guid.Empty);
        isPreview = QueryHelper.GetBoolean("isPreview", false);

        // Get media file information
        if (mfi == null)
        {
            mfi = MediaFileInfo.Provider.Get(mediafileGuid, SiteInfoProvider.GetSiteID(CurrentSiteName));
        }

        if (mfi != null)
        {
            // Get media library information
            MediaLibraryInfo mli = MediaLibraryInfo.Provider.Get(mfi.FileLibraryID);

            if (mli != null)
            {
                // Get path to media file folder
                string path = Path.GetDirectoryName(DirectoryHelper.CombinePath(MediaLibraryInfoProvider.GetMediaLibraryFolderPath(mli.LibraryID), mfi.FilePath));

                // Enable control if permissions are sufficient to edit image
                Enabled = DirectoryHelper.CheckPermissions(path, false, true, true, true);

                if (!Enabled)
                {
                    // Set error message
                    baseImageEditor.ShowError(GetString("img.errors.filesystempermissions"));
                }
            }
        }

        // Enable or disable image editor
        baseImageEditor.Enabled = Enabled;

        baseImageEditor.LoadImageType += baseImageEditor_LoadImageType;
        baseImageEditor.InitializeProperties += baseImageEditor_InitializeProperties;
        baseImageEditor.InitializeLabels += baseImageEditor_InitializeLabels;
        baseImageEditor.SaveImage += baseImageEditor_SaveImage;
        baseImageEditor.GetMetaData += baseImageEditor_GetMetaData;
        baseImageEditor.LoadImageUrl += baseImageEditor_LoadImageUrl;
    }


    /// <summary>
    /// Saves modified image data.
    /// </summary>
    /// <param name="name">Image name</param>
    /// <param name="extension">Image extension</param>
    /// <param name="mimetype">Image mimetype</param>
    /// <param name="title">Image title</param>
    /// <param name="description">Image description</param>
    /// <param name="binary">Image binary data</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    private void SaveImage(string name, string extension, string mimetype, string title, string description, byte[] binary, int width, int height)
    {
        // Process media file
        if (mfi == null)
        {
            mfi = MediaFileInfo.Provider.Get(mediafileGuid, SiteInfoProvider.GetSiteID(CurrentSiteName));
        }

        if (mfi == null)
        {
            return;
        }

        var mli = MediaLibraryInfo.Provider.Get(mfi.FileLibraryID);
        if (mli == null)
        {
            return;
        }

        string path = Path.GetDirectoryName(DirectoryHelper.CombinePath(MediaLibraryInfoProvider.GetMediaLibraryFolderPath(mli.LibraryID), mfi.FilePath));
        bool permissionsOK = DirectoryHelper.CheckPermissions(path, false, true, true, true);

        // Check file write permissions
        FileInfo file = FileInfo.New(MediaFileInfoProvider.GetMediaFilePath(mfi.FileLibraryID, mfi.FilePath));
        if (file != null)
        {
            permissionsOK = permissionsOK && !file.IsReadOnly;
        }

        if (!permissionsOK)
        {
            baseImageEditor.ShowError(GetString("img.errors.filesystempermissions"));
            SavingFailed = true;
            return;
        }

        MediaFileInfo originalMfi = mfi.Clone(true);

        try
        {
            var site = SiteInfo.Provider.Get(mfi.FileSiteID);
            if (site == null)
            {
                throw new NullReferenceException("Site of media file not specified.");
            }

            // Ensure object version
            SynchronizationHelper.EnsureObjectVersion(mfi);

            if (isPreview && !String.IsNullOrEmpty(PreviewPath))
            {
                // Save preview file only if it was modified
                if (binary != null)
                {
                    string previewExt = !String.IsNullOrEmpty(extension) && (extension != OldPreviewExt) ? extension : OldPreviewExt;
                    string previewName = Path.GetFileNameWithoutExtension(PreviewPath);
                    string previewFolder = Path.EnsureForwardSlashes(DirectoryHelper.CombinePath(Path.GetDirectoryName(mfi.FilePath).TrimEnd('/'), MediaLibraryHelper.GetMediaFileHiddenFolder(site.SiteName)));

                    // Delete old preview files with thumbnails
                    MediaFileInfoProvider.DeleteMediaFilePreview(SiteContext.CurrentSiteName, mli.LibraryID, mfi.FilePath);
                    MediaFileInfoProvider.DeleteMediaFilePreviewThumbnails(mfi);

                    // Save preview file
                    MediaFileInfoProvider.SaveFileToDisk(site.SiteName, mli.LibraryFolder, previewFolder, previewName, previewExt, mfi.FileGUID, binary, false, false);

                    SynchronizationHelper.LogObjectChange(mfi, TaskTypeEnum.UpdateObject);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(mimetype))
                {
                    mfi.FileMimeType = mimetype;
                }

                mfi.FileTitle = title;
                mfi.FileDescription = description;

                if (width > 0)
                {
                    mfi.FileImageWidth = width;
                }
                if (height > 0)
                {
                    mfi.FileImageHeight = height;
                }
                if (binary != null)
                {
                    mfi.FileBinary = binary;
                    mfi.FileSize = binary.Length;
                }

                string newExt = null;
                string newName = null;
                if (!String.IsNullOrEmpty(extension))
                {
                    newExt = extension;
                }
                if (!String.IsNullOrEmpty(name))
                {
                    newName = name;
                }

                // If file name or extension changed move preview file and remove all ald thumbnails
                if (NameOrExtensionWasModified(newName, newExt))
                {
                    string fileName = (newName ?? mfi.FileName);
                    string fileExt = (newExt ?? mfi.FileExtension);

                    string newPath = MediaFileInfoProvider.GetMediaFilePath(mfi.FileLibraryID, DirectoryHelper.CombinePath(Path.GetDirectoryName(mfi.FilePath), fileName) + fileExt);

                    // Rename file only if file with same name does not exsists
                    if (File.Exists(newPath))
                    {
                        baseImageEditor.ShowError(GetString("img.errors.fileexists"));
                        SavingFailed = true;
                        return;
                    }

                    // Ensure max length of file path
                    if (newPath.Length >= 260)
                    {
                        throw new IOExceptions.PathTooLongException();
                    }

                    // Remove old thumbnails
                    MediaFileInfoProvider.DeleteMediaFileThumbnails(mfi);
                    MediaFileInfoProvider.DeleteMediaFilePreviewThumbnails(mfi);

                    // Move media file
                    MediaFileInfoProvider.MoveMediaFile(site.SiteName, mli.LibraryID, mfi.FilePath, DirectoryHelper.CombinePath(Path.GetDirectoryName(mfi.FilePath), fileName) + fileExt);

                    // Set new file name or extension
                    mfi.FileName = fileName;
                    mfi.FileExtension = fileExt;
                    mfi.FileMimeType = MimeTypeHelper.GetMimetype(fileExt);

                    // Ensure new binary
                    if (binary != null)
                    {
                        mfi.FileBinary = binary;
                        mfi.FileSize = binary.Length;
                    }
                }
                else
                {
                    // Remove original media file before save if the image was modified
                    if (binary != null)
                    {
                        // Remove old thumbnails
                        MediaFileInfoProvider.DeleteMediaFileThumbnails(mfi);

                        string filePath = MediaFileInfoProvider.GetMediaFilePath(mfi.FileLibraryID, mfi.FilePath);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                }

                // Save new data
                mfi.EnsureUniqueFileName(false);
                MediaFileInfo.Provider.Set(mfi);
            }
        }
        catch (Exception e)
        {
            // Log exception
            Service.Resolve<IEventLogService>().LogException("ImageEditor", "Save file", e);

            baseImageEditor.ShowError(GetString("img.errors.processing"), tooltipText: e.Message);
            SavingFailed = true;
            // Save original media file info
            originalMfi.EnsureUniqueFileName(false);
            MediaFileInfo.Provider.Set(originalMfi);
        }
    }


    /// <summary>
    /// Returns <code>true</code> if <paramref name="newName"/> or <paramref name="newExtension"/> does not match <see cref="mfi"/>s attributes.
    /// </summary>
    /// <param name="newName">New file name.</param>
    /// <param name="newExtension">New extension.</param>
    private bool NameOrExtensionWasModified(string newName, string newExtension)
    {
        return (!String.IsNullOrEmpty(newName) && !string.Equals(mfi.FileName, newName, StringComparison.InvariantCulture))
               || (!String.IsNullOrEmpty(newExtension) && !string.Equals(mfi.FileExtension, newExtension, StringComparison.InvariantCultureIgnoreCase));
    }

    #endregion


    #region "Undo redo functionality"

    /// <summary>
    /// Returns true if the files are stored only in DB or user has disk read/write permissions. Otherwise false.
    /// </summary>
    public bool IsUndoRedoPossible()
    {
        return baseImageEditor.IsUndoRedoPossible();
    }


    /// <summary>
    /// Returns true if there is a previous version of the file which is being modified.
    /// </summary>
    public bool IsUndoEnabled()
    {
        return baseImageEditor.IsUndoEnabled();
    }


    /// <summary>
    /// Returns true if there is a next version of the file which is being modified.
    /// </summary>
    public bool IsRedoEnabled()
    {
        return baseImageEditor.IsRedoEnabled();
    }


    /// <summary>
    /// Processes the undo action.
    /// </summary>
    public void ProcessUndo()
    {
        baseImageEditor.ProcessUndo();
    }


    /// <summary>
    /// Processes the redo action.
    /// </summary>
    public void ProcessRedo()
    {
        baseImageEditor.ProcessRedo();
    }


    /// <summary>
    /// Saves current version of image and discards all other versions.
    /// </summary>
    public void SaveCurrentVersion()
    {
        baseImageEditor.SaveCurrentVersion();
    }

    #endregion
}