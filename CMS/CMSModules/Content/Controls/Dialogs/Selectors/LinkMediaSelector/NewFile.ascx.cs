using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_NewFile : CMSUserControl
{
    #region "Variables"

    private MediaSourceEnum mSourceType = MediaSourceEnum.MediaLibraries;
    private Guid mFormGUID = Guid.Empty;
    private string mLibraryFolderPath = String.Empty;
    private bool mFileSystemActionsEnabled = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if file system actions are enabled.
    /// </summary>
    public bool FileSystemActionsEnabled
    {
        get
        {
            return mFileSystemActionsEnabled;
        }
        set
        {
            mFileSystemActionsEnabled = value;
        }
    }


    /// <summary>
    /// Indicates if uploader is used for uploading files to file system (Doesn't mean media library file).
    /// </summary>
    public bool FileSystemMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets dialog configuration.
    /// </summary>
    public DialogConfiguration Config
    {
        get;
        set;
    }


    /// <summary>
    /// Selected source type.
    /// </summary>
    public MediaSourceEnum SourceType
    {
        get
        {
            return mSourceType;
        }
        set
        {
            mSourceType = value;
        }
    }


    /// <summary>
    /// Indicates whether the control is displayed as part of the copy/move dialog.
    /// </summary>
    public bool IsCopyMoveLinkDialog
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the current node.
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the parent node.
    /// </summary>
    public int ParentNodeID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the current library.
    /// </summary>
    public int LibraryID
    {
        get;
        set;
    }


    /// <summary>
    /// Folder path of the current library.
    /// </summary>
    public string LibraryFolderPath
    {
        get
        {
            return mLibraryFolderPath;
        }
        set
        {
            mLibraryFolderPath = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the document attachments are related to.
    /// </summary>
    public int DocumentID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets GUID of the form temporary attachments are related to.
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
    /// Class name of the document temporary attachments are related to.
    /// </summary>
    public string NodeClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Height of attachment.
    /// </summary>
    public int ResizeToHeight
    {
        get;
        set;
    }


    /// <summary>
    /// Width of attachment.
    /// </summary>
    public int ResizeToWidth
    {
        get;
        set;
    }


    /// <summary>
    /// Max side size of attachment.
    /// </summary>
    public int ResizeToMaxSideSize
    {
        get;
        set;
    }


    /// <summary>
    /// ID that, together with MetaFileObjectType and AttachmentCategory, specifies object the metafile is related to. 
    /// </summary>
    public int MetaFileObjectID
    {
        get;
        set;
    }


    /// <summary>
    /// Object type that, together with MetaFileObjectID and AttachmentCategory, specifies object the metafile is related to.
    /// </summary>
    public string MetaFileObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Object category that, together with MetaFileObjectID and MetaFileObjectType, specifies object the metafile is related to.
    /// </summary>
    public string MetaFileCategory
    {
        get;
        set;
    }


    /// <summary>
    /// Target folder path for physical files.
    /// </summary>
    public string TargetFolderPath
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets which files with extensions are allowed to be uploaded.
    /// </summary>
    public string AllowedExtensions
    {
        get;
        set;
    }


    /// <summary>
    /// Extension of a new file allowed to be created
    /// </summary>
    public string NewTextFileExtension
    {
        get;
        set;
    }


    /// <summary>
    /// Control identifier.
    /// </summary>
    protected string Identifier
    {
        get
        {
            String identifier = ViewState["Identifier"] as String;
            if (identifier == null)
            {
                ViewState["Identifier"] = identifier = Guid.NewGuid().ToString("N");
            }

            return identifier;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
        }
    }


    /// <summary>
    /// Setups controls.
    /// </summary>
    private void SetupControls()
    {
        if (FileSystemMode)
        {
            // New file button
            if (!String.IsNullOrEmpty(NewTextFileExtension))
            {
                plcNewFile.Visible = true;

                string query = "?identifier=" + Identifier;

                // New folder button
                WindowHelper.Remove(Identifier);

                Hashtable properties = new Hashtable();
                properties.Add("targetpath", TargetFolderPath);
                properties.Add("newfileextension", NewTextFileExtension);
                WindowHelper.Add(Identifier, properties);

                var url = UrlResolver.ResolveUrl("~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/EditTextFile.aspx") + query;
                url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url, false));

                btnNew.ToolTip = GetString("dialogs.actions.newfile.desc");
                btnNew.OnClientClick = String.Format("modalDialog('{0}', 'NewFile', 905, 688, null, true); return false;", url);
                btnNew.Text = GetString("general.create");

            }
            else
            {
                plcNewFile.Visible = false;
            }

            // Initialize file uploader
            fileUploader.SourceType = MediaSourceEnum.PhysicalFile;
            fileUploader.TargetFolderPath = TargetFolderPath;
            fileUploader.AllowedExtensions = AllowedExtensions;
            fileUploader.AfterSaveJavascript = "FSS_FilesUploaded";
        }
        else
        {
            // If attachments are being displayed and no document or form is specified - hide uploader
            if (!IsCopyMoveLinkDialog && (((SourceType != MediaSourceEnum.DocumentAttachments) && (SourceType != MediaSourceEnum.MetaFile))
                || ((SourceType == MediaSourceEnum.DocumentAttachments) && (Config.AttachmentDocumentID > 0 || Config.AttachmentFormGUID != Guid.Empty))
                || ((SourceType == MediaSourceEnum.MetaFile) && ((MetaFileObjectID > 0) && !string.IsNullOrEmpty(MetaFileObjectType) && !string.IsNullOrEmpty(MetaFileCategory)))))
            {
                // Initialize file uploader
                if (SourceType == MediaSourceEnum.MetaFile)
                {
                    fileUploader.ObjectID = MetaFileObjectID;
                    fileUploader.ObjectType = MetaFileObjectType;
                    fileUploader.Category = MetaFileCategory;

                    BaseInfo info = ProviderHelper.GetInfoById(MetaFileObjectType, MetaFileObjectID);

                    fileUploader.SiteID = info != null ? info.Generalized.ObjectSiteID : SiteContext.CurrentSiteID;
                }
                else
                {
                    fileUploader.DocumentID = DocumentID;
                    fileUploader.FormGUID = FormGUID;
                    fileUploader.NodeClassName = NodeClassName;
                    fileUploader.NodeParentNodeID = ((NodeID > 0) ? NodeID : ParentNodeID);
                    fileUploader.LibraryID = LibraryID;
                    fileUploader.LibraryFolderPath = LibraryFolderPath;
                    fileUploader.ResizeToHeight = ResizeToHeight;
                    fileUploader.ResizeToMaxSideSize = ResizeToMaxSideSize;
                    fileUploader.ResizeToWidth = ResizeToWidth;
                    fileUploader.CheckPermissions = true;
                }
                fileUploader.ParentElemID = CMSDialogHelper.GetMediaSource(SourceType);
                fileUploader.SourceType = SourceType;
                
            }
            else
            {
                plcDirectFileUploader.Visible = false;
                fileUploader.StopProcessing = true;
            }
        }

        // Initialize disabled button
        fileUploader.IsLiveSite = IsLiveSite;
        fileUploader.Text = GetString("general.upload");
        fileUploader.UploadMode = MultifileUploaderModeEnum.DirectMultiple;

        const string disableMenuItem =
@"function DisableNewFileBtn() {                                      
    $cmsj('#dialogsUploaderDiv').attr('style', 'display:none;');                                 
    $cmsj('#dialogsUploaderDisabledDiv').removeAttr('style');
}
";

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "disableMenuItem", ScriptHelper.GetScript(disableMenuItem));

        if (!FileSystemActionsEnabled)
        {
            // Disable file action
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "disableNewFile", ScriptHelper.GetScript("DisableNewFileBtn();"));
        }
    }

    
    /// <summary>
    /// Reloads data and update control.
    /// </summary>
    public void ReloadData()
    {
        SetupControls();

        fileUploader.ReloadData();

        pnlUpdate.Update();
    }

    #endregion
}
