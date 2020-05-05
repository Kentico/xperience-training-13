using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

public partial class CMSWebParts_MediaLibrary_MediaGallery : CMSAbstractWebPart
{
    #region "Content properties"

    /// <summary>
    /// Gets or sets the number which indicates how many files should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), gallery.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            gallery.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), gallery.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            gallery.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), gallery.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            gallery.ZeroRowsText = value;
        }
    }

    #endregion


    #region "UniPager properties"

    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public bool HidePagerForSinglePage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HidePagerForSinglePage"), gallery.HidePagerForSinglePage);
        }
        set
        {
            SetValue("HidePagerForSinglePage", value);
            gallery.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of records to display on a page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), gallery.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            gallery.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupSize"), gallery.GroupSize);
        }
        set
        {
            SetValue("GroupSize", value);
            gallery.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the querysting parameter.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("QueryStringKey"), gallery.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            gallery.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFirstLastAutomatically"), gallery.DisplayFirstLastAutomatically);
        }
        set
        {
            SetValue("DisplayFirstLastAutomatically", value);
            gallery.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPreviousNextAutomatically"), gallery.DisplayPreviousNextAutomatically);
        }
        set
        {
            SetValue("DisplayPreviousNextAutomatically", value);
            gallery.DisplayPreviousNextAutomatically = value;
        }
    }

    #endregion


    #region "UniPager Template properties"

    /// <summary>
    /// Gets or sets the pages template.
    /// </summary>
    public string PagesTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Pages"), string.Empty);
        }
        set
        {
            SetValue("Pages", value);
        }
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CurrentPage"), string.Empty);
        }
        set
        {
            SetValue("CurrentPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageSeparator"), string.Empty);
        }
        set
        {
            SetValue("PageSeparator", value);
            gallery.SeparatorTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstPage"), string.Empty);
        }
        set
        {
            SetValue("FirstPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LastPage"), string.Empty);
        }
        set
        {
            SetValue("LastPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousPage"), string.Empty);
        }
        set
        {
            SetValue("PreviousPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextPage"), string.Empty);
        }
        set
        {
            SetValue("NextPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousGroup"), string.Empty);
        }
        set
        {
            SetValue("PreviousGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextGroup"), string.Empty);
        }
        set
        {
            SetValue("NextGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerLayout"), string.Empty);
        }
        set
        {
            SetValue("PagerLayout", value);
            gallery.LayoutTemplate = value;
        }
    }

    #endregion


    #region "Library properties"

    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), gallery.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            gallery.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying selected file.
    /// </summary>
    public string SelectedItemTransformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemTransformation"), gallery.SelectedItemTransformation);
        }
        set
        {
            SetValue("SelectedItemTransformation", value);
            gallery.SelectedItemTransformation = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying file list header.
    /// </summary>
    public string HeaderTransformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderTransformation"), gallery.HeaderTransformation);
        }
        set
        {
            SetValue("HeaderTransformation", value);
            gallery.HeaderTransformation = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying file list footer.
    /// </summary>
    public string FooterTransformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FooterTransformation"), gallery.FooterTransformation);
        }
        set
        {
            SetValue("FooterTransformation", value);
            gallery.FooterTransformation = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for item separator.
    /// </summary>
    public string SeparatorTransformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SeparatorTransformation"), gallery.SeparatorTransformation);
        }
        set
        {
            SetValue("SeparatorTransformation", value);
            gallery.SeparatorTransformation = value;
        }
    }


    /// <summary>
    /// Gets or sets media library path to display files from.
    /// </summary>
    public string MediaLibraryPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MediaLibraryPath"), gallery.MediaLibraryPath);
        }
        set
        {
            SetValue("MediaLibraryPath", value);
            gallery.MediaLibraryPath = value;
        }
    }


    /// <summary>
    /// Gets or sets media library name.
    /// </summary>
    public string MediaLibraryName
    {
        get
        {
            string libraryName = ValidationHelper.GetString(GetValue("MediaLibraryName"), gallery.MediaLibraryName);
            if ((string.IsNullOrEmpty(libraryName) || libraryName == MediaLibraryInfoProvider.CURRENT_LIBRARY) && (MediaLibraryContext.CurrentMediaLibrary != null))
            {
                return MediaLibraryContext.CurrentMediaLibrary.LibraryName;
            }
            return libraryName;
        }
        set
        {
            SetValue("MediaLibraryName", value);
            gallery.MediaLibraryName = value;
        }
    }


    /// <summary>
    /// Hide folder tree.
    /// </summary>
    public bool HideFolderTree
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideFolderTree"), gallery.HideFolderTree);
        }
        set
        {
            SetValue("HideFolderTree", value);
            gallery.HideFolderTree = value;
        }
    }


    /// <summary>
    /// Indicates whether the links to media file should be processed in a secure way.
    /// </summary>
    public bool UseSecureLinks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseSecureLinks"), true);
        }
        set
        {
            SetValue("UseSecureLinks", value);
            gallery.UseSecureLinks = value;
        }
    }


    /// <summary>
    /// Gets or sets media library name.
    /// </summary>
    public int FilterMethod
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("FilterMethod"), gallery.FilterMethod);
        }
        set
        {
            SetValue("FilterMethod", value);
            gallery.FilterMethod = value;
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
            gallery.FileIDQueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the sort querysting parameter.
    /// </summary>
    public string SortQueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SortQueryStringKey"), String.Empty);
        }
        set
        {
            SetValue("SortQueryStringKey", value);
            gallery.SortQueryStringKey = value;
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
            gallery.PathQueryStringKey = value;
        }
    }


    /// <summary>
    /// Preview preffix for identification preview file.
    /// </summary>
    public string PreviewSuffix
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviewSuffix"), String.Empty);
        }
        set
        {
            SetValue("PreviewSuffix", value);
            gallery.PreviewSuffix = value;
        }
    }


    /// <summary>
    /// Icon set name.
    /// </summary>
    public string IconSet
    {
        get
        {
            return ValidationHelper.GetString(GetValue("IconSet"), String.Empty);
        }
        set
        {
            SetValue("IconSet", value);
            gallery.IconSet = value;
        }
    }


    /// <summary>
    /// Indicates if active content (video etc.) should be displayed.
    /// </summary>
    public bool DisplayActiveContent
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayActiveContent"), gallery.DisplayActiveContent);
        }
        set
        {
            SetValue("DisplayActiveContent", value);
            gallery.DisplayActiveContent = value;
        }
    }


    /// <summary>
    /// Indicates if subfolders content should be displayed.
    /// </summary>
    public bool ShowSubfoldersContent
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSubfoldersContent"), gallery.ShowSubfoldersContent);
        }
        set
        {
            SetValue("ShowSubfoldersContent", value);
            gallery.ShowSubfoldersContent = value;
        }
    }


    /// <summary>
    /// Indicates if files count should be displayed.
    /// </summary>
    public bool DisplayFileCount
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFileCount"), gallery.DisplayFileCount);
        }
        set
        {
            SetValue("DisplayFileCount", value);
            gallery.DisplayFileCount = value;
        }
    }


    /// <summary>
    /// Indicates if file upload form should be displayed.
    /// </summary>
    public bool AllowUpload
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowUpload"), gallery.AllowUpload);
        }
        set
        {
            SetValue("AllowUpload", value);
            gallery.AllowUpload = value;
        }
    }


    /// <summary>
    /// Indicates if preview file upload should be displayed in upload form.
    /// </summary>
    public bool AllowUploadPreview
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowUploadPreview"), gallery.AllowUploadPreview);
        }
        set
        {
            SetValue("AllowUploadPreview", value);
            gallery.AllowUploadPreview = value;
        }
    }

    #endregion


    #region "System settings"

    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            gallery.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, gallery.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            gallery.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            gallery.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets Check permissions property.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), true);
        }
        set
        {
            SetValue("CheckPermissions", value);
            gallery.CheckLibraryPermissions = value;
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
        gallery.OnNotAllowed += gallery_OnNotAllowed;

        // Library properties
        gallery.MediaLibraryName = MediaLibraryName;
        gallery.MediaLibraryPath = MediaLibraryPath;
        gallery.TransformationName = TransformationName;
        gallery.SelectedItemTransformation = SelectedItemTransformation;
        gallery.HeaderTransformation = HeaderTransformation;
        gallery.FooterTransformation = FooterTransformation;
        gallery.SeparatorTransformation = SeparatorTransformation;
        gallery.HideFolderTree = HideFolderTree;
        gallery.PreviewSuffix = PreviewSuffix;
        gallery.IconSet = IconSet;
        gallery.DisplayActiveContent = DisplayActiveContent;
        gallery.DisplayFileCount = DisplayFileCount;
        gallery.ShowSubfoldersContent = ShowSubfoldersContent;
        gallery.ZeroRowsText = ZeroRowsText;
        gallery.HideControlForZeroRows = HideControlForZeroRows;
        gallery.UseSecureLinks = UseSecureLinks;

        // Cache settings
        gallery.CacheItemName = CacheItemName;
        gallery.CacheDependencies = CacheDependencies;
        gallery.CacheMinutes = CacheMinutes;

        gallery.CheckLibraryPermissions = CheckPermissions;

        // Filters properties
        gallery.FilterMethod = FilterMethod;
        gallery.FileIDQueryStringKey = FileIDQueryStringKey;
        gallery.PathQueryStringKey = PathQueryStringKey;
        gallery.SortQueryStringKey = SortQueryStringKey;

        // Content properties
        gallery.SelectTopN = SelectTopN;

        // Uploader properties
        gallery.AllowUpload = AllowUpload;
        gallery.AllowUploadPreview = AllowUploadPreview;

        // UniPager properties
        gallery.PageSize = PageSize;
        gallery.GroupSize = GroupSize;
        gallery.QueryStringKey = QueryStringKey;
        gallery.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
        gallery.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
        gallery.HidePagerForSinglePage = HidePagerForSinglePage;

        switch (FilterMethod)
        {
            case 1:
                gallery.PagerMode = UniPagerMode.PostBack;
                break;

            default:
                gallery.PagerMode = UniPagerMode.Querystring;
                break;
        }

        // UniPager template properties
        gallery.PagesTemplate = PagesTemplate;
        gallery.CurrentPageTemplate = CurrentPageTemplate;
        gallery.SeparatorTemplate = SeparatorTemplate;
        gallery.FirstPageTemplate = FirstPageTemplate;
        gallery.LastPageTemplate = LastPageTemplate;
        gallery.PreviousPageTemplate = PreviousPageTemplate;
        gallery.NextPageTemplate = NextPageTemplate;
        gallery.PreviousGroupTemplate = PreviousGroupTemplate;
        gallery.NextGroupTemplate = NextGroupTemplate;
        gallery.LayoutTemplate = LayoutTemplate;
    }


    private void gallery_OnNotAllowed(string permissionType, CMSAdminControl sender)
    {
        if (sender != null)
        {
            sender.StopProcessing = true;
        }

        gallery.StopProcessing = true;
        gallery.Visible = false;
        gallery.UniPager.PageControl = null;
        messageElem.ErrorMessage = MediaLibraryHelper.GetAccessDeniedMessage(permissionType);
        messageElem.DisplayMessage = true;
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        gallery.ReloadData();
    }
}