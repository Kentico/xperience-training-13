using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Threading;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Content_CMSDesk_PublishArchive : CMSContentPage
{
    #region "Private variables & enums"

    private readonly List<int> nodeIds = new List<int>();
    private readonly Dictionary<int, string> list = new Dictionary<int, string>();
    private Hashtable mParameters;

    private int cancelNodeId;
    private CurrentUserInfo currentUser;
    private string currentSiteName;
    private int currentSiteId;
    private string currentCulture = CultureHelper.DefaultUICultureCode;
    private string canceledString;

    private WorkflowAction mCurrentAction = WorkflowAction.None;

    protected enum WorkflowAction
    {
        None = 0,
        Publish = 1,
        Archive = 2
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Current Error.
    /// </summary>
    public string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }


    /// <summary>
    /// Determines current action.
    /// </summary>
    private WorkflowAction CurrentAction
    {
        get
        {
            if (mCurrentAction == WorkflowAction.None)
            {
                string actionString = QueryHelper.GetString("action", WorkflowAction.None.ToString());
                mCurrentAction = (WorkflowAction)Enum.Parse(typeof(WorkflowAction), actionString);
            }

            return mCurrentAction;
        }
    }


    /// <summary>
    /// Where condition used for multiple actions.
    /// </summary>
    private string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(Parameters["where"], string.Empty);
        }
    }


    /// <summary>
    /// Gets selected class ID.
    /// </summary>
    private int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("classid", 0);
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }


    /// <summary>
    /// Indicates whether action is multiple.
    /// </summary>
    private bool AllLevels
    {
        get
        {
            return QueryHelper.GetBoolean("alllevels", false);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register main CMS script file
        ScriptHelper.RegisterCMS(this);

        if (QueryHelper.ValidateHash("hash") && (Parameters != null))
        {
            // Initialize current user
            currentUser = MembershipContext.AuthenticatedUser;

            // Check permissions
            if (!currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) &&
                !currentUser.IsAuthorizedPerResource("CMS.Content", "manageworkflow"))
            {
                RedirectToAccessDenied("CMS.Content", "manageworkflow");
            }

            // Set current UI culture
            currentCulture = CultureHelper.PreferredUICultureCode;

            // Initialize current site
            currentSiteName = SiteContext.CurrentSiteName;
            currentSiteId = SiteContext.CurrentSiteID;

            // Initialize events
            ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
            ctlAsyncLog.OnError += ctlAsyncLog_OnError;
            ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

            if (!RequestHelper.IsCallback())
            {
                DataSet allDocs = null;
                TreeProvider tree = new TreeProvider(currentUser);

                // Current Node ID to delete
                string parentAliasPath = ValidationHelper.GetString(Parameters["parentaliaspath"], string.Empty);
                if (string.IsNullOrEmpty(parentAliasPath))
                {
                    // Get IDs of nodes
                    string nodeIdsString = ValidationHelper.GetString(Parameters["nodeids"], string.Empty);
                    string[] nodeIdsArr = nodeIdsString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string nodeId in nodeIdsArr)
                    {
                        int id = ValidationHelper.GetInteger(nodeId, 0);
                        if (id != 0)
                        {
                            nodeIds.Add(id);
                        }
                    }
                }
                else
                {
                    var where = new WhereCondition(WhereCondition)
                        .WhereNotEquals("ClassName", SystemDocumentTypes.Root);
                    string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS,
                                                                 "NodeParentID, DocumentName,DocumentCheckedOutByUserID");
                    allDocs = tree.SelectNodes(currentSiteName, parentAliasPath.TrimEnd('/') + "/%",
                                               TreeProvider.ALL_CULTURES, true, DataClassInfoProvider.GetClassName(ClassID), where.ToString(true), "DocumentName", 1, false, 0,
                                               columns);

                    if (!DataHelper.DataSourceIsEmpty(allDocs))
                    {
                        foreach (DataRow row in allDocs.Tables[0].Rows)
                        {
                            nodeIds.Add(ValidationHelper.GetInteger(row["NodeID"], 0));
                        }
                    }
                }

                // Initialize strings based on current action
                string titleText = null;

                switch (CurrentAction)
                {
                    case WorkflowAction.Archive:
                        headQuestion.ResourceString = "content.archivequestion";
                        chkAllCultures.ResourceString = "content.archiveallcultures";
                        chkUnderlying.ResourceString = "content.archiveunderlying";
                        canceledString = GetString("content.archivecanceled");

                        // Setup title of log
                        ctlAsyncLog.TitleText = GetString("content.archivingdocuments");
                        // Setup page title text and image
                        titleText = GetString("Content.ArchiveTitle");
                        break;

                    case WorkflowAction.Publish:
                        headQuestion.ResourceString = "content.publishquestion";
                        chkAllCultures.ResourceString = "content.publishallcultures";
                        chkUnderlying.ResourceString = "content.publishunderlying";
                        canceledString = GetString("content.publishcanceled");

                        // Setup title of log
                        ctlAsyncLog.TitleText = GetString("content.publishingdocuments");
                        // Setup page title text and image
                        titleText = GetString("Content.PublishTitle");
                        break;
                }

                PageTitle.TitleText = titleText;
                EnsureDocumentBreadcrumbs(PageBreadcrumbs, action: PageTitle.TitleText);

                if (nodeIds.Count == 0)
                {
                    // Hide if no node was specified
                    pnlContent.Visible = false;
                    return;
                }

                // Register the dialog script
                ScriptHelper.RegisterDialogScript(this);

                // Set visibility of panels
                pnlContent.Visible = true;
                pnlLog.Visible = false;

                // Set all cultures checkbox
                DataSet culturesDS = CultureSiteInfoProvider.GetSiteCultures(currentSiteName);
                if ((DataHelper.DataSourceIsEmpty(culturesDS)) || (culturesDS.Tables[0].Rows.Count <= 1))
                {
                    chkAllCultures.Checked = true;
                    plcAllCultures.Visible = false;
                }

                if (nodeIds.Count > 0)
                {
                    pnlDocList.Visible = true;

                    // Create where condition
                    string where = new WhereCondition().WhereIn("NodeID", nodeIds).ToString(true);
                    string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeParentID, DocumentName,DocumentCheckedOutByUserID");

                    // Select nodes
                    DataSet ds = allDocs ?? tree.SelectNodes(currentSiteName, "/%", TreeProvider.ALL_CULTURES, true, null, where, "DocumentName", TreeProvider.ALL_LEVELS, false, 0, columns);

                    // Enumerate selected documents
                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        cancelNodeId = DataHelper.GetIntValue(ds.Tables[0].Rows[0], "NodeParentID");

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            AddToList(dr);
                        }

                        // Display enumeration of documents
                        foreach (KeyValuePair<int, string> line in list)
                        {
                            lblDocuments.Text += line.Value;
                        }
                    }
                }
            }

            // Set title for dialog mode
            string title = GetString("general.publish");

            if (CurrentAction == WorkflowAction.Archive)
            {
                title = GetString("general.archive");
            }

            SetTitle(title);
        }
        else
        {
            pnlPublish.Visible = false;
            ShowError(GetString("dialogs.badhashtext"));
        }


    }

    #endregion


    #region "Button actions"

    protected void btnNo_Click(object sender, EventArgs e)
    {
        // Go back to listing
        SelectNode();
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;
        pnlContent.Visible = false;

        CurrentError = string.Empty;

        ctlAsyncLog.EnsureLog();
        ctlAsyncLog.Parameter = LocalizationContext.PreferredCultureCode + ";" + SiteContext.CurrentSiteName;

        switch (CurrentAction)
        {
            case WorkflowAction.Publish:
                ctlAsyncLog.RunAsync(PublishAll, WindowsIdentity.GetCurrent());
                break;

            case WorkflowAction.Archive:
                ctlAsyncLog.RunAsync(ArchiveAll, WindowsIdentity.GetCurrent());
                break;
        }
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// Archives document(s).
    /// </summary>
    private void ArchiveAll(object parameter)
    {
        if (parameter == null)
        {
            return;
        }

        TreeProvider tree = new TreeProvider(currentUser);
        tree.AllowAsyncActions = false;

        try
        {
            AddLog(GetString("content.archivingdocuments", currentCulture));

            string[] parameters = ((string)parameter).Split(';');
            string siteName = parameters[1];
            int[] workNodes = nodeIds.ToArray();
            string where = new WhereCondition().WhereIn("NodeID", workNodes).ToString(true);
            string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeAliasPath, ClassName, DocumentCulture");
            string cultureCode = chkAllCultures.Checked ? TreeProvider.ALL_CULTURES : parameters[0];

            // Get the documents
            var documents = tree.SelectNodes(siteName, "/%", cultureCode, false, null, where, "NodeAliasPath DESC", TreeProvider.ALL_LEVELS, false, 0, columns);
            if (!DataHelper.DataSourceIsEmpty(documents))
            {
                foreach (DataRow nodeRow in documents.Tables[0].Rows)
                {
                    if (PerformArchive(tree, nodeRow, cultureCode))
                    {
                        return;
                    }
                }
            }
            else
            {
                AddError(GetString("content.nothingtoarchive", currentCulture));
            }
        }
        catch (ThreadAbortException ex)
        {
            if (!CMSThread.Stopped(ex))
            {
                // Log error
                LogExceptionToEventLog(GetString("content.archivefailed", currentCulture), ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(GetString("content.archivefailed", currentCulture), ex);
        }
    }


    /// <summary>
    /// Archives sub documents of given node.
    /// </summary>
    /// <returns>TRUE if operation fails and whole process should be canceled.</returns>
    private bool ArchiveSubDocuments(TreeNode parentNode, TreeProvider tree, string cultureCode)
    {
        var subDocuments = GetSubDocuments(parentNode, tree, cultureCode);
        if (DataHelper.DataSourceIsEmpty(subDocuments))
        {
            return false;
        }

        foreach (DataRow nodeRow in subDocuments.Tables[0].Rows)
        {
            if (PerformArchive(tree, nodeRow, cultureCode))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Publishes document(s).
    /// </summary>
    private void PublishAll(object parameter)
    {
        if (parameter == null)
        {
            return;
        }

        TreeProvider tree = new TreeProvider(currentUser);
        tree.AllowAsyncActions = false;

        try
        {
            AddLog(GetString("content.publishingdocuments", currentCulture));

            string[] parameters = ((string)parameter).Split(';');
            string siteName = parameters[1];
            int[] workNodes = nodeIds.ToArray();
            string where = new WhereCondition().WhereIn("NodeID", workNodes).ToString(true);
            string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeAliasPath, ClassName, DocumentCulture");
            string cultureCode = chkAllCultures.Checked ? TreeProvider.ALL_CULTURES : parameters[0];

            // Get the documents
            var documents = tree.SelectNodes(siteName, "/%", cultureCode, false, null, where, "NodeAliasPath DESC", TreeProvider.ALL_LEVELS, false, 0, columns);
            if (!DataHelper.DataSourceIsEmpty(documents))
            {
                foreach (DataRow nodeRow in documents.Tables[0].Rows)
                {
                    if (PerformPublish(tree, nodeRow, cultureCode))
                    {
                        return;
                    }
                }
            }
            else
            {
                AddError(GetString("content.nothingtopublish", currentCulture));
            }
        }
        catch (ThreadAbortException ex)
        {
            if (!CMSThread.Stopped(ex))
            {
                // Log error
                LogExceptionToEventLog(GetString("content.publishfailed", currentCulture), ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(GetString("content.publishfailed", currentCulture), ex);
        }
    }


    /// <summary>
    /// Publishes sub documents of given node.
    /// </summary>
    /// <returns>TRUE if operation fails and whole process should be canceled.</returns>
    private bool PublishSubDocuments(TreeNode parentNode, TreeProvider tree, string cultureCode)
    {
        var subDocuments = GetSubDocuments(parentNode, tree, cultureCode);
        if (DataHelper.DataSourceIsEmpty(subDocuments))
        {
            return false;
        }

        foreach (DataRow nodeRow in subDocuments.Tables[0].Rows)
        {
            if (PerformPublish(tree, nodeRow, cultureCode))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Performs necessary checks and publishes document.
    /// </summary>
    /// <returns>TRUE if operation fails and whole process should be canceled.</returns>
    private bool PerformPublish(TreeProvider tree, DataRow nodeRow, string cultureCode)
    {
        var node = GetDocument(nodeRow, tree);
        if (node == null)
        {
            string aliasPath = ValidationHelper.GetString(nodeRow["NodeAliasPath"], string.Empty);
            AddLog(String.Format(GetString("ContentRequest.DocumentNoLongerExists", currentCulture), HTMLHelper.HTMLEncode(aliasPath)));
            return false;
        }

        if (PerformPublish(node))
        {
            return true;
        }

        // Process underlying documents
        if (chkUnderlying.Checked && node.NodeHasChildren && PublishSubDocuments(node, tree, cultureCode))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Performs necessary checks and publishes document.
    /// </summary>
    /// <returns>TRUE if operation fails and whole process should be canceled.</returns>
    private bool PerformPublish(TreeNode node)
    {
        if (currentUser.UserHasAllowedCultures && !currentUser.IsCultureAllowed(node.DocumentCulture, node.NodeSiteName))
        {
            AddLog(String.Format(GetString("content.notallowedtomodifycultureversion"), node.DocumentCulture, HTMLHelper.HTMLEncode(node.NodeAliasPath)));
        }
        else
        {
            if (!UndoPossibleCheckOut(node))
            {
                return true;
            }

            WorkflowStepInfo currentStep = null;
            try
            {
                // Try to get workflow scope
                currentStep = node.WorkflowManager.EnsureStepInfo(node);
            }
            catch (ThreadAbortException te)
            {
                AddLog(te.Message);
            }
            catch
            {
                AddLog(String.Format(GetString("content.publishnowf"), HTMLHelper.HTMLEncode(node.NodeAliasPath + " (" + node.DocumentCulture + ")")));
                return false;
            }

            if (currentStep != null)
            {
                return Publish(node, currentStep);
            }
        }

        return false;
    }


    /// <summary>
    /// Performs necessary checks and archives document.
    /// </summary>
    /// <returns>TRUE if operation fails and whole process should be canceled.</returns>
    private bool PerformArchive(TreeProvider tree, DataRow nodeRow, string cultureCode)
    {
        var node = GetDocument(nodeRow, tree);
        if (node == null)
        {
            var aliasPath = ValidationHelper.GetString(nodeRow["NodeAliasPath"], String.Empty);
            AddLog(String.Format(GetString("ContentRequest.DocumentNoLongerExists", currentCulture), HTMLHelper.HTMLEncode(aliasPath)));
            return false;
        }

        if (PerformArchive(node))
        {
            return true;
        }

        // Process underlying documents
        if (chkUnderlying.Checked && node.NodeHasChildren && ArchiveSubDocuments(node, tree, cultureCode))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Performs necessary checks and archives document.
    /// </summary>
    /// <returns>TRUE if operation fails and whole process should be canceled.</returns>
    private bool PerformArchive(TreeNode node)
    {
        if (!UndoPossibleCheckOut(node))
        {
            return true;
        }
        try
        {
            if (currentUser.UserHasAllowedCultures && !currentUser.IsCultureAllowed(node.DocumentCulture, node.NodeSiteName))
            {
                AddLog(String.Format(GetString("content.notallowedtomodifycultureversion"), node.DocumentCulture, node.NodeAliasPath));
            }
            else
            {
                // Add log record
                AddLog(HTMLHelper.HTMLEncode(node.NodeAliasPath + " (" + node.DocumentCulture + ")"));

                // Archive document
                node.WorkflowManager.ArchiveDocument(node, String.Empty);
            }
        }
        catch (ThreadAbortException te)
        {
            AddLog(te.Message);
        }
        catch
        {
            AddLog(String.Format(GetString("content.archivenowf"), HTMLHelper.HTMLEncode(node.NodeAliasPath + " (" + node.DocumentCulture + ")")));
        }
        return false;
    }


    /// <summary>
    /// Publishes document.
    /// </summary>
    /// <param name="node">Node to publish</param>
    /// <param name="currentStep">Current workflow step</param>
    /// <returns>TRUE if operation fails</returns>
    private bool Publish(TreeNode node, WorkflowStepInfo currentStep)
    {
        string pathCulture = HTMLHelper.HTMLEncode(node.NodeAliasPath + " (" + node.DocumentCulture + ")");

        bool alreadyPublished = (currentStep == null) || currentStep.StepIsPublished;
        if (!alreadyPublished)
        {
            // Publish document
            currentStep = node.WorkflowManager.PublishDocument(node);
        }

        // Document is already published, check if still under workflow
        if (alreadyPublished && (currentStep != null) && currentStep.StepIsPublished)
        {
            WorkflowScopeInfo wsi = node.WorkflowManager.GetNodeWorkflowScope(node);
            if (wsi == null)
            {
                VersionManager vm = VersionManager.GetInstance(node.TreeProvider);
                vm.RemoveWorkflow(node);
            }
        }

        // Document already published
        if (alreadyPublished)
        {
            AddLog(String.Format(GetString("content.publishedalready"), pathCulture));
        }
        else if ((currentStep == null) || !currentStep.StepIsPublished)
        {
            AddError(String.Format(GetString("content.PublishWasApproved"), pathCulture));
            return true;
        }
        else
        {
            // Add log record
            AddLog(pathCulture);
        }

        return false;
    }


    /// <summary>
    /// Undoes checkout for given node.
    /// </summary>
    /// <param name="node">Node to undo checkout</param>
    /// <returns>FALSE when document is checked out and checkbox for undoing checkout is not checked</returns>
    private bool UndoPossibleCheckOut(TreeNode node)
    {
        if (!node.IsCheckedOut)
        {
            return true;
        }

        string nodeAliasPath = HTMLHelper.HTMLEncode(node.NodeAliasPath + " (" + node.DocumentCulture + ")");
        if (CurrentUser.UserID != node.DocumentCheckedOutByUserID)
        {
            // Get checked out message
            var user = UserInfoProvider.GetUserInfo(node.DocumentCheckedOutByUserID);
            string userName = user != null ? user.GetFormattedUserName(false) : "";
            AddError(String.Format(GetString("editcontent.documentnamecheckedoutbyanother"), nodeAliasPath, userName));
        }
        else
        {
            AddError(String.Format(GetString("content.checkedoutdocument"), nodeAliasPath));
        }

        return false;
    }

    #endregion


    #region "Help methods"

    private static TreeNode GetDocument(DataRow nodeRow, TreeProvider tree)
    {
        int documentId = ValidationHelper.GetInteger(nodeRow["DocumentID"], 0);
        return DocumentHelper.GetDocument(documentId, tree);
    }


    private static DataSet GetSubDocuments(TreeNode parentNode, TreeProvider tree, string cultureCode)
    {
        string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeAliasPath, ClassName, DocumentCulture");
        // Get subdocuments
        return tree.SelectNodes(parentNode.NodeSiteName, parentNode.NodeAliasPath + "/%", cultureCode, false, null, null, null, TreeProvider.ALL_LEVELS, false, 0, columns);
    }


    /// <summary>
    /// When exception occures, log it to event log.
    /// </summary>
    /// <param name="messageTitle">Title message</param>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(string messageTitle, Exception ex)
    {
        AddError(messageTitle + ": " + ex.Message);
        LogContext.LogEventToCurrent(EventType.ERROR, "Content", "PUBLISHDOC", EventLogProvider.GetExceptionLogMessage(ex), RequestContext.RawURL, currentUser.UserID, currentUser.UserName, 0, null, RequestContext.UserHostAddress, currentSiteId, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ltlScript.Text += ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Adds document to list.
    /// </summary>
    /// <param name="dr">Data row with document</param>
    private void AddToList(DataRow dr)
    {
        int nodeId = ValidationHelper.GetInteger(dr["NodeID"], 0);
        int linkedNodeId = ValidationHelper.GetInteger(dr["NodeLinkedNodeID"], 0);
        int documentCheckedOutByUserID = ValidationHelper.GetInteger(dr["DocumentCheckedOutByUserID"], 0);
        string name = HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["DocumentName"], string.Empty));
        if (documentCheckedOutByUserID != 0)
        {
            name += " " + DocumentUIHelper.GetDocumentMarkImage(Page, DocumentMarkEnum.CheckedOut);
        }
        if (linkedNodeId != 0)
        {
            name += " " + DocumentUIHelper.GetDocumentMarkImage(Page, DocumentMarkEnum.Link);
        }
        name += "<br />";
        list[nodeId] = name;
    }


    private void HandlePossibleError()
    {
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        AddScript("var __pendingCallbacks = new Array();");
        RefreshTree();
        AddLog(canceledString);
        ShowConfirmation(canceledString);
        HandlePossibleError();
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        RefreshTree();
        HandlePossibleError();
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(CurrentError))
        {
            SelectNode();
        }
        else
        {
            RefreshTree();
        }
        HandlePossibleError();
    }


    /// <summary>
    /// Selects node in content tree.
    /// </summary>
    private void SelectNode()
    {
        // Overwrite cancelNodeId variable if sub-levels are visible
        if (AllLevels && Parameters.ContainsKey("refreshnodeid"))
        {
            cancelNodeId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
        }

        if (cancelNodeId != 0)
        {
            if (RequiresDialog)
            {
                AddScript("CloseDialog();");
            }
            else
            {
                AddScript("SelectNode(" + cancelNodeId + ");");
            }
        }
    }


    /// <summary>
    /// Refreshes content tree.
    /// </summary>
    private void RefreshTree()
    {
        // Overwrite cancelNodeId variable if sub-levels are visible
        if (AllLevels && Parameters.ContainsKey("refreshnodeid"))
        {
            cancelNodeId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
        }

        if (cancelNodeId != 0)
        {
            AddScript("RefreshTree(" + cancelNodeId + ");");
        }
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}
