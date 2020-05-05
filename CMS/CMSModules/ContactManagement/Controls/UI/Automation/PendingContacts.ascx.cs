using System;

using CMS.Automation;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_ContactManagement_Controls_UI_Automation_PendingContacts : CMSAdminEditControl
{
    private bool mCanRemoveAutomationProcesses;

    #region "Public properties"

    /// <summary>
    /// Indicates if control is used as widget.
    /// </summary>
    public bool IsWidget
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets default page size of list control.
    /// </summary>
    public int PageSize
    {
        get;
        set;
    }


    /// <summary>
    /// If true, only pending contacts for the current user are shown.
    /// Current user has to be set as the owner of the contacts.
    /// </summary>
    public bool ShowOnlyMyPendingContacts
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
            listElem.OnBeforeDataReload += listElem_OnBeforeDataReload;
        }
        else
        {
            listElem.StopProcessing = true;
            listElem.Visible = false;
        }
    }


    protected void listElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                var stateID = ValidationHelper.GetInteger(actionArgument, 0);
                if (stateID > 0)
                {
                    if (mCanRemoveAutomationProcesses)
                    {
                        var stateInfo = AutomationStateInfoProvider.GetAutomationStateInfo(stateID);
                        AutomationStateInfoProvider.DeleteAutomationStateInfo(stateInfo);
                    }
                }
                break;
        }
    }


    /// <summary>
    /// Data source needs to be set up on before data reload as where clause is finally available.
    /// </summary>
    private void listElem_OnBeforeDataReload()
    {
        listElem.DataSource = GetPendingContacts(CurrentUser, listElem.WhereClause).TypedResult;
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    private void SetupControl()
    {
        mCanRemoveAutomationProcesses = WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteContext.CurrentSiteName);

        if (!RequestHelper.IsPostBack() && (PageSize > 0))
        {
            listElem.Pager.DefaultPageSize = PageSize;
        }

        listElem.ZeroRowsText = GetString("ma.pendingcontacts.nowaitingcontacts");
        listElem.EditActionUrl = "Process_Detail.aspx?stateid={0}";
        listElem.RememberStateByParam = String.Empty;
        listElem.OnExternalDataBound += listElem_OnExternalDataBound;

        // Register scripts for contact details dialog
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewContactDetails", ScriptHelper.GetScript(
            "function Refresh() { \n " +
            "window.location.href = window.location.href;\n" +
            "}"));

        // If widget register action for view process in dialog
        if (IsWidget)
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewPendingContactProcess", ScriptHelper.GetScript(
            "function viewPendingContactProcess(stateId) {" +
            "    modalDialog('" + UrlResolver.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/PendingContacts/Process_Detail.aspx") + "?dialog=1&stateId=' + stateId, 'ViewPendingContactProcess', '1024', '800');" +
            "}"));
        }
    }


    /// <summary>
    /// Returns object query of automation states for my pending contacts which can be used as datasource for unigrid.
    /// </summary>
    /// <param name="user">User for whom pending contacts are shown</param>
    /// <param name="contactsWhereCondition">Where condition for filtering contacts</param>
    private ObjectQuery<AutomationStateInfo> GetPendingContacts(UserInfo user, string contactsWhereCondition)
    {
        // Get complete where condition for pending steps
        var condition = WorkflowStepInfoProvider.GetAutomationPendingStepsWhereCondition(user, SiteContext.CurrentSiteID);

        // Get automation steps specified by condition with permission control
        var automationWorkflowSteps = WorkflowStepInfoProvider.GetWorkflowSteps()
                                                              .Where(condition)
                                                              .Column("StepID")
                                                              .WhereEquals("StepWorkflowType", (int)WorkflowTypeEnum.Automation);

        // Get all pending contacts from automation state where status is Pending and current user is the owner
        var allPendingContacts = AutomationStateInfoProvider.GetAutomationStates()
                                                            .WhereIn("StateStepID", automationWorkflowSteps)
                                                            .WhereEquals("StateStatus", (int)ProcessStatusEnum.Pending)
                                                            .WhereEquals("StateObjectType", ContactInfo.OBJECT_TYPE);

        var contactIDs = ContactInfo.Provider.Get()
                                            .Column("ContactID")
                                            .Where(contactsWhereCondition);
        if (ShowOnlyMyPendingContacts)
        {
            contactIDs.WhereEquals("ContactOwnerUserID", user.UserID);
        }

        return allPendingContacts.WhereIn("StateObjectID", contactIDs.AsMaterializedList("ContactID"));
    }


    protected object listElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;
        switch (sourceName.ToLowerCSafe())
        {
            // Set visibility for edit button
            case "edit":
                if (IsWidget)
                {
                    btn = sender as CMSGridActionButton;
                    if (btn != null)
                    {
                        btn.Visible = false;
                    }
                }
                break;

            // Set visibility for dialog edit button
            case "dialogedit":
                btn = sender as CMSGridActionButton;
                if (btn != null)
                {
                    btn.Visible = IsWidget;
                }
                break;

            case "view":
                btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", objectID);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;

            // Delete action
            case "delete":
               
                btn = (CMSGridActionButton)sender;
                btn.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(String.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(ContactInfo.OBJECT_TYPE).ToLowerCSafe()))) + ")) { return false; }" + btn.OnClientClick;
                if (!mCanRemoveAutomationProcesses)
                {
                    btn.Enabled = false;
                }
                break;
        }

        return null;
    }

    #endregion
}
