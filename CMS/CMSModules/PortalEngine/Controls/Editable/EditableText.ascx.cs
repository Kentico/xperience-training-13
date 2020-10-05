using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CKEditor.Web.UI;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

using CultureInfo = System.Globalization.CultureInfo;


public partial class CMSModules_PortalEngine_Controls_Editable_EditableText : CMSUserControl
{
    #region "Variables"

    protected string mHtmlAreaToolbar = String.Empty;
    protected string mHtmlAreaToolbarLocation = String.Empty;

    protected XmlData mImageAutoResize;
    protected int mResizeToWidth;
    protected int mResizeToHeight;
    protected int mResizeToMaxSideSize;
    protected bool mDimensionsLoaded;

    protected ISimpleDataContainer mIDataControl;
    protected IPageManager mIPageManager;

    protected CMSTextBox txtValue;

    protected Label lblTitle;
    protected Panel pnlEditor;
    protected Label lblError;

    protected bool mShowToolbar;

    protected Literal ltlContent;

    protected ViewModeEnum? mViewMode;

    protected PageInfo mCurrentPageInfo;
    private const string EDIT_PAGE_URL = "~/CMSModules/PortalEngine/UI/OnSiteEdit/EditText.aspx";
    private MacroResolver mMacroResolver;

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
    /// Gets the current context resolver
    /// </summary>
    public MacroResolver ContextResolver
    {
        get
        {
            if (mMacroResolver == null)
            {
                mMacroResolver = MacroContext.CurrentResolver.CreateChild();
                mMacroResolver.Culture = Thread.CurrentThread.CurrentCulture.ToString();
            }
            return mMacroResolver;
        }
    }


    /// <summary>
    /// Get or sets editor's title
    /// </summary>
    public string Title
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether CKEditor is used in inline mode.
    /// </summary>
    public bool UseInlineMode
    {
        get;
        set;
    }


    /// <summary>
    /// Editor instance
    /// </summary>
    public CMSHtmlEditor Editor
    {
        get;
        private set;
    }


    /// <summary>
    /// ID of the control content
    /// </summary>
    public string ContentID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether text is edited in dialog on-site mode
    /// </summary>
    public bool IsDialogEdit
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
            return UrlResolver.ResolveUrl(EDIT_PAGE_URL);
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
    /// Page place holder
    /// </summary>
    public CMSPagePlaceholder PagePlaceholder
    {
        get;
        set;
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
    /// Gets or sets the design panel
    /// </summary>
    public Panel DesignPanel
    {
        get;
        set;
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
                    mIDataControl = this;
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
    /// Gets or sets the error message when the input string does not comply with the input conditions
    /// </summary>
    public string ErrorMessage
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the type of server control which is displayed in the editable region.
    /// </summary>
    [Category("Appearance")]
    [Description("Gets or sets the type of server control which is displayed in the editable region.")]
    [DefaultValue(CMSEditableRegionTypeEnum.TextBox)]
    public virtual CMSEditableRegionTypeEnum RegionType
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the control title which is displayed in the editable mode.
    /// </summary>
    [Category("Appearance")]
    [Description("Gets or sets the control title which is displayed in the editable mode.")]
    public string RegionTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RegionTitle"), String.Empty);
        }
        set
        {
            SetValue("RegionTitle", value);
        }
    }


    /// <summary>
    /// Gets or sets the maximum length of the content.
    /// </summary>
    [Category("Behavior")]
    [Description("Gets or sets the maximum length of the content.")]
    public int MaxLength
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxLength"), 0);
        }
        set
        {
            SetValue("MaxLength", value);
        }
    }


    /// <summary>
    /// Encodes text inserted via text box or text area. Default value is false.
    /// </summary>
    [Category("Behavior")]
    [Description("Encodes text inserted via text box or text area. Default value is false.")]
    public bool EncodeText
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EncodeText"), false);
        }
        set
        {
            SetValue("EncodeText", value);
        }
    }


    /// <summary>
    /// Gets or sets the minimum length of the content.
    /// </summary>
    [Category("Behavior")]
    [Description("Gets or sets the minimum length of the content.")]
    public int MinLength
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MinLength"), 0);
        }
        set
        {
            SetValue("MinLength", value);
        }
    }


    /// <summary>
    /// Gets or sets the height of the control.
    /// </summary>
    [Category("Appearance")]
    [Description("Gets or sets the height of the control.")]
    public int DialogHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DialogHeight"), 0);
        }
        set
        {
            SetValue("DialogHeight", value);
        }
    }


    /// <summary>
    /// Gets or sets the width of the control.
    /// </summary>
    [Category("Appearance")]
    [Description("Gets or sets the width of the control.")]
    public int DialogWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DialogWidth"), 0);
        }
        set
        {
            SetValue("DialogWidth", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to wrap the text if using text area field.
    /// </summary>
    [Category("Appearance")]
    [Description("Gets or sets the value that indicates whether to wrap the text if using text area field.")]
    [DefaultValue(true)]
    public bool WordWrap
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("WordWrap"), true);
        }
        set
        {
            SetValue("WordWrap", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the HTML editor toolbar.
    /// </summary>
    public string HtmlAreaToolbar
    {
        get
        {
            return mHtmlAreaToolbar;
        }
        set
        {
            mHtmlAreaToolbar = value ?? "";
        }
    }


    /// <summary>
    /// Gets or sets the location of the HTML editor toolbar.
    /// </summary>
    public string HtmlAreaToolbarLocation
    {
        get
        {
            return mHtmlAreaToolbarLocation;
        }
        set
        {
            mHtmlAreaToolbarLocation = value ?? "";
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
    /// Enables or disables resolving of inline controls.
    /// </summary>
    public bool ResolveDynamicControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResolveDynamicControls"), true);
        }
        set
        {
            SetValue("ResolveDynamicControls", value);
        }
    }


    /// <summary>
    /// Default text displayed if no content filled.
    /// </summary>
    public string DefaultText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultText"), "");
        }
        set
        {
            SetValue("DefaultText", value);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Auto resize configuration.
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


    #region "Page Methods"

    /// <summary>
    /// PreRender event handler.
    /// </summary>
    private void Page_PreRender(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
            case ViewModeEnum.EditDisabled:
                // Set enabled
                if (Editor != null)
                {
                    Editor.Enabled = IsEnabled(ViewMode);
                }
                if (txtValue != null)
                {
                    txtValue.Enabled = IsEnabled(ViewMode);
                }

                if (mShowToolbar && IsEnabled(ViewMode))
                {
                    ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.TOOLBAR_SCRIPT_KEY, ScriptHelper.ToolbarScript);
                }

                if (lblError != null)
                {
                    lblError.Visible = (lblError.Text != "");
                }

                if (lblTitle != null)
                {
                    lblTitle.Text = RegionTitle;
                    lblTitle.Visible = (lblTitle.Text != "");
                }

                // Allow to select text in the source editor area
                if (DesignPanel != null)
                {
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "onselectstart", "document.getElementById('" + DesignPanel.ClientID + "').parentNode.onselectstart = function() { return true; };", true);
                }

                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads the control content.
    /// </summary>
    /// <param name="content">Content to load</param>
    /// <param name="forceReload">If true, the content is forced to reload</param>
    public void LoadContent(string content, bool forceReload)
    {
        if (StopProcessing)
        {
            return;
        }

        ApplySettings();

        bool contentIsEmpty = String.IsNullOrEmpty(content);
        content = contentIsEmpty ? DefaultText : content;

        // Resolve URLs
        content = HTMLHelper.ResolveUrls(content, null);

        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
            case ViewModeEnum.EditDisabled:

                switch (RegionType)
                {
                    case CMSEditableRegionTypeEnum.HtmlEditor:
                        // HTML editor
                        if ((Editor != null) && (forceReload || !RequestHelper.IsPostBack() || (ViewMode == ViewModeEnum.EditDisabled) || contentIsEmpty))
                        {
                            Editor.ResolvedValue = content;
                        }
                        break;

                    case CMSEditableRegionTypeEnum.TextArea:
                    case CMSEditableRegionTypeEnum.TextBox:

                        content = (EncodeText) ? HTMLHelper.HTMLDecode(content) : content;

                        // TextBox
                        if ((forceReload || !RequestHelper.IsPostBack() || contentIsEmpty) && (txtValue != null))
                        {
                            txtValue.Text = content;
                        }
                        break;
                }
                break;


            default:
                // Check authorization
                bool isAuthorized = true;
                if ((PageManager != null) && (CheckPermissions))
                {
                    isAuthorized = PageManager.IsAuthorized;
                }

                // Only published
                if ((PortalContext.ViewMode != ViewModeEnum.LiveSite) || !SelectOnlyPublished || ((CurrentPageInfo != null) && CurrentPageInfo.IsPublished))
                {
                    if (isAuthorized)
                    {
                        if (ltlContent == null)
                        {
                            ltlContent = (Literal)FindControl("ltlContent");
                        }
                        if (ltlContent != null)
                        {
                            ltlContent.Text = content;

                            // Resolve inline controls
                            if (ResolveDynamicControls)
                            {
                                ControlsHelper.ResolveDynamicControls(this);
                            }
                        }
                    }
                }
                break;
        }
    }


    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public bool IsValid()
    {
        bool isValid = true;

        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
            case ViewModeEnum.EditDisabled:
                switch (RegionType)
                {
                    case CMSEditableRegionTypeEnum.HtmlEditor:
                        if (Editor != null)
                        {
                            isValid = CheckIfLengthIsValid(Editor.ResolvedValue);
                        }
                        break;

                    case CMSEditableRegionTypeEnum.TextArea:
                    case CMSEditableRegionTypeEnum.TextBox:

                        if (txtValue != null)
                        {
                            var content = txtValue.Text;
                            content = (EncodeText) ? HTMLHelper.HTMLEncode(content) : content;

                            isValid = CheckIfLengthIsValid(content);
                        }
                        break;
                }
                break;
        }

        return isValid;
    }


    private bool CheckIfLengthIsValid(string text)
    {
        text = HTMLHelper.StripTags(text);

        if (String.IsNullOrEmpty(text))
        {
            return true;
        }

        if ((MinLength > 0) && (text.Length < MinLength))
        {
            ErrorMessage = lblError.Text = String.Format(GetString("EditableText.ErrorMin"), text.Length, MinLength);
            return false;
        }

        if ((MaxLength > 0) && (text.Length > MaxLength))
        {
            ErrorMessage = lblError.Text = String.Format(GetString("EditableText.ErrorMax"), text.Length, MaxLength);
            return false;
        }

        return true;
    }


    /// <summary>
    /// Gets the current control content.
    /// </summary>
    public string GetContent()
    {
        if (!StopProcessing)
        {
            EnsureChildControls();

            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    switch (RegionType)
                    {
                        case CMSEditableRegionTypeEnum.HtmlEditor:
                            // HTML editor
                            if (Editor != null)
                            {
                                return Editor.ResolvedValue;
                            }
                            break;

                        case CMSEditableRegionTypeEnum.TextArea:
                        case CMSEditableRegionTypeEnum.TextBox:
                            // TextBox
                            if (txtValue != null)
                            {
                                return (EncodeText) ? HTMLHelper.HTMLEncode(txtValue.Text) : txtValue.Text;
                            }
                            break;
                    }
                    break;
            }
        }

        return null;
    }


    /// <summary>
    /// Returns the array list of the field IDs (Client IDs of the inner controls) that should be spell checked.
    /// </summary>
    public List<string> GetSpellCheckFields()
    {
        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
                List<string> result = new List<string>();
                switch (RegionType)
                {
                    case CMSEditableRegionTypeEnum.HtmlEditor:
                        // HTML editor
                        if (Editor != null)
                        {
                            result.Add(Editor.ClientID);
                        }
                        break;

                    case CMSEditableRegionTypeEnum.TextArea:
                    case CMSEditableRegionTypeEnum.TextBox:
                        // TextBox
                        if (txtValue != null)
                        {
                            result.Add(txtValue.ClientID);
                        }
                        break;
                }
                return result;
        }
        return null;
    }


    protected void ApplySettings()
    {
        EnsureChildControls();

        if (!StopProcessing)
        {
            // Create controls by actual page mode
            switch (ViewMode)
            {
                case ViewModeEnum.Edit:
                case ViewModeEnum.EditDisabled:
                    {

                        // Display the region control based on the region type
                        switch (RegionType)
                        {
                            case CMSEditableRegionTypeEnum.HtmlEditor:
                                // HTML Editor
                                if (IsDialogEdit)
                                {
                                    Editor.ToolbarLocation = "out:CKToolbar";
                                    Editor.Title = Title;

                                    // Maximize editor to fill entire dialog
                                    Editor.RemoveButtons.Add("Maximize");

                                    // Desktop browsers
                                    Editor.Config["on"] = "{ 'instanceReady' : function(e) { e.editor.execCommand( 'maximize' ); } }";
                                }

                                // Set toolbar location
                                if (HtmlAreaToolbarLocation != "")
                                {
                                    // Show the toolbar
                                    if (HtmlAreaToolbarLocation.ToLowerCSafe() == "out:cktoolbar")
                                    {
                                        mShowToolbar = true;
                                    }

                                    Editor.ToolbarLocation = HtmlAreaToolbarLocation;
                                }

                                // Set the visual appearance
                                if (HtmlAreaToolbar != "")
                                {
                                    Editor.ToolbarSet = HtmlAreaToolbar;
                                }

                                // Get editor area css file
                                if (SiteContext.CurrentSite != null)
                                {
                                    Editor.EditorAreaCSS = string.Empty;
                                }

                                // Set "Insert image or media" dialog configuration
                                Editor.MediaDialogConfig.ResizeToHeight = ResizeToHeight;
                                Editor.MediaDialogConfig.ResizeToWidth = ResizeToWidth;
                                Editor.MediaDialogConfig.ResizeToMaxSideSize = ResizeToMaxSideSize;

                                // Set "Insert link" dialog configuration
                                Editor.LinkDialogConfig.ResizeToHeight = ResizeToHeight;
                                Editor.LinkDialogConfig.ResizeToWidth = ResizeToWidth;
                                Editor.LinkDialogConfig.ResizeToMaxSideSize = ResizeToMaxSideSize;

                                // Set "Quickly insert image" configuration
                                Editor.QuickInsertConfig.ResizeToHeight = ResizeToHeight;
                                Editor.QuickInsertConfig.ResizeToWidth = ResizeToWidth;
                                Editor.QuickInsertConfig.ResizeToMaxSideSize = ResizeToMaxSideSize;

                                break;

                            case CMSEditableRegionTypeEnum.TextArea:
                            case CMSEditableRegionTypeEnum.TextBox:
                                // TextBox
                                if (RegionType == CMSEditableRegionTypeEnum.TextArea)
                                {
                                    txtValue.TextMode = TextBoxMode.MultiLine;
                                }
                                else
                                {
                                    txtValue.TextMode = TextBoxMode.SingleLine;
                                }

                                if (DialogWidth > 0)
                                {
                                    txtValue.Width = new Unit(DialogWidth - 8);
                                }
                                else
                                {
                                    // Default width is 100%
                                    txtValue.Width = new Unit(100, UnitType.Percentage);
                                }

                                if (DialogHeight > 0)
                                {
                                    txtValue.Height = new Unit(DialogHeight);
                                }

                                txtValue.Wrap = WordWrap;

                                break;
                        }
                    }
                    break;
            }
        }
    }


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

        if (!StopProcessing)
        {
            // Load the properties
            RegionTitle = DataHelper.GetNotEmpty(GetValue("RegionTitle"), RegionTitle);
            RegionType = CMSEditableRegionTypeEnumFunctions.GetRegionTypeEnum(DataHelper.GetNotEmpty(GetValue("RegionType"), CMSEditableRegionTypeEnumFunctions.GetRegionTypeString(RegionType)));
            DialogWidth = DataHelper.GetNotZero(GetValue("DialogWidth"), DialogWidth);
            DialogHeight = DataHelper.GetNotZero(GetValue("DialogHeight"), DialogHeight);
            WordWrap = ValidationHelper.GetBoolean(GetValue("WordWrap"), WordWrap);
            MinLength = DataHelper.GetNotZero(GetValue("MinLength"), MinLength);
            MaxLength = DataHelper.GetNotZero(GetValue("MaxLength"), MaxLength);
            HtmlAreaToolbar = DataHelper.GetNotEmpty(GetValue("HtmlAreaToolbar"), HtmlAreaToolbar);
            HtmlAreaToolbarLocation = DataHelper.GetNotEmpty(GetValue("HtmlAreaToolbarLocation"), HtmlAreaToolbarLocation);
        }
    }


    /// <summary>
    /// Overridden CreateChildControls method.
    /// </summary>
    protected override void CreateChildControls()
    {
        SetupControl();

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

                    // Main editor panel
                    pnlEditor = new Panel();
                    pnlEditor.ID = "pnlEditor";
                    pnlEditor.CssClass = "EditableTextEdit EditableText_" + ContentID;
                    pnlEditor.Attributes.Add("data-tracksavechanges", "true");
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

                    // Display the region control based on the region type
                    switch (RegionType)
                    {
                        case CMSEditableRegionTypeEnum.HtmlEditor:
                            // HTML Editor
                            Editor = new CMSHtmlEditor();
                            Editor.ID = "htmlValue";
                            Editor.AutoDetectLanguage = false;
                            Editor.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                            Editor.Title = Title;
                            Editor.UseInlineMode = UseInlineMode;
                            Editor.ContentsLangDirection = CultureHelper.IsPreferredCultureRTL() ? LanguageDirection.RightToLeft : LanguageDirection.LeftToRight;
                            Editor.Node = CurrentPageInfo;

                            // Set the language
                            try
                            {
                                CultureInfo ci = CultureHelper.GetCultureInfo(DataHelper.GetNotEmpty(MembershipContext.AuthenticatedUser.PreferredUICultureCode, LocalizationContext.PreferredCultureCode));
                                Editor.DefaultLanguage = ci.TwoLetterISOLanguageName;
                            }
                            catch (ArgumentNullException)
                            {
                            }
                            catch (CultureNotFoundException)
                            {
                            }

                            Editor.AutoDetectLanguage = false;
                            Editor.Enabled = IsEnabled(ViewMode);

                            if (ViewMode == ViewModeEnum.EditDisabled)
                            {
                                pnlEditor.Controls.Add(new LiteralControl("<div style=\"width: 98%\">"));
                                pnlEditor.Controls.Add(Editor);
                                pnlEditor.Controls.Add(new LiteralControl("</div>"));
                            }
                            else
                            {
                                pnlEditor.Controls.Add(Editor);
                            }
                            break;

                        case CMSEditableRegionTypeEnum.TextArea:
                        case CMSEditableRegionTypeEnum.TextBox:
                            // TextBox
                            txtValue = new CMSTextBox();
                            txtValue.ID = "txtValue";
                            txtValue.CssClass = "EditableTextTextBox";
                            txtValue.Enabled = IsEnabled(ViewMode);
                            pnlEditor.Controls.Add(txtValue);
                            break;
                    }
                    break;

                default:
                    // Display content in non editing modes
                    ltlContent = new Literal();
                    ltlContent.ID = "ltlContent";
                    ltlContent.EnableViewState = false;
                    Controls.Add(ltlContent);
                    break;
            }
        }
    }


    /// <summary>
    /// Returns the value of the given web part property property.
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

    #endregion


    #region "Private methods"

    /// <summary>
    /// Indicates whether this control should be enabled and editing allowed.
    /// </summary>
    /// <param name="viewMode">The view mode.</param>
    private bool IsEnabled(ViewModeEnum viewMode)
    {
        return (viewMode.IsEdit()) && DocumentManager.AllowSave;
    }

    #endregion
}

