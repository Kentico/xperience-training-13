using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaLibrary : CMSAdminItemsControl
{
    #region "Variables"

    private bool wasLoaded;
    private bool copyMoveInProgress;

    private MediaLibraryInfo mLibraryInfo;
    private SiteInfo mLibrarySiteInfo;

    private bool mDisplayOnlyImportedFiles = ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSMediaLibraryDisplayOnlyImportedFiles"], false);
    private string mLibraryRootFolder;
    private string mFolderPath = string.Empty;
    private string mLibraryPath = string.Empty;

    private int mAutoResizeWidth;
    private int mAutoResizeHeight;
    private int mAutoResizeMaxSideSize;

    private string mSortDirection = "ASC";
    private string mSortColumns = "FileName";

    private string mCurrentAction;

    private const string MEDIA_LIBRARY_FOLDER = "~/CMSModules/MediaLibrary/";

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


    /// <summary>
    /// ID of the currently processed library.
    /// </summary>
    public int LibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["MediaLibraryID"], 0);
        }
        set
        {
            mLibrarySiteInfo = null;
            mLibraryRootFolder = null;
            ViewState["MediaLibraryID"] = value;
        }
    }


    /// <summary>
    /// Current media library info.
    /// </summary>
    public MediaLibraryInfo LibraryInfo
    {
        get
        {
            // If media library already obtained and the current library ID in the ViewState is valid in the context
            if (mLibraryInfo != null && (mLibraryInfo.LibraryID == LibraryID))
            {
                return mLibraryInfo;
            }

            mLibraryInfo = MediaLibraryInfo.Provider.Get(LibraryID);
            return mLibraryInfo;
        }
        set
        {
            mLibraryInfo = value;
        }
    }


    /// <summary>
    /// Info on site related to the current media library.
    /// </summary>
    public SiteInfo LibrarySiteInfo
    {
        get
        {
            if ((LibraryInfo != null) && (mLibrarySiteInfo == null))
            {
                mLibrarySiteInfo = SiteInfo.Provider.Get(LibraryInfo.LibrarySiteID);
            }
            return mLibrarySiteInfo;
        }
    }


    /// <summary>
    /// Returns media library root folder path.
    /// </summary>
    public string LibraryRootFolder
    {
        get
        {
            if ((LibrarySiteInfo != null) && (mLibraryRootFolder == null))
            {
                mLibraryRootFolder = MediaLibraryHelper.GetMediaRootFolderPath(LibrarySiteInfo.SiteName);
            }
            return mLibraryRootFolder;
        }
    }


    /// <summary>
    /// Indicates if file count should be displayed in folder tree.
    /// </summary>
    public bool DisplayFilesCount
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value which determines whether only imported media files are displayed.
    /// </summary>
    public bool DisplayOnlyImportedFiles
    {
        get
        {
            return mDisplayOnlyImportedFiles;
        }
        set
        {
            mDisplayOnlyImportedFiles = value;
            mediaView.DisplayOnlyImportedFiles = value;
        }
    }


    /// <summary>
    /// Gets library relative url path.
    /// </summary>
    public string LibraryPath
    {
        get
        {
            if (String.IsNullOrEmpty(mLibraryPath))
            {
                if (LibraryInfo != null)
                {
                    mLibraryPath = LibraryRootFolder + LibraryInfo.LibraryFolder;
                }
            }
            return mLibraryPath;
        }
    }


    /// <summary>
    /// Indicates whether the UI should process all of the actions required.
    /// </summary>
    public bool ShouldProcess
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ShouldProcess"], false);
        }
        set
        {
            ViewState["ShouldProcess"] = value;
        }
    }


    /// <summary>
    /// Action control is displayed for.
    /// </summary>
    public string Action
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets a folder path of the media library.
    /// </summary>
    public string FolderPath
    {
        get
        {
            return mFolderPath.Replace("|", "/");
        }
        set
        {
            mFolderPath = value ?? string.Empty;
            folderActions.FolderPath = mFolderPath;
            LastFolderPath = mFolderPath;
        }
    }


    /// <summary>
    /// Gets or sets a folder path of the media library.
    /// </summary>
    public string CopyMovePath
    {
        get;
        set;
    }


    /// <summary>
    /// Sets of file names action is related to.
    /// </summary>
    public string Files
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether all available files should be processed.
    /// </summary>
    public bool AllFiles
    {
        get;
        set;
    }


    /// <summary>
    /// Current action name.
    /// </summary>
    private string CurrentAction
    {
        get
        {
            return mCurrentAction ?? (mCurrentAction = hdnAction.Value.Trim().ToLowerCSafe());
        }
    }


    /// <summary>
    /// Current action name.
    /// </summary>
    private string CurrentArgument
    {
        get
        {
            return hdnArgument.Value;
        }
    }


    /// <summary>
    /// Indicates whether the control was already reloaded.
    /// </summary>
    private bool ControlReloaded
    {
        get;
        set;
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
    /// Gets or sets selected item to colorize.
    /// </summary>
    private string ItemToColorize
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ItemToColorize"], string.Empty);
        }
        set
        {
            ViewState["ItemToColorize"] = value;
        }
    }


    /// <summary>
    /// GUID of the recently edited file.
    /// </summary>
    private Guid LastEditFileGuid
    {
        get
        {
            return ValidationHelper.GetGuid(ViewState["LastEditFileGuid"], Guid.Empty);
        }
        set
        {
            ViewState["LastEditFileGuid"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the image was recently edited.
    /// </summary>
    private bool IsEditImage
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsEditImage"], false);
        }
        set
        {
            ViewState["IsEditImage"] = value;
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
            pnlUpdateViewHidden.Update();
        }
    }


    /// <summary>
    /// Gets or sets last selected folder path.
    /// </summary>
    private string LastFolderPath
    {
        get
        {
            return hdnLastSelectedPath.Value.Replace("|", "/");
        }
        set
        {
            hdnLastSelectedPath.Value = value;
            pnlUpdateViewHidden.Update();
        }
    }


    /// <summary>
    /// Gets width files are resized to on upload.
    /// </summary>
    private int AutoResizeWidth
    {
        get
        {
            if ((LibrarySiteInfo != null) && (mAutoResizeWidth == 0))
            {
                mAutoResizeWidth = SettingsKeyInfoProvider.GetIntValue(LibrarySiteInfo.SiteName + ".CMSAutoResizeImageWidth");
            }
            return mAutoResizeWidth;
        }
    }


    /// <summary>
    /// Gets height files are resized to on upload.
    /// </summary>
    private int AutoResizeHeight
    {
        get
        {
            if ((LibrarySiteInfo != null) && (mAutoResizeHeight == 0))
            {
                mAutoResizeHeight = SettingsKeyInfoProvider.GetIntValue(LibrarySiteInfo.SiteName + ".CMSAutoResizeImageHeight");
            }
            return mAutoResizeHeight;
        }
    }


    /// <summary>
    /// Gets max side size files are resized to on upload.
    /// </summary>
    private int AutoResizeMaxSideSize
    {
        get
        {
            if ((LibrarySiteInfo != null) && (mAutoResizeMaxSideSize == 0))
            {
                mAutoResizeMaxSideSize = SettingsKeyInfoProvider.GetIntValue(LibrarySiteInfo.SiteName + ".CMSAutoResizeImageMaxSideSize");
            }
            return mAutoResizeMaxSideSize;
        }
    }


    /// <summary>
    /// Paths of the files to be imported.
    /// </summary>
    private string ImportFilePaths
    {
        get
        {
            return ValidationHelper.GetString(ViewState["MediaFilePaths"], string.Empty);
        }
        set
        {
            ViewState["MediaFilePaths"] = value;
        }
    }


    /// <summary>
    /// Overall number of the files to be imported.
    /// </summary>
    private int ImportFilesNumber
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["MediaFilesNumber"], 0);
        }
        set
        {
            ViewState["MediaFilesNumber"] = value;
        }
    }


    /// <summary>
    /// Index of the currently processed file.
    /// </summary>
    private int ImportCurrFileIndex
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["MediaCurrFileIndex"], 0);
        }
        set
        {
            ViewState["MediaCurrFileIndex"] = value;
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
    /// Indicates whether the control is displayed as part of the copy/move dialog.
    /// </summary>
    private bool IsCopyMoveLinkDialog
    {
        get
        {
            return ((Action == "copy") || (Action == "move") || (Action == "link") || (Action == "linkdoc"));
        }
    }


    /// <summary>
    /// Indicates if folder tree was loaded succesfully.
    /// </summary>
    public bool FolderTreeLoaded
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["FolderTreeLoaded"], false);
        }
        set
        {
            ViewState["FolderTreeLoaded"] = value;
        }
    }


    /// <summary>
    /// Control identifier.
    /// </summary>
    protected string Identifier
    {
        get
        {
            String identifier = ViewState["Identifier"] as String;
            if (identifier == null)
            {
                ViewState["Identifier"] = identifier = Guid.NewGuid().ToString("N");
            }

            return identifier;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        // Make sure view control is set as soon as possible
        mediaView.SourceType = MediaSourceEnum.MediaLibraries;

        // Set IsLiveSite
        menuElem.IsLiveSite = IsLiveSite;
        folderActions.IsLiveSite = IsLiveSite;
        folderTree.IsLiveSite = IsLiveSite;
        mediaView.IsLiveSite = IsLiveSite;
        fileEdit.IsLiveSite = IsLiveSite;
        folderEdit.IsLiveSite = IsLiveSite;
        fileImport.IsLiveSite = IsLiveSite;
        multipleImport.IsLiveSite = IsLiveSite;
        folderCopyMove.IsLiveSite = IsLiveSite;

        base.OnInit(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (ShouldProcess)
        {
            // High-light item being edited
            if (ItemToColorize != string.Empty)
            {
                ColorizeRow(ItemToColorize.ToLowerCSafe());
            }

            // Handle bulk actions message visibility
            lblBAWarning.Visible = !String.IsNullOrEmpty(lblBAWarning.Text);

            // If full-listing mode is on
            bool rootHasMore = (LastFolderPath == string.Empty) && ((CurrentAction == "select") || (!wasLoaded && CurrentAction == string.Empty)) && folderTree.RootHasMore;
            if (!IsCopyMoveLinkDialog && (IsFullListingMode || rootHasMore))
            {
                IsFullListingMode = (rootHasMore || IsFullListingMode);

                string closeLink = String.Format("<span class=\"ListingClose\" style=\"cursor: pointer;\" onclick=\"SetAction('closelisting', ''); RaiseHiddenPostBack(); return false;\">{0}</span>", GetString("general.close"));
                string docNamePath = String.Format("<span class=\"ListingPath\">{0}</span>", GetFullFilePath(LastFolderPath));

                string listingMsg = string.Format(GetString("media.libraryui.listingInfo"), docNamePath, closeLink);
                mediaView.DisplayListingInfo(listingMsg);
            }

            menuElem.ShowParentButton = (IsFullListingMode && (LastFolderPath != string.Empty));
            pnlUpdateMenu.Update();

            // Simulate library root selection
            if (!wasLoaded && mediaView.Visible)
            {
                string path = GetFullFilePath(LastFolderPath);
                HandleFolderAction(path, false, false, !IsCopyMoveLinkDialog, true);
            }

            // Pre-load edit folder control when loading - applied only when displaying in administration
            if (Action == null)
            {
                if (!RequestHelper.IsPostBack())
                {
                    DisplayFolderProperties();
                }
            }
            else
            {
                if (!copyMoveInProgress && (CurrentAction != "search") && !(RequestHelper.IsPostBack() && (CurrentAction == string.Empty)))
                {
                    // Otherwise load copy/move properties
                    DisplayCopyMoveProperties();
                }
            }

            folderActions.Update();

            if (IsCopyMoveLinkDialog && !RequestHelper.IsPostBack())
            {
                // Hide media elements
                mediaView.StopProcessing = true;
                mediaView.Visible = false;

                // Disable New folder button
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "DisableNewFolderOnLoad", ScriptHelper.GetScript("if (window.DisableNewFolderBtn) { window.DisableNewFolderBtn(); }"));
            }
        }

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            InitializeDesignScripts();

            // Initialize control by default when in administration or displayed on LiveSite and already reloaded
            if ((!IsLiveSite || ShouldProcess) && !ControlReloaded)
            {
                InitializeControl();
            }
        }
        else
        {
            Visible = false;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes control.
    /// </summary>
    private void InitializeControl()
    {
        Visible = true;
        ShouldProcess = true;

        RaiseOnCheckPermissions(PERMISSION_READ, this);

        // Initialize inner controls
        SetupControls();

        // Special treatment for specific actions
        HandleSpecialActions();
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Full-listing mode when copy/move dialog
        if (IsCopyMoveLinkDialog)
        {
            IsFullListingMode = IsCopyMoveLinkDialog;
        }

        // Sub-menu actions control
        if (LibraryInfo != null)
        {
            folderActions.LibraryID = LibraryID;
            folderActions.IsLiveSite = IsLiveSite;
            folderActions.OnCheckPermissions += folderActions_OnCheckPermissions;
            folderActions.DeleteScript = String.Format("if(confirm({0}) == true) {{ SetAction('delete', ''); RaiseHiddenPostBack(); }} return false;", string.Format(ScriptHelper.GetString(GetString("media.folder.delete.confirmation")), "##FOLDERPATH##"));
        }

        // Folder tree control
        folderTree.OnCheckPermissions += folderTree_OnCheckPermissions;
        folderTree.DisplayFilesCount = DisplayFilesCount && !DisplayOnlyImportedFiles;

        // Initialize controlling scripts
        InitializeControlScripts();

        // Initialize menu
        InitializeMenuElem();

        // Initialize view
        InitializeViewElem();

        if (!RequestHelper.IsPostBack() && (LibraryInfo != null))
        {
            // Initialize folder tree
            InitializeTree();
        }

        // Initialize actions
        menuElem.FileSystemActionsEnabled = FolderTreeLoaded;
        folderActions.FileSystemActionsEnabled = FolderTreeLoaded;
        folderActions.CopyEnabled = FolderTreeLoaded;
    }


    /// <summary>
    /// Reloads control and its content.
    /// </summary>
    public void ReLoadUserControl()
    {
        SetupControls();

        if (mediaView.Visible)
        {
            LoadData(true);
        }
    }


    /// <summary>
    /// Loads all files for the view control.
    /// </summary>
    private void LoadData(bool forceSetup = false)
    {
        try
        {
            mediaView.StopProcessing = false;

            LoadDataSource(LastSearchedValue);
            mediaView.Reload(forceSetup);

            wasLoaded = true;
        }
        catch (Exception ex)
        {
            // Display error
            ShowError(GetString("media.error.loadingdata"));
            Service.Resolve<IEventLogService>().LogException("Media library", "LOADDATA", ex);
        }
    }


    /// <summary>
    /// Gets the path for the current folder
    /// </summary>
    private string GetCurrentPath()
    {
        string path;

        if (string.IsNullOrEmpty(LastFolderPath))
        {
            path = LibraryPath + "/";
        }
        else
        {
            path = LibraryPath + "/" + Path.EnsureForwardSlashes(LastFolderPath) + "/";
        }
        path = Path.EnsureSlashes(path).Replace("|", "\\");

        return path;
    }


    /// <summary>
    /// Returns set of files in the file system.
    /// </summary>
    /// <param name="path">File system path to query</param>
    /// <param name="where">Where condition</param>
    private DataSet GetFileSystemDataSource(string path, string where)
    {
        if (!string.IsNullOrEmpty(where))
        {
            fileSystemDataSource.WhereCondition = where;
        }

        string orderBy = GetFileSystemOrderBy();
        if (!string.IsNullOrEmpty(orderBy))
        {
            fileSystemDataSource.OrderBy = orderBy;
        }

        if (path == null)
        {
            path = GetCurrentPath();
        }

        fileSystemDataSource.Path = path;

        return (DataSet)fileSystemDataSource.LoadData(true);
    }


    private string GetFileSystemOrderBy()
    {
        var generalSizeColumn = "[FileSize]";
        var fileSystemSizeColumn = "[Size]";
        var orderBy = GetOrderBy();
        var isSizeOrderBy = orderBy.IndexOf(generalSizeColumn, StringComparison.OrdinalIgnoreCase) >= 0;

        // File system data source has different field name for file size
        return isSizeOrderBy ? orderBy.Replace(generalSizeColumn, fileSystemSizeColumn) : orderBy;
    }


    /// <summary>
    /// Loads all files for the view control.
    /// </summary>
    /// <param name="searchText">Text to filter loaded files</param>
    private void LoadDataSource(string searchText)
    {
        // Load media files data
        if ((LastFolderPath != null) && (LibraryID > 0))
        {
            string err = CheckPermissions();
            if (err == string.Empty)
            {
                DataSet result = null;
                int totalRecords = 0;
                int offset = mediaView.CurrentOffset;
                int pageSize = mediaView.CurrentPageSize;
                string orderBy = GetOrderBy();

                if (!IsCopyMoveLinkDialog)
                {
                    // Get only imported files if required
                    if (DisplayOnlyImportedFiles)
                    {
                        string normFolderPath = Path.EnsureForwardSlashes(FolderPath).Trim('/');
                        normFolderPath = SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(normFolderPath));

                        // Create WHERE condition
                        string where = String.Format("FilePath LIKE N'{0}%' AND FilePath NOT LIKE N'{1}_%/%' AND FileLibraryID = {2}", (String.IsNullOrEmpty(normFolderPath) ? string.Empty : normFolderPath + "/"), normFolderPath, LibraryID);

                        if (!string.IsNullOrEmpty(searchText))
                        {
                            where += String.Format(" AND (FileName LIKE N'%{0}%')", SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(searchText)));
                        }

                        const string columns = "(FileName + FileExtension) as CompletFileName, FileID, FileName, FileGUID, FilePath, FileExtension, FileExtension as Extension, FileImageWidth, FileImageHeight, FileTitle, FileSize, FileModifiedWhen, FileSiteID, FileDescription";

                        // Get all files from current folder and pass it to the media view control
                        var query = MediaFileInfoProvider.GetMediaFiles(where, orderBy, -1, columns);

                        if (IsFullListingMode)
                        {
                            // In full listing mode get all data from beginning to current page (to be able to sort folders)
                            query.Offset = 0;
                            query.MaxRecords = offset + pageSize;
                        }
                        else
                        {
                            // Get current page data
                            query.Offset = offset;
                            query.MaxRecords = pageSize;
                        }

                        result = query.Result;
                        totalRecords = query.TotalRecords;
                    }
                    else
                    {
                        try
                        {
                            string where = string.Empty;
                            if (!string.IsNullOrEmpty(searchText))
                            {
                                where = String.Format("FileName LIKE '%{0}%'", SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(searchText)));
                            }

                            string path = GetCurrentPath();

                            result = GetFileSystemDataSource(path, where);
                            totalRecords = !DataHelper.DataSourceIsEmpty(result) ? result.Tables[0].Rows.Count : 0;
                        }
                        catch (Exception ex)
                        {
                            Service.Resolve<IEventLogService>().LogException("Media library", "READOBJ", ex);
                        }
                    }
                }
                else
                {
                    result = CreateEmptyDataSet();
                }

                // If folders should be displayed as well include them in the DataSet
                if (IsFullListingMode || IsCopyMoveLinkDialog)
                {
                    // Add folder
                    totalRecords += IncludeFolders(result, searchText);

                    // Sort DataSet containing folders as well as files
                    SortMixedDataSet(result, orderBy);
                }

                // Trim DataSet to page size or Thumbnails view mode is used (there is no automatic trim)
                if (IsFullListingMode || (mediaView.ViewMode == DialogViewModeEnum.ThumbnailsView))
                {
                    int pagerForceNumberOfResults = -1;
                    result = DataHelper.TrimDataSetPage(result, offset, pageSize, ref pagerForceNumberOfResults);
                }

                mediaView.DataSource = result;
                mediaView.TotalRecords = totalRecords;
            }
            else
            {
                lblNoLibraries.ResourceString = string.Empty;
                lblNoLibraries.CssClass = "ErrorLabel";
                lblNoLibraries.Text = err;

                // Display error
                ShowError(err);
            }
        }
    }


    /// <summary>
    /// Creates new DataSet containing all the required columns ready to be filled with data.
    /// </summary>
    private DataSet CreateEmptyDataSet()
    {
        DataSet result = new DataSet();
        DataTable table = new DataTable("files");

        // Create columns
        DataColumn column = new DataColumn("FileName");
        table.Columns.Add(column);
        column = new DataColumn("FilePath");
        table.Columns.Add(column);
        column = new DataColumn("Directory");
        table.Columns.Add(column);
        column = new DataColumn("Created");
        table.Columns.Add(column);
        column = new DataColumn("Modified");
        table.Columns.Add(column);
        column = new DataColumn("Extension");
        table.Columns.Add(column);
        column = new DataColumn("FileURL");
        table.Columns.Add(column);
        column = new DataColumn("Size");
        table.Columns.Add(column);

        // Add table to data set
        result.Tables.Add(table);

        return result;
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
            if (ds.Tables.Contains("files"))
            {
                ds.Tables.Remove("files");
            }
            else
            {
                if (ds.Tables.Contains("table"))
                {
                    ds.Tables.Remove("table");
                }
            }
            ds.Tables.Add(sortedResult);
        }
    }


    /// <summary>
    /// Imports folder details into given set of data.
    /// </summary>
    /// <param name="ds">DataSet folders information should be imported to</param>
    /// <param name="searchText">Search text</param>
    private int IncludeFolders(DataSet ds, string searchText)
    {
        int lastFolderIndex = 0;

        // Data object specified
        if ((ds != null) && (ds.Tables["files"] != null))
        {
            // Get path to the currently selected folder
            string dirPath = DirectoryHelper.CombinePath(LibraryRootFolder, LibraryInfo.LibraryFolder) + ((LastFolderPath != string.Empty) ? "\\" + LastFolderPath : string.Empty);
            dirPath = Path.EnsureSlashes(dirPath);
            if (Directory.Exists(dirPath))
            {
                // Get directories in the current path
                string[] dirs = Directory.GetDirectories(dirPath);
                if (dirs != null)
                {
                    string hiddenFolder = MediaLibraryHelper.GetMediaFileHiddenFolder(LibrarySiteInfo.SiteName);
                    foreach (string folderPath in dirs)
                    {
                        if (!folderPath.EndsWithCSafe(hiddenFolder, true))
                        {
                            // Get directory info object to access additional information
                            DirectoryInfo dirInfo = DirectoryInfo.New(folderPath);
                            if (dirInfo != null)
                            {
                                if (!DisplayOnlyImportedFiles)
                                {
                                    DataRow dirRow = ds.Tables["files"].NewRow();

                                    bool includeFolder = true;
                                    string fileName = Path.GetFileName(dirInfo.FullName).ToLowerCSafe();
                                    if (!string.IsNullOrEmpty(searchText) && !fileName.Contains(searchText.ToLowerCSafe()))
                                    {
                                        includeFolder = false;
                                    }

                                    // Insert new row
                                    if (includeFolder)
                                    {
                                        // Fill new row with data
                                        dirRow["FileName"] = Path.GetFileName(dirInfo.FullName);
                                        dirRow["FilePath"] = dirPath;
                                        dirRow["Directory"] = dirInfo.Name;
                                        dirRow["Created"] = dirInfo.CreationTime;
                                        dirRow["Modified"] = dirInfo.LastWriteTime;
                                        dirRow["Extension"] = "<dir>";

                                        ds.Tables["files"].Rows.InsertAt(dirRow, lastFolderIndex);

                                        lastFolderIndex++;
                                    }
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
    /// Performs actions necessary to select particular item from a list.
    /// </summary>
    private void SelectMediaItem(string argument)
    {
        if (!string.IsNullOrEmpty(argument) && (LibraryInfo != null))
        {
            Hashtable argTable = CMSModules_MediaLibrary_Controls_MediaLibrary_MediaView.GetArgumentsTable(argument);
            if (argTable.Count >= 2)
            {
                // Get file name
                string fileName;
                if (DisplayOnlyImportedFiles)
                {
                    if (argTable.Contains("extension"))
                    {
                        // File name + file extension
                        fileName = argTable["filename"] + argTable["extension"].ToString();
                    }
                    else
                    {
                        // File name + file extension
                        fileName = argTable["filename"] + argTable["fileextension"].ToString();
                    }
                }
                else
                {
                    // File name - already contains extension
                    fileName = argTable["filename"].ToString();
                }

                // Get media file path
                string filePath = CreateFilePath(fileName);

                // Try to get media file info based on its path
                FileInfo fi = FileInfo.New(MediaFileInfoProvider.GetMediaFilePath(LibrarySiteInfo.SiteName, LibraryInfo.LibraryFolder, filePath));
                if (fi != null)
                {
                    string path = Path.EnsureForwardSlashes(filePath);
                    string where = String.Format("(FileLibraryID = {0}) AND (FilePath = N'{1}')", LibraryID, path.TrimStart('/').Replace("'", "''"));

                    // Try to get file from DB
                    using (DataSet ds = MediaFileInfoProvider.GetMediaFiles(where))
                    {
                        if (!DataHelper.DataSourceIsEmpty(ds))
                        {
                            // If in DB get file info
                            MediaFileInfo fileInfo = new MediaFileInfo(ds.Tables[0].Rows[0]);
                            DisplayEditProperties(fileInfo);
                        }
                    }

                    // FileGUID for imported and FileName for non-imported media file
                    ItemToColorize = argTable.Contains("fileguid") ? argTable["fileguid"].ToString() : EnsureFileName(argTable["filename"].ToString());
                }
            }
        }

        wasLoaded = true;
    }


    /// <summary>
    /// Loads folder tree.
    /// </summary>
    private void InitializeTree()
    {
        // Initialize folder tree control
        folderTree.CustomClickForMoreFunction = "SetAction('clickformore##TYPE##', '##NODEVALUE##'); RaiseHiddenPostBack();";
        folderTree.CustomSelectFunction = "SetAction('folderselect', '##NODEVALUE##'); RaiseHiddenPostBack();";
        folderTree.RootFolderPath = LibraryRootFolder;
        folderTree.MediaLibraryFolder = LibraryInfo.LibraryFolder;

        folderTree.ReloadData();

        pnlUpdateTree.Update();

        FolderTreeLoaded = folderTree.RootNodeLoaded;
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
    /// Ensures that given path is selected when available in the tree.
    /// </summary>
    /// <param name="path">Path to select</param>
    private void SelectTreePath(string path)
    {
        SelectTreePath(path, false);
    }


    /// <summary>
    /// Ensures that given path is selected when available in the tree.
    /// </summary>
    /// <param name="path">Path to select</param>
    /// <param name="forceReload">Indicates if the reload on folder tree should be called explicitly</param>
    private void SelectTreePath(string path, bool forceReload)
    {
        folderTree.PathToSelect = path;

        if (forceReload)
        {
            InitializeTree();
        }
    }


    /// <summary>
    /// Creates directory info for given folder media library path.
    /// </summary>
    /// <param name="libraryFolderPath">Folder to get directory info for.</param>
    /// <param name="libraryID">Media library ID.</param>
    private DirectoryInfo GetMediaLibraryDirectoryInfo(string libraryFolderPath, int libraryID)
    {
        string path = DirectoryHelper.CombinePath(MediaLibraryInfoProvider.GetMediaLibraryFolderPath(libraryID), libraryFolderPath);
        DirectoryInfo directoryInfo = null;

        try
        {
            directoryInfo = DirectoryInfo.New(path);
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("Media library", "LOADDIRECTORY", ex);
            ShowError(GetString("media.error.loadingdata"));
        }

        return directoryInfo;
    }

    #endregion


    #region "Initialization methods"

    /// <summary>
    /// Initializes all the script required for communication between controls.
    /// </summary>
    private void InitializeControlScripts()
    {
        ScriptHelper.RegisterJQuery(Page);

        // SetAction function setting action name and passed argument
        string setAction = "function SetAction(action, argument) {                                              " +
                           "    var hdnAction = document.getElementById('" + hdnAction.ClientID + "');     " +
                           "    var hdnArgument = document.getElementById('" + hdnArgument.ClientID + "'); " +
                           @"    if ((hdnAction != null) && (hdnArgument != null)) {                             
                                   if (action != null) {                                                       
                                       hdnAction.value = action;                                               
                                   }                                                                           
                                   if (argument != null) {                                                     
                                       hdnArgument.value = argument;                                           
                                   }                                                                           
                               }                                                                               
                            }                                                                                   
                                                                                                               
                            function SelectRootFolder() {                                                       
                                var rootFolder = document.getElementById('0');                                  
                                if(rootFolder != null) {                                                        
                                    if(SelectFolder) {                                    
                                        SelectFolder(0, rootFolder);                                            
                                    }                                                                           
                                }                                                                               
                            }                                                                                   
                            
                            function MatchSelectPart(selectText) {
                                if (selectText != null) {
                                    var regExp = new RegExp(/SelectionClick.*\(this,/i);
                                    var m = regExp.exec(selectText);
                                    if (m != null) {
                                        return m[0];
                                    } 
                                }        
                                return null;
                            }

                            function GetOriginalFullName(selectText) {
                                if (selectText != null) {                                           " +
                           "        var regExp = new RegExp(/\".*\"/i);                             " +
                           @"        var m = regExp.exec(selectText);
                                    if (m != null) {                                                " +
                           "           var result = m[0].replace('\"', '').replace('\"', '');       " +
                           @"           return result;
                                   } 
                               }        
                               return null;
                           }
                           ";

        // Get reference causing postback to hidden button
        string postBackRef = ControlsHelper.GetPostBackEventReference(hdnButton, string.Empty);

        // RaiseOnAction function causing postback to the hidden button
        string raiseOnAction = "function RaiseHiddenPostBack(){" + postBackRef + ";}";

        // Prepare for upload
        string refreshType = CMSDialogHelper.GetMediaSource(MediaSourceEnum.MediaLibraries);

        string initRefresh = @" function InitRefresh_" + refreshType + "(message, fullRefresh, itemInfo, action) {      " +
                             @"  if((message != null) && (message != ''))                                   
                                     {                                                                          
                                        window.alert(message);                                                  
                                     }                                                                          
                                     else                                                                       
                                     {                                                                          
                                        SetAction('libraryfilecreated', itemInfo);                                 
                                        RaiseHiddenPostBack();                                                  
                                     }                                                                          
                                   }    
                                                                        
                                function InitRefresh_LibraryUpdate(message, fullRefresh, itemInfo, action) {      " +
                             @"  if((message != null) && (message != ''))                                   
                                     {                                                                          
                                        window.alert(message);                                                  
                                     }                                                                          
                                     else                                                                       
                                     {                                                                          
                                        SetAction('libraryfileupdated', itemInfo);                                 
                                        RaiseHiddenPostBack();                                                  
                                     }                                                                          
                                   }                                                                                    ";

        setAction += initRefresh;

        ltlScript.Text = ScriptHelper.GetScript(setAction + raiseOnAction);
    }


    /// <summary>
    /// Initializes view element.
    /// </summary>
    private void InitializeViewElem()
    {
        mediaView.LibraryID = LibraryID;
        mediaView.IsLiveSite = IsLiveSite;
        mediaView.IsCopyMoveLinkDialog = IsCopyMoveLinkDialog;

        // Generate permanent URLs whenever node GUID output required        
        mediaView.UsePermanentUrls = false;
        mediaView.ListReloadRequired += mediaView_ListReloadRequired;
        mediaView.GetInformation += mediaView_GetInformation;
        mediaView.ListViewControl.OnBeforeSorting += ListViewControl_OnBeforeSorting;
        mediaView.ListViewControl.GridView.RowDataBound += GridView_RowDataBound;
        mediaView.ListViewControl.DataSourceIsSorted = true;

        // Set media properties
        mediaView.SelectableContent = SelectableContentEnum.AllFiles;
        mediaView.ViewMode = menuElem.SelectedViewMode;
        mediaView.SourceType = MediaSourceEnum.MediaLibraries;

        // Set auto-resize parameters
        mediaView.ResizeToHeight = AutoResizeHeight;
        mediaView.ResizeToMaxSideSize = AutoResizeMaxSideSize;
        mediaView.ResizeToWidth = AutoResizeWidth;

        // Single file import initialization
        fileImport.SaveRequired += fileImport_SaveRequired;
        fileImport.Action += fileImport_Action;

        fileEdit.Action += fileEdit_Action;

        // Multiple file import initialization
        multipleImport.Action += multipleImport_Action;
        multipleImport.SaveRequired += multipleImport_SaveRequired;
        multipleImport.GetItemUrl += multipleImport_GetItemUrl;

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
    /// Initialize design jQuery scripts.
    /// </summary>
    private void InitializeDesignScripts()
    {
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterScriptFile(Page, MEDIA_LIBRARY_FOLDER + "Controls/MediaLibrary/MediaLibrary.js");

        CMSDialogHelper.RegisterDialogHelper(Page);

        StringBuilder sb = new StringBuilder();

        sb.Append("setTimeout('InitializeDesign();',200);");
        sb.Append("$cmsj(window).unbind('resize').bind('resize',function() { InitializeDesign(); });");

        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "designScript", ScriptHelper.GetScript(sb.ToString()));
    }


    /// <summary>
    /// Initializes menu element.
    /// </summary>
    private void InitializeMenuElem()
    {
        var libraryFolderPath = LastFolderPath.Trim('/');

        folderActions.LibraryFolderPath = libraryFolderPath;

        menuElem.DisplayMode = DisplayMode;
        menuElem.IsCopyMoveLinkDialog = IsCopyMoveLinkDialog;
        menuElem.IsLiveSite = IsLiveSite;
        menuElem.LibraryID = LibraryID;
        menuElem.LibraryFolderPath = libraryFolderPath;
        menuElem.ResizeToHeight = AutoResizeHeight;
        menuElem.ResizeToMaxSideSize = AutoResizeMaxSideSize;
        menuElem.ResizeToWidth = AutoResizeWidth;
        menuElem.IsLiveSite = IsLiveSite;
        menuElem.UpdateViewMenu();

        folderActions.Update();

        plcFolderActions.Visible = !IsCopyMoveLinkDialog;
    }


    /// <summary>
    /// Initializes control used to perform copy/move action.
    /// </summary>
    private void InitializeCopyMoveProperties()
    {
        folderCopyMove.Action = Action;
        folderCopyMove.AllFiles = AllFiles;
        folderCopyMove.Files = Files;
        folderCopyMove.FolderPath = CopyMovePath;
        folderCopyMove.IsLiveSite = IsLiveSite;
        folderCopyMove.MediaLibraryID = LibraryID;

        // folderCopyMove.NewPath is initialized on 'copymoveselect' action in hdnBtn_Click       

        folderCopyMove.ReloadData();
    }

    #endregion


    #region "Action handler methods"

    /// <summary>
    /// Handles special actions that require initialization as soon as possible.
    /// </summary>
    private void HandleSpecialActions()
    {
        if (CurrentAction == "massaction")
        {
            // Remember mass action so other controls are aware of
            FolderPath = LastFolderPath;

            if (menuElem.SelectedViewMode == DialogViewModeEnum.ListView)
            {
                LoadData();
            }
        }
        else
        {
            if (CurrentAction == "select")
            {
                HandleSelectAction(CurrentArgument);
            }
        }
    }


    /// <summary>
    /// Handles actions occurring when some text is searched.
    /// </summary>
    /// <param name="argument">Argument holding information on searched text</param>
    private void HandleSearchAction(string argument)
    {
        LastSearchedValue = argument;
        FolderPath = LastFolderPath;

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
    }


    /// <summary>
    /// Handles actions occurring when new library file was created.
    /// </summary>
    /// <param name="argument">Argument holding information on new file path</param>
    private void HandleFileCreatedAction(string argument)
    {
        mediaView.ResetListSelection();

        string[] argArr = argument.Split('|');
        if (argArr.Length == 2)
        {
            string path = argArr[1];
            int mediaFileId = ValidationHelper.GetInteger(argArr[0], 0);

            MediaFileInfo fileInfo = MediaFileInfo.Provider.Get(mediaFileId);
            if (fileInfo != null)
            {
                path = path.Replace('|', '/').Replace('>', '/');
                path = GetFullFilePath(path);

                SelectTreePath(path);

                // Reload tree if files count needs to be refreshed
                if (DisplayFilesCount)
                {
                    InitializeTree();
                }

                // Select file upload target location            
                string selectPath = EnsureFolderPath(path);
                folderTree.SelectPath(selectPath);
                pnlUpdateTree.Update();

                // Update current folder path information
                FolderPath = GetFilePath(path);

                // Initialize file edit control
                DisplayEditProperties(fileInfo);

                // Highlight newly created file
                ItemToColorize = (DisplayOnlyImportedFiles ? fileInfo.FileGUID.ToString() : EnsureFileName(Path.GetFileName(fileInfo.FilePath)));
                ColorizeRow(ItemToColorize);
            }

            // Load new data and reload view control's content
            LoadData();

            pnlUpdateView.Update();
        }
    }


    /// <summary>
    /// Handles actions occurring when existing library file was updated.
    /// </summary>
    /// <param name="argument">Argument holding information on updated file path</param>
    private void HandleFileUpdatedAction(string argument)
    {
        mediaView.ResetListSelection();

        string[] argArr = argument.Split('|');
        if (argArr.Length == 2)
        {
            string path = argArr[1];
            int mediaFileId = ValidationHelper.GetInteger(argArr[0], 0);

            MediaFileInfo mfi = MediaFileInfo.Provider.Get(mediaFileId);
            if (mfi != null)
            {
                path = GetFullFilePath(path.Replace('>', '\\'));

                // Update current folder path information
                FolderPath = GetFilePath(path);

                ItemToColorize = (DisplayOnlyImportedFiles ? mfi.FileGUID.ToString() : EnsureFileName(Path.GetFileName(mfi.FilePath)));

                // Initialize file edit control
                DisplayEditProperties(mfi);

                LoadData(true);
                pnlUpdateView.Update();

                // Highlight newly created file                
                ColorizeRowDelayed(ItemToColorize);
            }
        }

        wasLoaded = true;

        ClearActionElems();
    }


    /// <summary>
    /// Handles actions related to the media folder.
    /// </summary>
    /// <param name="folderPath">Argument of handled action</param>
    /// <param name="isNewFolder">Indicates whether the action is related to the new folder created action</param>
    /// <param name="displayNormal">Indicates whether the properties should be minimized</param>
    private void HandleFolderAction(string folderPath, bool isNewFolder, bool displayNormal = true)
    {
        HandleFolderAction(folderPath, isNewFolder, displayNormal, true);
    }


    /// <summary>
    /// Handles actions related to the media folder.
    /// </summary>
    /// <param name="folderPath">Argument of handled action</param>
    /// <param name="isNewFolder">Indicates whether the action is related to the new folder created action</param>
    /// <param name="displayNormal">Indicates whether the properties should be minimized</param>
    /// <param name="selectAndDisplay">Indicates whether the folder should be selected and displayed</param>
    private void HandleFolderAction(string folderPath, bool isNewFolder, bool displayNormal, bool selectAndDisplay)
    {
        HandleFolderAction(folderPath, isNewFolder, displayNormal, selectAndDisplay, selectAndDisplay);
    }


    /// <summary>
    /// Handles actions related to the media folder.
    /// </summary>
    /// <param name="folderPath">Argument of handled action</param>
    /// <param name="isNewFolder">Indicates whether the action is related to the new folder created action</param>
    /// <param name="displayNormal">Indicates whether the properties should be minimized</param>
    /// <param name="select">Indicates whether the folder should be selected</param>
    /// <param name="display">Indicates whether the content of folder should be displayed</param>
    private void HandleFolderAction(string folderPath, bool isNewFolder, bool displayNormal, bool select, bool display)
    {
        // Always show menu when some folder action is required
        menuElem.Visible = true;

        // Update information on currently selected folder path
        FolderPath = GetFilePath(folderPath);

        // Reload tree if new folder was created     
        if (isNewFolder)
        {
            InitializeTree();
        }

        if (select)
        {
            string selectPath = EnsureFolderPath(folderPath);
            folderTree.SelectPath(selectPath);
        }

        // Update menu
        menuElem.LibraryID = LibraryID;
        var libraryFolderPath = menuElem.LibraryFolderPath = (!isNewFolder) ? LastFolderPath.Trim('/') : LastFolderPath;
        folderActions.LibraryFolderPath = libraryFolderPath;
        menuElem.MediaLibraryDirectoryInfo = GetMediaLibraryDirectoryInfo(libraryFolderPath, LibraryID);
        menuElem.UpdateViewMenu();
        folderActions.Update();

        if (display)
        {
            // Load new data and reload view control's content
            LoadData(true);
            pnlUpdateView.Update();
        }

        if (displayNormal)
        {
            DisplayNormal();
        }
    }


    /// <summary>
    /// Handles folder edit section.
    /// </summary>
    private void HandleFolderEdit()
    {
        string newPath = folderEdit.ProcessFolderAction();
        if (newPath != null)
        {
            newPath = Path.EnsureForwardSlashes(newPath);

            SelectTreePath(newPath);

            HandleFolderAction(newPath, true);
        }

        pnlUpdateProperties.Update();

        wasLoaded = true;
    }


    /// <summary>
    /// Handles attachment edit action.
    /// </summary>
    /// <param name="argument">Media file path coming from the view control</param>
    private void HandleEditInitialization(string argument)
    {
        IsEditImage = true;

        if (!string.IsNullOrEmpty(argument))
        {
            string filePath = CreateFilePath(argument);

            MediaFileInfo mfi = MediaFileInfoProvider.GetMediaFileInfo(LibrarySiteInfo.SiteName, filePath, LibraryInfo.LibraryFolder);
            if (mfi != null)
            {
                LastEditFileGuid = mfi.FileGUID;

                string parameters = "mediafileguid=" + mfi.FileGUID + "&sitename=" + LibrarySiteInfo.SiteName + "&refresh=1";
                parameters += "&hash=" + QueryHelper.GetHash("?" + parameters, true);

                // Edit image media file
                if (ImageHelper.IsSupportedByImageEditor(mfi.FileExtension))
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(Page), "EditLibraryUI", ScriptHelper.GetScript("EditImage('" + parameters + "');"));
                }
                // Edit non-image media file
                else
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(Page), "EditLibraryUI", ScriptHelper.GetScript("Edit('" + parameters + "');"));
                }
            }
        }

        wasLoaded = true;

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

            Guid fileGuid = ValidationHelper.GetGuid(argArr[0], Guid.Empty);
            if (fileGuid != Guid.Empty)
            {
                MediaFileInfo mfi = MediaFileInfo.Provider.Get(fileGuid, LibrarySiteInfo.SiteID);
                if (mfi != null)
                {
                    if (LastEditFileGuid == mfi.FileGUID && fileEdit.Visible && (fileEdit.FileID == mfi.FileID))
                    {
                        DisplayEditProperties(mfi);

                        ItemToColorize = EnsureFileName(Path.GetFileName(mfi.FilePath));
                    }

                    // Update select action to reflect changes made during editing
                    LoadData(true);
                    pnlUpdateView.Update();
                }
            }
            else
            {
                ItemToColorize = EnsureFileName(ValidationHelper.GetString(argument, string.Empty));
            }
        }

        wasLoaded = true;

        ClearActionElems();
    }


    /// <summary>
    /// Handles delete action.
    /// </summary>
    /// <param name="argument">Media file GUID coming from the view control</param>
    private void HandleFolderDelete(string argument)
    {
        // Check 'Folder delete' permission
        if (MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "folderdelete"))
        {
            if (!string.IsNullOrEmpty(argument))
            {
                // Get path and parent path
                string path = argument;
                string parentPath = GetParentFullPath(path);

                try
                {
                    // Delete folder
                    MediaLibraryInfoProvider.DeleteMediaLibraryFolder(LibrarySiteInfo.SiteName, LibraryID, path, false);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }

                SelectTreePath(parentPath);

                // Reload tree and select parent folders
                HandleFolderAction(parentPath, true);
            }

            ClearActionElems();
        }
        else
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("folderdelete"));
        }
    }


    /// <summary>
    /// Handles file delete action.
    /// </summary>
    /// <param name="argument">Path to the file to delete</param>
    private void HandleDeleteFile(string argument)
    {
        // Check 'File create' permission
        if (MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filedelete"))
        {
            if (!string.IsNullOrEmpty(argument))
            {
                string filePath = CreateFilePath(argument);

                var deletedMediaFile = PerformDeleteMediaFile(filePath);

                if ((fileEdit.FileID > 0) && (((deletedMediaFile != null) && (deletedMediaFile.FileID != fileEdit.FileID)) || (deletedMediaFile == null)))
                {
                    // Reload file edit control if some other file is selected
                    fileEdit.ReLoadUserControl(true);
                }
                else
                {
                    fileEdit.FileID = 0;
                }
                pnlUpdateProperties.Update();

                // Reload folder content
                string path = GetFullFilePath(LastFolderPath);

                SelectTreePath(path, !DisplayFilesCount);

                HandleFolderAction(path, DisplayFilesCount);

                // If deleted item was currently selected one
                if (ItemToColorize == EnsureFileName(Path.GetFileName(filePath)))
                {
                    DisplayFolderProperties();
                }
            }

            wasLoaded = true;
        }
        else
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filedelete"));
        }
    }


    private MediaFileInfo PerformDeleteMediaFile(string filePath)
    {
        MediaFileInfo deletedMediaFile = null;
        try
        {
            // Delete media file
            deletedMediaFile = MediaFileInfoProvider.GetMediaFileInfo(LibraryID, filePath);
            if (deletedMediaFile != null)
            {
                // Delete media file info and file
                MediaFileInfo.Provider.Delete(deletedMediaFile);
            }
            else
            {
                // Delete file
                MediaFileInfoProvider.DeleteMediaFile(LibrarySiteInfo.SiteID, LibraryID, filePath);
            }
        }
        catch (Exception ex)
        {
            LogAndShowError("Error", "DELETEMEDIAFILE", ex);
        }

        return deletedMediaFile;
    }


    /// <summary>
    /// Handles importing single file.
    /// </summary>
    /// <param name="argument">Argument</param>
    private void HandleImportFile(string argument)
    {
        if (!string.IsNullOrEmpty(argument))
        {
            string filePath = CreateFilePath(argument);
            string path = MediaFileInfoProvider.GetMediaFilePath(LibrarySiteInfo.SiteName, LibraryInfo.LibraryFolder, filePath);

            // Try to get file info
            FileInfo fi = FileInfo.New(path);
            if (fi != null)
            {
                plcEditFolder.Visible = false;
                plcFileEdit.Visible = false;
                plcMultipleImport.Visible = false;

                plcImportFile.Visible = true;

                fileImport.SetupTexts();
                fileImport.FilePath = filePath;
                fileImport.LibraryID = LibraryID;
                fileImport.DisplayForm(fi);

                pnlUpdateProperties.Update();

                // FileGUID for imported and FileName for non-imported media file
                ItemToColorize = EnsureFileName(fi.Name);
            }
        }

        ClearActionElems();
        wasLoaded = true;
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

        DisplayFolderProperties();
        pnlUpdateProperties.Update();

        ClearActionElems();
    }


    /// <summary>
    /// Handles copy/move action.
    /// </summary>
    private void HandleCopyMoveAction()
    {
        copyMoveInProgress = true;

        InitializeCopyMoveProperties();

        folderCopyMove.IsLoad = !folderTree.FolderSelected;
        folderCopyMove.PerformAction();

        pnlUpdateProperties.Update();
    }


    /// <summary>
    /// Handles azure files import prepare action.
    /// To lower case all non imported files.
    /// </summary>
    private void HandleExternalStoragePrepare()
    {
        string path = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(LibrarySiteInfo.SiteName, LibraryInfo.LibraryFolder);
        path = DirectoryHelper.CombinePath(path, LastFolderPath);
        Directory.PrepareFilesForImport(path);

        SelectTreePath(path);

        // Reload tree if files count needs to be refreshed
        if (DisplayFilesCount)
        {
            InitializeTree();
        }

        // Select file upload target location            
        string selectPath = EnsureFolderPath(path);
        folderTree.SelectPath(selectPath);
        pnlUpdateTree.Update();

        // Load new data and reload view control's content
        LoadData();
        pnlUpdateView.Update();
    }

    #endregion


    #region "Bulk action methods"

    /// <summary>
    /// Handles bulk action.
    /// </summary>
    /// <param name="parameters">Action parameters.</param>
    private void HandleBulkAction(string parameters)
    {
        string[] tokens = parameters.Split('|');
        if (tokens.Length == 2 && LibraryInfo != null)
        {
            string action = tokens[0];
            IList<string> selectedFileNames = null;
            bool includeAllFiles = (tokens[1].ToLowerInvariant() == "all");
            if (!includeAllFiles)
            {
                string folderPath = DirectoryHelper.CombinePath(LibraryRootFolder + LibraryInfo.LibraryFolder, LastFolderPath);
                selectedFileNames = GetExistingSelectedFileNames(folderPath);
            }
            if (!includeAllFiles && selectedFileNames.Count == 0)
            {
                ShowBulkActionsError(GetString("Media.Error.NoFilesSelected"));
            }
            else
            {
                switch (action.ToLowerInvariant())
                {
                    case "copy":
                        BulkCopy(selectedFileNames, includeAllFiles);
                        break;
                    case "move":
                        BulkMove(selectedFileNames, includeAllFiles);
                        break;
                    case "delete":
                        BulkDelete(selectedFileNames, includeAllFiles);
                        break;
                    case "import":
                        BulkImport(selectedFileNames, includeAllFiles);
                        break;
                    default:
                        ShowBulkActionsError(GetString("media.mass.noaction"));
                        break;
                }
            }
        }
        wasLoaded = true;
        ClearActionElems();
    }


    /// <summary>
    /// Performs file delete action.
    /// </summary>
    /// <param name="fileName">Name for the file to delete</param>
    private void BulkDeleteFile(string fileName)
    {
        string filePath = CreateFilePath(fileName);

        // Delete media file
        MediaFileInfo mfi = MediaFileInfoProvider.GetMediaFileInfo(LibraryID, filePath);
        if (mfi != null)
        {
            // Delete media file info and file
            MediaFileInfo.Provider.Delete(mfi);
        }
        else
        {
            // Delete file
            MediaFileInfoProvider.DeleteMediaFile(LibrarySiteInfo.SiteID, LibraryID, filePath);
        }
    }


    /// <summary>
    /// Returns true if file information is not in database.
    /// </summary>
    /// <param name="fileName">File name</param>
    private DataRow FileIsNotInDatabase(string fileName)
    {
        if (FileList == null)
        {
            string where = String.Format("FilePath LIKE N'{0}%' AND FilePath NOT LIKE N'{0}_%/%' AND FileLibraryID = {1}", Path.EnsureForwardSlashes(SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(LastFolderPath))).Trim('/'), LibraryID);

            if (!string.IsNullOrEmpty(LastSearchedValue))
            {
                // Search using name and extension
                var name = Path.GetFileNameWithoutExtension(LastSearchedValue);
                var extension = Path.GetExtension(LastSearchedValue);
                // If there is no extension use the same search expression for the extension as well
                if (string.IsNullOrEmpty(extension))
                {
                    extension = name;
                }

                where += " AND ((FileName LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(name)) + "%') OR (FileExtension LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(extension)) + "%'))";
            }

            const string columns = "FileID, FilePath, FileGUID, FileName, FileExtension, FileImageWidth, FileImageHeight, FileTitle, FileSize, FileLibraryID, FileSiteID, FileDescription";

            // Get all files from current folder
            DataSet ds = MediaFileInfoProvider.GetMediaFiles(where, "FileName", 0, columns);
            if (ds != null)
            {
                FileList = new Hashtable();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    // Allow case insensitive decisioning
                    FileList[row["FilePath"].ToString().ToLowerCSafe()] = row;
                }
            }
        }

        if (FileList != null)
        {
            if (String.IsNullOrEmpty(LastFolderPath))
            {
                fileName = fileName.ToLowerCSafe();
                if (FileList.Contains(fileName))
                {
                    return (FileList[fileName] as DataRow);
                }
            }
            else
            {
                string filePath = Path.EnsureForwardSlashes(LastFolderPath).Trim('/') + "/" + fileName;
                filePath = filePath.ToLowerCSafe();
                if (FileList.Contains(filePath))
                {
                    return (FileList[filePath] as DataRow);
                }
            }
        }

        return null;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Behaves as mediator in communication line between control taking action and the rest of the same level controls.
    /// </summary>
    protected void hdnButton_Click(object sender, EventArgs e)
    {
        // Get information on action causing postback        
        string argument = CurrentArgument;

        switch (CurrentAction)
        {
            case "search":
                folderCopyMove.IsLoad = false;

                HandleSearchAction(argument);
                break;

            case "select":
                wasLoaded = true;
                ClearActionElems();
                // Handled in the Page_Load
                break;

            case "libraryfilecreated":
                HandleFileCreatedAction(argument);
                break;

            case "libraryfileupdated":
                HandleFileUpdatedAction(argument);
                break;

            case "clickformorefolder":
            case "clickformorelink":
                folderCopyMove.IsLoad = false;

                mediaView.ResetListSelection();

                ResetSearchFilter();
                HandleDisplayMore(argument);

                PerformCopyMoveInit();
                break;

            case "closelisting":
                IsFullListingMode = false;
                folderTree.CloseListing = true;
                folderCopyMove.IsLoad = false;

                // Reload folder
                string path = GetFullFilePath(LastFolderPath);

                SelectTreePath(path);

                HandleFolderAction(path, true);
                break;

            case "parentselect":
                folderCopyMove.IsLoad = false;

                path = GetParentFullPath(LastFolderPath);

                ResetSearchFilter();

                SelectTreePath(path);

                HandleFolderAction(path, true);

                DisplayFolderProperties();

                PerformCopyMoveInit();

                ClearActionElems();
                break;

            case "folderselect":
                folderCopyMove.IsLoad = false;

                ResetSearchFilter();

                if (IsCopyMoveLinkDialog)
                {
                    ItemToColorize = "";
                }

                argument = argument.Replace('|', '/');
                HandleFolderAction(argument, false);

                mediaView.ResetListSelection();

                // Only when folder isn't changed in copy/move dialog
                if (Action == null)
                {
                    DisplayFolderProperties();
                }

                PerformCopyMoveInit();
                ClearActionElems();
                break;

            case "morefolderselect":
                folderCopyMove.IsLoad = false;

                string folderPath = CreateFilePath(argument);
                folderPath = GetFullFilePath(folderPath);

                ResetSearchFilter();

                SelectTreePath(folderPath);

                HandleFolderAction(folderPath, true);

                DisplayFolderProperties();

                PerformCopyMoveInit();

                ClearActionElems();
                break;

            case "folderedit":
                HandleFolderEdit();
                break;

            case "delete":
                HandleFolderDelete(LastFolderPath);
                DisplayFolderProperties();
                break;

            case "newfolder":
                argument = argument.Replace('|', '/').Replace("//", "/").TrimEnd('.');
                ResetSearchFilter();

                SelectTreePath(argument, true);

                HandleFolderAction(argument, true);

                if (!IsCopyMoveLinkDialog)
                {
                    DisplayFolderProperties();
                }
                else
                {
                    folderCopyMove.IsLoad = false;

                    path = GetFilePath(argument);
                    ItemToColorize = EnsureFileName(path);
                    folderCopyMove.NewPath = path;
                }
                break;

            case "copymovefolder":
                argument = argument.Replace('|', '/').Replace("//", "/");
                argument = GetFullFilePath(argument);

                mediaView.ResetListSelection();

                SelectTreePath(argument);

                HandleFolderAction(argument, true);

                DisplayFolderProperties();
                break;

            case "copymoveselect":
                wasLoaded = true;
                ItemToColorize = EnsureFileName(argument);
                folderCopyMove.NewPath = CreateFilePath(argument);
                folderCopyMove.IsLoad = false;
                break;

            case "copymove":
                HandleCopyMoveAction();
                break;

            case "copymovefinished":
                argument = argument.Replace('|', '/');
                path = GetFullFilePath(argument);

                // Clear selection
                mediaView.ResetListSelection();
                // Clear searched text
                ResetSearchFilter();
                // Select target path
                SelectTreePath(path);
                // Load data from target path
                HandleFolderAction(path, true);
                //Display properties of target folder
                DisplayFolderProperties();
                break;

            case "massaction":
                HandleBulkAction(argument);
                break;

            case "editlibraryui":
                HandleEditInitialization(argument);
                break;

            case "edit":
                HandleEdit(argument);
                break;

            case "deletefile":
                HandleDeleteFile(argument);
                break;

            case "importfile":
                HandleImportFile(argument);
                break;

            case "externalstorageprepare":
                HandleExternalStoragePrepare();
                break;

            default:
                ColorizeLastSelectedRow();
                pnlUpdateView.Update();
                break;
        }
    }


    /// <summary>
    /// Handles event occurring when inner media view control requires load of the data.
    /// </summary>
    private void mediaView_ListReloadRequired()
    {
        LoadData();
    }


    private void fileImport_Action(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "save":
                MediaFileInfo mfi = (actionArgument as MediaFileInfo);
                if (mfi != null)
                {
                    // Update item view
                    LoadData(true);
                    pnlUpdateView.Update();

                    // Display item properties
                    DisplayEditProperties(mfi);

                    ItemToColorize = (DisplayOnlyImportedFiles ? mfi.FileGUID.ToString() : EnsureFileName(Path.GetFileName(mfi.FilePath)));

                    // Highlight newly created file                
                    ColorizeRowDelayed(ItemToColorize);
                }
                break;

            case "error":
                fileImport.SetupTexts();
                break;
        }

        wasLoaded = true;
    }


    protected MediaFileInfo fileImport_SaveRequired(FileInfo file, string title, string desc, string name, string filePath)
    {
        MediaFileInfo mfi = SaveNewFile(file, title, desc, name, filePath);

        string url = mediaView.GetItemUrl(LibrarySiteInfo, mfi.FileGUID, mfi.FileName, mfi.FileExtension, mfi.FilePath, false, 0, 0, 0);

        var sb = new StringBuilder();
        sb.Append("FileName|", mfi.FileName, ".", mfi.FileExtension.TrimStart('.'),
                  "|Extension|", mfi.FileExtension,
                  "|FilePath|", mfi.FilePath,
                  "|FileURL|", url,
                  "|Size|", mfi.FileSize);

        HandleSelectAction(sb.ToString());

        return mfi;
    }


    protected void multipleImport_Action(string actionName, object actionArgument)
    {
        if (!string.IsNullOrEmpty(actionName))
        {
            switch (actionName.ToLowerCSafe().Trim())
            {
                case "importnofiles":
                    ShowError(GetString("general.noitems"));
                    break;

                case "importcancel":
                    DisplayFolderProperties();
                    DisplayNormal();

                    // Show menu after import was done or cancelled
                    menuElem.Visible = true;
                    pnlUpdateMenu.Update();
                    break;

                case "reloaddata":
                    string path = GetFullFilePath(LastFolderPath);
                    HandleFolderAction(path, false);
                    break;

                case "singlefileimported":
                    multipleImport.SetupTexts();
                    wasLoaded = true;

                    // Hide menu after single file import
                    menuElem.Visible = false;
                    pnlUpdateMenu.Update();

                    break;

                case "importerror":
                    string msg = ValidationHelper.GetString(actionArgument, "");
                    multipleImport.DisplayError(msg);
                    wasLoaded = true;
                    break;
            }
        }

        pnlUpdateProperties.Update();
    }


    protected MediaFileInfo multipleImport_SaveRequired(FileInfo file, string title, string desc, string name, string filePath)
    {
        return SaveNewFile(file, title, desc, name, filePath);
    }


    protected string multipleImport_GetItemUrl(string fileName)
    {
        if (!String.IsNullOrEmpty(fileName))
        {
            string path = GetCurrentPath();
            string where = String.Format("FileName LIKE '{0}'", SqlHelper.EscapeLikeQueryPatterns(fileName).Replace("'", "''"));

            // Get the file data
            DataSet fileData = GetFileSystemDataSource(path, where);
            bool external = StorageHelper.IsExternalStorage(path);

            if (!DataHelper.DataSourceIsEmpty(fileData))
            {
                string originalUrl = fileData.Tables[0].Rows[0]["FileURL"].ToString();
                string fileUrl = originalUrl;

                if (external && fileUrl.StartsWithCSafe(SystemContext.ApplicationPath, true))
                {
                    fileUrl = fileUrl.Substring(SystemContext.ApplicationPath.Length);
                }

                fileUrl = File.GetFileUrl(fileUrl, SiteInfoProvider.GetSiteName(LibraryInfo.LibrarySiteID));

                // Return original url if not file found on external storage
                if (String.IsNullOrEmpty(fileUrl))
                {
                    fileUrl = originalUrl;
                }

                // Escape special characters if file is not on external storage (external storage handles URL characters on their own) 
                if (!external)
                {
                    fileUrl = URLHelper.EscapeSpecialCharacters(fileUrl);
                }

                return fileUrl;
            }
        }

        return string.Empty;
    }


    protected object mediaView_GetInformation(string type, object parameter)
    {
        switch (type.ToLowerCSafe())
        {
            case "fileisnotindatabase":
                string fileName = ValidationHelper.GetString(parameter, "");
                return FileIsNotInDatabase(fileName);

            case "siteidrequired":
                return LibrarySiteInfo.SiteID;

            default:
                return null;
        }
    }


    private void folderTree_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void folderActions_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void fileEdit_Action(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "reloadmedialibrary":
                HandleFileCreatedAction(actionArgument.ToString());

                // Update menu 
                folderActions.LibraryFolderPath = LastFolderPath.Trim('/');
                menuElem.UpdateViewMenu();
                folderActions.Update();
                break;
            default:
                ItemToColorize = EnsureFileName((string)actionArgument);
                break;
        }
    }


    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView drv = (e.Row.DataItem as DataRowView);
            if ((drv != null) && drv.DataView.Table.Columns.Contains("Extension") && (drv["Extension"].ToString().ToLowerCSafe() == "<dir>"))
            {
                // Disable selection check box for folder items
                CMSCheckBox selectBox = e.Row.Cells[0].Controls[0] as CMSCheckBox;

                if (selectBox != null)
                {
                    selectBox.Enabled = false;
                }
            }
        }
    }


    protected void ListViewControl_OnBeforeSorting(object sender, EventArgs e)
    {
        GridViewSortEventArgs sortArg = (e as GridViewSortEventArgs);
        if (sortArg != null)
        {
            SortDirection = (mediaView.ListViewControl.SortDirect.ToLowerCSafe().EndsWithCSafe("desc") ? "DESC" : "ASC");

            string sortExpr = (sortArg.SortExpression == "FileSize") ? "Size" : sortArg.SortExpression;
            SortColumns = sortExpr;
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns URL of the dialog to open.
    /// </summary>
    private string GetCopyMoveDialogUrl()
    {
        string result = MEDIA_LIBRARY_FOLDER + "CMSPages/SelectFolder.aspx";
        if (!IsLiveSite)
        {
            result = MEDIA_LIBRARY_FOLDER + "Tools/FolderActions/SelectFolder.aspx";
        }

        return result;
    }


    /// <summary>
    /// Performs actions required to reload control and its data.
    /// </summary>
    public override void ReloadData()
    {
        ControlReloaded = true;

        // Display default
        ClearForm();

        // Force control initialization
        InitializeControl();
    }


    /// <summary>
    /// Ensures all the actions to set control in its default state.
    /// </summary>
    public override void ClearForm()
    {
        FolderPath = string.Empty;
        InitializeTree();

        DisplayFolderProperties();
    }


    /// <summary>
    /// Stores new media file info into the DB.
    /// </summary>
    /// <param name="fi">Info on file to be stored</param>
    /// <param name="title">Title</param>
    /// <param name="description">Description of new media file</param>
    /// <param name="name">Name of new media file</param>
    /// <param name="filePath">File path</param>
    private MediaFileInfo SaveNewFile(FileInfo fi, string title, string description, string name, string filePath)
    {
        string path = Path.EnsureForwardSlashes(filePath);
        string fileName = name;

        string fullPath = fi.FullName;
        string extension = URLHelper.GetSafeFileName(fi.Extension, SiteContext.CurrentSiteName);

        // Check if filename is changed ad move file if necessary
        if (fileName + extension != fi.Name)
        {
            fullPath = MediaLibraryHelper.EnsureUniqueFileName(DirectoryHelper.CombinePath(Path.GetDirectoryName(fullPath), fileName) + extension);
            path = Path.EnsureForwardSlashes(Path.GetDirectoryName(path) + "/" + Path.GetFileName(fullPath)).TrimStart('/');
            // Rename file to new safe file name
            File.Move(fi.FullName, fullPath);
        }

        // Create media file info
        var fileInfo = new MediaFileInfo(fullPath, LibraryInfo.LibraryID, Path.EnsureForwardSlashes(Path.GetDirectoryName(path)));

        fileInfo.FileTitle = title;
        fileInfo.FileDescription = description;

        // Save media file info
        MediaFileInfoProvider.ImportMediaFileInfo(fileInfo);

        // Save FileID in ViewState
        fileEdit.FileID = fileInfo.FileID;
        fileEdit.FilePath = fileInfo.FilePath;

        return fileInfo;
    }


    /// <summary>
    /// Ensures given file name in the way it is usable as ID.
    /// </summary>
    /// <param name="fileName">Name of the file to ensure</param>
    private string EnsureFileName(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            char[] specialChars = "#;&,.+*~':\"!^$[]()=>|/\\-%@`{}".ToCharArray();
            foreach (char specialChar in specialChars)
            {
                fileName = fileName.Replace(specialChar, '_');
            }
            return fileName.Replace(" ", string.Empty).ToLowerCSafe();
        }

        return fileName;
    }


    /// <summary>
    /// Returns file path without library root folder.
    /// </summary>
    /// <param name="argument">Original file path</param>
    private string GetFilePath(string argument)
    {
        if (!string.IsNullOrEmpty(argument))
        {
            int rootFolderNameIndex = argument.IndexOfCSafe('/');

            if (rootFolderNameIndex > -1)
            {
                argument = argument.Substring(rootFolderNameIndex);
            }
            else
            {
                if (argument.StartsWithCSafe(LibraryInfo.LibraryFolder))
                {
                    argument = argument.Remove(0, LibraryInfo.LibraryFolder.Length);
                }
            }

            return argument.TrimStart('/');
        }

        return string.Empty;
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
    private string GetParentFullPath(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            path = Path.EnsureForwardSlashes(path);

            int lastSlash = path.LastIndexOfCSafe('/');
            if (lastSlash > -1)
            {
                path = path.Substring(0, lastSlash).Trim('/');
            }
            else
            {
                path = string.Empty;
            }

            if (LibraryInfo != null)
            {
                path = LibraryInfo.LibraryFolder + '/' + path;
            }
        }

        if (path != null)
        {
            return path.TrimEnd('/');
        }

        return null;
    }


    /// <summary>
    /// Gets file path based on its file name, recently selected folder path and library folder.
    /// </summary>
    /// <param name="fileName">Name of the file (including extension)</param>
    private string CreateFilePath(string fileName)
    {
        string filePath = String.IsNullOrEmpty(LastFolderPath) ? fileName : DirectoryHelper.CombinePath(LastFolderPath, fileName);

        return Path.EnsureSlashes(filePath).Replace('|', '\\');
    }


    /// <summary>
    /// Highlights item specified by its ID.
    /// </summary>
    /// <param name="itemId">String representation of item ID</param>
    private void ColorizeRow(string itemId)
    {
        // Keep item selected
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "ColorizeSelectedRow", ScriptHelper.GetScript("function tryColorizeRow(itemId) { if (ColorizeRow) { ColorizeRow(itemId); } else { setTimeout(\"tryColorizeRow('\" + itemId + \"')\", 500); } }; tryColorizeRow('" + itemId + "');"));
    }


    /// <summary>
    /// Highlights item specified by its ID.
    /// </summary>
    /// <param name="itemId">String representation of item ID</param>
    private void ColorizeRowDelayed(string itemId)
    {
        // Keep item selected
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "ColorizeSelectedRowDelayed", ScriptHelper.GetScript("function tryColorizeRow(itemId) { if (ColorizeRow) { setTimeout(\"tryColorizeRow('\" + itemId + \"')\", 500); } else { setTimeout(\"tryColorizeRow('\" + itemId + \"')\", 500); } }; tryColorizeRow('" + itemId + "');"));
    }


    /// <summary> 
    /// Highlights row recently selected.
    /// </summary>
    private void ColorizeLastSelectedRow()
    {
        // Keep item selected
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "ColorizeLastSelectedRow", ScriptHelper.GetScript("if (ColorizeLastRow) { ColorizeLastRow(); }"));
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
    /// Initializes and displays file info properties.
    /// </summary>
    /// <param name="fileInfo">File to edit</param>
    private void DisplayEditProperties(MediaFileInfo fileInfo)
    {
        plcFileEdit.Visible = true;

        plcEditFolder.Visible = false;
        folderEdit.StopProcessing = true;

        plcMultipleImport.Visible = false;
        plcImportFile.Visible = false;
        plcCopyMove.Visible = false;
        folderCopyMove.StopProcessing = true;

        fileEdit.StopProcessing = false;
        fileEdit.FileID = fileInfo.FileID;
        fileEdit.IsLiveSite = IsLiveSite;
        fileEdit.FileInfo = fileInfo;
        fileEdit.FilePath = fileInfo.FilePath;
        fileEdit.FolderPath = LastFolderPath;
        fileEdit.LibraryInfo = LibraryInfo;
        fileEdit.LibrarySiteInfo = LibrarySiteInfo;
        fileEdit.MediaLibraryID = LibraryID;
        fileEdit.IsLiveSite = IsLiveSite;
        fileEdit.ReLoadUserControl();

        pnlUpdateProperties.Update();
    }


    /// <summary>
    /// Initializes and displays the folder info properties.
    /// </summary>
    private void DisplayFolderProperties()
    {
        plcEditFolder.Visible = true;

        plcFileEdit.Visible = false;
        fileEdit.StopProcessing = true;

        plcMultipleImport.Visible = false;
        plcImportFile.Visible = false;
        plcCopyMove.Visible = false;
        folderCopyMove.StopProcessing = true;

        folderEdit.UseViewStateProperties = true;
        folderEdit.Action = "edit";
        folderEdit.CustomScript = "SetAction('folderedit', ''); RaiseHiddenPostBack(); return false;";
        folderEdit.IsLiveSite = IsLiveSite;
        folderEdit.LibraryID = LibraryID;
        folderEdit.LibraryFolder = LibraryInfo.LibraryFolder;
        folderEdit.FolderPath = LastFolderPath;

        // Special treatment for root folder
        if (LastFolderPath == string.Empty)
        {
            folderEdit.Enabled = false;
            folderEdit.FolderPath = LibraryInfo.LibraryFolder;
        }
        else
        {
            folderEdit.Enabled = true;
        }
        folderEdit.ReloadData();

        ItemToColorize = string.Empty;

        pnlUpdateProperties.Update();
    }


    /// <summary>
    /// Initializes and displays the multiple import properties.
    /// </summary>
    private void DisplayMultipleImportProperties()
    {
        plcEditFolder.Visible = false;
        plcFileEdit.Visible = false;
        fileEdit.StopProcessing = true;
        plcImportFile.Visible = false;
        plcCopyMove.Visible = false;
        folderCopyMove.StopProcessing = true;

        plcMultipleImport.Visible = true;

        // Hide menu for multiple import
        menuElem.Visible = false;
        pnlUpdateMenu.Update();

        // Initialize multiple files import control
        multipleImport.ImportCurrFileIndex = ImportCurrFileIndex;
        multipleImport.ImportFilePaths = ImportFilePaths;
        multipleImport.ImportFilesNumber = ImportFilesNumber;
        multipleImport.RootFolderPath = LibraryRootFolder;
        multipleImport.FolderPath = LastFolderPath;
        multipleImport.IsLiveSite = IsLiveSite;
        multipleImport.LibraryID = LibraryID;
        multipleImport.LibrarySiteInfo = LibrarySiteInfo;
        multipleImport.SetupTexts();
        multipleImport.SetupImport(true);

        // Display multiple files import control in full size
        DisplayFull();

        pnlUpdateProperties.Update();
    }


    /// <summary>
    /// Initializes and displays file info properties.
    /// </summary>
    private void DisplayCopyMoveProperties()
    {
        plcCopyMove.Visible = true;
        folderCopyMove.Visible = true;

        folderEdit.StopProcessing = true;
        plcFileEdit.Visible = false;
        plcEditFolder.Visible = false;
        plcMultipleImport.Visible = false;
        plcImportFile.Visible = false;

        InitializeCopyMoveProperties();

        pnlUpdateProperties.Update();
    }


    /// <summary>
    /// Displays properties in full size.
    /// </summary>
    private void DisplayFull()
    {
        // Change CSS class so properties are displayed in full size
        divDialogView.Attributes["class"] = "DialogElementHidden";
        divDialogResizer.Attributes["class"] = "DialogElementHidden";
        divDialogProperties.Attributes["class"] = "DialogPropertiesFullSize";

        pnlUpdateContent.Update();
    }


    /// <summary>
    /// Displays properties in default size.
    /// </summary>
    private void DisplayNormal()
    {
        if (divDialogView.Attributes["class"].EqualsCSafe("DialogElementHidden", true))
        {
            // Change CSS class so properties are displayed in normal size
            divDialogView.Attributes["class"] = "DialogViewContent MediaViewContent scroll-area";
            divDialogResizer.Attributes["class"] = "DialogResizerVLine";
            divDialogProperties.Attributes["class"] = "DialogProperties";

            // Initialize resizers
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "initresizers", ScriptHelper.GetScript("InitResizers();"));

            pnlUpdateContent.Update();
        }
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
    /// Performs copy/move new path initialization if necessary.
    /// </summary>
    private void PerformCopyMoveInit()
    {
        if (IsCopyMoveLinkDialog)
        {
            wasLoaded = true;
            folderCopyMove.NewPath = LastFolderPath;
            folderCopyMove.IsLoad = false;
        }
    }

    /// <summary>
    /// Copies the specified files (or all files) in the current folder.
    /// </summary>
    /// <param name="fileNames">A collection of file names.</param>
    /// <param name="includeAllFiles">A value indicating, whether all files in the current folder will be copied.</param>
    private void BulkCopy(IEnumerable<string> fileNames, bool includeAllFiles)
    {
        string baseUrl = ResolveUrl(GetCopyMoveDialogUrl());
        DialogUrlBuilder builder = new DialogUrlBuilder(baseUrl, Identifier).WithAction("copy").WithFolderPath(LastFolderPath).WithLibraryId(LibraryID);
        if (includeAllFiles)
        {
            builder.WithAllFiles();
        }
        else
        {
            builder.WithFileNames(fileNames);
        }
        string dialogUrl = builder.GetUrl();
        RegisterBulkActionStartupScript("modalCopy", dialogUrl, "CopyFiles");
    }


    /// <summary>
    /// Moves the specified files (or all files) in the current folder.
    /// </summary>
    /// <param name="fileNames">A collection of file names.</param>
    /// <param name="includeAllFiles">A value indicating, whether all files in the current folder will be moved.</param>
    private void BulkMove(IEnumerable<string> fileNames, bool includeAllFiles)
    {
        string baseUrl = ResolveUrl(GetCopyMoveDialogUrl());
        DialogUrlBuilder builder = new DialogUrlBuilder(baseUrl, Identifier).WithAction("move").WithFolderPath(LastFolderPath).WithLibraryId(LibraryID);
        if (includeAllFiles)
        {
            builder.WithAllFiles();
        }
        else
        {
            builder.WithFileNames(fileNames);
        }
        string dialogUrl = builder.GetUrl();
        RegisterBulkActionStartupScript("modalMove", dialogUrl, "MoveFiles");
    }


    /// <summary>
    /// Deletes the specified files (or all files) in the current folder.
    /// </summary>
    /// <param name="fileNames">A collection of file names.</param>
    /// <param name="includeAllFiles">A value indicating, whether all files in the current folder will be deleted.</param>
    private void BulkDelete(IEnumerable<string> fileNames, bool includeAllFiles)
    {
        // Check 'File delete' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filedelete"))
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filedelete"));
            return;
        }
        try
        {
            bool displayFolderProperties;
            if (includeAllFiles)
            {
                BulkDeleteAllFiles(out displayFolderProperties);
            }
            else
            {
                BulkDeleteFiles(fileNames, out displayFolderProperties);
            }
            FileList = null;
            // Reset data source, so that it gets reloaded with fresh data later
            fileSystemDataSource.InvalidateLoadedData();
            // Reload data
            string path = GetFullFilePath(LastFolderPath);
            SelectTreePath(path, !DisplayFilesCount);
            HandleFolderAction(path, DisplayFilesCount);
            // If deleted item was currently selected one
            if (displayFolderProperties)
            {
                DisplayFolderProperties();
            }
        }
        catch (Exception exception)
        {
            ShowError(GetString("media.delete.failed"), exception.Message);
        }
        finally
        {
            mediaView.ResetListSelection();
        }
    }


    /// <summary>
    /// Deletes the specified files in the current folder.
    /// </summary>
    /// <param name="fileNames">A collection of file names.</param>
    /// <param name="displayFolderProperties">A value indicating, whether folder properties should be displayed when the operation is complete.</param>
    private void BulkDeleteFiles(IEnumerable<string> fileNames, out bool displayFolderProperties)
    {
        displayFolderProperties = false;
        foreach (string fileName in fileNames)
        {
            BulkDeleteFile(fileName);
            string currentFilePath = CreateFilePath(fileName);
            if (ItemToColorize == EnsureFileName(Path.GetFileName(currentFilePath)))
            {
                displayFolderProperties = true;
            }
        }
    }


    /// <summary>
    /// Deletes all files in the current folder.
    /// </summary>
    /// <param name="displayFolderProperties">A value indicating, whether folder properties should be displayed when the operation is compelete.</param>
    private void BulkDeleteAllFiles(out bool displayFolderProperties)
    {
        displayFolderProperties = true;
        DataSet files = GetFileSystemDataSource(null, null);
        if (!DataHelper.IsEmpty(files))
        {
            foreach (DataRow file in files.Tables[0].Rows)
            {
                string fileName = ValidationHelper.GetString(file["FileName"], String.Empty);
                BulkDeleteFile(fileName);
            }
        }
    }


    /// <summary>
    /// Imports the specified files (or all files) in the current folder.
    /// </summary>
    /// <param name="fileNames">A collection of file names.</param>
    /// <param name="includeAllFiles">A value indicating, whether all files in the current folder will be imported.</param>
    private void BulkImport(IList<string> fileNames, bool includeAllFiles)
    {
        // Check 'File create' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filecreate"))
        {
            ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filecreate"));
            return;
        }
        if (includeAllFiles)
        {
            fileNames = new List<string>();
            DataSet files = GetFileSystemDataSource(null, null);

            foreach (DataRow file in files.Tables[0].Rows)
            {
                string fileName = file["FileName"].ToString();
                if (FileIsNotInDatabase(fileName) == null)
                {
                    fileNames.Add(fileName);
                }
            }
        }
        else
        {
            List<string> nonImported = new List<string>();
            foreach (string fileName in fileNames)
            {
                if (FileIsNotInDatabase(fileName) == null)
                {
                    nonImported.Add(fileName);
                }
            }

            fileNames = nonImported;
        }

        if (fileNames.Count == 0)
        {
            ShowError(GetString("media.file.import.allalreadyimported"));
        }
        else
        {
            ImportFilePaths = String.Join("|", fileNames.ToArray());
            ImportFilesNumber = fileNames.Count;
            ImportCurrFileIndex = 1;
            DisplayMultipleImportProperties();
        }
        mediaView.ResetListSelection();
    }


    /// <summary>
    /// Returns a collection of existing selected file names in media view.
    /// </summary>
    /// <param name="folderPath">The folder path for a check whether the file name refers to an existing file.</param>
    /// <returns>A collection of existing selected file names in media view.</returns>
    private IList<string> GetExistingSelectedFileNames(string folderPath)
    {
        List<string> fileNames = new List<string>(mediaView.SelectedItems.Count);
        folderPath = Path.EnsureEndSlash(folderPath);
        foreach (string fileName in mediaView.SelectedItems)
        {
            string filePath = folderPath + fileName;
            if (File.Exists(filePath)) fileNames.Add(fileName);
        }
        return fileNames;
    }


    /// <summary>
    /// Registers startup script that opens modal dialog for bulk actions.
    /// </summary>
    /// <param name="key">The key of the script.</param>
    /// <param name="dialogUrl">The dialog URL.</param>
    /// <param name="name">The name of the dialog.</param>
    private void RegisterBulkActionStartupScript(string key, string dialogUrl, string name)
    {
        string script = String.Format("modalDialog('{0}', '{1}', '90%', '70%');", ScriptHelper.GetString(dialogUrl, false), name);
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), key, ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Shows error message for bulk actions.
    /// </summary>
    /// <param name="text">Message text</param>
    private void ShowBulkActionsError(string text)
    {
        lblBAWarning.Text = text;
    }


    /// <summary>
    /// Represents a URL of the modal dialog for bulk actions.
    /// </summary>
    private class DialogUrlBuilder
    {
        #region "Variables"

        /// <summary>
        /// An instance of the StringBuilder class to accumulate the dialog URL.
        /// </summary>
        private readonly StringBuilder builder;


        /// <summary>
        /// Properties table used to store settings.
        /// </summary>
        private readonly Hashtable properties;


        /// <summary>
        /// A value indicating, whether the dialog URL contains at least one parameter.
        /// </summary>
        private bool hasParameters;


        /// <summary>
        /// Media library identifier used for window helper.
        /// </summary>
        private readonly String identifier;

        #endregion


        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the DialogBuilderClass with the specified dialog page URL.
        /// </summary>
        /// <param name="baseUrl">The dialog page URL.</param>
        /// <param name="id">Dialog ID</param>
        public DialogUrlBuilder(string baseUrl, string id)
        {
            builder = new StringBuilder(baseUrl);
            identifier = id;
            properties = new Hashtable();
            WindowHelper.Remove(identifier);
        }

        #endregion


        #region "Public methods"

        /// <summary>
        /// Appends the specified action to the current dialog URL.
        /// </summary>
        /// <param name="action">An action.</param>
        /// <returns>A reference to this instance.</returns>
        public DialogUrlBuilder WithAction(string action)
        {
            AddParameter("action", action);
            return this;
        }


        /// <summary>
        /// Appends the specified folder path to the current dialog URL.
        /// </summary>
        /// <param name="folderPath">A folder path.</param>
        /// <returns>A reference to this instance.</returns>
        public DialogUrlBuilder WithFolderPath(string folderPath)
        {
            AddProperty("path", Path.EnsureForwardSlashes(folderPath));
            return this;
        }


        /// <summary>
        /// Appends the specified library identifier to the current dialog URL.
        /// </summary>
        /// <param name="libraryId">A library identifier.</param>
        /// <returns>A reference to this instance.</returns>
        public DialogUrlBuilder WithLibraryId(int libraryId)
        {
            AddProperty("libraryid", libraryId.ToString("D", CultureInfo.InvariantCulture));
            return this;
        }


        /// <summary>
        /// Appends the specified file names to the current dialog URL.
        /// </summary>
        /// <param name="fileNames">A collection of file names.</param>
        /// <returns>A reference to this instance.</returns>
        public void WithFileNames(IEnumerable<string> fileNames)
        {
            AddProperty("files", String.Join("|", fileNames.ToArray()));
        }


        /// <summary>
        /// Appends the "all files" flag to the current dialog URL.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public void WithAllFiles()
        {
            AddProperty("allFiles", "1");
        }


        /// <summary>
        /// Returns the current dialog URL.
        /// </summary>
        /// <returns>The current dialog URL.</returns>
        public string GetUrl()
        {
            AddParameter("identifier", identifier);

            string url = builder.ToString();
            url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url, false));

            WindowHelper.Add(identifier, properties);

            return url;
        }

        #endregion


        #region "Private methods"

        /// <summary>
        /// Appends a parameter to the current dialog URL without encoding.
        /// </summary>
        /// <param name="name">A parameter name</param>
        /// <param name="value">A parameter value</param>
        private void AddParameter(string name, string value)
        {
            builder.Append(hasParameters ? "&" : "?").Append(name).Append("=").Append(value);
            hasParameters = true;
        }


        /// <summary>
        /// Adds property to hashtable.
        /// </summary>
        /// <param name="name">A property name</param>
        /// <param name="value">A property value</param>
        private void AddProperty(string name, string value)
        {
            properties.Add(name, value);
        }

        #endregion
    }

    #endregion
}
