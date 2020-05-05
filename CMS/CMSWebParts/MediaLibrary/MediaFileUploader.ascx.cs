using System;

using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_MediaLibrary_MediaFileUploader : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets name of the media library where the file should be uploaded.
    /// </summary>
    public string LibraryName
    {
        get
        {
            string libraryName = ValidationHelper.GetString(GetValue("LibraryName"), String.Empty);
            if ((string.IsNullOrEmpty(libraryName) || libraryName == MediaLibraryInfoProvider.CURRENT_LIBRARY) && (MediaLibraryContext.CurrentMediaLibrary != null))
            {
                return MediaLibraryContext.CurrentMediaLibrary.LibraryName;
            }
            return libraryName;
        }
        set
        {
            SetValue("LibraryName", value);
        }
    }


    /// <summary>
    /// Gets or sets the destination path within the media library.
    /// </summary>
    public string DestinationPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DestinationPath"), "");
        }
        set
        {
            SetValue("DestinationPath", value);
        }
    }


    /// <summary>
    /// Idicates if preview upload dialog shoul displayed.
    /// </summary>
    public bool EnableUploadPreview
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableUploadPreview"), false);
        }
        set
        {
            SetValue("EnableUploadPreview", value);
        }
    }


    /// <summary>
    /// Preview suffix for identification of preview file.
    /// </summary>
    public string PreviewSuffix
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviewSuffix"), "");
        }
        set
        {
            SetValue("PreviewSuffix", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            uploader.StopProcessing = true;
        }
        else
        {
            MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryName, SiteContext.CurrentSiteName);
            if (mli != null)
            {
                uploader.LibraryID = mli.LibraryID;
                uploader.DestinationPath = DestinationPath;
                uploader.EnableUploadPreview = EnableUploadPreview;
                uploader.PreviewSuffix = PreviewSuffix;
                uploader.OnNotAllowed += uploader_OnNotAllowed;
            }
        }
    }


    private void uploader_OnNotAllowed(string permissionType, CMSAdminControl sender)
    {
        if (sender != null)
        {
            sender.StopProcessing = true;
        }
        uploader.StopProcessing = true;
        uploader.Visible = false;
        messageElem.ErrorMessage = MediaLibraryHelper.GetAccessDeniedMessage("filecreate");
        messageElem.DisplayMessage = true;
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }
}