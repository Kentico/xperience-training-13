using System;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.MediaLibrary.Web.UI;

public partial class CMSWebParts_MediaLibrary_MediaGalleryFileList : CMSAbstractWebPart
{
    #region "Variables"

    // Indicates whether control was binded
    private bool binded;
    // Datasource instance
    private CMSBaseDataSource mDataSourceControl;

    private bool? mDisplayDetail;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the name of the media library name.
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
    /// Returns true if file ID query string is pressent.
    /// </summary>
    public bool DisplayDetail
    {
        get
        {
            if (mDisplayDetail == null)
            {
                mDisplayDetail = QueryHelper.GetInteger(FileIDQueryStringKey, 0) > 0;
            }

            return mDisplayDetail.Value;
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
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), String.Empty);
        }
        set
        {
            SetValue("TransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying selected file.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), String.Empty);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets FooterTemplate property.
    /// </summary>
    public string FooterTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FooterTransformationName"), String.Empty);
        }
        set
        {
            SetValue("FooterTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets HeaderTemplate property.
    /// </summary>
    public string HeaderTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderTransformationName"), String.Empty);
        }
        set
        {
            SetValue("HeaderTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets SeparatorTemplate property.
    /// </summary>
    public string SeparatorTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SeparatorTransformationName"), String.Empty);
        }
        set
        {
            SetValue("SeparatorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), true);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), String.Empty);
        }
        set
        {
            SetValue("ZeroRowsText", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying selected file.
    /// </summary>
    public string DataSourceName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataSourceName"), String.Empty);
        }
        set
        {
            SetValue("DataSourceName", value);
        }
    }


    /// <summary>
    /// Control with data source.
    /// </summary>
    public CMSBaseDataSource DataSourceControl
    {
        get
        {
            // Check if control is empty and load it with the data
            if (mDataSourceControl == null)
            {
                if (!String.IsNullOrEmpty(DataSourceName))
                {
                    mDataSourceControl = CMSControlsHelper.GetFilter(DataSourceName) as CMSBaseDataSource;
                }
            }

            return mDataSourceControl;
        }
        set
        {
            mDataSourceControl = value;
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
        }
    }


    /// <summary>
    /// Indicates if active content (video etc.) should be displayed.
    /// </summary>
    public bool DisplayActiveContent
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayActiveContent"), false);
        }
        set
        {
            SetValue("DisplayActiveContent", value);
        }
    }

    #endregion


    /// <summary>
    /// OnContentLoaded override.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        SetupControlLoad();
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if ((DataSourceControl != null) && (!DataHelper.DataSourceIsEmpty(DataSourceControl.DataSource)) && (!binded))
        {
            repItems.DataSource = DataSourceControl.DataSource;
            repItems.DataBind();
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControlLoad()
    {
        // Handle filter change event
        if (DataSourceControl != null)
        {
            DataSourceControl.OnFilterChanged += DataSourceControl_OnFilterChanged;
        }

        repItems.ItemDataBound += repItems_ItemDataBound;
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(MediaLibraryName, SiteContext.CurrentSiteName);
            if (mli != null)
            {
                // If don't have 'Manage' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "read"))
                {
                    // Check 'File create' permission
                    if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "libraryaccess"))
                    {
                        repItems.Visible = false;

                        messageElem.ErrorMessage = MediaLibraryHelper.GetAccessDeniedMessage("libraryaccess");
                        messageElem.DisplayMessage = true;
                        return;
                    }
                }
            }

            int fid = QueryHelper.GetInteger(FileIDQueryStringKey, 0);
            if (fid > 0)
            {
                if (!String.IsNullOrEmpty(SelectedItemTransformationName))
                {
                    repItems.ItemTemplate = TransformationHelper.LoadTransformation(this, SelectedItemTransformationName);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(TransformationName))
                {
                    repItems.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
                }
            }
            if (!String.IsNullOrEmpty(FooterTransformationName))
            {
                repItems.FooterTemplate = TransformationHelper.LoadTransformation(this, FooterTransformationName);
            }
            if (!String.IsNullOrEmpty(HeaderTransformationName))
            {
                repItems.HeaderTemplate = TransformationHelper.LoadTransformation(this, HeaderTransformationName);
            }
            if (!String.IsNullOrEmpty(SeparatorTransformationName))
            {
                repItems.SeparatorTemplate = TransformationHelper.LoadTransformation(this, SeparatorTransformationName);
            }

            repItems.DataBindByDefault = false;
            repItems.OnPageChanged += repItems_OnPageChanged;

            // Add repeater to the filter collection
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), repItems);

            repItems.HideControlForZeroRows = HideControlForZeroRows;
            repItems.ZeroRowsText = ZeroRowsText;
        }
    }


    /// <summary>
    /// OnFilterChanged event handler.
    /// </summary>
    private void DataSourceControl_OnFilterChanged()
    {
        // Reload data
        if (DataSourceControl != null)
        {
            repItems.DataSource = DataSourceControl.DataSource;
            repItems.DataBind();
            binded = true;
        }
    }


    private void repItems_OnPageChanged(object sender, EventArgs e)
    {
        // Reload data
        if (DataSourceControl != null)
        {
            repItems.DataSource = DataSourceControl.DataSource;
            repItems.DataBind();
            binded = true;
        }
    }


    private void repItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
        ctrlFilePreview.DisplayActiveContent = DisplayDetail || DisplayActiveContent;
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        SetupControlLoad();
        repItems.ReloadData(true);
    }
}