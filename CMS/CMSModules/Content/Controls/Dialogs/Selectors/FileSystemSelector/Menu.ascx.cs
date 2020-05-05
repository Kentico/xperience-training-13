using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_Menu : CMSUserControl
{
    #region "Public properties"

    /// <summary>
    /// If true, upload button is available
    /// </summary>
    public bool AllowNew
    {
        get
        {
            return NewFile.Visible;
        }
        set
        {
            NewFile.Visible = value;
        }
    }


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
    /// Gets or sets path of the parent of the currently selected node.
    /// </summary>
    public string NodeParentPath
    {
        get
        {
            return ValidationHelper.GetString(hdnLastNodeParentPath.Value, "");
        }
        set
        {
            hdnLastNodeParentPath.Value = value;
        }
    }

    
    /// <summary>
    /// Indicates if help icon should be hidden.
    /// </summary>
    public bool RemoveHelpIcon
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

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            // Initialize controls
            SetupControls();
        }
        else
        {
            Visible = false;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            // Initialize controls
            SetupControls();
        }
    }


    /// <summary>
    /// Reloads part of the menu providing file related actions.
    /// </summary>
    public void UpdateActionsMenu()
    {
        // Initialize actions menu
        SetupControls();

        // Apply updated information
        NewFile.ReloadData();
    }

    
    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        if (AllowNew)
        {
            NewFile.AllowedExtensions = AllowedExtensions;
            NewFile.TargetFolderPath = TargetFolderPath;
            NewFile.NewTextFileExtension = NewTextFileExtension;
            NewFile.SourceType = MediaSourceEnum.PhysicalFile;
            NewFile.IsLiveSite = IsLiveSite;
        }

        // Parent directory button
        if (ShowParentButton && !String.IsNullOrEmpty(NodeParentPath))
        {
            plcParentButton.Visible = true;
            btnParent.OnClientClick = String.Format("SelectNode('{0}');SetParentAction('{0}'); return false;", NodeParentPath.Replace("\\", "\\\\").Replace("'", "\\'"));
        }
    }

    #endregion
}