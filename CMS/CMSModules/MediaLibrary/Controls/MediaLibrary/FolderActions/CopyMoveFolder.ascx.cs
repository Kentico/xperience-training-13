using System;
using System.Collections;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_CopyMoveFolder_Control : CMSUserControl
{
    #region "Variables"

    private bool mIsLoad = true;
    private bool mPerformAction;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the media library.
    /// </summary>
    public int MediaLibraryID
    {
        get;
        set;
    }


    /// <summary>
    /// Type of the action.
    /// </summary>
    public string Action
    {
        get;
        set;
    }


    /// <summary>
    /// Media library Folder path.
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
    /// Path where the item(s) should be copied/moved.
    /// </summary>
    public string NewPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["NewPath"], "");
        }
        set
        {
            ViewState["NewPath"] = value;
        }
    }


    /// <summary>
    /// List of files to copy/move.
    /// </summary>
    public string Files
    {
        get;
        set;
    }


    /// <summary>
    /// Refresh script which is processed when the action is finished.
    /// </summary>
    public string RefreshScript
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether all files should be copied.
    /// </summary>
    public bool AllFiles
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the control is just loaded.
    /// </summary>
    public bool IsLoad
    {
        get
        {
            return mIsLoad;
        }
        set
        {
            mIsLoad = value;
        }
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    public string Identifier
    {
        get
        {
            string identifier = ValidationHelper.GetString(ViewState["Identifier"], null);
            if (identifier == null)
            {
                identifier = Guid.NewGuid().ToString("N");
                ViewState["Identifier"] = identifier;
            }

            return identifier;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        innerFrame.Attributes.Add("src", GetFrameUrl());
    }


    public void PerformAction()
    {
        mPerformAction = true;
        innerFrame.Attributes["src"] = GetFrameUrl();
    }


    /// <summary>
    /// Reloads control and its data.
    /// </summary>
    public void ReloadData()
    {
        innerFrame.Attributes["src"] = GetFrameUrl();
    }


    /// <summary>
    /// Returns correct URL for IFrame.
    /// </summary>
    private string GetFrameUrl()
    {
        string frameUrl = ResolveUrl("~/CMSModules/MediaLibrary/Controls/MediaLibrary/FolderActions/CopyMoveFolder.aspx");

        Hashtable props = new Hashtable();
        // Fill properties table
        props.Add("path", FolderPath);
        props.Add("libraryid", MediaLibraryID);
        props.Add("newpath", NewPath);
        props.Add("files", Files);
        props.Add("allFiles", AllFiles);
        props.Add("load", IsLoad);
        props.Add("performaction", mPerformAction);
        props.Add("action", Action);

        WindowHelper.Add(Identifier, props);

        frameUrl = URLHelper.AddParameterToUrl(frameUrl, "params", Identifier);
        frameUrl = URLHelper.AddParameterToUrl(frameUrl, "islivesite", IsLiveSite.ToString());
        frameUrl = URLHelper.AddParameterToUrl(frameUrl, "hash", QueryHelper.GetHash(frameUrl));
        return frameUrl;
    }
}