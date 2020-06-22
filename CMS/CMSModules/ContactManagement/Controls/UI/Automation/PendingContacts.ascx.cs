using System;
using System.Data;

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


public partial class CMSModules_ContactManagement_Controls_UI_Automation_PendingContacts : CMSAdminControl
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


    #region "Control events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            SetupControl();
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
                        var stateInfo = AutomationStateInfo.Provider.Get(stateID);
                        AutomationStateInfo.Provider.Delete(stateInfo);
                    }
                }
                break;
        }
    }


    protected DataSet listElem_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        return GetPendingContacts(CurrentUser, completeWhere, currentOrder, currentTopN, columns, currentOffset, currentPageSize, ref totalRecords);
    }


    protected object listElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;
        switch (sourceName.ToLowerInvariant())
        {
            // Set visibility for edit button
            case "edit":
                btn = (CMSGridActionButton)sender;
                btn.Visible = !IsWidget;
                break;

            // Set visibility for dialog edit button
            case "dialogedit":
                btn = (CMSGridActionButton)sender;
                btn.Visible = IsWidget;
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
                btn.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(String.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(ContactInfo.OBJECT_TYPE).ToLowerInvariant()))) + ")) { return false; }" + btn.OnClientClick;
                if (!mCanRemoveAutomationProcesses)
                {
                    btn.Enabled = false;
                }
                break;
        }

        return null;
    }

    #endregion


    #region "Methods"

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
        listElem.EditActionUrl = "Process_Detail.aspx?stateid={0}&contactid={1}";
        listElem.RememberStateByParam = String.Empty;

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
    /// Returns data of automation states for my pending contacts.
    /// </summary>
    private DataSet GetPendingContacts(UserInfo user, string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        // Get all pending contacts from automation state where status is Pending
        var allPendingContacts = AutomationStateInfo.Provider.Get()
                                                            .WhereEquals("StateStatus", (int)ProcessStatusEnum.Pending)
                                                            .WhereEquals("StateObjectType", ContactInfo.OBJECT_TYPE);
        

        // Get complete where condition for pending steps
        var condition = WorkflowStepInfoProvider.GetAutomationPendingStepsWhereCondition(user, SiteContext.CurrentSiteID);
        if (!String.IsNullOrEmpty(condition?.WhereCondition))
        {
            // Get automation steps specified by condition with permission control
            var automationWorkflowSteps = WorkflowStepInfo.Provider.Get()
                                                                  .Where(condition)
                                                                  .Column("StepID")
                                                                  .WhereEquals("StepWorkflowType", (int)WorkflowTypeEnum.Automation);

            allPendingContacts.WhereIn("StateStepID", automationWorkflowSteps);
        }

        // Get contact IDs based on filtering or ownership
        ObjectQuery<ContactInfo> contactIDs = null;
        if (ShowOnlyMyPendingContacts || !String.IsNullOrEmpty(completeWhere))
        {
            contactIDs = ContactInfo.Provider.Get()
                                            .Column("ContactID")
                                            .Where(completeWhere);
            if (ShowOnlyMyPendingContacts)
            {
                contactIDs.WhereEquals("ContactOwnerUserID", user.UserID);
            }
        }

        var query = allPendingContacts.OrderBy(currentOrder).TopN(currentTopN).Columns(columns);

        if (contactIDs != null)
        {
            // Add restriction based on selected contacts
            query.WhereIn("StateObjectID", SqlInstallationHelper.DatabaseIsSeparated() ? contactIDs.AsMaterializedList("ContactID") : contactIDs);
        }

        query.IncludeBinaryData = false;
        query.Offset = currentOffset;
        query.MaxRecords = currentPageSize;

        var data = query.Result;
        totalRecords = query.TotalRecords;

        return data;
    }

    #endregion
}
