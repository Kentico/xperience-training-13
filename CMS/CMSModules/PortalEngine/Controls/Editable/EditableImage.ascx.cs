using System;
using System.Collections;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.PortalEngine;

using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_Editable_EditableImage : CMSUserControl
{
    #region "Variables"

    protected const int NOT_KOWN = -1;

    protected XmlData mImageAutoResize = null;
    protected int mResizeToWidth = 0;
    protected int mResizeToHeight = 0;
    protected int mResizeToMaxSideSize = 0;
    protected bool mDimensionsLoaded = false;

    protected ISimpleDataContainer mIDataControl = null;
    protected IPageManager mIPageManager = null;

    protected ViewModeEnum? mViewMode = null;

    protected PageInfo mCurrentPageInfo = null;

    private string mEditPageUrl = "~/CMSModules/PortalEngine/UI/OnSiteEdit/EditImage.aspx";
    private string mEditDialogWidth = "80%";

    #endregion


    #region "Controls"

    /// <summary>
    /// Image.
    /// </summary>
    protected Image imgImage = null;

    /// <summary>
    /// Region title.
    /// </summary>
    protected Label lblTitle = null;

    /// <summary>
    /// Error label.
    /// </summary>
    protected Label lblError = null;

    /// <summary>
    /// Region panel.
    /// </summary>
    protected Panel pnlEditor = null;

    /// <summary>
    /// Image title.
    /// </summary>
    protected string mImageTitle = null;

    #endregion


    #region "Public properties & events"

    /// <summary>
    /// Fires when web part property value is requested.
    /// </summary>
    /// <param name="value">Web part property value to be processed</param>
    /// <returns>Returns processed web part property value.</returns>
    public delegate object OnGetValueEventHandler(object value);


    /// <summary>
    /// Occurs when GetValue method of the DataControl object is called.
    /// </summary>
    public event OnGetValueEventHandler OnGetValue;


    /// <summary>
    /// ID of the control content
    /// </summary>
    public string ContentID
    {
        get;
        set;
    }
    
    
    /// <summary>
    /// Gets the url of the page which ensures editing of the web part's editable content in the On-Site editing mode.
    /// </summary>
    public string EditPageUrl
    {
        get
        {
            return UrlResolver.ResolveUrl(mEditPageUrl);
        }
    }


    /// <summary>
    /// Gets the width of the edit dialog in the On-Site editing mode.
    /// </summary>
    public string EditDialogWidth
    {
        get
        {
            return mEditDialogWidth;
        }
    }


    /// <summary>
    /// If set, only published content is displayed on a live site.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), false);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
        }
    }


    /// <summary>
    /// Page mode of the current web part.
    /// </summary>
    public ViewModeEnum ViewMode
    {
        get
        {
            if (mViewMode == null)
            {
                if (PageManager != null)
                {
                    return PageManager.ViewMode;
                }

                return PortalContext.ViewMode;
            }
            return mViewMode.Value;
        }
        set
        {
            mViewMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the title of the image region which is displayed in the EDIT mode.
    /// </summary>
    public string ImageTitle
    {
        get
        {
            string title = ValidationHelper.GetString(GetValue("ImageTitle"), String.Empty);
            if (String.IsNullOrEmpty(title))
            {
                title = ValidationHelper.GetString(GetValue("WebPartTitle"), String.Empty);
                if (String.IsNullOrEmpty(title))
                {
                    title = ValidationHelper.GetString(GetValue("ControlID"), String.Empty);
                }
            }

            return title;
        }
        set
        {
            SetValue("ImageTitle", value);
        }
    }


    /// <summary>
    /// Gets or sets the image width.
    /// </summary>
    public int ImageWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ImageWidth"), 0);
        }
        set
        {
            SetValue("ImageWidth", value);
        }
    }


    /// <summary>
    /// Gets or sets the image height.
    /// </summary>
    public int ImageHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ImageHeight"), 0);
        }
        set
        {
            SetValue("ImageHeight", value);
        }
    }


    /// <summary>
    /// Gets or sets the alternative image text (ALT tag of the image).
    /// </summary>
    public string AlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternateText"), "");
        }
        set
        {
            SetValue("AlternateText", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the path to file is displayed in EDIT mode.
    /// </summary>
    public bool DisplaySelectorTextBox
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplaySelectorTextBox"), false);
        }
        set
        {
            SetValue("DisplaySelectorTextBox", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the css class applied to the image.
    /// </summary>
    public string ImageCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ImageCssClass"), "");
        }
        set
        {
            SetValue("ImageCssClass", value);
        }
    }


    /// <summary>
    /// Gets or sets the style tag of the image.
    /// </summary>
    public string ImageStyle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ImageStyle"), "");
        }
        set
        {
            SetValue("ImageStyle", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    /// <summary>
    /// Width the image should be automatically resized to after it is uploaded.
    /// </summary>
    public int ResizeToWidth
    {
        get
        {
            if (!mDimensionsLoaded)
            {
                // Use image auto resize settings
                Hashtable settings = ImageAutoResize.ConvertToHashtable();
                ImageHelper.GetAutoResizeDimensions(settings, SiteContext.CurrentSiteName, out mResizeToWidth, out mResizeToHeight, out mResizeToMaxSideSize);
                mDimensionsLoaded = true;
            }
            return mResizeToWidth;
        }
        set
        {
            mResizeToWidth = value;
            mDimensionsLoaded = true;
        }
    }


    /// <summary>
    /// Height the image should be automatically resized to after it is uploaded.
    /// </summary>
    public int ResizeToHeight
    {
        get
        {
            if (!mDimensionsLoaded)
            {
                // Use image auto resize settings
                Hashtable settings = ImageAutoResize.ConvertToHashtable();
                ImageHelper.GetAutoResizeDimensions(settings, SiteContext.CurrentSiteName, out mResizeToWidth, out mResizeToHeight, out mResizeToMaxSideSize);
                mDimensionsLoaded = true;
            }
            return mResizeToHeight;
        }
        set
        {
            mResizeToHeight = value;
            mDimensionsLoaded = true;
        }
    }


    /// <summary>
    /// Max side size the image should be automatically resized to after it is uploaded.
    /// </summary>
    public int ResizeToMaxSideSize
    {
        get
        {
            if (!mDimensionsLoaded)
            {
                // Use image auto resize settings
                Hashtable settings = ImageAutoResize.ConvertToHashtable();
                ImageHelper.GetAutoResizeDimensions(settings, SiteContext.CurrentSiteName, out mResizeToWidth, out mResizeToHeight, out mResizeToMaxSideSize);
                mDimensionsLoaded = true;
            }
            return mResizeToMaxSideSize;
        }
        set
        {
            mResizeToMaxSideSize = value;
            mDimensionsLoaded = true;
        }
    }


    /// <summary>
    /// Default image displayed when no image is selected.
    /// </summary>
    public string DefaultImage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultImage"), "");
        }
        set
        {
            SetValue("DefaultImage", value);
        }
    }



    /// <summary>
    /// Gets or sets the IDataControl to get/set values
    /// </summary>
    public ISimpleDataContainer DataControl
    {
        get
        {
            if (mIDataControl == null)
            {
                mIDataControl = ControlsHelper.GetParentControl(this, typeof(ISimpleDataContainer)) as ISimpleDataContainer;
                if (mIDataControl == null)
                {
                    // ASPX mode - editable text in a dialog (On-site editing)
                    mIDataControl = this as ISimpleDataContainer;
                }
            }
            return mIDataControl;
        }
        set
        {
            mIDataControl = value;
        }
    }


    /// <summary>
    /// Parent page manager.
    /// </summary>
    public IPageManager PageManager
    {
        get
        {
            return mIPageManager;
        }
        set
        {
            mIPageManager = value;
        }
    }


    /// <summary>
    /// Current page info
    /// </summary>
    public PageInfo CurrentPageInfo
    {
        get
        {
            if ((mCurrentPageInfo == null) && (PagePlaceholder != null))
            {
                mCurrentPageInfo = PagePlaceholder.PageInfo;
            }

            return mCurrentPageInfo;
        }
        set
        {
            mCurrentPageInfo = value;
        }
    }


    /// <summary>
    /// Page place holder
    /// </summary>
    public CMSPagePlaceholder PagePlaceholder
    {
        get;
        set;
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Auto resize configuration for the image.
    /// </summary>
    private XmlData ImageAutoResize
    {
        get
        {
            if (mImageAutoResize == null)
            {
                mImageAutoResize = new XmlData("AutoResize");
                mImageAutoResize.LoadData(ValidationHelper.GetString(GetValue("ImageAutoResize"), ""));
            }
            return mImageAutoResize;
        }
    }

    #endregion



    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    public void SetupControl()
    {
        // Do not hide for roles in edit or preview mode
        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
            case ViewModeEnum.EditLive:
            case ViewModeEnum.EditDisabled:
            case ViewModeEnum.Design:
            case ViewModeEnum.DesignDisabled:
            case ViewModeEnum.EditNotCurrent:
            case ViewModeEnum.Preview:
                SetValue("DisplayToRoles", String.Empty);
                break;
        }
    }


    /// <summary>
    /// Overridden CreateChildControls method.
    /// </summary>
    protected override void CreateChildControls()
    {
        Controls.Clear();
        base.CreateChildControls();

        if (!StopProcessing)
        {
            if (!CMSAbstractEditableControl.RequestEditViewMode(ViewMode, ContentID))
            {
                ViewMode = ViewModeEnum.Preview;
            }
                        
            // Create controls by actual page mode
            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    {
                        // Main editor panel
                        pnlEditor = new Panel();
                        pnlEditor.ID = "pnlEditor";
                        pnlEditor.CssClass = "EditableImageEdit EditableImage_" + ContentID;
                        pnlEditor.Attributes.Add("data-tracksavechanges", "true");
                        if (ImageWidth > 0)
                        {
                            pnlEditor.Style.Add(HtmlTextWriterStyle.Width, ImageWidth.ToString() + "px;");
                        }
                        Controls.Add(pnlEditor);

                        // Title label
                        lblTitle = new Label();
                        lblTitle.EnableViewState = false;
                        lblTitle.CssClass = "EditableTextTitle";
                        pnlEditor.Controls.Add(lblTitle);

                        // Error label
                        lblError = new Label();
                        lblError.EnableViewState = false;
                        lblError.CssClass = "EditableTextError";
                        pnlEditor.Controls.Add(lblError);
                    }
                    break;

                default:
                    {
                        // Display content in non editing modes
                        imgImage = new Image();
                        imgImage.ID = "imgImage";
                        imgImage.GenerateEmptyAlternateText = true;
                        if (ImageCssClass != "")
                        {
                            imgImage.CssClass = ImageCssClass;
                        }
                        if (ImageStyle != "")
                        {
                            imgImage.Attributes.Add("style", ImageStyle);
                        }

                        imgImage.AlternateText = AlternateText;
                        imgImage.ToolTip = AlternateText;
                        imgImage.EnableViewState = false;
                        Controls.Add(imgImage);
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Loads the control content.
    /// </summary>
    /// <param name="content">Content to load</param>
    /// <param name="forceReload">If true, the content is forced to reload</param>
    public void LoadContent(string content, bool forceReload)
    {
        if (!StopProcessing)
        {
            // Load the properties
            EnsureChildControls();

            string path = null;
            string altText = null;
            // Load the image data
            if (!string.IsNullOrEmpty(content))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(content);

                XmlNodeList properties = xml.SelectNodes("image/property");
                if (properties != null)
                {
                    foreach (XmlNode node in properties)
                    {
                        if (node.Attributes["name"] != null)
                        {
                            switch (node.Attributes["name"].Value.ToLowerCSafe())
                            {
                                case "imagepath":
                                    path = ResolveUrl(node.InnerText.Trim());
                                    break;

                                case "alttext":
                                    altText = node.InnerText.Trim();
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                // Ensure correct url from media selector
                path = Server.HtmlDecode(DefaultImage);
            }

            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    // Set image title
                    if (ImageTitle != "")
                    {
                        lblTitle.Text = HTMLHelper.HTMLEncode(ImageTitle);
                    }
                    else
                    {
                        lblTitle.Visible = false;
                    }

                    break;

                default:
                    Visible = true;

                    if (string.IsNullOrEmpty(path))
                    {
                        Visible = false;
                        return;
                    }

                    // Force image width
                    if (ImageWidth > 0)
                    {
                        imgImage.Width = ImageWidth;
                        path = URLHelper.AddParameterToUrl(path, "width", ImageWidth.ToString());
                    }

                    // Force image height
                    if (ImageHeight > 0)
                    {
                        imgImage.Height = ImageHeight;
                        path = URLHelper.AddParameterToUrl(path, "height", ImageHeight.ToString());
                    }

                    // Use specific alternate text or default alternate text
                    imgImage.AlternateText = String.IsNullOrEmpty(altText) ? AlternateText : altText;

                    // Check authorization
                    bool isAuthorized = true;
                    if (CheckPermissions)
                    {
                        isAuthorized = PageManager.IsAuthorized;
                    }

                    // Only published
                    if ((PortalContext.ViewMode != ViewModeEnum.LiveSite) || !SelectOnlyPublished || ((CurrentPageInfo != null) && CurrentPageInfo.IsPublished))
                    {
                        if (isAuthorized)
                        {
                            imgImage.ImageUrl = path;
                        }
                        else
                        {
                            imgImage.Visible = false;
                        }
                    }
                    else
                    {
                        imgImage.Visible = false;
                    }

                    break;
            }
        }
    }


    /// <summary>
    /// Gets the current control content.
    /// </summary>
    public string GetContent()
    {
        if (!StopProcessing)
        {
            EnsureChildControls();
        }

        return null;
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    private void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    if (lblError != null)
                    {
                        lblError.Visible = (lblError.Text != "");
                    }

                    if (lblTitle != null)
                    {
                        lblTitle.Text = HTMLHelper.HTMLEncode(ImageTitle);
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Returns the value of the given web part property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if ((DataControl != null) && (DataControl != this))
        {
            var value = DataControl.GetValue(propertyName);

            if (OnGetValue != null)
            {
                value = OnGetValue(value);
            }

            return value;
        }

        return base.GetValue(propertyName);
    }


    /// <summary>
    /// Sets the property value of the control, setting the value affects only local property value.
    /// </summary>
    /// <param name="propertyName">Property name to set</param>
    /// <param name="value">New property value</param>
    public override bool SetValue(string propertyName, object value)
    {
        if ((DataControl != null) && (DataControl != this))
        {
            DataControl.SetValue(propertyName, value);
        }

        return base.SetValue(propertyName, value);
    }

}
