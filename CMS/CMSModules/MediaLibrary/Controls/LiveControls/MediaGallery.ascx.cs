using System;
using System.Globalization;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.MediaLibrary.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_LiveControls_MediaGallery : CMSAdminControl
{
    #region "Variables"

    private MediaLibraryInfo mMediaLibrary;
    private string mMediaLibraryPath;
    private int mMediaFileID;
    private bool mDisplayActiveContent = true;
    private bool? mDisplayDetail;
    private bool mUseSecureLinks = true;

    private bool mHideControlForZeroRows = true;

    private bool hidden;

    #endregion


    #region "Content properties"

    /// <summary>
    /// Gets or sets the number which indicates how many files should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return mHideControlForZeroRows;
        }
        set
        {
            mHideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get;
        set;
    }


    /// <summary>
    /// Returns true if file ID query string is pressent.
    /// </summary>
    public bool? DisplayDetail
    {
        get
        {
            return mDisplayDetail ?? (mDisplayDetail = QueryHelper.GetInteger(FileIDQueryStringKey, 0) > 0);
        }
    }

    #endregion


    #region "UniPager properties"

    /// <summary>
    /// Gets or sets the unipager control.
    /// </summary>
    public UniPager UniPager
    {
        get
        {
            return UniPagerControl;
        }
        set
        {
            UniPagerControl = value;
        }
    }

    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public bool HidePagerForSinglePage
    {
        get
        {
            return UniPagerControl.HidePagerForSinglePage;
        }
        set
        {
            UniPagerControl.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of records to display on a page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return UniPagerControl.PageSize;
        }
        set
        {
            UniPagerControl.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return UniPagerControl.GroupSize;
        }
        set
        {
            UniPagerControl.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager mode ('querystring' or 'postback').
    /// </summary>
    public UniPagerMode PagerMode
    {
        get
        {
            return UniPagerControl.PagerMode;
        }
        set
        {
            UniPagerControl.PagerMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the querysting parameter.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return UniPagerControl.QueryStringKey;
        }
        set
        {
            UniPagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return UniPagerControl.DisplayFirstLastAutomatically;
        }
        set
        {
            UniPagerControl.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return UniPagerControl.DisplayPreviousNextAutomatically;
        }
        set
        {
            UniPagerControl.DisplayPreviousNextAutomatically = value;
        }
    }

    #endregion


    #region "UniPager Template properties"

    /// <summary>
    /// Gets or sets the pages template.
    /// </summary>
    public string PagesTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get;
        set;
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying selected file.
    /// </summary>
    public string SelectedItemTransformation
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying file list header.
    /// </summary>
    public string HeaderTransformation
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying file list footer.
    /// </summary>
    public string FooterTransformation
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for item separator.
    /// </summary>
    public string SeparatorTransformation
    {
        get;
        set;
    }


    /// <summary>
    /// Media library name.
    /// </summary>
    public string MediaLibraryName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets media library path to display files from.
    /// </summary>
    public string MediaLibraryPath
    {
        get
        {
            return (mMediaLibraryPath != null) ? mMediaLibraryPath.Trim('/') : mMediaLibraryPath;
        }
        set
        {
            mMediaLibraryPath = value;
        }
    }


    /// <summary>
    /// Indicates if subfolders content should be displayed.
    /// </summary>
    public bool ShowSubfoldersContent
    {
        get;
        set;
    }


    /// <summary>
    /// File list folder path.
    /// </summary>
    public string FolderPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FolderPath"], string.Empty);
        }
        set
        {
            ViewState["FolderPath"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the file id querystring parameter.
    /// </summary>
    public string FileIDQueryStringKey
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the sort querystring parameter.
    /// </summary>
    public string SortQueryStringKey
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the path querystring parameter.
    /// </summary>
    public string PathQueryStringKey
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets filter method.
    /// </summary>
    public int FilterMethod
    {
        get;
        set;
    }


    /// <summary>
    /// Media library info object.
    /// </summary>
    public MediaLibraryInfo MediaLibrary
    {
        get
        {
            return mMediaLibrary ?? (mMediaLibrary = MediaLibraryInfo.Provider.Get(MediaLibraryName, SiteContext.CurrentSiteID));
        }
    }


    /// <summary>
    /// Hide folder tree.
    /// </summary>
    public bool HideFolderTree
    {
        get;
        set;
    }


    /// <summary>
    /// Preview prefix for identification preview file.
    /// </summary>
    public string PreviewSuffix
    {
        get;
        set;
    }


    /// <summary>
    /// Icon set name.
    /// </summary>
    public string IconSet
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if active content (video etc.) should be displayed.
    /// </summary>
    public bool DisplayActiveContent
    {
        get
        {
            return mDisplayActiveContent;
        }
        set
        {
            mDisplayActiveContent = value;
        }
    }


    /// <summary>
    /// Indicates if file count in directory should be displayed in folder tree.
    /// </summary>
    public bool DisplayFileCount
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if file upload form should be displayed.
    /// </summary>
    public bool AllowUpload
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if preview file upload should be displayed in upload form.
    /// </summary>
    public bool AllowUploadPreview
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the links to media file should be processed in a secure way.
    /// </summary>
    public bool UseSecureLinks
    {
        get
        {
            return mUseSecureLinks;
        }
        set
        {
            mUseSecureLinks = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public string CacheItemName
    {
        get
        {
            return fileDataSource.CacheItemName;
        }
        set
        {
            fileDataSource.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public string CacheDependencies
    {
        get
        {
            return fileDataSource.CacheDependencies;
        }
        set
        {
            fileDataSource.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public int CacheMinutes
    {
        get
        {
            return fileDataSource.CacheMinutes;
        }
        set
        {
            fileDataSource.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets Check permissions property.
    /// </summary>
    public bool CheckLibraryPermissions
    {
        get
        {
            return fileDataSource.CheckPermissions;
        }
        set
        {
            fileDataSource.CheckPermissions = value;
        }
    }

    #endregion


    #region "Life cycle methods"

    protected override void CreateChildControls()
    {
        // Hide the control if there is no MediaLibrary
        if (MediaLibrary == null)
        {
            hidden = true;
            Visible = false;
            StopProcessing = true;
            StopProcessingInnerControls();
            return;
        }

        if (StopProcessing)
        {
            UniPagerControl.PageControl = null;
            StopProcessingInnerControls();
        }
        else
        {
            // Check 'Media gallery access' permission
            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(MediaLibrary, "Read") && !MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(MediaLibrary, "libraryaccess"))
            {
                RaiseOnNotAllowed("libraryaccess");
                StopProcessingInnerControls();
                return;
            }

            base.CreateChildControls();
            InitializeInnerControls();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (HideFolderTree)
        {
            folderTree.Visible = false;
            folderTreeContainer.Visible = false;
        }

        int fileId = QueryHelper.GetInteger(FileIDQueryStringKey, 0);
        bool hasFilter = false;
        if (fileId > 0)
        {
            hasFilter = true;
            fileDataSource.WhereCondition = "FileID = " + fileId.ToString("D", CultureInfo.InvariantCulture);
            fileDataSource.OrderBy = null;
            fileDataSource.FilePath = null;
            // Hide uploader
            fileUploader.Visible = false;
        }
        else
        {
            if (MediaLibrary != null)
            {
                hasFilter = true;
                fileDataSource.OrderBy = mediaLibrarySort.OrderBy;
                fileDataSource.LibraryName = MediaLibraryName;
                fileDataSource.WhereCondition = folderTree.WhereCondition;
            }
        }
        // Bind data into fileList
        if (hasFilter)
        {
            fileList.DataSource = fileDataSource.DataSource;
            fileList.DataBind();
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads the data in the control.
    /// </summary>
    public override void ReloadData()
    {
        fileDataSource.InvalidateLoadedData();
    }

    #endregion


    #region "Private methods"

    private void FilterChanged()
    {
        // Set uploader if upload is allowed
        if (AllowUpload)
        {
            fileUploader.DestinationPath = folderTree.CurrentFolder;
        }
        fileDataSource.InvalidateLoadedData();
    }


    private void fileList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.Controls.Count <= 0)
        {
            return;
        }

        // Try find control with id 'filePreview'
        MediaFilePreview ctrlFilePreview = e.Item.Controls[0].FindControl("filePreview") as MediaFilePreview;
        if (ctrlFilePreview == null)
        {
            return;
        }

        //Set control
        ctrlFilePreview.IconSet = IconSet;
        ctrlFilePreview.PreviewSuffix = PreviewSuffix;
        ctrlFilePreview.UseSecureLinks = UseSecureLinks;
        ctrlFilePreview.DisplayActiveContent = (DisplayDetail == true) || DisplayActiveContent;
    }


    private void InitializeInnerControls()
    {
        if (MediaLibrary != null)
        {
            // If the control was hidden because there were no data on init, show the control and process it
            if (hidden)
            {
                Visible = true;
                StopProcessing = false;
                folderTree.StopProcessing = false;
                fileDataSource.StopProcessing = false;
            }

            if (string.IsNullOrEmpty(MediaLibraryPath))
            {
                // If there is no path set
                folderTree.RootFolderPath = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName);
                folderTree.MediaLibraryFolder = MediaLibrary.LibraryFolder;
            }
            else
            {
                // Set root folder with library path
                folderTree.RootFolderPath = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName) + MediaLibrary.LibraryFolder;
                folderTree.MediaLibraryFolder = Path.GetFileName(MediaLibraryPath);
                folderTree.MediaLibraryPath = Path.EnsureForwardSlashes(MediaLibraryPath);
            }

            folderTree.FileIDQueryStringKey = FileIDQueryStringKey;
            folderTree.PathQueryStringKey = PathQueryStringKey;
            folderTree.FilterMethod = FilterMethod;
            folderTree.ShowSubfoldersContent = ShowSubfoldersContent;
            folderTree.DisplayFileCount = DisplayFileCount;

            // Get media file id from query
            mMediaFileID = QueryHelper.GetInteger(FileIDQueryStringKey, 0);

            // Media library sort
            mediaLibrarySort.OnFilterChanged += FilterChanged;
            mediaLibrarySort.FileIDQueryStringKey = FileIDQueryStringKey;
            mediaLibrarySort.SortQueryStringKey = SortQueryStringKey;
            mediaLibrarySort.FilterMethod = FilterMethod;

            // File upload properties
            string currentFolder = folderTree.SelectedPath;
            if (!String.IsNullOrEmpty(MediaLibraryPath))
            {
                currentFolder = String.Join("\\", MediaLibraryPath, currentFolder);
            }

            fileUploader.Visible = AllowUpload;
            fileUploader.EnableUploadPreview = AllowUploadPreview;
            fileUploader.PreviewSuffix = PreviewSuffix;
            fileUploader.LibraryID = MediaLibrary.LibraryID;
            fileUploader.DestinationPath = currentFolder;
            fileUploader.OnNotAllowed += fileUploader_OnNotAllowed;
            fileUploader.OnAfterFileUpload += fileUploader_OnAfterFileUpload;

            // Data properties
            fileDataSource.TopN = SelectTopN;
            fileDataSource.SiteName = SiteContext.CurrentSiteName;

            // Cache settings
            fileDataSource.CacheItemName = CacheItemName;
            fileDataSource.CacheDependencies = CacheDependencies;
            fileDataSource.CacheMinutes = CacheMinutes;

            fileDataSource.CheckPermissions = CheckLibraryPermissions;

            // UniPager properties
            UniPagerControl.PageSize = PageSize;
            UniPagerControl.GroupSize = GroupSize;
            UniPagerControl.QueryStringKey = QueryStringKey;
            UniPagerControl.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
            UniPagerControl.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
            UniPagerControl.HidePagerForSinglePage = HidePagerForSinglePage;
            UniPagerControl.PagerMode = PagerMode;

            // List properties
            fileList.HideControlForZeroRows = HideControlForZeroRows;
            fileList.ZeroRowsText = ZeroRowsText;
            fileList.ItemDataBound += fileList_ItemDataBound;

            // Initialize templates for FileList and UniPager
            InitTemplates();
        }

        // Append filter changed event if folder is hidden or path query string id is set
        if (!HideFolderTree || !String.IsNullOrEmpty(PathQueryStringKey))
        {
            folderTree.OnFilterChanged += FilterChanged;
        }

        // Folder tree
        if (HideFolderTree)
        {
            return;
        }

        folderTree.ImageFolderPath = GetImageUrl(CultureHelper.IsPreferredCultureRTL() ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree", true);
    }


    private void InitTemplates()
    {
        // If is media file id sets use SelectedItemTransformation and hide paging and sorting
        if (mMediaFileID > 0)
        {
            fileList.ItemTemplate = TransformationHelper.LoadTransformation(this, SelectedItemTransformation);
            UniPagerControl.Visible = false;
            mediaLibrarySort.StopProcessing = true;
            mediaLibrarySort.Visible = false;
        }
        else
        {
            // Else use transformation name
            fileList.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
        }

        if (!String.IsNullOrEmpty(HeaderTransformation))
        {
            fileList.HeaderTemplate = TransformationHelper.LoadTransformation(this, HeaderTransformation);
        }

        if (!String.IsNullOrEmpty(FooterTransformation))
        {
            fileList.FooterTemplate = TransformationHelper.LoadTransformation(this, FooterTransformation);
        }

        if (!String.IsNullOrEmpty(SeparatorTransformation))
        {
            fileList.SeparatorTemplate = TransformationHelper.LoadTransformation(this, SeparatorTransformation);
        }

        if (!String.IsNullOrEmpty(PagesTemplate))
        {
            UniPagerControl.PageNumbersTemplate = TransformationHelper.LoadTransformation(UniPagerControl, PagesTemplate);
        }

        if (!String.IsNullOrEmpty(CurrentPageTemplate))
        {
            UniPagerControl.CurrentPageTemplate = TransformationHelper.LoadTransformation(UniPagerControl, CurrentPageTemplate);
        }

        if (!String.IsNullOrEmpty(SeparatorTemplate))
        {
            UniPagerControl.PageNumbersSeparatorTemplate = TransformationHelper.LoadTransformation(UniPagerControl, SeparatorTemplate);
        }

        if (!String.IsNullOrEmpty(FirstPageTemplate))
        {
            UniPagerControl.FirstPageTemplate = TransformationHelper.LoadTransformation(UniPagerControl, FirstPageTemplate);
        }

        if (!String.IsNullOrEmpty(LastPageTemplate))
        {
            UniPagerControl.LastPageTemplate = TransformationHelper.LoadTransformation(UniPagerControl, LastPageTemplate);
        }

        if (!String.IsNullOrEmpty(PreviousPageTemplate))
        {
            UniPagerControl.PreviousPageTemplate = TransformationHelper.LoadTransformation(UniPagerControl, PreviousPageTemplate);
        }

        if (!String.IsNullOrEmpty(NextPageTemplate))
        {
            UniPagerControl.NextPageTemplate = TransformationHelper.LoadTransformation(UniPagerControl, NextPageTemplate);
        }

        if (!String.IsNullOrEmpty(PreviousGroupTemplate))
        {
            UniPagerControl.PreviousGroupTemplate = TransformationHelper.LoadTransformation(UniPagerControl, PreviousGroupTemplate);
        }

        if (!String.IsNullOrEmpty(NextGroupTemplate))
        {
            UniPagerControl.NextGroupTemplate = TransformationHelper.LoadTransformation(UniPagerControl, NextGroupTemplate);
        }

        if (!String.IsNullOrEmpty(LayoutTemplate))
        {
            UniPagerControl.LayoutTemplate = TransformationHelper.LoadTransformation(UniPagerControl, LayoutTemplate);
        }
    }


    private void StopProcessingInnerControls()
    {
        folderTree.StopProcessing = true;
        fileDataSource.StopProcessing = true;
        mediaLibrarySort.StopProcessing = true;
        fileUploader.StopProcessing = true;
    }


    private void fileUploader_OnNotAllowed(string permissionType, CMSAdminControl sender)
    {
        RaiseOnNotAllowed(permissionType);
    }


    private void fileUploader_OnAfterFileUpload()
    {
        fileDataSource.InvalidateLoadedData();
    }

    #endregion
}