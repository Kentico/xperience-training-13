using System;
using System.Text;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Attachments_DirectFileUploader_DirectFileUploader : DirectFileUploader
{
    #region "Constants"

    /// <summary>
    /// Storage key used for direct file uploader.
    /// </summary>
    private const string DIRECT_FILE_UPLOADER_STORAGE_KEY = "DirectFileUploader";

    #endregion


    #region "Variables"

    private bool mEnabled = true;
    private string mIFrameUrl;
    private MediaSourceEnum mSourceType = MediaSourceEnum.DocumentAttachments;

    #endregion


    #region "Properties"

    /// <summary>
    /// Provides access to properties of inner controls necessary for extensibility.
    /// </summary>
    public override IUploadHandler UploadHandler
    {
        get
        {
            return mfuDirectUploader;
        }
    }


    /// <summary>
    /// Gets or sets the style of the button.
    /// Button is not rendered at all when <see cref="ShowIconMode"/> is true. Therefore this property has no effect.
    /// </summary>
    public override ButtonStyle ButtonStyle
    {
        get
        {
            return btnUpload.ButtonStyle;
        }
        set
        {
            btnUpload.ButtonStyle = value;
        }
    }


    /// <summary>
    /// Gets or sets type of the content uploaded by the control.
    /// </summary>
    public override MediaSourceEnum SourceType
    {
        get
        {
            return mSourceType;
        }
        set
        {
            mSourceType = value;
            mfuDirectUploader.SourceType = SourceType;
        }
    }


    /// <summary>
    /// Gets or sets node site name.
    /// </summary>
    public override string IFrameUrl
    {
        get
        {
            return mIFrameUrl ?? (mIFrameUrl = GetIframeUrl(containerDiv.ClientID, uploaderFrame.ClientID));
        }
    }


    /// <summary>
    /// Indicates whether the control is displayed.
    /// </summary>
    public override bool Visible
    {
        get
        {
            return base.Visible;
        }
        set
        {
            base.Visible = value;
            uploaderFrame.Visible = value;
        }
    }


    /// <summary>
    /// Indicates whether uploader is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Indicates if advanced (HTML5) uploader should be used. If false, only alternative content is rendered.
    /// Default value is true.
    /// </summary>
    public bool EnableAdvancedUploader
    {
        get
        {
            return mfuDirectUploader.EnableAdvancedUploader;
        }
        set
        {
            mfuDirectUploader.EnableAdvancedUploader = value;
        }
    }


    /// <summary>
    /// Control key.
    /// </summary>
    private string ControlKey
    {
        get
        {
            return String.Format("{0}_{1}", ControlGroup, ParentElemID);
        }
    }


    /// <summary>
    /// Iframe CSS class.
    /// </summary>
    private string IFrameCSSClass
    {
        get
        {
            return "DFUframe_" + ControlGroup;
        }
    }


    /// <summary>
    /// GUID of attachment group.
    /// </summary>
    public override Guid AttachmentGroupGUID
    {
        get
        {
            return base.AttachmentGroupGUID;
        }
        set
        {
            base.AttachmentGroupGUID = value;
            mfuDirectUploader.AttachmentGroupGUID = AttachmentGroupGUID;
        }
    }


    /// <summary>
    /// Name of document attachment column.
    /// </summary>
    public override string AttachmentGUIDColumnName
    {
        get
        {
            return base.AttachmentGUIDColumnName;
        }
        set
        {
            base.AttachmentGUIDColumnName = value;
            mfuDirectUploader.AttachmentGUIDColumnName = AttachmentGUIDColumnName;
        }
    }


    /// <summary>
    /// GUID of attachment.
    /// </summary>
    public override Guid AttachmentGUID
    {
        get
        {
            return base.AttachmentGUID;
        }
        set
        {
            base.AttachmentGUID = value;
            mfuDirectUploader.AttachmentGUID = AttachmentGUID;
        }
    }


    /// <summary>
    /// Indicates if full refresh should be performed after uploading attachments under workflow
    /// </summary>
    public override bool FullRefresh
    {
        get
        {
            return base.FullRefresh;
        }
        set
        {
            base.FullRefresh = value;
            mfuDirectUploader.FullRefresh = value;
        }
    }


    /// <summary>
    /// Width of attachment.
    /// </summary>
    public override int ResizeToWidth
    {
        get
        {
            return base.ResizeToWidth;
        }
        set
        {
            base.ResizeToWidth = value;
            mfuDirectUploader.ResizeToWidth = ResizeToWidth;
        }
    }


    /// <summary>
    /// Height of attachment.
    /// </summary>
    public override int ResizeToHeight
    {
        get
        {
            return base.ResizeToHeight;
        }
        set
        {
            base.ResizeToHeight = value;
            mfuDirectUploader.ResizeToHeight = ResizeToHeight;
        }
    }


    /// <summary>
    /// Maximum side size of attachment.
    /// </summary>
    public override int ResizeToMaxSideSize
    {
        get
        {
            return base.ResizeToMaxSideSize;
        }
        set
        {
            base.ResizeToMaxSideSize = value;
            mfuDirectUploader.ResizeToMaxSideSize = ResizeToMaxSideSize;
        }
    }


    /// <summary>
    /// GUID of form.
    /// </summary>
    public override Guid FormGUID
    {
        get
        {
            return base.FormGUID;
        }
        set
        {
            base.FormGUID = value;
            mfuDirectUploader.FormGUID = FormGUID;
        }
    }


    /// <summary>
    /// ID of document.
    /// </summary>
    public override int DocumentID
    {
        get
        {
            return base.DocumentID;
        }
        set
        {
            base.DocumentID = value;
            mfuDirectUploader.DocumentID = DocumentID;
        }
    }


    /// <summary>
    /// Parent node ID.
    /// </summary>
    public override int NodeParentNodeID
    {
        get
        {
            return base.NodeParentNodeID;
        }
        set
        {
            base.NodeParentNodeID = value;
            mfuDirectUploader.DocumentParentNodeID = NodeParentNodeID;
        }
    }


    /// <summary>
    /// Document class name.
    /// </summary>
    public override string NodeClassName
    {
        get
        {
            return base.NodeClassName;
        }
        set
        {
            base.NodeClassName = value;
            mfuDirectUploader.NodeClassName = NodeClassName;
        }
    }


    /// <summary>
    /// Current site name.
    /// </summary>
    public override string NodeSiteName
    {
        get
        {
            return base.NodeSiteName;
        }
        set
        {
            base.NodeSiteName = value;
            mfuDirectUploader.NodeSiteName = NodeSiteName;
        }
    }


    /// <summary>
    /// Indicates if control is in insert mode (only new attachments are added, no update).
    /// </summary>
    public override bool InsertMode
    {
        get
        {
            return base.InsertMode;
        }
        set
        {
            base.InsertMode = value;
            mfuDirectUploader.IsInsertMode = InsertMode;
        }
    }


    /// <summary>
    /// Gets or sets which files with extensions are allowed to be uploaded.
    /// </summary>
    public override string AllowedExtensions
    {
        get
        {
            return base.AllowedExtensions;
        }
        set
        {
            base.AllowedExtensions = value;
            mfuDirectUploader.AllowedExtensions = AllowedExtensions;
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
            base.IsLiveSite = value;
            mfuDirectUploader.IsLiveSite = IsLiveSite;
        }
    }


    /// <summary>
    /// Indicates if only images is allowed for upload.
    /// </summary>
    public override bool OnlyImages
    {
        get
        {
            return base.OnlyImages;
        }
        set
        {
            base.OnlyImages = value;
            mfuDirectUploader.OnlyImages = OnlyImages;
        }
    }


    /// <summary>
    /// ID of the current library.
    /// </summary>
    public override int LibraryID
    {
        get
        {
            return base.LibraryID;
        }
        set
        {
            base.LibraryID = value;
            mfuDirectUploader.MediaLibraryID = LibraryID;
        }
    }


    /// <summary>
    /// Folder path of the current library.
    /// </summary>
    public override string LibraryFolderPath
    {
        get
        {
            return base.LibraryFolderPath;
        }
        set
        {
            base.LibraryFolderPath = value;
            mfuDirectUploader.MediaFolderPath = LibraryFolderPath;
        }
    }


    /// <summary>
    /// Media file name.
    /// </summary>
    public override string MediaFileName
    {
        get
        {
            return base.MediaFileName;
        }
        set
        {
            base.MediaFileName = value;
            mfuDirectUploader.MediaFileName = MediaFileName;
        }
    }


    /// <summary>
    /// Determines whether the uploader should upload media file thumbnail or basic media file.
    /// </summary>
    public override bool IsMediaThumbnail
    {
        get
        {
            return base.IsMediaThumbnail;
        }
        set
        {
            base.IsMediaThumbnail = value;
            mfuDirectUploader.IsMediaThumbnail = IsMediaThumbnail;
        }
    }


    /// <summary>
    /// ID of the media file.
    /// </summary>
    public override int MediaFileID
    {
        get
        {
            return base.MediaFileID;
        }
        set
        {
            base.MediaFileID = value;
            mfuDirectUploader.MediaFileID = MediaFileID;
        }
    }


    /// <summary>
    /// JavaScript function name called after save of new file.
    /// </summary>
    public override string AfterSaveJavascript
    {
        get
        {
            return base.AfterSaveJavascript;
        }
        set
        {
            base.AfterSaveJavascript = value;
            mfuDirectUploader.AfterSaveJavascript = AfterSaveJavascript;
        }
    }


    /// <summary>
    /// ID of parent element.
    /// </summary>
    public override string ParentElemID
    {
        get
        {
            return base.ParentElemID;
        }
        set
        {
            base.ParentElemID = value;
            mfuDirectUploader.ParentElemID = ParentElemID;
        }
    }


    /// <summary>
    /// Target folder path, to which physical files will be uploaded.
    /// </summary>
    public override string TargetFolderPath
    {
        get
        {
            return base.TargetFolderPath;
        }
        set
        {
            base.TargetFolderPath = value;
            mfuDirectUploader.TargetFolderPath = TargetFolderPath;
        }
    }


    /// <summary>
    /// Target file name, to which will be used after files will be uploaded.
    /// </summary>
    public override string TargetFileName
    {
        get
        {
            return base.TargetFileName;
        }
        set
        {
            base.TargetFileName = value;
            mfuDirectUploader.TargetFileName = TargetFileName;
        }
    }


    /// <summary>
    /// Upload mode for the uploader application.
    /// </summary>
    public override MultifileUploaderModeEnum UploadMode
    {
        get
        {
            return base.UploadMode;
        }
        set
        {
            base.UploadMode = value;
            mfuDirectUploader.UploadMode = UploadMode;
        }
    }


    /// <summary>
    /// Indicates whether the post-upload JavaScript function call should include created attachment GUID information.
    /// </summary>
    public override bool IncludeNewItemInfo
    {
        get
        {
            return base.IncludeNewItemInfo;
        }
        set
        {
            base.IncludeNewItemInfo = value;
            mfuDirectUploader.IncludeNewItemInfo = IncludeNewItemInfo;
        }
    }


    /// <summary>
    /// Indicates if supported browser.
    /// </summary>
    public override bool RaiseOnClick
    {
        get
        {
            return base.RaiseOnClick;
        }
        set
        {
            base.RaiseOnClick = value;
            mfuDirectUploader.RaiseOnClick = RaiseOnClick;
        }
    }


    /// <summary>
    /// Value indicating whether multiselect is enabled in the open file dialog window.
    /// </summary>
    public override bool Multiselect
    {
        get
        {
            return base.Multiselect;
        }
        set
        {
            base.Multiselect = value;
            mfuDirectUploader.Multiselect = Multiselect;
        }
    }


    /// <summary>
    /// Max number of possible upload files.
    /// </summary>
    public override int MaxNumberToUpload
    {
        get
        {
            return base.MaxNumberToUpload;
        }
        set
        {
            base.MaxNumberToUpload = value;
            mfuDirectUploader.MaxNumberToUpload = MaxNumberToUpload;
        }
    }


    /// <summary>
    /// Site ID form metafile upload.
    /// </summary>
    public override int SiteID
    {
        get
        {
            return base.SiteID;
        }
        set
        {
            base.SiteID = value;
            mfuDirectUploader.SiteID = SiteID;
        }
    }


    /// <summary>
    /// Category/Group for uploaded metafile.
    /// </summary>
    public override string Category
    {
        get
        {
            return base.Category;
        }
        set
        {
            base.Category = value;
            mfuDirectUploader.Category = Category;
        }
    }


    /// <summary>
    /// Metafile ID for reupload.
    /// </summary>
    public override int MetaFileID
    {
        get
        {
            return base.MetaFileID;
        }
        set
        {
            base.MetaFileID = value;
            mfuDirectUploader.MetaFileID = MetaFileID;
        }
    }


    /// <summary>
    /// Object ID for metafile upload.
    /// </summary>
    public override int ObjectID
    {
        get
        {
            return base.ObjectID;
        }
        set
        {
            base.ObjectID = value;
            mfuDirectUploader.ObjectID = ObjectID;
        }
    }


    /// <summary>
    /// Object type for metafile upload.
    /// </summary>
    public override string ObjectType
    {
        get
        {
            return base.ObjectType;
        }
        set
        {
            base.ObjectType = value;
            mfuDirectUploader.ObjectType = ObjectType;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            ScriptHelper.RegisterJQuery(Page);
            ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.js");

            if (!RequestHelper.IsPostBack() || ForceLoad)
            {
                ReloadData();
            }

            if (ControlGroup != null)
            {
                // First instance of the control is loaded
                RequestStockHelper.AddToStorage(DIRECT_FILE_UPLOADER_STORAGE_KEY, ControlKey, true, true);

                string script = String.Format(@"
function DFULoadIframes_{0}() {{
    if (window.File && window.FileReader && window.FileList && window.Blob) {{
        return;
    }}
    var iframe = document.getElementById('{1}');
    if (iframe!=null) {{
        iframe.setAttribute('allowTransparency','true');
        if (window.DFUframes != null) {{
            var iframes = $cmsj('iframe.{2}');
            for(var i = 0; i < iframes.length; i++) {{
                var f = iframes[i];
                var p = f.parentNode.parentNode;
                var imgs = p.getElementsByTagName('img');
                if ((imgs != null) && (imgs[0] != null)) {{
                    p.removeChild(imgs[0]);
                }}
                var o = null;
                var cw = iframe.contentWindow;
                if (cw != null)
                {{ 
                    var cwd = cw.document;
                    if (cwd != null) {{
                        var cn = cwd.childNodes;
                        if ((cn != null) && (cn.length > 0) && (cn[1].innerHTML != null)) {{
                            var containerId = DFUframes[f.id].match(/containerid=([^&]+)/i)[1];
                    
                            o = cn[1].innerHTML;
                            o = o.replace(/action=[^\\s]+/, 'action=""' + DFUframes[f.id] + '""').replace('{3}','');
                            o = o.replace(/(\.\.\/)+App_Themes\//ig, '{4}App_Themes/');
                            o = o.replace(/OnUploadBegin\('[^']+'\)/ig, 'OnUploadBegin(\'' + containerId + '\')');

                            var cd = f.contentWindow.document;
                            cd.write(o);
                            cd.close();

                            f.style.display = '';
                            f.setAttribute('allowTransparency','true');
                        }}
                    }}
                }}
            }}
        }}
    }}
}}", ControlKey, uploaderFrame.ClientID, IFrameCSSClass, ERROR_FUNCTION, ResolveUrl("~"));

                RegisterScript("DFUIframesLoader_" + ControlKey, script);
            }
        }
    }


    /// <summary>
    /// Loads data of the control.
    /// </summary>
    public override void ReloadData()
    {
        mIFrameUrl = null;
        LoadIFrame();

        // Initialize design
        InitDesign();

        mfuDirectUploader.ContainerID = containerDiv.ClientID;
        mfuDirectUploader.OnUploadBegin = "DFU.OnUploadBegin";
        mfuDirectUploader.OnUploadCompleted = "DFU.OnUploadCompleted";

        // Display progress only for UI
        if (!IsLiveSite || ShowProgress)
        {
            mfuDirectUploader.OnUploadProgressChanged = "DFU.OnUploadProgressChanged";
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads inner IFrame content.
    /// </summary>
    private void LoadIFrame()
    {
        // Set iframe attributes
        if ((ControlGroup == null) || !ValidationHelper.GetBoolean(RequestStockHelper.GetItem(DIRECT_FILE_UPLOADER_STORAGE_KEY, ControlKey), false))
        {
            uploaderFrame.Attributes.Add("src", ResolveUrl(IFrameUrl));

            if (ControlGroup != null)
            {
                uploaderFrame.Attributes.Add("onload", String.Format("(function tryLoadDFU_{0}() {{ if (window.DFULoadIframes_{0}) {{ window.DFULoadIframes_{0}(); }} else {{ setTimeout(tryLoadDFU_{0}, 200); }} }})();", ControlKey));
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (RequestHelper.IsCallback() && BrowserHelper.IsIE())
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    uploaderFrame.Attributes.Add("allowTransparency", "true");
                }
                else
                {
                    string script = String.Format("var frameElem = document.getElementById('{0}'); if(frameElem) {{ frameElem.setAttribute('allowTransparency','true') }}", uploaderFrame.ClientID);
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "DFUTrans_" + ClientID, script, true);
                }
            }
        }
        else
        {
            uploaderFrame.Attributes.Add("style", "display:none;vertical-align:middle;");
            uploaderFrame.Attributes.Add("class", IFrameCSSClass);
            string script = String.Format("if(!window.DFUframes) {{ var DFUframes = new Object(); }}if(!window.DFUpanels) {{ var DFUpanels = new Object(); }}DFUframes['{0}'] = {1};",
                                          uploaderFrame.ClientID,
                                          ScriptHelper.GetString(ResolveUrl(IFrameUrl)));
            RegisterScript("DFUArrays_" + ClientID, script);
        }

        uploaderFrame.Attributes.Add("title", uploaderFrame.ID);
        uploaderFrame.Attributes.Add("name", uploaderFrame.ClientID);
    }


    /// <summary>
    /// Initialize design.
    /// </summary>
    private void InitDesign()
    {
        // Register css styles for uploader
        CssRegistration.RegisterCssBlock(Page, "dfu_" + containerDiv.ClientID, CreateCss(containerDiv.ClientID));

        bool isRTL = IsLiveSite ? CultureHelper.IsPreferredCultureRTL() : CultureHelper.IsUICultureRTL();

        // Prepare loading image
        imgLoading.Style.Add("float", isRTL ? "right" : "left");
        imgLoading.Attributes["title"] = GetString("tree.loading");

        // Loading css class
        lblProgress.CssClass = InnerLoadingElementClass;

        // Ensure nowrap on loading text
        pnlLoading.Style.Add("white-space", "nowrap;");
        pnlLoading.Style.Add("display", "none");

        // Decide between icon or text mode
        uploadIcon.Visible = ShowIconMode;
        btnUpload.Visible = !ShowIconMode;

        // Disable everything properly
        if (!Enabled)
        {
            btnUpload.Enabled = false;
            uploadIcon.Attributes["class"] += " icon-disabled";
            pnlInnerDiv.CssClass += " uploader-button-disabled";
        }

        uploaderFrame.Visible = Enabled;
        mfuDirectUploader.Visible = Enabled;

        // Inner div html and design
        if (!String.IsNullOrEmpty(Text))
        {
            btnUpload.Text = Text;
        }
        if (!String.IsNullOrEmpty(InnerElementClass))
        {
            pnlInnerDiv.CssClass += " " + InnerElementClass;
        }

        // Container div styles
        containerDiv.Style.Add("position", "relative");
        if (DisplayInline)
        {
            containerDiv.Style.Add("float", isRTL ? "right" : "left");
        }

        if (!String.IsNullOrEmpty(ControlGroup))
        {
            containerDiv.Attributes.Add("class", ControlGroup);
        }

        string initScript = String.Format("if (typeof(DFU) !== 'undefined') {{ $cmsj(function () {{DFU.initializeDesign({0});}}); }}", ScriptHelper.GetString(containerDiv.ClientID));

        RegisterScript("DFUInit_" + ClientID, initScript);
    }


    /// <summary>
    /// Registers script to the page. The script is enclosed in HTML script tag.
    /// </summary>
    /// <param name="key">A unique identifier for the script block</param>
    /// <param name="script">The registered script</param>
    private void RegisterScript(string key, string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), key, script, true);
    }

    #endregion
}
