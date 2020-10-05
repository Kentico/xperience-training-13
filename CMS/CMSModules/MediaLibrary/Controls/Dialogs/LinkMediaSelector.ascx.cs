using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_Dialogs_LinkMediaSelector : LinkMediaSelector
{
    #region "Constants"

    /// <summary>
    /// Storage key used for link media selector.
    /// </summary>
    private const string LINK_MEDIA_SELECTOR_STORAGE_KEY = "LinkMediaSelector";

    #endregion


    #region "Private variables"

    // Media library variables
    private string mFolderPath = string.Empty;
    private string mMediaLibraryRootFolder;

    private string mCurrentAction;

    private MediaLibraryInfo mLibraryInfo;
    private SiteInfo mLibrarySiteInfo;

    private string mSortDirection = "ASC";
    private string mSortColumns = "FileName";

    private bool wasLoaded;

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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns current properties (according to OutputFormat).
    /// </summary>
    protected override ItemProperties ItemProperties
    {
        get
        {
            switch (Config.OutputFormat)
            {
                case OutputFormatEnum.HTMLMedia:
                    return htmlMediaProp;

                case OutputFormatEnum.HTMLLink:
                    return htmlLinkProp;

                case OutputFormatEnum.BBMedia:
                    return bbMediaProp;

                case OutputFormatEnum.BBLink:
                    return bbLinkProp;

                case OutputFormatEnum.URL:
                    return urlProp;

                default:
                    return null;
            }
        }
    }


    /// <summary>
    /// Update panel where properties control resides.
    /// </summary>
    protected override UpdatePanel PropertiesUpdatePanel
    {
        get
        {
            return pnlUpdateProperties;
        }
    }


    /// <summary>
    /// Gets or sets last searched value.
    /// </summary>
    private string LastSearchedValue
    {
        get
        {
            return hdnLastSearchedValue.Value;
        }
        set
        {
            hdnLastSearchedValue.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets last selected folder path.
    /// </summary>
    private string LastFolderPath
    {
        get
        {
            return hdnLastSelectedPath.Value;
        }
        set
        {
            hdnLastSelectedPath.Value = value;
        }
    }


    /// <summary>
    /// Indicates if full listing mode is enabled. This mode enables navigation to child and parent folders/documents from current view.
    /// </summary>
    private bool IsFullListingMode
    {
        get
        {
            return mediaView.IsFullListingMode;
        }
        set
        {
            mediaView.IsFullListingMode = value;
        }
    }


    /// <summary>
    /// Current action name.
    /// </summary>
    private string CurrentAction
    {
        get
        {
            return mCurrentAction ?? (mCurrentAction = hdnAction.Value.Trim().ToLowerInvariant());
        }
    }


    /// <summary>
    /// Indicates whether the library has changed recently.
    /// </summary>
    private bool LibraryChanged
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets direction in which data should be ordered.
    /// </summary>
    private string SortDirection
    {
        get
        {
            return mSortDirection;
        }
        set
        {
            mSortDirection = value;
        }
    }


    /// <summary>
    /// Gets or sets sort columns data are ordered by.
    /// </summary>
    public string SortColumns
    {
        get
        {
            return mSortColumns;
        }
        set
        {
            mSortColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets list of files from the currently selected folder.
    /// </summary>
    private Hashtable FileList
    {
        get;
        set;
    }

    #endregion


    #region "Library properties"

    /// <summary>
    /// Gets or sets a folder path of the media library.
    /// </summary>
    private string FolderPath
    {
        get
        {
            return mFolderPath;
        }
        set
        {
            mFolderPath = (value ?? string.Empty);
            LastFolderPath = mFolderPath;
        }
    }


    /// <summary>
    /// Gets or sets an ID of the media library.
    /// </summary>
    private int LibraryID
    {
        get
        {
            return librarySelector.LibraryID;
        }
        set
        {
            librarySelector.LibraryID = value;
            folderTree.MediaLibraryID = value;
            menuElem.LibraryID = value;
            menuElem.UpdateViewMenu();
            mLibraryInfo = null;
            mediaView.LibraryInfo = null;
            mLibrarySiteInfo = null;
            mMediaLibraryRootFolder = null;
        }
    }


    /// <summary>
    /// Current media library information.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (LibraryID > 0))
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
    /// Gets info on site library is related to.
    /// </summary>
    private SiteInfo LibrarySiteInfo
    {
        get
        {
            if ((mLibrarySiteInfo == null) && (LibraryInfo != null))
            {
                mLibrarySiteInfo = SiteInfo.Provider.Get(LibraryInfo.LibrarySiteID);
            }
            return mLibrarySiteInfo;
        }
    }


    /// <summary>
    /// Returns media library root folder path.
    /// </summary>
    private string MediaLibraryRootFolder
    {
        get
        {
            if ((mMediaLibraryRootFolder == null) && (LibrarySiteInfo != null))
            {
                mMediaLibraryRootFolder = MediaLibraryHelper.GetMediaRootFolderPath(LibrarySiteInfo.SiteName);
            }
            return mMediaLibraryRootFolder;
        }
    }


    /// <summary>
    /// Gets current starting path if set.
    /// </summary>
    private string StartingPath
    {
        get
        {
            if (Config.LibStartingPath != string.Empty)
            {
                string startingPath = Path.EnsureForwardSlashes(Config.LibStartingPath);
                if (startingPath != "/")
                {
                    startingPath = startingPath.Trim('/') + "/";

                    return Path.EnsureForwardSlashes(startingPath);
                }
            }
            return string.Empty;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        mediaView.OutputFormat = Config.OutputFormat;
        mediaView.Config = Config;
    }


    protected override void OnPreRender(EventArgs e)
    {
        // High-light item being edited
        if (ItemToColorize != Guid.Empty)
        {
            ColorizeRow(ItemToColorize.ToString());
        }

        // If full-listing mode is on
        ProcessIsFullListingMode();

        if (!wasLoaded && mediaView.Visible)
        {
            LoadData();
        }

        librarySelector.LoadLibraryData();

        // Make sure properties are hidden for non-existing library
        if (LibraryInfo == null)
        {
            ShowError(GetString("dialogs.libraries.nolibrary"));
        }

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();

            SetupProperties();

            InitializeDesignScripts();

            if (!RequestHelper.IsPostBack())
            {
                InitializeControls();

                LoadSelector();

                // Is item selected in the editor MediaFile? Load it
                if ((MediaSource != null) && (MediaSource.MediaFileID > 0))
                {
                    LoadSelectedItem();
                }
                else
                {
                    LoadUserConfiguration();
                }

                // Clear properties if link dialog is opened and no link is edited
                bool isLink = (Config.OutputFormat == OutputFormatEnum.BBLink || Config.OutputFormat == OutputFormatEnum.HTMLLink);
                if (isLink && !IsItemLoaded)
                {
                    ItemProperties.ClearProperties(true);
                }
            }
            else
            {
                FolderPath = LastFolderPath;
                mediaView.LibraryID = LibraryID;
                mediaView.ViewMode = menuElem.SelectedViewMode;
            }
        }
        else
        {
            Visible = false;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        base.ShowError(text, description, tooltipText, persistent);

        HideMediaElements();
    }


    /// <summary>
    /// Returns selected item parameters as name-value collection.
    /// </summary>
    public void GetSelectedItem()
    {
        // Clear unused information from the session
        ClearSelectedItemInfo();

        if (ItemProperties.Validate())
        {
            // Store tab information in the user's dialogs configuration
            StoreDialogsConfiguration();

            Hashtable props = ItemProperties.GetItemProperties();

            // Get JavaScript for inserting the item
            string insertItemScript = GetInsertItem(props);
            if (!string.IsNullOrEmpty(insertItemScript))
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "insertItemScript", ScriptHelper.GetScript(insertItemScript));
            }
        }
        else
        {
            pnlUpdateProperties.Update();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads selector element.
    /// </summary>
    private void LoadSelector()
    {
        librarySelector.LoadData();
    }


    /// <summary>
    /// Initializes properties controls.
    /// </summary>
    private void SetupProperties()
    {
        htmlLinkProp.Visible = false;
        htmlMediaProp.Visible = false;
        bbLinkProp.Visible = false;
        bbMediaProp.Visible = false;
        urlProp.Visible = false;

        if (ItemProperties != null)
        {
            ItemProperties.Visible = true;
            ItemProperties.Config = Config;
        }

        htmlLinkProp.StopProcessing = !htmlLinkProp.Visible;
        htmlMediaProp.StopProcessing = !htmlMediaProp.Visible;
        bbLinkProp.StopProcessing = !bbLinkProp.Visible;
        bbMediaProp.StopProcessing = !bbMediaProp.Visible;
        urlProp.StopProcessing = !urlProp.Visible;
    }


    /// <summary>
    /// Initializes additional controls.
    /// </summary>
    private void SetupControls()
    {
        SourceType = MediaSourceEnum.MediaLibraries;
        htmlMediaProp.SourceType = MediaSourceEnum.MediaLibraries;
        bbMediaProp.SourceType = MediaSourceEnum.MediaLibraries;
        urlProp.SourceType = MediaSourceEnum.MediaLibraries;

        // Set editor client ID for the properties
        ItemProperties.EditorClientID = Config.EditorClientID;
        ItemProperties.IsLiveSite = IsLiveSite;
        ItemProperties.SourceType = MediaSourceEnum.MediaLibraries;

        // Setup library selector
        InitializeLibrarySelector();

        // Set menu properties
        InitializeMenuElem();

        // Set media view properties
        InitializeViewElem();

        // Initialize helper scripts
        InitializeControlScripts();
    }


    /// <summary>
    /// Performs actions necessary to select particular item from a list.
    /// </summary>
    private void SelectMediaItem(string argument)
    {
        if (!string.IsNullOrEmpty(argument))
        {
            Hashtable argTable = CMSModules_MediaLibrary_Controls_MediaLibrary_MediaView.GetArgumentsTable(argument);
            if (argTable.Count >= 2)
            {
                Guid fileGuid = ValidationHelper.GetGuid(argTable["fileguid"], Guid.Empty);

                // Do not update properties when selecting recently edited image item
                bool avoidPropUpdate = (IsEditImage && (ItemToColorize == fileGuid));

                ItemToColorize = fileGuid;

                // Get information from argument
                string fileName = argTable["filename"].ToString();
                string fileExt = argTable["fileextension"].ToString();
                int imageWidth = ValidationHelper.GetInteger(argTable["fileimagewidth"], 0);
                int imageHeight = ValidationHelper.GetInteger(argTable["fileimageheight"], 0);
                long fileSize = ValidationHelper.GetLong(argTable["filesize"], 0);
                string fileUrl = argTable["url"].ToString();

                string fileNameWithouExtension = fileName;
                fileName = AttachmentHelper.GetFullFileName(fileName, fileExt);
                string filePermanentUrl;

                if (LibrarySiteInfo.SiteID != SiteContext.CurrentSiteID)
                {
                    filePermanentUrl = MediaFileURLProvider.GetMediaFileAbsoluteUrl(LibrarySiteInfo.SiteName, fileGuid, fileName);
                }
                else
                {
                    filePermanentUrl = Config.UseFullURL ? MediaFileURLProvider.GetMediaFileAbsoluteUrl(LibrarySiteInfo.SiteName, fileGuid, fileName) : MediaFileURLProvider.GetMediaFileUrl(fileGuid, fileName);
                }

                if (!fileUrl.StartsWith("~", StringComparison.Ordinal))
                {
                    filePermanentUrl = UrlResolver.ResolveUrl(filePermanentUrl);
                }

                ItemProperties.SiteDomainName = LibrarySiteInfo.DomainName;

                if (!avoidPropUpdate)
                {
                    SelectMediaItem(fileNameWithouExtension, fileExt, imageWidth, imageHeight, fileSize, fileUrl, filePermanentUrl);
                }
            }
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns file path without library root folder.
    /// </summary>
    /// <param name="argument">Original file path</param>
    private static string GetFilePath(string argument)
    {
        if (!string.IsNullOrEmpty(argument))
        {
            argument = Path.EnsureForwardSlashes(argument);

            int rootFolderNameIndex = argument.IndexOf("/", StringComparison.Ordinal);

            argument = (rootFolderNameIndex > -1) ? argument.Substring(rootFolderNameIndex) : string.Empty;

            return argument.TrimStart('/');
        }

        return string.Empty;
    }


    /// <summary>
    /// Returns folder path based on the given file path.
    /// </summary>
    /// <param name="filePath">File path</param>
    private static string GetFolderPath(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            int lastSlashIndex = filePath.LastIndexOf("/", StringComparison.Ordinal);

            return (lastSlashIndex > -1) ? filePath.Substring(0, lastSlashIndex) : string.Empty;
        }

        return string.Empty;
    }


    /// <summary>
    /// Returns complete folder path including library folder.
    /// </summary>
    /// <param name="path">Folder path.</param>
    private string GetCompletePath(string path)
    {
        // Include starting path if used
        if (StartingPath != string.Empty)
        {
            path = StartingPath + path;
        }

        path = (LibraryInfo != null) ? String.Format("{0}/{1}", LibraryInfo.LibraryFolder, path) : path;

        return path.TrimEnd('/');
    }


    /// <summary>
    /// Ensures folder path coming from JavaScript to keep normalized form.
    /// </summary>
    /// <param name="path">Path to ensure</param>
    private string EnsureFolderPath(string path)
    {
        char separator = folderTree.PathSeparator;

        return (!string.IsNullOrEmpty(path)) ? path.Replace('/', separator).Replace("\\\\", separator.ToString()).TrimStart(separator) : string.Empty;
    }


    /// <summary>
    /// Clears hidden control elements for the future use.
    /// </summary>
    private void ClearActionElems()
    {
        hdnAction.Value = string.Empty;
        hdnArgument.Value = string.Empty;
    }


    /// <summary>
    /// Hides or displays media elements as required.
    /// </summary>
    /// <param name="isDisplayed">Indicates whether the elements should display content</param>
    private void HandleMediaElements(bool isDisplayed)
    {
        // Folder tree
        folderTree.StopProcessing = !isDisplayed;
        // Reload tree and hide it if not displayed (Ajax toolkit bug)
        if (!isDisplayed)
        {
            folderTree.ReloadData();
            pnlTree.CssClass = "Hidden";
        }
        else
        {
            pnlTree.CssClass = string.Empty;
        }
        pnlUpdateTree.Update();

        // Media view
        mediaView.StopProcessing = !isDisplayed;
        mediaView.Visible = isDisplayed;
        pnlUpdateView.Update();

        // Properties
        ItemProperties.StopProcessing = !isDisplayed;
        ItemProperties.Visible = isDisplayed;
        pnlUpdateProperties.Update();

        lblEmpty.Text = CMSDialogHelper.GetSelectItemMessage(Config, MediaSourceEnum.MediaLibraries);
        pnlEmpty.Visible = !isDisplayed;
    }


    /// <summary>
    /// Hides media elements.
    /// </summary>
    private void HideMediaElements()
    {
        HandleMediaElements(false);
    }


    /// <summary>
    /// Displays media elements.
    /// </summary>
    private void DisplayMediaElements()
    {
        HandleMediaElements(true);
    }


    /// <summary>
    /// Displays properties in default size.
    /// </summary>
    private void DisplayNormal()
    {
        // Change CSS class so properties are displayed in normal size
        if (string.Equals(divDialogView.Attributes["class"], "DialogElementHidden", StringComparison.OrdinalIgnoreCase))
        {
            divDialogView.Attributes["class"] = "DialogViewContent scroll-area";
            divDialogResizer.Attributes["class"] = "DialogResizerVLine";
            divDialogProperties.Attributes["class"] = "DialogProperties";

            pnlUpdateContent.Update();
        }
    }


    /// <summary>
    /// Ensures that filter is no more applied.
    /// </summary>
    private void ResetSearchFilter()
    {
        mediaView.ResetSearch();
        LastSearchedValue = string.Empty;
    }


    /// <summary>
    /// Ensures first page is displayed in the control displaying the content.
    /// </summary>
    private void ResetPageIndex()
    {
        mediaView.ResetPageIndex();
    }


    /// <summary>
    /// Returns full file path including library folder.
    /// </summary>
    /// <param name="path">File path to get full path for</param>
    private string GetFullFilePath(string path)
    {
        if (LibraryInfo != null && path != null)
        {
            return Path.EnsureForwardSlashes(String.Format("{0}/{1}", LibraryInfo.LibraryFolder, path), true);
        }

        return string.Empty;
    }


    /// <summary>
    /// Gets folder path of the parent of the folder specified by its path.
    /// </summary>
    /// <param name="path">Path of the folder.</param>
    /// <param name="includeRoot">Indicates if root should be included</param>
    private string GetParentFullPath(string path, bool includeRoot = true)
    {
        if (!string.IsNullOrEmpty(path))
        {
            path = Path.EnsureForwardSlashes(path);

            int lastSlash = path.LastIndexOf("/", StringComparison.Ordinal);
            path = lastSlash > -1 ? path.Substring(0, lastSlash).Trim('/') : string.Empty;

            if (includeRoot && (LibraryInfo != null))
            {
                path = String.Format("{0}/{1}", LibraryInfo.LibraryFolder, path);
            }
        }

        if (path != null)
        {
            return path.TrimEnd('/');
        }

        return null;
    }


    /// <summary>
    /// Ensures sorting of data set containing both files and folders.
    /// </summary>
    /// <param name="ds">DateSet to sort</param>
    /// <param name="orderBy">Expression used to sort the data</param>
    private static void SortMixedDataSet(DataSet ds, string orderBy)
    {
        if ((ds != null) && !string.IsNullOrEmpty(orderBy) && !DataHelper.IsEmpty(ds))
        {
            DataHelper.SortDataTable(ds.Tables[0], orderBy);
            DataTable sortedResult = ds.Tables[0].DefaultView.ToTable();
            ds.Tables.RemoveAt(0);
            ds.Tables.Add(sortedResult);
        }
    }


    /// <summary>
    /// Imports folder details into given set of data.
    /// </summary>
    /// <param name="ds">DataSet folders information should be imported to</param>
    /// <param name="searchText">Search text</param>
    /// <returns>Number of added folders</returns>
    private int IncludeFolders(DataSet ds, string searchText)
    {
        int lastFolderIndex = 0;

        // Data object specified
        if ((ds != null) && (ds.Tables[0] != null))
        {
            // Get path to the currently selected folder
            string dirPath = DirectoryHelper.CombinePath(MediaLibraryRootFolder, LibraryInfo.LibraryFolder) + ((StartingPath != string.Empty) ? "\\" + StartingPath : string.Empty) + ((LastFolderPath != string.Empty) ? "\\" + LastFolderPath : string.Empty);
            dirPath = Path.EnsureSlashes(dirPath, true);

            if (Directory.Exists(dirPath))
            {
                // Get directories in the current path
                string[] dirs = Directory.GetDirectories(dirPath);
                if (dirs != null)
                {
                    string hiddenFolder = MediaLibraryHelper.GetMediaFileHiddenFolder(LibrarySiteInfo.SiteName);
                    foreach (string folderPath in dirs)
                    {
                        if (!folderPath.EndsWith(hiddenFolder, StringComparison.OrdinalIgnoreCase))
                        {
                            // Get directory info object to access additional information
                            DirectoryInfo dirInfo = DirectoryInfo.New(folderPath);
                            if (dirInfo != null)
                            {
                                DataRow dirRow = ds.Tables[0].NewRow();

                                bool includeFolder = true;
                                string fileName = Path.GetFileName(dirInfo.FullName);

                                if (!string.IsNullOrEmpty(searchText) && (fileName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) == -1))
                                {
                                    includeFolder = false;
                                }

                                // Insert new row
                                if (includeFolder)
                                {
                                    // Fill new row with data
                                    dirRow["FileGuid"] = Guid.Empty;
                                    dirRow["FileName"] = Path.GetFileName(dirInfo.FullName);
                                    dirRow["FilePath"] = dirPath;
                                    dirRow["FileExtension"] = "<dir>";
                                    dirRow["FileImageWidth"] = 0;
                                    dirRow["FileImageHeight"] = 0;
                                    dirRow["FileTitle"] = string.Empty;
                                    dirRow["FileSize"] = 0;
                                    dirRow["FileModifiedWhen"] = dirInfo.LastWriteTime;
                                    dirRow["FileSiteID"] = LibrarySiteInfo.SiteID;
                                    dirRow["FileID"] = 0;

                                    ds.Tables[0].Rows.InsertAt(dirRow, lastFolderIndex);

                                    lastFolderIndex++;
                                }
                            }
                        }
                    }
                }
            }
        }

        return lastFolderIndex;
    }


    /// <summary>
    /// Gets file path based on its file name, recently selected folder path and library folder.
    /// </summary>
    /// <param name="fileName">Name of the file (including extension)</param>
    private string CreateFilePath(string fileName)
    {
        return String.IsNullOrEmpty(LastFolderPath) ? fileName : DirectoryHelper.CombinePath(LastFolderPath, fileName);
    }


    /// <summary>
    /// Returns true if file information is not in database.
    /// </summary>
    /// <param name="fileName">File name</param>
    private DataRow FileIsNotInDatabase(string fileName)
    {
        if (FileList == null)
        {
            string where = String.Format("FilePath LIKE N'{0}%' AND FilePath NOT LIKE N'{0}_%/%' AND FileLibraryID = {1}",
                                         Path.EnsureForwardSlashes(SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(LastFolderPath))).Trim('/'),
                                         LibraryID);

            if (!string.IsNullOrEmpty(LastSearchedValue))
            {
                where = SqlHelper.AddWhereCondition(where, String.Format("((FileName LIKE N'%{0}%')  OR (FileExtension LIKE N'%{0}%'))",
                                                                              SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(LastSearchedValue))));
            }

            const string columns = "FileID, FilePath, FileGUID, FileName, FileExtension, FileImageWidth, FileImageHeight, FileTitle, FileSize, FileLibraryID, FileSiteID, FileDescription";

            // Get all files from current folder
            DataSet ds = MediaFileInfoProvider.GetMediaFiles(where, "FileName", 0, columns);
            if (ds != null)
            {
                FileList = new Hashtable();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    FileList[row["FilePath"].ToString()] = row;
                }
            }
        }

        if (FileList != null)
        {
            if (String.IsNullOrEmpty(LastFolderPath))
            {
                if (FileList.Contains(fileName))
                {
                    return (FileList[fileName] as DataRow);
                }
            }
            else
            {
                string filePath = String.Format("{0}/{1}", Path.EnsureForwardSlashes(LastFolderPath).Trim('/'), fileName);
                if (FileList.Contains(filePath))
                {
                    return (FileList[filePath] as DataRow);
                }
            }
        }

        return null;
    }

    #endregion


    #region "Dialog configuration"

    /// <summary>
    /// Loads selected item parameters into the selector.
    /// </summary>
    public void LoadSelectedItem()
    {
        var mediaSource = MediaSource;
        if (mediaSource != null)
        {
            IsItemLoaded = true;

            var library = LibraryInfo;

            // Try to pre-select media library
            if ((mediaSource.MediaFileLibraryID > 0) && (library != null))
            {
                librarySelector.SelectedLibraryID = mediaSource.MediaFileLibraryID;
                librarySelector.GlobalLibraryName = library.LibraryName;
            }

            // Try to pre-select path
            if (!string.IsNullOrEmpty(mediaSource.MediaFilePath))
            {
                // Without library root
                FolderPath = GetFolderPath(mediaSource.MediaFilePath);

                if (StartingPath == string.Empty)
                {
                    FolderPath = GetCompletePath(FolderPath);
                }
                else
                {
                    folderTree.PathToSelect = FolderPath;

                    folderTree.StopProcessing = true;

                    // With library root
                    FolderPath = GetFullFilePath(FolderPath);
                }

                // Save value to request
                RequestStockHelper.AddToStorage(LINK_MEDIA_SELECTOR_STORAGE_KEY, "FolderPath", FolderPath);
            }

            // Reload HTML properties
            if (Config.OutputFormat == OutputFormatEnum.HTMLMedia)
            {
                // Force media properties control to load selected item
                htmlMediaProp.ViewMode = mediaSource.MediaType;
            }

            // Ensure inserted media file URL
            string url;
            if (Config.OutputFormat == OutputFormatEnum.URL)
            {
                url = ValidationHelper.GetString(Parameters[DialogParameters.URL_URL], string.Empty);
            }
            else if (ImageHelper.IsImage(mediaSource.Extension))
            {
                url = ValidationHelper.GetString(Parameters[DialogParameters.IMG_URL], string.Empty);
            }
            else
            {
                url = ValidationHelper.GetString(Parameters[DialogParameters.AV_URL], string.Empty);
            }
            // Get permanent URL for media file
            if (url != string.Empty)
            {
                bool isDifferentSite = (mediaSource.SiteID != SiteContext.CurrentSiteID);
                string siteName = LibrarySiteInfo.SiteName;
                string fileName = AttachmentHelper.GetFullFileName(mediaSource.FileName, mediaSource.Extension);

                string filePermanentUrl =
                    isDifferentSite ?
                    MediaFileURLProvider.GetMediaFileAbsoluteUrl(siteName, mediaSource.MediaFileGuid, fileName) :
                    MediaFileURLProvider.GetMediaFileUrl(mediaSource.MediaFileGuid, fileName);

                if (library != null)
                {
                    string fileDirectUrl =
                        isDifferentSite ?
                            MediaFileURLProvider.GetMediaFileAbsoluteUrl(siteName, library.LibraryFolder, mediaSource.MediaFilePath) :
                            MediaFileURLProvider.GetMediaFileUrl(siteName, library.LibraryFolder, mediaSource.MediaFilePath);

                    Parameters[DialogParameters.URL_PERMANENT] = UrlResolver.ResolveUrl(filePermanentUrl);
                    Parameters[DialogParameters.URL_DIRECT] = UrlResolver.ResolveUrl(fileDirectUrl);
                }
            }

            // Load properties
            ItemProperties.LoadItemProperties(Parameters);
            pnlUpdateProperties.Update();

            // Remember item being edited for later high-lighting
            ItemToColorize = mediaSource.MediaFileGuid;
        }

        HandleMediaElements(true);
        ClearSelectedItemInfo();

        // Display properties in normal size
        DisplayNormal();
    }


    /// <summary>
    /// Stores current tab's configuration for the user.
    /// </summary>
    private void StoreDialogsConfiguration()
    {
        string path = GetCompletePath(LastFolderPath);

        // Actualize configuration
        UserInfo user = UserInfo.Provider.Get(MembershipContext.AuthenticatedUser.UserID);
        if (user != null)
        {
            user.UserSettings.UserDialogsConfiguration["media.sitename"] = LibrarySiteInfo.SiteName;
            user.UserSettings.UserDialogsConfiguration["media.libraryname"] = LibraryInfo.LibraryName;
            user.UserSettings.UserDialogsConfiguration["media.path"] = path;
            user.UserSettings.UserDialogsConfiguration["media.viewmode"] = CMSDialogHelper.GetDialogViewMode(menuElem.SelectedViewMode);

            user.UserSettings.UserDialogsConfiguration["selectedtab"] = CMSDialogHelper.GetMediaSource(MediaSourceEnum.MediaLibraries);

            // Update user info
            UserInfo.Provider.Set(user);
        }
    }


    /// <summary>
    /// Loads dialogs according user's configuration.
    /// </summary>
    private void LoadUserConfiguration()
    {
        if (MembershipContext.AuthenticatedUser.UserSettings.UserDialogsConfiguration != null)
        {
            XmlData dialogConfig = MembershipContext.AuthenticatedUser.UserSettings.UserDialogsConfiguration;

            string libraryName = (dialogConfig.ContainsColumn("media.libraryname") ? (string)dialogConfig["media.libraryname"] : string.Empty);
            string path = (dialogConfig.ContainsColumn("media.path") ? (string)dialogConfig["media.path"] : string.Empty);
            string siteName = (dialogConfig.ContainsColumn("media.sitename") ? (string)dialogConfig["media.sitename"] : string.Empty);

            // Set user dialogs configuration only if all sites available in selector or selected site is equal to users
            if ((librarySelector.Sites == AvailableSitesEnum.All) || (librarySelector.SelectedSiteName == siteName))
            {
                if ((libraryName != string.Empty) && (siteName != string.Empty))
                {
                    MediaLibraryInfo mli = MediaLibraryInfo.Provider.Get(libraryName, SiteInfoProvider.GetSiteID(siteName));
                    if (mli != null)
                    {
                        librarySelector.SelectedSiteName = siteName;
                        librarySelector.SelectedLibraryID = mli.LibraryID;
                    }
                }

                if (path != string.Empty)
                {
                    FolderPath = path;

                    if (StartingPath != string.Empty)
                    {
                        path = GetFilePath(path);
                    }

                    folderTree.PathToSelect = path;

                    // Save value to request
                    RequestStockHelper.AddToStorage(LINK_MEDIA_SELECTOR_STORAGE_KEY, "FolderPath", FolderPath);
                }
            }
        }
    }


    /// <summary>
    /// Ensures that full-listing mode is displayed when necessary.
    /// </summary>
    private void ProcessIsFullListingMode()
    {
        bool rootHasMore = (LastFolderPath == string.Empty) && ((CurrentAction == "select") || ((!wasLoaded || LibraryChanged) && (CurrentAction == string.Empty))) && folderTree.RootHasMore;
        if (IsFullListingMode || rootHasMore)
        {
            IsFullListingMode = (rootHasMore || IsFullListingMode);

            string folderPath = LastFolderPath;
            if (StartingPath != string.Empty)
            {
                folderPath = StartingPath + folderPath;
                folderPath = GetFullFilePath(folderPath);
            }

            // Check path of the edited item
            if (!RequestHelper.IsPostBack())
            {
                string editedPath = ValidationHelper.GetString(RequestStockHelper.GetItem(LINK_MEDIA_SELECTOR_STORAGE_KEY, "FolderPath"), String.Empty);
                folderPath = (editedPath != string.Empty) ? editedPath : folderPath;
            }

            string closeLink = String.Format("<span class=\"ListingClose\" style=\"cursor: pointer;\" onclick=\"SetAction('closelisting', ''); RaiseHiddenPostBack(); return false;\">{0}</span>", GetString("general.close"));
            string docNamePath = String.Format("<span class=\"ListingPath\">{0}</span>", GetFullFilePath(folderPath));

            string listingMsg = string.Format(GetString("media.libraryui.listingInfo"), docNamePath, closeLink);
            mediaView.DisplayListingInfo(listingMsg);
        }
        menuElem.ShowParentButton = (IsFullListingMode && (GetCompletePath(LastFolderPath) != GetFullFilePath(StartingPath)));
        pnlUpdateMenu.Update();
    }

    #endregion


    #region "Initialization methods"

    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void InitializeControls()
    {
        ViewMode = menuElem.SelectedViewMode;

        // View mode obtained from the user's settings
        XmlData dialogConfig = MembershipContext.AuthenticatedUser.UserSettings.UserDialogsConfiguration;
        if (dialogConfig != null)
        {
            // Get user's view mode
            string viewMode = (dialogConfig.ContainsColumn("media.viewmode") ? (string)dialogConfig["media.viewmode"] : string.Empty);
            if (viewMode != string.Empty)
            {
                ViewMode = CMSDialogHelper.GetDialogViewMode(viewMode);
            }
        }

        // Select default site
        SelectSite();

        mediaView.ViewMode = ViewMode;

        menuElem.SelectedViewMode = ViewMode;
    }


    /// <summary>
    /// Selects site based on according dialogs configuration.
    /// </summary>
    private void SelectSite()
    {
        // Select site based on dialog configuration
        string siteName = !string.IsNullOrEmpty(Config.LibSelectedSite) ? Config.LibSelectedSite : SiteContext.CurrentSiteName;

        if (Config.LibSites != AvailableSitesEnum.All)
        {
            librarySelector.SelectedSiteName = siteName;
        }

        // Select site based on selected item 
        if ((MediaSource != null) && (MediaSource.MediaFileLibraryID > 0))
        {
            LibraryInfo = MediaLibraryInfo.Provider.Get(MediaSource.MediaFileLibraryID);
            if (LibraryInfo != null)
            {
                siteName = SiteInfo.Provider.Get(LibraryInfo.LibrarySiteID).SiteName;
            }
        }
        // Select site based on user's configuration
        else
        {
            // Select side based on the user's configuration
            if (MembershipContext.AuthenticatedUser.UserSettings.UserDialogsConfiguration != null)
            {
                XmlData dialogConfig = MembershipContext.AuthenticatedUser.UserSettings.UserDialogsConfiguration;

                // Get site name
                string usersSiteName = (dialogConfig.ContainsColumn("media.sitename") ? (string)dialogConfig["media.sitename"] : string.Empty);
                if (usersSiteName != string.Empty)
                {
                    siteName = usersSiteName;
                }
            }
        }

        // Apply previously obtained sitename only when all sites are available
        if (Config.LibSites == AvailableSitesEnum.All)
        {
            librarySelector.SelectedSiteName = siteName;
        }
    }


    /// <summary>
    /// Initializes view element.
    /// </summary>
    private void InitializeViewElem()
    {
        mediaView.LibraryID = LibraryID;
        mediaView.IsLiveSite = IsLiveSite;

        UsePermanentUrls = true;
        mediaView.UsePermanentUrls = UsePermanentUrls;

        mediaView.ListViewControl.OnBeforeSorting += ListViewControl_OnBeforeSorting;
        mediaView.ListReloadRequired += mediaView_ListReloadRequired;
        mediaView.ListViewControl.DataSourceIsSorted = true;
        mediaView.GetInformation += mediaView_GetInformation;

        // Set media properties
        mediaView.SelectableContent = SelectableContent;
        mediaView.SourceType = SourceType;
        mediaView.ViewMode = menuElem.SelectedViewMode;

        // Set autoresize parameters
        mediaView.ResizeToHeight = Config.ResizeToHeight;
        mediaView.ResizeToMaxSideSize = Config.ResizeToMaxSideSize;
        mediaView.ResizeToWidth = Config.ResizeToWidth;

        // If folder was changed reset current page index for control displaying content
        switch (CurrentAction)
        {
            case "folderselect":
            case "clickformorefolder":
            case "morefolderselect":
            case "parentselect":
            case "clickformorelink":
                ResetPageIndex();
                break;
        }
    }


    /// <summary>
    /// Initializes menu element.
    /// </summary>
    private void InitializeMenuElem()
    {
        menuElem.LibraryFolderPath = (StartingPath + LastFolderPath).Trim('/');
        menuElem.ResizeToHeight = Config.ResizeToHeight;
        menuElem.ResizeToMaxSideSize = Config.ResizeToMaxSideSize;
        menuElem.ResizeToWidth = Config.ResizeToWidth;
        menuElem.DisplayMode = DisplayMode;
        menuElem.IsLiveSite = IsLiveSite;
        menuElem.SourceType = SourceType;
        menuElem.LibraryID = LibraryID;

        menuElem.UpdateViewMenu();
    }


    /// <summary>
    /// Loads folder tree.
    /// </summary>
    private void InitializeTree()
    {
        // Initialize folder tree control
        folderTree.CustomSelectFunction = "SetAction('folderselect', '##NODEVALUE##'); RaiseHiddenPostBack();";
        folderTree.CustomClickForMoreFunction = "SetAction('clickformore##TYPE##', '##NODEVALUE##'); RaiseHiddenPostBack();";
        folderTree.IsLiveSite = IsLiveSite;

        // Set starting path
        if (!string.IsNullOrEmpty(StartingPath))
        {
            folderTree.RootFolderPath = MediaLibraryRootFolder.TrimEnd('\\');
            folderTree.MediaLibraryFolder = Path.GetFileNameWithoutExtension(StartingPath.Trim('/'));
            folderTree.MediaLibraryPath = EnsureFolderPath(DirectoryHelper.CombinePath(LibraryInfo.LibraryFolder, StartingPath.Trim('/')));
        }
        else
        {
            folderTree.RootFolderPath = MediaLibraryRootFolder;
            folderTree.MediaLibraryFolder = LibraryInfo.LibraryFolder;
        }

        folderTree.ReloadData();
        pnlUpdateTree.Update();
    }


    /// <summary>
    /// Initializes library selector based on dialog configuration.
    /// </summary>
    private void InitializeLibrarySelector()
    {
        librarySelector.IsLiveSite = IsLiveSite;

        // Sites
        librarySelector.Sites = Config.LibSites;

        // Libraries
        librarySelector.GlobalLibraries = Config.LibGlobalLibraries;
        librarySelector.GlobalLibraryName = Config.LibGlobalLibraryName;
    }


    /// <summary>
    /// Initializes all the script required for communication between controls.
    /// </summary>
    private void InitializeControlScripts()
    {
        // Get reference causing postback to hidden button
        string postBackRef = ControlsHelper.GetPostBackEventReference(hdnButton, string.Empty);

        // Prepare for upload
        string refreshType = CMSDialogHelper.GetMediaSource(MediaSourceEnum.MediaLibraries);

        // SetAction function setting action name and passed argument
        string setAction = String.Format(@"
function SetAction(action, argument) {{
    var hdnAction = document.getElementById('{0}');
    var hdnArgument = document.getElementById('{1}');
    if ((hdnAction != null) && (hdnArgument != null)) {{
        if (action != null) {{
            hdnAction.value = action;
        }}
        if (argument != null) {{
            hdnArgument.value = argument;
        }}
    }}
}}
function RaiseHiddenPostBack(){{
    {2};
}}
function InitRefresh_{3}(message, fullRefresh, itemInfo, action) {{
    if((message != null) && (message != ''))
    {{
        window.alert(message);
    }}
    else
    {{
        SetAction('libraryfilecreated', itemInfo);
        RaiseHiddenPostBack();
    }}
}}
function imageEdit_Refresh(guid){{
    SetAction('edit', guid);
    RaiseHiddenPostBack();
}}", hdnAction.ClientID, hdnArgument.ClientID, postBackRef, refreshType);

        ltlScript.Text = ScriptHelper.GetScript(setAction);
    }

    #endregion


    #region "Load object methods"

    /// <summary>
    /// Loads library applying current library information.
    /// </summary>
    private void SelectLibrary()
    {
        LibraryID = librarySelector.LibraryID;
        if (LibraryID > 0)
        {
            // Reload data using new information
            DisplayMediaElements();

            // Initialize tree
            InitializeTree();

            // Reload view element
            InitializeViewElem();

            ResetPageIndex();
        }
    }


    /// <summary>
    /// Checks permissions for the current user taking specified action.
    /// </summary>
    private string CheckPermissions()
    {
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, PERMISSION_READ) &&
            !MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "LibraryAccess"))
        {
            return GetString("media.security.noaccess");
        }

        return string.Empty;
    }


    /// <summary>
    /// Loads all files for the view control.
    /// </summary>
    /// 
    private void LoadData(bool forceSetup = false)
    {
        mediaView.StopProcessing = false;

        LoadDataSource(LastSearchedValue);
        mediaView.Reload(forceSetup);

        wasLoaded = true;
    }


    /// <summary>
    /// Loads all files for the view control.
    /// </summary>
    /// <param name="searchText">Text to filter loaded files</param>
    private void LoadDataSource(string searchText)
    {
        // Load media files data
        if ((FolderPath != null) && (LibraryID > 0))
        {
            string err = CheckPermissions();
            if (err == string.Empty)
            {
                string filePath = (StartingPath + Path.EnsureForwardSlashes(FolderPath)).Trim('/');
                filePath = SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(filePath));

                // Create WHERE condition
                string where = String.Format("FilePath LIKE N'{0}%' AND FilePath NOT LIKE N'{1}_%/%' AND FileLibraryID = {2}", (String.IsNullOrEmpty(filePath) ? string.Empty : filePath + "/"), filePath, LibraryID);

                bool searchEnabled = !string.IsNullOrEmpty(searchText);
                if (searchEnabled)
                {
                    where = SqlHelper.AddWhereCondition(where, String.Format("(FileName LIKE N'%{0}%') OR (FileExtension LIKE N'%{0}%')", SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(searchText))));
                }

                // Don't use offset and page size for when searching (works only for first page)
                int offset = searchEnabled ? 0 : mediaView.CurrentOffset;
                int pageSize = searchEnabled ? 0 : mediaView.CurrentPageSize;

                const string columns = "FileGUID, FileName, FilePath, FileExtension, FileImageWidth, FileImageHeight, FileTitle, FileSize, FileModifiedWhen, FileSiteID, FileID, FileLibraryID, FileDescription";

                // Use different order by when thumbnails view mode is used or unigrid has no order set
                string orderBy = GetOrderBy();

                // Get files from current folder
                var query = MediaFileInfoProvider.GetMediaFiles(where, orderBy, -1, columns);

                if (IsFullListingMode)
                {
                    // In full listing mode get all data from begining to current page (to be able to sort folders)
                    query.Offset = 0;
                    query.MaxRecords = offset + pageSize;
                }
                else
                {
                    // Get current page data
                    query.Offset = offset;
                    query.MaxRecords = pageSize;
                }

                DataSet result = query.Result;
                int totalRecords = query.TotalRecords;

                // If folders should be displayed as well include them in the DataSet
                if (IsFullListingMode)
                {
                    // Add folders
                    totalRecords += IncludeFolders(result, searchText);

                    // Sort DataSet containing folders as well as files
                    SortMixedDataSet(result, orderBy);
                }

                // Trim DataSet to page size
                if (IsFullListingMode)
                {
                    int pagerForceNumberOfResults = -1;
                    result = DataHelper.TrimDataSetPage(result, offset, pageSize, ref pagerForceNumberOfResults);
                }

                // Pass data to the media view control
                mediaView.DataSource = result;
                mediaView.TotalRecords = totalRecords;
            }
            else
            {
                // Display error
                ShowError(err);
            }
        }
    }


    /// <summary>
    /// Gets correct order by depends on selected view mode and used unigrid sort.
    /// </summary>
    private string GetOrderBy()
    {
        if ((mediaView.ViewMode == DialogViewModeEnum.ThumbnailsView) || String.IsNullOrEmpty(mediaView.ListViewControl.SortDirect))
        {
            return String.Format("{0} {1}", SortColumns.Trim(), SortDirection);
        }

        return mediaView.ListViewControl.SortDirect;
    }

    #endregion


    #region "Common event methods"

    /// <summary>
    /// Handles actions related to the media folder.
    /// </summary>
    /// <param name="folderPath">Path of folder</param>
    /// <param name="isNewFolder">Indicates whether the action is related to the new folder created action</param>
    /// <param name="reloadTree">Indicates if tree should be reloaded</param>
    private void HandleFolderAction(string folderPath, bool isNewFolder, bool reloadTree = false)
    {
        // Update information on currently selected folder path
        FolderPath = GetFilePath(folderPath);

        // Reload tree if new folder was created     
        if (isNewFolder)
        {
            InitializeTree();

            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "EnsureTopWindow", ScriptHelper.GetScript("if (self.focus) { self.focus(); }"));
        }

        // Bear in mind starting path
        if (StartingPath != string.Empty)
        {
            if (isNewFolder)
            {
                folderPath = FolderPath;
            }
            // If action occurs as result of library changed - select root folder
            else if (FolderPath == string.Empty)
            {
                folderPath = StartingPath.Trim('/').Trim('\\');
            }
        }

        string selectPath = EnsureFolderPath(folderPath);

        // Check if required folder exists
        string folderToCheck = folderPath;
        if (isNewFolder || StartingPath != string.Empty)
        {
            if (isNewFolder && (StartingPath == string.Empty))
            {
                folderToCheck = GetFilePath(folderPath);
            }
            else
            {
                if (!isNewFolder)
                {
                    var startingFolderPath = GetFolderPath(StartingPath.TrimEnd('/'));
                    folderToCheck = string.IsNullOrEmpty(startingFolderPath) ? folderPath : $"{startingFolderPath}/{folderPath}";
                }
            }
        }
        else
        {
            folderToCheck = GetFilePath(folderPath);
        }


        if (!folderTree.FolderExists(folderToCheck))
        {
            // Select root folder by default
            FolderPath = string.Empty;
            selectPath = StartingPath == string.Empty ? (LibraryInfo == null) ? string.Empty : LibraryInfo.LibraryFolder : folderTree.MediaLibraryFolder;
        }

        if (isNewFolder && (StartingPath != string.Empty))
        {
            selectPath = EnsureFolderPath(selectPath);
        }

        if (reloadTree)
        {
            // Make sure the path is expanded in the tree
            folderTree.PathToSelect = selectPath;
            folderTree.ReloadData();
            pnlUpdateTree.Update();
        }

        // Select required folder if available
        folderTree.SelectPath(selectPath);

        // Update menu
        menuElem.LibraryID = LibraryID;
        menuElem.LibraryFolderPath = (!isNewFolder) ? (StartingPath + FolderPath).Trim('/') : FolderPath;
        menuElem.UpdateViewMenu();

        // Load new data and reload view control's content
        LoadData(true);
        pnlUpdateView.Update();

        DisplayNormal();
    }


    /// <summary>
    /// Handles actions occurring when some text is searched.
    /// </summary>
    /// <param name="argument">Argument holding information on searched text</param>
    private void HandleSearchAction(string argument)
    {
        LastSearchedValue = argument;

        // Load new data filtered by searched text and reload view control's content
        LoadData();

        pnlUpdateView.Update();

        // Keep focus in search text box
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "SetSearchFocus", ScriptHelper.GetScript("SetSearchFocus();"));
    }


    /// <summary>
    /// Handles actions occurring when some item is selected.
    /// </summary>
    /// <param name="argument">Argument holding information on selected item</param>
    private void HandleSelectAction(string argument)
    {
        // Create new selected media item and pass it to the properties dialog
        SelectMediaItem(argument);

        ClearActionElems();
    }


    /// <summary>
    /// Handles display more action.
    /// </summary>
    private void HandleDisplayMore(string argument)
    {
        // Set display mode
        IsFullListingMode = true;

        argument = argument.Replace('|', '/');
        HandleFolderAction(argument, false);

        pnlUpdateProperties.Update();

        ClearActionElems();
    }


    /// <summary>
    /// Handles attachment edit action.
    /// </summary>
    /// <param name="argument">Media file GUID coming from the view control</param>
    private void HandleEdit(string argument)
    {
        IsEditImage = true;

        if (!string.IsNullOrEmpty(argument))
        {
            string[] argArr = argument.Split('|');

            string siteName = argArr[1];

            Guid mediaFileGuid = ValidationHelper.GetGuid(argArr[0], Guid.Empty);
            MediaFileInfo mfi = MediaFileInfo.Provider.Get(mediaFileGuid, SiteInfoProvider.GetSiteID(siteName));
            if (mfi != null)
            {
                string url = mediaView.GetItemUrl(LibrarySiteInfo, mfi.FileGUID, mfi.FileName, mfi.FileExtension, mfi.FilePath, false, 0, 0, 0);
                string permUrl = mediaView.GetItemUrl(LibrarySiteInfo, mfi.FileGUID, mfi.FileName, mfi.FileExtension, mfi.FilePath, true, 0, 0, 0);

                if (ItemToColorize == mediaFileGuid)
                {
                    SelectMediaItem(mfi.FileName, mfi.FileExtension, mfi.FileImageWidth, mfi.FileImageHeight, mfi.FileSize, url, permUrl);
                }

                // Load new data and reload view control's content
                LoadData();
                // Update content to reflect changes made during editing
                pnlUpdateView.Update();
            }
        }

        ClearActionElems();
    }


    /// <summary>
    /// Handles actions occurring when new library file was created.
    /// </summary>
    /// <param name="argument">Argument holding information on new file path</param>
    private void HandleFileCreatedAction(string argument)
    {
        string[] argArr = argument.Split('|');
        if (argArr.Length == 2)
        {
            int mediaFileId = ValidationHelper.GetInteger(argArr[0], 0);

            MediaFileInfo fileInfo = MediaFileInfo.Provider.Get(mediaFileId);
            if (fileInfo != null)
            {
                if (CMSDialogHelper.IsItemSelectable(SelectableContent, fileInfo.FileExtension))
                {
                    // Get file URL
                    string fileUrl = mediaView.GetItemUrl(LibrarySiteInfo, fileInfo.FileGUID, fileInfo.FileName, fileInfo.FileExtension, fileInfo.FilePath, false, 0, 0, 0);
                    string permUrl = mediaView.GetItemUrl(LibrarySiteInfo, fileInfo.FileGUID, fileInfo.FileName, fileInfo.FileExtension, fileInfo.FilePath, true, 0, 0, 0);

                    SelectMediaItem(fileInfo.FileName, fileInfo.FileExtension, fileInfo.FileImageWidth,
                                    fileInfo.FileImageHeight, fileInfo.FileSize, fileUrl, permUrl);

                    ItemToColorize = fileInfo.FileGUID;
                }
            }

            // Trim root folder when starting path is set
            if (StartingPath != string.Empty)
            {
                string startingPath = StartingPath.Trim('/');
                if (FolderPath.StartsWith(startingPath, StringComparison.Ordinal))
                {
                    FolderPath = FolderPath.Substring(startingPath.Length);
                }
            }

            InitializeMenuElem();

            // Load new data and reload view control's content
            LoadData();

            pnlUpdateView.Update();
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Behaves as mediator in communication line between control taking action and the rest of the same level controls.
    /// </summary>
    protected void hdnButton_Click(object sender, EventArgs e)
    {
        // Get information on action causing postback        
        string argument = hdnArgument.Value;

        switch (CurrentAction)
        {
            case "insertitem":
                GetSelectedItem();
                break;

            case "search":
                HandleSearchAction(argument);
                break;

            case "select":
                HandleSelectAction(argument);
                break;

            case "clickformorefolder":
            case "clickformorelink":
                mediaView.ResetListSelection();

                ResetSearchFilter();
                HandleDisplayMore(argument);
                break;

            case "morefolderselect":
                string folderPath = CreateFilePath(argument);
                folderPath = StartingPath != string.Empty ? StartingPath + folderPath : GetFullFilePath(folderPath);

                ResetSearchFilter();
                HandleFolderAction(folderPath, false, true);

                ClearActionElems();
                break;

            case "libraryfilecreated":
                HandleFileCreatedAction(argument);
                break;

            case "folderselect":
                ResetSearchFilter();
                argument = argument.Replace('|', '/');
                HandleFolderAction(argument, false);
                break;

            case "parentselect":
                string path = StartingPath != string.Empty ? GetParentFullPath(StartingPath + LastFolderPath, false) : GetParentFullPath(LastFolderPath);

                ResetSearchFilter();
                HandleFolderAction(path, false);

                ClearActionElems();
                break;

            case "closelisting":
                IsFullListingMode = false;
                folderTree.CloseListing = true;

                folderPath = StartingPath != string.Empty ? StartingPath + LastFolderPath : GetFullFilePath(LastFolderPath);

                // Reload folder
                HandleFolderAction(folderPath, false);
                break;

            case "newfolder":
                argument = argument.Replace('|', '/').Replace("//", "/");

                ResetSearchFilter();
                HandleFolderAction(argument, true, true);
                break;

            case "cancelfolder":
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "EnsureTopWindow", ScriptHelper.GetScript("if (self.focus) { self.focus(); }"));
                ClearActionElems();
                break;

            case "edit":
                HandleEdit(argument);
                break;

            default:
                pnlUpdateView.Update();
                break;
        }
    }


    protected void librarySelector_LibraryChanged(object sender, EventArgs e)
    {
        // Force control to reload library info
        LibraryInfo = null;
        FolderPath = string.Empty;
        IsFullListingMode = false;
        LibraryChanged = true;

        // Do not clear item info when editing
        if (RequestHelper.IsPostBack())
        {
            ItemToColorize = Guid.Empty;
        }

        if (LibraryInfo != null)
        {
            // Load selected library
            SelectLibrary();

            // Select folder tree path obtained from the configuration
            string fullPath = !RequestHelper.IsPostBack() ? ValidationHelper.GetString(RequestStockHelper.GetItem(LINK_MEDIA_SELECTOR_STORAGE_KEY, "FolderPath"), String.Empty) : GetCompletePath(FolderPath);

            if (fullPath.StartsWith(LibraryInfo.LibraryFolder, StringComparison.Ordinal))
            {
                // Remove library folder when tree starts at starting path different from the library root folder
                if (StartingPath != string.Empty)
                {
                    fullPath = fullPath.Replace(LibraryInfo.LibraryFolder, string.Empty).TrimStart('/');
                }
            }
            else
            {
                fullPath = LibraryInfo.LibraryFolder;
            }

            ProcessIsFullListingMode();

            HandleFolderAction(fullPath, false, true);

            // Clear properties if library changed
            if (RequestHelper.IsPostBack())
            {
                ItemProperties.ClearProperties(true);
            }

        }
        else
        {
            ShowError(GetString("dialogs.libraries.nolibrary"));
        }

        // Update library selection
        pnlUpdateSelectors.Update();
    }


    /// <summary>
    /// Handles event occurring when inner media view control requires load of the data.
    /// </summary>
    private void mediaView_ListReloadRequired()
    {
        LoadData();
    }


    protected object mediaView_GetInformation(string type, object parameter)
    {
        if (type == null)
        {
            return null;
        }

        switch (type.ToLowerInvariant())
        {
            case "fileisnotindatabase":
                string fileName = ValidationHelper.GetString(parameter, string.Empty);
                return FileIsNotInDatabase(fileName);

            case "siteidrequired":
                return LibrarySiteInfo.SiteID;

            default:
                return null;
        }
    }


    protected void ListViewControl_OnBeforeSorting(object sender, EventArgs e)
    {
        GridViewSortEventArgs sortArg = (e as GridViewSortEventArgs);
        if (sortArg != null)
        {
            SortDirection = (mediaView.ListViewControl.SortDirect.EndsWith("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC");

            SortColumns = sortArg.SortExpression;
        }
    }

    #endregion
}
