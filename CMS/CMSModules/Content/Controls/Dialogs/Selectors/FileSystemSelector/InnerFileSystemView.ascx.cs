using System;
using System.Collections;
using System.Data;
using System.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_InnerFileSystemView : CMSUserControl
{
    #region "Variables"

    // Data set settings
    private DataSet mDataSource;
    private string mInfoText = string.Empty;
    private string mFileIdColumn = "path";
    private string mFileNameColumn = "name";
    private string mFileExtensionColumn = "type";
    private string mFileSizeColumn = "size";

    private string mSearchText = string.Empty;
    private string mFullStartingPath = string.Empty;

    private bool? mAllowEdit;

    private DialogViewModeEnum mViewMode = DialogViewModeEnum.ListView;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets whether the view is allowed to edit the items
    /// </summary>
    public bool AllowEdit
    {
        get
        {
            if (mAllowEdit == null)
            {
                mAllowEdit = MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
            }

            return mAllowEdit.Value;
        }
        set
        {
            mAllowEdit = value;
        }
    }


    /// <summary>
    /// Switch between folders and files mode.
    /// </summary>
    public bool ShowOnlyFolders
    {
        get;
        set;
    }


    /// <summary>
    /// Source file system path.
    /// </summary>
    public string FileSystemPath
    {
        get;
        set;
    }


    /// <summary>
    /// Dialog configuration containning all necessary settings.
    /// </summary>
    public FileSystemDialogConfiguration Configuration
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets full starting path property.
    /// </summary>
    public string FullStartingPath
    {
        get
        {
            if (String.IsNullOrEmpty(mFullStartingPath))
            {
                mFullStartingPath = Configuration.StartingPath.StartsWithCSafe("~/") ? Server.MapPath(Configuration.StartingPath) : Configuration.StartingPath;
            }
            return mFullStartingPath;
        }
        set
        {
            mFullStartingPath = value;
        }
    }


    /// <summary>
    /// Gets or sets source of the data for view controls.
    /// </summary>
    public DataSet DataSource
    {
        get
        {
            if (DataHelper.DataSourceIsEmpty(mDataSource))
            {
                mDataSource = GetDataSet(FileSystemPath, SearchText);
            }
            return mDataSource;
        }
        set
        {
            mDataSource = value;
        }
    }


    /// <summary>
    /// Gets or sets text of the information label.
    /// </summary>
    public string InfoText
    {
        get
        {
            return mInfoText;
        }
        set
        {
            mInfoText = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on the file identifier.
    /// </summary>
    public string FileIdColumn
    {
        get
        {
            return mFileIdColumn;
        }
        set
        {
            mFileIdColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on file name.
    /// </summary>
    public string FileNameColumn
    {
        get
        {
            return mFileNameColumn;
        }
        set
        {
            mFileNameColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on file extension.
    /// </summary>
    public string FileExtensionColumn
    {
        get
        {
            return mFileExtensionColumn;
        }
        set
        {
            mFileExtensionColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets name of the column holding information on file size.
    /// </summary>
    public string FileSizeColumn
    {
        get
        {
            return mFileSizeColumn;
        }
        set
        {
            mFileSizeColumn = value;
        }
    }


    /// <summary>
    /// Indicates whether the content tree is displaying more than max tree nodes.
    /// </summary>
    public bool IsDisplayMore
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsDisplayMore"], false);
        }
        set
        {
            ViewState["IsDisplayMore"] = value;
        }
    }


    /// <summary>
    /// Gets a UniGrid control used to display files in LIST view mode.
    /// </summary>
    public UniGrid ListViewControl
    {
        get
        {
            return gridList;
        }
    }


    /// <summary>
    /// Gets a repeater control used to display files in THUMBNAILS view mode.
    /// </summary>
    public BasicRepeater ThumbnailsViewControl
    {
        get
        {
            return repThumbnailsView;
        }
    }


    /// <summary>
    /// Gets or sets text to be searched.
    /// </summary>
    public string SearchText
    {
        get
        {
            return mSearchText;
        }
        set
        {
            mSearchText = value;
        }
    }


    /// <summary>
    /// Gets or sets a view mode used to display files.
    /// </summary>
    public DialogViewModeEnum ViewMode
    {
        get
        {
            return mViewMode;
        }
        set
        {
            mViewMode = value;
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


    #region "Helper methods"

    /// <summary>
    /// Check if extension is set as allowed.
    /// </summary>
    /// <param name="info">File to check</param>
    /// <returns>True if extension is allowed otherwise false</returns>
    private bool IsAllowedExtension(FileInfo info)
    {
        if (!String.IsNullOrEmpty(Configuration.AllowedExtensions))
        {
            if (Configuration.AllowedExtensions.ToLowerCSafe().Contains(info.Extension.ToLowerCSafe().TrimStart('.')))
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Check if extension is not set as excluded.
    /// </summary>
    /// <param name="info">File to check</param>
    /// <returns>True if extension isn't excluded otherwise false</returns>
    private bool IsNotExcludedExtension(FileInfo info)
    {
        if (!String.IsNullOrEmpty(Configuration.ExcludedExtensions))
        {
            if (Configuration.ExcludedExtensions.ToLowerCSafe().Contains(info.Extension.ToLowerCSafe().TrimStart('.')))
            {
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Returns correct ID for the item (for colorizing the item when selected).
    /// </summary>
    /// <param name="dataItem">Container.DataItem</param>
    protected string GetID(object dataItem)
    {
        DataRowView dr = dataItem as DataRowView;
        if (dr != null)
        {
            return GetColorizeID(dr);
        }

        return "";
    }


    /// <summary>
    /// Returns correct ID for the given item (for colorizing the item when selected).
    /// </summary>
    /// <param name="dr">Item to get the ID of</param>
    protected string GetColorizeID(DataRowView dr)
    {
        string id = dr[FileIdColumn].ToString().ToLowerCSafe();
        if (String.IsNullOrEmpty(id))
        {
            id = dr["Path"].ToString().ToLowerCSafe();
        }
        return id;
    }


    /// <summary>
    /// Check if item in row could be selected.
    /// </summary>
    /// <param name="isFile">Indicates if item is file</param>
    /// <returns>True if could be selected otherwise false</returns>
    private bool IsItemSelectable(bool isFile)
    {
        if ((!Configuration.ShowFolders && isFile) || (Configuration.ShowFolders && !isFile))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Check if folder is allowed and not excluded.
    /// </summary>
    /// <param name="info">DiretoryInfo to check</param>
    /// <returns>True if folder is allowed and not excluded otherwise false</returns>
    private bool IsAllowedAndNotExcludedFolder(DirectoryInfo info)
    {
        bool isAllowed = false;
        bool isExcluded = false;
        string startPath = Path.EnsureEndSlash(FullStartingPath.ToLowerCSafe());
        string folderName = info.FullName.ToLowerCSafe();

        // Check if folder is allowed
        if (String.IsNullOrEmpty(Configuration.AllowedFolders))
        {
            isAllowed = true;
        }
        else
        {
            foreach (string path in Configuration.AllowedFolders.ToLowerCSafe().Split(';'))
            {
                if (folderName.StartsWithCSafe(startPath + path))
                {
                    isAllowed = true;
                }
            }
        }

        // Check if folder isn't excluded
        if (!String.IsNullOrEmpty(Configuration.ExcludedFolders))
        {
            foreach (string path in Configuration.ExcludedFolders.ToLowerCSafe().Split(';'))
            {
                if (folderName.StartsWithCSafe(startPath + path))
                {
                    isExcluded = true;
                }
            }
        }
        return (isAllowed) && (!isExcluded);
    }

    #endregion


    #region "Events & delegates"

    /// <summary>
    /// Delegate for an event occurring when argument set is required.
    /// </summary>
    /// <param name="dr">DataRow holding information on currently processed file</param>
    public delegate string OnGetArgumentSet(DataRow dr);


    /// <summary>
    /// Event occurring when argument set is required.
    /// </summary>
    public event OnGetArgumentSet GetArgumentSet;

    #endregion


    #region "Raise events methods"

    /// <summary>
    /// Fires specific action and returns result provided by the parent control.
    /// </summary>
    /// <param name="dr">Data related to the action</param>
    private string RaiseOnGetArgumentSet(DataRow dr)
    {
        if (GetArgumentSet != null)
        {
            return GetArgumentSet(dr);
        }
        return string.Empty;
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        InfoText = string.Empty;

        // Initialize displaying controls according view mode
        LoadViewControls();

        // Include javascript functions
        InitializeControlScripts();

        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                InitializeListView();
                ListViewControl.OnExternalDataBound += ListViewControl_OnExternalDataBound;
                ListViewControl.GridView.RowDataBound += GridView_RowDataBound;
                break;

            case DialogViewModeEnum.ThumbnailsView:
                InitializeThumbnailsView();
                ThumbnailsViewControl.ItemDataBound += ViewControl_ItemDataBound;
                break;
        }
    }


    /// <summary>
    /// OnPreRender event.
    /// </summary>
    /// <param name="e">Event argument set</param>
    protected override void OnPreRender(EventArgs e)
    {
        // Display information on empty data
        bool isEmpty = DataHelper.DataSourceIsEmpty(DataSource);
        if (isEmpty)
        {
            plcViewArea.Visible = false;
        }
        else
        {
            lblInfo.Visible = false;
            plcViewArea.Visible = true;
        }

        // If info text is set display it
        if (!string.IsNullOrEmpty(InfoText))
        {
            lblInfo.Text = InfoText;
            lblInfo.Visible = true;
        }
        else
        {
            if (isEmpty)
            {
                lblInfo.Text = Configuration.ShowFolders ? GetString("dialogs.filesystem.nofolders") : GetString("dialogs.view.list.nodata");
                lblInfo.Visible = true;
            }
        }

        // Register the scripts
        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.RegisterDialogScript(Page);

        base.OnPreRender(e);
    }


    /// <summary>
    /// PageLoad event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event argument set</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = !StopProcessing;
        if (!StopProcessing)
        {
            if (RequestHelper.IsPostBack())
            {
                Reload(true);
            }
        }
    }


    /// <summary>
    /// Loads view controls according currently set view mode.
    /// </summary>
    public void LoadViewControls()
    {
        InfoText = "";

        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                plcListView.Visible = true;

                // Stop processing                
                plcThumbnailsView.Visible = false;
                break;

            case DialogViewModeEnum.ThumbnailsView:
                plcThumbnailsView.Visible = true;
                repThumbnailsView.DataBindByDefault = false;
                pagerElemThumbnails.UniPager.ConnectToPagedControl(repThumbnailsView);

                // Stop processing
                plcListView.Visible = false;
                break;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Loads data from the data source property.
    /// </summary>
    public void ReloadData()
    {
        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                ReloadListView();
                break;

            case DialogViewModeEnum.ThumbnailsView:
                ReloadThumbnailsView();
                break;
        }
    }


    /// <summary>
    /// Reloads control with data.
    /// </summary>
    /// <param name="forceSetup">Indicates whether the inner controls should be re-setuped</param>
    public void Reload(bool forceSetup)
    {
        Visible = !StopProcessing;
        if (Visible)
        {
            if (forceSetup)
            {
                // Initialize controls
                SetupControls();
            }

            // Load passed data
            ReloadData();
        }
    }

    #endregion


    #region "List view methods"

    /// <summary>
    /// Initializes list view controls.
    /// </summary>
    private void InitializeListView()
    {
        gridList.OrderBy = "isfile,name";
    }


    /// <summary>
    /// Reloads list view according source type.
    /// </summary>
    private void ReloadListView()
    {
        // Fill the grid data source
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            gridList.DataSource = DataSource;
            gridList.ReloadData();
        }
    }

    #endregion


    #region "Thumbnails view methods"

    /// <summary>
    /// Setups the UniPager
    /// </summary>
    /// <param name="pager">Pager to setup</param>
    private void SetupPager(UniPager pager)
    {
        pager.DisplayFirstLastAutomatically = false;
        pager.DisplayPreviousNextAutomatically = false;
        pager.HidePagerForSinglePage = true;
        pager.PagerMode = UniPagerMode.PostBack;
    }


    /// <summary>
    /// Initializes controls for the thumbnails view mode.
    /// </summary>
    private void InitializeThumbnailsView()
    {
        // Initialize page size
        pagerElemThumbnails.PageSizeOptions = "10,20,50,100";
        pagerElemThumbnails.DefaultPageSize = 10;

        // Basic control properties
        repThumbnailsView.HideControlForZeroRows = true;

        // UniPager properties     
        SetupPager(pagerElemThumbnails.UniPager);
    }


    /// <summary>
    /// Loads content for media libraries thumbnails view element.
    /// </summary>
    private void ReloadThumbnailsView()
    {
        // Connects repeater with data source
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            repThumbnailsView.DataSource = DataSource;
            repThumbnailsView.DataBind();
        }
    }


    protected void ViewControl_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Item.DataItem;
        IDataContainer data = new DataRowContainer(drv);

        string fileNameColumn = FileNameColumn;

        // Get information on file
        string fileName = HTMLHelper.HTMLEncode(data.GetValue(fileNameColumn).ToString());
        string ext = HTMLHelper.HTMLEncode(data.GetValue(FileExtensionColumn).ToString());

        // Load file type
        Label lblType = e.Item.FindControl("lblTypeValue") as Label;
        if (lblType != null)
        {
            fileName = fileName.Substring(0, fileName.Length - ext.Length);
            lblType.Text = ResHelper.LocalizeString(ext);
        }

        // Load file name
        Label lblName = e.Item.FindControl("lblFileName") as Label;
        if (lblName != null)
        {
            lblName.Text = fileName;
        }

        // Load file size
        Label lblSize = e.Item.FindControl("lblSizeValue") as Label;
        if (lblSize != null)
        {
            long size = ValidationHelper.GetLong(data.GetValue(FileSizeColumn), 0);
            lblSize.Text = DataHelper.GetSizeString(size);
        }

        string argument = RaiseOnGetArgumentSet(drv.Row);
        string selectAction = GetSelectScript(drv, argument);

        // Initialize EDIT button
        var btnEdit = e.Item.FindControl("btnEdit") as CMSAccessibleButton;
        if (btnEdit != null)
        {
            btnEdit.ToolTip = GetString("general.edit");

            string path = data.GetValue("Path").ToString();
            string editScript = GetEditScript(path, ext);

            if (!String.IsNullOrEmpty(editScript))
            {
                btnEdit.OnClientClick = editScript;
            }
            else
            {
                btnEdit.Visible = false;
            }
        }

        // Initialize DELETE button
        var btnDelete = e.Item.FindControl("btnDelete") as CMSAccessibleButton;
        if (btnDelete != null)
        {
            btnDelete.ToolTip = GetString("general.delete");
            btnDelete.OnClientClick = GetDeleteScript(argument);
        }

        // Image area
        Image fileImage = e.Item.FindControl("imgElem") as Image;
        if (fileImage != null)
        {
            string url;
            if (ImageHelper.IsImage(ext))
            {
                url = URLHelper.UnMapPath(data.GetValue("Path").ToString());

                // Generate new tooltip command
                if (!String.IsNullOrEmpty(url))
                {
                    url = String.Format("{0}?chset={1}", url, Guid.NewGuid());

                    UIHelper.EnsureTooltip(fileImage, ResolveUrl(url), 0, 0, null, null, ext, null, null, 300);
                }

                fileImage.ImageUrl = ResolveUrl(url);
            }
            else
            {
                fileImage.Visible = false;

                Literal literalImage = e.Item.FindControl("ltrImage") as Literal;
                if (literalImage != null)
                {
                    literalImage.Text = UIHelper.GetFileIcon(Page, ext, FontIconSizeEnum.Dashboard);
                }
            }
        }

        // Selectable area
        Panel pnlItemInageContainer = e.Item.FindControl("pnlThumbnails") as Panel;
        if (pnlItemInageContainer != null)
        {
            pnlItemInageContainer.Attributes["onclick"] = selectAction;
            pnlItemInageContainer.Attributes["style"] = "cursor:pointer;";
        }

    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the necessary JavaScript blocks.
    /// </summary>
    private void InitializeControlScripts()
    {
        string script = String.Format(
@"  
var attemptNo = 0;

function ColorizeRow(itemId) {{
    if (itemId != null) 
    {{
        var hdnField = document.getElementById('{0}');         
        if (hdnField != null) 
        {{
            // If some item was previously selected
            if ((hdnField.value != null) && (hdnField.value != '')) 
            {{   
                // Get selected item and reset its selection
                var lastColorizedElem = document.getElementById(hdnField.value);
                if (lastColorizedElem != null) 
                {{   
                    ColorizeElement(lastColorizedElem, true);
                }}
            }}

            // Update field value
            hdnField.value = itemId;
        }}                                                              

        // Colorize currently selected item
        var elem = document.getElementById(itemId);
        if (elem != null) 
        {{
            ColorizeElement(elem, false);
            attemptNo = 0;
        }}
        else
        {{
            if(attemptNo < 1)
            {{
                setTimeout('ColorizeRow(\'' + itemId + '\')', 300);
                attemptNo = attemptNo + 1;
            }}
            else
            {{
                attemptNo = 0;
            }}
        }}
    }}
}}

function ColorizeLastRow() {{
    var hdnField = document.getElementById('{0}');     
    if (hdnField != null) 
    {{
        // If some item was previously selected
        if ((hdnField.value != null) && (hdnField.value != '')) 
        {{               
            // Get selected item and reset its selection
            var lastColorizedElem = document.getElementById(hdnField.value);
            if (lastColorizedElem != null) 
            {{    
                ColorizeElement(lastColorizedElem, false);
            }}
        }}
    }}
}}

function ColorizeElement(elem, clear) {{
    if(!clear){{
        elem.className += ' Selected';
    }}
    else {{
        elem.className = elem.className.replace(' Selected','');
    }}
}}

function ClearColorizedRow()
{{
    var hdnField = document.getElementById('{0}');      
    if (hdnField != null) 
    {{
        // If some item was previously selected
        if ((hdnField.value != null) && (hdnField.value != '')) 
        {{               
            // Get selected item and reset its selection
            var lastColorizedElem = document.getElementById(hdnField.value);
            if (lastColorizedElem != null) 
            {{   
                ColorizeElement(lastColorizedElem, true);

                // Update field value
                hdnField.value = '';
            }}
        }}
    }}
}}
",
            hdnItemToColorize.ClientID
        );


        ScriptManager.RegisterStartupScript(this, GetType(), "DialogsColorize", script, true);
    }


    /// <summary>
    /// Gets DataSource dataSet.
    /// </summary>
    /// <param name="fileSystemPath">File system path to obtain dataset for</param>
    /// <param name="searchText">Text to be searched</param>
    private DataSet GetDataSet(string fileSystemPath, string searchText)
    {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();

        // Defining table columns
        dt.Columns.Add(FileNameColumn, typeof(string));
        dt.Columns.Add(FileExtensionColumn, typeof(string));
        dt.Columns.Add(FileIdColumn, typeof(string));
        dt.Columns.Add(FileSizeColumn, typeof(long));
        dt.Columns.Add("filemodified", typeof(DateTime));
        dt.Columns.Add("isfile", typeof(bool));
        dt.Columns.Add("childscount", typeof(int));

        if (!string.IsNullOrEmpty(fileSystemPath))
        {
            try
            {
                // Get directory info
                if (Directory.Exists(fileSystemPath))
                {
                    DirectoryInfo di = DirectoryInfo.New(fileSystemPath);

                    // Check if folders should be displayed
                    if ((IsDisplayMore) || (Configuration.ShowFolders))
                    {
                        // Get folders array and filter it
                        DirectoryInfo[] folders = di.GetDirectories();
                        folders = Array.FindAll(folders, IsAllowedAndNotExcludedFolder);

                        int childCount = 0;

                        foreach (DirectoryInfo folder in folders)
                        {
                            if ((String.IsNullOrEmpty(searchText)) || (folder.Name.ToLowerCSafe().Contains(searchText.ToLowerCSafe())))
                            {
                                try
                                {
                                    // Set children number
                                    if (Configuration.ShowFolders)
                                    {
                                        childCount = folder.GetDirectories().Length;
                                    }
                                    else
                                    {
                                        childCount = folder.GetDirectories().Length;
                                        if (childCount == 0)
                                        {
                                            FileInfo[] files = folder.GetFiles();
                                            // Check for alowed extensions 
                                            if (!String.IsNullOrEmpty(Configuration.AllowedExtensions))
                                            {
                                                files = Array.FindAll(files, IsAllowedExtension);
                                            }

                                            // Check for excluded extensions 
                                            if (!String.IsNullOrEmpty(Configuration.ExcludedExtensions))
                                            {
                                                files = Array.FindAll(files, IsNotExcludedExtension);
                                            }
                                            childCount = files.Length;
                                        }
                                    }
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    childCount = 0;
                                }
                                finally
                                {
                                    dt.Rows.Add(folder.Name, string.Empty, folder.FullName, 0, folder.LastWriteTime, false, childCount);
                                }
                            }
                        }
                    }

                    // Try to load files
                    try
                    {
                        if (!Configuration.ShowFolders)
                        {
                            // Obtain list of files
                            FileInfo[] files = di.GetFiles();

                            // Check for alowed extensions 
                            if (!String.IsNullOrEmpty(Configuration.AllowedExtensions))
                            {
                                files = Array.FindAll(files, IsAllowedExtension);
                            }

                            // Check for excluded extensions 
                            if (!String.IsNullOrEmpty(Configuration.ExcludedExtensions))
                            {
                                files = Array.FindAll(files, IsNotExcludedExtension);
                            }

                            // Add files item to table 
                            foreach (FileInfo file in files)
                            {
                                if ((String.IsNullOrEmpty(searchText)) || (Path.GetFileNameWithoutExtension(file.Name).ToLowerCSafe().Contains(searchText.ToLowerCSafe())))
                                {
                                    dt.Rows.Add(file.Name, file.Extension, file.FullName, file.Length, file.LastWriteTime, true, 0);
                                }
                            }
                        }
                    }
                    catch (SecurityException se)
                    {
                        Service.Resolve<IEventLogService>().LogException("FileSystemDialog", "SECURITYEXCEPTION", se);
                    }
                }
            }
            catch (Exception e)
            {
                Service.Resolve<IEventLogService>().LogException("FileSystemDialog", "FOLDERNOTACCESSIBLE", e);
            }
        }
        ds.Tables.Add(dt);
        return ds;
    }


    /// <summary>
    /// Returns panel with image according extension of the processed file.
    /// </summary>
    /// <param name="ext">Extension of the file used to determine icon</param>
    /// <param name="url">File url</param>
    /// <param name="isFile">True for file items</param>
    /// <param name="item">Control inserted as a file name</param>
    /// <param name="isSelectable">If item can be selected</param>
    private Panel GetListItem(string ext, string url, bool isFile, Control item, bool isSelectable)
    {
        Panel pnl = new Panel();
        pnl.CssClass = "DialogListItem" + (isSelectable ? string.Empty : "Unselectable");
        pnl.Controls.Add(new LiteralControl("<div class=\"DialogListItemNameRow\">"));

        var image = isFile ? UIHelper.GetFileIcon(Page, ext) : UIHelper.GetAccessibleIconTag("icon-folder");
        pnl.Controls.Add(new LiteralControl(image));

        // Generate new tooltip command
        if (!String.IsNullOrEmpty(url))
        {
            url = String.Format("{0}?chset={1}", url, Guid.NewGuid());

            UIHelper.EnsureTooltip(pnl, ResolveUrl(url), 0, 0, null, null, ext, null, null, 300);
        }

        if ((isSelectable) && (item is LinkButton))
        {
            // Create clickabe compelte panel
            pnl.Attributes["onclick"] = ((LinkButton)item).Attributes["onclick"];
            ((LinkButton)item).Attributes["onclick"] = null;
        }


        // Add file name                  
        pnl.Controls.Add(new LiteralControl(String.Format("<span class=\"DialogListItemName\" {0}>", (!isSelectable ? "style=\"cursor:default;\"" : string.Empty))));
        pnl.Controls.Add(item);
        pnl.Controls.Add(new LiteralControl("</span></div>"));

        return pnl;
    }


    /// <summary>
    /// Unigrid row data bound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Argument identifier</param>
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView drv = (e.Row.DataItem as DataRowView);
            if (drv != null)
            {
                e.Row.Attributes["id"] = GetColorizeID(drv);
            }
        }
    }


    /// <summary>
    /// On external databound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Action what is called</param>
    /// <param name="parameter">Parameter</param>
    /// <returns>Result object</returns>
    protected object ListViewControl_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Initializing variables
        object result = null;

        string argument;
        string ext;

        CMSGridActionButton btn;
        DataRowView drv;
        GridViewRow gvr;

        switch (sourceName.ToLowerCSafe())
        {
            // Select event
            case "select":
                {
                    gvr = (GridViewRow)parameter;
                    drv = (DataRowView)gvr.DataItem;
                    btn = (CMSGridActionButton)sender;

                    // Get item extension
                    bool isSelectable = IsItemSelectable(ValidationHelper.GetBoolean(drv["isfile"], true));

                    // Check if item is selectable, if not remove select action button
                    if (!isSelectable)
                    {
                        btn.ToolTip = "";
                        btn.Enabled = false;
                    }
                    else
                    {
                        argument = RaiseOnGetArgumentSet(drv.Row);

                        // Initialize command
                        btn.OnClientClick = GetSelectScript(drv, argument);

                        result = btn;
                    }
                    break;
                }

            // Select subdocs event
            case "selectsubdocs":
                btn = sender as CMSGridActionButton;
                if (btn != null)
                {
                    gvr = (GridViewRow)parameter;
                    drv = (DataRowView)gvr.DataItem;

                    string nodeId = ValidationHelper.GetString(drv[FileIdColumn], "");
                    int childCount = ValidationHelper.GetInteger(drv["childscount"], 0);

                    if ((IsDisplayMore || Configuration.ShowFolders) && !ValidationHelper.GetBoolean(drv["isfile"], true))
                    {
                        // Check if item is selectable, if not remove select action button
                        if (childCount > 0)
                        {
                            // Initialize command
                            btn.OnClientClick = "SetParentAction('" + nodeId.Replace("\\", "\\\\").Replace("'", "\\'") + "'); return false;";
                        }
                        else
                        {
                            btn.ToolTip = GetString("dialogs.list.actions.showsubfolders");
                            btn.Enabled = false;
                        }
                    }
                    else
                    {
                        // Hide subdocs button
                        btn.Attributes["style"] = "display:none;";
                    }
                }
                break;

            // Edit action
            case "edit":
                {
                    btn = sender as CMSGridActionButton;
                    if (btn != null)
                    {
                        if (AllowEdit && Configuration.AllowManage)
                        {
                            gvr = (GridViewRow)parameter;
                            drv = (DataRowView)gvr.DataItem;

                            ext = drv[FileExtensionColumn].ToString();
                            string path = drv["Path"].ToString();

                            string editScript = GetEditScript(path, ext);

                            // Assign the onclick script
                            if (!String.IsNullOrEmpty(editScript))
                            {
                                btn.OnClientClick = editScript;

                                return result;
                            }
                        }

                        btn.Visible = false;
                    }
                }
                break;

            // Delete event
            case "delete":
                {
                    btn = ((CMSGridActionButton)sender);

                    if (AllowEdit && Configuration.AllowManage)
                    {
                        gvr = (parameter as GridViewRow);
                        drv = (DataRowView)gvr.DataItem;

                        argument = RaiseOnGetArgumentSet(drv.Row);

                        // Initialize command
                        btn.OnClientClick = GetDeleteScript(argument);

                        return result;
                    }

                    btn.Visible = false;
                }
                break;

            // Name event
            case "name":
                {
                    drv = (DataRowView)parameter;

                    // Get name and extension
                    string name = drv[FileNameColumn].ToString();
                    ext = drv[FileExtensionColumn].ToString();

                    bool isFile = ValidationHelper.GetBoolean(drv["isfile"], true);

                    // Remove extension if available
                    if (isFile)
                    {
                        name = HTMLHelper.HTMLEncode(Path.GetFileNameWithoutExtension(name));
                    }

                    string url = null;
                    if (ImageHelper.IsImage(ext))
                    {
                        url = URLHelper.UnMapPath(drv["Path"].ToString());
                    }

                    // Check if item is selectable
                    if (!IsItemSelectable(isFile))
                    {
                        LiteralControl ltlName = new LiteralControl(name);

                        // Get final panel
                        result = GetListItem(ext, url, isFile, ltlName, false);
                    }
                    else
                    {
                        // Make a file name link
                        LinkButton lnkBtn = new LinkButton();

                        // Escape chars for post back JavaScript event
                        lnkBtn.ID = name.Replace("'", "").Replace("$", "");
                        lnkBtn.Text = HTMLHelper.HTMLEncode(name);

                        argument = RaiseOnGetArgumentSet(drv.Row);

                        // Initialize command
                        lnkBtn.Attributes["onclick"] = GetSelectScript(drv, argument);

                        // Get final panel
                        result = GetListItem(ext, url, isFile, lnkBtn, true);
                    }
                }
                break;

            // Type event
            case "type":
                drv = (DataRowView)parameter;

                // Remove starting dot
                result = drv[FileExtensionColumn].ToString().ToLowerCSafe();
                break;

            // Size event
            case "size":
                drv = (DataRowView)parameter;

                // Get formatted size string
                if (ValidationHelper.GetBoolean(drv["isfile"], true))
                {
                    long size = ValidationHelper.GetLong(drv[FileSizeColumn], 0);
                    result = DataHelper.GetSizeString(size);
                }
                else
                {
                    return "";
                }
                break;

            // File modified event
            case "filemodified":
                drv = (DataRowView)parameter;
                result = drv["filemodified"].ToString();
                break;
        }

        return result;
    }


    /// <summary>
    /// Gets the item selection script
    /// </summary>
    /// <param name="drv">Item data</param>
    /// <param name="argument">Item argument</param>
    protected string GetSelectScript(DataRowView drv, string argument)
    {
        return String.Format("ColorizeRow({0}); SetSelectAction({1}); return false;", ScriptHelper.GetString(GetColorizeID(drv)), ScriptHelper.GetString(argument));
    }


    /// <summary>
    /// Gets the deletion script for the item
    /// </summary>
    /// <param name="argument">Item argument</param>
    protected string GetDeleteScript(string argument)
    {
        return String.Format("if (confirm({0})) {{ SetDeleteAction({1}); }} return false;", ScriptHelper.GetLocalizedString("General.ConfirmDelete"), ScriptHelper.GetString(argument));
    }


    /// <summary>
    /// Gets the editing script for the given path and extension
    /// </summary>
    /// <param name="path">File path</param>
    /// <param name="ext">File extension</param>
    protected string GetEditScript(string path, string ext)
    {
        string fileIdentifier = Identifier + path.GetHashCode();
        WindowHelper.Remove(fileIdentifier);

        Hashtable props = new Hashtable();
        props.Add("filepath", URLHelper.UnMapPath(path));

        WindowHelper.Add(fileIdentifier, props);

        const string CONTENT_FOLDER = "~/CMSModules/Content/";

        if (ImageHelper.IsSupportedByImageEditor(ext))
        {
            // Image editing (image editor)
            string parameters = String.Format("?identifier={0}&refresh=1", fileIdentifier);
            string validationHash = QueryHelper.GetHash(parameters);
            string url = UrlResolver.ResolveUrl(CONTENT_FOLDER + "CMSDesk/Edit/ImageEditor.aspx") + parameters + "&hash=" + validationHash;

            return String.Format("modalDialog({0}, 'imageeditor', 905, 670); return false;", ScriptHelper.GetString(url));
        }

        // Text file editing
        if (FileHelper.IsTextFileExtension(ext))
        {
            // Prepare parameters
            string parameters = String.Format("?identifier={0}", fileIdentifier);
            string validationHash = QueryHelper.GetHash(parameters);
            string url = UrlResolver.ResolveUrl(CONTENT_FOLDER + "Controls/Dialogs/Selectors/FileSystemSelector/EditTextFile.aspx") + parameters + "&hash=" + validationHash;

            return String.Format("modalDialog({0}, 'texteditor', 905, 688); return false;", ScriptHelper.GetString(url));
        }

        return null;
    }

    #endregion
}
