using System;
using System.Collections;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_FolderActions : CMSAdminItemsControl
{
    /// <summary>
    /// Delegate for the events fired whenever some action occurs.
    /// </summary>
    public event OnActionEventHandler OnAction;


    #region "Variables"

    private string mDeleteScript = "";
    private bool mFileSystemActionsEnabled = true;
    private string mLibraryFolderPath = String.Empty;

    private const string MEDIA_LIBRARY_FOLDER = "~/CMSModules/MediaLibrary/";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Node ID
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// URL of the new media folder dialog.
    /// </summary>
    public string NewFolderDialogUrl
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
    /// Gets or sets dialog configuration.
    /// </summary>
    public DialogConfiguration Config
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
    /// Path to the folder currently processed.
    /// </summary>
    public string FolderPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FolderPath"], "");
        }
        set
        {
            ViewState["FolderPath"] = value;
        }
    }


    /// <summary>
    /// Currently processed library ID.
    /// </summary>
    public int LibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["LibraryID"], 0);
        }
        set
        {
            ViewState["LibraryID"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the DELETE action should be displayed.
    /// </summary>
    public bool DisplayDelete
    {
        get
        {
            return btnDelete.Visible;
        }
        set
        {
            btnDelete.Visible = value;
        }
    }


    /// <summary>
    /// Indicates whether the COPY action should be displayed.
    /// </summary>
    public bool DisplayCopy
    {
        get
        {
            return btnCopy.Visible;
        }
        set
        {
            btnCopy.Visible = value;
        }
    }


    /// <summary>
    /// Indicates whether the copy action is enabled.
    /// </summary>
    public bool CopyEnabled
    {
        get
        {
            return btnCopy.Enabled;
        }
        set
        {
            btnCopy.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates whether the MOVE action should be displayed.
    /// </summary>
    public bool DisplayMove
    {
        get
        {
            return btnMove.Visible;
        }
        set
        {
            btnMove.Visible = value;
        }
    }


    /// <summary>
    /// JavaScript called when Delete button is clicked. If specified no postback is raised.
    /// </summary>
    public string DeleteScript
    {
        get
        {
            return mDeleteScript;
        }
        set
        {
            mDeleteScript = value;
        }
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


    #region "Event handlers"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RaiseOnCheckPermissions(PERMISSION_READ, this);
        btnDelete.Click += btnDelete_Click;

        // Initialize actions menu
        InitializeMenu();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize nested controls
            SetupControl();
        }
        else
        {
            Visible = false;
        }
    }


    void btnDelete_Click(object sender, EventArgs e)
    {
        RaiseOnActionEvent("delete");
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls and control itself.
    /// </summary>
    private void SetupControl()
    {
        // Register modal dialog handling script
        ScriptHelper.RegisterDialogScript(Page);

        // Setup buttons
        btnAdd.ToolTip = GetString("dialogs.actions.newfolder.desc");
        btnCopy.ToolTip = GetString("media.tree.copyfolder");
        btnMove.ToolTip = GetString("media.tree.movefolder");
        btnDelete.ToolTip = GetString("media.folder.delete");

        // If delete script is set
        if (!string.IsNullOrEmpty(DeleteScript))
        {
            // Register delete script
            btnDelete.OnClientClick = DeleteScript.Replace("##FOLDERPATH##", Path.EnsureSlashes(FolderPath).Replace("'", "\\'")) + "return false;";
        }

        // If folder path is set
        if (String.IsNullOrEmpty(FolderPath))
        {
            // Disable delete action
            btnDelete.Enabled = false;

            // Disable move action
            btnMove.Enabled = false;
        }

        if (!FileSystemActionsEnabled)
        {
            // Disable file and folder actions
            btnAdd.Enabled = false;
        }

        if (!IsLiveSite)
        {
            Config = DialogConfiguration.GetDialogConfiguration();
            if (Config != null)
            {
                switch (Config.CustomFormatCode.ToLowerCSafe())
                {
                    case "link":
                        btnAdd.Visible = false;
                        break;

                    case "linkdoc":
                        btnAdd.Visible = false;
                        break;

                    case "relationship":
                        btnAdd.Visible = false;
                        break;

                    case "selectpath":
                        btnAdd.Visible = false;
                        break;
                }
            }
        }

        string disableMenuItem = @"
function DisableNewFolderBtn() { 
    $cmsj('#' + '" + btnAdd.ClientID + @"').attr('disabled', 'disabled');
}";

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "disableMenuItem", ScriptHelper.GetScript(disableMenuItem));
    }


    /// <summary>
    /// Initializes menu.
    /// </summary>
    private void InitializeMenu()
    {
        WindowHelper.Remove(Identifier);

        Hashtable props = new Hashtable();
        props.Add("libraryid", LibraryID);
        props.Add("path", LibraryFolderPath);
        props.Add("cancel", false);

        WindowHelper.Add(Identifier, props);

        const string MEDIA_FORMCONTROLS_FOLDER = MEDIA_LIBRARY_FOLDER + "FormControls/";

        if (IsLiveSite)
        {
            if (AuthenticationHelper.IsAuthenticated())
            {
                NewFolderDialogUrl = "~/CMS/Dialogs/CMSModules/MediaLibrary/FormControls/LiveSelectors/InsertImageOrMedia/NewMediaFolder.aspx?identifier=" + Identifier;
            }
            else
            {
                NewFolderDialogUrl = MEDIA_FORMCONTROLS_FOLDER + "LiveSelectors/InsertImageOrMedia/NewMediaFolder.aspx?identifier=" + Identifier;
            }
        }
        else
        {
            NewFolderDialogUrl = MEDIA_FORMCONTROLS_FOLDER + "Selectors/InsertImageOrMedia/NewMediaFolder.aspx?identifier=" + Identifier;
        }

        // Add security hash
        NewFolderDialogUrl = URLHelper.AddParameterToUrl(NewFolderDialogUrl, "hash", QueryHelper.GetHash(NewFolderDialogUrl, false));

        btnAdd.OnClientClick = "modalDialog('" + UrlResolver.ResolveUrl(NewFolderDialogUrl) + "', 'NewFolder', 680, 200, null, true); return false;";
    }


    /// <summary>
    /// Fires the OnAction event.
    /// </summary>
    /// <param name="actionName">Name of the action that takes place</param>
    private void RaiseOnActionEvent(string actionName)
    {
        // Let other controls now the action takes place
        if (OnAction != null)
        {
            OnAction(actionName, FolderPath);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        string url = 
            (IsLiveSite) ? 
            MEDIA_LIBRARY_FOLDER + "CMSPages/SelectFolder.aspx" :
            MEDIA_LIBRARY_FOLDER + "Tools/FolderActions/SelectFolder.aspx";

        WindowHelper.Remove(Identifier);

        Hashtable props = new Hashtable();
        props.Add("path", FolderPath);
        props.Add("libraryid", LibraryID);

        WindowHelper.Add(Identifier, props);

        // Add query into url
        url += "?action={0}&identifier=" + Identifier;

        // Create copy and move url
        string copyUrl = String.Format(url, "copy");
        string moveUrl = String.Format(url, "move");

        // Add security hash to urls
        copyUrl = URLHelper.AddParameterToUrl(copyUrl, "hash", QueryHelper.GetHash(copyUrl, false));
        moveUrl = URLHelper.AddParameterToUrl(moveUrl, "hash", QueryHelper.GetHash(moveUrl, false));

        // Register modal dialogs
        btnCopy.OnClientClick = String.Format("modalDialog('{0}', 'CopyFolder', '90%', '70%'); return false;", ResolveUrl(copyUrl));
        if (!String.IsNullOrEmpty(FolderPath))
        {
            btnMove.OnClientClick = String.Format("modalDialog('{0}', 'MoveFolder', '90%', '70%'); return false;", ResolveUrl(moveUrl));
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads the menu.
    /// </summary>
    public void Update()
    {
        // Initialize actions menu
        InitializeMenu();

        pnlUpdateSelectors.Update();
    }

    #endregion
}
