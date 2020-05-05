using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.SiteProvider;

public partial class CMSWebParts_MediaLibrary_MediaGalleryFolderTree : CMSAbstractWebPart
{
    #region "Library properties"

    /// <summary>
    /// Gets or sets media library path to display files from.
    /// </summary>
    public string MediaLibraryPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MediaLibraryPath"), String.Empty);
        }
        set
        {
            SetValue("MediaLibraryPath", value);
        }
    }


    /// <summary>
    /// Gets or sets media library name.
    /// </summary>
    public string MediaLibraryName
    {
        get
        {
            string libraryName = ValidationHelper.GetString(GetValue("MediaLibraryName"), "");
            if ((string.IsNullOrEmpty(libraryName) || libraryName == MediaLibraryInfoProvider.CURRENT_LIBRARY) && (MediaLibraryContext.CurrentMediaLibrary != null))
            {
                return MediaLibraryContext.CurrentMediaLibrary.LibraryName;
            }
            return libraryName;
        }
        set
        {
            SetValue("MediaLibraryName", value);
        }
    }


    /// <summary>
    /// Indicates if files count should be displayed.
    /// </summary>
    public bool DisplayFileCount
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFileCount"), folderTree.DisplayFileCount);
        }
        set
        {
            SetValue("DisplayFileCount", value);
            folderTree.DisplayFileCount = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), "");
        }
        set
        {
            SetValue("FilterName", value);
            folderTree.SourceFilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the file id querysting parameter.
    /// </summary>
    public string FileIDQueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FileIDQueryStringKey"), String.Empty);
        }
        set
        {
            SetValue("FileIDQueryStringKey", value);
            folderTree.FileIDQueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the path querysting parameter.
    /// </summary>
    public string PathQueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PathQueryStringKey"), String.Empty);
        }
        set
        {
            SetValue("PathQueryStringKey", value);
            folderTree.PathQueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the filter method.
    /// </summary>
    public int FilterMethod
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("FilterMethod"), 0);
        }
        set
        {
            SetValue("FilterMethod", value);
            folderTree.FilterMethod = value;
        }
    }

    #endregion


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
        MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(MediaLibraryName, SiteContext.CurrentSiteName);

        if (mli != null)
        {
            // If dont have 'Manage' permission
            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "read"))
            {
                // Check 'File create' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "libraryaccess"))
                {
                    folderTree.StopProcessing = true;
                    folderTree.Visible = false;
                    messageElem.ErrorMessage = MediaLibraryHelper.GetAccessDeniedMessage("libraryaccess");
                    messageElem.DisplayMessage = true;
                    return;
                }
            }

            // Tree
            if (string.IsNullOrEmpty(MediaLibraryPath))
            {
                folderTree.RootFolderPath = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName);
                folderTree.MediaLibraryFolder = mli.LibraryFolder;
            }
            else
            {
                folderTree.RootFolderPath = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName) + mli.LibraryFolder;
                int index = MediaLibraryPath.LastIndexOfCSafe('/');
                if ((index > -1) && (MediaLibraryPath.Length > (index + 1)))
                {
                    folderTree.MediaLibraryFolder = MediaLibraryPath.Substring(index + 1);
                }
                else
                {
                    folderTree.MediaLibraryFolder = MediaLibraryPath;
                }
                folderTree.MediaLibraryPath = Path.EnsureBackslashes(MediaLibraryPath);
            }

            // Set images path
            if (CultureHelper.IsPreferredCultureRTL())
            {
                folderTree.ImageFolderPath = GetImageUrl("RTL/Design/Controls/Tree", true, true);
            }
            else
            {
                folderTree.ImageFolderPath = GetImageUrl("Design/Controls/Tree", true, true);
            }

            folderTree.SourceFilterName = FilterName;
            folderTree.FileIDQueryStringKey = FileIDQueryStringKey;
            folderTree.PathQueryStringKey = PathQueryStringKey;
            folderTree.DisplayFileCount = DisplayFileCount;

            // Add tree to the filter collection
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), folderTree);
        }
        else
        {
            folderTree.StopProcessing = true;
            folderTree.Visible = false;
        }
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