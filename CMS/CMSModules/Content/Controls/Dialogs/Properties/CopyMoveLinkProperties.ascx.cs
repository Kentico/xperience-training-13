using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.Internal;


public partial class CMSModules_Content_Controls_Dialogs_Properties_CopyMoveLinkProperties : ContentActionsControl
{
    #region "Private variables, constants & enums"

    private const string underlying = "Underlying";

    private string parentAlias = string.Empty;
    private string whereCondition = string.Empty;
    private string canceledString;

    private int targetId;

    private bool multiple;
    private bool sameSite = true;

    private readonly List<int> nodeIds = new List<int>();
    private readonly List<int> targetIds = new List<int>();
    private DataSet documentsToProcess;
    private Hashtable mParameters;

    private SiteInfo targetSite;

    /// <summary>
    /// Possible actions
    /// </summary>
    protected enum Action
    {
        Move = 0,
        Copy = 1,
        Link = 2,
        LinkDoc = 3
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns current dialog action.
    /// </summary>
    private Action CurrentAction
    {
        get
        {
            try
            {
                return (Action)Enum.Parse(typeof(Action), ValidationHelper.GetString(Parameters["output"], "move"), true);
            }
            catch
            {
                return Action.Move;
            }
        }
    }


    /// <summary>
    /// Perform action.
    /// </summary>
    private bool DoAction
    {
        get
        {
            string action = ValidationHelper.GetString(Parameters["action"], null);
            int trgId = ValidationHelper.GetInteger(Parameters["targetid"], 0);

            return (!String.IsNullOrEmpty(action) && (trgId > 0));
        }
    }


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
    /// Indicates if permissions should be copied or preserved.
    /// </summary>
    private static bool CopyPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(SessionHelper.GetValue("CopyMoveDocCopyPermissions"), false);
        }
        set
        {
            SessionHelper.SetValue("CopyMoveDocCopyPermissions", value);
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
    /// Indicates if documents are shown from all levels.
    /// </summary>
    private bool AllLevels
    {
        get
        {
            return Parameters.ContainsKey("alllevels") && ValidationHelper.GetBoolean(Parameters["alllevels"], false);
        }
    }


    /// <summary>
    /// Gets or sets selected class ID.
    /// </summary>
    private int ClassID
    {
        get;
        set;
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


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            return;
        }

        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        // Check if hashtable containing dialog parameters is not empty
        if ((Parameters == null) || (Parameters.Count == 0))
        {
            return;
        }

        // Register CopyMove.js script file
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Content/Controls/Dialogs/Properties/CopyMove.js");

        IsDialogAction = true;
        // Setup tree provider
        TreeProvider.AllowAsyncActions = false;

        if (!RequestHelper.IsCallback())
        {
            ClassID = ValidationHelper.GetInteger(Parameters["classid"], 0);

            // Get whether action is multiple
            multiple = ValidationHelper.GetBoolean(Parameters["multiple"], false);

            // Get the source node
            string nodeIdsString = ValidationHelper.GetString(Parameters["sourcenodeids"], string.Empty);
            ParseIds(nodeIdsString, nodeIds);

            // Get target node id
            targetId = ValidationHelper.GetInteger(Parameters["targetid"], 0);

            using (TreeNode tn = TreeProvider.SelectSingleNode(targetId))
            {
                if ((tn != null) && (tn.NodeSiteID != CurrentSite.SiteID))
                {
                    SiteInfo si = SiteInfo.Provider.Get(tn.NodeSiteID);
                    if (si != null)
                    {
                        targetSite = si;
                    }
                }
                else
                {
                    targetSite = CurrentSite;
                }
            }

            // Set if operation take place on same site
            sameSite = (CurrentSite == targetSite);

            // Set introducing text
            if (targetId == 0)
            {
                switch (CurrentAction)
                {
                    case Action.Move:
                    case Action.Copy:
                    case Action.Link:
                        lblEmpty.Text = GetString("dialogs.copymove.select");
                        break;

                    case Action.LinkDoc:
                        lblEmpty.Text = GetString("dialogs.linkdoc.select");
                        break;
                }
            }

            // Check if target of action is another site
            if (!sameSite)
            {
                plcCopyPermissions.Visible = false;
                plcPreservePermissions.Visible = false;
            }

            if (!RequestHelper.IsPostBack())
            {
                object check;

                // Preset checkbox value
                switch (CurrentAction)
                {
                    case Action.Copy:
                        // Ensure underlying items checkbox
                        check = WindowHelper.GetItem(Action.Copy + underlying);
                        if (check == null)
                        {
                            WindowHelper.Add(Action.Copy + underlying, true);
                        }
                        chkUnderlying.Checked = ValidationHelper.GetBoolean(check, true);
                        if (sameSite)
                        {
                            chkCopyPermissions.Checked = CopyPermissions;
                        }
                        break;

                    case Action.Link:
                    case Action.LinkDoc:
                        if (sameSite)
                        {
                            chkCopyPermissions.Checked = CopyPermissions;
                        }
                        break;

                    case Action.Move:
                        if (sameSite)
                        {
                            chkPreservePermissions.Checked = CopyPermissions;
                        }
                        break;
                }
            }

            string listInfoString = string.Empty;

            // Set up layout and strings depending on selected action
            switch (CurrentAction)
            {
                case Action.Move:
                    listInfoString = "dialogs.move.listinfo";
                    canceledString = "ContentRequest.MoveCanceled";
                    plcUnderlying.Visible = false;
                    plcCopyPermissions.Visible = false;
                    break;

                case Action.Copy:
                    listInfoString = "dialogs.copy.listinfo";
                    canceledString = "ContentRequest.CopyingCanceled";
                    lblUnderlying.ResourceString = "contentrequest.copyunderlying";
                    plcUnderlying.Visible = true;
                    plcPreservePermissions.Visible = false;
                    break;

                case Action.Link:
                case Action.LinkDoc:
                    listInfoString = "dialogs.link.listinfo";
                    canceledString = "ContentRequest.LinkCanceled";
                    plcUnderlying.Visible = false;
                    plcPreservePermissions.Visible = false;
                    break;
            }

            // Localize string
            canceledString = GetString(canceledString);

            // Get alias path of document selected in tree
            string selectedAliasPath = TreePathUtils.GetAliasPathByNodeId(targetId);

            // Set target alias path
            if ((CurrentAction == Action.Copy) || (CurrentAction == Action.Move) || (CurrentAction == Action.Link))
            {
                lblAliasPath.Text = selectedAliasPath;

                // Get source node
                if (nodeIds.Count == 1)
                {
                    TreeNode sourceNode = null;
                    int nodeId = ValidationHelper.GetInteger(nodeIds[0], 0);
                    sourceNode = TreeProvider.SelectSingleNode(nodeId);

                    // Hide checkbox if document has no children
                    if ((sourceNode != null) && !sourceNode.NodeHasChildren)
                    {
                        plcUnderlying.Visible = false;
                    }
                }
            }

            if (CurrentAction == Action.LinkDoc)
            {
                targetIds.AddRange(MultipleSelectionDialogHelper.GetSelectedPageIds());

                string aliasPaths = SessionHelper.GetValue("CopyMoveDocAliasPaths").ToString();

                lblDocToCopyList.Text = aliasPaths;
            }

            // Set visibility of panels
            pnlGeneralTab.Visible = true;
            pnlLog.Visible = false;

            // Get where condition for multiple operation
            whereCondition = ValidationHelper.GetString(Parameters["where"], string.Empty);

            // Get the alias paths of the documents to copy/move/link
            parentAlias = ValidationHelper.GetString(Parameters["parentalias"], string.Empty);

            if (!String.IsNullOrEmpty(parentAlias))
            {
                lblDocToCopy.Text = GetString(listInfoString + "all") + ResHelper.Colon;
                lblDocToCopyList.Text = HTMLHelper.HTMLEncode(parentAlias);
            }
            else
            {
                lblDocToCopy.Text = GetString(listInfoString) + ResHelper.Colon;

                // Get the list of alias paths
                if (!String.IsNullOrEmpty(nodeIdsString))
                {
                    // Set alias paths
                    if ((CurrentAction == Action.Copy) || (CurrentAction == Action.Move) || (CurrentAction == Action.Link))
                    {
                        // Get alias paths from session
                        string aliasPaths = SessionHelper.GetValue("CopyMoveDocAliasPaths").ToString();
                        // As source paths
                        lblDocToCopyList.Text = aliasPaths;
                    }
                    else
                    {
                        int nodeId = ValidationHelper.GetInteger(nodeIds[0], 0);
                        var aliasPaths = TreePathUtils.GetAliasPathByNodeId(nodeId);
                        // As target path
                        lblAliasPath.Text = aliasPaths;
                    }
                }
            }

            if (!RequestHelper.IsPostBack() && DoAction)
            {
                // Perform Move / Copy / Link action
                PerformAction();
            }

            pnlEmpty.Visible = (targetId <= 0);
            pnlGeneralTab.Visible = (targetId > 0);
        }
    }

    #endregion


    #region "Control events"

    protected void chkUnderlying_OnCheckedChanged(object sender, EventArgs e)
    {
        // Store whether to copy/link also underlying documents
        switch (CurrentAction)
        {
            case Action.Copy:
                WindowHelper.Add(Action.Copy + underlying, chkUnderlying.Checked);
                break;
        }
    }


    protected void chkPreservePermissions_OnCheckedChanged(object sender, EventArgs e)
    {
        // Set whether to copy permissions
        CopyPermissions = chkPreservePermissions.Checked;
    }


    protected void chkCopyPermissions_OnCheckedChanged(object sender, EventArgs e)
    {
        // Set whether to copy permissions
        CopyPermissions = chkCopyPermissions.Checked;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Moves document(s).
    /// </summary>
    /// <param name="parameter">Indicates if document permissions should be preserved</param>
    private void Move(object parameter)
    {
        int oldSiteId = 0;
        int newSiteId = 0;
        int parentId = 0;
        int nodeId = 0;
        TreeNode node = null;
        bool preservePermissions = ValidationHelper.GetBoolean(parameter, false);
        ctlAsyncLog.Parameter = null;

        string siteName = CurrentSite.SiteName;

        AddLog(GetString("ContentRequest.StartMove"));

        if (targetId == 0)
        {
            AddError(GetString("ContentRequest.ErrorMissingTarget"));
            return;
        }

        // Check if allow child type
        TreeNode targetNode = TreeProvider.SelectSingleNode(targetId, TreeProvider.ALL_CULTURES);
        if (targetNode == null)
        {
            AddError(GetString("ContentRequest.ErrorMissingTarget"));
            return;
        }

        // No pages are allowed to be created under linked page.
        if (targetNode.IsLink)
        {
            AddError(GetString("contentrequest.errorlinkparent"));
            return;
        }

        try
        {
            PrepareNodeIdsForAllDocuments(siteName);
            if (DataHelper.DataSourceIsEmpty(documentsToProcess))
            {
                // Create where condition
                string where = new WhereCondition().WhereIn("NodeID", nodeIds).ToString(true);
                string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeParentID, DocumentName, NodeAliasPath, NodeLinkedNodeID");
                documentsToProcess = TreeProvider.SelectNodes(siteName, "/%", TreeProvider.ALL_CULTURES, true, null, where, AllLevels ? "NodeLevel DESC, NodeAliasPath" : null, TreeProvider.ALL_LEVELS, false, 0, columns);
            }

            if (!DataHelper.DataSourceIsEmpty(documentsToProcess))
            {
                foreach (DataRow nodeRow in documentsToProcess.Tables[0].Rows)
                {
                    nodeId = ValidationHelper.GetInteger(nodeRow["NodeID"], 0);
                    string className = nodeRow["ClassName"].ToString();
                    string aliasPath = nodeRow["NodeAliasPath"].ToString();
                    string docCulture = nodeRow["DocumentCulture"].ToString();

                    // Get document to move
                    node = DocumentHelper.GetDocument(siteName, aliasPath, docCulture, false, className, null, null, TreeProvider.ALL_LEVELS, false, null, TreeProvider);

                    if (node == null)
                    {
                        AddLog(string.Format(GetString("ContentRequest.DocumentNoLongerExists"), HTMLHelper.HTMLEncode(aliasPath)));
                        continue;
                    }

                    oldSiteId = node.NodeSiteID;
                    parentId = node.NodeParentID;

                    // Move the document
                    MoveNode(node, targetNode, TreeProvider, preservePermissions);
                    newSiteId = node.NodeSiteID;
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            if (!CMSThread.Stopped(ex))
            {
                // Try to get ID of site
                int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
                // Log event to event log
                LogExceptionToEventLog("MOVEDOC", "ContentRequest.MoveFailed", nodeId, ex, siteId);
            }
        }
        catch (Exception ex)
        {
            // Try to get ID of site
            int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
            // Log event to event log
            LogExceptionToEventLog("MOVEDOC", "ContentRequest.MoveFailed", nodeId, ex, siteId);
            HandlePossibleErrors();
        }
        finally
        {
            if (multiple)
            {
                ctlAsyncLog.Parameter = GetRefreshListingScript();
            }
            else
            {
                if (nodeId == 0)
                {
                    // If processed node does not exist
                    HandleNonExistentDocument(siteName);
                }
                else
                {
                    // Set moved document in current site or parent node if copy to other site
                    if (oldSiteId == newSiteId)
                    {
                        // Process result
                        ctlAsyncLog.Parameter = "SelectNode(" + nodeId + ");RefreshTree(" + targetId + ", " + nodeId + ");";
                    }
                    else
                    {
                        ctlAsyncLog.Parameter = "RefreshTree(" + parentId + ", " + parentId + ");SelectNode(" + parentId + ");";
                    }
                }
            }
        }
    }


    /// <summary>
    /// Copies document(s).
    /// </summary>
    private void Copy(object parameter)
    {
        int nodeId = 0;
        int oldSiteId = 0;
        int newSiteId = 0;
        TreeNode node = null;

        // Process Action parameters
        string[] parameters = ValidationHelper.GetString(parameter, "False;False").Split(';');
        if (parameters.Length != 2)
        {
            parameters = "False;False".Split(';');
        }
        bool includeChildNodes = ValidationHelper.GetBoolean(parameters[0], false);
        bool copyPermissions = ValidationHelper.GetBoolean(parameters[1], false);
        ctlAsyncLog.Parameter = null;
        string siteName = CurrentSite.SiteName;

        AddLog(GetString("ContentRequest.StartCopy"));

        if (targetId == 0)
        {
            AddError(GetString("ContentRequest.ErrorMissingTarget"));
            return;
        }
        // Get target document
        TreeNode targetNode = TreeProvider.SelectSingleNode(targetId, TreeProvider.ALL_CULTURES);
        if (targetNode == null)
        {
            AddError(GetString("ContentRequest.ErrorMissingTarget"));
            return;
        }

        // No pages are allowed to be created under linked page.
        if (targetNode.IsLink)
        {
            AddError(GetString("contentrequest.errorlinkparent"));
            return;
        }

        try
        {
            PrepareNodeIdsForAllDocuments(siteName);
            if (DataHelper.DataSourceIsEmpty(documentsToProcess))
            {
                // Create where condition
                string where = new WhereCondition().WhereIn("NodeID", nodeIds).ToString(true);
                string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeAliasPath, ClassName, DocumentCulture");

                documentsToProcess = TreeProvider.SelectNodes(siteName, "/%", TreeProvider.ALL_CULTURES, true, null, where, null, TreeProvider.ALL_LEVELS, false, 0, columns);
            }

            if (!DataHelper.DataSourceIsEmpty(documentsToProcess))
            {
                foreach (DataRow nodeRow in documentsToProcess.Tables[0].Rows)
                {
                    // Get the current document
                    nodeId = ValidationHelper.GetInteger(nodeRow["NodeID"], 0);
                    string className = nodeRow["ClassName"].ToString();
                    string aliasPath = nodeRow["NodeAliasPath"].ToString();
                    string docCulture = nodeRow["DocumentCulture"].ToString();
                    node = DocumentHelper.GetDocument(siteName, aliasPath, docCulture, false, className, null, null, TreeProvider.ALL_LEVELS, false, null, TreeProvider);

                    if (node == null)
                    {
                        AddLog(string.Format(GetString("ContentRequest.DocumentNoLongerExists"), HTMLHelper.HTMLEncode(aliasPath)));
                        continue;
                    }

                    oldSiteId = node.NodeSiteID;

                    // Copy the document
                    TreeNode copiedNode = CopyNode(node, targetNode, includeChildNodes, TreeProvider, copyPermissions);
                    if (copiedNode != null)
                    {
                        node = copiedNode;
                    }
                    newSiteId = node.NodeSiteID;
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            if (!CMSThread.Stopped(ex))
            {
                // Try to get ID of site
                int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
                // Log event to event log
                LogExceptionToEventLog("COPYDOC", "ContentRequest.CopyFailed", nodeId, ex, siteId);
            }
        }
        catch (Exception ex)
        {
            // Try to get ID of site
            int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
            // Log event to event log
            LogExceptionToEventLog("COPYDOC", "ContentRequest.CopyFailed", nodeId, ex, siteId);
            HandlePossibleErrors();
        }
        finally
        {
            if (multiple)
            {
                AddLog(GetString("ContentRequest.CopyOK"));
                ctlAsyncLog.Parameter = GetRefreshListingScript();
            }
            else
            {
                // Set moved document in current site or parent node if copy to other site
                if (oldSiteId == newSiteId)
                {
                    if (nodeId == 0)
                    {
                        // If processed node does not exist
                        HandleNonExistentDocument(siteName);
                    }
                    else
                    {
                        // Process result
                        if (node != null)
                        {
                            nodeId = (multiple ? nodeId : node.NodeID);

                            ctlAsyncLog.Parameter = "SelectNode(" + nodeId + ");RefreshTree(" + targetId + ", " + nodeId + ");";
                        }
                        else
                        {
                            AddError(GetString("ContentRequest.CopyFailed"));
                        }
                    }
                }
                else
                {
                    AddLog(GetString("ContentRequest.CopyOK"));
                    ctlAsyncLog.Parameter = string.Empty;
                }
            }
        }
    }


    private void Link(object parameter)
    {
        bool copyPermissions = ValidationHelper.GetBoolean(parameter, false);
        ctlAsyncLog.Parameter = null;
        Link(targetId, nodeIds, Action.Link, copyPermissions);
    }


    private void LinkDoc(object parameter)
    {
        if (nodeIds.Count > 0)
        {
            ctlAsyncLog.Parameter = null;

            if (!targetIds.Any())
            {
                AddError(GetString("contentrequest.selectpage"));
                return;
            }

            int currentTargetId = ValidationHelper.GetInteger(nodeIds[0], 0);
            bool copyPermissions = ValidationHelper.GetBoolean(parameter, false);

            Link(currentTargetId, targetIds, Action.LinkDoc, copyPermissions);
        }
    }


    /// <summary>
    /// Links selected document(s).
    /// </summary>
    /// <param name="targetNodeId">Target node ID</param>
    /// <param name="sourceNodes">Nodes</param>
    /// <param name="performedAction">Action to be performed</param>
    /// <param name="copyPermissions">Indicates if the document permissions should be copied</param>
    private void Link(int targetNodeId, List<int> sourceNodes, Action performedAction, bool copyPermissions)
    {
        int nodeId = 0;
        int oldSiteId = 0;
        int newSiteId = 0;
        TreeNode node = null;

        string siteName = (performedAction == Action.LinkDoc) ? targetSite.SiteName : CurrentSite.SiteName;

        AddLog(GetString("ContentRequest.StartLink"));

        if (targetNodeId == 0)
        {
            AddError(GetString("ContentRequest.ErrorMissingTarget"));
            return;
        }

        // Check if allow child type
        TreeNode targetNode = TreeProvider.SelectSingleNode(targetNodeId, TreeProvider.ALL_CULTURES);
        if (targetNode == null)
        {
            AddError(GetString("ContentRequest.ErrorMissingTarget"));
            return;
        }

        // No pages are allowed to be created under linked page.
        if (targetNode.IsLink)
        {
            AddError(GetString("contentrequest.errorlinkparent"));
            return;
        }

        try
        {
            PrepareNodeIdsForAllDocuments(siteName);

            if (DataHelper.DataSourceIsEmpty(documentsToProcess))
            {
                // Create where condition
                string where = new WhereCondition().WhereIn("NodeID", sourceNodes).ToString(true);
                string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeParentID, DocumentName, NodeAliasPath, NodeLinkedNodeID");

                documentsToProcess = TreeProvider.SelectNodes(siteName, "/%", TreeProvider.ALL_CULTURES, true, null, where, null, TreeProvider.ALL_LEVELS, false, 0, columns);
            }

            if (!DataHelper.DataSourceIsEmpty(documentsToProcess))
            {
                foreach (DataRow nodeRow in documentsToProcess.Tables[0].Rows)
                {
                    nodeId = ValidationHelper.GetInteger(nodeRow["NodeID"], 0);
                    string className = nodeRow["ClassName"].ToString();
                    string aliasPath = nodeRow["NodeAliasPath"].ToString();
                    string docCulture = nodeRow["DocumentCulture"].ToString();

                    // Get document to link
                    node = DocumentHelper.GetDocument(siteName, aliasPath, docCulture, false, className, null, null, TreeProvider.ALL_LEVELS, false, null, TreeProvider);

                    if (node == null)
                    {
                        AddLog(string.Format(GetString("ContentRequest.DocumentNoLongerExists"), HTMLHelper.HTMLEncode(aliasPath)));
                        continue;
                    }

                    oldSiteId = node.NodeSiteID;

                    // Link the document
                    TreeNode linkedNode = LinkNode(node, targetNode, TreeProvider, copyPermissions);
                    if (linkedNode != null)
                    {
                        node = linkedNode;
                    }
                    newSiteId = node.NodeSiteID;
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            if (!CMSThread.Stopped(ex))
            {
                // Try to get ID of site
                int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
                // Log event to event log
                LogExceptionToEventLog("LINKDOC", "ContentRequest.LinkFailed", nodeId, ex, siteId);
            }
        }
        catch (Exception ex)
        {
            // Try to get ID of site
            int siteId = (node != null) ? node.NodeSiteID : SiteContext.CurrentSiteID;
            // Log event to event log
            LogExceptionToEventLog("LINKDOC", "ContentRequest.LinkFailed", nodeId, ex, siteId);
            HandlePossibleErrors();
        }
        finally
        {
            if (multiple)
            {
                AddLog(GetString("ContentRequest.LinkOK"));
                ctlAsyncLog.Parameter = GetRefreshListingScript();
            }
            else
            {
                if (nodeId == 0)
                {
                    // If processed node does not exist
                    HandleNonExistentDocument(siteName);
                }
                else
                {
                    // Set linked document in current site or parent node if linked to other site
                    if (oldSiteId == newSiteId)
                    {
                        if (node == null)
                        {
                            AddError(GetString("ContentRequest.LinkFailed"));
                        }
                    }
                    else
                    {
                        AddLog(GetString("ContentRequest.LinkOK"));
                    }

                    // Process result
                    if (node != null)
                    {
                        ctlAsyncLog.Parameter = "SelectNode(" + targetNodeId + ");RefreshTree(" + targetNodeId + ", " + targetNodeId + ");";
                    }
                }
            }
        }
    }


    /// <summary>
    /// Parses ids in string format and add them into list.
    /// </summary>
    /// <param name="idsString">Node ids in string</param>
    /// <param name="idsList">Node ids in list</param>
    private void ParseIds(string idsString, IList<int> idsList)
    {
        string[] nodeIdsArr = idsString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string nodeId in nodeIdsArr)
        {
            int id = ValidationHelper.GetInteger(nodeId, 0);
            if (id != 0)
            {
                idsList.Add(id);
            }
        }
    }


    /// <summary>
    /// Performs the Move / Copy / Link action.
    /// </summary>
    public void PerformAction()
    {
        switch (CurrentAction)
        {
            case Action.Move:
                ctlAsyncLog.TitleText = GetString("contentrequest.startmove");
                // Process move
                RunAsync(Move);
                break;

            case Action.Copy:
                ctlAsyncLog.TitleText = GetString("contentrequest.startcopy");
                // Process copy
                RunAsync(Copy);
                break;

            case Action.Link:
                ctlAsyncLog.TitleText = GetString("contentrequest.StartLink");
                // Process link
                RunAsync(Link);
                break;

            case Action.LinkDoc:
                ctlAsyncLog.TitleText = GetString("contentrequest.StartLink");
                // Process link
                RunAsync(LinkDoc);
                break;
        }
    }

    #endregion


    #region "Help methods"

    /// <summary>
    /// Refreshes tree and selects root document.
    /// </summary>
    /// <param name="siteName">Name of site</param>
    private void HandleNonExistentDocument(string siteName)
    {
        AddError(GetString("ContentRequest.DocumentNotExists"));
        TreeNode root = TreeProvider.SelectSingleNode(siteName, "/", TreeProvider.ALL_CULTURES, true, null, false);
        if (root != null)
        {
            ctlAsyncLog.Parameter = "SelectNode(" + root.NodeID + ");RefreshTree(" + root.NodeID + ", " + root.NodeID + ");";
        }
    }


    /// <summary>
    /// When exception occurs, log it to event log and add message to asnyc log.
    /// </summary>
    /// <param name="errorTitle">Error to add to async log</param>
    /// <param name="nodeId">ID of node which caused operation to fail</param>
    /// <param name="ex">Exception to log</param>
    /// <param name="siteId">Site identifier</param>
    /// <param name="eventCode">Code of event</param>
    private void LogExceptionToEventLog(string eventCode, string errorTitle, int nodeId, Exception ex, int siteId)
    {
        LogContext.LogEventToCurrent(EventType.ERROR, "Content", eventCode, EventLogProvider.GetExceptionLogMessage(ex),
                            RequestContext.RawURL, CurrentUser.UserID, CurrentUser.UserName, nodeId, null, RequestContext.UserHostAddress, siteId, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);

        AddError(GetString(errorTitle));
    }


    /// <summary>
    /// Prepares IDs of nodes when action is performed for all documents under specified parent.
    /// </summary>
    private void PrepareNodeIdsForAllDocuments(string siteName)
    {
        if (!string.IsNullOrEmpty(parentAlias))
        {
            var where = new WhereCondition(whereCondition)
                .WhereNotEquals("ClassName", SystemDocumentTypes.Root);
            string columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, "NodeParentID, DocumentName, NodeAliasPath, NodeLinkedNodeID");
            documentsToProcess = TreeProvider.SelectNodes(siteName, parentAlias.TrimEnd('/') + "/%", TreeProvider.ALL_CULTURES, true, DataClassInfoProvider.GetClassName(ClassID), where.ToString(true), AllLevels ? "NodeLevel DESC, NodeAliasPath" : null, AllLevels ? -1 : 1, false, 0, columns);
            nodeIds.Clear();
            if (!DataHelper.DataSourceIsEmpty(documentsToProcess))
            {
                foreach (DataRow row in documentsToProcess.Tables[0].Rows)
                {
                    nodeIds.Add(ValidationHelper.GetInteger(row["NodeID"], 0));
                }
            }
        }
    }

    #endregion


    #region "Handling async thread"

    /// <summary>
    /// Runs async thread.
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        pnlLog.Visible = true;
        pnlGeneralTab.Visible = false;

        CurrentError = string.Empty;

        bool copyPerm = CopyPermissions && sameSite;

        AddScript("InitializeLog();");

        switch (CurrentAction)
        {
            case Action.Copy:
                ctlAsyncLog.Parameter = String.Format("{0};{1}", ValidationHelper.GetBoolean(WindowHelper.GetItem(Action.Copy + underlying), false), copyPerm);
                break;

            case Action.Link:
            case Action.LinkDoc:
                ctlAsyncLog.Parameter = String.Format("{0}", copyPerm);
                break;

            case Action.Move:
                ctlAsyncLog.Parameter = copyPerm.ToString();
                break;
        }

        ctlAsyncLog.EnsureLog();
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected override void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected override void AddError(string error)
    {
        AddLog(error);
        string separator = multiple ? "<br />" : "\n";
        CurrentError = (error + separator + CurrentError);
    }


    /// <summary>
    /// Ensures any error or info is displayed to user.
    /// </summary>
    /// <returns>True if error occurred.</returns>
    protected bool HandlePossibleErrors()
    {
        if (!string.IsNullOrEmpty(CurrentError))
        {
            if (multiple)
            {
                ShowError(CurrentError);
            }
            else
            {
                AddAlert(CurrentError);
            }

            return true;
        }

        return false;
    }


    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        // Perform actions necessary to cancel running action
        pnlLog.Visible = false;
        pnlGeneralTab.Visible = true;

        AddScript("var __pendingCallbacks = new Array();");
        DestroyLog();
        RefreshDialogTree();
        string refreshScript = ValidationHelper.GetString(ctlAsyncLog.Parameter, string.Empty);
        if (!string.IsNullOrEmpty(refreshScript))
        {
            AddScript(refreshScript);
        }
        AddLog(canceledString);
        ShowConfirmation(canceledString);
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        // Handle error
        pnlLog.Visible = false;
        pnlGeneralTab.Visible = true;
        DestroyLog();
        RefreshContentTree();
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        DestroyLog();
        ClearSelection();
        if (HandlePossibleErrors())
        {
            // If error occurred
            if (multiple)
            {
                RefreshListing();
            }
            else
            {
                RefreshContentTree();
            }
        }
        else
        {
            string refreshScript = ValidationHelper.GetString(ctlAsyncLog.Parameter, string.Empty);
            if (!string.IsNullOrEmpty(refreshScript))
            {
                AddScript(refreshScript);
            }
            CloseDialog();
        }
    }

    #endregion


    #region "JavaScripts"

    /// <summary>
    /// Adds the alert message to the output request window.
    /// </summary>
    /// <param name="message">Message to display</param>
    private void AddAlert(string message)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), message.GetHashCode().ToString(), ScriptHelper.GetAlertScript(message));
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public void AddScript(string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Adds script to refresh dialog tree.
    /// </summary>
    private void RefreshDialogTree()
    {
        StringBuilder refTree = new StringBuilder();
        refTree.Append(@"
if(parent != null)
{
    if(parent.SetAction != null)
    {
        parent.SetAction('refreshtree', '", targetId, @"');
    }
    if(parent.RaiseHiddenPostBack != null)
    {
        parent.RaiseHiddenPostBack();
    }
}
        ");
        AddScript(refTree.ToString());
    }


    /// <summary>
    /// Refreshes listing page.
    /// </summary>
    private void RefreshListing()
    {
        AddScript(GetRefreshListingScript());
    }


    /// <summary>
    /// Returns script for refreshing listing page.
    /// </summary>
    private string GetRefreshListingScript()
    {
        return "RefreshListing();";
    }


    /// <summary>
    /// Adds script to destroy log.
    /// </summary>
    private void DestroyLog()
    {
        AddScript("DestroyLog();");
    }


    /// <summary>
    /// Adds script to refresh content tree.
    /// </summary>
    private void RefreshContentTree()
    {
        AddScript("TreeRefresh();");
    }


    /// <summary>
    /// Adds script to clear selection of listing.
    /// </summary>
    private void ClearSelection()
    {
        AddScript("ClearSelection();");
    }


    /// <summary>
    /// Adds script to close dialog.
    /// </summary>
    private void CloseDialog()
    {
        AddScript("CloseDialog();");
    }

    #endregion
}
