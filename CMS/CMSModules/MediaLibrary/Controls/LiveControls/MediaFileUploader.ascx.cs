using System;

using CMS.Base;
using CMS.Base.UploadExtensions;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_LiveControls_MediaFileUploader : CMSAdminControl
{
    #region "Properties"

    /// <summary>
    /// Delegate of event fired when file has been uploaded.
    /// </summary>
    public delegate void OnAfterFileUploadEventHandler();

    /// <summary>
    /// Event raised when file has been uploaded.
    /// </summary>
    public event OnAfterFileUploadEventHandler OnAfterFileUpload;


    /// <summary>
    /// Gets or sets ID of the media library where the file should be uploaded.
    /// </summary>
    public int LibraryID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the destination path within the media library.
    /// </summary>
    public string DestinationPath
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if preview upload dialog should displayed.
    /// </summary>
    public bool EnableUploadPreview
    {
        get;
        set;
    }


    /// <summary>
    /// Preview suffix for identification of preview file.
    /// </summary>
    public string PreviewSuffix
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        btnUpload.Click += btnUpload_Click;
        ControlsHelper.RegisterPostbackControl(btnUpload);

        // Show preview upload
        if (EnableUploadPreview)
        {
            plcPreview.Visible = true;
        }
    }
    

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        MediaLibraryInfo mli = MediaLibraryInfo.Provider.Get(LibraryID);
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "manage"))
        {
            // Check 'File create' permission
            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "filecreate"))
            {
                RaiseOnNotAllowed("filecreate");
                return;
            }
        }

        if (String.IsNullOrWhiteSpace(fileUploader.FileName) && !fileUploader.HasFile)
        {
            lblError.Text = GetString("media.selectfile");
            lblError.Visible = true;
            return;
        }

        // Check if preview file is image
        if ((previewUploader.HasFile) &&
            (!ImageHelper.IsImage(Path.GetExtension(previewUploader.FileName))) &&
            (Path.GetExtension(previewUploader.FileName).ToLowerCSafe() != ".ico") &&
            (Path.GetExtension(previewUploader.FileName).ToLowerCSafe() != ".tif") &&
            (Path.GetExtension(previewUploader.FileName).ToLowerCSafe() != ".tiff") &&
            (Path.GetExtension(previewUploader.FileName).ToLowerCSafe() != ".wmf"))
        {
            lblError.Text = GetString("Media.File.PreviewIsNotImage");
            lblError.Visible = true;
            return;
        }

        // Check if the preview file with given extension is allowed for library module
        // Check if file with given extension is allowed for library module
        string fileExt = Path.GetExtension(fileUploader.FileName).TrimStart('.');
        string previewFileExt = Path.GetExtension(previewUploader.FileName).TrimStart('.');

        // Check file extension
        if (!MediaLibraryHelper.IsExtensionAllowed(fileExt))
        {
            lblError.Text = String.Format(GetString("media.newfile.extensionnotallowed"), HTMLHelper.HTMLEncode(fileExt));
            lblError.Visible = true;
            return;
        }

        // Check preview extension
        if ((previewFileExt.Trim() != "") && !MediaLibraryHelper.IsExtensionAllowed(previewFileExt))
        {
            lblError.Text = String.Format(GetString("media.newfile.extensionnotallowed"), HTMLHelper.HTMLEncode(previewFileExt));
            lblError.Visible = true;
            return;
        }

        if (mli != null)
        {
            try
            {
                // Create new Media file
                MediaFileInfo mfi = new MediaFileInfo(fileUploader.PostedFile.ToUploadedFile(), LibraryID, DestinationPath);

                // Save record to the database
                MediaFileInfo.Provider.Set(mfi);

                // Save preview if presented
                if (previewUploader.HasFile)
                {
                    // Get preview suffix if not set
                    if (String.IsNullOrEmpty(PreviewSuffix))
                    {
                        PreviewSuffix = MediaLibraryHelper.GetMediaFilePreviewSuffix(SiteContext.CurrentSiteName);
                    }

                    if (!String.IsNullOrEmpty(PreviewSuffix))
                    {
                        // Get physical path within the media library
                        String path;
                        if ((DestinationPath != null) && DestinationPath.TrimEnd('/') != "")
                        {
                            path = DirectoryHelper.CombinePath(Path.EnsureSlashes(DestinationPath, true), MediaLibraryHelper.GetMediaFileHiddenFolder(SiteContext.CurrentSiteName));
                        }
                        else
                        {
                            path = MediaLibraryHelper.GetMediaFileHiddenFolder(SiteContext.CurrentSiteName);
                        }

                        string previewExtension = Path.GetExtension(previewUploader.PostedFile.FileName);
                        string previewName = Path.GetFileNameWithoutExtension(MediaLibraryHelper.GetPreviewFileName(mfi.FileName, mfi.FileExtension, previewExtension, SiteContext.CurrentSiteName, PreviewSuffix));

                        // Save preview file
                        MediaFileInfoProvider.SaveFileToDisk(SiteContext.CurrentSiteName, mli.LibraryFolder, path, previewName, previewExtension, mfi.FileGUID, previewUploader.PostedFile.InputStream, false);
                    }
                }
                
                // Clear cache
                if (PortalContext.CurrentPageManager != null)
                {
                    PortalContext.CurrentPageManager.ClearCache();
                }

                // Display info to the user
                lblInfo.Text = GetString("media.fileuploaded");
                lblInfo.Visible = true;

                if (OnAfterFileUpload != null)
                {
                    OnAfterFileUpload();
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
                lblError.ToolTip = ex.StackTrace;
            }
        }
    }
}