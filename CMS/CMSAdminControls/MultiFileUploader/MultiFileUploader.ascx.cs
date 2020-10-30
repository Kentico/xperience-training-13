using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Membership;

public partial class CMSAdminControls_MultiFileUploader_MultiFileUploader : CMSUserControl, IUploadHandler
{
    #region "Constants"

    private const string PARAMETER_SEPARATOR = "|";

    #endregion


    #region "Variables"

    private bool? mEnabled;
    private int mResizeToWidth = -1;
    private int mResizeToHeight = -1;
    private int mResizeToMaxSideSize = -1;
    private MultifileUploaderModeEnum mUploadMode = MultifileUploaderModeEnum.DirectSingle;
    private MediaSourceEnum mSourceType = MediaSourceEnum.Web;

    private string mNodeSiteName;
    private string mUploadHandlerUrl;

    private Guid mAttachmentGroupGUID = Guid.Empty;
    private Guid mFormGUID = Guid.Empty;
    private bool mIsFiledAttachment;
    private Guid mAttachmentGUID = Guid.Empty;

    private long? mMaximumUpload;

    #endregion


    #region "General properties"

    /// <summary>
    /// Event fired when additional parameters are constructed. These parameters are passed to the upload handler.
    /// Can be used to add custom parameters.
    /// </summary>
    public event EventHandler<MfuAdditionalParameterEventArgs> AttachAdditionalParameters;


    /// <summary>
    /// Indicates whether the post-upload JavaScript function call should include information about new item.
    /// The default value is false.
    /// </summary>
    public bool IncludeNewItemInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets type of the content uploaded by the control.
    /// </summary>
    public virtual MediaSourceEnum SourceType
    {
        get
        {
            return mSourceType;
        }
        set
        {
            mSourceType = value;
            // Ensure field attachment flag
            if (value == MediaSourceEnum.Attachment)
            {
                IsFieldAttachment = true;
            }
        }
    }


    /// <summary>
    /// Unique ID of the control, for which the postback should be made after upload is finished.
    /// </summary>
    public string EventTarget
    {
        get;
        set;
    }


    /// <summary>
    /// Target folder path, to which physical files will be uploaded.
    /// </summary>
    public string TargetFolderPath
    {
        get;
        set;
    }


    /// <summary>
    /// Target file name, to which physical files will be uploaded.
    /// </summary>
    public string TargetFileName
    {
        get;
        set;
    }


    /// <summary>
    /// Target alias path, to which files will be uploaded.
    /// </summary>
    public string TargetAliasPath
    {
        get;
        set;
    }


    /// <summary>
    /// Target culture, to which files will be uploaded.
    /// </summary>
    public string TargetCulture
    {
        get;
        set;
    }


    /// <summary>
    /// ID of parent element.
    /// </summary>
    public string ParentElemID
    {
        get;
        set;
    }


    /// <summary>
    /// JavaScript function name called after save of new file.
    /// </summary>
    public string AfterSaveJavascript
    {
        get;
        set;
    }


    /// <summary>
    /// Uploader instance guid.
    /// </summary>
    private Guid InstanceGuid
    {
        get
        {
            object o = ViewState["InstanceGuid"] ?? Guid.NewGuid();
            return ValidationHelper.GetGuid(o, Guid.Empty);
        }
    }


    /// <summary>
    /// Indicates if only images is allowed for upload.
    /// The default value is false.
    /// </summary>
    public bool OnlyImages
    {
        get;
        set;
    }


    /// <summary>
    /// Upload mode for the MultiFileUploader JavaScript module.
    /// The default value is DirectSingle.
    /// </summary>
    public MultifileUploaderModeEnum UploadMode
    {
        get
        {
            return mUploadMode;
        }
        set
        {
            mUploadMode = value;
        }
    }


    /// <summary>
    /// Size of the upload chunk in bytes. If set to <c>0</c> the default value is used. 
    /// The default value is 4194304 bytes.
    /// </summary>
    public long UploadChunkSize
    {
        get;
        set;
    }


    /// <summary>
    /// Value of the maximum upload size of a single file in bytes.
    /// The default value is based on HTTP runtime web.config setting "maxRequestLength".
    /// </summary>
    public long MaximumUpload
    {
        get
        {
            if (mMaximumUpload == null)
            {
                var section = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
                
                mMaximumUpload = (section != null) ? section.MaxRequestLength * 1024 : 0;
            }

            return mMaximumUpload.GetValueOrDefault();
        }
        set
        {
            mMaximumUpload = value;
        }
    }


    /// <summary>
    /// Value of the maximum total upload size in bytes.
    /// The default value is 0 (no maximum total upload).
    /// </summary>
    public long MaximumTotalUpload
    {
        get;
        set;
    }


    /// <summary>
    /// Max number of possible upload files.
    /// The default value is 0 (no limit).
    /// </summary>
    public int MaxNumberToUpload
    {
        get;
        set;
    }


    /// <summary>
    /// Value indicating whether multiselect is enabled in the open file dialog window.
    /// The default value is false.
    /// </summary>
    public bool Multiselect
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets automatic image resize width.
    /// Value of setting CMSAutoResizeImageWidth is used as default value.
    /// </summary>
    public int ResizeToWidth
    {
        get
        {
            if (mResizeToWidth == -1)
            {
                mResizeToWidth = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAutoResizeImageWidth");
            }
            return mResizeToWidth;
        }
        set
        {
            mResizeToWidth = value;
        }
    }


    /// <summary>
    /// Gets or sets automatic image resize height.
    /// Value of setting CMSAutoResizeImageHeight is used as default value.
    /// </summary>
    public int ResizeToHeight
    {
        get
        {
            if (mResizeToHeight == -1)
            {
                mResizeToHeight = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAutoResizeImageHeight");
            }
            return mResizeToHeight;
        }
        set
        {
            mResizeToHeight = value;
        }
    }


    /// <summary>
    /// Gets or sets automatic image resize maximum side size.
    /// Value of setting CMSAutoResizeImageMaxSideSize is used as default value.
    /// </summary>
    public int ResizeToMaxSideSize
    {
        get
        {
            if (mResizeToMaxSideSize == -1)
            {
                mResizeToMaxSideSize = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAutoResizeImageMaxSideSize");
            }
            return mResizeToMaxSideSize;
        }
        set
        {
            mResizeToMaxSideSize = value;
        }
    }


    /// <summary>
    /// List of allowed extensions.
    /// </summary>
    public string AllowedExtensions
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the Upload handler's URL
    /// If not set or set to null, automatic value according to uploader mode is provided.
    /// Set to override automatic handler selection.
    /// </summary>
    public string UploadHandlerUrl
    {
        get
        {
            if (!String.IsNullOrEmpty(mUploadHandlerUrl))
            {
                return URLHelper.ResolveUrl(mUploadHandlerUrl);
            }

            string url;
            
            // Using different path for authenticated user to enforce authentication under AD
            var authenticatedHandlerPath = MembershipContext.AuthenticatedUser.IsPublic() ? "" : "/Authenticated";

            if (MediaLibraryID > 0)
            {
                url = $"~/CMSModules/MediaLibrary/CMSPages{authenticatedHandlerPath}/MultiFileUploader.ashx";
            }
            else
            {
                url = $"~/CMSModules/Content/CMSPages{authenticatedHandlerPath}/MultiFileUploader.ashx";
            }
            
            return URLHelper.ResolveUrl(url);
        }
        set
        {
            mUploadHandlerUrl = value;
        }
    }


    /// <summary>
    /// Value indicating whether control is in insert mode or not.
    /// Default is false.
    /// </summary>
    public bool IsInsertMode
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether some JavaScript should be raised on click event.
    /// Default value is false.
    /// </summary>
    public bool RaiseOnClick
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            if (mEnabled == null)
            {
                mEnabled = DirectoryHelper.CheckPermissions(UploaderHelper.TempPath, true, true, true, true);
            }

            return mEnabled.Value;
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
            return pnlUpload.Visible;
        }
        set
        {
            pnlUpload.Visible = value;
        }
    }


    /// <summary>
    /// HTML content which is displayed if browser doesn't support HTML5 uploader.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public virtual ControlCollection AlternateContent
    {
        get
        {
            return Controls;
        }
    }

    #endregion


    #region "Media Library Properties"

    /// <summary>
    /// ID of the media file.
    /// </summary>
    public int MediaFileID
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether the uploader should upload media file thumbnail or basic media file.
    /// Default value is false.
    /// </summary>
    public bool IsMediaThumbnail
    {
        get;
        set;
    }


    /// <summary>
    /// Media file name.
    /// </summary>
    public string MediaFileName
    {
        get;
        set;
    }


    /// <summary>
    /// Id of the media library.
    /// </summary>
    public int MediaLibraryID
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the media library folder.
    /// </summary>
    public string MediaFolderPath
    {
        get;
        set;
    }

    #endregion


    #region "File type properties"

    /// <summary>
    /// Current site name.
    /// </summary>
    public string NodeSiteName
    {
        get
        {
            return mNodeSiteName ?? (mNodeSiteName = SiteContext.CurrentSiteName);
        }
        set
        {
            mNodeSiteName = value;
        }
    }

    #endregion


    #region "Attachment properties"

    /// <summary>
    /// GUID of the attachment.
    /// </summary>
    public Guid AttachmentGUID
    {
        get
        {
            return mAttachmentGUID;
        }
        set
        {
            mAttachmentGUID = value;
        }
    }


    /// <summary>
    /// GUID of the attachment group.
    /// </summary>
    public Guid AttachmentGroupGUID
    {
        get
        {
            return mAttachmentGroupGUID;
        }
        set
        {
            mAttachmentGroupGUID = value;
        }
    }


    /// <summary>
    /// The name of the document attachment column.
    /// </summary>
    public string AttachmentGUIDColumnName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if full refresh should be performed after uploading attachments under workflow
    /// </summary>
    public bool FullRefresh
    {
        get;
        set;
    }

    /// <summary>
    /// GUID of the form.
    /// </summary>
    public Guid FormGUID
    {
        get
        {
            return mFormGUID;
        }
        set
        {
            mFormGUID = value;
        }
    }


    /// <summary>
    /// ID of the document.
    /// </summary>
    public int DocumentID
    {
        get;
        set;
    }


    /// <summary>
    /// Id of the parent node.
    /// </summary>
    public int DocumentParentNodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Document class name.
    /// </summary>
    public string NodeClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether the upload is for field attachment or not.
    /// </summary>
    public bool IsFieldAttachment
    {
        get
        {
            return mIsFiledAttachment;
        }
        set
        {
            mIsFiledAttachment = value;
        }
    }

    #endregion


    #region "MetaFile Properties"

    /// <summary>
    /// ID of the site, where to upload MetaFile.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Metafile ID for reupload.
    /// </summary>
    public int MetaFileID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the meta file parent object.
    /// </summary>
    public int ObjectID
    {
        get;
        set;
    }


    /// <summary>
    /// The object type of the metafile.
    /// </summary>
    public string ObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// The category of the metafile.
    /// </summary>
    public string Category
    {
        get;
        set;
    }

    #endregion


    #region "Javascript Properties"

    /// <summary>
    /// Container client Id passed to each upload javascript functions(OnUploadBegin, OnUploadProgressChanged, OnUploadCompleted) as first parameter.
    /// </summary>
    public string ContainerID
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the javascript function called when upload progress is changed.
    /// </summary>
    public string OnUploadProgressChanged
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the javascript function called when upload is finished.
    /// </summary>
    public string OnUploadCompleted
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the javascript function called before upload begins.
    /// </summary>
    public string OnUploadBegin
    {
        get;
        set;
    }

    #endregion


    #region "Page Events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlUpload.Attributes.Add("onclick", "document.getElementById('" + uploadFile.ClientID + "').click();");

        if ((UploadMode == MultifileUploaderModeEnum.DirectMultiple) || Multiselect)
        {
            uploadFile.Attributes.Add("multiple", "multiple");
        }

        var data = new
        {
            UploaderClientID = uploadFile.ClientID,
            InstanceGuid,
            ModeParameters = GetModeParameters(),
            AditionalParameters = GetAdditionalParameters(),
            MaxNumberToUpload,
            MaximumTotalUpload,
            MaximumTotalUploadString = (MaximumTotalUpload > 0) ? DataHelper.GetSizeString(MaximumTotalUpload) : "",
            MaximumUploadSize = MaximumUpload,
            MaximumUploadSizeString = (MaximumUpload > 0) ? DataHelper.GetSizeString(MaximumUpload) : "",
            UploadChunkSize = (UploadChunkSize > 0) ? UploadChunkSize : UploaderHelper.UPLOAD_CHUNK_SIZE,
            UploadMode,
            UploadPage = UploadHandlerUrl,
            ResizeArgs = string.Format("{0};{1};{2}", ResizeToWidth, ResizeToHeight, ResizeToMaxSideSize),
            AllowedExtensions = GetAllowedExtensions(),
            OnUploadBegin,
            OnUploadCompleted,
            OnUploadProgressChanged,
            ContainerID,
            OverlayClientID = pnlUpload.ClientID
        };

        LoadResources();
        ScriptHelper.RegisterModule(this, "AdminControls/MultiFileUploader", data);
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Prepares resource strings for error messages.
    /// </summary>
    private void LoadResources()
    {
        var resources = new List<String>
                                     {
                                         "multifileuploader.maxnumbertoupload",
                                         "multifileuploader.maxuploadamount",
                                         "multifileuploader.maxuploadsize",
                                         "multifileuploader.extensionnotallowed"
                                     };

        // Create javascript object with all resources
        var sb = new StringBuilder("window.MFUResources = {};\n");

        resources.ForEach(resource =>
                          sb.Append(String.Format("window.MFUResources['{0}'] = {1};\n",
                                                  resource,
                                                  ScriptHelper.GetString(ResHelper.GetString(resource)))
                              )
            );

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "MFUResources", sb.ToString(), true);
    }


    /// <summary>
    /// Returns arguments string with security hash.
    /// </summary>
    /// <param name="args">Arguments array</param>
    /// <param name="purpose">A unique string identifying the purpose of the hash string.</param>
    private static string GetArgumentsString(string[] args, string purpose)
    {
        string arguments = null;

        if ((args != null) && (args.Length > 0))
        {
            arguments = String.Join(PARAMETER_SEPARATOR, args);
        }

        if (!String.IsNullOrEmpty(arguments))
        {
            arguments = String.Format("{0}{1}Hash{1}{2}", arguments, PARAMETER_SEPARATOR, ValidationHelper.GetHashString(arguments, new HashSettings(purpose)));
        }

        return HttpUtility.UrlEncode(arguments);
    }


    /// <summary>
    /// Returns additional parameters needed by the handler.
    /// </summary>
    private string GetAdditionalParameters()
    {
        string[] args =
        {
            "SourceType", SourceType.ToString(),
            "ParentElementID", ParentElemID,
            "IsInsertMode", IsInsertMode.ToString(),
            "AfterSaveJavascript", AfterSaveJavascript,
            "TargetFolderPath", TargetFolderPath,
            "TargetFileName", TargetFileName,
            "IncludeNewItemInfo", IncludeNewItemInfo.ToString(),
            "OnlyImages", OnlyImages.ToString(),
            "RaiseOnClick", RaiseOnClick.ToString(),
            "TargetAliasPath", TargetAliasPath,
            "TargetCulture", TargetCulture,
            "EventTarget", EventTarget
        };

        if (AttachAdditionalParameters != null)
        {
            MfuAdditionalParameterEventArgs eventArgs = new MfuAdditionalParameterEventArgs();
            eventArgs.AddParameters(args);
            AttachAdditionalParameters(this, eventArgs);
            args = eventArgs.GetParameters();
        }


        return "AdditionalParameters=" + GetArgumentsString(args, UploaderHelper.ADDIOTIONAL_PARAMETERS_HASHING_PURPOSE);
    }


    /// <summary>
    /// Returns parameters according to the upload mode.
    /// </summary>
    private string GetModeParameters()
    {
        string[] args;

        if (MediaLibraryID > 0)
        {
            // MediaLibrary mode
            args = new[]
                       {
                           "MediaLibraryID", MediaLibraryID.ToString(),
                           "MediaFolderPath", MediaFolderPath,
                           "MediaFileID", MediaFileID.ToString(),
                           "IsMediaThumbnail", IsMediaThumbnail.ToString(),
                           "MediaFileName", MediaFileName
                       };
            return "MediaLibraryArgs=" + GetArgumentsString(args, UploaderHelper.MEDIA_LIBRARY_ARGS_HASHING_PURPOSE);
        }

        if (ObjectID > 0)
        {
            // MetaFile mode
            args = new[]
            {
                "MetaFileID", MetaFileID.ToString(),
                "ObjectID", ObjectID.ToString(),
                "SiteID", SiteID.ToString(),
                "ObjectType", ObjectType,
                "Category", Category
            };
            return "MetaFileArgs=" + GetArgumentsString(args, UploaderHelper.META_FILE_ARGS_HASHING_PURPOSE);
        }

        if ((DocumentID > 0) || (FormGUID != Guid.Empty))
        {
            // Attachment mode
            args = new[]
            {
                "DocumentID", DocumentID.ToString(),
                "DocumentParentNodeID", DocumentParentNodeID.ToString(),
                "NodeClassName", NodeClassName,
                "AttachmentGUIDColumnName", AttachmentGUIDColumnName,
                "AttachmentGUID", AttachmentGUID.ToString(),
                "AttachmentGroupGUID", AttachmentGroupGUID.ToString(),
                "FormGUID", FormGUID.ToString(),
                "IsFieldAttachment", mIsFiledAttachment.ToString(),
                "FullRefresh", FullRefresh.ToString()
            };
            return "AttachmentArgs=" + GetArgumentsString(args, UploaderHelper.ATTACHEMENT_ARGS_HASHING_PURPOSE);
        }
        return String.Empty;
    }


    /// <summary>
    /// Returns the allowed extensions with hash.
    /// </summary>
    private string GetAllowedExtensions()
    {
        string filter;

        if (String.IsNullOrEmpty(AllowedExtensions))
        {
            if (MediaLibraryID > 0)
            {
                filter = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSMediaFileAllowedExtensions");
            }
            else
            {
                filter = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSUploadExtensions");
            }
        }
        else
        {
            filter = AllowedExtensions;
        }

        if (!string.IsNullOrEmpty(filter))
        {
            // Append hash to list of allowed extensions
            string hash = ValidationHelper.GetHashString(filter, new HashSettings(UploaderHelper.ALLOWED_EXTENSIONS_HASHING_PURPOSE));
            filter = String.Format("{0}{1}Hash{1}{2}", filter, PARAMETER_SEPARATOR, hash);
        }

        return filter;
    }

    #endregion
}
