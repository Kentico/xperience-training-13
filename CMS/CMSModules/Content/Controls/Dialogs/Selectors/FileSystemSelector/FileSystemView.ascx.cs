using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_FileSystemView : CMSUserControl
{
    private const char ARG_SEPARATOR = '|';


    #region "Private variables"

    private FileSystemDialogConfiguration mConfig;
    private string mStartingPath = "";
    private string mSearchText = "";

    protected string mSaveText;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets whether the view is allowed to edit the items
    /// </summary>
    public bool AllowEdit
    {
        get
        {
            return innermedia.AllowEdit;
        }
        set
        {
            innermedia.AllowEdit = value;
        }
    }


    /// <summary>
    /// Gets or sets text of the information label.
    /// </summary>
    public string InfoText
    {
        get
        {
            return innermedia.InfoText;
        }
        set
        {
            innermedia.InfoText = value;
        }
    }


    /// <summary>
    /// Gets current dialog configuration.
    /// </summary>
    public FileSystemDialogConfiguration Config
    {
        get
        {
            return mConfig ?? (mConfig = new FileSystemDialogConfiguration());
        }
        set
        {
            mConfig = value;
        }
    }


    /// <summary>
    /// Indicates whether the content tree is displaying more than max tree nodes.
    /// </summary>
    public bool IsDisplayMore
    {
        get
        {
            return innermedia.IsDisplayMore;
        }
        set
        {
            innermedia.IsDisplayMore = value;
        }
    }


    /// <summary>
    /// Gets or sets starting path of control.
    /// </summary>
    public string StartingPath
    {
        get
        {
            return mStartingPath;
        }
        set
        {
            mStartingPath = value.StartsWithCSafe("~/") ? Server.MapPath(value) : value;
        }
    }


    /// <summary>
    /// Search text to filter data.
    /// </summary>
    public string SearchText
    {
        get
        {
            mSearchText = ValidationHelper.GetString(ViewState["SearchText"], "");
            return mSearchText;
        }
        set
        {
            mSearchText = value;
            ViewState["SearchText"] = mSearchText;
        }
    }


    /// <summary>
    /// Data source
    /// </summary>
    public DataSet DataSource
    {
        get
        {
            return innermedia.DataSource;
        }
        set
        {
            innermedia.DataSource = value;
        }
    }


    /// <summary>
    /// Gets or sets a view mode used to display files.
    /// </summary>
    public DialogViewModeEnum ViewMode
    {
        get
        {
            return innermedia.ViewMode;
        }
        set
        {
            innermedia.ViewMode = value;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Control events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If processing the request should not continue
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            // Initialize controls
            SetupControls();
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (String.IsNullOrEmpty(lblListingInfo.Text))
        {
            RefreshPathInfo();
        }
    }


    /// <summary>
    /// Loads control's content.
    /// </summary>
    public void Reload()
    {
        // Initialize controls
        SetupControls();

        ReloadData();
    }


    /// <summary>
    /// Displays listing info message.
    /// </summary>
    /// <param name="infoMsg">Info message to display</param>
    public void DisplayListingInfo(string infoMsg)
    {
        if (!string.IsNullOrEmpty(infoMsg))
        {
            plcListingInfo.Visible = true;
            lblListingInfo.Text = infoMsg;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        ScriptHelper.RegisterScriptFile(this, "Controls/FileSystemView.js");

        // Initialize inner view control
        innermedia.FileSystemPath = StartingPath;

        const string SELECTOR_FOLDER = "~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/";

        // Set grid definition
        innermedia.ListViewControl.GridName = Config.ShowFolders ? SELECTOR_FOLDER + "FolderView.xml" : SELECTOR_FOLDER + "FileSystemView.xml";

        // Set inner control binding columns
        innermedia.FileIdColumn = "path";
        innermedia.FileNameColumn = "name";
        innermedia.FileExtensionColumn = "type";
        innermedia.FileSizeColumn = "size";
        innermedia.SearchText = SearchText;

        // Register for inner media events
        innermedia.GetArgumentSet += innermedia_GetArgumentSet;

        innermedia.Configuration = Config;
        innermedia.ViewMode = ViewMode;
    }
    

    /// <summary>
    /// Loads data from data source property.
    /// </summary>
    private void ReloadData()
    {
        innermedia.Reload(true);

        RefreshPathInfo();
    }


    /// <summary>
    /// Refreshes the path information
    /// </summary>
    private void RefreshPathInfo()
    {
        // Get relative path
        string path = StartingPath;
        if (!String.IsNullOrEmpty(path))
        {
            string appPath = SystemContext.WebApplicationPhysicalPath;

            if (path.StartsWithCSafe(appPath, true))
            {
                path = "~" + Path.EnsureSlashes(path.Substring(appPath.Length));
            }

            // Display information about current path
            string info = String.Format(GetString("FileSystemSelector.Info"), HTMLHelper.HTMLEncode(path));
            DisplayListingInfo(info);
        }
    }

    #endregion


    #region "Inner media view event handlers"

    /// <summary>
    /// Returns argument set according passed DataRow and flag indicating whether the set is obtained for selected item.
    /// </summary>
    /// <param name="dr">DataRow with all the item data</param>
    private string innermedia_GetArgumentSet(DataRow dr)
    {
        // Return required argument set
        return GetArgumentSet(dr);
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns argument set for the passed file data row.
    /// </summary>
    /// <param name="dr">Data row object holding all the data on current file</param>
    private string GetArgumentSet(DataRow dr)
    {
        // Common information for both content & attachments
        string result = String.Format(
            "{1}{0}{2}{0}{3}", 
            ARG_SEPARATOR, 
            dr[innermedia.FileIdColumn], 
            DataHelper.GetSizeString(ValidationHelper.GetLong(dr[innermedia.FileSizeColumn], 0)), 
            dr["isfile"]
        );

        return result;
    }


    /// <summary>
    /// Ensures no item is selected.
    /// </summary>
    public void ResetSearch()
    {
        dialogSearch.ResetSearch();
        SearchText = "";
    }

    #endregion
}
