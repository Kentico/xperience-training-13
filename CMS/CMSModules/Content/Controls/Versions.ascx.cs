using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Content_Controls_Versions : CMSUserControl
{
    #region "Variables"

    private WorkflowInfo mWorkflowInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Identifier of edited node.
    /// </summary>
    public int NodeID
    {
        get
        {
            return versionsElem.NodeID;
        }
    }


    /// <summary>
    /// Currently edited node.
    /// </summary>
    public TreeNode Node
    {
        get
        {
            return versionsElem.Node;
        }
        set
        {
            versionsElem.Node = value;
        }
    }


    /// <summary>
    /// Tree provider.
    /// </summary>
    public TreeProvider Tree
    {
        get
        {
            return versionsElem.TreeProvider;
        }
    }


    /// <summary>
    /// Version manager.
    /// </summary>
    public VersionManager VersionManager
    {
        get
        {
            return versionsElem.VersionManager;
        }
    }


    /// <summary>
    /// Workflow manager.
    /// </summary>
    public WorkflowManager WorkflowManager
    {
        get
        {
            return versionsElem.WorkflowManager;
        }
    }


    /// <summary>
    /// Returns workflow step information of current node.
    /// </summary>
    public WorkflowInfo WorkflowInfo
    {
        get
        {
            return mWorkflowInfo ?? (mWorkflowInfo = WorkflowManager.GetNodeWorkflow(Node));
        }
        set
        {
            mWorkflowInfo = value;
        }
    }


    /// <summary>
    /// Returns workflow step information of current node.
    /// </summary>
    public WorkflowStepInfo WorkflowStepInfo
    {
        get
        {
            return versionsElem.WorkflowStepInfo;
        }
    }


    /// <summary>
    /// Indicates if control is enabled
    /// </summary>
    public bool Enabled 
    {
        get
        {
            return pnlVersions.Enabled;
        }
        set
        {
            pnlVersions.Enabled = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);
        CMSPage page = Page as CMSPage;
        if (page != null)
        {
            versionsElem.InfoLabel = page.CurrentMaster.InfoLabel;
            versionsElem.ErrorLabel = page.CurrentMaster.ErrorLabel;
        }
        versionsElem.AfterDestroyHistory += versionsElem_AfterDestroyHistory;
        versionsElem.CombineWithDefaultCulture = false;

        if (Node != null)
        {
            // Check read permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())));
            }

            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        DocumentManager.DocumentInfo = DocumentManager.GetDocumentInfo(true);

        ScriptHelper.RegisterEditScript(Page, false);
    }

    #endregion


    #region "Methods"
    
    private void ShowInfo(string message, bool persistent)
    {
        if (IsLiveSite)
        {
            ShowInformation(message, persistent: persistent);
        }
        else
        {
            DocumentManager.DocumentInfo = message;
        }
    }


    /// <summary>
    /// Reloads the page data.
    /// </summary>
    private void ReloadData()
    {
        // If no workflow set for node, hide the data  
        if (WorkflowInfo == null)
        {
            headCheckOut.ResourceString = "properties.scopenotset";
            DisableForm();
            pnlVersions.Visible = false;
        }
        else
        {
            if (!WorkflowStepInfo.StepIsDefault && !WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve))
            {
                ShowInfo(GetString("EditContent.NotAuthorizedToApprove"), true);
            }
        }

        bool useCheckInCheckOut = false;
        if (WorkflowInfo != null)
        {
            useCheckInCheckOut = WorkflowInfo.UseCheckInCheckOut(SiteContext.CurrentSiteName);
        }

        // Check modify permissions
        if (!versionsElem.CanModify)
        {
            DisableForm();
            plcForm.Visible = false;
            ShowInfo(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())), true);
        }
        else if (useCheckInCheckOut || (versionsElem.CheckedOutByUserID != 0))
        {
            btnCheckout.Visible = false;
            btnCheckout.Enabled = true;
            btnCheckin.Visible = false;
            btnCheckin.Enabled = true;
            btnUndoCheckout.Visible = false;
            btnUndoCheckout.Enabled = true;
            txtComment.Enabled = true;
            txtVersion.Enabled = true;
            lblComment.Enabled = true;
            lblVersion.Enabled = true;

            // Check whether to check out or in
            if (WorkflowInfo == null)
            {
                btnCheckout.Visible = true;
                headCheckOut.ResourceString ="VersionsProperties.CheckOut";
                DisableForm();
            }
            else if (!Node.IsCheckedOut)
            {
                headCheckOut.ResourceString ="VersionsProperties.CheckOut";
                DisableForm();
                btnCheckout.Visible = true;
                // Do not allow checkout for published or archived step in advanced workflow
                btnCheckout.Enabled = (WorkflowInfo.IsBasic || (!WorkflowStepInfo.StepIsPublished && !WorkflowStepInfo.StepIsArchived));
            }
            else
            {
                // If checked out by current user, allow to check-in
                if (versionsElem.CheckedOutByUserID == MembershipContext.AuthenticatedUser.UserID)
                {
                    btnCheckin.Visible = true;
                    btnUndoCheckout.Visible = true;
                }
                else
                {
                    // Else checked out by somebody else
                    btnCheckin.Visible = true;
                    btnCheckout.Visible = false;

                    btnUndoCheckout.Visible = versionsElem.CanCheckIn;
                    btnUndoCheckout.Enabled = versionsElem.CanCheckIn;
                    btnCheckin.Enabled = versionsElem.CanCheckIn;
                    txtComment.Enabled = versionsElem.CanCheckIn;
                    txtVersion.Enabled = versionsElem.CanCheckIn;
                }

                headCheckOut.ResourceString = "VersionsProperties.CheckIn";
            }

            if (!WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve))
            {
                DisableForm();
            }
        }
        else
        {
            plcForm.Visible = false;
        }
    }


    /// <summary>
    /// Disables the editing form.
    /// </summary>
    private void DisableForm()
    {
        txtComment.Enabled = false;
        txtVersion.Enabled = false;

        btnCheckin.Enabled = false;
        btnCheckout.Enabled = false;
        btnUndoCheckout.Enabled = false;
    }


    /// <summary>
    /// Add java script for refresh tree view.
    /// </summary>
    private void AddAfterActionScript()
    {
        if (!IsLiveSite && (Node != null))
        {
            ScriptHelper.RefreshTree(Page, Node.NodeID, Node.NodeParentID);
        }
    }

    #endregion


    #region "Button handling"

    protected void versionsElem_AfterDestroyHistory(object sender, EventArgs e)
    {
        AddAfterActionScript();
        ReloadData();
    }


    protected void btnCheckout_Click(object sender, EventArgs e)
    {
        try
        {
            // Check permissions
            if (!WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve))
            {
                DisableForm();
                return;
            }

            VersionManager.EnsureVersion(Node, Node.IsPublished);

            // Check out the document
            VersionManager.CheckOut(Node);

            // Refresh tree if icon checked out should be displayed
            if (DocumentUIHelper.IsIconUsed(IconType.CheckedOut, SiteContext.CurrentSiteName))
            {
                AddAfterActionScript();
            }

            ReloadData();
            versionsElem.ReloadData();
        }
        catch (WorkflowException)
        {
            ShowError(GetString("EditContent.DocumentCannotCheckOut"));
        }
        catch (Exception ex)
        {
            // Log exception
            Service.Resolve<IEventLogService>().LogException("Content", "CHECKOUT", ex);
            ShowError(ex.Message);
        }
    }


    protected void btnCheckin_Click(object sender, EventArgs e)
    {
        try
        {
            // Check permissions
            if (!WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve) || ((versionsElem.CheckedOutByUserID != MembershipContext.AuthenticatedUser.UserID) && !versionsElem.CanCheckIn))
            {
                DisableForm();
                return;
            }

            // Check in the document
            string version = null;
            if (txtVersion.Text.Trim() != string.Empty)
            {
                version = txtVersion.Text.Trim();
            }
            string comment = null;
            if (txtComment.Text.Trim() != string.Empty)
            {
                comment = txtComment.Text.Trim();
            }

            VersionManager.CheckIn(Node, version, comment);

            txtComment.Text = "";
            txtVersion.Text = "";

            DocumentManager.ClearContentChanged();

            // Refresh tree if icon checked out was displayed
            if (DocumentUIHelper.IsIconUsed(IconType.CheckedOut, SiteContext.CurrentSiteName))
            {
                AddAfterActionScript();
            }

            ReloadData();
            versionsElem.ReloadData();
        }
        catch (WorkflowException)
        {
            ShowError(GetString("EditContent.DocumentCannotCheckIn"));
        }
        catch (Exception ex)
        {
            // Log exception
            Service.Resolve<IEventLogService>().LogException("Content", "CHECKIN", ex);
            ShowError(ex.Message);
        }
    }


    protected void btnUndoCheckout_Click(object sender, EventArgs e)
    {
        try
        {
            // Check permissions
            if (!WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve))
            {
                DisableForm();
                return;
            }

            // Undo check out
            VersionManager.UndoCheckOut(Node);

            txtComment.Text = "";
            txtVersion.Text = "";

            DocumentManager.ClearContentChanged();

            // Refresh tree if icon checked out was displayed
            if (DocumentUIHelper.IsIconUsed(IconType.CheckedOut, SiteContext.CurrentSiteName))
            {
                AddAfterActionScript();
            }

            ReloadData();
            versionsElem.ReloadData();
        }
        catch (WorkflowException)
        {
            ShowError(GetString("EditContent.DocumentCannotUndoCheckOut"));
        }
        catch (Exception ex)
        {
            // Log exception
            Service.Resolve<IEventLogService>().LogException("Content", "UNDOCHECKOUT", ex);
            ShowError(ex.Message);
        }
    }

    #endregion
}
