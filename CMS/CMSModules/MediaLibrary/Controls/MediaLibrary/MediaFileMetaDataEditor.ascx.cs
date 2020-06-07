using System;

using CMS.Base;
using CMS.Core;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileMetaDataEditor : CMSUserControl
{
    #region "Variables"

    private MediaFileInfo mediaFileInfo = null;
    private string mSiteName = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Object type.
    /// </summary>
    public string ObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Object GUID.
    /// </summary>
    public Guid ObjectGuid
    {
        get;
        set;
    }


    /// <summary>
    /// Site name. If is null, return current site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mSiteName ?? (mSiteName = SiteContext.CurrentSiteName);
        }
        set
        {
            mSiteName = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Gets object extension.
    /// </summary>
    /// <param name="extension">Object extension</param>
    public delegate void OnGetObjectExtension(string extension);

    public event OnGetObjectExtension GetObjectExtension;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set properties
        metaDataEditor.ObjectGuid = ObjectGuid;
        metaDataEditor.ObjectType = ObjectType;
        metaDataEditor.SiteName = SiteName;

        // Register events
        metaDataEditor.InitializeObject += metaDataEditor_InitializeObject;
        metaDataEditor.OnSetMetaData += metaDataEditor_SetMetaData;
        metaDataEditor.Save += metaDataEditor_Save;
    }


    /// <summary>
    /// Initializes media file info.
    /// </summary>
    /// <param name="objectGuid">Media file GUID</param>
    /// <param name="siteName">Site name</param>
    private void metaDataEditor_InitializeObject(Guid objectGuid, string siteName)
    {
        // Get mediafile
        mediaFileInfo = MediaFileInfo.Provider.Get(objectGuid, SiteInfoProvider.GetSiteID(siteName));

        // If media file is not null 
        if (mediaFileInfo != null)
        {
            MediaLibraryInfo mli = MediaLibraryInfo.Provider.Get(ValidationHelper.GetInteger(mediaFileInfo.FileLibraryID, 0));

            // Check permission 'FileModify'
            if (metaDataEditor.CheckPermissions && !MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "filemodify"))
            {
                RedirectToAccessDenied(GetString("metadata.errors.filemodify"));
            }

            // Fire event GetObjectExtension
            if (GetObjectExtension != null)
            {
                GetObjectExtension(mediaFileInfo.FileExtension);
            }
        }
        else
        {
            RedirectToInformation(GetString("editedobject.notexists"));
        }
    }


    /// <summary>
    /// Sets metadata.
    /// </summary>
    private void metaDataEditor_SetMetaData(object sender, EventArgs e)
    {
        if (mediaFileInfo != null)
        {
            metaDataEditor.ObjectTitle = mediaFileInfo.FileTitle;
            metaDataEditor.ObjectDescription = mediaFileInfo.FileDescription;
            metaDataEditor.ObjectFileName = mediaFileInfo.FileName;
            metaDataEditor.ObjectExtension = mediaFileInfo.FileExtension;
            metaDataEditor.ObjectSize = DataHelper.GetSizeString(mediaFileInfo.FileSize);
        }
    }


    /// <summary>
    /// Save title and description of media file info.
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    private bool metaDataEditor_Save(string fileName, string title, string description)
    {
        bool saved = false;

        if (mediaFileInfo != null)
        {
            try
            {
                if (mediaFileInfo.FileName != fileName)
                {
                    // Get original file path
                    string extension = mediaFileInfo.FileExtension;

                    // New file path
                    string newPath = DirectoryHelper.CombinePath(Path.GetDirectoryName(mediaFileInfo.FilePath), fileName + extension);
                    string newFullPath = MediaFileInfoProvider.GetMediaFilePath(mediaFileInfo.FileLibraryID, newPath);
                    string newExtension = Path.GetExtension(newPath);

                    if (!String.IsNullOrEmpty(newExtension))
                    {
                        newExtension = newExtension.TrimStart('.');
                    }

                    if (!MediaLibraryHelper.IsExtensionAllowed(newExtension))
                    {
                        // New extension is not allowed
                        metaDataEditor.ShowError(GetString("dialogs.filesystem.NotAllowedExtension").Replace("%%extensions%%", MediaLibraryHelper.GetAllowedExtensions(SiteName).TrimEnd(';').Replace(";", ", ")));
                        return false;
                    }

                    // Rename file
                    if (!File.Exists(newFullPath))
                    {
                        MediaFileInfoProvider.MoveMediaFile(SiteName, mediaFileInfo.FileLibraryID, mediaFileInfo.FilePath, newPath);

                        // Move preview file if exists
                        if (MediaLibraryHelper.HasPreview(SiteName, mediaFileInfo.FileLibraryID, mediaFileInfo.FilePath))
                        {
                            MediaLibraryHelper.MoveMediaFilePreview(mediaFileInfo, fileName + extension);
                        }
                    }
                    else
                    {
                        // File already exists.
                        metaDataEditor.ShowError(GetString("img.errors.fileexists"));
                        return false;
                    }

                    mediaFileInfo.FileName = fileName;

                    string subFolderPath = null;

                    int lastSlash = mediaFileInfo.FilePath.LastIndexOfCSafe('/');
                    if (lastSlash > 0)
                    {
                        subFolderPath = mediaFileInfo.FilePath.Substring(0, lastSlash);
                    }

                    if (!string.IsNullOrEmpty(subFolderPath))
                    {
                        mediaFileInfo.FilePath = String.Format("{0}/{1}{2}", subFolderPath, fileName, extension);
                    }
                    else
                    {
                        mediaFileInfo.FilePath = fileName + extension;
                    }
                }
                mediaFileInfo.FileTitle = title;
                mediaFileInfo.FileDescription = description;

                // Save new data
                mediaFileInfo.EnsureUniqueFileName(false);
                MediaFileInfo.Provider.Set(mediaFileInfo);

                saved = true;
            }
            catch (Exception ex)
            {
                metaDataEditor.ShowError(GetString("metadata.errors.processing"));
                Service.Resolve<IEventLogService>().LogException("Metadata editor", "SAVE", ex);
            }
        }

        return saved;
    }


    /// <summary>
    /// Saves metadata of media file (title, description).
    /// </summary>
    /// <returns>Returns True if media file was successfully saved.</returns>
    public bool SaveMetadata()
    {
        return metaDataEditor.SaveMetadata();
    }

    #endregion
}