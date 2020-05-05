using System;
using System.Collections;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Web_WebContentSelector : CMSUserControl
{
    #region "Variables"

    private SelectableContentEnum mSelectableContent = SelectableContentEnum.OnlyMedia;
    private DialogConfiguration mConfig;
    private int mWidth;
    private int mHeight;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns current properties (according to OutputFormat).
    /// </summary>
    private ItemProperties ItemProperties
    {
        get
        {
            switch (Config.OutputFormat)
            {
                case OutputFormatEnum.HTMLMedia:
                    return propMedia;

                case OutputFormatEnum.BBMedia:
                    return propBBMedia;

                default:
                    return propURL;
            }
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current dialog configuration.
    /// </summary>
    public DialogConfiguration Config
    {
        get
        {
            return mConfig ?? (mConfig = DialogConfiguration.GetDialogConfiguration());
        }
    }


    /// <summary>
    /// Gets or sets the type of the content which can be selected.
    /// </summary>
    public SelectableContentEnum SelectableContent
    {
        get
        {
            return mSelectableContent;
        }
        set
        {
            mSelectableContent = value;
        }
    }


    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            propMedia.IsLiveSite = value;
            propBBMedia.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!StopProcessing)
        {
            drpMediaType.Items.Add(new ListItem(GetString("dialogs.web.select"), ""));
            drpMediaType.Items.Add(new ListItem(GetString("dialogs.web.image"), "image"));
            if (Config.SelectableContent != SelectableContentEnum.OnlyImages)
            {
                drpMediaType.Items.Add(new ListItem(GetString("dialogs.web.av"), "av"));
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (Config.OutputFormat == OutputFormatEnum.URL)
        {
            plcMediaType.Visible = false;
            plcRefresh.Visible = false;
        }

        if (Config.UseSimpleURLProperties)
        {
            plcAlternativeText.Visible = false;
        }

        drpMediaType.SelectedIndexChanged += drpMediaType_SelectedIndexChanged;

        imgRefresh.ToolTip = imgRefresh.ScreenReaderDescription = GetString("dialogs.web.refresh");
        imgRefresh.Click += imgRefresh_Click;

        // Get reference causing postback to hidden button
        string postBackRef = ControlsHelper.GetPostBackEventReference(hdnButton, "");
        ltlScript.Text = ScriptHelper.GetScript("function RaiseHiddenPostBack(){" + postBackRef + ";}\n");
        plcInfo.Visible = false;

        // OnChange and OnKeyDown event triggers
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "txtUrlChange", ScriptHelper.GetScript("$cmsj(function(){ $cmsj('" + txtUrl.ClientID + "').change(function (){ $cmsj('#" + imgRefresh.ClientID + "').trigger('click');});});"));
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "txtUrlKeyDown", ScriptHelper.GetScript("$cmsj(function(){ $cmsj('#" + txtUrl.ClientID + "').keydown(function(event){ if (event.keyCode == 13) { $cmsj('#" + imgRefresh.ClientID + "').trigger('click'); return false;}});});"));

        InitializeDesignScripts();

        if (!RequestHelper.IsPostBack())
        {
            InitFromQueryString();
            DisplayProperties();

            if (Config.OutputFormat == OutputFormatEnum.BBMedia)
            {
                // For BB editor properties are always visible and only image is allowed.
                plcBBMediaProp.Visible = true;
                propBBMedia.NoSelectionText = "";
                drpMediaType.Items.Remove(new ListItem(GetString("dialogs.web.select"), ""));
            }

            Hashtable selectedItem = SessionHelper.GetValue("DialogParameters") as Hashtable;
            if ((selectedItem != null) && (selectedItem.Count > 0))
            {
                LoadSelectedItem(selectedItem);
                SessionHelper.SetValue("DialogParameters", null);
            }
            else
            {
                // Try get selected item from session
                selectedItem = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
                if ((selectedItem != null) && (selectedItem.Count > 0))
                {
                    LoadSelectedItem(selectedItem);
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Check if alternate text should be visible
        var imgAltElemId = QueryHelper.GetString(DialogParameters.IMG_ALT_CLIENTID, String.Empty);
        if (String.IsNullOrEmpty(imgAltElemId))
        {
            plcAlternativeText.Visible = false;
        }

        // Load alternate text value
        if (!RequestHelper.IsPostBack() && plcAlternativeText.Visible)
        {
            ScriptHelper.RegisterWOpenerScript(Page);
            // Set alternate text
            string scriptAlt = @"if (wopener) {
                                var hdnAlt = wopener.document.getElementById('" + ScriptHelper.GetString(imgAltElemId, false) + @"');
                                var txt = document.getElementById('" + txtAlt.ClientID + @"');
                                if ((hdnAlt != null) && (txt != null)) {
                                        txt.value = hdnAlt.value;
                                    }
                                }";
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "DialogAltImageScript", ScriptHelper.GetScript(scriptAlt));
        }

        base.OnPreRender(e);
    }


    protected void imgRefresh_Click(object sender, EventArgs e)
    {
        if (plcMediaType.Visible)
        {
            MediaSource source = CMSDialogHelper.GetMediaData(txtUrl.Text, null);
            if ((source == null) || (source.MediaType == MediaTypeEnum.Unknown))
            {
                ItemProperties.ItemNotSystem = true;

                // Try get source type from URL extension
                int index = txtUrl.Text.LastIndexOfCSafe('.');
                if (index > 0)
                {
                    string ext = txtUrl.Text.Substring(index);
                    if (ext.Contains("?"))
                    {
                        ext = URLHelper.RemoveQuery(ext);
                    }
                    if (source == null)
                    {
                        source = new MediaSource();
                    }
                    source.Extension = ext;
                    if (source.MediaType == MediaTypeEnum.Image)
                    {
                        // Get the data
                        GetExternalImageDimensions(txtUrl.Text.Trim());
                    }
                    else
                    {
                        source.MediaWidth = 300;
                        source.MediaHeight = 200;
                    }
                }
            }
            else
            {
                ItemProperties.ItemNotSystem = false;
            }

            if (source != null)
            {
                // Set default dimensions when not specified
                if ((mWidth == 0) && (mHeight == 0))
                {
                    mWidth = source.MediaWidth;
                    mHeight = source.MediaHeight;
                }
                switch (source.MediaType)
                {
                    case MediaTypeEnum.Image:
                        drpMediaType.SelectedValue = "image";
                        break;

                    case MediaTypeEnum.AudioVideo:
                        drpMediaType.SelectedValue = "av";
                        break;

                    default:
                        drpMediaType.SelectedValue = "";
                        plcInfo.Visible = true;
                        lblInfo.ResourceString = "dialogs.web.selecttype";
                        break;
                }
            }

            if (source != null)
            {
                SetLastType(source.MediaType);
            }

            ShowProperties();
        }
    }


    /// <summary>
    /// Gets image dimensions from external URL.
    /// </summary>
    private void GetExternalImageDimensions(string imageUrl)
    {
        try
        {
            WebClient wc = new WebClient();
            byte[] img = wc.DownloadData(imageUrl);
            wc.Dispose();

            var ih = new ImageHelper(img);

            if (ih.ImageWidth > 0)
            {
                mWidth = ih.ImageWidth;
            }
            if (ih.ImageHeight > 0)
            {
                mHeight = ih.ImageHeight;
            }
        }
        catch
        {
        }
    }


    protected void drpMediaType_SelectedIndexChanged(object sender, EventArgs e)
    {
        MediaTypeEnum type = MediaTypeEnum.Unknown;
        switch (drpMediaType.SelectedValue.ToLowerCSafe())
        {
            case "image":
                // Get image dimensions from external source
                GetExternalImageDimensions(txtUrl.Text.Trim());

                type = MediaTypeEnum.Image;
                break;

            case "av":
                type = MediaTypeEnum.AudioVideo;
                break;
        }

        SetLastType(type);
        ShowProperties();
    }


    protected void hdnButtonUrl_Click(object sender, EventArgs e)
    {
        Hashtable properties = new Hashtable();
        properties[DialogParameters.URL_URL] = txtUrl.Text.Trim();
        properties[DialogParameters.EDITOR_CLIENTID] = Config.EditorClientID;
        drpMediaType.SelectedValue = "";
        ItemProperties.LoadProperties(properties);
    }


    protected void hdnButton_Click(object sender, EventArgs e)
    {
        Hashtable properties = GetSelectedItem();
        string script;
        if (Config.OutputFormat == OutputFormatEnum.URL)
        {
            properties[DialogParameters.URL_URL] = txtUrl.Text.Trim();
            properties[DialogParameters.EDITOR_CLIENTID] = Config.EditorClientID;
            properties[DialogParameters.IMG_ALT] = txtAlt.Text.Trim();
            properties[DialogParameters.IMG_ALT_CLIENTID] = QueryHelper.GetString(DialogParameters.IMG_ALT_CLIENTID, String.Empty);
            script = CMSDialogHelper.GetUrlItem(properties);
        }
        else
        {
            switch (drpMediaType.SelectedValue)
            {
                case "image":
                    script = CMSDialogHelper.GetImageItem(properties);
                    break;

                case "av":
                    script = CMSDialogHelper.GetAVItem(properties);
                    break;

                default:
                    script = CMSDialogHelper.GetUrlItem(properties);
                    break;
            }
        }
        if (!String.IsNullOrEmpty(script))
        {
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "insertItemScript", ScriptHelper.GetScript(script));
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Shows correct properties according to the settings.
    /// </summary>
    private void ShowProperties()
    {
        ItemProperties.Config = Config;

        // Save session data before shoving properties
        Hashtable dialogParameters = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
        if (dialogParameters != null)
        {
            dialogParameters = (Hashtable)dialogParameters.Clone();
        }

        DisplayProperties();

        MediaItem mi = new MediaItem();
        mi.Url = txtUrl.Text;
        if (mWidth > 0)
        {
            mi.Width = mWidth;
        }
        if (mHeight > 0)
        {
            mi.Height = mHeight;
        }

        // Try get source type from URL extension
        string ext = null;
        int index = txtUrl.Text.LastIndexOfCSafe('.');
        if (index > 0)
        {
            ext = txtUrl.Text.Substring(index);
        }

        if (Config.OutputFormat == OutputFormatEnum.HTMLMedia)
        {
            switch (drpMediaType.SelectedValue)
            {
                case "image":
                    propMedia.ViewMode = MediaTypeEnum.Image;
                    mi.Extension = String.IsNullOrEmpty(ext) ? "jpg" : ext;
                    break;

                case "av":
                    propMedia.ViewMode = MediaTypeEnum.AudioVideo;
                    mi.Extension = String.IsNullOrEmpty(ext) ? "avi" : ext;
                    break;

                default:
                    plcHTMLMediaProp.Visible = false;
                    break;
            }
            if (RequestHelper.IsPostBack())
            {
                ItemProperties.LoadSelectedItems(mi, dialogParameters);
            }
        }
        else if ((Config.OutputFormat == OutputFormatEnum.BBMedia) && (RequestHelper.IsPostBack()))
        {
            mi.Extension = String.IsNullOrEmpty(ext) ? "jpg" : ext;
            ItemProperties.LoadSelectedItems(mi, dialogParameters);
        }
        else if ((Config.OutputFormat == OutputFormatEnum.URL) && (RequestHelper.IsPostBack()))
        {
            ItemProperties.LoadSelectedItems(mi, dialogParameters);
        }
        // Set saved session data back into session
        if (dialogParameters != null)
        {
            SessionHelper.SetValue("DialogSelectedParameters", dialogParameters);
        }
    }


    /// <summary>
    /// Display panel of properties.
    /// </summary>
    private void DisplayProperties()
    {
        plcBBMediaProp.Visible = false;
        plcHTMLMediaProp.Visible = false;
        plcURLProp.Visible = false;

        switch (Config.OutputFormat)
        {
            case OutputFormatEnum.HTMLMedia:
                plcHTMLMediaProp.Visible = true;
                break;

            case OutputFormatEnum.BBMedia:
                plcBBMediaProp.Visible = true;
                break;

            default:
                plcURLProp.Visible = true;
                break;
        }
    }


    /// <summary>
    /// Update last type value in dialog selected parameters.
    /// </summary>
    /// <param name="type">Type</param>
    private void SetLastType(MediaTypeEnum type)
    {
        // Get selected prameters
        Hashtable dialogParameters = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable ?? new Hashtable();

        // Update last type
        dialogParameters[DialogParameters.LAST_TYPE] = type;
        SessionHelper.SetValue("DialogSelectedParameters", dialogParameters);
    }


    /// <summary>
    /// Initialize design jQuery scripts.
    /// </summary>
    private void InitializeDesignScripts()
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "designScript", ScriptHelper.GetScript("setTimeout('InitializeDesign();',200);$cmsj(window).resize(function() { InitializeDesign(); });"));
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Initializes its properties according to the URL parameters.
    /// </summary>
    public void InitFromQueryString()
    {
        switch (Config.OutputFormat)
        {
            case OutputFormatEnum.HTMLMedia:
                SelectableContent = SelectableContentEnum.OnlyMedia;
                break;

            case OutputFormatEnum.BBMedia:
                SelectableContent = SelectableContentEnum.OnlyImages;
                break;

            default:
                string content = QueryHelper.GetString("content", "");
                SelectableContent = content == "img" ? SelectableContentEnum.OnlyImages : SelectableContentEnum.AllContent;
                break;
        }
    }


    /// <summary>
    /// Returns selected item parameters as name-value collection.
    /// </summary>
    public Hashtable GetSelectedItem()
    {
        return ItemProperties.GetItemProperties();
    }


    /// <summary>
    /// Loads selected item parameters into the selector.
    /// </summary>
    /// <param name="properties">Name-value collection representing item to load</param>
    public void LoadSelectedItem(Hashtable properties)
    {
        if ((properties != null) && (properties.Count > 0))
        {
            Hashtable temp = (Hashtable)properties.Clone();

            if ((properties[DialogParameters.AV_URL] != null) && ((properties[DialogParameters.LAST_TYPE] == null) || ((MediaTypeEnum)properties[DialogParameters.LAST_TYPE] == MediaTypeEnum.AudioVideo)))
            {
                drpMediaType.SelectedValue = "av";
                txtUrl.Text = properties[DialogParameters.AV_URL].ToString();
            }
            else if ((properties[DialogParameters.IMG_URL] != null) && ((properties[DialogParameters.LAST_TYPE] == null) || ((MediaTypeEnum)properties[DialogParameters.LAST_TYPE] == MediaTypeEnum.Image)))
            {
                drpMediaType.SelectedValue = "image";

                // Update URL
                string url = ValidationHelper.GetString(properties[DialogParameters.IMG_URL], "");
                txtUrl.Text = url;

                // Get original dimensions
                GetExternalImageDimensions(url);

                temp[DialogParameters.IMG_ORIGINALWIDTH] = mWidth;
                temp[DialogParameters.IMG_ORIGINALHEIGHT] = mHeight;

            }
            else if ((properties[DialogParameters.URL_URL] != null) && ((properties[DialogParameters.LAST_TYPE] == null) || ((MediaTypeEnum)properties[DialogParameters.LAST_TYPE] == MediaTypeEnum.Unknown)))
            {
                txtUrl.Text = properties[DialogParameters.URL_URL].ToString();
            }
            if (!String.IsNullOrEmpty(txtUrl.Text))
            {
                ShowProperties();
                // Load temp properties because ShowProperties() change original properties
                ItemProperties.LoadItemProperties(temp);
            }
        }
    }

    #endregion
}
