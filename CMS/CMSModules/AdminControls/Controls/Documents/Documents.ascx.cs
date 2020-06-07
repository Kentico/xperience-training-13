using System;
using System.Data;

using CMS.Base;

using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;
using CMS.WorkflowEngine;

using Action = CMS.UIControls.UniGridConfig.Action;
using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_AdminControls_Controls_Documents_Documents : CMSAdminEditControl
{
    #region "Constants"

    // Source fields
    private const string SOURCE_MODIFIEDWHEN = "DocumentModifiedWhen";

    private const string SOURCE_CLASSDISPLAYNAME = "ClassDisplayName";

    private const string SOURCE_WORKFLOWSTEPID = "DocumentWorkflowStepID";

    private const string SOURCE_VERSION = "DocumentLastVersionNumber";

    private const string SOURCE_DOCUMENTNAME = "DocumentName";

    private const string SOURCE_NODEID = "NodeID";

    private const string SOURCE_CLASSNAME = "ClassName";

    private const string SOURCE_CLASSICONCLASS = "ClassIconClass";

    private const string SOURCE_NODESITEID = "NodeSiteID";

    private const string SOURCE_NODELINKEDNODEID = "NodeLinkedNodeID";

    private const string SOURCE_DOCUMENTCULTURE = "DocumentCulture";

    private const string SOURCE_NODEALIASPATH = "NodeAliasPath";

    private const string SOURCE_TYPE = "Type";

    // External source fields
    private const string EXTERNALSOURCE_STEPDISPLAYNAME = "stepdisplayname";

    private const string EXTERNALSOURCE_MODIFIEDWHEN = "modifiedwhen";

    private const string EXTERNALSOURCE_MODIFIEDWHENTOOLTIP = "modifiedwhentooltip";

    private const string EXTERNALSOURCE_VERSION = "versionnumber";
    
    private const string EXTERNALSOURCE_DOCUMENTNAME = "documentname";

    private const string EXTERNALSOURCE_DOCUMENTNAMETOOLTIP = "documentnametooltip";

    private const string EXTERNALSOURCE_CLASSDISPLAYNAME = "classdisplayname";

    private const string EXTERNALSOURCE_CLASSDISPLAYNAMETOOLTIP = "classdisplaynametooltip";

    private const string EXTERNALSOURCE_PREVIEW = "preview";

    private const string EXTERNALSOURCE_EDIT = "edit";

    private const string SELECTION_COLUMN = "DocumentID";

    // Listing type
    private const string LISTINGTYPE_RECYCLEBIN = "Recycle bin";

    // Versioning
    private const string VERSION_COLUMN = "DocumentCheckedOutVersionHistoryID";

    #endregion


    #region "Private variables"

    private CurrentUserInfo currentUserInfo;
    private SiteInfo currentSiteInfo;
    private TreeProvider mTree;
    private SiteInfo selectedSiteInfo;

    // Property variables
    private string mOrderBy = "DocumentModifiedWhen";
    private string mPath = TreeProvider.ALL_DOCUMENTS;
    private string mSiteName = String.Empty;
    private string mDocumentType = String.Empty;
    private string mItemsPerPage = String.Empty;
    private string mDocumentAge = String.Empty;
    private string mDocumentName = String.Empty;
    private string mGMTTooltip;

    private ListingTypeEnum mListingType = ListingTypeEnum.MyDocuments;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Site name for filter.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mSiteName;
        }
        set
        {
            mSiteName = value;
        }
    }


    /// <summary>
    /// Order by for grid.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return mOrderBy;
        }

        set
        {
            UniGrid.OrderBy = value;
            mOrderBy = value;
        }
    }


    /// <summary>
    /// Gets the number of document age conditions.
    /// </summary>
    protected int AgeModifiersCount
    {
        get
        {
            int count = 0;
            if (!String.IsNullOrEmpty(DocumentAge))
            {
                string[] ages = DocumentAge.Split(';');
                if (ages.Length == 2)
                {
                    if (ValidationHelper.GetInteger(ages[1], 0) > 0)
                    {
                        count++;
                    }

                    if (ValidationHelper.GetInteger(ages[0], 0) > 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }


    /// <summary>
    /// Items per page.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return mItemsPerPage;
        }
        set
        {
            mItemsPerPage = value;
        }
    }


    /// <summary>
    /// If true, only documents from running sites are displayed
    /// </summary>
    public bool DisplayOnlyRunningSites
    {
        get;
        set;
    }


    /// <summary>
    /// Path filter for grid.
    /// </summary>
    public string Path
    {
        get
        {
            if (String.IsNullOrEmpty(mPath))
            {
                return TreeProvider.ALL_DOCUMENTS;
            }
            return mPath;
        }
        set
        {
            mPath = value;
        }
    }


    /// <summary>
    /// Age of documents in days.
    /// </summary>
    public string DocumentAge
    {
        get
        {
            return mDocumentAge;
        }
        set
        {
            mDocumentAge = value;
        }
    }


    /// <summary>
    /// Document name for grid filter.
    /// </summary>
    public string DocumentName
    {
        get
        {
            return mDocumentName;
        }
        set
        {
            mDocumentName = value;
        }
    }


    /// <summary>
    /// Document type for filter.
    /// </summary>
    public string DocumentType
    {
        get
        {
            return mDocumentType;
        }
        set
        {
            mDocumentType = value;
        }
    }


    /// <summary>
    /// Type of items to show.
    /// </summary>
    public ListingTypeEnum ListingType
    {
        get
        {
            return mListingType;
        }

        set
        {
            mListingType = value;
        }
    }


    /// <summary>
    /// Is one of 'my desk' document listings.
    /// </summary>
    public bool MyDeskDocuments
    {
        get
        {
            return (ListingType == ListingTypeEnum.CheckedOut) || (ListingType == ListingTypeEnum.RecentDocuments) || (ListingType == ListingTypeEnum.MyDocuments) || (ListingType == ListingTypeEnum.PendingDocuments) || (ListingType == ListingTypeEnum.OutdatedDocuments) || (ListingType == ListingTypeEnum.All);
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
            gridElem.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Text displayed when data source is empty.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return UniGrid.ZeroRowsText;
        }
        set
        {
            UniGrid.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Inner grid control.
    /// </summary>
    public UniGrid UniGrid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Tree provider.
    /// </summary>
    public TreeProvider Tree
    {
        get
        {
            return mTree ?? (mTree = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
        set
        {
            mTree = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
            return;
        }

        currentUserInfo = MembershipContext.AuthenticatedUser;

        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.HideControlForZeroRows = false;
        gridElem.OnAfterRetrieveData += GridElem_OnAfterRetrieveData;    

        // Main controls
        SetupGridColumns();
        SetupGridActions();
        SetupGridOptions();

        // Additional settings
        SetupGridAdditionalOptions();
        SetupGridQueryParameters();
        SetupGridFilterWhereCondition();
    }

       
    /// <summary>
    /// Page_PreRender event handler.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Initialize JavaScript module
        ScriptHelper.RegisterModule(this, "AdminControls/DocumentsGrid", new
        {
            GridSelector = "#" + gridElem.ClientID,
            PagesApplicationHash = ApplicationUrlHelper.GetApplicationHash("cms.content", "content"),
            OpenInNewWindow = (ListingType == ListingTypeEnum.PageTemplateDocuments) && IsCMSDesk
        });
    }

    #endregion


    #region "Grid events"
    
    /// <summary>
    /// On after retrieve data handler to apply versioned data
    /// </summary>
    /// <param name="data">Retrieved data</param>
    private DataSet GridElem_OnAfterRetrieveData(DataSet data)
    {
        if (IsLiveSite)
        {
            return data;
        }

        var manager = VersionManager.GetInstance(Tree);

        manager.ApplyVersionData(data, false);

        return data;
    }

    
    /// <summary>
    /// External data binding handler.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Prepare variables
        string culture;
        DataRowView data;
        sourceName = sourceName.ToLowerCSafe();
        SiteInfo site;

        switch (sourceName)
        {
            // Edit button
            case EXTERNALSOURCE_EDIT:
                if (sender is CMSGridActionButton)
                {
                    var editButton = (CMSGridActionButton)sender;
                    data = UniGridFunctions.GetDataRowView(editButton.Parent as DataControlFieldCell);
                    site = GetSiteFromRow(data);
                    int nodeId = ValidationHelper.GetInteger(data[SOURCE_NODEID], 0);
                    culture = ValidationHelper.GetString(data[SOURCE_DOCUMENTCULTURE], string.Empty);
                    string type = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(data, SOURCE_TYPE), string.Empty);
                    
                    // Check permissions
                    if ((site.Status != SiteStatusEnum.Running) || (!CMSPage.IsUserAuthorizedPerContent(site.SiteName) || ((ListingType == ListingTypeEnum.All) && (type == LISTINGTYPE_RECYCLEBIN))))
                    {
                        editButton.Enabled = false;
                        editButton.Style.Add(HtmlTextWriterStyle.Cursor, "default");
                    }
                    else
                    {
                        editButton.Attributes.Add("data-site-url", ResolveSiteUrl(site));
                        editButton.Attributes.Add("data-node-id", nodeId.ToString());
                        editButton.Attributes.Add("data-document-culture", culture);
                    }

                    editButton.OnClientClick = "return false";
                    return editButton;
                }
                return sender;

            // Preview button
            case EXTERNALSOURCE_PREVIEW:
                if (sender is CMSGridActionButton)
                {
                    var previewButton = (CMSGridActionButton)sender;
                    data = UniGridFunctions.GetDataRowView(previewButton.Parent as DataControlFieldCell);
                    site = GetSiteFromRow(data);
                    string type = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(data, SOURCE_TYPE), string.Empty);
                    string className = ValidationHelper.GetString(data[SOURCE_CLASSNAME], string.Empty);

                    if ((site.Status != SiteStatusEnum.Running) || 
                        ((ListingType == ListingTypeEnum.All) && (type == LISTINGTYPE_RECYCLEBIN)) ||
                        !DataClassInfoProvider.GetDataClassInfo(className).ClassHasURL)
                    {
                        previewButton.Enabled = false;
                        previewButton.Style.Add(HtmlTextWriterStyle.Cursor, "default");
                    }
                    else
                    {
                        culture = ValidationHelper.GetString(data[SOURCE_DOCUMENTCULTURE], string.Empty);
                        var nodeId = ValidationHelper.GetInteger(data[SOURCE_NODEID], 0);
                        var url = DocumentUIHelper.GetPageHandlerPreviewPath(nodeId, culture, CurrentUser.UserName);
                        
                        previewButton.Attributes.Add("data-preview-url", URLHelper.GetAbsoluteUrl(url, site.DomainName));
                    }

                    previewButton.OnClientClick = "return false";
                    return previewButton;
                }
                return sender;

            // Document name column
            case EXTERNALSOURCE_DOCUMENTNAME:
                {
                    data = (DataRowView)parameter;

                    string name = ValidationHelper.GetString(data[SOURCE_DOCUMENTNAME], string.Empty);
                    string className = ValidationHelper.GetString(data[SOURCE_CLASSNAME], string.Empty);

                    if (name == string.Empty)
                    {
                        name = GetString("general.root");
                    }
                    // Add document type icon
                    string result = string.Empty;
                    switch (ListingType)
                    {
                        case ListingTypeEnum.DocTypeDocuments:
                            break;

                        default:
                            var dataClass = DataClassInfoProvider.GetDataClassInfo(className);

                            if (dataClass != null)
                            {
                                var iconClass = (string)dataClass.GetValue(SOURCE_CLASSICONCLASS);
                                result = UIHelper.GetDocumentTypeIcon(Page, className, iconClass);
                            }
                            break;
                    }

                    result += "<span>" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(name, 50)) + "</span>";

                    // Show document marks only if method is not called from grid export
                    if ((sender != null) && (ListingType != ListingTypeEnum.All))
                    {
                        bool isLink = (data.Row.Table.Columns.Contains(SOURCE_NODELINKEDNODEID) && (data[SOURCE_NODELINKEDNODEID] != DBNull.Value));
                        if (isLink)
                        {
                            // Add link icon
                            result += DocumentUIHelper.GetDocumentMarkImage(Parent.Page, DocumentMarkEnum.Link);
                        }
                    }
                    return result;
                }

            // Class name column
            case EXTERNALSOURCE_CLASSDISPLAYNAME:
                string displayName = ValidationHelper.GetString(parameter, string.Empty);
                if (sourceName.ToLowerCSafe() == EXTERNALSOURCE_CLASSDISPLAYNAMETOOLTIP)
                {
                    displayName = TextHelper.LimitLength(displayName, 50);
                }
                if (displayName == string.Empty)
                {
                    displayName = "-";
                }
                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(displayName));

            case EXTERNALSOURCE_DOCUMENTNAMETOOLTIP:
                data = (DataRowView)parameter;
                return UniGridFunctions.DocumentNameTooltip(data);

            case EXTERNALSOURCE_STEPDISPLAYNAME:
                // Step display name
                int stepId = ValidationHelper.GetInteger(parameter, 0);
                if (stepId > 0)
                {
                    return new ObjectTransformation(WorkflowStepInfo.OBJECT_TYPE, stepId)
                    {
                        Transformation = "{%stepdisplayname|(encode)%}"
                    };
                }

                return "-";
           
            // Version column
            case EXTERNALSOURCE_VERSION:
                if (parameter == DBNull.Value)
                {
                    parameter = "-";
                }
                parameter = HTMLHelper.HTMLEncode(parameter.ToString());
                return parameter;

            // Document timestamp column
            case EXTERNALSOURCE_MODIFIEDWHEN:
            case EXTERNALSOURCE_MODIFIEDWHENTOOLTIP:
                if (String.IsNullOrEmpty(parameter.ToString()))
                {
                    return String.Empty;
                }

                if (currentSiteInfo == null)
                {
                    currentSiteInfo = SiteContext.CurrentSite;
                }

                if (sourceName.EqualsCSafe(EXTERNALSOURCE_MODIFIEDWHEN, StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime time = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                    return TimeZoneHelper.ConvertToUserTimeZone(time, true, currentUserInfo, currentSiteInfo);
                }

                return mGMTTooltip ?? (mGMTTooltip = TimeZoneHelper.GetUTCLongStringOffset(currentUserInfo, currentSiteInfo));
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Setups grid columns by given listing type.
    /// </summary>
    private void SetupGridColumns()
    {
        // Initialize UniGrid collections
        gridElem.GridColumns = new UniGridColumns();

        // Add document name column
        Column documentName = new Column();
        documentName.Source = UniGrid.ALL;
        documentName.Sort = SOURCE_DOCUMENTNAME;
        documentName.ExternalSourceName = EXTERNALSOURCE_DOCUMENTNAME;
        documentName.Caption = "$general.documentname$";
        documentName.Wrap = false;
        documentName.Tooltip = new ColumnTooltip();
        documentName.Tooltip.Source = UniGrid.ALL;
        documentName.Tooltip.ExternalSourceName = EXTERNALSOURCE_DOCUMENTNAMETOOLTIP;
        documentName.Tooltip.Width = "0";
        documentName.Tooltip.Encode = false;
        documentName.CssClass = (ListingType == ListingTypeEnum.All) ? "main-column-80" : "main-column-100";

        if (ListingType != ListingTypeEnum.OutdatedDocuments)
        {
            documentName.Filter = new ColumnFilter();
            documentName.Filter.Type = UniGridFilterTypeEnum.Text;
            documentName.Filter.Source = SOURCE_DOCUMENTNAME;
        }

        gridElem.GridColumns.Columns.Add(documentName);

        if (ListingType == ListingTypeEnum.All)
        {
            // Add listing type (only if combined view)
            Column listingType = new Column();
            listingType.Source = "type";
            listingType.ExternalSourceName = "type";
            listingType.Sort = "type";
            listingType.Wrap = false;
            listingType.Caption = "$general.listingtype$";
            gridElem.GridColumns.Columns.Add(listingType);
        }

        if (ListingType != ListingTypeEnum.DocTypeDocuments)
        {
            // Initialize document class name column
            Column classDisplayName = new Column();
            classDisplayName.Source = SOURCE_CLASSDISPLAYNAME;
            classDisplayName.ExternalSourceName = EXTERNALSOURCE_CLASSDISPLAYNAME;
            classDisplayName.MaxLength = 50;
            classDisplayName.Caption = "$general.type$";
            classDisplayName.Wrap = false;
            classDisplayName.Tooltip = new ColumnTooltip();
            classDisplayName.Tooltip.Source = SOURCE_CLASSDISPLAYNAME;
            classDisplayName.Tooltip.ExternalSourceName = EXTERNALSOURCE_CLASSDISPLAYNAMETOOLTIP;
            classDisplayName.Tooltip.Width = "0";

            if (ListingType != ListingTypeEnum.OutdatedDocuments)
            {
                classDisplayName.Filter = new ColumnFilter();
                classDisplayName.Filter.Type = UniGridFilterTypeEnum.Text;
                classDisplayName.Filter.Source = SOURCE_CLASSDISPLAYNAME;
            }

            gridElem.GridColumns.Columns.Add(classDisplayName);
        }

        // Add timestamp column
        Column modifiedWhen = new Column();
        modifiedWhen.Source = SOURCE_MODIFIEDWHEN;
        modifiedWhen.ExternalSourceName = EXTERNALSOURCE_MODIFIEDWHEN;
        modifiedWhen.Wrap = false;
        modifiedWhen.Tooltip = new ColumnTooltip();
        modifiedWhen.Tooltip.ExternalSourceName = EXTERNALSOURCE_MODIFIEDWHENTOOLTIP;
        modifiedWhen.Caption = (ListingType == ListingTypeEnum.CheckedOut) ? "$general.checkouttime$" : "$general.modified$";
        gridElem.GridColumns.Columns.Add(modifiedWhen);

        if (!IsLiveSite)
        {
            // Add column with workflow information
            Column workflowStep = new Column();
            workflowStep.Caption = "$general.workflowstep$";
            workflowStep.Wrap = false;
            workflowStep.AllowSorting = false;
            workflowStep.Source = SOURCE_WORKFLOWSTEPID;
            workflowStep.ExternalSourceName = EXTERNALSOURCE_STEPDISPLAYNAME;
            gridElem.GridColumns.Columns.Add(workflowStep);
        }

        // Add version information
        if (ListingType == ListingTypeEnum.WorkflowDocuments)
        {
            Column versionNumber = new Column();
            versionNumber.Source = SOURCE_VERSION;
            versionNumber.ExternalSourceName = EXTERNALSOURCE_VERSION;
            versionNumber.Caption = "$general.version$";
            versionNumber.Wrap = false;
            gridElem.GridColumns.Columns.Add(versionNumber);
        }

        // Add culture column
        Column culture = new Column();
        culture.Source = SOURCE_DOCUMENTCULTURE;
        culture.ExternalSourceName = "#culturenamewithflag";
        culture.Caption = "$general.language$";
        culture.Wrap = false;
        culture.AllowSorting = false;
        gridElem.GridColumns.Columns.Add(culture);

        if (String.IsNullOrEmpty(SiteName) || (SiteName == UniGrid.ALL))
        {
            // Add site name column
            Column siteName = new Column();
            siteName.Caption = "$general.site$";
            siteName.Wrap = false;
            siteName.Source = SOURCE_NODESITEID;
            siteName.ExternalSourceName = "#sitename";
            siteName.AllowSorting = false;

            gridElem.GridColumns.Columns.Add(siteName);
        }

        // Prepare columns to select
        string baseColumns = SqlHelper.MergeColumns(new []
        {
            SOURCE_CLASSNAME,
            SOURCE_CLASSDISPLAYNAME,
            SOURCE_MODIFIEDWHEN,
            SOURCE_NODEID,
            SOURCE_NODESITEID,
            SOURCE_DOCUMENTCULTURE,
            SOURCE_DOCUMENTNAME,
            SOURCE_NODEALIASPATH,
            SOURCE_NODELINKEDNODEID,
            SOURCE_DOCUMENTCULTURE
        });
        
        if (!IsLiveSite)
        {
            baseColumns = SqlHelper.MergeColumns(new[]
                {
                    baseColumns,
                    VERSION_COLUMN,
                    SOURCE_WORKFLOWSTEPID
                });
        }

        // Set UniGrid options
        switch (ListingType)
        {
            case ListingTypeEnum.PageTemplateDocuments:
                gridElem.Columns = SqlHelper.MergeColumns(new []
                {
                    baseColumns,
                    "NodeACLID",
                    "NodeOwner"
                });
                gridElem.ObjectType = TreeNode.OBJECT_TYPE;
                break;

            case ListingTypeEnum.WorkflowDocuments:
                gridElem.Columns = SqlHelper.MergeColumns(new []
                {
                    baseColumns,
                    SELECTION_COLUMN,
                    SOURCE_VERSION
                });
                gridElem.ObjectType = TreeNode.OBJECT_TYPE;
                break;

            case ListingTypeEnum.All:
                gridElem.ObjectType = UserDocumentsListInfo.OBJECT_TYPE;
                break;

            default:
                gridElem.Columns = baseColumns;
                gridElem.ObjectType = TreeNode.OBJECT_TYPE;
                break;
        }
    }


    /// <summary>
    /// Setups grid options.
    /// </summary>
    private void SetupGridOptions()
    {
        gridElem.GridOptions = new UniGridOptions();

        // Add filter for specific listing types
        if ((ListingType == ListingTypeEnum.MyDocuments) || (ListingType == ListingTypeEnum.CheckedOut) || (ListingType == ListingTypeEnum.RecentDocuments) ||
            (ListingType == ListingTypeEnum.PendingDocuments) || (ListingType == ListingTypeEnum.OutdatedDocuments) || (ListingType == ListingTypeEnum.All))
        {
            gridElem.GridOptions.DisplayFilter = true;

            if (ListingType == ListingTypeEnum.OutdatedDocuments)
            {
                // Set custom filter path
                gridElem.GridOptions.FilterPath = "~/CMSModules/AdminControls/Controls/Documents/OutdatedDocumentsFilter.ascx";
                gridElem.HideFilterButton = true;
            }
        }

        if (ListingType == ListingTypeEnum.WorkflowDocuments)
        {
            gridElem.GridOptions.ShowSelection = true;
            gridElem.GridOptions.SelectionColumn = SELECTION_COLUMN;
        }
    }


    /// <summary>
    /// Setups grid actions.
    /// </summary>
    private void SetupGridActions()
    {
        if (IsLiveSite)
        {
            // Do not add actions on the live site
            return;
        }

        gridElem.GridActions = new UniGridActions();

        // Add edit action
        Action editAction = new Action();
        editAction.Name = "Edit";
        editAction.ExternalSourceName = EXTERNALSOURCE_EDIT;
        editAction.FontIconClass = "icon-edit";
        editAction.FontIconStyle = GridIconStyle.Allow;
        editAction.Caption = "$contentmenu.edit$";
        gridElem.GridActions.Actions.Add(editAction);

        // Add view action
        Action viewAction = new Action();
        viewAction.Name = "Preview";
        viewAction.ExternalSourceName = EXTERNALSOURCE_PREVIEW;
        viewAction.FontIconClass = "icon-eye";
        viewAction.FontIconStyle = GridIconStyle.Allow;
        viewAction.Caption = "$documents.navigatetodocument$";
        gridElem.GridActions.Actions.Add(viewAction);
    }


    /// <summary>
    /// Setups grid zero rows text, main part of where condition by given listing type, default page size and order by.
    /// </summary>
    private void SetupGridAdditionalOptions()
    {
        // Set proper Zero rows text
        switch (ListingType)
        {
            case ListingTypeEnum.CheckedOut:
                gridElem.ZeroRowsText = GetString("mydesk.ui.nochecked");
                break;

            case ListingTypeEnum.MyDocuments:
                gridElem.ZeroRowsText = GetString("general.nodatafound");
                break;

            case ListingTypeEnum.RecentDocuments:
                gridElem.ZeroRowsText = GetString("general.nodatafound");
                break;

            case ListingTypeEnum.PendingDocuments:
                gridElem.ZeroRowsText = GetString("mydesk.ui.nowaitingdocs");
                break;

            case ListingTypeEnum.OutdatedDocuments:
                gridElem.ZeroRowsText = GetString("mydesk.ui.nooutdated");
                break;

            case ListingTypeEnum.PageTemplateDocuments:
                gridElem.ZeroRowsText = GetString("administration-pagetemplate_header.documents.nodata");
                break;

            case ListingTypeEnum.CategoryDocuments:
                gridElem.ZeroRowsText = GetString("Category_Edit.Documents.nodata");
                break;

            case ListingTypeEnum.TagDocuments:
                gridElem.ZeroRowsText = GetString("taggroup_edit.documents.nodata");
                break;

            case ListingTypeEnum.DocTypeDocuments:
                gridElem.ZeroRowsText = GetString("DocumentType_Edit_General.Documents.nodata");
                break;

            case ListingTypeEnum.All:
                gridElem.ZeroRowsText = GetString("mydesk.ui.nodata");
                break;
        }

        // Page size
        if (!RequestHelper.IsPostBack() && !String.IsNullOrEmpty(ItemsPerPage))
        {
            gridElem.Pager.DefaultPageSize = ValidationHelper.GetInteger(ItemsPerPage, -1);
        }

        // Order by
        switch (ListingType)
        {
            case ListingTypeEnum.WorkflowDocuments:
            case ListingTypeEnum.OutdatedDocuments:
            case ListingTypeEnum.PageTemplateDocuments:
            case ListingTypeEnum.CategoryDocuments:
            case ListingTypeEnum.TagDocuments:
            case ListingTypeEnum.ProductDocuments:
            case ListingTypeEnum.DocTypeDocuments:
                gridElem.OrderBy = SOURCE_DOCUMENTNAME;
                break;

            default:
                gridElem.OrderBy = OrderBy;
                break;
        }
    }


    /// <summary>
    /// Setups grid query parameters.
    /// </summary>
    private void SetupGridQueryParameters()
    {
        // Create query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@Now", DateTime.Now);

        if (ListingType == ListingTypeEnum.OutdatedDocuments)
        {
            parameters.Add("@SiteID", SiteContext.CurrentSiteID);
        }

        // Initialize UserID query parameter
        int userID = currentUserInfo.UserID;
        if (ListingType == ListingTypeEnum.PendingDocuments)
        {
            parameters.Add("@SiteID", SiteInfoProvider.GetSiteID(SiteName));

            if ((currentUserInfo.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)) || (currentUserInfo.IsAuthorizedPerResource("CMS.Content", "manageworkflow")))
            {
                userID = -1;
            }
        }

        parameters.Add("@UserID", userID);

        // Document Age
        if (DocumentAge != String.Empty)
        {
            string[] ages = DocumentAge.Split(';');
            if (ages.Length == 2)
            {
                // Add from a to values to temp parameters
                int from = ValidationHelper.GetInteger(ages[1], 0);
                int to = ValidationHelper.GetInteger(ages[0], 0);

                if (@from > 0)
                {
                    gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, SOURCE_MODIFIEDWHEN + " >= @FROM");
                    parameters.Add("@FROM", DateTime.Now.AddDays((-1) * @from));
                }

                if (to > 0)
                {
                    gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, SOURCE_MODIFIEDWHEN + " <= @TO");
                    parameters.Add("@TO", DateTime.Now.AddDays((-1) * to));
                }
            }
        }

        // Set parameters
        gridElem.QueryParameters = parameters;
    }


    /// <summary>
    /// Setups grid where condition to filter out by given restrictions.
    /// </summary>
    private void SetupGridFilterWhereCondition()
    {
        string where = String.Empty;

        // Set proper base where condition
        switch (ListingType)
        {
            case ListingTypeEnum.CheckedOut:
                where = "DocumentCheckedOutByUserID = @UserID";
                break;

            case ListingTypeEnum.MyDocuments:
                where = "NodeOwner = @UserID";
                break;

            case ListingTypeEnum.RecentDocuments:
                where = "((DocumentCreatedByUserID = @UserID OR DocumentModifiedByUserID = @UserID OR DocumentCheckedOutByUserID = @UserID))";
                break;

            case ListingTypeEnum.PendingDocuments:
                where = "DocumentWorkflowStepID IN (SELECT StepID FROM CMS_WorkflowStep WHERE " + WorkflowStepInfoProvider.GetWorkflowPendingStepsWhereCondition(currentUserInfo, new SiteInfoIdentifier(SiteName)).ToString(false) + ")";
                break;

            case ListingTypeEnum.OutdatedDocuments:
                where = "DocumentCreatedByUserID = @UserID OR DocumentModifiedByUserID = @UserID OR DocumentCheckedOutByUserID = @UserID";
                break;

            case ListingTypeEnum.All:
                where = String.Format("(UserID1 = {0} OR  UserID2 = {0} OR UserID3 = {0})", currentUserInfo.UserID);
                break;
        }

        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, where);

        // Selected site filter
        if (!String.IsNullOrEmpty(SiteName) && (SiteName != UniGrid.ALL))
        {
            selectedSiteInfo = SiteInfo.Provider.Get(SiteName);
            if (selectedSiteInfo != null)
            {
                gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, SOURCE_NODESITEID + " = " + selectedSiteInfo.SiteID);
            }
        }

        // Site running filter
        if ((SiteName == UniGrid.ALL) && DisplayOnlyRunningSites)
        {
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "NodeSiteID IN (SELECT SiteID FROM CMS_Site WHERE SiteStatus = 'RUNNING')");
        }

        // Path filter
        if (Path != String.Empty)
        {
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, SOURCE_NODEALIASPATH + " LIKE N'" + SqlHelper.EscapeQuotes(MacroResolver.ResolveCurrentPath(Path)) + "'");
        }

        // Document type filer
        if (!String.IsNullOrEmpty(DocumentType))
        {
            string classNames = DocumentTypeHelper.GetClassNames(DocumentType);
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, SqlHelper.GetWhereCondition<string>(SOURCE_CLASSNAME, classNames.Split(';'), true));
        }

        // Document name filter
        if (DocumentName != String.Empty)
        {
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, SOURCE_DOCUMENTNAME + " LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(DocumentName)) + "%'");
        }
    }


    /// <summary>
    /// Fills given site object based on data from given row.
    /// </summary>
    /// <param name="data">Row with site reference</param>
    /// <returns>Initialized site info</returns>
    private SiteInfo GetSiteFromRow(DataRowView data)
    {
        return SiteInfo.Provider.Get(ValidationHelper.GetInteger(data[SOURCE_NODESITEID], 0));
    }


    /// <summary>
    /// Make URL for site in form 'http(s)://sitedomain/application/admin'.
    /// </summary>
    /// <param name="site">Site info object</param>
    private string ResolveSiteUrl(SiteInfo site)
    {
        string sitedomain = site.DomainName.TrimEnd('/');

        string application = null;
        // Support of multiple web sites on single domain
        if (!sitedomain.Contains("/"))
        {
            application = ResolveUrl("~/.").TrimEnd('/');
        }

        // Application includes string '/admin'.
        application += "/Admin/CMSAdministration.aspx";

        return RequestContext.CurrentScheme + "://" + sitedomain + application;
    }

    #endregion
}
