using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI;
using WebControls = System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using CMS.IO;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.Internal;
using CMS.WorkflowEngine;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_LinkMediaSelector : ContentLinkMediaSelector
{
    #region "Private variables"

    // Content variables
    private int mSiteID;
    private int mNodeID;

    private TreeNode mTreeNodeObj;

    #endregion


    #region "Public properties"

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


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets last searched value.
    /// </summary>
    private string LastSearchedValue
    {
        get
        {
            return hdnLastSearchedValue.Value;
        }
        set
        {
            hdnLastSearchedValue.Value = value;
        }
    }


    /// <summary>
    /// Gets current action name.
    /// </summary>
    private string CurrentAction
    {
        get
        {
            return hdnAction.Value.ToLowerCSafe().Trim();
        }
        set
        {
            hdnAction.Value = value;
        }
    }


    /// <summary>
    /// Gets current action argument value.
    /// </summary>
    private string CurrentArgument
    {
        get
        {
            return hdnArgument.Value;
        }
    }


    /// <summary>
    /// Returns current properties (according to OutputFormat).
    /// </summary>
    protected override ItemProperties ItemProperties
    {
        get
        {
            return GetItemProperties();
        }
    }


    /// <summary>
    /// Update panel where properties control resides.
    /// </summary>
    protected override UpdatePanel PropertiesUpdatePanel
    {
        get
        {
            return pnlUpdateProperties;
        }
    }


    /// <summary>
    /// Gets ID of the site files are being displayed for.
    /// </summary>
    private int SiteID
    {
        get
        {
            if (mSiteID == 0)
            {
                mSiteID = siteSelector.SiteID;
            }
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets name of the site files are being displayed for.
    /// </summary>
    private string SiteName
    {
        get
        {
            return siteSelector.SiteName;
        }
        set
        {
            siteSelector.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the node selected in the content tree.
    /// </summary>
    private int NodeID
    {
        get
        {
            if (mNodeID == 0)
            {
                mNodeID = ValidationHelper.GetInteger(hdnLastNodeSlected.Value, 0);
            }
            return mNodeID;
        }
        set
        {
            // Set node only if changed
            if (mNodeID != value)
            {
                mNodeID = value;
                hdnLastNodeSlected.Value = value.ToString();
                mTreeNodeObj = null;
            }
        }
    }


    /// <summary>
    /// Gets the node attachments are related to.
    /// </summary>
    private TreeNode TreeNodeObj
    {
        get
        {
            if (mTreeNodeObj == null)
            {
                var node = GetDocument(SiteName, NodeID);

                mTreeNodeObj = node;
                mediaView.TreeNodeObj = mTreeNodeObj;
            }
            return mTreeNodeObj;
        }
        set
        {
            mTreeNodeObj = value;
        }
    }


    /// <summary>
    /// Indicates if full listing mode is enabled. This mode enables navigation to child and parent folders/documents from current view.
    /// </summary>
    private bool IsFullListingMode
    {
        get
        {
            return mediaView.IsFullListingMode;
        }
        set
        {
            mediaView.IsFullListingMode = value;
        }
    }


    /// <summary>
    /// The original URL of the link.
    /// </summary>
    private string OriginalUrl
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Gets the item properties control
    /// </summary>
    private ItemProperties GetItemProperties()
    {
        switch (Config.OutputFormat)
        {
            case OutputFormatEnum.HTMLMedia:
                return htmlMediaProp;

            case OutputFormatEnum.HTMLLink:
                return htmlLinkProp;

            case OutputFormatEnum.BBMedia:
                return bbMediaProp;

            case OutputFormatEnum.BBLink:
                return bbLinkProp;

            default:
                if ((Config.CustomFormatCode == "copy") || (Config.CustomFormatCode == "move") || (Config.CustomFormatCode == "link") || (Config.CustomFormatCode == "linkdoc"))
                {
                    return docCopyMoveProp;
                }
                return urlProp;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        menuElem.Config = Config;
        mediaView.Config = Config;

        // Get source type according URL parameters
        SourceType = CMSDialogHelper.GetMediaSource(QueryHelper.GetString("source", "attachments"));
        mediaView.OutputFormat = Config.OutputFormat;

        // All sites for copy, move and link dialog
        siteSelector.OnlyRunningSites = !IsCopyMoveLinkDialog;

        // Prepare help
        SetHelp();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // High-light item being edited
        if (ItemToColorize != Guid.Empty)
        {
            ColorizeRow(ItemToColorize.ToString());
        }

        if (!RequestHelper.IsPostBack() && (siteSelector.DropDownSingleSelect.SelectedItem == null))
        {
            EnsureLoadedSite();
        }

        // Handle empty site selector
        HandleSiteEmpty();

        menuElem.ShowParentButton = (menuElem.ShowParentButton && (StartingPathNodeID != NodeID) && IsFullListingMode);
        pnlUpdateMenu.Update();

        // Display info on listing more content
        if (IsFullListingMode && (TreeNodeObj != null))
        {
            DisplayFullListingInfo();
        }

        if (IsCopyMoveLinkDialog)
        {
            DisplayMediaElements();
        }
    }


    /// <summary>
    /// Displays the information about the full listing mode
    /// </summary>
    private void DisplayFullListingInfo()
    {
        string closeLink = String.Format("<span class=\"ListingClose\" style=\"cursor: pointer;\" onclick=\"SetAction('closelisting', ''); RaiseHiddenPostBack(); return false;\">{0}</span>", TextHelper.FirstLetterToUpper(GetString("general.clickhere")));
        string docName = String.Format("<span class=\"ListingPath\">{0}</span>", HTMLHelper.HTMLEncode(TreeNodeObj.DocumentName));

        string listingMsg = string.Format(GetString("dialogs.content.listingInfo"), docName, closeLink);

        mediaView.DisplayListingInfo(listingMsg);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            CheckPermissions(TreeNodeObj);

            SetupProperties();

            InitializeDialogs();

            SetupControls();

            // Needed for pager in thumbnails mode to work properly
            mediaView.Reload();

            EnsureLoadedData();
        }
        else
        {
            siteSelector.StopProcessing = true;
            Visible = false;
        }

        ScriptHelper.RegisterWOpenerScript(Page);
    }

    #endregion


    #region "Inherited methods"

    /// <summary>
    /// Returns selected item parameters as name-value collection.
    /// </summary>
    public void GetSelectedItem()
    {
        // Clear unused information from session
        ClearSelectedItemInfo();
        ClearActionElems();

        if (ItemProperties.Validate())
        {
            // Store tab information in the user's dialogs configuration
            StoreDialogsConfiguration();

            // Get selected item information
            Hashtable props = ItemProperties.GetItemProperties();

            // Get JavaScript for inserting the item
            string script = GetInsertItem(props);
            if (!string.IsNullOrEmpty(script))
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "insertItemScript", ScriptHelper.GetScript(script));
            }

            if ((Config.CustomFormatCode.ToLowerCSafe() == "copy") || (Config.CustomFormatCode.ToLowerCSafe() == "move") || (Config.CustomFormatCode.ToLowerCSafe() == "link") || (Config.CustomFormatCode.ToLowerCSafe() == "linkdoc"))
            {
                // Reload the iframe
                pnlUpdateProperties.Update();
            }
        }
        else
        {
            // Display error message
            pnlUpdateProperties.Update();
        }
    }

    #endregion


    #region "Dialog configuration"

    /// <summary>
    /// Stores current tab's configuration for the user.
    /// </summary>
    private void StoreDialogsConfiguration()
    {
        UserInfo ui = UserInfo.Provider.Get(MembershipContext.AuthenticatedUser.UserID);
        if (ui != null)
        {
            var userSettings = ui.UserSettings;

            // Store configuration based on the current tab
            switch (SourceType)
            {
                case MediaSourceEnum.Content:
                    // Actualize configuration
                    {
                        userSettings.UserDialogsConfiguration["content.sitename"] = SiteName;
                        userSettings.UserDialogsConfiguration["content.path"] = GetContentPath(NodeID);
                        userSettings.UserDialogsConfiguration["content.viewmode"] = CMSDialogHelper.GetDialogViewMode(menuElem.SelectedViewMode);
                    }
                    break;

                case MediaSourceEnum.DocumentAttachments:
                    // Actualize configuration
                    userSettings.UserDialogsConfiguration["attachments.viewmode"] = CMSDialogHelper.GetDialogViewMode(menuElem.SelectedViewMode);
                    break;
            }

            userSettings.UserDialogsConfiguration["selectedtab"] = CMSDialogHelper.GetMediaSource(SourceType);

            UserInfo.Provider.Set(ui);
        }
    }


    /// <summary>
    /// Initializes dialogs according URL configuration, selected item or user configuration.
    /// </summary>
    private void InitializeDialogs()
    {
        if (!RequestHelper.IsPostBack())
        {
            LoadDialogConfiguration();

            // Item is selected in the editor
            if (MediaSource != null)
            {
                LoadItemConfiguration();
            }
            else if (!IsCopyMoveLinkDialog)
            {
                LoadUserConfiguration();
            }
        }
    }


    /// <summary>
    /// Loads dialogs according configuration coming from the URL.
    /// </summary>
    private void LoadDialogConfiguration()
    {
        if (SourceType == MediaSourceEnum.Content)
        {
            LoadSite();

            pnlUpdateSelectors.Update();
        }
    }


    /// <summary>
    /// Configures the site selector and preselects the site based on configuration
    /// </summary>
    private void LoadSite()
    {
        var siteWhereCondition = InitSiteSelector();
        var config = Config;

        contentTree.Culture = config.Culture;

        if (!LoadSiteFromConfig(config))
        {
            LoadDefaultSite(siteWhereCondition);
        }
    }


    /// <summary>
    /// Loads the default site from available sites
    /// </summary>
    /// <param name="siteWhereCondition">Site where condition</param>
    private void LoadDefaultSite(string siteWhereCondition)
    {
        // Select default site
        string siteName = SiteContext.CurrentSiteName;

        // Try select current site
        if (!string.IsNullOrEmpty(siteName))
        {
            contentTree.SiteName = siteName;
            siteSelector.SiteName = siteName;
        }
        else
        {
            // Select first site from users sites
            DataSet ds = SiteInfo.Provider.Get()
                                         .Where(siteWhereCondition)
                                         .OrderBy("SiteDisplayName");

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                siteName = ValidationHelper.GetString(ds.Tables[0].Rows[0]["SiteName"], String.Empty);
                if (!String.IsNullOrEmpty(siteName))
                {
                    contentTree.SiteName = siteName;
                    siteSelector.SiteName = siteName;
                }
            }
        }
    }


    /// <summary>
    /// Initializes the site selector
    /// </summary>
    /// <returns>Returns site where condition which limits the available sites</returns>
    private string InitSiteSelector()
    {
        // Initialize site selector
        string siteWhereCondition = GetSiteWhere();

        siteSelector.StopProcessing = false;
        siteSelector.UniSelector.WhereCondition = siteWhereCondition;

        return siteWhereCondition;
    }


    /// <summary>
    /// Loads the site selection from the given configuration. Returns true if the site was loaded from configuration
    /// </summary>
    /// <param name="config">Dialog configuration</param>
    private bool LoadSiteFromConfig(DialogConfiguration config)
    {
        if (!string.IsNullOrEmpty(config.ContentSelectedSite))
        {
            contentTree.SiteName = config.ContentSelectedSite;
            siteSelector.SiteName = config.ContentSelectedSite;

            return true;
        }

        return false;
    }


    /// <summary>
    /// Loads selected item parameters into the selector.
    /// </summary>
    public void LoadItemConfiguration()
    {
        if (MediaSource != null)
        {
            IsItemLoaded = true;
            OriginalUrl = MediaSource.OriginalUrl;

            switch (MediaSource.SourceType)
            {
                case MediaSourceEnum.Content:
                    siteSelector.SiteID = MediaSource.SiteID;
                    contentTree.SiteName = SiteName;

                    // Try to select node in the tree
                    if (MediaSource.NodeID > 0)
                    {
                        NodeID = MediaSource.NodeID;
                        contentTree.SelectedNodeID = NodeID;
                    }
                    break;
            }

            // Reload HTML properties
            if (Config.OutputFormat == OutputFormatEnum.HTMLMedia)
            {
                // Force media properties control to load selected item
                htmlMediaProp.ViewMode = MediaSource.MediaType;
            }

            // Display properties in full size only when output format is not HTML link
            if ((SourceType == MediaSourceEnum.Content) && (Config.OutputFormat != OutputFormatEnum.HTMLLink))
            {
                DisplayFull();
            }

            htmlMediaProp.HistoryID = MediaSource.HistoryID;

            // Load properties
            ItemProperties.LoadItemProperties(Parameters);
            pnlUpdateProperties.Update();

            // Remember item to colorize
            switch (SourceType)
            {
                case MediaSourceEnum.MetaFile:
                    LastAttachmentGuid = MediaSource.MetaFileGuid;
                    break;

                default:
                    ItemToColorize = (SourceType == MediaSourceEnum.Content) ? MediaSource.NodeGuid : MediaSource.AttachmentGuid;
                    LastAttachmentGuid = ItemToColorize;
                    break;
            }
        }

        ClearSelectedItemInfo();
    }


    /// <summary>
    /// Loads dialogs according user's configuration.
    /// </summary>
    private void LoadUserConfiguration()
    {
        var config = Config;
        var currentUser = MembershipContext.AuthenticatedUser;
        var userSettings = currentUser.UserSettings;

        if ((userSettings.UserDialogsConfiguration != null) &&
            (userSettings.UserDialogsConfiguration.ColumnNames != null))
        {
            XmlData dialogConfig = userSettings.UserDialogsConfiguration;

            string siteName = "";
            string aliasPath = "";
            string viewMode = "";

            // Store configuration based on the current tab
            switch (SourceType)
            {
                case MediaSourceEnum.Content:
                    {
                        siteName = (dialogConfig.ContainsColumn("content.sitename") ? (string)dialogConfig["content.sitename"] : "");
                        aliasPath = (dialogConfig.ContainsColumn("content.path") ? (string)dialogConfig["content.path"] : "");
                        viewMode = (dialogConfig.ContainsColumn("content.viewmode") ? (string)dialogConfig["content.viewmode"] : "");
                    }
                    break;

                case MediaSourceEnum.DocumentAttachments:
                    viewMode = (dialogConfig.ContainsColumn("attachments.viewmode") ? (string)dialogConfig["attachments.viewmode"] : "");
                    break;
            }

            // Update dialog configuration (only if ContentSelectedSite is not set)
            if (!string.IsNullOrEmpty(siteName) && string.IsNullOrEmpty(config.ContentSelectedSite))
            {
                // Check if site from user settings exists and is running
                SiteInfo si = SiteInfo.Provider.Get(siteName);
                if ((si == null) || (si.Status == SiteStatusEnum.Stopped) || !currentUser.IsInSite(siteName))
                {
                    // If not, use current site
                    siteName = SiteContext.CurrentSiteName;
                }

                siteSelector.SiteName = siteName;
            }

            if (!string.IsNullOrEmpty(aliasPath))
            {
                var isOnStartingPath = aliasPath.StartsWithCSafe(config.ContentStartingPath, true);
                var path = (isOnStartingPath ? aliasPath : config.ContentStartingPath);

                NodeID = GetContentNodeId(SiteName, path);

                // Initialize root node
                if (NodeID == 0)
                {
                    NodeID = GetContentNodeId(SiteName, "/");
                    menuElem.ShowParentButton = false;
                }

                // Select and expand node
                contentTree.SelectedNodeID = NodeID;
                contentTree.ExpandNodeID = NodeID;
            }
            else if (SourceType == MediaSourceEnum.Content)
            {
                SelectRootNode();
            }

            if (!string.IsNullOrEmpty(viewMode))
            {
                menuElem.SelectedViewMode = CMSDialogHelper.GetDialogViewMode(viewMode);
            }
        }
        else
        {
            // Initialize site selector
            if (!string.IsNullOrEmpty(config.ContentSelectedSite))
            {
                siteSelector.SiteName = config.ContentSelectedSite;
            }
            else
            {
                siteSelector.SiteID = SiteContext.CurrentSiteID;
            }

            SelectRootNode();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Ensures that required data are displayed.
    /// </summary>
    private void EnsureLoadedData()
    {
        bool processLoad = true;
        bool isLink = (Config.OutputFormat == OutputFormatEnum.BBLink || Config.OutputFormat == OutputFormatEnum.HTMLLink) ||
                      (Config.OutputFormat == OutputFormatEnum.URL && SelectableContent == SelectableContentEnum.AllContent);

        // If all content is selectable do not select root by default - leave selection empty
        if ((SelectableContent == SelectableContentEnum.AllContent) && !isLink && !IsCopyMoveLinkDialog && !RequestHelper.IsPostBack())
        {
            // Even no file is selected by default load source for the Attachment tab
            processLoad = (SourceType == MediaSourceEnum.DocumentAttachments);

            NodeID = 0;
            contentTree.SelectedNodeID = NodeID;
            ItemProperties.ClearProperties(true);
        }

        // Clear properties if link dialog is opened and no link is edited
        if (!RequestHelper.IsPostBack() && isLink && !IsItemLoaded)
        {
            ItemProperties.ClearProperties(true);
        }

        // If no action takes place
        if ((CurrentAction == "") ||
            !(isLink && CurrentAction.Contains("edit")))
        {
            if (processLoad && RequestHelper.IsPostBack())
            {
                LoadDataSource();
            }

            // Select folder coming from user/selected item configuration
            if (!RequestHelper.IsPostBack() && processLoad)
            {
                HandleFolderAction(NodeID.ToString(), false, false);

                // Handle preselecting only for content links (from content tree)
                if ((SourceType == MediaSourceEnum.Content) && (IsCopyMoveLinkDialog || IsLinkOutput) && !IsItemLoaded)
                {
                    HandleDialogSelect();
                }
            }
        }
    }


    /// <summary>
    /// Ensures that loaded site is really the one selected in the drop-down list.
    /// </summary>
    private void EnsureLoadedSite()
    {
        if (SourceType != MediaSourceEnum.DocumentAttachments)
        {
            siteSelector.Reload(true);

            // Name of the site selected in the site DDL
            string siteName = "";

            int siteId = siteSelector.SiteID;
            if (siteId > 0)
            {
                SiteInfo si = SiteInfo.Provider.Get(siteId);
                if (si != null)
                {
                    siteName = si.SiteName;
                }
            }

            if (siteName != SiteName)
            {
                SiteName = siteName;

                // Get site root by default
                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                TreeNodeObj = tree.SelectSingleNode(SiteName, "/", null, false, SystemDocumentTypes.Root, null, null, 1, true, DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS);
                NodeID = TreeNodeObj.NodeID;

                InitializeContentTree();

                contentTree.SelectedNodeID = NodeID;
                contentTree.ExpandNodeID = NodeID;

                EnsureLoadedData();
            }
        }
    }


    /// <summary>
    /// Makes sure that media elements aren't active while no folder is selected.
    /// </summary>
    private void DisplayMediaElements()
    {
        if (!IsAction)
        {
            if (!mediaView.Visible)
            {
                mediaView.Visible = true;
            }

            EnsureLoadedData();
            mediaView.Reload();
        }
    }


    /// <summary>
    /// Initializes properties controls.
    /// </summary>
    private void SetupProperties()
    {
        htmlLinkProp.Visible = false;
        htmlMediaProp.Visible = false;
        bbLinkProp.Visible = false;
        bbMediaProp.Visible = false;
        urlProp.Visible = false;
        docCopyMoveProp.Visible = false;

        ItemProperties.Visible = true;

        htmlLinkProp.StopProcessing = !htmlLinkProp.Visible;
        htmlMediaProp.StopProcessing = !htmlMediaProp.Visible;
        bbLinkProp.StopProcessing = !bbLinkProp.Visible;
        bbMediaProp.StopProcessing = !bbMediaProp.Visible;
        urlProp.StopProcessing = !urlProp.Visible;
        docCopyMoveProp.StopProcessing = !docCopyMoveProp.Visible;

        ItemProperties.Config = Config;
    }


    /// <summary>
    /// Initializes additional controls.
    /// </summary>
    private void SetupControls()
    {
        UsePermanentUrls = SourceType != MediaSourceEnum.Content;

        SetupSiteSelector();
        SetupPropertiesControl();

        InitializeMenuElement();
        InitializeDesignScripts();

        SetupMediaView();

        if (!IsInAsyncPostBack)
        {
            // Initialize scripts
            InitializeControlScripts();

            if (RequestHelper.IsPostBack() && (SourceType != MediaSourceEnum.DocumentAttachments))
            {
                siteSelector_OnSelectionChanged(this, null);
            }
        }

        // Based on the required source type perform setting of necessary controls
        if (SourceType == MediaSourceEnum.Content)
        {
            if (!IsInAsyncPostBack)
            {
                // Initialize content tree control
                InitializeContentTree();
            }
            else
            {
                contentTree.Visible = false;
                contentTree.StopProcessing = true;
            }
        }
        else
        {
            // Hide and disable content related controls
            HideContentControls();
        }

        ResetPageIndex();
    }


    /// <summary>
    /// Sets up the properties control
    /// </summary>
    private void SetupPropertiesControl()
    {
        // Set editor client id for properties
        var properties = ItemProperties;

        properties.EditorClientID = Config.EditorClientID;
        properties.IsLiveSite = IsLiveSite;
        properties.SourceType = SourceType;
    }


    /// <summary>
    /// Sets up the site selector control
    /// </summary>
    private void SetupSiteSelector()
    {
        if (SourceType != MediaSourceEnum.DocumentAttachments)
        {
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.UniSelector.OnSelectionChanged += siteSelector_OnSelectionChanged;
            siteSelector.AdditionalDropDownCSSClass = "DialogSiteDropdown";
        }
        else
        {
            siteSelector.StopProcessing = true;
            pnlUpdateSelectors.Visible = false;
        }
    }


    /// <summary>
    /// Sets up the media view control
    /// </summary>
    private void SetupMediaView()
    {
        if (Config.CustomFormatCode == "linkdoc")
        {
            if (!IsPostBack)
            {
                MultipleSelectionDialogHelper.ClearSelectedPages();
            }
            mediaView.ListViewControl.SelectionJavascript = "selectDocument";
            PreselectItems();
        }

        mediaView.UsePermanentUrls = UsePermanentUrls;
        mediaView.IsLiveSite = IsLiveSite;
        mediaView.SelectableContent = SelectableContent;
        mediaView.SourceType = SourceType;
        mediaView.ViewMode = menuElem.SelectedViewMode;
        mediaView.ResizeToHeight = Config.ResizeToHeight;
        mediaView.ResizeToMaxSideSize = Config.ResizeToMaxSideSize;
        mediaView.ResizeToWidth = Config.ResizeToWidth;
        mediaView.AttachmentNodeParentID = Config.AttachmentParentID;
        mediaView.ListReloadRequired += mediaView_ListReloadRequired;
        mediaView.ListViewControl.OnBeforeSorting += ListViewControl_OnBeforeSorting;
        mediaView.ListViewControl.GridView.RowDataBound += GridView_OnRowDataBound;
        mediaView.InnerMediaControl.PageSizeDropDownList.SelectedIndexChanged += PageSizeDropDownList_SelectedIndexChanged;
    }


    private void PreselectItems()
    {
        mediaView.ListViewControl.SelectedItems = MultipleSelectionDialogHelper.GetSelectedPageIds().Select(id => id.ToString()).ToList();
    }


    private void GridView_OnRowDataBound(object sender, WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == WebControls.DataControlRowType.Header)
        {
            if (Config.CustomFormatCode == "linkdoc")
            {
                var checkbox = ControlsHelper.GetChildControl<CMSCheckBox>(e.Row);

                if (checkbox != null)
                {
                    checkbox.Visible = false;
                }
            }
        }
    }


    protected void PageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        mediaView.ResetPageIndex();
        mediaView_ListReloadRequired();
    }


    protected void ListViewControl_OnBeforeSorting(object sender, EventArgs e)
    {
        // Reload data for new sorting
        LoadDataSource();
        mediaView.Reload();
    }


    /// <summary>
    /// Initializes menu element.
    /// </summary>
    private void InitializeMenuElement()
    {
        menuElem.IsCopyMoveLinkDialog = IsCopyMoveLinkDialog;
        menuElem.DisplayMode = DisplayMode;
        menuElem.IsLiveSite = IsLiveSite;
        menuElem.SourceType = SourceType;
        menuElem.ResizeToHeight = Config.ResizeToHeight;
        menuElem.ResizeToMaxSideSize = Config.ResizeToMaxSideSize;
        menuElem.ResizeToWidth = Config.ResizeToWidth;

        if (SourceType == MediaSourceEnum.Content)
        {
            menuElem.NodeID = NodeID;
        }
        else
        {
            // Initialize menu element for attachments
            menuElem.DocumentID = Config.AttachmentDocumentID;
            menuElem.FormGUID = Config.AttachmentFormGUID;
            menuElem.NodeClassName = Config.AttachmentNodeClassName;
            menuElem.ParentNodeID = Config.AttachmentParentID;
            menuElem.MetaFileObjectID = Config.MetaFileObjectID;
            menuElem.MetaFileObjectType = Config.MetaFileObjectType;
            menuElem.MetaFileCategory = Config.MetaFileCategory;
        }
        menuElem.UpdateViewMenu();
    }


    /// <summary>
    /// Initializes all the script required for communication between controls.
    /// </summary>
    protected void InitializeControlScripts()
    {
        // Prepare for upload
        string refreshType = CMSDialogHelper.GetMediaSource(SourceType);
        string cmdName;

        switch (SourceType)
        {
            case MediaSourceEnum.DocumentAttachments:
                cmdName = "attachment";
                break;
            case MediaSourceEnum.MetaFile:
                cmdName = "metafile";
                break;
            default:
                cmdName = "content";
                break;
        }

        ltlScript.Text = ScriptHelper.GetScript(String.Format(@"
function SetAction(action, argument) {{
    var hdnAction = document.getElementById('{0}');
    var hdnArgument = document.getElementById('{1}');
    if ((hdnAction != null) && (hdnArgument != null)) {{
        if (action != null) {{
            hdnAction.value = action;
        }}
        if (argument != null) {{
            hdnArgument.value = argument;
        }}
    }}
}}

function InitRefresh_{2}(message, fullRefresh, refreshTree, itemInfo, action) {{
    if((message != null) && (message != ''))
    {{
        window.alert(message);
    }}
    else
    {{
        if(action == 'insert')
        {{
            SetAction('{3}created', itemInfo);
        }}
        else if(action == 'update')
        {{
            SetAction('{3}updated', itemInfo);
        }}
        else if(action == 'refresh')
        {{
            SetAction('{3}edit', itemInfo);
        }}
        RaiseHiddenPostBack();
    }}
}}

function imageEdit_AttachmentRefresh(arg){{
    SetAction('attachmentedit', arg);
    RaiseHiddenPostBack();
}}

function imageEdit_ContentRefresh(arg){{
    SetAction('contentedit', arg);
    RaiseHiddenPostBack();
}}

function RaiseHiddenPostBack(){{
    {4};
}}
",
            hdnAction.ClientID,
            hdnArgument.ClientID,
            refreshType,
            cmdName,
            ControlsHelper.GetPostBackEventReference(hdnButton, ""))
        );
    }


    /// <summary>
    /// Loads all files for the view control.
    /// </summary>
    private void LoadDataSource()
    {
        switch (SourceType)
        {
            case MediaSourceEnum.Content:
                LoadContentDataSource(LastSearchedValue);
                break;

            case MediaSourceEnum.MetaFile:
                LoadMetaFileDataSource(LastSearchedValue);
                break;

            default:
                LoadAttachmentsDataSource(LastSearchedValue);
                break;
        }
    }


    private void mediaView_ListReloadRequired()
    {
        LoadDataSource();
        mediaView.Reload();
    }


    /// <summary>
    /// Performs actions necessary to select particular item from a list.
    /// </summary>
    private void SelectMediaItem(string argument)
    {
        if (string.IsNullOrEmpty(argument))
        {
            return;
        }

        Hashtable argTable = ContentMediaView.GetArgumentsTable(argument);
        if (argTable.Count < 2)
        {
            return;
        }

        bool isMetaFile = (SourceType == MediaSourceEnum.MetaFile);
        string ext = (!isMetaFile ? argTable["attachmentextension"].ToString() : argTable["metafileextension"].ToString());

        // Check if selected file from tree is allowed to be selected for URL
        if (Config.OutputFormat == OutputFormatEnum.URL)
        {
            if ((SelectableContent == SelectableContentEnum.OnlyImages) && !ImageHelper.IsImage(ext))
            {
                return;
            }

            if (SelectableContent == SelectableContentEnum.OnlyMedia)
            {
                var isMedia = ImageHelper.IsImage(ext) || MediaHelper.IsAudioVideo(ext);
                if (!isMedia)
                {
                    return;
                }
            }
        }

        // Get information from argument
        string name = argTable["name"].ToString();
        int imageWidth = (!isMetaFile ? ValidationHelper.GetInteger(argTable["attachmentimagewidth"], 0) : ValidationHelper.GetInteger(argTable["metafileimagewidth"], 0));
        int imageHeight = (!isMetaFile ? ValidationHelper.GetInteger(argTable["attachmentimageheight"], 0) : ValidationHelper.GetInteger(argTable["metafileimageheight"], 0));
        int nodeID = ValidationHelper.GetInteger(argTable["nodeid"], NodeID);
        long size = (!isMetaFile ? ValidationHelper.GetLong(argTable["attachmentsize"], 0) : ValidationHelper.GetInteger(argTable["metafilesize"], 0));
        string url = argTable["url"].ToString();
        string aliasPath = null;

        // Do not update properties when selecting recently edited image item
        bool avoidPropUpdate = false;

        // Remember last selected attachment GUID
        Guid attGuid;
        switch (SourceType)
        {
            case MediaSourceEnum.DocumentAttachments:
                {
                    attGuid = ValidationHelper.GetGuid(argTable["attachmentguid"], Guid.Empty);

                    avoidPropUpdate = (LastAttachmentGuid == attGuid);

                    LastAttachmentGuid = attGuid;
                    ItemToColorize = LastAttachmentGuid;
                }
                break;

            case MediaSourceEnum.MetaFile:
                LastAttachmentGuid = ValidationHelper.GetGuid(argTable["metafileguid"], Guid.Empty);
                break;

            default:
                {
                    aliasPath = argTable["nodealiaspath"].ToString();
                    attGuid = ValidationHelper.GetGuid(argTable["attachmentguid"], Guid.Empty);
                    ItemProperties.SiteDomainName = mediaView.SiteObj.DomainName;

                    avoidPropUpdate = (ItemToColorize == attGuid);

                    ItemToColorize = ValidationHelper.GetGuid(argTable["nodeguid"], Guid.Empty);
                }
                break;
        }

        avoidPropUpdate = (avoidPropUpdate && IsEditImage);
        if (avoidPropUpdate)
        {
            return;
        }

        if (SourceType == MediaSourceEnum.DocumentAttachments)
        {
            int versionHistoryId = 0;

            if (TreeNodeObj != null)
            {
                // Get the node workflow
                WorkflowManager wm = WorkflowManager.GetInstance(TreeNodeObj.TreeProvider);
                WorkflowInfo wi = wm.GetNodeWorkflow(TreeNodeObj);
                if (wi != null)
                {
                    // Get the document version
                    versionHistoryId = TreeNodeObj.DocumentCheckedOutVersionHistoryID;
                }
            }

            MediaItem item = InitializeMediaItem(name, ext, imageWidth, imageHeight, size, url, null, versionHistoryId, nodeID, aliasPath);

            SelectMediaItem(item);
        }
        else
        {
            // Select item
            SelectMediaItem(name, ext, imageWidth, imageHeight, size, url, null, nodeID, aliasPath);
        }
    }


    /// <summary>
    /// Performs actions necessary to select particular item from a list.
    /// </summary>
    private void SelectMediaItem(string docName, string url, string aliasPath)
    {
        SelectMediaItem(docName, null, 0, 0, 0, url, null, NodeID, aliasPath);
    }


    /// <summary>
    /// Selects root node of currently selected site.
    /// </summary>
    private void SelectRootNode()
    {
        // Reset selected node to root node
        var rootPath = String.IsNullOrEmpty(CurrentUser.UserStartingAliasPath) ? "/" : CurrentUser.UserStartingAliasPath;
        NodeID = GetContentNodeId(SiteName, rootPath);
        contentTree.SelectedNodeID = NodeID;
        contentTree.ExpandNodeID = NodeID;
        menuElem.ShowParentButton = false;
    }


    /// <summary>
    /// Clears hidden control elements fo future use.
    /// </summary>
    private void ClearActionElems()
    {
        CurrentAction = "";
        hdnArgument.Value = "";
    }


    /// <summary>
    /// Displays properties in full size, not showing the listing
    /// </summary>
    private void DisplayFull()
    {
        if (divDialogView.Attributes["class"].StartsWithCSafe("DialogViewContent", true))
        {
            divDialogView.Attributes["class"] = "DialogElementHidden";
            divDialogResizer.Attributes["class"] = "DialogElementHidden";
            divDialogProperties.Attributes["class"] = "DialogPropertiesFullSize";

            if (IsFullDisplay)
            {
                pnlUpdateView.Update();
                pnlUpdateProperties.Update();
            }
            else
            {
                pnlUpdateContent.Update();
                IsFullDisplay = true;
            }
        }
    }


    /// <summary>
    /// Displays properties in default size.
    /// </summary>
    private void DisplayNormal()
    {
        if (divDialogView.Attributes["class"].EqualsCSafe("DialogElementHidden", true))
        {
            divDialogView.Attributes["class"] = "DialogViewContent scroll-area";
            divDialogResizer.Attributes["class"] = "DialogResizerVLine";
            divDialogProperties.Attributes["class"] = "DialogProperties";

            if (IsFullDisplay)
            {
                pnlUpdateContent.Update();
                IsFullDisplay = false;
            }
            else
            {
                pnlUpdateView.Update();
                pnlUpdateProperties.Update();
            }
        }
    }


    /// <summary>
    /// Ensures that filter is no more applied.
    /// </summary>
    private void ResetSearchFilter()
    {
        mediaView.ResetSearch();
        LastSearchedValue = "";
    }


    /// <summary>
    /// Ensures first page is displayed in the control displaying the content if necessary
    /// </summary>
    private void ResetPageIndex()
    {
        // If folder was changed reset current page index for control displaying content
        switch (CurrentAction)
        {
            case "morecontentselect":
            case "contentselect":
            case "parentselect":
                mediaView.ResetPageIndex();
                break;
        }
    }

    #endregion


    #region "Content methods"

    /// <summary>
    /// Hides and disables content related controls.
    /// </summary>
    private void HideContentControls()
    {
        pnlLeftContent.Visible = false;
        plcSeparator.Visible = false;
        pnlRightContent.CssClass = "DialogCompleteBlock";
        siteSelector.StopProcessing = true;
        contentTree.StopProcessing = true;
    }


    /// <summary>
    /// Initializes content tree element.
    /// </summary>
    private void InitializeContentTree()
    {
        contentTree.Visible = true;

        contentTree.DeniedNodePostback = false;
        contentTree.NodeTextTemplate = "<span class=\"ContentTreeItem\" onclick=\"SelectNode(##NODEID##, this); SetAction('contentselect', '##NODEID##'); RaiseHiddenPostBack(); return false;\">##ICON##<span class=\"Name\">##NODENAME##</span></span>##STATUSICONS##";
        contentTree.SelectedNodeTextTemplate = "<span id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"SelectNode(##NODEID##, this); SetAction('contentselect', '##NODEID##'); RaiseHiddenPostBack(); return false;\">##ICON##<span class=\"Name\">##NODENAME##</span></span>##STATUSICONS##";
        contentTree.MaxTreeNodeText = String.Format("<span class=\"ContentTreeItem\" onclick=\"SetAction('morecontentselect', ##PARENTNODEID##); RaiseHiddenPostBack(); return false;\"><span class=\"Name\">{0}</span></span>", GetString("general.SeeListing"));
        contentTree.IsLiveSite = IsLiveSite;
        contentTree.SelectOnlyPublished = IsLiveSite;
        contentTree.SelectPublishedData = IsLiveSite;

        contentTree.SiteName = SiteName;

        // Starting path node ID
        StartingPathNodeID = GetStartNodeId();

        // Select root node for first request
        if (!RequestHelper.IsPostBack() && (NodeID == 0))
        {
            NodeID = StartingPathNodeID;
        }
    }


    /// <summary>
    /// Returns ID of the starting node according current starting path settings.
    /// </summary>
    private int GetStartNodeId()
    {
        var config = Config;

        if (!string.IsNullOrEmpty(config.ContentStartingPath))
        {
            contentTree.Path = config.ContentStartingPath;
        }
        else if (!string.IsNullOrEmpty(MembershipContext.AuthenticatedUser.UserStartingAliasPath))
        {
            contentTree.Path = MembershipContext.AuthenticatedUser.UserStartingAliasPath;
        }

        return GetContentNodeId(SiteName, contentTree.Path);
    }


    /// <summary>
    /// Applies loaded nodes to the view control.
    /// </summary>
    /// <param name="nodes">Nodes to load</param>
    private void LoadNodes(DataSet nodes)
    {
        bool originalNotEmpty = !DataHelper.DataSourceIsEmpty(nodes);
        if (!DataHelper.DataSourceIsEmpty(nodes))
        {
            mediaView.DataSource = nodes;
        }
        else if (originalNotEmpty && IsLiveSite)
        {
            mediaView.InfoText = GetString("dialogs.document.NotAuthorizedToViewAny");
        }
    }


    /// <summary>
    /// Gets all child nodes in the specified parent path.
    /// </summary>
    /// <param name="searchText">Text to filter searched nodes</param>
    /// <param name="parentAliasPath">Alias path of the parent</param>
    /// <param name="siteName">Name of the related site</param>
    /// <param name="totalRecords">Total records</param>
    private DataSet GetNodes(string searchText, string parentAliasPath, string siteName, out int totalRecords)
    {
        var query = GetQuery(searchText, parentAliasPath, siteName);

        DataSet nodes = query.Result;
        totalRecords = query.TotalRecords;

        return nodes;
    }


    private MultiDocumentQuery GetQuery(string searchText, string parentAliasPath, string siteName)
    {
        var where = GetWhereCondition(searchText);
        var classNames = GetClassNames(siteName, parentAliasPath);
        var orderBy = mediaView.ListViewControl.SortDirect;

        // Get nodes with coupled data to be able to recover presentation URL in case coupled data macro is used in URL pattern
        var query = GetBaseQuery(siteName, parentAliasPath)
            .WithCoupledColumns()
            .Types(classNames)
            .PublishedVersion(IsLiveSite)
            .Where(where)
            .OrderBy(orderBy);

        // Don't use paged query for searching (works only for first page)
        if (!IsSearchMode(searchText))
        {
            ApplyPaging(query);
        }

        return query;
    }


    private WhereCondition GetWhereCondition(string searchText)
    {
        var where = new WhereCondition().WhereNotEquals("NodeAliasPath", "/");
        AddWhereConditionForSearch(searchText, where);

        return where;
    }


    private string[] GetClassNames(string siteName, string parentAliasPath)
    {
        var query = GetBaseQuery(siteName, parentAliasPath)
            .PublishedVersion()
            .Column("ClassName")
            .Distinct();

        return query
            .GetListResult<string>()
            .Distinct(StringComparer.InvariantCultureIgnoreCase)
            .ToArray();
    }


    private void ApplyPaging(MultiDocumentQuery query)
    {
        query.Page(mediaView.CurrentPage - 1, mediaView.CurrentPageSize);
    }


    private void AddCombineWithCultureSettings(MultiDocumentQuery query, string siteName)
    {
        if (IsFullListingMode)
        {
            query.CombineWithAnyCulture();
        }
        else
        {
            var combine = SiteInfoProvider.CombineWithDefaultCulture(siteName);
            query.CombineWithDefaultCulture(combine);
        }
    }


    private static void AddWhereConditionForSearch(string searchText, WhereCondition where)
    {
        if (IsSearchMode(searchText))
        {
            where.Where(new WhereCondition().WhereContains("DocumentName", searchText));
        }
    }


    private static bool IsSearchMode(string searchText)
    {
        return !string.IsNullOrEmpty(searchText);
    }


    private MultiDocumentQuery GetBaseQuery(string siteName, string parentAliasPath)
    {
        var query = DocumentHelper.GetDocuments()
                             .Published(IsLiveSite)
                             .Culture(Config.Culture)
                             .NestingLevel(1)
                             .OnSite(siteName)
                             .Path(parentAliasPath, PathTypeEnum.Children)
                             .CheckPermissions();

        AddCombineWithCultureSettings(query, siteName);

        return query;
    }



    /// <summary>
    /// Loads all files for the view control.
    /// </summary>
    /// <param name="searchText">Text to filter loaded files</param>
    private void LoadContentDataSource(string searchText)
    {
        DataSet nodes = null;
        int totalRecords = -1;

        // Load data
        if (NodeID > 0)
        {
            // Get selected node            
            TreeNode node = TreeNodeObj;
            if (TreeNodeObj != null)
            {
                // Get selected node site info
                var si = SiteInfo.Provider.Get(node.NodeSiteID);
                if (si != null)
                {
                    bool fullDisplay = false;

                    if (node.TreeProvider.CheckDocumentUIPermissions(si.SiteName) && (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) != AuthorizationResultEnum.Allowed))
                    {
                        return;
                    }

                    // Check permissions
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.ExploreTree) == AuthorizationResultEnum.Allowed)
                    {
                        fullDisplay = LoadDocument(node);
                    }
                    else
                    {
                        mediaView.InfoText = GetString("dialogs.document.NotAuthorizedToExpolore");
                    }

                    if (fullDisplay)
                    {
                        // Display the properties in full mode, not showing the listing
                        DisplayFull();
                    }
                    else
                    {
                        // In normal mode, load the documents into the listing
                        nodes = GetNodes(searchText, node.NodeAliasPath, si.SiteName, out totalRecords);

                        LoadNodes(nodes);

                        DisplayNormal();
                    }
                }
            }
            else
            {
                DisplayNormal();
            }
        }

        mediaView.DataSource = nodes;
        mediaView.TotalRecords = totalRecords;
    }


    private string GetNodeUrl(TreeNode node)
    {
        return mediaView.GetContentItemUrl(node);
    }


    /// <summary>
    /// Loads the given document node as the selected item
    /// </summary>
    /// <param name="node">Document</param>
    /// <returns>Returns true if only the document properties should be displayed in full mode</returns>
    private bool LoadDocument(TreeNode node)
    {
        // Check if the current content is selectable
        bool isSelectable = SelectableContent == SelectableContentEnum.AllContent;

        if (isSelectable && !IsFullListingMode && (IsAction || !RequestHelper.IsPostBack()))
        {
            if ((ItemToColorize == Guid.Empty) || (ItemToColorize == node.NodeGUID))
            {
                string linkUrl = GetNodeUrl(node);
                linkUrl = CMSDialogHelper.AppendQueryAndAnchorFromOriginalUrl(linkUrl, OriginalUrl);

                ItemToColorize = node.NodeGUID;

                SelectMediaItem(node.DocumentName, linkUrl, node.NodeAliasPath);
            }
        }

        // Display full-size properties if detailed view required
        return
            !IsCopyMoveLinkDialog &&
            !IsFullListingMode &&
            isSelectable &&
            !node.NodeHasChildren &&
            !IsLinkOutput;
    }


    /// <summary>
    /// Handles actions related to the folders.
    /// </summary>
    /// <param name="argument">Argument related to the folder action</param>
    /// <param name="reloadTree">Indicates whether to reload tree</param>
    /// <param name="callSelection">Indicates if selection should be called</param>
    private void HandleFolderAction(string argument, bool reloadTree, bool callSelection = true)
    {
        NodeID = ValidationHelper.GetInteger(argument, 0);

        // Update new folder information
        menuElem.NodeID = NodeID;
        menuElem.UpdateViewMenu();

        // Reload content tree if new folder was created
        if (reloadTree)
        {
            InitializeContentTree();

            // Fill with new info
            contentTree.SelectedNodeID = NodeID;
            contentTree.ExpandNodeID = NodeID;

            contentTree.ReloadData();
            pnlUpdateTree.Update();

            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "EnsureTopWindow", ScriptHelper.GetScript("if (self.focus) { self.focus(); }"));
        }

        // Load new data 
        LoadDataSource();

        // Load selected item
        var attachment = CurrentAttachmentInfo;
        if (attachment != null)
        {
            SelectAttachment(callSelection, attachment);
        }

        // Get parent node ID info
        int parentId = StartingPathNodeID != NodeID ? GetParentNodeID(NodeID) : 0;
        if (parentId > 0)
        {
            // Show parent button and setup correct parent node ID
            menuElem.ShowParentButton = true;
            menuElem.ParentNodeID = parentId;
        }
        else
        {
            // Parent button is not needed
            menuElem.ShowParentButton = false;
        }

        // Reload view control's content
        mediaView.Reload();
        pnlUpdateView.Update();

        ClearActionElems();
    }


    /// <summary>
    /// Selects the given attachment as current item
    /// </summary>
    /// <param name="callSelection">If true, the action to select media item should be raised</param>
    /// <param name="attachment"></param>
    protected void SelectAttachment(bool callSelection, DocumentAttachment attachment)
    {
        string fileName = Path.GetFileNameWithoutExtension(attachment.AttachmentName);

        if (callSelection)
        {
            SelectMediaItem(
                fileName,
                attachment.AttachmentExtension,
                attachment.AttachmentImageWidth,
                attachment.AttachmentImageHeight,
                attachment.AttachmentSize,
                attachment.AttachmentUrl
                );
        }

        if (SourceType == MediaSourceEnum.DocumentAttachments)
        {
            ItemToColorize = attachment.AttachmentGUID;
        }
        else if (TreeNodeObj != null && attachment.AttachmentDocumentID == TreeNodeObj.DocumentID)
        {
            ItemToColorize = TreeNodeObj.NodeGUID;
        }
    }


    /// <summary>
    /// Handles actions occurring when new content file document was created.
    /// </summary>
    /// <param name="argument">Argument holding information on new document node ID</param>
    private void HandleContentFileCreatedAction(string argument)
    {
        string[] argArr = argument.Split('|');
        if (argArr.Length == 1)
        {
            HandleFolderAction(argArr[0], true);
        }
    }


    /// <summary>
    /// Handles attachment edit action.
    /// </summary>
    /// <param name="argument">Attachment GUID coming from view control</param>
    private void HandleContentEdit(string argument)
    {
        IsEditImage = true;

        if (!string.IsNullOrEmpty(argument))
        {
            var argTable = ContentMediaView.GetArgumentsTable(argument);
            var attachmentGuid = ValidationHelper.GetGuid(argTable["attachmentguid"], Guid.Empty);

            // Node ID was specified
            int nodeId = 0;
            if (argTable.Count == 2)
            {
                nodeId = ValidationHelper.GetInteger(argTable["nodeid"], 0);
            }

            var ai = AttachmentInfo.Provider.Get(attachmentGuid, SiteID);
            if (ai != null)
            {
                var node = GetAttachmentNodeById(nodeId, ai);

                if (node != null)
                {
                    // Check node site
                    if (node.NodeSiteID != SiteContext.CurrentSiteID)
                    {
                        mediaView.SiteObj = SiteInfo.Provider.Get(node.NodeSiteID);
                    }

                    // Get node URL
                    string url = GetNodeUrl(node);

                    // Update properties if node is currently selected
                    if (attachmentGuid == ItemToColorize)
                    {
                        SelectMediaItem(ai.AttachmentName, ai.AttachmentExtension, ai.AttachmentImageWidth, ai.AttachmentImageHeight, ai.AttachmentSize, url, null, node.NodeID, node.NodeAliasPath);
                    }

                    // Update select action to reflect changes made during editing
                    LoadDataSource();
                    mediaView.Reload();
                    pnlUpdateView.Update();
                }
            }
        }

        ClearActionElems();
    }


    private static TreeNode GetAttachmentNodeById(int nodeId, AttachmentInfo ai)
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        if (nodeId > 0)
        {
            return tree.SelectSingleNode(nodeId, LocalizationContext.PreferredCultureCode);
        }

        return tree.SelectSingleDocument(ai.AttachmentDocumentID);
    }

    #endregion


    #region "Attachment methods"

    /// <summary>
    /// Loads all attachments for the view control.
    /// </summary>
    /// <param name="searchText">Text to filter loaded files</param>
    private void LoadAttachmentsDataSource(string searchText)
    {
        DataSet attachments = null;
        int totalRecords = -1;
        int currentPageSize = mediaView.CurrentPageSize;

        // Only unsorted attachments are being displayed
        string where = String.IsNullOrEmpty(searchText) ? "(AttachmentIsUnsorted = 1)" : SqlHelper.AddWhereCondition("(AttachmentIsUnsorted = 1)", String.Format("(AttachmentName LIKE N'%{0}%')", SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(searchText))));

        // Get document attachments
        if (Config.AttachmentDocumentID != 0)
        {
            if (TreeNodeObj != null)
            {
                // Check permissions
                if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNodeObj, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed)
                {
                    // Get the node workflow
                    var wi = TreeNodeObj.GetWorkflow();
                    int versionHistoryId = TreeNodeObj.DocumentCheckedOutVersionHistoryID;

                    // If document uses workflow, get the attachments from version history
                    if ((wi != null) && (versionHistoryId > 0))
                    {
                        // Get only main attachments for given version
                        var query =
                            AttachmentHistoryInfo.Provider.Get()
                                .InVersionExceptVariants(versionHistoryId)
                                .Where(where)
                                .OrderBy(mediaView.ListViewControl.SortDirect)
                                .BinaryData(false)
                                .Page(mediaView.CurrentPage - 1, mediaView.CurrentPageSize);

                        attachments = query.Result;
                        totalRecords = query.TotalRecords;
                    }
                    else
                    {
                        // Get only main attachments for published document
                        var query =
                            AttachmentInfo.Provider.Get()
                                .ExceptVariants()
                                .Where(where)
                                .WhereEquals("AttachmentDocumentID", TreeNodeObj.DocumentID)
                                .OrderBy(mediaView.ListViewControl.SortDirect)
                                .BinaryData(false)
                                .Page(mediaView.CurrentPage - 1, mediaView.CurrentPageSize);

                        attachments = query.Result;
                        totalRecords = query.TotalRecords;
                    }
                }
            }
        }
        // Get form attachments
        else if (AttachmentsAreTemporary)
        {
            var query =
                AttachmentInfo.Provider.Get()
                    .ExceptVariants()
                    .Where(where)
                    .WhereEquals("AttachmentFormGUID", Config.AttachmentFormGUID)
                    .OrderBy("AttachmentOrder", "AttachmentName")
                    .BinaryData(false)
                    .TopN(currentPageSize);

            query.Offset = mediaView.CurrentOffset;
            query.MaxRecords = currentPageSize;

            attachments = query.Result;
            mediaView.TotalRecords = query.TotalRecords;
        }

        if (DataHelper.DataSourceIsEmpty(attachments) && mediaView.CurrentPage > 1)
        {
            // Switch to previous page if current page has no data
            mediaView.CurrentPage -= 1;
            LoadAttachmentsDataSource(searchText);
            return;
        }

        mediaView.DataSource = attachments;
        mediaView.TotalRecords = totalRecords;
    }


    /// <summary>
    /// Handles new attachment create action.
    /// </summary>
    /// <param name="argument">Argument coming from upload control</param>
    private void HandleAttachmentCreatedAction(string argument)
    {
        HandleAttachmentAction(argument, false);

        // Reload view
        LoadDataSource();

        mediaView.Reload();
        pnlUpdateView.Update();
    }


    /// <summary>
    /// Handles attachment update action.
    /// </summary>
    /// <param name="argument">Argument coming from upload control</param>
    private void HandleAttachmentUpdatedAction(string argument)
    {
        HandleAttachmentAction(argument, true);
    }


    /// <summary>
    /// Handles attachment edit action.
    /// </summary>
    /// <param name="argument">Attachment GUID coming from view control</param>
    private void HandleAttachmentEdit(string argument)
    {
        IsEditImage = true;

        if (!string.IsNullOrEmpty(argument))
        {
            var argArr = argument.Split('|');
            var attachmentGuid = ValidationHelper.GetGuid(argArr[1], Guid.Empty);

            DocumentAttachment ai;

            int versionHistoryId = 0;
            if (TreeNodeObj != null)
            {
                versionHistoryId = TreeNodeObj.DocumentCheckedOutVersionHistoryID;
            }

            if (versionHistoryId == 0)
            {
                ai = (DocumentAttachment)AttachmentInfo.Provider.Get(attachmentGuid, SiteContext.CurrentSiteID);
            }
            else
            {
                // Get the attachment version data
                ai = DocumentHelper.GetAttachment(attachmentGuid, versionHistoryId, false);
            }

            if (ai != null)
            {
                string url = mediaView.GetAttachmentItemUrl(ai.AttachmentGUID, ai.AttachmentName, ai.AttachmentImageHeight, ai.AttachmentImageWidth, 0);

                if (LastAttachmentGuid == attachmentGuid)
                {
                    SelectMediaItem(ai.AttachmentName, ai.AttachmentExtension, ai.AttachmentImageWidth, ai.AttachmentImageHeight, ai.AttachmentSize, url);
                }

                // Update select action to reflect changes made during editing
                LoadDataSource();
                mediaView.Reload();
                pnlUpdateView.Update();
            }
        }

        ClearActionElems();
    }


    /// <summary>
    /// Handles attachment action.
    /// </summary>
    /// <param name="argument">Argument coming from upload control</param>
    /// <param name="isUpdate">Indicates if is update</param>
    private void HandleAttachmentAction(string argument, bool isUpdate)
    {
        // Get attachment URL first
        var attachmentGuid = ValidationHelper.GetGuid(argument, Guid.Empty);
        if (attachmentGuid != Guid.Empty)
        {
            // Ensure site information
            var si = SiteContext.CurrentSite;
            if ((TreeNodeObj != null) && (si.SiteID != TreeNodeObj.NodeSiteID))
            {
                si = SiteInfo.Provider.Get(TreeNodeObj.NodeSiteID);
            }

            var ai = DocumentHelper.GetAttachment(attachmentGuid, si.SiteName, false);
            if (ai != null)
            {
                if (CMSDialogHelper.IsItemSelectable(SelectableContent, ai.AttachmentExtension))
                {
                    // Get attachment URL
                    string url = mediaView.GetAttachmentItemUrl(ai.AttachmentGUID, ai.AttachmentName, 0, 0, 0);

                    // Remember last selected attachment GUID
                    if (SourceType == MediaSourceEnum.DocumentAttachments)
                    {
                        LastAttachmentGuid = ai.AttachmentGUID;
                    }

                    // Get the node workflow
                    int versionHistoryId = 0;
                    if (TreeNodeObj != null)
                    {
                        var wm = WorkflowManager.GetInstance(TreeNodeObj.TreeProvider);
                        var wi = wm.GetNodeWorkflow(TreeNodeObj);
                        if (wi != null)
                        {
                            // Ensure the document version
                            var vm = VersionManager.GetInstance(TreeNodeObj.TreeProvider);
                            versionHistoryId = vm.EnsureVersion(TreeNodeObj, TreeNodeObj.IsPublished);
                        }

                        ItemToColorize = (SourceType == MediaSourceEnum.DocumentAttachments) ? attachmentGuid : TreeNodeObj.NodeGUID;
                    }

                    var item = InitializeMediaItem(ai.AttachmentName, ai.AttachmentExtension, ai.AttachmentImageWidth, ai.AttachmentImageHeight, ai.AttachmentSize, url, null, versionHistoryId, 0, "");

                    SelectMediaItem(item);
                }
                else
                {
                    // Unselect old attachment and clear properties
                    ItemProperties.ClearProperties(true);
                    pnlUpdateProperties.Update();
                }

                mediaView.InfoText = (isUpdate ? GetString("dialogs.attachment.updated") : GetString("dialogs.attachment.created"));

                pnlUpdateView.Update();
            }
        }

        ClearActionElems();
    }


    /// <summary>
    /// Handles actions occurring when attachment is moved.
    /// </summary>
    /// <param name="argument">Argument holding information on attachment being moved</param>
    /// <param name="action">Action specifying whether the attachment is moved up/down</param>
    private void HandleAttachmentMoveAction(string argument, string action)
    {
        // Check permissions
        string errMsg = CheckAttachmentPermissions();

        if (errMsg == "")
        {
            Guid attachmentGuid = ValidationHelper.GetGuid(argument, Guid.Empty);
            if (attachmentGuid != Guid.Empty)
            {
                // Move temporary attachment
                if (!AttachmentsAreTemporary)
                {
                    var workflow = TreeNodeObj.GetWorkflow();
                    var useAutomaticCheckInOut = (workflow != null) && !workflow.UseCheckInCheckOut(TreeNodeObj.NodeSiteName);

                    if (action == "attachmentmoveup")
                    {
                        PerformAttachmentAction(TreeNodeObj, useAutomaticCheckInOut, () => DocumentHelper.MoveAttachmentUp(attachmentGuid, TreeNodeObj));
                    }
                    else
                    {
                        PerformAttachmentAction(TreeNodeObj, useAutomaticCheckInOut, () => DocumentHelper.MoveAttachmentDown(attachmentGuid, TreeNodeObj));
                    }
                }
                else
                {
                    var ai = AttachmentInfoProvider.GetTemporaryAttachmentInfo(attachmentGuid);
                    if (ai != null)
                    {
                        if (action == "attachmentmoveup")
                        {
                            ai.Generalized.MoveObjectUp();
                        }
                        else
                        {
                            ai.Generalized.MoveObjectDown();
                        }
                    }
                }

                // Reload data
                LoadDataSource();

                mediaView.Reload();
            }
        }
        else
        {
            // Display error
            ShowError(errMsg);
        }

        ClearActionElems();

        pnlUpdateView.Update();
    }


    /// <summary>
    /// Handles actions occurring when some attachment is being removed.
    /// </summary>
    /// <param name="argument">Argument holding information on attachment</param>
    private void HandleDeleteAttachmentAction(string argument)
    {
        string errMsg = CheckAttachmentPermissions();

        if (errMsg == "")
        {
            Guid attachmentGuid = ValidationHelper.GetGuid(argument, Guid.Empty);
            if (attachmentGuid != Guid.Empty)
            {
                DeleteAttachment(TreeNodeObj, attachmentGuid);

                // Reload data
                LoadDataSource();
                mediaView.Reload();

                // Selected attachment was removed
                if (LastAttachmentGuid == attachmentGuid)
                {
                    // Reset properties
                    ItemProperties.ClearProperties();
                    pnlUpdateProperties.Update();
                }
            }
        }
        else
        {
            // Display error
            ShowError(errMsg);
        }

        pnlUpdateView.Update();
    }

    #endregion


    #region "MetaFile methods"

    /// <summary>
    /// Loads all metafiles for the view control.
    /// </summary>
    /// <param name="searchText">Text to filter loaded files</param>
    private void LoadMetaFileDataSource(string searchText)
    {
        string where = !String.IsNullOrEmpty(searchText) ? new WhereCondition().WhereContains("MetaFileName", searchText).ToString(true) : string.Empty;

        int totalRecords = -1;
        const string columns = "MetaFileID,MetaFileObjectType,MetaFileObjectID,MetaFileGroupName,MetaFileName,MetaFileExtension,MetaFileSize,MetaFileMimeType,MetaFileImageWidth,MetaFileImageHeight,MetaFileGUID,MetaFileLastModified,MetaFileSiteID,MetaFileTitle,MetaFileDescription";

        // Get metafiles
        mediaView.DataSource = MetaFileInfoProvider.GetMetaFiles(Config.MetaFileObjectID, Config.MetaFileObjectType, Config.MetaFileCategory, where, mediaView.ListViewControl.SortDirect, columns, -1, mediaView.CurrentOffset, mediaView.CurrentPageSize, ref totalRecords);
        mediaView.TotalRecords = totalRecords;
    }


    /// <summary>
    /// Checks metafile permissions.
    /// </summary>
    private string CheckMetaFilePermissions()
    {
        string message = string.Empty;

        if (!UserInfoProvider.IsAuthorizedPerObject(Config.MetaFileObjectType, Config.MetaFileObjectID, PermissionsEnum.Modify, SiteName, MembershipContext.AuthenticatedUser))
        {
            message = GetString("general.nopermission");
        }

        return message;
    }


    /// <summary>
    /// Handles actions occurring when some metafile is being removed.
    /// </summary>
    /// <param name="argument">Argument holding information on metafile</param>
    private void HandleMetaFileDelete(string argument)
    {
        // Check permissions
        string errMsg = CheckMetaFilePermissions();

        if (string.IsNullOrEmpty(errMsg))
        {
            // Get meta file ID
            int metaFileId = ValidationHelper.GetInteger(argument, 0);
            try
            {
                Guid mfGuid = Guid.Empty;

                MetaFileInfo mf = MetaFileInfo.Provider.Get(metaFileId);
                if (mf != null)
                {
                    mfGuid = mf.MetaFileGUID;
                    // Delete meta file
                    MetaFileInfo.Provider.Delete(mf);
                }

                // Reload data
                LoadDataSource();
                mediaView.Reload();

                // Selected metafile was removed
                if (LastAttachmentGuid == mfGuid)
                {
                    // Reset properties
                    ItemProperties.ClearProperties();
                    pnlUpdateProperties.Update();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        else
        {
            // Display error
            ShowError(errMsg);
        }

        pnlUpdateView.Update();

        ClearActionElems();
    }


    /// <summary>
    /// Handles metafile edit action.
    /// </summary>
    /// <param name="argument">MetaFile GUID coming from view control in format 'attachmentguid|[MetaFileGUID]'</param>
    private void HandleMetaFileEdit(string argument)
    {
        IsEditImage = true;

        if (!string.IsNullOrEmpty(argument))
        {
            string[] argArr = argument.Split('|');

            // Get meta file GUID
            Guid mfGuid = ValidationHelper.GetGuid(argArr[1], Guid.Empty);

            MetaFileInfo mf = MetaFileInfoProvider.GetMetaFileInfo(mfGuid, SiteName, false);
            if (mf != null)
            {
                // Update select action to reflect changes made during editing
                LoadDataSource();
                mediaView.Reload();

                // Reload properties section
                if (LastAttachmentGuid == mfGuid)
                {
                    string url = mediaView.GetMetaFileItemUrl(mfGuid, mf.MetaFileName, mf.MetaFileImageHeight, mf.MetaFileImageWidth, 0);
                    SelectMediaItem(mf.MetaFileName, mf.MetaFileExtension, mf.MetaFileImageWidth, mf.MetaFileImageHeight, mf.MetaFileSize, url);
                }

                pnlUpdateView.Update();
            }
        }

        ClearActionElems();
    }


    /// <summary>
    /// Handles metafile update action.
    /// </summary>
    /// <param name="argument">MetaFile ID</param>
    private void HandleMetaFileUpdated(string argument)
    {
        if (!string.IsNullOrEmpty(argument))
        {
            // Get meta file ID
            int metaFileId = ValidationHelper.GetInteger(argument, 0);

            Guid mfGuid = Guid.Empty;

            MetaFileInfo mf = MetaFileInfo.Provider.Get(metaFileId);
            if (mf != null)
            {
                mfGuid = mf.MetaFileGUID;
            }

            // Update select action to reflect changes made during update
            LoadDataSource();
            mediaView.Reload();

            // Reload properties section
            if ((LastAttachmentGuid == mfGuid) && (mf != null))
            {
                string url = mediaView.GetMetaFileItemUrl(mfGuid, mf.MetaFileName, mf.MetaFileImageHeight, mf.MetaFileImageWidth, 0);
                SelectMediaItem(mf.MetaFileName, mf.MetaFileExtension, mf.MetaFileImageWidth, mf.MetaFileImageHeight, mf.MetaFileSize, url);
            }

            pnlUpdateView.Update();
        }

        ClearActionElems();
    }

    #endregion


    #region "Common event methods"

    /// <summary>
    /// Handles actions occurring when some text is searched.
    /// </summary>
    /// <param name="argument">Argument holding information on searched text</param>
    private void HandleSearchAction(string argument)
    {
        LastSearchedValue = argument;

        // Load new data filtered by searched text 
        LoadDataSource();

        // Reload content
        mediaView.Reload();
        pnlUpdateView.Update();

        SetSearchFocus();
    }


    /// <summary>
    /// Handles actions occurring when some item is selected.
    /// </summary>
    /// <param name="argument">Argument holding information on selected item</param>
    private void HandleSelectAction(string argument)
    {
        // Create new selected media item
        SelectMediaItem(argument);

        // Forget recent action
        ClearActionElems();
    }


    /// <summary>
    /// Handles actions occurring when some item in copy/move/link/select path dialog is selected.
    /// </summary>
    private void HandleDialogSelect()
    {
        var node = TreeNodeObj;
        if (node != null)
        {
            var nodeDetails = GetNodeDetails(node, SiteName);

            // If node details exists
            if (nodeDetails != null)
            {
                string argument = mediaView.GetArgumentSet(nodeDetails);

                SelectMediaItem(String.Format("{0}|URL|{1}", argument, GetNodeUrl(nodeDetails)));

                ItemToColorize = node.NodeGUID;
            }
        }
        else
        {
            // Remove selected item
            ItemToColorize = Guid.Empty;
        }

        // Forget recent action
        ClearActionElems();
    }


    /// <summary>
    /// Handles the unavailability of the site
    /// </summary>
    private void HandleSiteEmpty()
    {
        if ((SourceType != MediaSourceEnum.DocumentAttachments) && (SourceType != MediaSourceEnum.MetaFile) && String.IsNullOrEmpty(SiteName))
        {
            contentTree.Visible = false;
            siteSelector.Enabled = false;
            lblTreeInfo.Visible = true;

            // Disable menu
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "DialogsDisableMenuActions", ScriptHelper.GetScript("if(window.DisableNewFileBtn){ window.DisableNewFileBtn(); } if(window.DisableNewFolderBtn){ window.DisableNewFolderBtn(); }"));
        }
    }

    #endregion


    #region "Event handlers"

    protected void siteSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        IsAction = true;

        // Update information on current site 
        SiteID = siteSelector.SiteID;
        if (SiteID > 0)
        {
            mediaView.SiteObj = SiteInfo.Provider.Get(SiteID);
        }

        // Reset selected node to root node
        NodeID = StartingPathNodeID = GetStartNodeId();
        if (NodeID == 0)
        {
            NodeID = GetContentNodeId(SiteName, "/");
            menuElem.ShowParentButton = false;
        }

        if (SelectableContent != SelectableContentEnum.AllContent)
        {
            contentTree.SelectedNodeID = NodeID;
            contentTree.ExpandNodeID = NodeID;
        }
        else
        {
            mediaView.DataSource = null;
        }

        // Clear item to colorize identifier (site name was changed)
        ItemToColorize = Guid.Empty;

        // Clear properties from session to set new one later
        ItemProperties.ClearProperties();

        // Reload media view for new site
        LoadDataSource();

        // Update information on parent node ID for new folder creation
        menuElem.NodeID = NodeID;
        menuElem.UpdateViewMenu();

        // Reload content tree for new site
        contentTree.SiteName = siteSelector.SiteName;
        InitializeContentTree();
        pnlUpdateTree.Update();

        // Load selected item
        var attachment = CurrentAttachmentInfo;
        if (attachment != null)
        {
            SelectMediaItem(
                attachment.AttachmentName,
                attachment.AttachmentExtension,
                attachment.AttachmentImageWidth,
                attachment.AttachmentImageHeight,
                attachment.AttachmentSize,
                attachment.AttachmentUrl
            );
        }

        // Setup media view
        mediaView.Reload();
        pnlUpdateView.Update();

        // Setup properties
        DisplayNormal();
        pnlUpdateProperties.Update();
    }


    /// <summary>
    /// Behaves as mediator in communication line between control taking action and the rest of the same level controls.
    /// </summary>
    protected void hdnButton_Click(object sender, EventArgs e)
    {
        IsAction = true;

        switch (CurrentAction)
        {
            case "insertitem":
                GetSelectedItem();
                break;

            case "search":
                HandleSearchAction(CurrentArgument);
                break;

            case "select":
                HandleSelectAction(CurrentArgument);
                break;

            case "selectdocument":
                HandleDocumentSelect(CurrentArgument);
                break;

            case "morecontentselect":
            case "contentselect":
                ResetSearchFilter();

                if (IsLinkOutput)
                {
                    CurrentAttachmentInfo = null;
                }

                // If more content is requested, enable the full listing
                if (!IsFullListingMode)
                {
                    IsFullListingMode = (CurrentAction == "morecontentselect");
                }

                HandleFolderAction(CurrentArgument, IsFullListingMode);

                if (IsCopyMoveLinkDialog || IsLinkOutput)
                {
                    HandleDialogSelect();
                }
                break;

            case "parentselect":
                ResetSearchFilter();

                HandleFolderAction(CurrentArgument, true);

                if (IsCopyMoveLinkDialog)
                {
                    HandleDialogSelect();
                }
                break;

            case "refreshtree":
                ResetSearchFilter();
                HandleFolderAction(CurrentArgument, true);
                break;

            case "contentcreated":
                HandleContentFileCreatedAction(CurrentArgument);

                if (IsCopyMoveLinkDialog)
                {
                    HandleDialogSelect();
                }
                break;

            case "closelisting":
                IsFullListingMode = false;
                HandleFolderAction(NodeID.ToString(), false);
                break;

            case "newfolder":
                ResetSearchFilter();
                HandleFolderAction(CurrentArgument, true);

                if (IsCopyMoveLinkDialog)
                {
                    // Refresh content tree when new folder is created in Copy/Move dialog
                    RefreshContentTree();
                    HandleDialogSelect();
                }
                break;

            case "cancelfolder":
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "EnsureTopWindow", ScriptHelper.GetScript("if (self.focus) { self.focus(); }"));
                ClearActionElems();
                break;

            case "attachmentmoveup":
                HandleAttachmentMoveAction(CurrentArgument, CurrentAction);
                break;

            case "attachmentmovedown":
                HandleAttachmentMoveAction(CurrentArgument, CurrentAction);
                break;

            case "attachmentdelete":
                HandleDeleteAttachmentAction(CurrentArgument);
                break;

            case "attachmentcreated":
                HandleAttachmentCreatedAction(CurrentArgument);
                break;

            case "attachmentupdated":
                HandleAttachmentUpdatedAction(CurrentArgument);
                break;

            case "attachmentedit":
                HandleAttachmentEdit(CurrentArgument);
                break;

            case "contentedit":
                HandleContentEdit(CurrentArgument);
                break;

            case "metafiledelete":
                HandleMetaFileDelete(CurrentArgument);
                break;

            case "metafileedit":
                HandleMetaFileEdit(CurrentArgument);
                break;

            case "metafileupdated":
                HandleMetaFileUpdated(CurrentArgument);
                break;

            default:
                pnlUpdateView.Update();
                break;
        }
    }


    private void HandleDocumentSelect(string argument)
    {
        if (string.IsNullOrEmpty(argument))
        {
            return;
        }

        Hashtable argTable = ContentMediaView.GetArgumentsTable(argument);
        if (argTable.Count < 2)
        {
            return;
        }

        var nodeId = ValidationHelper.GetInteger(argTable["nodeid"], 0);
        var isChecked = ValidationHelper.GetBoolean(argTable["ischecked"], false);
        var nodeData = DocumentNodeDataInfo.Provider.Get(nodeId);

        if (isChecked)
        {
            MultipleSelectionDialogHelper.AddSelectedPage(nodeId, nodeData.NodeAliasPath);
        }
        else
        {
            MultipleSelectionDialogHelper.RemoveSelectedPage(nodeId, nodeData.NodeAliasPath);
        }

        var mediaItem = InitializeMediaItem(nodeData.NodeName, string.Empty, 0, 0, 0, string.Empty, string.Empty, 0, nodeId, nodeData.NodeAliasPath);
        SelectMediaItem(mediaItem);
    }

    #endregion
}
