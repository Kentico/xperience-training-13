using System;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Workflows_Workflow_Documents : CMSWorkflowPage
{
    #region "Private variables & enumerations"

    private CurrentUserInfo mCurrentUser;
    private string mCurrentCulture = CultureHelper.DefaultUICultureCode;

    private const string GET_WORKFLOW_DOCS_WHERE = "DocumentWorkflowStepID IN (SELECT StepID FROM CMS_WorkflowStep WHERE StepWorkflowID = {0})";

    private enum Action
    {
        SelectAction = 0,
        PublishAndFinish = 1,
        RemoveWorkflow = 2
    }

    protected enum What
    {
        SelectedDocuments = 0,
        AllDocuments = 1
    }

    private What mCurrentWhat = default(What);

    #endregion


    #region "Properties"
    
    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
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
    /// Current Info.
    /// </summary>
    private string CurrentInfo
    {
        get
        {
            return ctlAsyncLog.ProcessData.Information;
        }
        set
        {
            ctlAsyncLog.ProcessData.Information = value;
        }
    }


    /// <summary>
    /// Gets or sets the cancel string.
    /// </summary>
    public string CanceledString
    {
        get
        {
            return ctlAsyncLog.ProcessData.CancelledInfo;
        }
        set
        {
            ctlAsyncLog.ProcessData.CancelledInfo = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mCurrentUser = MembershipContext.AuthenticatedUser;
        mCurrentCulture = CultureHelper.PreferredUICultureCode;

        RegisterEventHandlers();

        if (!RequestHelper.IsCallback())
        {
            // Set visibility of panels
            pnlContent.Visible = true;
            pnlLog.Visible = false;

            InitializeUniGrid();

            RegisterPerformActionScript();

            AssignPerformActionScriptToBtnOkOnclick();

            // Initialize dropdown list with actions
            if (!RequestHelper.IsPostBack())
            {
                if (mCurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || mCurrentUser.IsAuthorizedPerResource("CMS.Content", "manageworkflow"))
                {
                    PopulateActionDropDown();
                }

                PopulateWhatSelectionDropDown();
            }
        }
        docElem.SiteName = filterDocuments.SelectedSite;
    }
    
    #endregion


    #region "Other events"

    protected void UniGrid_OnBeforeDataReload()
    {
        string where = string.Format(GET_WORKFLOW_DOCS_WHERE, WorkflowId);
        where = SqlHelper.AddWhereCondition(where, filterDocuments.WhereCondition);
        docElem.UniGrid.WhereCondition = SqlHelper.AddWhereCondition(docElem.UniGrid.WhereCondition, where);
    }


    protected void UniGrid_OnAfterDataReload()
    {
        plcFilter.Visible = docElem.UniGrid.DisplayExternalFilter(filterDocuments.FilterIsSet);
        pnlFooter.Visible = !DataHelper.DataSourceIsEmpty(docElem.UniGrid.GridView.DataSource);
    }

    #endregion


    #region "Button handling"

    protected void btnOk_OnClick(object sender, EventArgs e)
    {
        pnlLog.Visible = true;
        pnlContent.Visible = false;

        CurrentError = string.Empty;
        
        int actionValue = ValidationHelper.GetInteger(drpAction.SelectedValue, 0);
        Action action = (Action)actionValue;

        int whatValue = ValidationHelper.GetInteger(drpWhat.SelectedValue, 0);
        mCurrentWhat = (What)whatValue;

        ctlAsyncLog.Parameter = GetCurrentWhere();
        switch (action)
        {
            case Action.PublishAndFinish:
                // Publish and finish workflow
                ctlAsyncLog.TitleText = GetString("content.publishingdocuments");
                ctlAsyncLog.RunAsync(PublishAndFinish, WindowsIdentity.GetCurrent());
                break;

            case Action.RemoveWorkflow:
                // Remove workflow
                ctlAsyncLog.TitleText = GetString("workflowdocuments.removingwf");
                ctlAsyncLog.RunAsync(RemoveWorkflow, WindowsIdentity.GetCurrent());
                break;
        }
    }

    #endregion


    #region "Thread methods"

    private void PublishAndFinish(object parameter)
    {
        TreeNode node = null;
        Tree.AllowAsyncActions = false;
        CanceledString = GetString("content.publishcanceled", mCurrentCulture);
        try
        {
            // Begin log
            AddLog(GetString("content.preparingdocuments", mCurrentCulture));

            string where = parameter as string;

            // Get the documents
            DataSet documents = GetDocumentsToProcess(where);

            if (!DataHelper.DataSourceIsEmpty(documents))
            {
                // Create instance of workflow manager class
                WorkflowManager wm = WorkflowManager.GetInstance(Tree);

                // Begin publishing
                AddLog(GetString("content.publishingdocuments", mCurrentCulture));
                foreach (DataTable classTable in documents.Tables)
                {
                    foreach (DataRow nodeRow in classTable.Rows)
                    {
                        // Get the current document
                        string className = ValidationHelper.GetString(nodeRow["ClassName"], string.Empty);
                        string aliasPath = ValidationHelper.GetString(nodeRow["NodeAliasPath"], string.Empty);
                        string docCulture = ValidationHelper.GetString(nodeRow["DocumentCulture"], string.Empty);
                        string siteName = SiteInfoProvider.GetSiteName(nodeRow["NodeSiteID"].ToInteger(0));

                        node = DocumentHelper.GetDocument(siteName, aliasPath, docCulture, false, className, null, null, TreeProvider.ALL_LEVELS, false, null, Tree);

                        // Publish document
                        if (Publish(node, wm))
                        {
                            return;
                        }
                    }
                }

                CurrentInfo = GetString("workflowdocuments.publishcomplete");
            }
            else
            {
                AddError(GetString("content.nothingtopublish", mCurrentCulture));
            }
        }
        catch (ThreadAbortException ex)
        {
            if (CMSThread.Stopped(ex))
            {
                // When canceled
                CurrentInfo = CanceledString;
            }
            else
            {
                int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
                // Log error
                LogExceptionToEventLog("PUBLISHDOC", "content.publishfailed", ex, siteId);
            }
        }
        catch (Exception ex)
        {
            int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
            // Log error
            LogExceptionToEventLog("PUBLISHDOC", "content.publishfailed", ex, siteId);
        }
    }


    private void RemoveWorkflow(object parameter)
    {
        VersionManager verMan = VersionManager.GetInstance(Tree);
        TreeNode node = null;
        // Custom logging
        Tree.LogEvents = false;
        Tree.AllowAsyncActions = false;
        CanceledString = GetString("workflowdocuments.removingcanceled", mCurrentCulture);
        try
        {
            // Begin log
            AddLog(GetString("content.preparingdocuments", mCurrentCulture));

            string where = parameter as string;

            // Get the documents
            DataSet documents = GetDocumentsToProcess(where);

            if (!DataHelper.DataSourceIsEmpty(documents))
            {
                // Begin log
                AddLog(GetString("workflowdocuments.removingwf", mCurrentCulture));

                foreach (DataTable classTable in documents.Tables)
                {
                    foreach (DataRow nodeRow in classTable.Rows)
                    {
                        // Get the current document
                        string className = ValidationHelper.GetString(nodeRow["ClassName"], string.Empty);
                        string aliasPath = ValidationHelper.GetString(nodeRow["NodeAliasPath"], string.Empty);
                        string docCulture = ValidationHelper.GetString(nodeRow["DocumentCulture"], string.Empty);
                        string siteName = SiteInfoProvider.GetSiteName(nodeRow["NodeSiteID"].ToInteger(0));

                        // Get published version
                        node = Tree.SelectSingleNode(siteName, aliasPath, docCulture, false, className, false);
                        string encodedAliasPath = HTMLHelper.HTMLEncode(ValidationHelper.GetString(aliasPath, string.Empty) + " (" + node.GetValue("DocumentCulture") + ")");

                        // Destroy document history
                        verMan.DestroyDocumentHistory(node.DocumentID);

                        using (new CMSActionContext { LogEvents = false })
                        {
                            // Clear workflow
                            DocumentHelper.ClearWorkflowInformation(node);
                            node.Update();
                        }

                        // Add log record
                        AddLog(encodedAliasPath);

                        // Add record to eventlog
                        LogContext.LogEventToCurrent(EventType.INFORMATION, "Content", "REMOVEDOCWORKFLOW", string.Format(GetString("workflowdocuments.removeworkflowsuccess"), encodedAliasPath), RequestContext.RawURL, mCurrentUser.UserID, mCurrentUser.UserName, node.NodeID, node.GetDocumentName(), RequestContext.UserHostAddress, node.NodeSiteID, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
                    }
                }
                CurrentInfo = GetString("workflowdocuments.removecomplete");
            }
            else
            {
                AddError(GetString("workflowdocuments.nodocumentstoclear", mCurrentCulture));
            }
        }
        catch (ThreadAbortException ex)
        {
            if (CMSThread.Stopped(ex))
            {
                // When canceled
                CurrentInfo = CanceledString;
            }
            else
            {
                int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
                // Log error
                LogExceptionToEventLog("REMOVEDOCWORKFLOW", "workflowdocuments.removefailed", ex, siteId);
            }
        }
        catch (Exception ex)
        {
            int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
            // Log error
            LogExceptionToEventLog("REMOVEDOCWORKFLOW", "workflowdocuments.removefailed", ex, siteId);
        }
    }

    #endregion


    #region "Private methods"

    private void RegisterEventHandlers()
    {
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;
    }


    private void InitializeUniGrid()
    {
        docElem.ZeroRowsText = GetString(filterDocuments.FilterIsSet ? "unigrid.filteredzerorowstext" : "workflowdocuments.nodata");
        docElem.UniGrid.OnAfterDataReload += UniGrid_OnAfterDataReload;
        docElem.UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
        docElem.Tree = Tree;
    }


    private void RegisterPerformActionScript()
    {
        var actionScript = new StringBuilder();
        actionScript.AppendLine("function PerformAction(selectionFunction, selectionField, dropId)");
        actionScript.AppendLine("{");
        actionScript.AppendLine("   var selectionFieldElem = document.getElementById(selectionField);");
        actionScript.AppendLine("   var label = document.getElementById('" + lblValidation.ClientID + "');");
        actionScript.AppendLine("   var items = selectionFieldElem.value;");
        actionScript.AppendLine("   var whatDrp = document.getElementById('" + drpWhat.ClientID + "');");
        actionScript.AppendLine("   var action = document.getElementById(dropId).value;");
        actionScript.AppendLine("   if (action == '" + (int)Action.SelectAction + "')");
        actionScript.AppendLine("   {");
        actionScript.AppendLine("       label.innerHTML = " + ScriptHelper.GetLocalizedString("massaction.selectsomeaction") + ";");
        actionScript.AppendLine("       return false;");
        actionScript.AppendLine("   }");
        actionScript.AppendLine("   if(!eval(selectionFunction) || whatDrp.value == '" + (int)What.AllDocuments + "')");
        actionScript.AppendLine("   {");
        actionScript.AppendLine("       var confirmed = false;");
        actionScript.AppendLine("       var confMessage = '';");
        actionScript.AppendLine("       switch(action)");
        actionScript.AppendLine("       {");
        actionScript.AppendLine("           case '" + (int)Action.PublishAndFinish + "':");
        actionScript.AppendLine("               confMessage = " + ScriptHelper.GetLocalizedString("workflowdocuments.confrimpublish") + ";");
        actionScript.AppendLine("               break;");
        actionScript.AppendLine("           case '" + (int)Action.RemoveWorkflow + "':");
        actionScript.AppendLine("               confMessage = " + ScriptHelper.GetLocalizedString("workflowdocuments.confirmremove") + ";");
        actionScript.AppendLine("               break;");
        actionScript.AppendLine("       }");
        actionScript.AppendLine("       return confirm(confMessage);");
        actionScript.AppendLine("   }");
        actionScript.AppendLine("   else");
        actionScript.AppendLine("   {");
        actionScript.AppendLine("       label.innerHTML = " + ScriptHelper.GetLocalizedString("documents.selectdocuments") + ";");
        actionScript.AppendLine("       return false;");
        actionScript.AppendLine("   }");
        actionScript.AppendLine("}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "actionScript", ScriptHelper.GetScript(actionScript.ToString()));
    }


    private void AssignPerformActionScriptToBtnOkOnclick()
    {
        // Add action to button
        btnOk.OnClientClick = "return PerformAction('" + docElem.UniGrid.GetCheckSelectionScript() + "','" + docElem.UniGrid.GetSelectionFieldClientID() + "','" + drpAction.ClientID + "');";
    }


    private void PopulateActionDropDown()
    {
        drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
        drpAction.Items.Add(new ListItem(GetString("workflowdocuments." + Action.PublishAndFinish), Convert.ToInt32(Action.PublishAndFinish).ToString()));
        drpAction.Items.Add(new ListItem(GetString("workflowdocuments." + Action.RemoveWorkflow), Convert.ToInt32(Action.RemoveWorkflow).ToString()));
    }


    private void PopulateWhatSelectionDropDown()
    {
        drpWhat.Items.Add(new ListItem(GetString("contentlisting." + What.SelectedDocuments), Convert.ToInt32(What.SelectedDocuments).ToString()));
        drpWhat.Items.Add(new ListItem(GetString("contentlisting." + What.AllDocuments), Convert.ToInt32(What.AllDocuments).ToString()));
    }


    /// <summary>
    /// Gets a set of documents to be processed.
    /// </summary>
    /// <returns>Set of documents based on where condition and other settings</returns>
    private DataSet GetDocumentsToProcess(string where)
    {
        string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeAliasPath, ClassName, DocumentCulture");
        return DocumentHelper.GetDocuments(TreeProvider.ALL_SITES, "/%", TreeProvider.ALL_CULTURES, false, TreeProvider.ALL_CLASSNAMES, where, "NodeAliasPath DESC", -1, false, -1, columns, Tree);
    }


    /// <summary>
    /// Gets current where condition.
    /// </summary>
    /// <returns>Where condition determining documents to process</returns>
    private string GetCurrentWhere()
    {
        var documentIds = docElem.UniGrid.SelectedItems;

        // Prepare the where condition
        var condition = new WhereCondition();

        if (mCurrentWhat == What.SelectedDocuments)
        {
            // Get where for selected documents
            condition.WhereIn("DocumentID", documentIds.ToList());
        }
        else
        {
            if (!string.IsNullOrEmpty(filterDocuments.WhereCondition))
            {
                // Add filter condition
                condition.Where(filterDocuments.WhereCondition);
            }

            // Select documents only under current workflow
            condition.WhereIn("DocumentWorkflowStepID", new IDQuery<WorkflowStepInfo>().WhereEquals("StepWorkflowID", WorkflowId));
        }
        return condition.ToString(true);
    }


    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    /// <param name="eventCode">Code of event</param>
    /// <param name="errorTitle">Message to log to asynchronous log</param>
    /// <param name="ex">Exception to log</param>
    /// <param name="siteId">ID of site</param>
    private void LogExceptionToEventLog(string eventCode, string errorTitle, Exception ex, int siteId)
    {
        AddError(GetString(errorTitle, mCurrentCulture) + ": " + ex.Message);
        LogContext.LogEventToCurrent(EventType.ERROR, "Content", eventCode, EventLogProvider.GetExceptionLogMessage(ex), RequestContext.RawURL, mCurrentUser.UserID, mCurrentUser.UserName, 0, null, RequestContext.UserHostAddress, siteId, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
    }


    /// <summary>
    /// Publishes document.
    /// </summary>
    /// <param name="node">Node to publish</param>
    /// <param name="wm">Workflow manager</param>
    /// <returns>Whether node is already published</returns>
    private bool Publish(TreeNode node, WorkflowManager wm)
    {
        string pathCulture = HTMLHelper.HTMLEncode(node.NodeAliasPath + " (" + node.DocumentCulture + ")");
        WorkflowStepInfo currentStep = wm.GetStepInfo(node);
        bool alreadyPublished = (currentStep == null) || currentStep.StepIsPublished;

        if (!alreadyPublished)
        {
            using (new CMSActionContext { LogEvents = false })
            {
                // Remove possible checkout
                if (node.DocumentCheckedOutByUserID > 0)
                {
                    TreeProvider.ClearCheckoutInformation(node);
                    node.Update();
                }
            }

            // Publish document
            currentStep = wm.PublishDocument(node);
        }

        // Document is already published, check if still under workflow
        if (alreadyPublished && (currentStep != null) && currentStep.StepIsPublished)
        {
            WorkflowScopeInfo wsi = wm.GetNodeWorkflowScope(node);
            if (wsi == null)
            {
                VersionManager vm = VersionManager.GetInstance(node.TreeProvider);
                vm.RemoveWorkflow(node);
            }
        }

        // Document already published
        if (alreadyPublished)
        {
            AddLog(string.Format(GetString("content.publishedalready"), pathCulture));
        }
        else if (currentStep == null || !currentStep.StepIsPublished)
        {
            AddError(string.Format(GetString("content.PublishWasApproved"), pathCulture));
            return true;
        }
        else
        {
            // Add log record
            AddLog(pathCulture);
        }

        return false;
    }


    private void HandlePossibleError()
    {
        if (!string.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        if (!string.IsNullOrEmpty(CurrentInfo))
        {
            ShowConfirmation(CurrentInfo);
        }
        // Clear selection
        docElem.UniGrid.ResetSelection();
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


    #region "Handling async thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        CurrentInfo = CanceledString;
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array();");
        HandlePossibleError();
    }
    

    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        HandlePossibleError();
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        HandlePossibleError();
    }

    #endregion
}
