using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Globalization;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;

public partial class CMSModules_Content_Controls_UserContributions_ContributionList : CMSUserControl
{
    #region "Variables"

    /// <summary>
    /// On after delete event.
    /// </summary>
    public event EventHandler OnAfterDelete = null;


    private bool mAllowInsert = true;
    private bool mAllowDelete = true;
    private bool mAllowEdit = true;
    private string mNewItemPageTemplate = String.Empty;
    private string mAllowedChildClasses = String.Empty;
    private bool mCheckDocPermissionsForInsert = true;
    private bool mLogActivity;
    private UserContributionAllowUserEnum mAllowUsers = UserContributionAllowUserEnum.DocumentOwner;
    private TreeNode mParentNode;

    /// <summary>
    /// Data properties variable.
    /// </summary>
    protected CMSDataProperties mDataProperties = new CMSDataProperties();

    #endregion


    #region "Document properties"

    /// <summary>
    /// Component name
    /// </summary>
    public override string ComponentName
    {
        get
        {
            return base.ComponentName;
        }
        set
        {
            base.ComponentName = value;
            DocumentManager.ComponentName = value;
            editDoc.ComponentName = value;
        }
    }


    /// <summary>
    /// Document manager
    /// </summary>
    public override ICMSDocumentManager DocumentManager
    {
        get
        {
            return editDoc.DocumentManager;
        }
    }


    /// <summary>
    /// Class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return mDataProperties.ClassNames;
        }
        set
        {
            mDataProperties.ClassNames = value;
        }
    }


    /// <summary>
    /// Combine with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return mDataProperties.CombineWithDefaultCulture;
        }
        set
        {
            mDataProperties.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return mDataProperties.CultureCode;
        }
        set
        {
            mDataProperties.CultureCode = value;
        }
    }


    /// <summary>
    /// Maximal relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return mDataProperties.MaxRelativeLevel;
        }
        set
        {
            mDataProperties.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return mDataProperties.OrderBy;
        }
        set
        {
            mDataProperties.OrderBy = value;
        }
    }


    /// <summary>
    /// Nodes path.
    /// </summary>
    public string Path
    {
        get
        {
            return mDataProperties.Path;
        }
        set
        {
            mDataProperties.Path = value;
        }
    }


    /// <summary>
    /// Select only published nodes.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return mDataProperties.SelectOnlyPublished;
        }
        set
        {
            mDataProperties.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mDataProperties.SiteName;
        }
        set
        {
            mDataProperties.SiteName = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mDataProperties.WhereCondition;
        }
        set
        {
            mDataProperties.WhereCondition = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets Edit form control.
    /// </summary>
    public CMSModules_Content_Controls_UserContributions_EditForm EditForm
    {
        get
        {
            return editDoc;
        }
    }


    /// <summary>
    /// Indicates whether the list of documents should be displayed.
    /// </summary>
    public bool DisplayList
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DisplayList"], true);
        }
        set
        {
            ViewState["DisplayList"] = value;
        }
    }


    /// <summary>
    /// Path for new created documents.
    /// </summary>
    public string NewDocumentPath
    {
        get
        {
            string newDocPath = ValidationHelper.GetString(ViewState["NewDocumentPath"], "");
            // If new document path is not set, use source path
            if (newDocPath.Trim() == String.Empty)
            {
                newDocPath = Path;
            }

            // Ensure correct format of the path
            if (newDocPath.EndsWithCSafe("/%"))
            {
                newDocPath = newDocPath.Remove(newDocPath.Length - 2);
            }

            if (String.IsNullOrEmpty(newDocPath))
            {
                newDocPath = "/";
            }

            return newDocPath;
        }
        set
        {
            ViewState["NewDocumentPath"] = value;
        }
    }


    /// <summary>
    /// Indicates whether inserting new document is allowed.
    /// </summary>
    public bool AllowInsert
    {
        get
        {
            return mAllowInsert;
        }
        set
        {
            mAllowInsert = value;
        }
    }


    /// <summary>
    /// Indicates whether editing document is allowed.
    /// </summary>
    public bool AllowEdit
    {
        get
        {
            return mAllowEdit;
        }
        set
        {
            mAllowEdit = value;
        }
    }


    /// <summary>
    /// Indicates whether deleting document is allowed.
    /// </summary>
    public bool AllowDelete
    {
        get
        {
            return mAllowDelete;
        }
        set
        {
            mAllowDelete = value;
            editDoc.AllowDelete = mAllowDelete;
        }
    }


    /// <summary>
    /// Page template the new items are assigned to.
    /// </summary>
    public string NewItemPageTemplate
    {
        get
        {
            return mNewItemPageTemplate;
        }
        set
        {
            mNewItemPageTemplate = value;
        }
    }


    /// <summary>
    /// Type of the child documents that are allowed to be created.
    /// </summary>
    public string AllowedChildClasses
    {
        get
        {
            return mAllowedChildClasses;
        }
        set
        {
            mAllowedChildClasses = value;
        }
    }


    /// <summary>
    /// Alternative form name.
    /// </summary>
    public string AlternativeFormName
    {
        get;
        set;
    }


    /// <summary>
    /// Form validation error message.
    /// </summary>
    public string ValidationErrorMessage
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether group permissions should be checked.
    /// </summary>
    public bool CheckGroupPermissions
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if document type permissions are required to create new document.
    /// </summary>
    public bool CheckDocPermissionsForInsert
    {
        get
        {
            return mCheckDocPermissionsForInsert;
        }
        set
        {
            mCheckDocPermissionsForInsert = value;
        }
    }


    /// <summary>
    /// Which group of users can work with the documents.
    /// </summary>
    public UserContributionAllowUserEnum AllowUsers
    {
        get
        {
            return mAllowUsers;
        }
        set
        {
            mAllowUsers = value;
        }
    }


    /// <summary>
    /// Gets or sets New item button label.
    /// </summary>
    public string NewItemButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NewItemButtonText"), "ContributionList.lnkNewDoc");
        }
        set
        {
            SetValue("NewItemButtonText", value);
            btnNewDoc.ResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets List button label.
    /// </summary>
    public string ListButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ListButtonText"), "general.pages");
        }
        set
        {
            SetValue("ListButtonText", value);
            btnList.ResourceString = value;
        }
    }


    /// <summary>
    /// Indicates whether activity logging is enabled.
    /// </summary>
    public bool LogActivity
    {
        get
        {
            return mLogActivity;
        }
        set
        {
            mLogActivity = value;
            editDoc.LogActivity = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;

            editDoc.StopProcessing = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Parent node.
    /// </summary>
    private TreeNode ParentNode
    {
        get
        {
            return mParentNode ?? (mParentNode = GetParentNode());
        }
    }


    /// <summary>
    /// Parent node id.
    /// </summary>
    private int ParentNodeID
    {
        get
        {
            return (ParentNode == null ? 0 : ParentNode.NodeID);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check license
        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.UserContributions);

        mDataProperties.ParentControl = this;

        // Reload data
        ReloadData();

        // Control initialization
        gridDocs.OnExternalDataBound += gridDocs_OnExternalDataBound;
        gridDocs.OnAction += gridDocs_OnAction;
        gridDocs.GridView.CssClass = "ContributionsGrid";
        gridDocs.IsLiveSite = IsLiveSite;

        btnNewDoc.ResourceString = HTMLHelper.HTMLEncode(NewItemButtonText);
        btnList.ResourceString = HTMLHelper.HTMLEncode(ListButtonText);

        // Hide/Show edit document form
        bool editVisible = (editDoc.NodeID > 0);

        pnlEdit.Visible = editVisible;

        editDoc.SiteName = SiteName;
        editDoc.CultureCode = CultureCode;
        editDoc.LogActivity = LogActivity;
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Register the scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ContributionList_" + ClientID, ltlScript.Text);
        ltlScript.Text = String.Empty;

        base.OnPreRender(e);
    }


    private void ReloadData()
    {
        if (StopProcessing)
        {
            // Do nothing
            gridDocs.StopProcessing = true;
            editDoc.StopProcessing = true;
        }
        else
        {
            if (((AllowUsers == UserContributionAllowUserEnum.Authenticated) || (AllowUsers == UserContributionAllowUserEnum.DocumentOwner))
                && !AuthenticationHelper.IsAuthenticated())
            {
                // Not authenticated, do not display anything
                pnlList.Visible = false;
                pnlEdit.Visible = false;

                StopProcessing = true;
            }
            else
            {
                SetContext();

                // Hide document list
                gridDocs.Visible = false;

                // If the list of documents should be displayed ...
                if (DisplayList)
                {
                    // Get all documents of the current user
                    TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

                    // Generate additional where condition
                    WhereCondition condition = new WhereCondition(WhereCondition);

                    if (!String.IsNullOrEmpty(ClassNames))
                    {
                        condition.WhereIn("ClassName", ClassNames.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    
                    // Add user condition
                    if (AllowUsers == UserContributionAllowUserEnum.DocumentOwner)
                    {
                        condition.WhereEquals("NodeOwner", MembershipContext.AuthenticatedUser.UserID);
                    }

                    // Get the documents
                    var query =
                        DocumentHelper.GetDocuments()
                            .OnSite(SiteName)
                            .Path(MacroResolver.ResolveCurrentPath(Path))
                            .Where(condition)
                            .OrderBy(OrderBy)
                            .Published(SelectOnlyPublished)
                            .NestingLevel(MaxRelativeLevel)
                            .CheckPermissions(CheckPermissions);

                    TreeProvider.SetQueryCultures(query, CultureCode, CombineWithDefaultCulture);

                    // Do not apply published from / to columns to make sure the published information is correctly evaluated
                    query.Properties.ExcludedVersionedColumns = new[] { "DocumentPublishFrom", "DocumentPublishTo" };

                    var ds = query.Result;

                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        // Display and initialize grid if datasource is not empty
                        gridDocs.Visible = true;
                        gridDocs.DataSource = ds;
                        gridDocs.OrderBy = OrderBy;
                        editDoc.AlternativeFormName = AlternativeFormName;
                    }
                }

                bool isAuthorizedToCreateDoc = false;
                if (ParentNode != null)
                {
                    // Check if single class name is set
                    string className = (!string.IsNullOrEmpty(AllowedChildClasses) && !AllowedChildClasses.Contains(";")) ? AllowedChildClasses : null;

                    // Check user's permission to create new document if allowed
                    isAuthorizedToCreateDoc = !CheckPermissions || MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(ParentNodeID, className);
                    // Check group's permission to create new document if allowed
                    isAuthorizedToCreateDoc &= CheckGroupPermission("createpages");

                    if (!CheckDocPermissionsForInsert && CheckPermissions)
                    {
                        // If document permissions are not required check create permission on parent document
                        isAuthorizedToCreateDoc = MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(ParentNode, NodePermissionsEnum.Create) == AuthorizationResultEnum.Allowed;
                    }

                    if (AllowUsers == UserContributionAllowUserEnum.DocumentOwner)
                    {
                        if (VirtualContext.ReadonlyMode)
                        {
                            isAuthorizedToCreateDoc = false;
                        }
                        else
                        {
                            // Check if user is document owner (or global admin)
                            isAuthorizedToCreateDoc = isAuthorizedToCreateDoc 
                                && ((ParentNode.NodeOwner == MembershipContext.AuthenticatedUser.UserID) 
                                || MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin));
                        }
                    }
                }

                // Enable/disable inserting new document
                pnlNewDoc.Visible = (isAuthorizedToCreateDoc && AllowInsert);

                if (!gridDocs.Visible && !pnlNewDoc.Visible && pnlList.Visible)
                {
                    // Not authenticated to create new docs and grid is hidden
                    StopProcessing = true;
                }

                ReleaseContext();
            }
        }
    }


    /// <summary>
    /// Initializes and shows edit form with available documents.
    /// </summary>
    protected void btnNewDoc_Click(object sender, EventArgs e)
    {
        // Initialize EditForm control
        editDoc.EnableViewState = true;
        editDoc.AllowedChildClasses = AllowedChildClasses;
        editDoc.AllowDelete = AllowDelete && CheckGroupPermission("deletepages");
        editDoc.CheckPermissions = CheckPermissions;
        editDoc.CheckDocPermissionsForInsert = CheckDocPermissionsForInsert;
        editDoc.Action = "new";
        // Set parent nodeId
        editDoc.NodeID = ParentNodeID;
        editDoc.ClassID = 0;
        editDoc.AlternativeFormName = AlternativeFormName;
        editDoc.ValidationErrorMessage = ValidationErrorMessage;
        editDoc.LogActivity = LogActivity;

        pnlEdit.Visible = true;
        pnlList.Visible = false;
    }


    /// <summary>
    /// Gets parent node ID.
    /// </summary>
    private TreeNode GetParentNode()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        return tree.SelectSingleNode(SiteName, MacroResolver.ResolveCurrentPath(NewDocumentPath), TreeProvider.ALL_CULTURES);
    }


    /// <summary>
    /// Displays document list and hides edit form.
    /// </summary>
    protected void btnList_Click(object sender, EventArgs e)
    {
        // EditDoc control registers progress script on PreRender event - but when btnList button is clicked, EditDoc control gets hidden, so 
        // progress script needs to be registered again here to hide progress layout properly.
        if (editDoc.UseProgressScript)
        {
            ScriptHelper.RegisterLoader(Page);
        }
        pnlList.Visible = true;
        pnlEdit.Visible = false;
        editDoc.Action = "";
        editDoc.NodeID = 0;
        editDoc.EnableViewState = false;
    }


    /// <summary>
    /// UniGrid action buttons event handler.
    /// </summary>
    protected void gridDocs_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            // Edit document
            case "edit":
                // Check group's permission to edit document if allowed
                if (CheckGroupPermission("editpages"))
                {
                    editDoc.NodeID = ValidationHelper.GetInteger(actionArgument, 0);
                    editDoc.Action = "edit";
                    editDoc.CheckPermissions = CheckPermissions;
                    editDoc.AllowDelete = AllowDelete && CheckGroupPermission("deletepages");

                    pnlEdit.Visible = true;
                    pnlList.Visible = false;
                }
                break;

            // Delete document
            case "delete":
                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

                // Delete specified node
                int documentId = ValidationHelper.GetInteger(actionArgument, 0);
                TreeNode node = DocumentHelper.GetDocument(documentId, tree);
                if (node != null)
                {
                    // Check user's permission to delete document if allowed
                    bool hasUserDeletePermission = !CheckPermissions || IsUserAuthorizedToDeleteDocument(node);
                    // Check group's permission to delete document if allowed
                    hasUserDeletePermission &= CheckGroupPermission("deletepages");

                    if (hasUserDeletePermission)
                    {
                        DocumentHelper.DeleteDocument(node, tree);

                        // Fire OnAfterDelete
                        RaiseOnAfterDelete();

                        ReloadData();
                    }
                    // Access denied - not authorized to delete the document
                    else
                    {
                        AddAlert(String.Format(GetString("cmsdesk.notauthorizedtodeletedocument"), HTMLHelper.HTMLEncode(node.GetDocumentName())));
                    }
                }
                break;
        }
    }


    /// <summary>
    /// UniGrid external data bound.
    /// </summary>
    private object gridDocs_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();
        switch (sourceName)
        {
            // Column 'DocumentWorkflowStepID'
            case "documentworkflowstepid":
                int stepId = ValidationHelper.GetInteger(parameter, 0);
                if (stepId > 0)
                {
                    // Get workflow step display name
                    WorkflowStepInfo wsi = WorkflowStepInfo.Provider.Get(stepId);
                    if (wsi != null)
                    {
                        return ResHelper.LocalizeString(wsi.StepDisplayName);
                    }
                }
                break;

            case "documentmodifiedwhen":
            case "documentmodifiedwhentooltip":

                TimeZoneInfo tzi;

                // Get current time for user contribution list on live site
                string result = TimeZoneUIMethods.GetDateTimeForControl(this, ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME), out tzi).ToString();

                // Display time zone shift if needed
                if ((tzi != null) && TimeZoneHelper.TimeZonesEnabled)
                {
                    if (sourceName.EndsWithCSafe("tooltip"))
                    {
                        result = TimeZoneHelper.GetUTCLongStringOffset(tzi);
                    }
                    else
                    {
                        result += TimeZoneHelper.GetUTCStringOffset(tzi);
                    }
                }
                return result;

            // Action 'edit'
            case "edit":
                ((Control)sender).Visible = AllowEdit && CheckGroupPermission("editpages");
                break;

            // Action 'delete'
            case "delete":
                ((Control)sender).Visible = AllowDelete && CheckGroupPermission("deletepages");
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Checks whether the user is authorized to delete document.
    /// </summary>
    /// <param name="node">Document node</param>
    protected bool IsUserAuthorizedToDeleteDocument(TreeNode node)
    {
        // Check delete permission
        bool isAuthorized = MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, new [] { NodePermissionsEnum.Delete, NodePermissionsEnum.Read }) == AuthorizationResultEnum.Allowed;

        return isAuthorized;
    }


    /// <summary>
    /// Adds the alert message to the output request window.
    /// </summary>
    /// <param name="message">Message to display</param>
    private void AddAlert(string message)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), message.GetHashCode().ToString(), ScriptHelper.GetAlertScript(message));
    }


    /// <summary>
    /// Returns true if group permissions should be checked and specified permission is allowed in current group.
    /// Also returns true if group permissions should not be checked.
    /// </summary>
    /// <param name="permissionName">Permission to check (createpages, editpages, deletepages)</param>
    protected bool CheckGroupPermission(string permissionName)
    {
        if (CheckGroupPermissions && !MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Raises the OnAfterDelete event.
    /// </summary>
    private void RaiseOnAfterDelete()
    {
        if (OnAfterDelete != null)
        {
            OnAfterDelete(this, null);
        }
    }

    #endregion
}
