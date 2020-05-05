using System;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Properties_URLProperties : ItemProperties
{
    #region "Private properties"

    /// <summary>
    /// Indicates whether the properties are used for path selector form control.
    /// </summary>
    private bool IsSelectPath
    {
        get
        {
            return (Config.CustomFormatCode == "selectpath");
        }
    }


    /// <summary>
    /// Indicates whether the 'content changed' flag should be set.
    /// </summary>
    private bool ContentChanged
    {
        get
        {
            return (QueryHelper.GetString("contentchanged", "") != "false");
        }
    }


    /// <summary>
    /// Indicates whether the properties are used for single path selector form control.
    /// </summary>
    private bool IsSelectSinglePath
    {
        get
        {
            return (QueryHelper.GetString("selectionmode", "") == "single") && IsSelectPath;
        }
    }


    /// <summary>
    /// Indicates whether the properties are used for select relationship path.
    /// </summary>
    private bool IsRelationship
    {
        get
        {
            return (Config.CustomFormatCode == "relationship");
        }
    }


    /// <summary>
    /// Gets or sets the original URL (the one which come when properties are loaded).
    /// </summary>
    private string OriginalUrl
    {
        get
        {
            return ValidationHelper.GetString(ViewState["OriginalUrl"], "");
        }
        set
        {
            ViewState["OriginalUrl"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the permanent URL.
    /// </summary>
    private string PermanentUrl
    {
        get
        {
            return ValidationHelper.GetString(ViewState["PermanentUrl"], "");
        }
        set
        {
            ViewState["PermanentUrl"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to show or hide the controls in WidthHeightSelector.
    /// </summary>
    private bool ShowSizeControls
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ShowSizeControls"], false);
        }
        set
        {
            ViewState["ShowSizeControls"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the properties were not loaded yet.
    /// </summary>
    private bool NoSelectedYet
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["NoSelectedYet"], true);
        }
        set
        {
            ViewState["NoSelectedYet"] = value;
        }
    }


    /// <summary>
    /// Indicates whether by default the chk box "Only subitems" is checked or not.
    /// </summary>
    private bool SubItemsNotByDefault
    {
        get
        {
            return QueryHelper.GetBoolean("SubItemsNotByDefault", false);
        }
    }


    /// <summary>
    /// Returns the default width of the width height selector.
    /// </summary>
    private int DefaultWidth
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["DefaultHeight"], 0);
        }
        set
        {
            ViewState["DefaultHeight"] = value;
        }
    }


    /// <summary>
    /// Returns the default height of the width height selector.
    /// </summary>
    private int DefaultHeight
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["DefaultWidth"], 0);
        }
        set
        {
            ViewState["DefaultWidth"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the current item is image.
    /// </summary>
    private bool CurrentIsImage
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CurrentIsImage"], false);
        }
        set
        {
            ViewState["CurrentIsImage"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the current item is audio/video.
    /// </summary>
    private bool CurrentIsMedia
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CurrentIsMedia"], false);
        }
        set
        {
            ViewState["CurrentIsMedia"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the properties are displayed for the media selector.
    /// </summary>
    private bool IsMediaSelector
    {
        get
        {
            return ((Config.OutputFormat == OutputFormatEnum.URL) && (Config.SelectableContent == SelectableContentEnum.AllFiles));
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value which determines whether the control is displayed on the Web tab.
    /// </summary>
    public bool IsWeb
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsWeb"], false);
        }
        set
        {
            ViewState["IsWeb"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (Config != null)
        {
            if ((Config.OutputFormat == OutputFormatEnum.URL) && IsWeb)
            {
                plcPropContent.Visible = false;
            }

            if ((Config.OutputFormat == OutputFormatEnum.URL) && (Config.SelectableContent == SelectableContentEnum.AllContent))
            {
                DisplaySizeSelector(false);
                pnlImagePreview.CssClass = "DialogPropertiesPreview DialogPropertiesPreviewFull";
                pnlMediaPreview.CssClass = "DialogPropertiesPreview DialogPropertiesPreviewFull";
            }
        }

        // Refresh button
        imgRefresh.ToolTip = imgRefresh.ScreenReaderDescription = GetString("dialogs.web.refresh");

        tabImageGeneral.HeaderText = GetString("general.general");

        string postBackRef = ControlsHelper.GetPostBackEventReference(btnHidden, "");
        string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";
        string postBackRefTxt = ControlsHelper.GetPostBackEventReference(btnTxtHidden, "");

        // Assign javascript change event to all fields (to refresh the preview)
        widthHeightElem.HeightTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        widthHeightElem.WidthTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
        widthHeightElem.ScriptAfterChange = postBackRef;
        txtUrl.Attributes["onchange"] = postBackRefTxt;
        txtUrl.Attributes["onkeydown"] = postBackKeyDownRef;

        btnHidden.Click += btnHidden_Click;
        btnTxtHidden.Click += btnTxtHidden_Click;
        btnHiddenSize.Click += btnHiddenSize_Click;

        if (IsSelectPath || IsRelationship)
        {
            plcUrl.Visible = false;
            plcSizeSelector.Visible = false;
            plcPreviewArea.Visible = false;
            plcAltText.Visible = false;

            plcSelectPath.Visible = true;

            string resString = (IsSelectPath ? "dialogs.web.selectedpath" : "dialogs.web.selecteddoc");
            lblSelectPah.ResourceString = resString;
            lblSelectPah.DisplayColon = true;
        }

        if (IsSelectPath && !IsSelectSinglePath)
        {
            plcIncludeSubitems.Visible = true;
            chkItems.Attributes.Add("onclick", "chkItemsChecked_Changed(this.checked);");
        }

        LoadPreview();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Check if alternate text should be visible
        var imgAltElemId = QueryHelper.GetString(DialogParameters.IMG_ALT_CLIENTID, String.Empty);
        if (String.IsNullOrEmpty(imgAltElemId))
        {
            plcAltText.Visible = false;
        }

        if (!RequestHelper.IsPostBack())
        {
            widthHeightElem.Locked = true;
            
            if (plcAltText.Visible)
            {
                ScriptHelper.RegisterWOpenerScript(Page);
                // Set alternate text
                string scriptAlt = @"
if (wopener) {
    var hdnAlt = wopener.document.getElementById('" + ScriptHelper.GetString(imgAltElemId, false) + @"');
    var txt = document.getElementById('" + txtAlt.ClientID + @"');
    if ((hdnAlt != null) && (txt != null)) {
       txt.value = hdnAlt.value;
    }
}";
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "DialogAltImageScript", ScriptHelper.GetScript(scriptAlt));
            }
        }

        widthHeightElem.CustomRefreshCode = ControlsHelper.GetPostBackEventReference(btnHiddenSize, "") + "; return false;";
        widthHeightElem.ShowActions = ShowSizeControls;

        bool isLink = Config.OutputFormat == OutputFormatEnum.BBLink || Config.OutputFormat == OutputFormatEnum.HTMLLink ||
                       (Config.OutputFormat == OutputFormatEnum.URL && Config.SelectableContent == SelectableContentEnum.AllContent);

        DisplaySizeSelector(ShowSizeControls && !isLink);

        lblEmpty.Text = NoSelectionText;

        // If size and alternate text input should not be displayed, hide them
        if (Config.UseSimpleURLProperties)
        {
            HideComplexURLProperties();
        }
    }


    /// <summary>
    /// Update parameters and preview from URL textbox.
    /// </summary>
    private void UpdateFromUrl()
    {
        int width = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(txtUrl.Text, "width"), 0);
        int height = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(txtUrl.Text, "height"), 0);

        // Update WIDTH and HEIGHT information according URL
        if (width > 0)
        {
            widthHeightElem.Width = width;
        }
        if (height > 0)
        {
            widthHeightElem.Height = height;
        }

        pnlUpdateWidthHeight.Update();
        LoadPreview();
    }


    protected void imgRefresh_Click(object sender, EventArgs e)
    {
        UpdateFromUrl();
    }


    protected void btnTxtHidden_Click(object sender, EventArgs e)
    {
        UpdateFromUrl();
    }


    protected void btnHiddenSize_Click(object sender, EventArgs e)
    {
        widthHeightElem.Width = DefaultWidth;
        widthHeightElem.Height = DefaultHeight;

        // Remove width & height parameters from url
        string url = URLHelper.RemoveParameterFromUrl(URLHelper.RemoveParameterFromUrl(OriginalUrl, "width"), "height");

        // If media selector - insert dimensions to the URL
        if (IsMediaSelector)
        {
            url = EnsureMediaSelector(url);
        }
        
        txtUrl.Text = url;

        LoadPreview();
    }


    protected void btnHidden_Click(object sender, EventArgs e)
    {
        // Update item URL
        bool getPermanent = ((widthHeightElem.Width < DefaultWidth) ||
                             (widthHeightElem.Height < DefaultHeight)) &&
                            (SourceType == MediaSourceEnum.MediaLibraries);

        string url = UpdateUrl(widthHeightElem.Width, widthHeightElem.Height, (getPermanent ? PermanentUrl : OriginalUrl));

        // If media selector - insert dimensions to the URL
        if (IsMediaSelector)
        {
            url = EnsureMediaSelector(url);
        }

        pnlUpdateImgUrl.Update();
        txtUrl.Text = url;

        LoadPreview();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns URL updated according specified properties.
    /// </summary>
    /// <param name="height">Height of the item</param>
    /// <param name="width">Width of the item</param>
    /// <param name="url">URL to update</param>
    private string UpdateUrl(int width, int height, string url)
    {
        // WIDTH & HEIGHT isn't required in URL
        if (((Config.OutputFormat == OutputFormatEnum.URL) && !CurrentIsImage) && !(IsMediaSelector && ShowSizeControls))
        {
            return url;
        }

        // Media selector always put dimensions in the URL when size selector is visible and direct path to the filesystem is not used
        bool forceSizeToUrl = (IsMediaSelector && ShowSizeControls);

        return CMSDialogHelper.UpdateUrl(width, height, DefaultWidth, DefaultHeight, url, SourceType, forceSizeToUrl);
    }


    /// <summary>
    /// Performs additional actions for the media selector.
    /// </summary>
    /// <param name="url">Base item URL</param>
    private string EnsureMediaSelector(string url)
    {
        url = EnsureExtension(txtUrl.Text, url);

        if (ShowSizeControls)
        {
            url = CMSDialogHelper.UpdateUrl(widthHeightElem.Width, widthHeightElem.Height, DefaultWidth, DefaultHeight, url, SourceType, true);
        }

        return url;
    }


    /// <summary>
    /// Ensures that URL contains extension as required.
    /// </summary>
    /// <param name="refUrl">Original URL possibly containing extension</param>
    /// <param name="updateUrl">URL to ensure extension for</param>
    private string EnsureExtension(string refUrl, string updateUrl)
    {
        string ext = URLHelper.GetUrlParameter(refUrl, "ext");
        if (!string.IsNullOrEmpty(ext))
        {
            updateUrl = URLHelper.UpdateParameterInUrl(updateUrl, "ext", ext);
        }

        return updateUrl;
    }


    /// <summary>
    /// Configures the preview control.
    /// </summary>
    private void LoadPreview()
    {
        pnlImagePreview.Visible = CurrentIsImage;
        pnlMediaPreview.Visible = (!CurrentIsImage && CurrentIsMedia);
        if (CurrentIsImage)
        {
            string url = txtUrl.Text.Trim();
            if (!string.IsNullOrEmpty(url))
            {
                url = URLHelper.UpdateParameterInUrl(url, "chset", Guid.NewGuid().ToString());
            }

            // Add latest version requirement for live site
            if (IsLiveSite)
            {
                // Add requirement for latest version of files for current document
                string newparams = "latestforhistoryid=" + HistoryID;
                newparams += "&hash=" + ValidationHelper.GetHashString("h" + HistoryID, new HashSettings(""));

                url += "&" + newparams;
            }

            imagePreview.URL = url;
            imagePreview.SizeToURL = ValidationHelper.GetBoolean(ViewState[DialogParameters.IMG_SIZETOURL], false);

            // Don't allow to resize image over the original dimensions
            imagePreview.Width = widthHeightElem.Width > DefaultWidth ? DefaultWidth : widthHeightElem.Width;
            imagePreview.Height = widthHeightElem.Height > DefaultHeight ? DefaultHeight : widthHeightElem.Height;
        }
        else
        {
            string url = txtUrl.Text.Trim();
            string ext = ValidationHelper.GetString(ViewState[DialogParameters.URL_EXT], "");
            if (!String.IsNullOrEmpty(ext))
            {
                url = URLHelper.UpdateParameterInUrl(url, "ext", "." + ext.TrimStart('.'));
            }
            mediaPreview.Url = url;

            if (CurrentIsMedia && ((widthHeightElem.Width == 0) || widthHeightElem.Height == 0))
            {
                widthHeightElem.Width = 300;
                widthHeightElem.Height = GetDefaultAVHeight(ext);
            }

            if (MediaHelper.IsAudioVideo(ext))
            {
                mediaPreview.AVControls = true;
            }

            mediaPreview.Width = widthHeightElem.Width;
            mediaPreview.Height = widthHeightElem.Height;
        }

        // Ensure extension is at the end of URL
        string urlExt = URLHelper.GetUrlParameter(txtUrl.Text, "ext");
        if (!string.IsNullOrEmpty(urlExt))
        {
            txtUrl.Text = URLHelper.UpdateParameterInUrl(txtUrl.Text, "ext", urlExt);
        }

        SaveSession();
    }


    /// <summary>
    /// Save current properties into session.
    /// </summary>
    private void SaveSession()
    {
        var savedProperties = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable ?? new Hashtable();
        var props = GetItemProperties();

        foreach (DictionaryEntry entry in props)
        {
            savedProperties[entry.Key] = entry.Value;
        }

        SessionHelper.SetValue("DialogSelectedParameters", savedProperties);
    }


    /// <summary>
    /// Show or hide size selector.
    /// </summary>
    /// <param name="display">Indicates if size selector should be displayed</param>
    private void DisplaySizeSelector(bool display)
    {
        plcSizeSelector.Visible = display;
    }


    /// <summary>
    /// Hides complex URL Properties (size and Alternate text) and handles other necessary changes (setting colspans, etc.).
    /// </summary>
    private void HideComplexURLProperties()
    {
        DisplaySizeSelector(false);

        plcAltText.Visible = false;
    }

    #endregion


    #region "Overridden methods"

    public override void LoadSelectedItems(MediaItem item, Hashtable properties)
    {
        if (properties == null)
        {
            properties = new Hashtable();
        }

        string url = item.Url;
        OriginalUrl = item.Url;
        PermanentUrl = item.PermanentUrl;
        HistoryID = item.HistoryID;

        switch (item.MediaType)
        {
            case MediaTypeEnum.Image:
                properties[DialogParameters.IMG_WIDTH] = item.Width;
                properties[DialogParameters.IMG_HEIGHT] = item.Height;
                properties[DialogParameters.IMG_ORIGINALWIDTH] = item.Width;
                properties[DialogParameters.IMG_ORIGINALHEIGHT] = item.Height;
                break;

            case MediaTypeEnum.AudioVideo:
                properties[DialogParameters.AV_WIDTH] = 400;
                properties[DialogParameters.AV_HEIGHT] = 300;
                properties[DialogParameters.AV_CONTROLS] = true;
                break;
        }

        ViewState[DialogParameters.URL_EXT] = item.Extension;
        properties[DialogParameters.URL_EXT] = item.Extension;
        properties[DialogParameters.URL_URL] = url;

        ViewState[DialogParameters.DOC_TARGETNODEID] = item.NodeID;
        properties[DialogParameters.DOC_TARGETNODEID] = item.NodeID;
        properties[DialogParameters.DOC_NODEALIASPATH] = item.AliasPath;

        LoadProperties(properties);

        if (tabImageGeneral.Visible)
        {
            LoadPreview();
        }
    }


    /// <summary>
    /// Loads the properties into control.
    /// </summary>
    /// <param name="p">Collection with properties</param>
    public override void LoadItemProperties(Hashtable p)
    {
        LoadProperties(p);
        if (tabImageGeneral.Visible)
        {
            LoadPreview();
        }
    }


    /// <summary>
    /// Loads the properties into control.
    /// </summary>
    /// <param name="properties">Collection with properties</param>
    public override void LoadProperties(Hashtable properties)
    {
        if (properties != null)
        {
            // Display the properties
            pnlEmpty.Visible = false;
            pnlTabs.CssClass = "Dialog_Tabs";


            #region "Image general tab"

            // Display size selector only if required or image
            string ext = ValidationHelper.GetString(properties[DialogParameters.URL_EXT], "");
            CurrentIsImage = ImageHelper.IsImage(ext);
            CurrentIsMedia = !CurrentIsImage && MediaHelper.IsAudioVideo(ext);
            ShowSizeControls = DisplaySizeSelector();

            if (tabImageGeneral.Visible)
            {
                string url = HttpUtility.HtmlDecode(ValidationHelper.GetString(properties[DialogParameters.URL_URL], ""));
                if (url != "")
                {
                    OriginalUrl = url;
                }

                if ((Config.SelectableContent == SelectableContentEnum.OnlyMedia) || IsMediaSelector)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "ext", "." + ext.TrimStart('.'));
                }

                if (CurrentIsImage)
                {
                    int width = ValidationHelper.GetInteger(properties[DialogParameters.IMG_WIDTH], 0);
                    int height = ValidationHelper.GetInteger(properties[DialogParameters.IMG_HEIGHT], 0);

                    DefaultWidth = ValidationHelper.GetInteger(properties[DialogParameters.IMG_ORIGINALWIDTH], 0);
                    DefaultHeight = ValidationHelper.GetInteger(properties[DialogParameters.IMG_ORIGINALHEIGHT], 0);

                    // Ensure WIDTH & HEIGHT dimensions
                    if (width == 0)
                    {
                        width = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(url, "width"), 0);
                        if (width == 0)
                        {
                            width = DefaultWidth;
                        }
                    }
                    if (height == 0)
                    {
                        height = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(url, "height"), 0);
                        if (height == 0)
                        {
                            height = DefaultHeight;
                        }
                    }

                    widthHeightElem.Width = width;
                    widthHeightElem.Height = height;

                    ViewState[DialogParameters.IMG_SIZETOURL] = ValidationHelper.GetBoolean(properties[DialogParameters.IMG_SIZETOURL], false);
                }
                else
                {
                    DefaultWidth = 300;
                    DefaultHeight = GetDefaultAVHeight(ext);

                    // Restore current dimensions from URL if present
                    widthHeightElem.Width = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(url, "width"), DefaultWidth);
                    widthHeightElem.Height = ValidationHelper.GetInteger(URLHelper.GetUrlParameter(url, "height"), DefaultHeight);
                }

                // If media selector - insert dimensions to the URL
                if (IsMediaSelector && ShowSizeControls)
                {
                    url = CMSDialogHelper.UpdateUrl(widthHeightElem.Width, widthHeightElem.Height, CurrentIsImage ? DefaultWidth : 0, CurrentIsImage ? DefaultHeight : 0, url, SourceType, true);
                }
                txtUrl.Text = url;

                // Initialize media file URLs
                if (SourceType == MediaSourceEnum.MediaLibraries)
                {
                    OriginalUrl = (string.IsNullOrEmpty(OriginalUrl) ? ValidationHelper.GetString(properties[DialogParameters.URL_DIRECT], "") : OriginalUrl);
                    PermanentUrl = (string.IsNullOrEmpty(PermanentUrl) ? ValidationHelper.GetString(properties[DialogParameters.URL_PERMANENT], "") : PermanentUrl);
                }
                else
                {
                    OriginalUrl = url;
                }
            }

            #endregion


            #region "General items"

            ViewState[DialogParameters.URL_EXT] = (properties[DialogParameters.URL_EXT] != null ? ValidationHelper.GetString(properties[DialogParameters.URL_EXT], "") : ValidationHelper.GetString(properties[DialogParameters.IMG_EXT], ""));
            ViewState[DialogParameters.URL_URL] = ValidationHelper.GetString(properties[DialogParameters.URL_URL], "");
            EditorClientID = ValidationHelper.GetString(properties[DialogParameters.EDITOR_CLIENTID], "");

            #endregion


            #region "Select path & Relationship items"

            if (IsRelationship || IsSelectPath)
            {
                var aliasPath = SqlHelper.EscapeLikeText(properties[DialogParameters.DOC_NODEALIASPATH].ToString(""));
                if (!String.IsNullOrEmpty(aliasPath))
                {
                    txtSelectPath.Text = aliasPath;
                }

                if (IsSelectPath && !IsSelectSinglePath)
                {
                    if (NoSelectedYet)
                    {
                        chkItems.Checked = !SubItemsNotByDefault;
                        NoSelectedYet = false;
                    }

                    if (chkItems.Checked)
                    {
                        txtSelectPath.Text = txtSelectPath.Text.TrimEnd('/') + "/%";
                    }
                }
            }

            #endregion
        }
    }


    /// <summary>
    /// Decides whether the size selector should be displayed.
    /// </summary>
    private bool DisplaySizeSelector()
    {
        // Start with media selector
        bool result = (IsMediaSelector && (CurrentIsImage || CurrentIsMedia));

        // Is image selector ?
        result = result || ((Config.OutputFormat == OutputFormatEnum.URL) && (Config.SelectableContent == SelectableContentEnum.OnlyImages));

        return result;
    }


    /// <summary>
    /// Returns default height for the A/V items.
    /// </summary>
    /// <param name="ext">Extension of the file</param>
    private int GetDefaultAVHeight(string ext)
    {
        // Audio default height = 45, video = 200
        return (MediaHelper.IsAudio(ext) ? 45 : 200);
    }


    /// <summary>
    /// Returns all parameters of the selected item as name – value collection.
    /// </summary>
    public override Hashtable GetItemProperties()
    {
        Hashtable retval = new Hashtable();


        #region "Image general tab"

        string ext = ValidationHelper.GetString(ViewState[DialogParameters.URL_EXT], "");
        string url = ValidationHelper.GetString(ViewState[DialogParameters.URL_URL], "");

        if (!(IsRelationship || IsSelectPath))
        {
            bool resolveUrl = (!Config.ContentUseRelativeUrl && !((Config.OutputFormat == OutputFormatEnum.URL) && (Config.SelectableContent == SelectableContentEnum.OnlyMedia)));

            // Exception for MediaSelector control (it can't be resolved)
            url = (resolveUrl ? UrlResolver.ResolveUrl(url) : url);

            if (MediaHelper.IsAudioVideo(ext))
            {
                retval[DialogParameters.AV_URL] = txtUrl.Text.Trim();
            }
            else if (tabImageGeneral.Visible)
            {
                string imgUrl = txtUrl.Text.Trim();
                bool sizeToUrl = ValidationHelper.GetBoolean(ViewState[DialogParameters.IMG_SIZETOURL], false);

                if (widthHeightElem.Width < DefaultWidth)
                {
                    retval[DialogParameters.IMG_WIDTH] = widthHeightElem.Width;
                    if (sizeToUrl)
                    {
                        url = URLHelper.AddParameterToUrl(url, "width", widthHeightElem.Width.ToString());
                    }
                }
                if (widthHeightElem.Height < DefaultHeight)
                {
                    retval[DialogParameters.IMG_HEIGHT] = widthHeightElem.Height;
                    if (sizeToUrl)
                    {
                        url = URLHelper.AddParameterToUrl(url, "height", widthHeightElem.Height.ToString());
                    }
                }

                
                
                retval[DialogParameters.IMG_URL] = (resolveUrl ? UrlResolver.ResolveUrl(imgUrl) : imgUrl);
                retval[DialogParameters.IMG_EXT] = ValidationHelper.GetString(ViewState[DialogParameters.URL_EXT], "");
                retval[DialogParameters.IMG_SIZETOURL] = sizeToUrl;
                retval[DialogParameters.IMG_ALT] = txtAlt.Text.Trim();
                retval[DialogParameters.IMG_ALT_CLIENTID] = QueryHelper.GetString(DialogParameters.IMG_ALT_CLIENTID, String.Empty);
            }
        }

        #endregion


        #region "General items"

        retval[DialogParameters.URL_EXT] = ext;
        retval[DialogParameters.URL_URL] = url;
        retval[DialogParameters.EDITOR_CLIENTID] = EditorClientID;

        #endregion


        #region "Select path & Relationship items"

        if (IsRelationship || IsSelectPath)
        {
            string path = txtSelectPath.Text;
            if (chkItems.Checked)
            {
                if (!path.EndsWithCSafe("/%"))
                {
                    path = path.TrimEnd('/') + "/%";
                }
            }
            else if (path.EndsWithCSafe("/%"))
            {
                path = path.Substring(0, path.Length - 2);
            }
            retval[DialogParameters.DOC_NODEALIASPATH] = path;

            if (!ContentChanged)
            {
                // Don't set 'content changed' flag
                retval[DialogParameters.CONTENT_CHANGED] = false;
            }

            // Fill target node id only if single path selection is enabled or in relationship dialog
            if (IsSelectSinglePath || IsRelationship)
            {
                retval[DialogParameters.DOC_TARGETNODEID] = ViewState[DialogParameters.DOC_TARGETNODEID];
            }
        }

        #endregion


        return retval;
    }


    /// <summary>
    /// Validates all the user input.
    /// </summary>
    public override bool Validate()
    {
        string errorMessage = "";

        if (!widthHeightElem.Validate())
        {
            errorMessage += " " + GetString("dialogs.image.invalidsize");
        }

        errorMessage = errorMessage.Trim();
        if (errorMessage != "")
        {
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "ImagePropertiesError", ScriptHelper.GetAlertScript(errorMessage));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Clears the properties form.
    /// </summary>
    public override void ClearProperties(bool hideProperties)
    {
        // Hide the properties
        pnlEmpty.Visible = hideProperties;
        pnlTabs.CssClass = (hideProperties ? "DialogElementHidden" : "Dialog_Tabs");
        chkItems.Checked = false;

        widthHeightElem.Height = 0;
        widthHeightElem.Width = 0;
        imagePreview.URL = "";
        txtUrl.Text = "";
        txtSelectPath.Text = "";
    }

    #endregion
}
