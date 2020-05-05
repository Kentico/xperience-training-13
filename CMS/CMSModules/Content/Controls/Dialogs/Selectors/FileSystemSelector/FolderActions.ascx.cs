using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_FolderActions : CMSAdminItemsControl
{
    #region "Public properties"

    /// <summary>
    /// Target folder path for physical files.
    /// </summary>
    public string TargetFolderPath
    {
        get;
        set;
    }


    /// <summary>
    /// Full file system starting path of dialog. Expects physical file system path with backslashes.
    /// </summary>
    public string FullStartingPath
    {
        get;
        set;
    }


    /// <summary>
    /// Enable delete folder button
    /// </summary>
    public bool EnableDeleteFolder
    {
        get
        {
            return btnDelete.Enabled;
        }
        set
        {
            btnDelete.Enabled = value;
        }
    }


    /// <summary>
    /// Enable add folder button
    /// </summary>
    public bool EnableAddFolder
    {
        get
        {
            return btnAdd.Enabled;
        }
        set
        {
            btnAdd.Enabled = value;
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

        btnDelete.Click += btnDelete_Click;

        if (!StopProcessing)
        {
            // Initialize controls
            SetupControls();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize nested controls
            InitializeMenu();
        }
        else
        {
            Visible = false;
        }
    }


    private void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            var path = Path.GetFullPath(TargetFolderPath);
            if (path.StartsWith(FullStartingPath, StringComparison.InvariantCultureIgnoreCase))
            {
                // Delete the folder
                Directory.Delete(path, true);

                // Refresh the tree
                string parentPath = Path.GetDirectoryName(path);

                string script = String.Format("SetParentAction({0})", ScriptHelper.GetString(parentPath));

                ScriptHelper.RegisterStartupScript(Page, typeof(string), "DeleteRefresh", ScriptHelper.GetScript(script));

                pnlUpdateSelectors.Update();
            }
            else
            {
                ShowError(GetString("dialogs.filesystem.invalidfilepath"));
            }
        }
        catch (Exception ex)
        {
            LogAndShowError("FileSystemSelector", "DELETEFOLDER", ex);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls and control itself.
    /// </summary>
    private void SetupControls()
    {
        // Register modal dialog handling script
        ScriptHelper.RegisterDialogScript(Page);

        // Setup buttons
        btnAdd.ToolTip = GetString("dialogs.actions.newfolder.desc");
        btnDelete.ToolTip = GetString("media.folder.delete");
    }


    /// <summary>
    /// Initializes menu.
    /// </summary>
    private void InitializeMenu()
    {
        string query = "?identifier=" + Identifier;
        string url = UrlResolver.ResolveUrl("~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/NewFolder.aspx") + query;
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url, false));

        btnAdd.ToolTip = GetString("dialogs.actions.newfolder.desc");
        btnAdd.OnClientClick = String.Format("modalDialog('{0}', 'NewFolder', 680, 200, null, true); return false;", url);

        // Delete folder button
        btnDelete.ToolTip = GetString("media.folder.delete");
        btnDelete.OnClientClick = String.Format("if (!confirm({0})) {{ return false; }} ", ScriptHelper.GetLocalizedString("General.ConfirmDelete"));

        // New folder button
        WindowHelper.Remove(Identifier);

        Hashtable props = new Hashtable();
        props.Add("targetpath", TargetFolderPath);

        WindowHelper.Add(Identifier, props);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads part of the menu providing file related actions.
    /// </summary>
    public void Update()
    {
        // Initialize actions menu
        InitializeMenu();

        pnlUpdateSelectors.Update();
    }

    #endregion
}
