using System;
using System.Collections;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Properties_BBMediaProperties : ItemProperties
{
    #region "Private properties"

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

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether the URL text box should be hidden.
    /// </summary>
    public bool HideUrlBox
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["HideUrlBox"], false);
        }
        set
        {
            ViewState["HideUrlBox"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (!RequestHelper.IsPostBack() && IsWeb)
            {
                pnlEmpty.Visible = false;
                pnlTabs.CssClass = "Dialog_Tabs";
            }

            // Refresh button
            imgRefresh.Click += imgRefresh_Click;
            imgRefresh.ToolTip = imgRefresh.ScreenReaderDescription = GetString("dialogs.web.refresh");

            tabImageGeneral.HeaderText = GetString("general.general");

            string postBackRef = ControlsHelper.GetPostBackEventReference(btnHidden, "");
            string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";
            string postBackRefTxt = ControlsHelper.GetPostBackEventReference(btnTxtHidden, "");
            string postBackKeyDownTxtRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRefTxt + "; return false;}";

            // Assign javascript change event to all fields (to refresh the preview)
            widthHeightElem.HeightTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
            widthHeightElem.WidthTextBox.Attributes["onkeydown"] = postBackKeyDownRef;
            widthHeightElem.ScriptAfterChange = postBackRef;
            txtUrl.Attributes["onchange"] = postBackRefTxt;
            txtUrl.Attributes["onkeydown"] = postBackKeyDownTxtRef;

            btnHidden.Click += btnHidden_Click;
            btnTxtHidden.Click += btnTxtHidden_Click;
            btnHiddenSize.Click += btnHiddenSize_Click;

            widthHeightElem.CustomRefreshCode = ControlsHelper.GetPostBackEventReference(btnHiddenSize, "") + ";return false;";
            widthHeightElem.ShowActions = true;

            if (!RequestHelper.IsPostBack())
            {
                EditorClientID = QueryHelper.GetString("editor_clientid", "");
                widthHeightElem.Locked = true;
                LoadPreview();
            }

            plcUrlBox.Visible = !HideUrlBox;
            string className = (HideUrlBox) ? "large-preview" : "small-preview";
            pnlPreviewSize.CssClass = className;

            if (!string.IsNullOrEmpty(NoSelectionText))
            {
                lblEmpty.Text = NoSelectionText;
            }
            else
            {
                pnlEmpty.Visible = false;
            }

            LoadPreview();
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
        txtUrl.Text = url;

        LoadPreview();
    }


    protected void btnHidden_Click(object sender, EventArgs e)
    {
        // Update item URL
        bool getPermanent = ((widthHeightElem.Width <= DefaultWidth) ||
                             (widthHeightElem.Height <= DefaultHeight)) &&
                            (SourceType == MediaSourceEnum.MediaLibraries);

        txtUrl.Text = UpdateUrl(widthHeightElem.Width, widthHeightElem.Height, (getPermanent ? PermanentUrl : OriginalUrl));

        pnlUpdateImgUrl.Update();
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
        return CMSDialogHelper.UpdateUrl(width, height, DefaultWidth, DefaultHeight, url, SourceType);
    }


    /// <summary>
    /// Configures the preview control.
    /// </summary>
    private void LoadPreview()
    {
        string url = txtUrl.Text;
        if (!string.IsNullOrEmpty(url))
        {
            bool isImage = false;
            MediaSource source = CMSDialogHelper.GetMediaData(url, null);
            if (source != null)
            {
                isImage = ImageHelper.IsImage(source.Extension);
            }

            // Only for image extensions add chset to url
            if (isImage)
            {
                url = URLHelper.UpdateParameterInUrl(url, "chset", Guid.NewGuid().ToString());

                int versionHistoryId = HistoryID;
                if (IsLiveSite && (versionHistoryId > 0))
                {
                    // Add requirement for latest version of files for current document
                    string newparams = "latestforhistoryid=" + versionHistoryId;
                    newparams += "&hash=" + ValidationHelper.GetHashString("h" + versionHistoryId, new HashSettings(""));

                    url += "&" + newparams;
                }
            }

            imagePreview.Visible = true;
            imagePreview.URL = url;
            imagePreview.SizeToURL = ValidationHelper.GetBoolean(ViewState[DialogParameters.IMG_SIZETOURL], false);
            imagePreview.Width = widthHeightElem.Width > DefaultWidth ? DefaultWidth : widthHeightElem.Width;
            imagePreview.Height = widthHeightElem.Height > DefaultHeight ? DefaultHeight : widthHeightElem.Height;
            
            SaveSession();
        }
        else
        {
            imagePreview.Visible = false;
        }
    }


    /// <summary>
    /// Save current properties into session.
    /// </summary>
    private void SaveSession()
    {
        Hashtable savedProperties = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
        if (savedProperties == null)
        {
            savedProperties = new Hashtable();
        }
        Hashtable properties = GetItemProperties();
        foreach (DictionaryEntry entry in properties)
        {
            savedProperties[entry.Key] = entry.Value;
        }
        SessionHelper.SetValue("DialogSelectedParameters", savedProperties);
    }

    #endregion


    #region "Overridden methods"

    public override void LoadSelectedItems(MediaItem item, Hashtable properties)
    {
        if (!string.IsNullOrEmpty(item.Url))
        {
            // Display the properties
            pnlEmpty.Visible = false;
            pnlTabs.CssClass = "Dialog_Tabs";

            widthHeightElem.Width = item.Width;
            widthHeightElem.Height = item.Height;

            DefaultWidth = item.Width;
            DefaultHeight = item.Height;
            HistoryID = item.HistoryID;

            if (properties == null)
            {
                properties = new Hashtable();
            }
            properties[DialogParameters.IMG_WIDTH] = item.Width;
            properties[DialogParameters.IMG_HEIGHT] = item.Height;
            properties[DialogParameters.IMG_ORIGINALWIDTH] = item.Width;
            properties[DialogParameters.IMG_ORIGINALHEIGHT] = item.Height;

            properties[DialogParameters.IMG_URL] = item.Url;
            txtUrl.Text = item.Url;
            OriginalUrl = item.Url;
            PermanentUrl = item.PermanentUrl;

            properties[DialogParameters.EDITOR_CLIENTID] = EditorClientID;
        }
        LoadProperties(properties);
        LoadPreview();
    }


    /// <summary>
    /// Loads the properites into control.
    /// </summary>
    /// <param name="properties">Collection with properties</param>
    public override void LoadItemProperties(Hashtable properties)
    {
        LoadProperties(properties);
        LoadPreview();
    }


    public override void LoadProperties(Hashtable properties)
    {
        if (properties != null)
        {
            // Display the properties
            pnlEmpty.Visible = false;
            pnlTabs.CssClass = "Dialog_Tabs";


            #region "Image general tab"

            if (tabImageGeneral.Visible)
            {
                int width = ValidationHelper.GetInteger(properties[DialogParameters.IMG_WIDTH], 0);
                int height = ValidationHelper.GetInteger(properties[DialogParameters.IMG_HEIGHT], 0);

                DefaultWidth = ValidationHelper.GetInteger(properties[DialogParameters.IMG_ORIGINALWIDTH], 0);
                DefaultHeight = ValidationHelper.GetInteger(properties[DialogParameters.IMG_ORIGINALHEIGHT], 0);

                widthHeightElem.Width = width;
                widthHeightElem.Height = height;

                OriginalUrl = ValidationHelper.GetString(properties[DialogParameters.IMG_URL], "");
                txtUrl.Text = OriginalUrl;
                ViewState[DialogParameters.IMG_SIZETOURL] = ValidationHelper.GetBoolean(properties[DialogParameters.IMG_SIZETOURL], false);
            }

            #endregion


            #region "General items"

            EditorClientID = ValidationHelper.GetString(properties[DialogParameters.EDITOR_CLIENTID], "");

            #endregion
        }
    }


    /// <summary>
    /// Returns all parameters of the selected item as name – value collection.
    /// </summary>
    public override Hashtable GetItemProperties()
    {
        Hashtable retval = new Hashtable();


        #region "Image general tab"

        if (tabImageGeneral.Visible)
        {
            string url = txtUrl.Text.Trim();
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
            retval[DialogParameters.IMG_URL] = UrlResolver.ResolveUrl(url);
            retval[DialogParameters.IMG_SIZETOURL] = sizeToUrl;
        }

        #endregion


        #region "General items"

        retval[DialogParameters.EDITOR_CLIENTID] = EditorClientID;

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
            LoadPreview();
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

        widthHeightElem.Height = 0;
        widthHeightElem.Width = 0;
        imagePreview.URL = "";
        txtUrl.Text = "";
    }

    #endregion
}
