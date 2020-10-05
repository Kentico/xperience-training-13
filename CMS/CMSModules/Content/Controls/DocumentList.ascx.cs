using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_Controls_DocumentList : CMSUserControl, IPostBackEventHandler
{
    #region "Private and protected variables & enumerations"

    // Private fields
    private CurrentUserInfo currentUserInfo;
    private SiteInfo currentSiteInfo;
    private TreeProvider mTree;
    private TreeNode mNode;
    private HashSet<DocumentFlagsControl> mFlagsControls;
    private DataSet mSiteCultures;
    private DialogConfiguration mConfig;
    private string mCultureCode;
    private bool isRootDocument;
    private bool isCurrentDocument;

    private string mDefaultSiteCulture;
    private string currentSiteName;
    private int mNodeId;
    private int? mWOpenerNodeId;
    private bool dataLoaded;
    private bool checkPermissions;
    private bool mShowDocumentTypeIcon = true;
    private bool mDocumentNameAsLink = true;
    private bool mShowDocumentMarks = true;
    private string aliasPath;
    private string mSelectItemJSFunction = "SelectItem";
    private string mOrderBy = "NodeLevel ASC, NodeOrder ASC, NodeName ASC, NodeAlias ASC";
    private string mDeleteReturnUrl = string.Empty;
    private string mPublishReturnUrl = string.Empty;
    private string mArchiveReturnUrl = string.Empty;
    private bool? mIsEditVisible;
    private int mClassID = UniSelector.US_NONE_RECORD;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether UI element Edit is displayed.
    /// </summary>
    private bool IsEditVisible
    {
        get
        {
            if (mIsEditVisible == null)
            {
                mIsEditVisible = CurrentUser.IsAuthorizedPerUIElement(ModuleName.CONTENT, "Edit");
            }
            return mIsEditVisible.Value;
        }
    }


    /// <summary>
    /// Default culture of the site.
    /// </summary>
    private string DefaultSiteCulture
    {
        get
        {
            return mDefaultSiteCulture ?? (mDefaultSiteCulture = CultureHelper.GetDefaultCultureCode(currentSiteName));
        }
    }


    /// <summary>
    /// Hashtable with document flags controls.
    /// </summary>
    private HashSet<DocumentFlagsControl> FlagsControls
    {
        get
        {
            return mFlagsControls ?? (mFlagsControls = new HashSet<DocumentFlagsControl>());
        }
    }


    /// <summary>
    /// Site cultures.
    /// </summary>
    private DataSet SiteCultures
    {
        get
        {
            if (mSiteCultures == null)
            {
                mSiteCultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName).Copy();
                if (!DataHelper.DataSourceIsEmpty(mSiteCultures))
                {
                    DataTable cultureTable = mSiteCultures.Tables[0];
                    DataRow[] defaultCultureRow = cultureTable.Select("CultureCode='" + DefaultSiteCulture + "'");

                    // Ensure default culture to be first
                    DataRow dr = cultureTable.NewRow();
                    if (defaultCultureRow.Length > 0)
                    {
                        dr.ItemArray = defaultCultureRow[0].ItemArray;
                        cultureTable.Rows.InsertAt(dr, 0);
                        cultureTable.Rows.Remove(defaultCultureRow[0]);
                    }
                }
            }
            return mSiteCultures;
        }
    }


    /// <summary>
    /// Gets the configuration for Copy and Move dialog.
    /// </summary>
    private DialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                mConfig = new DialogConfiguration();
                mConfig.ContentSelectedSite = SiteContext.CurrentSiteName;
                mConfig.OutputFormat = OutputFormatEnum.Custom;
                mConfig.SelectableContent = SelectableContentEnum.AllContent;
                mConfig.HideAttachments = false;
            }
            return mConfig;
        }
    }


    /// <summary>
    /// Holds current where condition of filter.
    /// </summary>
    private string CurrentWhereCondition
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CurrentWhereCondition"], string.Empty);
        }
        set
        {
            ViewState["CurrentWhereCondition"] = value;
        }
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    private string Identifier
    {
        get
        {
            string identifier = hdnIdentifier.Value;
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = "DocumentListDialogIdentifier";
                hdnIdentifier.Value = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Tree provider for current user
    /// </summary>
    private TreeProvider Tree
    {
        get
        {
            return mTree ?? (mTree = new TreeProvider(MembershipContext.AuthenticatedUser)
            {
                PreferredCultureCode = CultureCode
            });
        }
    }


    /// <summary>
    /// Gets the WOpenerNodeID from the url query.
    /// </summary>
    private int WOpenerNodeID
    {
        get
        {
            if (mWOpenerNodeId == null)
            {
                mWOpenerNodeId = QueryHelper.GetInteger("wopenernodeid", 0);
            }

            return mWOpenerNodeId.Value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event raised when external source data required.
    /// </summary>
    public event OnExternalDataBoundEventHandler OnExternalAdditionalDataBound;


    /// <summary>
    /// Event raised when creating document flags control.
    /// </summary>
    public event OnExternalDataBoundEventHandler OnDocumentFlagsCreating;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the node which child nodes are to be displayed.
    /// </summary>
    public int NodeID
    {
        get
        {
            return mNodeId;
        }
        set
        {
            mNodeId = value;
            mNode = null;
        }
    }


    /// <summary>
    /// ID of the document type which child nodes are to be displayed.
    /// </summary>
    public int ClassID
    {
        get
        {
            return mClassID;
        }
        set
        {
            mClassID = value;
        }
    }


    /// <summary>
    /// Culture to consider as preferred.
    /// </summary>
    private string CultureCode
    {
        get
        {
            return mCultureCode ?? (mCultureCode = QueryHelper.GetString("culture", LocalizationContext.PreferredCultureCode));
        }
    }


    /// <summary>
    /// TreeNode object specified by NodeID property.
    /// </summary>
    public TreeNode Node
    {
        get
        {
            return mNode ?? (mNode = DocumentHelper.GetDocument(NodeID, TreeProvider.ALL_CULTURES, true, Tree));
        }
        set
        {
            mNode = value;

            if (value != null)
            {
                mNodeId = value.NodeID;
            }
        }
    }


    /// <summary>
    /// Unigrid object used for listing documents.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridDocuments;
        }
    }


    /// <summary>
    /// Additional column names for listing separated by coma ','.
    /// </summary>
    public string AdditionalColumns
    {
        get;
        set;
    }


    /// <summary>
    /// Where condition used to restrict selection of documents.
    /// </summary>
    public WhereCondition WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Default order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return mOrderBy;
        }
        set
        {
            mOrderBy = value;
        }
    }


    /// <summary>
    /// Name of the javascript function used for selecting items. Current node id and parent node id are supplied as parameters.
    /// </summary>
    public string SelectItemJSFunction
    {
        get
        {
            return mSelectItemJSFunction;
        }
        set
        {
            mSelectItemJSFunction = value;
        }
    }


    /// <summary>
    /// Name of the javascript function called when flag is clicked.
    /// </summary>
    public string SelectLanguageJSFunction
    {
        get;
        set;
    }


    /// <summary>
    /// Return URL for delete action
    /// </summary>
    public string DeleteReturnUrl
    {
        get
        {
            return mDeleteReturnUrl;
        }
        set
        {
            mDeleteReturnUrl = value;
        }
    }


    /// <summary>
    /// Return URL for publish action
    /// </summary>
    public string PublishReturnUrl
    {
        get
        {
            return mPublishReturnUrl;
        }
        set
        {
            mPublishReturnUrl = value;
        }
    }


    /// <summary>
    /// Return URL for archive action
    /// </summary>
    public string ArchiveReturnUrl
    {
        get
        {
            return mArchiveReturnUrl;
        }
        set
        {
            mArchiveReturnUrl = value;
        }
    }


    /// <summary>
    /// Return URL for translate action
    /// </summary>
    public string TranslateReturnUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether all levels of children are to be searched.
    /// </summary>
    public bool ShowAllLevels
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ShowAllLevels"], false);
        }
        set
        {
            ViewState["ShowAllLevels"] = value;
        }
    }


    /// <summary>
    /// Indicates whether document type icon will be shown before document name. True by default.
    /// </summary>
    public bool ShowDocumentTypeIcon
    {
        get
        {
            return mShowDocumentTypeIcon;
        }
        set
        {
            mShowDocumentTypeIcon = value;
        }
    }


    /// <summary>
    /// Indicates whether tooltip containing document type name will be added to document type icon.
    /// </summary>
    public bool ShowDocumentTypeIconTooltip
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether document names will be rendered as links. True by default.
    /// </summary>
    public bool DocumentNameAsLink
    {
        get
        {
            return mDocumentNameAsLink;
        }
        set
        {
            mDocumentNameAsLink = value;
        }
    }


    /// <summary>
    /// Indicates whether document marks will be shown after document name. True by default.
    /// </summary>
    public bool ShowDocumentMarks
    {
        get
        {
            return mShowDocumentMarks;
        }
        set
        {
            mShowDocumentMarks = value;
        }
    }


    /// <summary>
    /// Path where content tree in copy/move/link dialogs will start.
    /// </summary>
    public string CopyMoveLinkStartingPath
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the page is displayed as dialog. 
    /// </summary>
    public bool RequiresDialog
    {
        get;
        set;
    }


    /// <summary>
    /// If set, extenders on the UniGrid and MassActions will be initialized with the specified scope as a prefix and "UniGrid" resp. "MassActions" as the rest of the name.
    /// If left to null, extenders won't be initialized. Has to be set before Page_Init.
    /// </summary>
    public string ExtenderScopePrefix
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentSiteName = SiteContext.CurrentSiteName;
        currentUserInfo = MembershipContext.AuthenticatedUser;

        gridDocuments.FilterLimit = 1;
        gridDocuments.FilterIsSet = true;
        gridDocuments.ZeroRowsText = GetString("content.nochilddocumentsfound");

        CreateMassActionItems();

        if (!string.IsNullOrEmpty(ExtenderScopePrefix))
        {
            gridDocuments.InitializeExtenders<IExtensibleUniGrid>(ExtenderScopePrefix + "UniGrid");
            ctrlMassActions.InitializeExtenders<IExtensibleMassActions>(ExtenderScopePrefix + "MassActions");
        }
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        gridDocuments.StopProcessing = ctrlMassActions.StopProcessing = StopProcessing;
        if (StopProcessing)
        {
            return;
        }

        InitializeFilterForm();
        InitializeMassActionsControl();

        if (NodeID <= 0)
        {
            return;
        }

        checkPermissions = Tree.CheckDocumentUIPermissions(currentSiteName);

        if (Node != null)
        {
            if (currentUserInfo.IsAuthorizedPerDocument(Node, NodePermissionsEnum.ExploreTree) != AuthorizationResultEnum.Allowed)
            {
                CMSPage.RedirectToAccessDenied("CMS.Content", "exploretree");
            }

            aliasPath = Node.NodeAliasPath;
        }

        ScriptHelper.RegisterLoader(Page);
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);

        // Prepare JavaScript for actions
        StringBuilder actionScript = new StringBuilder();
        actionScript.Append(
            @" function MoveNode(action, nodeId){
    document.getElementById('", hdnMoveId.ClientID, @"').value = action + ';' + nodeId ;
    ", Page.ClientScript.GetPostBackEventReference(this, "move"), @"  
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "actionScript", ScriptHelper.GetScript(actionScript.ToString()));

        // Setup the grid
        gridDocuments.OrderBy = OrderBy;
        gridDocuments.OnExternalDataBound += gridDocuments_OnExternalDataBound;
        gridDocuments.OnDataReload += gridDocuments_OnDataReload;
        gridDocuments.GridView.RowDataBound += GridView_RowDataBound;
        gridDocuments.GridView.RowCreated += GridView_RowCreated;
        gridDocuments.ShowActionsMenu = true;

        // Initialize columns
        string columns = @"DocumentGUID, DocumentName, NodeParentID, NodeLevel, NodeOrder, NodeName, NodeAlias, NodeHasChildren, 
                    ClassDisplayName, DocumentModifiedWhen, DocumentLastVersionNumber, DocumentIsArchived, DocumentCheckedOutByUserID,
                    DocumentPublishedVersionHistoryID, DocumentWorkflowStepID, DocumentCheckedOutVersionHistoryID, DocumentPublishFrom, NodeAliasPath, DocumentIsWaitingForTranslation";

        if (checkPermissions)
        {
            columns = SqlHelper.MergeColumns(columns, DocumentColumnLists.SECURITYCHECK_REQUIRED_COLUMNS);
        }

        columns = SqlHelper.MergeColumns(columns, DocumentColumnLists.GETPUBLISHED_REQUIRED_COLUMNS);

        gridDocuments.Columns = SqlHelper.MergeColumns(columns, AdditionalColumns);

        // Store the refresh node id. It will be used for refreshing the dialog after dialog actions are performed (move, delete...)
        StringBuilder refreshScripts = new StringBuilder();
        refreshScripts.Append(@"
function RefreshTree()
{
    if((parent != null) && (parent.RefreshTree != null))
    {
        ", (!RequiresDialog)
            ? ("parent.RefreshTree(" + NodeID + @"," + NodeID + ");")
            : ControlsHelper.GetPostBackEventReference(this, "refresh", false, false), @"
    }
}

function ClearSelection()
{ 
", gridDocuments.GetClearSelectionScript(), @"
}
function RefreshGrid()
{
    ClearSelection();
    RefreshTree();
", gridDocuments.GetReloadScript(), @"
}");
        // Register refresh scripts
        string refreshScript = ScriptHelper.GetScript(refreshScripts.ToString());
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "refreshListing", refreshScript);

        // Get all possible columns to retrieve
        gridDocuments.AllColumns = SqlHelper.JoinColumnList(ObjectTypeManager.GetColumnNames(PredefinedObjectType.NODE, PredefinedObjectType.DOCUMENTLOCALIZATION));
    }


    /// <summary>
    /// Initializes filter form.
    /// </summary>
    private void InitializeFilterForm()
    {
        // If ClassID is not used don't initialize
        if (ClassID != UniSelector.US_NONE_RECORD)
        {
            if (ClassID > 0)
            {
                var className = DataClassInfoProvider.GetClassName(ClassID);
                var formName = className + ".filter";
                var form = AlternativeFormInfoProvider.GetAlternativeFormInfo(formName);
                formName += (form != null) ? "+cms.document.simplefilter" : "+cms.document.filter";

                gridDocuments.FilterFormName = formName;
            }
            else
            {
                gridDocuments.FilterFormName = "cms.document.filter";
            }
        }
    }


    /// <summary>
    /// Handles the RowCreated event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
    protected void GridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        // Reset the indicator
        isRootDocument = false;
        isCurrentDocument = false;
    }


    /// <summary>
    /// Handles the RowDataBound event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (isRootDocument)
            {
                // Colorize the root row
                e.Row.CssClass += " OnSiteGridRoot";

                // Hide the action checkbox
                if (e.Row.Cells.Count > 0)
                {
                    e.Row.Cells[0].Controls.Clear();
                }
            }

            if (isCurrentDocument)
            {
                // Colorize the current document row
                e.Row.CssClass += " OnSiteGridCurrentDocument";
            }
        }
    }


    /// <summary>
    /// OnPreRender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        gridDocuments.StopProcessing = StopProcessing;
        if (StopProcessing)
        {
            return;
        }

        if (!dataLoaded)
        {
            gridDocuments.ReloadData();
        }

        string order = null;
        CurrentWhereCondition = GetCompleteWhereCondition(gridDocuments.WhereClause, ref order).ToString(true);

        // Hide column with languages if only one culture is assigned to the site
        if (DataHelper.DataSourceIsEmpty(SiteCultures) || (SiteCultures.Tables[0].Rows.Count <= 1))
        {
            // Hide column with flags
            if (gridDocuments.NamedColumns.ContainsKey("documentculture"))
            {
                gridDocuments.NamedColumns["documentculture"].Visible = false;
            }

            // Hide language filter 
            gridDocuments.FilterForm.FieldsToHide.Add("DocumentCulture");
        }
        else
        {
            if (FlagsControls.Count != 0)
            {
                // Get all document node IDs
                HashSet<int> nodeIds = new HashSet<int>();
                foreach (DocumentFlagsControl ucDocFlags in FlagsControls)
                {
                    nodeIds.Add(ucDocFlags.NodeID);
                }

                var condition = new WhereCondition();
                condition.WhereIn("NodeID", nodeIds);

                // Get all culture documents
                DataSet docs = Tree.SelectNodes(currentSiteName, "/%", TreeProvider.ALL_CULTURES, false, null, condition.ToString(true), null, -1, false, 0, "NodeID, DocumentLastVersionNumber, DocumentCulture, DocumentModifiedWhen, DocumentLastPublished");

                if (!DataHelper.DataSourceIsEmpty(docs))
                {
                    var groupedDocs = new GroupedDataSource(docs, "NodeID");

                    // Initialize the document flags controls
                    foreach (var docFlagCtrl in FlagsControls)
                    {
                        docFlagCtrl.DataSource = groupedDocs;
                        docFlagCtrl.ReloadData();
                    }
                }
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    protected DataSet gridDocuments_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        dataLoaded = true;

        if (Node == null)
        {
            return null;
        }

        if (gridDocuments.FilterForm.FieldControls != null)
        {
            // Get value of filter checkbox
            FormEngineUserControl checkboxControl = gridDocuments.FilterForm.FieldControls["ShowAllLevels"];
            if (checkboxControl != null)
            {
                ShowAllLevels = ValidationHelper.GetBoolean(checkboxControl.Value, false);
            }
        }

        // Get the site
        SiteInfo si = SiteInfo.Provider.Get(Node.NodeSiteID);
        if (si == null)
        {
            return null;
        }

        if (ShowAllLevels && !checkPermissions)
        {
            // Add required columns if check permissions is false because of all levels enabled (Browse tree security check is performed)
            columns = SqlHelper.MergeColumns(DocumentColumnLists.SECURITYCHECK_REQUIRED_COLUMNS, columns);
        }

        if (!checkPermissions || (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed))
        {
            var completeCondition = GetCompleteWhereCondition(completeWhere, ref currentOrder);

            // Merge document and grid specific columns
            columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, columns);

            // Add additional order by column to ensure correct deterministic ordering in all cases (eg. order by NodeHasChilder)
            currentOrder = String.Join(",", currentOrder, "NodeID ASC");

            var query = DocumentHelper.GetDocuments()
                                      .TopN(gridDocuments.TopN)
                                      .Columns(columns)
                                      .OnSite(currentSiteName)
                                      .Published(false)
                                      .CombineWithDefaultCulture()
                                      .Culture(GetSiteCultures())
                                      .Where(completeCondition)
                                      .OrderBy(currentOrder);

            // Do not apply published from / to columns to make sure the published information is correctly evaluated
            query.Properties.ExcludedVersionedColumns = new[] { "DocumentPublishFrom", "DocumentPublishTo" };

            query.Offset = currentOffset;
            query.MaxRecords = currentPageSize;

            if (ClassID > 0)
            {
                query.ClassName = DataClassInfoProvider.GetClassName(ClassID);
            }

            DataSet ds = query.Result;
            totalRecords = query.TotalRecords;

            ds = FilterPagesByPermissions(ds, checkPermissions, ShowAllLevels);
            return ds;
        }

        gridDocuments.ZeroRowsText = GetString("ContentTree.ReadDocumentDenied");

        // Hide mass actions when no data
        ctrlMassActions.Visible = false;
        return null;
    }


    private WhereCondition GetCompleteWhereCondition(string completeWhere, ref string currentOrder)
    {
        return new WhereCondition(GetLevelWhereCondition(ShowAllLevels, ref currentOrder))
            .And(WhereCondition)
            .And(new WhereCondition(completeWhere) { WhereIsComplex = true });
    }


    private DataSet FilterPagesByPermissions(DataSet ds, bool checkPermission, bool showAllLevels)
    {

        if (!checkPermission && !showAllLevels)
        {
            return ds;
        }

        NodePermissionsEnum[] permissions;

        // Get permissions to check
        if (checkPermission && showAllLevels)
        {
            permissions = new[]
            {
                NodePermissionsEnum.Read,
                NodePermissionsEnum.ExploreTree
            };
        }
        else if (checkPermission)
        {
            permissions = new[]
            {
                NodePermissionsEnum.Read
            };
        }
        else
        {
            // Check only for 'Show all levels'
            permissions = new[]
            {
                NodePermissionsEnum.ExploreTree
            };
        }

        return TreeSecurityProvider.FilterDataSetByPermissions(ds, permissions, currentUserInfo, false, true);
    }


    /// <summary>
    /// Creates level where condition.
    /// </summary>
    /// <param name="showAllLevels">Indicates if pages from all levels should be retrieved</param>
    /// <param name="orderBy">Current order by</param>
    private WhereCondition GetLevelWhereCondition(bool showAllLevels, ref string orderBy)
    {
        var levelCondition = new WhereCondition();
        if (showAllLevels)
        {
            string path = aliasPath ?? string.Empty;
            levelCondition.WhereStartsWith("NodeAliasPath", path.TrimEnd('/') + "/")
                          .WhereGreaterThan("NodeLevel", 0);
        }
        else
        {
            levelCondition.WhereEquals("NodeParentID", Node.NodeID)
                          .WhereEquals("NodeLevel", Node.NodeLevel + 1);

            // Extend the where condition to include the root document
            if (RequiresDialog && (Node != null) && (Node.NodeParentID == 0))
            {
                levelCondition.Or().WhereNull("NodeParentID");

                orderBy = String.Join(",", "NodeParentID ASC", orderBy);
            }
        }

        if (ClassID > 0)
        {
            levelCondition.WhereEquals("NodeClassID", ClassID);
        }

        return levelCondition;
    }


    /// <summary>
    /// External data binding handler.
    /// </summary>
    protected object gridDocuments_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        int currentNodeId;

        sourceName = sourceName.ToLowerCSafe();
        switch (sourceName)
        {
            case "view":
                {
                    // Dialog view item
                    DataRowView data = ((DataRowView)((GridViewRow)parameter).DataItem);
                    CMSGridActionButton btn = ((CMSGridActionButton)sender);
                    // Current row is the Root document
                    isRootDocument = (ValidationHelper.GetInteger(data["NodeParentID"], 0) == 0);
                    currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);
                    isCurrentDocument = (currentNodeId == WOpenerNodeID);

                    string culture = ValidationHelper.GetString(data["DocumentCulture"], string.Empty);
                    // Existing document culture
                    if (culture.ToLowerCSafe() == CultureCode.ToLowerCSafe())
                    {
                        string className = ValidationHelper.GetString(data["ClassName"], string.Empty);
                        if (DataClassInfoProvider.GetDataClassInfo(className).ClassHasURL)
                        {
                            var relativeUrl = isRootDocument ? "~/" : DocumentUIHelper.GetPageHandlerPreviewPath(currentNodeId, culture, CurrentUser.UserName);
                            string url = ResolveUrl(relativeUrl);

                            btn.OnClientClick = "ViewItem(" + ScriptHelper.GetString(url) + "); return false;";
                        }
                        else
                        {
                            btn.Enabled = false;
                            btn.Style.Add(HtmlTextWriterStyle.Cursor, "default");
                        }
                    }
                    // New culture version
                    else
                    {
                        btn.OnClientClick = "wopener.NewDocumentCulture(" + currentNodeId + ", '" + CultureCode + "'); CloseDialog(); return false;";
                    }
                }
                break;

            case "edit":
                {
                    CMSGridActionButton btn = ((CMSGridActionButton)sender);
                    if (IsEditVisible)
                    {
                        DataRowView data = ((DataRowView)((GridViewRow)parameter).DataItem);
                        string culture = ValidationHelper.GetString(data["DocumentCulture"], string.Empty);
                        currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);
                        int nodeParentId = ValidationHelper.GetInteger(data["NodeParentID"], 0);

                        if (!RequiresDialog || (culture.ToLowerCSafe() == CultureCode.ToLowerCSafe()))
                        {
                            // Go to the selected document or create a new culture version when not used in a dialog
                            btn.OnClientClick = "EditItem(" + currentNodeId + ", " + nodeParentId + "); return false;";
                        }
                        else
                        {
                            // New culture version in a dialog
                            btn.OnClientClick = "wopener.NewDocumentCulture(" + currentNodeId + ", '" + CultureCode + "'); CloseDialog(); return false;";
                        }
                    }
                    else
                    {
                        btn.Visible = false;
                    }
                }
                break;

            case "delete":
                {
                    // Delete button
                    CMSGridActionButton btn = ((CMSGridActionButton)sender);

                    // Hide the delete button for the root document
                    btn.Visible = !isRootDocument;
                }
                break;

            case "contextmenu":
                {
                    // Dialog context menu item
                    CMSGridActionButton btn = ((CMSGridActionButton)sender);

                    // Hide the context menu for the root document
                    btn.Visible = !isRootDocument && !ShowAllLevels;
                }
                break;

            case "versionnumber":
                {
                    // Version number
                    if (parameter == DBNull.Value)
                    {
                        parameter = "-";
                    }
                    parameter = HTMLHelper.HTMLEncode(parameter.ToString());

                    return parameter;
                }

            case "documentname":
                {
                    // Document name
                    DataRowView data = (DataRowView)parameter;
                    string className = ValidationHelper.GetString(data["ClassName"], string.Empty);
                    string classDisplayName = ValidationHelper.GetString(data["classdisplayname"], null);
                    string name = ValidationHelper.GetString(data["DocumentName"], string.Empty);
                    string culture = ValidationHelper.GetString(data["DocumentCulture"], string.Empty);
                    string cultureString = null;

                    currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);
                    int nodeParentId = ValidationHelper.GetInteger(data["NodeParentID"], 0);

                    if (isRootDocument)
                    {
                        // User site name for the root document
                        name = SiteContext.CurrentSiteName;
                    }

                    // Default culture
                    if (culture.ToLowerCSafe() != CultureCode.ToLowerCSafe())
                    {
                        cultureString = " (" + culture + ")";
                    }

                    StringBuilder sb = new StringBuilder();

                    if (ShowDocumentTypeIcon)
                    {
                        // Prepare tooltip for document type icon
                        string iconTooltip = "";
                        if (ShowDocumentTypeIconTooltip && (classDisplayName != null))
                        {
                            string safeClassName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(classDisplayName));
                            iconTooltip = string.Format("onmouseout=\"UnTip()\" onmouseover=\"Tip('{0}')\"", HTMLHelper.EncodeForHtmlAttribute(safeClassName));
                        }

                        var dataClass = DataClassInfoProvider.GetDataClassInfo(className);
                        if (dataClass != null)
                        {
                            var iconClass = (string)dataClass.GetValue("ClassIconClass");
                            sb.Append(UIHelper.GetDocumentTypeIcon(Page, className, iconClass, additionalAttributes: iconTooltip));
                        }
                    }

                    string safeName = HTMLHelper.HTMLEncode(TextHelper.LimitLength(name, 50));
                    if (DocumentNameAsLink && !isRootDocument)
                    {
                        string tooltip = UniGridFunctions.DocumentNameTooltip(data);

                        string selectFunction = SelectItemJSFunction + "(" + currentNodeId + ", " + nodeParentId + ");";
                        sb.Append("<a href=\"javascript: ", selectFunction, "\"");
                        sb.Append(" onmouseout=\"UnTip()\" onmouseover=\"Tip('", HTMLHelper.EncodeForHtmlAttribute(tooltip), "')\">", safeName, cultureString, "</a>");
                    }
                    else
                    {
                        sb.Append(safeName, cultureString);
                    }

                    // Show document marks only if method is not called from grid export and document marks are allowed
                    if ((sender != null) && ShowDocumentMarks)
                    {
                        // Prepare parameters
                        int workflowStepId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(data, "DocumentWorkflowStepID"), 0);
                        WorkflowStepTypeEnum stepType = WorkflowStepTypeEnum.Undefined;

                        if (workflowStepId > 0)
                        {
                            WorkflowStepInfo stepInfo = WorkflowStepInfo.Provider.Get(workflowStepId);
                            if (stepInfo != null)
                            {
                                stepType = stepInfo.StepType;
                            }
                        }

                        // Create data container
                        IDataContainer container = new DataRowContainer(data);

                        // Add icons and use current culture of processed node because of 'Not translated document' icon
                        sb.Append(" ", DocumentUIHelper.GetDocumentMarks(Page, currentSiteName, ValidationHelper.GetString(container.GetValue("DocumentCulture"), string.Empty), stepType, container));
                    }

                    return sb.ToString();
                }

            case "documentculture":
                {
                    DocumentFlagsControl ucDocFlags = null;

                    if (OnDocumentFlagsCreating != null)
                    {
                        // Raise event for obtaining custom DocumentFlagControl
                        object result = OnDocumentFlagsCreating(this, sourceName, parameter);
                        ucDocFlags = result as DocumentFlagsControl;

                        // Check if something other than DocumentFlagControl was returned
                        if ((ucDocFlags == null) && (result != null))
                        {
                            return result;
                        }
                    }

                    // Dynamically load document flags control when not created
                    if (ucDocFlags == null)
                    {
                        ucDocFlags = LoadUserControl("~/CMSAdminControls/UI/DocumentFlags.ascx") as DocumentFlagsControl;
                    }

                    // Set document flags properties
                    if (ucDocFlags != null)
                    {
                        DataRowView data = (DataRowView)parameter;

                        // Get node ID
                        currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);

                        if (!string.IsNullOrEmpty(SelectLanguageJSFunction))
                        {
                            ucDocFlags.SelectJSFunction = SelectLanguageJSFunction;
                        }

                        ucDocFlags.ID = "docFlags" + currentNodeId;
                        ucDocFlags.SiteCultures = SiteCultures;
                        ucDocFlags.NodeID = currentNodeId;
                        ucDocFlags.StopProcessing = true;

                        // Keep the control for later usage
                        FlagsControls.Add(ucDocFlags);
                        return ucDocFlags;
                    }
                }
                break;

            case "modifiedwhen":
            case "modifiedwhentooltip":
                // Modified when
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    return string.Empty;
                }

                DateTime modifiedWhen = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                currentUserInfo = currentUserInfo ?? MembershipContext.AuthenticatedUser;
                currentSiteInfo = currentSiteInfo ?? SiteContext.CurrentSite;

                return sourceName.EqualsCSafe("modifiedwhen", StringComparison.InvariantCultureIgnoreCase)
                    ? TimeZoneHelper.ConvertToUserTimeZone(modifiedWhen, true, currentUserInfo, currentSiteInfo)
                    : TimeZoneHelper.GetUTCLongStringOffset(currentUserInfo, currentSiteInfo);

            default:
                if (OnExternalAdditionalDataBound != null)
                {
                    return OnExternalAdditionalDataBound(sender, sourceName, parameter);
                }

                break;
        }

        return parameter;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets sorted site cultures where preferred culture has top priority and default culture second.
    /// </summary>
    private string[] GetSiteCultures()
    {
        var sortedCultures = new List<string>
        {
            LocalizationContext.PreferredCultureCode,
            CultureHelper.GetDefaultCultureCode(currentSiteName)
        };
        sortedCultures.AddRange(CultureSiteInfoProvider.GetSiteCultureCodes(currentSiteName));

        return sortedCultures.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }


    /// <summary>
    /// Creates data structure containing properties which are needed to perform mass action. This mainly includes 
    /// filter values (so correct where condition can be produced), URLs of the opened dialogs, etc.
    /// </summary>
    /// <returns>Mass actions parameters</returns>
    private DocumentListMassActionsParameters GetMassActionsParameters()
    {
        return new DocumentListMassActionsParameters
        {
            ShowAllLevels = ShowAllLevels,
            ClassID = ClassID,
            CurrentWhereCondition = CurrentWhereCondition,
            Identifier = Identifier,
            Node = Node,
            RequiresDialog = RequiresDialog,
            WOpenerNodeID = WOpenerNodeID,
            ArchiveReturnUrl = ArchiveReturnUrl,
            DeleteReturnUrl = DeleteReturnUrl,
            GetCopyMoveLinkBaseActionUrl = actionCode =>
            {
                Config.CustomFormatCode = actionCode.ToLowerCSafe();
                if (actionCode.Equals("Link", StringComparison.OrdinalIgnoreCase))
                {
                    Config.ContentSites = AvailableSitesEnum.OnlyCurrentSite;
                }

                if (!string.IsNullOrEmpty(CopyMoveLinkStartingPath))
                {
                    Config.ContentStartingPath = CopyMoveLinkStartingPath;
                }

                return CMSDialogHelper.GetDialogUrl(Config, false, null, false);
            },
            TranslateReturnUrl = TranslateReturnUrl,
            PublishReturnUrl = PublishReturnUrl,
        };
    }


    /// <summary>
    /// Sets properties of the mass actions control. Has to be called on Page_Load, because UniGrid's method GetSelectionFieldClientID() doesn't work before Page_Load.
    /// </summary>
    private void InitializeMassActionsControl()
    {
        ctrlMassActions.SelectedItemsClientID = gridDocuments.GetSelectionFieldClientID();
        ctrlMassActions.AdditionalParameters = new Lazy<object>(GetMassActionsParameters);
        ctrlMassActions.SelectedItemsResourceString = "contentlisting.SelectedDocuments";
        ctrlMassActions.AllItemsResourceString = "contentlisting.AllDocuments";
    }


    /// <summary>
    /// Creates default mass actions. Has to be called on Page_Init before MassActions extenders are initialized, so default actions will be at the beginning.
    /// </summary>
    private void CreateMassActionItems()
    {
        var urlBuilder = new DocumentListMassActionsUrlGenerator();

        // Converts functions with signature as in DocumentListMassActionsUrlGenerator to CreateUrlDelegate as MassActionItem expects
        Func<Func<List<int>, DocumentListMassActionsParameters, string>, CreateUrlDelegate> functionConverter = generateActionFunction =>
        {
            return (scope, selectedNodeIDs, parameters) =>
            {
                return generateActionFunction(scope == MassActionScopeEnum.AllItems ? null : selectedNodeIDs, (DocumentListMassActionsParameters)parameters);
            };
        };

        ctrlMassActions.AddMassActions(
            new MassActionItem
            {
                CodeName = "action|move",
                DisplayNameResourceString = "general.move",
                CreateUrl = functionConverter(urlBuilder.GetMoveActionUrl),
                ActionType = MassActionTypeEnum.OpenModal,
            },
            new MassActionItem
            {
                CodeName = "action|copy",
                DisplayNameResourceString = "general.copy",
                CreateUrl = functionConverter(urlBuilder.GetCopyActionUrl),
                ActionType = MassActionTypeEnum.OpenModal,
            });

        ctrlMassActions.AddMassActions(new MassActionItem
        {
            CodeName = "action|link",
            DisplayNameResourceString = "general.link",
            CreateUrl = functionConverter(urlBuilder.GetLinkActionUrl),
            ActionType = MassActionTypeEnum.OpenModal,
        });

        ctrlMassActions.AddMassActions(new MassActionItem
        {
            CodeName = "action|delete",
            DisplayNameResourceString = "general.delete",
            CreateUrl = functionConverter(urlBuilder.GetDeleteActionUrl),
            ActionType = MassActionTypeEnum.Redirect,
        });

        if (CultureSiteInfoProvider.IsSiteMultilingual(currentSiteName) &&
            LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.TranslationServices) &&
            TranslationServiceHelper.IsTranslationAllowed(currentSiteName) &&
            TranslationServiceHelper.AnyServiceAvailable(currentSiteName))
        {
            ctrlMassActions.AddMassActions(new MassActionItem
            {
                CodeName = "action|translate",
                DisplayNameResourceString = "general.translate",
                CreateUrl = functionConverter(urlBuilder.GetTranslateActionUrl),
                ActionType = MassActionTypeEnum.Redirect,
            });
        }

        if (currentUserInfo.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || currentUserInfo.IsAuthorizedPerResource("CMS.Content", "ManageWorkflow"))
        {
            ctrlMassActions.AddMassActions(
                new MassActionItem
                {
                    CodeName = "action|publish",
                    DisplayNameResourceString = "general.publish",
                    CreateUrl = functionConverter(urlBuilder.GetPublishActionUrl),
                    ActionType = MassActionTypeEnum.Redirect,
                },
                new MassActionItem
                {
                    CodeName = "action|archive",
                    DisplayNameResourceString = "general.archive",
                    CreateUrl = functionConverter(urlBuilder.GetArchiveActionUrl),
                    ActionType = MassActionTypeEnum.Redirect,
                });
        }
    }

    #endregion


    #region "IPostBackEventHandler members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "move")
        {
            // Keep current user object
            var cu = MembershipContext.AuthenticatedUser;

            // Parse input value
            string[] values = hdnMoveId.Value.Split(';');

            // Create tree provider
            TreeProvider tree = new TreeProvider(cu);

            // Get tree node object
            int nodeId = ValidationHelper.GetInteger(values[1], 0);
            TreeNode node = tree.SelectSingleNode(nodeId);

            // Check whether node exists
            if (node == null)
            {
                ShowError(GetString("ContentRequest.ErrorMissingSource"));
                return;
            }

            try
            {
                // Check permissions
                if (cu.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed)
                {
                    // Switch by action
                    switch (values[0])
                    {
                        case "up":
                            tree.MoveNodeUp(node);
                            break;

                        case "down":
                            tree.MoveNodeDown(node);
                            break;

                        case "top":
                            tree.SetNodeOrder(node, DocumentOrderEnum.First);
                            break;

                        case "bottom":
                            tree.SetNodeOrder(node, DocumentOrderEnum.Last);
                            break;
                    }

                    if (!RequiresDialog)
                    {
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshAfterMove", ScriptHelper.GetScript("parent.RefreshTree(" + node.NodeParentID + ", " + node.NodeParentID + ");"));
                    }

                    // Log the synchronization tasks for the entire tree level
                    DocumentSynchronizationHelper.LogDocumentChangeOrder(node.NodeSiteName, node.NodeAliasPath, tree);
                }
                else
                {
                    ShowError(GetString("ContentRequest.MoveDenied"));
                }
            }
            catch (Exception ex)
            {
                var logData = new EventLogData(EventTypeEnum.Error, "Content", "MOVE")
                {
                    EventDescription = EventLogProvider.GetExceptionLogMessage(ex),
                    EventUrl = RequestContext.RawURL,
                    UserID = cu.UserID,
                    UserName = cu.UserName,
                    NodeID = nodeId,
                    DocumentName = node.DocumentName,
                    IPAddress = RequestContext.UserHostAddress,
                    SiteID = SiteContext.CurrentSite.SiteID
                };

                Service.Resolve<IEventLogService>().LogEvent(logData);

                ShowError(GetString("ContentRequest.MoveFailed") + " : " + ex.Message);
            }
        }
        else if (eventArgument == "refresh")
        {
            // Register the refresh script after the 'move' action is performed
            Hashtable parameters = WindowHelper.GetItem(Identifier) as Hashtable;
            if ((parameters == null) || (parameters.Count <= 0))
            {
                return;
            }

            int refreshNodeId = ValidationHelper.GetInteger(parameters["refreshnodeid"], 0);
            string refreshScript = "parent.RefreshTree(" + refreshNodeId + ", " + refreshNodeId + ")";
            ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshAfterMove", refreshScript, true);
        }
    }

    #endregion
}