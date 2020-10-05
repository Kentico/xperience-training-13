using System;
using System.Text;

using CMS.Base.Web.UI;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_Menu : CMSUserControl
{
    #region "Private variables"

    private MediaSourceEnum mSourceType = MediaSourceEnum.MediaLibraries;
    private bool mFileSystemActionsEnabled = true;
    private Guid mFormGUID = Guid.Empty;
    private string mLibraryFolderPath = String.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value which determines whether to show the Parent button or not.
    /// </summary>
    public bool ShowParentButton
    {
        get
        {
            return plcParentButton.Visible;
        }
        set
        {
            plcParentButton.Visible = value;
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
    /// ID of the parent node.
    /// </summary>
    public int ParentNodeID
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
    /// Directory info of media library required for external storage.
    /// </summary>
    public DirectoryInfo MediaLibraryDirectoryInfo
    {
        get;
        set;
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
    /// Indicates whether the control is displayed as part of the copy/move/link dialog.
    /// </summary>
    public bool IsCopyMoveLinkDialog
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
    /// Gets or sets dialog configuration.
    /// </summary>
    public DialogConfiguration Config
    {
        get;
        set;
    }


    /// <summary>
    /// Returns currently selected tab view mode.
    /// </summary>
    public DialogViewModeEnum SelectedViewMode
    {
        get
        {
            return menuView.SelectedViewMode;
        }
        set
        {
            menuView.SelectedViewMode = value;
        }
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
    /// ID of the current node.
    /// </summary>
    public int NodeID
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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize controls
            SetupControls();

            // Initialize ViewModeMenu
            if (SourceType == MediaSourceEnum.Content)
            {
                menuView.Visible = false;
                Visible = false;
            }
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// OnPreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Parent directory button
        if (ShowParentButton)
        {
            plcParentButton.Visible = true;
            btnParent.OnClientClick = String.Format("SetParentAction('{0}'); return false;", ParentNodeID > 0 ? ParentNodeID.ToString() : "");
        }
    }


    /// <summary>
    /// Reloads the View mode menu.
    /// </summary>
    public void UpdateViewMenu()
    {
        if (Visible)
        {
            SetupControls();

            // Initialize ViewModeMenu
            if (IsCopyMoveLinkDialog)
            {
                menuView.Visible = false;
            }

            NewFile.ReloadData();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Register modal dialog and header shadow scripts
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");

        NewFile.NodeID = NodeID;
        NewFile.ParentNodeID = ParentNodeID;
        NewFile.MetaFileCategory = MetaFileCategory;
        NewFile.MetaFileObjectID = MetaFileObjectID;
        NewFile.MetaFileObjectType = MetaFileObjectType;
        NewFile.FileSystemActionsEnabled = FileSystemActionsEnabled;
        NewFile.FileSystemMode = FileSystemMode;
        NewFile.LibraryFolderPath = LibraryFolderPath;
        NewFile.LibraryID = LibraryID;
        NewFile.IsCopyMoveLinkDialog = IsCopyMoveLinkDialog;
        NewFile.ResizeToHeight = ResizeToHeight;
        NewFile.ResizeToMaxSideSize = ResizeToMaxSideSize;
        NewFile.ResizeToWidth = ResizeToWidth;
        NewFile.SourceType = SourceType;
        NewFile.TargetFolderPath = TargetFolderPath;
        NewFile.NewTextFileExtension = NewTextFileExtension;
        NewFile.FormGUID = FormGUID;
        NewFile.NodeClassName = NodeClassName;
        NewFile.DocumentID = DocumentID;
        NewFile.Config = Config;
        NewFile.AllowedExtensions = AllowedExtensions;

        var displayPrepareForImport = !IsCopyMoveLinkDialog && (MediaLibraryDirectoryInfo != null) && StorageHelper.IsExternalStorage(MediaLibraryDirectoryInfo.FullName);
        btnPrepareForImport.Visible = displayPrepareForImport;

        if (displayPrepareForImport)
        {
            btnPrepareForImport.Enabled = MediaLibraryDirectoryInfo.Exists;
            btnPrepareForImport.ToolTipResourceString = btnPrepareForImport.Enabled ? null : "media.folder.prepareforimportnotavailable";
        }
    }

    #endregion
}
