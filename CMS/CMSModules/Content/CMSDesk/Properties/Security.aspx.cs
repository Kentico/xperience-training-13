using System;
using System.Data;
using System.Security.Principal;
using System.Threading;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_Properties_Security : CMSPropertiesPage
{
    #region "Variables"

    protected SiteInfo currentSite = null;
    protected CurrentUserInfo currentUser = null;

    protected bool inheritsPermissions = false;
    protected string ipAddress = null;
    protected string currentUrl = null;

    protected HeaderAction backHeaderAction = null;

    #endregion


    #region "Properties"

    public override HeaderActions HeaderActions
    {
        get
        {
            return menuElem.HeaderActions;
        }
    }


    /// <summary>
    /// Document name.
    /// </summary>
    protected string DocumentName
    {
        get
        {
            if (Node != null)
            {
                return Node.GetDocumentName();
            }
            return string.Empty;
        }
    }


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
    /// Current Info.
    /// </summary>
    public string CurrentInfo
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

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        DocumentManager.OnCheckPermissions += DocumentManager_OnCheckPermissions;
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;

        DocumentManager.UseDocumentHelper = false;

        securityElem.Node = Node;
        securityElem.StopProcessing = pnlUIPermissionsPart.IsHidden;

        base.OnInit(e);

        ctlAsyncLog.Title.HideTitle = true;

        pnlAccessPart.Visible = !pnlAuth.IsHidden;

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Security"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.Security");
        }

        // Redirect to information page when no UI elements displayed
        if (pnlAuth.IsHidden && pnlUIPermissionsPart.IsHidden)
        {
            RedirectToUINotAvailable();
        }

        // Check changes when user wants to change the inheritance
        if (DocumentManager.RegisterSaveChangesScript)
        {
            lnkInheritance.OnClientClick = "return CheckChanges();";
        }
    }


    protected void DocumentManager_OnCheckPermissions(object sender, SimpleDocumentManagerEventArgs e)
    {
        e.CheckDefault = false;
        e.ErrorMessage = String.Format(GetString("cmsdesk.notauthorizedtoeditdocumentpermissions"), e.Node.NodeAliasPath);
        e.IsValid = CanModifyPermission(false, e.Node, currentUser);
    }


    protected void Page_Load(Object sender, EventArgs e)
    {
        currentSite = SiteContext.CurrentSite;
        currentUser = MembershipContext.AuthenticatedUser;

        ipAddress = RequestContext.UserHostAddress;
        currentUrl = RequestContext.RawURL;

        if (!RequestHelper.IsCallback())
        {
            pnlLog.Visible = false;
            pnlPageContent.Visible = true;

            // Gets the node
            if (Node != null)
            {
                // Check license
                if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, string.Empty) != string.Empty)
                {
                    if (!LicenseKeyInfoProvider.IsFeatureAvailable(RequestContext.CurrentDomain, FeatureEnum.DocumentLevelPermissions))
                    {
                        if (LicenseHelper.IsUnavailableUIHidden())
                        {
                            plcContainer.Visible = false;
                        }
                        else
                        {
                            pnlPermissions.Visible = false;
                            lblLicenseInfo.Visible = true;
                            lblLicenseInfo.Text = GetString("Security.NotAvailableInThisEdition");
                        }
                    }
                }

                // Register scripts
                ScriptHelper.RegisterDialogScript(this);

                // Check if document inherits permissions and display info
                inheritsPermissions = AclInfoProvider.DoesNodeInheritPermissions(Node);
                lblInheritanceInfo.Text = inheritsPermissions ? GetString("Security.InheritsInfo.Inherits") : GetString("Security.InheritsInfo.DoesNotInherit");

                if (!RequestHelper.IsPostBack())
                {
                    SetupAccess();
                }

                // Hide link to the inheritance settings if this is the root node
                if (Node.NodeParentID == 0)
                {
                    plcAuthParent.Visible = false;
                    lnkInheritance.Visible = false;
                }
                else
                {
                    // Add parent caption
                    radParent.Text = GetString("Security.Parent") + " (" + GetInheritedAccessCaption("IsSecuredNode") + ")";
                }
            }
            else
            {
                pnlPageContent.Visible = false;
            }
        }

        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        pnlPageContent.Enabled = !DocumentManager.ProcessingAction;

        InitializeBackButton();

        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.AppendTooltip(iconHelp, GetString("security.access.tooltip"), null);
    }


    private void InitializeBackButton()
    {
        if (backHeaderAction == null)
        {
            backHeaderAction = new HeaderAction
            {
                ButtonStyle = ButtonStyle.Default,
                Text = GetString("general.back"),
                RedirectUrl = currentUrl,
                Visible = false,
            };
            menuElem.AddExtraAction(backHeaderAction);
        }
    }


    /// <summary>
    /// Gets inherited caption for security settings.
    /// </summary>
    /// <param name="columnName">Column name of inherited value</param>
    private string GetInheritedAccessCaption(string columnName)
    {
        // Get culture invariant inherited data
        var inherited = Node.GetInheritedValue(columnName);

        // Check inherited 3rd state value
        int value = ValidationHelper.GetInteger(inherited, -1);
        if (value == 2)
        {
            return GetString("Security.Never");
        }

        // Get inherited boolean value
        return ValidationHelper.GetBoolean(inherited, false) ? GetString("General.Yes") : GetString("General.No");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        CheckModifyPermission(false);

        if (pnlAccessPart.Visible)
        {
            pnlAccessPart.Visible = !pnlAuth.IsHidden;
        }
    }

    #endregion


    #region "Methods"

    private void SetupAccess()
    {
        // Set secured radio buttons
        switch (Node.IsSecuredNode)
        {
            case false:
                radNo.Checked = true;
                break;

            case true:
                radYes.Checked = true;
                break;

            default:
                if (Node.NodeParentID == 0)
                {
                    radNo.Checked = true;
                }
                else
                {
                    radParent.Checked = true;
                }
                break;
        }
    }


    /// <summary>
    /// Checks if current use can modify  the permission.
    /// </summary>
    /// <param name="redirect">If true and can't modify the user is redirected to denied page</param>
    private void CheckModifyPermission(bool redirect)
    {
        CanModifyPermission(redirect, Node, currentUser);
    }


    /// <summary>
    /// Checks if current use can modify  the permission.
    /// </summary>
    /// <param name="redirect">If true and can't modify the user is redirected to denied page</param>
    /// <param name="currentNode">Current node</param>
    /// <param name="user">Current user</param>    
    private bool CanModifyPermission(bool redirect, TreeNode currentNode, UserInfo user)
    {
        bool hasPermission = false;
        if (currentNode != null)
        {
            hasPermission = (user.IsAuthorizedPerDocument(currentNode, NodePermissionsEnum.ModifyPermissions) == AuthorizationResultEnum.Allowed);

            // If hasn't permission and redirect enabled
            if (!hasPermission)
            {
                if (redirect)
                {
                    RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoeditdocumentpermissions"), currentNode.NodeAliasPath));
                }
                else
                {
                    pnlAccessPart.Enabled = false;
                    pnlInheritance.Enabled = false;
                    lnkInheritance.Visible = false;
                }
            }
        }
        return hasPermission;
    }


    /// <summary>
    /// Switches back to default layout (after some action).
    /// </summary>
    private void SwitchBackToPermissionsMode()
    {
        plcContainer.Visible = true;
        pnlAccessPart.Visible = true;
        pnlInheritance.Visible = false;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Displays inheritance settings.
    /// </summary>
    protected void lnkInheritance_Click(Object sender, EventArgs e)
    {
        // Check permission
        CheckModifyPermission(true);

        plcContainer.Visible = false;
        pnlAccessPart.Visible = false;
        pnlInheritance.Visible = true;

        backHeaderAction.Visible = true;
        menuElem.ShowSave = false;

        // Test if current document inherits permissions
        if (inheritsPermissions)
        {
            plcBreakClear.Visible = true;
            plcBreakCopy.Visible = true;
            plcRestore.Visible = false;
        }
        else
        {
            plcBreakClear.Visible = false;
            plcBreakCopy.Visible = false;
            plcRestore.Visible = true;
        }

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    protected void lnkBreakWithCopy_Click(Object sender, EventArgs e)
    {
        // Check permission
        CheckModifyPermission(true);

        // Break permission inheritance and copy parent permissions
        AclInfoProvider.BreakInheritance(Node, true);

        // Log staging task
        TaskParameters taskParam = new TaskParameters();
        taskParam.SetParameter("copyPermissions", true);
        DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.BreakACLInheritance, Node.TreeProvider, SynchronizationInfoProvider.ENABLED_SERVERS, taskParam, Node.TreeProvider.AllowAsyncActions);

        // Insert information about this event to event log.
        if (DocumentManager.Tree.LogEvents)
        {
            var logData = new EventLogData(EventTypeEnum.Information, "Content", "DOCPERMISSIONSMODIFIED")
            {
                EventDescription = ResHelper.GetAPIString("security.documentpermissionsbreakcopy", "Inheritance of the parent page permissions have been broken. Parent page permissions have been copied."),
                EventUrl = currentUrl,
                UserID = DocumentManager.Tree.UserInfo.UserID,
                UserName = DocumentManager.Tree.UserInfo.UserName,
                NodeID = Node.NodeID,
                DocumentName = DocumentName,
                IPAddress = ipAddress,
                SiteID = Node.NodeSiteID
            };

            Service.Resolve<IEventLogService>().LogEvent(logData);
        }

        lblInheritanceInfo.Text = GetString("Security.InheritsInfo.DoesNotInherit");
        SwitchBackToPermissionsMode();

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    protected void lnkBreakWithClear_Click(Object sender, EventArgs e)
    {
        // Check permission
        CheckModifyPermission(true);

        // Break permission inheritance and clear permissions
        AclInfoProvider.BreakInheritance(Node, false);

        // Log staging task and flush cache
        DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.BreakACLInheritance, Node.TreeProvider, SynchronizationInfoProvider.ENABLED_SERVERS, null, Node.TreeProvider.AllowAsyncActions);
        Node.ClearCache();

        // Insert information about this event to event log.
        if (DocumentManager.Tree.LogEvents)
        {
            var logData = new EventLogData(EventTypeEnum.Information, "Content", "DOCPERMISSIONSMODIFIED")
            {
                EventDescription = ResHelper.GetAPIString("security.documentpermissionsbreakclear", "Inheritance of the parent page permissions have been broken."),
                EventUrl = currentUrl,
                UserID = currentUser.UserID,
                UserName = currentUser.UserName,
                NodeID = Node.NodeID,
                DocumentName = DocumentName,
                IPAddress = ipAddress,
                SiteID = Node.NodeSiteID
            };

            Service.Resolve<IEventLogService>().LogEvent(logData);
        }

        lblInheritanceInfo.Text = GetString("Security.InheritsInfo.DoesNotInherit");
        SwitchBackToPermissionsMode();

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    protected void lnkRestoreInheritance_Click(Object sender, EventArgs e)
    {
        ResetNodePermission(currentSite.SiteName, Node.NodeAliasPath, false, currentUser, null);

        lblInheritanceInfo.Text = GetString("Security.InheritsInfo.Inherits");
        SwitchBackToPermissionsMode();

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    protected void lnkRestoreInheritanceRecursively_Click(object sender, EventArgs e)
    {
        // Setup design
        pnlLog.Visible = true;
        pnlPageContent.Visible = false;
        ctlAsyncLog.TitleText = GetString("cmsdesk.restoringpermissioninheritance");

        CurrentError = string.Empty;
        CurrentInfo = string.Empty;

        // Recursively
        ctlAsyncLog.RunAsync(ResetNodePermission, WindowsIdentity.GetCurrent());

        lblInheritanceInfo.Text = GetString("Security.InheritsInfo.Inherits");
        SwitchBackToPermissionsMode();

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    /// <summary>
    /// Async reset node action.
    /// </summary>
    /// <param name="parameter">Accepts CurrentUserInfo</param>
    protected void ResetNodePermission(object parameter)
    {
        // Add information to log
        AddLog(GetString("cmsdesk.restoringpermissioninheritance"));

        TreeProvider tr = new TreeProvider();
        ResetNodePermission(currentSite.SiteName, Node.NodeAliasPath, true, tr.UserInfo, tr);
    }


    /// <summary>
    /// Resets permission inheritance of node and its children.
    /// </summary>
    /// <param name="siteName">Name of site</param>
    /// <param name="nodeAliasPath">Alias path</param>
    /// <param name="recursive">Indicates whether to recursively reset all nodes below the current node</param>
    /// <param name="user">Current user</param>
    /// <param name="tr">Tree provider</param>
    /// <returns>Whether TRUE if no permission conflict has occurred</returns>
    private bool ResetNodePermission(string siteName, string nodeAliasPath, bool recursive, UserInfo user, TreeProvider tr)
    {
        // Check permissions
        bool permissionsResult = false;
        try
        {
            if (tr == null)
            {
                tr = new TreeProvider(user);
            }
            // Get node by alias path
            TreeNode treeNode = tr.SelectSingleNode(siteName, nodeAliasPath, null, true, null, false);
            permissionsResult = CanModifyPermission(!recursive, treeNode, user);

            if (treeNode != null)
            {
                // If user has permissions
                if (permissionsResult)
                {
                    // Break inheritance of a node
                    if (!AclInfoProvider.DoesNodeInheritPermissions(treeNode))
                    {
                        // Restore inheritance of a node
                        AclInfoProvider.RestoreInheritance(treeNode);

                        // Log current encoded alias path
                        AddLog(HTMLHelper.HTMLEncode(nodeAliasPath));

                        // Log staging task and flush cache
                        DocumentSynchronizationHelper.LogDocumentChange(treeNode, TaskTypeEnum.RestoreACLInheritance, treeNode.TreeProvider, SynchronizationInfoProvider.ENABLED_SERVERS, null, treeNode.TreeProvider.AllowAsyncActions);
                        Node.ClearCache();

                        // Insert information about this event to event log.
                        if (DocumentManager.Tree.LogEvents)
                        {
                            if (recursive)
                            {
                                LogContext.LogEventToCurrent(EventType.INFORMATION, "Content", "DOCPERMISSIONSMODIFIED", string.Format(ResHelper.GetAPIString("security.documentpermissionsrestoredfordoc", "Permissions of the page '{0}' have been restored to the parent page permissions."), nodeAliasPath), null, user.UserID, user.UserName, treeNode.NodeID, treeNode.GetDocumentName(), ipAddress, Node.NodeSiteID, null, null, null, DateTime.Now);
                            }
                            else
                            {
                                var logData = new EventLogData(EventTypeEnum.Information, "Content", "DOCPERMISSIONSMODIFIED")
                                {
                                    EventDescription = ResHelper.GetAPIString("security.documentpermissionsrestored", "Permissions have been restored to the parent page permissions."),
                                    EventUrl = currentUrl,
                                    UserID = user.UserID,
                                    UserName = user.UserName,
                                    NodeID = treeNode.NodeID,
                                    DocumentName = treeNode.GetDocumentName(),
                                    IPAddress = ipAddress,
                                    SiteID = Node.NodeSiteID
                                };

                                Service.Resolve<IEventLogService>().LogEvent(logData);
                            }
                        }
                    }
                    else
                    {
                        AddLog(string.Format(GetString("cmsdesk.skippingrestoring"), HTMLHelper.HTMLEncode(nodeAliasPath)));
                    }
                }

                // Recursively reset node inheritance
                if (recursive)
                {
                    // Get child nodes of current node
                    DataSet ds = DocumentManager.Tree.SelectNodes(siteName, treeNode.NodeAliasPath.TrimEnd('/') + "/%", TreeProvider.ALL_CULTURES, true, null, null, null, 1, false, -1, DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS + ",NodeAliasPath");
                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string childNodeAliasPath = ValidationHelper.GetString(dr["NodeAliasPath"], string.Empty);

                            if (!string.IsNullOrEmpty(childNodeAliasPath))
                            {
                                bool tempPermissionsResult = ResetNodePermission(siteName, childNodeAliasPath, true, user, tr);
                                permissionsResult = tempPermissionsResult && permissionsResult;
                            }
                        }
                    }
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            if (CMSThread.Stopped(ex))
            {
                // When canceled
                CurrentInfo = GetString("cmsdesk.restoringcanceled");
                AddLog(CurrentInfo);
            }
            else
            {
                // Log error
                CurrentError = GetString("cmsdesk.restoringfailed") + ": " + ex.Message;
                AddLog(CurrentError);
            }
        }
        catch (Exception ex)
        {
            // Log error
            CurrentError = GetString("cmsdesk.restoringfailed") + ": " + ex.Message;
            AddLog(CurrentError);
        }
        return permissionsResult;
    }


    /// <summary>
    /// OnSaveData event handler. Sets security properties.
    /// </summary>
    protected void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        if (node != null)
        {
            string message = null;
            bool clearCache = false;

            // Authentication
            if (pnlAccessPart.Visible)
            {
                if (!pnlAuth.IsHidden)
                {
                    bool? isSecuredNode = node.IsSecuredNode;

                    if (radYes.Checked)
                    {
                        isSecuredNode = true;
                    }
                    else if (radNo.Checked)
                    {
                        isSecuredNode = false;
                    }
                    else if (radParent.Checked)
                    {
                        isSecuredNode = null;
                    }

                    // Set secured areas settings
                    if (isSecuredNode != node.IsSecuredNode)
                    {
                        node.IsSecuredNode = isSecuredNode;
                        clearCache = true;
                        message += ResHelper.GetAPIString("security.documentaccessauthchanged", "Page authentication settings have been modified.");
                    }
                }
            }

            // Insert information about this event to event log.
            if (DocumentManager.Tree.LogEvents && (message != null))
            {
                var logData = new EventLogData(EventTypeEnum.Information, "Content", "DOCPERMISSIONSMODIFIED")
                {
                    EventDescription = message,
                    EventUrl = currentUrl,
                    UserID = currentUser.UserID,
                    UserName = currentUser.UserName,
                    NodeID = node.NodeID,
                    DocumentName = DocumentName,
                    IPAddress = ipAddress,
                    SiteID = node.NodeSiteID
                };

                Service.Resolve<IEventLogService>().LogEvent(logData);
            }

            // Clear cache if security settings changed
            if (clearCache)
            {
                CacheHelper.ClearFileNodeCache(node.NodeSiteName);
            }

            // Clear ACL settings
            securityElem.InvalidateAcls();
        }
    }

    #endregion


    #region "Async processing"

    protected void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        securityElem.LoadOperators(true);
        securityElem.ErrorLabel.Text = CurrentError;
        securityElem.InfoLabel.Text = CurrentInfo;
    }


    protected void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        securityElem.LoadOperators(true);
        securityElem.ErrorLabel.Text = CurrentError;
        securityElem.InfoLabel.Text = CurrentInfo;
    }


    protected void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        securityElem.LoadOperators(true);
        securityElem.ErrorLabel.Text = CurrentError;
        securityElem.InfoLabel.Text = CurrentInfo;
    }

    #endregion


    #region "Log handling"

    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Adds the log error.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddErrorLog(string newLog)
    {
        AddErrorLog(newLog, null);
    }


    /// <summary>
    /// Adds the log error.
    /// </summary>
    /// <param name="newLog">New log information</param>
    /// <param name="errorMessage">Error message</param>
    protected void AddErrorLog(string newLog, string errorMessage)
    {
        AddLog(newLog);
    }

    #endregion
}
