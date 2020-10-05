using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_SelectFolder : CMSAdminControl
{
    #region "Variables"

    private int mMediaLibraryID = 0;
    private string mAction = null;
    private string mFolderPath = null;
    private string mFiles = null;
    private bool mAllFiles = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the media library to display.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return mMediaLibraryID;
        }
        set
        {
            mMediaLibraryID = value;
        }
    }


    /// <summary>
    /// Action control is displayed for.
    /// </summary>
    public string Action
    {
        get
        {
            return mAction;
        }
        set
        {
            mAction = value;
        }
    }


    /// <summary>
    /// Folder path of the files action is related to.
    /// </summary>
    public string FolderPath
    {
        get
        {
            return mFolderPath;
        }
        set
        {
            mFolderPath = value;
        }
    }


    /// <summary>
    /// Sets of file names action is related to.
    /// </summary>
    public string Files
    {
        get
        {
            return mFiles;
        }
        set
        {
            mFiles = value;
        }
    }


    /// <summary>
    /// Indicates whether all available files should be processed.
    /// </summary>
    public bool AllFiles
    {
        get
        {
            return mAllFiles;
        }
        set
        {
            mAllFiles = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            SetupControls();
        }
        else
        {
            mediaLibrary.StopProcessing = true;
            mediaLibrary.ShouldProcess = false;
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "redirect", ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }"));
        }
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Setup title
        InitializeTitle();

        mediaLibrary.IsLiveSite = IsLiveSite;
        mediaLibrary.ShouldProcess = true;
        mediaLibrary.LibraryID = MediaLibraryID;
        mediaLibrary.Action = Action;
        mediaLibrary.CopyMovePath = FolderPath;
        mediaLibrary.Files = Files;
        mediaLibrary.AllFiles = AllFiles;
    }


    #region "Private methods"

    /// <summary>
    /// Setup title according to action.
    /// </summary>
    private void InitializeTitle()
    {
        titleElem.IsLiveSite = IsLiveSite;
        titleElem.ShowCloseButton = !IsLiveSite;
        titleElem.ShowFullScreenButton = !IsLiveSite;
        if ((Files != "") || AllFiles)
        {
            if (Action == "copy")
            {
                titleElem.TitleText = GetString("media.tree.copyfiles");
            }
            else
            {
                titleElem.TitleText = GetString("media.tree.movefiles");
            }
        }
        else
        {
            if (Action == "copy")
            {
                titleElem.TitleText = GetString("media.tree.copyfolder");
            }
            else
            {
                titleElem.TitleText = GetString("media.tree.movefolder");
            }
        }
    }

    #endregion
}
