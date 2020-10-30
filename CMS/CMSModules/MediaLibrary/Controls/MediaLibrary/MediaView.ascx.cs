using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.Base;

using System.Text;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Core;
using CMS.DocumentEngine;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaView : MediaView
{
    #region "Events & delegates"

    /// <summary>
    /// Delegate for the event occurring when information on file import status is required.
    /// </summary>
    /// <param name="type">Type of the required information</param>
    /// <param name="parameter">Parameter related</param>
    public delegate object OnGetInformation(string type, object parameter);


    /// <summary>
    /// Event occurring when information on file import status is required.
    /// </summary>
    public event OnGetInformation GetInformation;

    #endregion


    #region "Private variables"

    // Media library variables
    private MediaLibraryInfo mLibraryInfo;
    private SiteInfo mLibrarySite;

    private bool mIsCopyMoveLinkDialog;
    private bool? mHasModify;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets a view mode used to display files.
    /// </summary>
    public override DialogViewModeEnum ViewMode
    {
        get
        {
            return base.ViewMode;
        }
        set
        {
            base.ViewMode = value;
            innermedia.ViewMode = value;
        }
    }


    /// <summary>
    /// Gets or sets an ID of the media library.
    /// </summary>
    public int LibraryID
    {
        get;
        set;
    }


    /// <summary>
    /// Current media library information.
    /// </summary>
    public MediaLibraryInfo LibraryInfo
    {
        get
        {
            if (mLibraryInfo == null)
            {
                mLibraryInfo = MediaLibraryInfo.Provider.Get(LibraryID);
            }
            return mLibraryInfo;
        }
        set
        {
            mLibraryInfo = value;
        }
    }


    /// <summary>
    /// Indicates whether control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            innermedia.StopProcessing = value;
            base.StopProcessing = value;
        }
    }


    /// <summary>
    /// Gets currently selected page size.
    /// </summary>
    public int CurrentPageSize
    {
        get
        {
            return innermedia.CurrentPageSize;
        }
    }


    /// <summary>
    /// Gets currently selected page size.
    /// </summary>
    public int CurrentOffset
    {
        get
        {
            return innermedia.CurrentOffset;
        }
    }


    /// <summary>
    /// Gets or sets the OutputFormat (needed for correct dialog type recognition).
    /// </summary>
    public OutputFormatEnum OutputFormat
    {
        get
        {
            return innermedia.OutputFormat;
        }
        set
        {
            innermedia.OutputFormat = value;
        }
    }


    /// <summary>
    /// Gets a UniGrid control used to display files in LIST view mode.
    /// </summary>
    public UniGrid ListViewControl
    {
        get
        {
            return innermedia.ListViewControl;
        }
    }


    /// <summary>
    /// Indicates if full listing mode is enabled. This mode enables navigation to child and parent folders/documents from current view.
    /// </summary>
    public bool IsFullListingMode
    {
        get
        {
            return innermedia.IsFullListingMode;
        }
        set
        {
            innermedia.IsFullListingMode = value;
        }
    }


    /// <summary>
    /// Indicates whether the control is displayed as part of the copy/move dialog.
    /// </summary>
    public bool IsCopyMoveLinkDialog
    {
        get
        {
            return mIsCopyMoveLinkDialog;
        }
        set
        {
            mIsCopyMoveLinkDialog = value;
            innermedia.IsCopyMoveLinkDialog = value;
        }
    }


    /// <summary>
    /// Gets list of names of selected files.
    /// </summary>
    public List<string> SelectedItems
    {
        get
        {
            return innermedia.SelectedItems;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether only imported media files are displayed.
    /// </summary>
    public bool DisplayOnlyImportedFiles
    {
        get;
        set;
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets site info current library is related to.
    /// </summary>
    private SiteInfo LibrarySite
    {
        get
        {
            if ((mLibrarySite == null) && (LibraryInfo != null))
            {
                mLibrarySite = SiteInfo.Provider.Get(LibraryInfo.LibrarySiteID);
            }
            return mLibrarySite;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = !StopProcessing;

        // If processing the request should not continue
        if (!StopProcessing)
        {
            // Initialize controls
            SetupControls();
        }

        innermedia.ViewMode = ViewMode;
        innermedia.IsLiveSite = IsLiveSite;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        InitializeControlScripts();

        // Initialize inner view control
        innermedia.ViewMode = ViewMode;
        innermedia.DataSource = DataSource;
        innermedia.TotalRecords = TotalRecords;
        innermedia.SelectableContent = SelectableContent;

        const string MEDIA_DIALOGS_FOLDER = "~/CMSModules/MediaLibrary/Controls/Dialogs/";

        string gridName = MEDIA_DIALOGS_FOLDER + "MediaListView.xml";
        if (!IsCopyMoveLinkDialog && (DisplayMode == ControlDisplayModeEnum.Simple))
        {
            innermedia.DisplayMode = DisplayMode;
            gridName = MEDIA_DIALOGS_FOLDER + "MediaListView_UI.xml";
        }
        else if (IsCopyMoveLinkDialog && (DisplayMode == ControlDisplayModeEnum.Simple))
        {
            innermedia.DisplayMode = DisplayMode;
            gridName = MEDIA_DIALOGS_FOLDER + "MediaListView_CopyMove.xml";
        }

        innermedia.ListViewControl.GridName = gridName;
        innermedia.ListViewControl.OnPageChanged += ListViewControl_OnPageChanged;

        innermedia.SourceType = SourceType;

        innermedia.ResizeToHeight = ResizeToHeight;
        innermedia.ResizeToMaxSideSize = ResizeToMaxSideSize;
        innermedia.ResizeToWidth = ResizeToWidth;

        // Set inner control binding columns
        innermedia.FileIdColumn = "FileGUID";
        innermedia.FileNameColumn = "FileName";
        innermedia.FileExtensionColumn = ((DisplayMode == ControlDisplayModeEnum.Simple) && !DisplayOnlyImportedFiles) ? "Extension" : "FileExtension";
        innermedia.FileSizeColumn = "FileSize";
        innermedia.FileWidthColumn = "FileImageWidth";
        innermedia.FileHeightColumn = "FileImageHeight";

        // Register for inner media events
        innermedia.GetArgumentSet += innermedia_GetArgumentSet;
        innermedia.GetListItemUrl += innermedia_GetListItemUrl;
        innermedia.GetThumbsItemUrl += innermedia_GetThumbsItemUrl;
        innermedia.GetInformation += innermedia_GetInformation;
        innermedia.GetModifyPermission += innermedia_GetModifyPermission;
    }


    private bool innermedia_GetModifyPermission(IDataContainer data)
    {
        if (mHasModify == null)
        {
            mHasModify = (LibraryInfo != null) && MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify");
        }

        return mHasModify.Value;
    }


    private object innermedia_GetInformation(string type, object parameter)
    {
        return GetInformationInternal(type, parameter);
    }


    private object GetInformationInternal(string type, object parameter)
    {
        if (GetInformation != null)
        {
            return GetInformation(type, parameter);
        }

        return null;
    }


    private void ListViewControl_OnPageChanged(object sender, EventArgs e)
    {
        RaiseListReloadRequired();
    }


    /// <summary>
    /// Returns URL of the media item according site settings.
    /// </summary>
    /// <param name="argument">Argument containing information on current media item</param>
    /// <param name="data">Data row object holding all the data on the current file</param>
    /// <param name="isPreview">Indicates whether the file has a preview file or not</param>
    /// <param name="height">Specifies height of the image</param>
    /// <param name="width">Specifies width of the image</param>
    /// <param name="maxSideSize">Specifies maximum size of the image</param>
    private string GetItemUrlInternal(string argument, IDataContainer data, bool isPreview, int height, int width, int maxSideSize)
    {
        MediaFileInfo mfi = null;

        // Get filename with extension
        string fileName;
        if (data.ContainsColumn("FileExtension"))
        {
            string ext = ValidationHelper.GetString(data.GetValue("FileExtension"), "");
            // In format 'name'
            fileName = ValidationHelper.GetString(data.GetValue("FileName"), "");

            fileName = AttachmentHelper.GetFullFileName(fileName, ext);
        }
        else
        {
            // In format 'name.ext'
            fileName = data.GetValue("FileName").ToString();
        }

        // Try to get imported data row
        DataRow importedRow = GetInformationInternal("fileisnotindatabase", fileName) as DataRow;
        if (importedRow != null)
        {
            mfi = new MediaFileInfo(importedRow);
        }

        // If data row is not from DB check external library
        if (!data.ContainsColumn("FileGUID"))
        {
            bool isExternal = false;
            // Check if is external media library
            if (data.ContainsColumn("FilePath"))
            {
                // Get file path
                string filePath = ValidationHelper.GetString(data.GetValue("FilePath"), String.Empty);
                if (!String.IsNullOrEmpty(filePath))
                {
                    // Test if file path is inside system root folder
                    string rootPath = Server.MapPath("~/");
                    if (!filePath.StartsWithCSafe(rootPath))
                    {
                        isExternal = true;
                    }
                }
            }

            if (isExternal && data.ContainsColumn("FileName"))
            {
                return (mfi != null ? GetItemUrl(mfi, isPreview, height, width, maxSideSize) : String.Empty);
            }
        }

        // Files are obtained from the FS
        if (data.ContainsColumn("FileURL"))
        {
            // Information comming from FileSystem (FS)
            return URLHelper.ResolveUrl(data.GetValue("FileURL").ToString());
        }
        else
        {
            return (mfi != null ? GetItemUrl(mfi, isPreview, height, width, maxSideSize) : GetItemUrl(argument, isPreview, height, width, maxSideSize));
        }
    }


    /// <summary>
    /// Initializes scripts used by the control.
    /// </summary>
    private void InitializeControlScripts()
    {
        ScriptHelper.RegisterStartupScript(this, GetType(), "DialogsSelectAction", ScriptHelper.GetScript(@"
function SetSelectAction(argument) {
    // Raise select action
    SetAction('select', argument);
    RaiseHiddenPostBack();
}
function SetParentAction(argument) {
    // Raise select action
    SetAction('parentselect', argument);
    RaiseHiddenPostBack();
}"));
    }


    /// <summary>
    /// Loads data from data source property.
    /// </summary>
    /// <param name="forceSetup">Indicates whether the inner controls should be re-set</param>
    private void ReloadData(bool forceSetup)
    {
        innermedia.Reload(forceSetup);
    }


    private bool IsLibrarySiteDifferentThanCurrentSite()
    {
        return (SiteContext.CurrentSiteID != LibraryInfo.LibrarySiteID) || (SiteContext.CurrentSiteID != GetCurrentSiteId());
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Loads control's content.
    /// </summary>
    /// <param name="forceSetup">Indicates whether the inner controls should be re-setuped</param>
    public void Reload(bool forceSetup)
    {
        Visible = !StopProcessing;
        if (Visible)
        {
            // Initialize controls
            SetupControls();

            ReloadData(forceSetup);
        }
    }


    /// <summary>
    /// Loads control's content.
    /// </summary>    
    public void Reload()
    {
        Reload(false);
    }


    /// <summary>
    /// Ensures no item is selected.
    /// </summary>
    public void ResetListSelection()
    {
        innermedia.ResetListSelection();
    }


    /// <summary>
    /// Ensures first page is displayed in the control displaying the content.
    /// </summary>
    public void ResetPageIndex()
    {
        innermedia.ResetPageIndex();
    }


    /// <summary>
    /// Displays listing info message.
    /// </summary>
    /// <param name="infoMsg">Info message to display</param>
    public void DisplayListingInfo(string infoMsg)
    {
        if (!string.IsNullOrEmpty(infoMsg))
        {
            plcListingInfo.Visible = true;
            lblListingInfo.Text = infoMsg;
        }
    }


    /// <summary>
    /// Returns URL of the media item according site settings.
    /// </summary>
    /// <param name="fileInfo">Media file information</param>
    /// <param name="isPreview">Indicates whether the file has a preview file or not</param>
    /// <param name="height">Height of the requested image</param>
    /// <param name="width">Width of the requested image</param>
    /// <param name="maxSideSize">Maximum dimension for images displayed for thumbnails view</param>
    public string GetItemUrl(MediaFileInfo fileInfo, bool isPreview, int height, int width, int maxSideSize)
    {
        if (fileInfo != null)
        {
            return GetItemUrl(fileInfo, LibrarySite, fileInfo.FileGUID, fileInfo.FileName, fileInfo.FileExtension, fileInfo.FilePath, isPreview, height, width, maxSideSize);
        }

        return "";
    }


    /// <summary>
    /// Returns URL of the media item according site settings.
    /// </summary>
    /// <param name="argument">Argument containing information on current media item</param>
    /// <param name="isPreview">Indicates whether the file has a preview file or not</param>
    /// <param name="height">Height of the requested image</param>
    /// <param name="width">Width of the requested image</param>
    /// <param name="maxSideSize">Maximum dimension for images displayed for thumbnails view</param>
    public string GetItemUrl(string argument, bool isPreview, int height, int width, int maxSideSize)
    {
        Hashtable argTable = GetArgumentsTable(argument);
        if (argTable.Count >= 2)
        {
            // Get information from argument
            Guid fileGuid = ValidationHelper.GetGuid(argTable["fileguid"], Guid.Empty);
            string fileName = argTable["filename"].ToString();
            string fileExtension = argTable["fileextension"].ToString();
            string filePath = argTable["filepath"].ToString();

            return GetItemUrl(null, LibrarySite, fileGuid, fileName, fileExtension, filePath, isPreview, height, width, maxSideSize);
        }

        return "";
    }


    /// <summary>
    /// Returns URL of the media item according site settings.
    /// </summary>
    /// <param name="site">Information on site file belongs to</param>
    /// <param name="fileGuid">GUID of the file URL is generated for</param>
    /// <param name="fileName">Name of the file URL is generated for</param>
    /// <param name="fileExtension">Extension of the file URL is generated for</param>
    /// <param name="filePath">Media file path of the file URL is generated for</param>
    /// <param name="isPreview">Indicates whether the file URL is generated for preview</param>
    /// <param name="height">Specifies height of the image</param>
    /// <param name="width">Specifies width of the image</param>
    /// <param name="maxSideSize">Maximum dimension for images displayed for thumbnails view</param>
    public string GetItemUrl(SiteInfo site, Guid fileGuid, string fileName, string fileExtension, string filePath, bool isPreview, int height, int width, int maxSideSize)
    {
        return GetItemUrl(null, site, fileGuid, fileName, fileExtension, filePath, isPreview, height, width, maxSideSize);
    }


    /// <summary>
    /// Returns URL of the media item according site settings.
    /// </summary>
    /// <param name="fileInfo">File info object</param>
    /// <param name="site">Information on site file belongs to</param>
    /// <param name="fileGuid">GUID of the file URL is generated for</param>
    /// <param name="fileName">Name of the file URL is generated for</param>
    /// <param name="fileExtension">Extension of the file URL is generated for</param>
    /// <param name="filePath">Media file path of the file URL is generated for</param>
    /// <param name="isPreview">Indicates whether the file URL is generated for preview</param>
    /// <param name="height">Specifies height of the image</param>
    /// <param name="width">Specifies width of the image</param>
    /// <param name="maxSideSize">Maximum dimension for images displayed for thumbnails view</param>
    public string GetItemUrl(MediaFileInfo fileInfo, SiteInfo site, Guid fileGuid, string fileName, string fileExtension, string filePath, bool isPreview, int height, int width, int maxSideSize)
    {
        string mediaFileUrl;
        bool resize = (maxSideSize > 0);

        fileName = AttachmentHelper.GetFullFileName(fileName, fileExtension);

        bool generateAbsoluteURL = Config?.UseFullURL ?? false;
        generateAbsoluteURL = ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSUseMediaFileAbsoluteURLs"], generateAbsoluteURL);
        bool isDifferentSite = false;

        if (generateAbsoluteURL || (isDifferentSite = IsLibrarySiteDifferentThanCurrentSite()))
        {
            if (isDifferentSite)
            {
                mediaFileUrl = UsePermanentUrls ? 
                    MediaFileURLProvider.GetMediaFileAbsoluteUrl(site.SiteName, fileGuid, fileName) : 
                    MediaFileURLProvider.GetMediaFileAbsoluteUrl(site.SiteName, LibraryInfo.LibraryFolder, filePath);
            }
            else if (UsePermanentUrls || resize || isPreview)  // If permanent URLs should be generated
            {
                // URL in format 'http://domainame/getmedia/123456-25245-45454-45455-5455555545/testfile.gif'
                mediaFileUrl = MediaFileURLProvider.GetMediaFileAbsoluteUrl(site.SiteName, fileGuid, fileName);
            }
            else
            {
                // URL in format 'http://domainame/cms/SampleSite/media/testlibrary/folder1/testfile.gif'
                mediaFileUrl = MediaFileURLProvider.GetMediaFileAbsoluteUrl(site.SiteName, LibraryInfo.LibraryFolder, filePath);
            }
        }
        else
        {
            if (UsePermanentUrls || resize || isPreview)
            {
                // URL in format '/cms/getmedia/123456-25245-45454-45455-5455555545/testfile.gif'
                mediaFileUrl = MediaFileURLProvider.GetMediaFileUrl(fileGuid, fileName);
            }
            else
            {
                if (fileInfo != null)
                {
                    mediaFileUrl = MediaFileURLProvider.GetMediaFileUrl(fileInfo, SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder);
                }
                else
                {
                    // URL in format '/cms/SampleSite/media/testlibrary/folder1/testfile.gif'
                    mediaFileUrl = MediaFileURLProvider.GetMediaFileUrl(SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder, filePath);
                }
            }
        }

        // If image dimensions are specified
        if (resize)
        {
            mediaFileUrl = URLHelper.AddParameterToUrl(mediaFileUrl, "maxsidesize", maxSideSize.ToString());
        }
        if (height > 0)
        {
            mediaFileUrl = URLHelper.AddParameterToUrl(mediaFileUrl, "height", height.ToString());
        }
        if (width > 0)
        {
            mediaFileUrl = URLHelper.AddParameterToUrl(mediaFileUrl, "width", width.ToString());
        }

        // Media selector should returns non-resolved URL in all cases
        bool isMediaSelector = (OutputFormat == OutputFormatEnum.URL) && (SelectableContent == SelectableContentEnum.OnlyMedia);

        return (isMediaSelector || ((Config != null) && Config.ContentUseRelativeUrl)) ? mediaFileUrl : UrlResolver.ResolveUrl(mediaFileUrl);
    }


    /// <summary>
    /// Ensures no item is selected.
    /// </summary>
    public void ResetSearch()
    {
        dialogSearch.ResetSearch();
    }


    /// <summary>
    /// Returns argument set for the passed file data row.
    /// </summary>
    /// <param name="data">Data object holding all the data on the current file</param>
    public static string GetArgumentSet(IDataContainer data)
    {
        StringBuilder sb = new StringBuilder();

        if (data.ContainsColumn("FileGUID"))
        {
            sb.Append("FileName|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileName")));
            sb.Append("|FileGUID|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileGUID")));
            sb.Append("|FilePath|" + CMSDialogHelper.EscapeArgument(data.GetValue("FilePath")));
            sb.Append("|FileExtension|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileExtension")));
            sb.Append("|FileImageWidth|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileImageWidth")));
            sb.Append("|FileImageHeight|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileImageHeight")));
            sb.Append("|FileTitle|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileTitle")));
            sb.Append("|FileSize|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileSize")));
            sb.Append("|FileID|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileID")));
        }
        else
        {
            sb.Append("FileName|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileName")));
            sb.Append("|Extension|" + CMSDialogHelper.EscapeArgument(data.GetValue("Extension")));
            sb.Append("|FileURL|" + CMSDialogHelper.EscapeArgument(data.GetValue("FileURL")));
            sb.Append("|Size|" + CMSDialogHelper.EscapeArgument(data.GetValue("Size")));
        }

        return sb.ToString();
    }


    /// <summary>
    /// Returns arguments table for the passed argument.
    /// </summary>
    /// <param name="argument">Argument containing information on current media item</param>
    public static Hashtable GetArgumentsTable(string argument)
    {
        Hashtable table = new Hashtable();

        string[] argArr = argument.Split('|');
        // Fill table
        for (int i = 0; i < argArr.Length; i = i + 2)
        {
            table[argArr[i].ToLowerCSafe()] = CMSDialogHelper.UnEscapeArgument(argArr[i + 1]);
        }

        return table;
    }

    #endregion


    #region "Inner media view event handlers"

    /// <summary>
    /// Returns argument set according passed DataRow and flag indicating whether the set is obtained for selected item.
    /// </summary>
    /// <param name="data">Data object</param>
    private string innermedia_GetArgumentSet(IDataContainer data)
    {
        // Return required argument set
        return GetArgumentSet(data);
    }


    /// <summary>
    /// Returns URL of item according specified conditions.
    /// </summary>
    /// <param name="data">Data object holding information on item</param>
    /// <param name="isPreview">Indicates whether the URL is requested for item preview</param>
    private string innermedia_GetListItemUrl(IDataContainer data, bool isPreview)
    {
        // Get set of important information
        string arg = GetArgumentSet(data);

        // Get item url
        return GetItemUrlInternal(arg, data, isPreview, 0, 0, 0);
    }


    /// <summary>
    /// Returns URL of item according specified conditions.
    /// </summary>
    /// <param name="data">Data object holding information on item</param>
    /// <param name="isPreview">Indicates whether the URL is requested for item preview</param>
    /// <param name="height">Image height</param>
    /// <param name="width">Image width</param>
    /// <param name="maxSideSize">Specifies maximum size of the image</param>
    /// <param name="extension">File extension</param>
    private IconParameters innermedia_GetThumbsItemUrl(IDataContainer data, bool isPreview, int height, int width, int maxSideSize, string extension)
    {
        var parameters = new IconParameters();

        if (LibraryInfo == null)
        {
            return parameters;
        }

        // get argument set
        string arg = GetArgumentSet(data);

        // If image is requested for preview
        if (!isPreview)
        {
            // Get item URL
            parameters.Url = GetItemUrlInternal(arg, data, false, height, width, maxSideSize);

            return parameters;
        }

        if (extension.ToLowerCSafe() == "<dir>")
        {
            parameters.IconClass = "icon-folder";
        }
        else
        {
            // Check if file has a preview
            if (!ImageHelper.IsSupportedByImageEditor(extension))
            {
                // File isn't image and no preview exists - get the default file icon
                parameters.IconClass = UIHelper.GetFileIconClass(extension);
            }
            else
            {
                // Files are obtained from the FS
                if (!data.ContainsColumn("FileURL"))
                {
                    parameters.Url = GetItemUrl(arg, true, height, width, maxSideSize);
                }
                else
                {
                    parameters.IconClass = UIHelper.GetFileIconClass(extension);
                }
            }
        }

        // Setup icon size for fon icons
        if (!string.IsNullOrEmpty(parameters.IconClass))
        {
            parameters.IconSize = FontIconSizeEnum.Dashboard;
        }
        return parameters;
    }

    #endregion
}
