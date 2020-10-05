using System;
using System.Web;
using System.Web.UI;

using CMS.Base;
using CMS.Base.UploadExtensions;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_Dialogs_DirectFileUploader_DirectMediaFileUploaderControl : DirectFileUploader
{
    #region "Variables"

    private MediaLibraryInfo mLibraryInfo;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Library info uploaded file is related to.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if (LibraryID > 0)
            {
                mLibraryInfo = MediaLibraryInfo.Provider.Get(LibraryID);
            }
            return mLibraryInfo;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Uploaded file.
    /// </summary>
    public override HttpPostedFile PostedFile
    {
        get
        {
            return ucFileUpload.PostedFile;
        }
    }


    /// <summary>
    /// File upload user control.
    /// </summary>
    public override CMSFileUpload FileUploadControl
    {
        get
        {
            return ucFileUpload;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            Page.Error += Page_Error;

            // Initialize uploader
            FileUploadControl.Attributes.Add("class", "fileUpload");
            FileUploadControl.Attributes.Add("style", "cursor: pointer;");
            FileUploadControl.Attributes.Add("onchange", String.Format("if (typeof(parent.DFU) !== 'undefined') {{ parent.DFU.OnUploadBegin('{0}'); {1}; }}", GetContainerID(), Page.ClientScript.GetPostBackEventReference(btnHidden, string.Empty, false)));

            // DFU init script
            string dfuScript = String.Format("if (typeof(parent.DFU) !== 'undefined'){{parent.DFU.init(document.getElementById('{0}')); window.resize = parent.DFU.init(document.getElementById('{0}'));}}", ucFileUpload.ClientID);
            ScriptHelper.RegisterStartupScript(this, typeof(string), "DFUScript_" + ucFileUpload.ClientID, ScriptHelper.GetScript(dfuScript));
            ScriptHelper.RegisterStartupScript(this, typeof(string), "DFU_Display_" + ucFileUpload.ClientID, "document.getElementById('uploaderDiv').style.display = 'block';", true);

            btnHidden.Attributes.Add("style", "display:none;");
        }
    }


    protected void btnHidden_Click(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        try
        {
            switch (SourceType)
            {
                case MediaSourceEnum.MediaLibraries:
                    HandleLibrariesUpload();
                    break;
            }
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("DIRECTFILEUPLOADER", "UPLOADFILE", ex);
            OnError(e);
        }
    }

    #endregion


    #region "Media libraries"

    /// <summary>
    /// Provides operations necessary to create and store new library file.
    /// </summary>
    private void HandleLibrariesUpload()
    {
        // Get related library info        
        if (LibraryInfo != null)
        {
            MediaFileInfo mediaFile = null;

            // Get the site name
            SiteInfo si = SiteInfo.Provider.Get(LibraryInfo.LibrarySiteID);
            string siteName = (si != null) ? si.SiteName : SiteContext.CurrentSiteName;

            string message = string.Empty;
            try
            {
                // Check the allowed extensions
                CheckAllowedExtensions();

                if (MediaFileID > 0)
                {
                    #region "Check permissions"

                    if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "FileModify"))
                    {
                        throw new Exception(GetString("media.security.nofilemodify"));
                    }

                    #endregion


                    mediaFile = MediaFileInfo.Provider.Get(MediaFileID);
                    if (mediaFile != null)
                    {
                        // Ensure object version
                        SynchronizationHelper.EnsureObjectVersion(mediaFile);

                        if (IsMediaThumbnail)
                        {
                            string newFileExt = Path.GetExtension(ucFileUpload.FileName).TrimStart('.');
                            if ((ImageHelper.IsImage(newFileExt)) && (newFileExt.ToLowerCSafe() != "ico") &&
                                (newFileExt.ToLowerCSafe() != "wmf"))
                            {
                                // Update or creation of Media File update
                                string previewSuffix = MediaLibraryHelper.GetMediaFilePreviewSuffix(siteName);

                                if (!String.IsNullOrEmpty(previewSuffix))
                                {
                                    string previewExtension = Path.GetExtension(ucFileUpload.PostedFile.FileName);
                                    string previewName = Path.GetFileNameWithoutExtension(MediaLibraryHelper.GetPreviewFileName(mediaFile.FileName, mediaFile.FileExtension, previewExtension, siteName, previewSuffix));
                                    string previewFolder = DirectoryHelper.CombinePath(Path.EnsureForwardSlashes(LibraryFolderPath.TrimEnd('/')), MediaLibraryHelper.GetMediaFileHiddenFolder(siteName));

                                    byte[] previewFileBinary = new byte[ucFileUpload.PostedFile.ContentLength];
                                    ucFileUpload.PostedFile.InputStream.Read(previewFileBinary, 0, ucFileUpload.PostedFile.ContentLength);

                                    // Delete current preview thumbnails
                                    MediaFileInfoProvider.DeleteMediaFilePreview(siteName, mediaFile.FileLibraryID, mediaFile.FilePath);

                                    // Save preview file
                                    MediaFileInfoProvider.SaveFileToDisk(siteName, LibraryInfo.LibraryFolder, previewFolder, previewName, previewExtension, mediaFile.FileGUID, previewFileBinary, false, false);

                                    // Log synchronization task
                                    SynchronizationHelper.LogObjectChange(mediaFile, TaskTypeEnum.UpdateObject);
                                }

                                // Drop the cache dependencies
                                CacheHelper.TouchKeys(MediaFileInfoProvider.GetDependencyCacheKeys(mediaFile, true));
                            }
                            else
                            {
                                message = GetString("media.file.onlyimgthumb");
                            }
                        }
                        else
                        {
                            // Get folder path
                            string path = Path.GetDirectoryName(DirectoryHelper.CombinePath(MediaLibraryInfoProvider.GetMediaLibraryFolderPath(LibraryInfo.LibraryID), mediaFile.FilePath));

                            // If file system permissions are sufficient for file update
                            if (DirectoryHelper.CheckPermissions(path, false, true, true, true))
                            {
                                // Delete existing media file
                                MediaFileInfoProvider.DeleteMediaFile(LibraryInfo.LibrarySiteID, LibraryInfo.LibraryID, mediaFile.FilePath, true);

                                // Update media file preview
                                if (MediaLibraryHelper.HasPreview(siteName, LibraryInfo.LibraryID, mediaFile.FilePath))
                                {
                                    // Get new unique file name
                                    string newName = URLHelper.GetSafeFileName(ucFileUpload.PostedFile.FileName, siteName);

                                    // Get new file path
                                    string newPath = DirectoryHelper.CombinePath(path, newName);
                                    newPath = MediaLibraryHelper.EnsureUniqueFileName(newPath);
                                    newName = Path.GetFileName(newPath);

                                    // Rename preview
                                    MediaLibraryHelper.MoveMediaFilePreview(mediaFile, newName);

                                    // Delete preview thumbnails
                                    MediaFileInfoProvider.DeleteMediaFilePreviewThumbnails(mediaFile);
                                }

                                // Receive media info on newly posted file
                                mediaFile = GetUpdatedFile(mediaFile);

                                // Save media file information
                                MediaFileInfo.Provider.Set(mediaFile);
                            }
                            else
                            {
                                // Set error message
                                message = String.Format(GetString("media.accessdeniedtopath"), path);
                            }
                        }
                    }
                }
                else
                {
                    #region "Check permissions"

                    // Creation of new media file
                    if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "FileCreate"))
                    {
                        throw new Exception(GetString("media.security.nofilecreate"));
                    }

                    #endregion


                    // No file for upload specified
                    if (!ucFileUpload.HasFile)
                    {
                        throw new Exception(GetString("media.newfile.errorempty"));
                    }

                    // Create new media file record
                    mediaFile = new MediaFileInfo(ucFileUpload.PostedFile.ToUploadedFile(), LibraryID, LibraryFolderPath, ResizeToWidth, ResizeToHeight, ResizeToMaxSideSize, LibraryInfo.LibrarySiteID);

                    mediaFile.FileDescription = "";

                    // Save the new file info
                    MediaFileInfo.Provider.Set(mediaFile);
                }
            }
            catch (Exception ex)
            {
                // Creation of new media file failed
                message = ex.Message;
            }
            finally
            {
                // Create media file info string
                string mediaInfo = "";
                if ((mediaFile != null) && (mediaFile.FileID > 0) && (IncludeNewItemInfo))
                {
                    mediaInfo = mediaFile.FileID + "|" + LibraryFolderPath.Replace('\\', '>').Replace("'", "\\'");
                }

                // Ensure message text
                message = TextHelper.EnsureLineEndings(message, " ");

                if (RaiseOnClick)
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(Page), "UploaderOnClick", ScriptHelper.GetScript("if (parent.UploaderOnClick) { parent.UploaderOnClick('" + MediaFileName.Replace(" ", "").Replace(".", "").Replace("-", "") + "'); }"));
                }

                string script = String.Format(@"
if (typeof(parent.DFU) !== 'undefined') {{ 
    parent.DFU.OnUploadCompleted('{0}'); 
}} 
if ((window.parent != null) && (/parentelemid={1}/i.test(window.location.href)) && (window.parent.InitRefresh_{1} != null)){{
    window.parent.InitRefresh_{1}({2}, false, '{3}'{4});
}}", GetContainerID(), GetParentClientID(), ScriptHelper.GetString(message.Trim()), mediaInfo, (InsertMode ? ", 'insert'" : ", 'update'"));

                // Call function to refresh parent window                                                     
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "RefreshParrent", ScriptHelper.GetScript(script));
            }
        }
    }


    /// <summary>
    /// Gets media file info object representing the updated version of original file.
    /// </summary>
    /// <param name="originalFile">Original file data</param>
    private MediaFileInfo GetUpdatedFile(MediaFileInfo originalFile)
    {
        // Get info on media file from uploaded file
        MediaFileInfo mediaFile = new MediaFileInfo(ucFileUpload.PostedFile.ToUploadedFile(), LibraryID, LibraryFolderPath, ResizeToWidth, ResizeToHeight, ResizeToMaxSideSize, LibraryInfo.LibrarySiteID);

        // Create new file based on original
        MediaFileInfo updatedMediaFile = new MediaFileInfo(originalFile, false)
                                             {
                                                 // Update necessary information
                                                 FileName = mediaFile.FileName,
                                                 FileExtension = mediaFile.FileExtension,
                                                 FileSize = mediaFile.FileSize,
                                                 FileMimeType = mediaFile.FileMimeType,
                                                 FilePath = mediaFile.FilePath,
                                                 FileModifiedByUserID = MembershipContext.AuthenticatedUser.UserID,
                                                 FileBinary = mediaFile.FileBinary,
                                                 FileImageHeight = mediaFile.FileImageHeight,
                                                 FileImageWidth = mediaFile.FileImageWidth,
                                                 FileBinaryStream = mediaFile.FileBinaryStream
                                             };

        return updatedMediaFile;
    }

    #endregion


    #region "Private methods"

    private static string GetContainerID()
    {
        return  QueryHelper.GetControlClientId("containerid", string.Empty);
    }


    private string GetParentClientID()
    {
        return ValidationHelper.GetControlClientId(ParentElemID);
    }

    #endregion
}