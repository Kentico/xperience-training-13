using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_ContactManagement_Controls_UI_ContactGroup_Contacts : CMSAdminListControl, ICallbackEventHandler
{
    #region "Variables & constants"

    private Hashtable mParameters;
    private ContactGroupInfo cgi;
    private bool mAuthorizedToModifyContactGroups;

    private const string START_PROCESS_CONFIRMED = "StartProcessConfirmed";


    /// <summary>
    /// Available actions in mass action selector.
    /// </summary>
    protected enum Action
    {
        SelectAction = 0,
        Remove = 1,
        ChangeStatus = 2,
        StartNewProcess = 3
    }


    /// <summary>
    /// Selected objects in mass action selector.
    /// </summary>
    protected enum What
    {
        Selected = 0,
        All = 1
    }


    /// <summary>
    /// URL of modal dialog for contact status selection.
    /// </summary>
    protected const string CONTACT_STATUS_DIALOG = "~/CMSModules/ContactManagement/FormControls/ContactStatusDialog.aspx";


    /// <summary>
    /// URL of selection dialog.
    /// </summary>
    public const string SELECTION_DIALOG = "~/CMSAdminControls/UI/UniSelector/SelectionDialog.aspx";

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder => plcMess;


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid => gridElem;


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
            gridElem.StopProcessing = value;
            contactSelector.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if  filter is used on live site or in UI.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            contactSelector.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the callback argument.
    /// </summary>
    private string CallbackArgument
    {
        get;
        set;
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    private Guid Identifier
    {
        get
        {
            if (!Guid.TryParse(hdnIdentifier.Value, out var identifier))
            {
                identifier = Guid.NewGuid();
                hdnIdentifier.Value = identifier.ToString();
            }

            return identifier;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Get edited object (contact group)
        if (UIContext.EditedObject != null)
        {
            cgi = (ContactGroupInfo)UIContext.EditedObject;

            // Check permissions
            AuthorizationHelper.AuthorizedReadContact(true);
            mAuthorizedToModifyContactGroups = AuthorizationHelper.AuthorizedModifyContact(false);

            // Setup unigrid
            string where = "(ContactGroupMemberContactGroupID = " + cgi.ContactGroupID + ")";
            gridElem.WhereCondition = where;
            gridElem.OnAction += gridElem_OnAction;
            gridElem.ZeroRowsText = GetString("om.contact.nocontacts");
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

            if (!string.IsNullOrEmpty(cgi.ContactGroupDynamicCondition))
            {
                // Set specific confirmation to remove grid action
                var removeAction = (CMS.UIControls.UniGridConfig.Action)gridElem.GridActions.Actions[1];
                removeAction.Confirmation = "$om.contactgroupmember.confirmremove$";
            }

            // Initialize dropdown lists
            if (!RequestHelper.IsPostBack())
            {
                drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("om.contact." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("om.contact." + What.All), Convert.ToInt32(What.All).ToString()));

                if (mAuthorizedToModifyContactGroups)
                {
                    drpAction.Items.Add(new ListItem(GetString("general.remove"), Convert.ToInt32(Action.Remove).ToString()));
                }

                if (AuthorizationHelper.AuthorizedModifyContact(false) && IsFullContactManagementAvailable())
                {
                    drpAction.Items.Add(new ListItem(GetString("om.account." + Action.ChangeStatus), Convert.ToInt32(Action.ChangeStatus).ToString()));
                }

                if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ONLINEMARKETING, "StartProcess")
                    && IsFullContactManagementAvailable())
                {
                    drpAction.Items.Add(new ListItem(GetString("ma.automationprocess.select"), Convert.ToInt32(Action.StartNewProcess).ToString()));
                }
            }
            else
            {
                if (ControlsHelper.CausedPostBack(btnOk))
                {
                    // Set delayed reload for unigrid if mass action is performed
                    gridElem.DelayedReload = true;
                }
            }

            // Initialize contact selector
            contactSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            contactSelector.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;

            // Register JS scripts
            RegisterScripts();
        }
        else
        {
            StopProcessing = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide mass actions if there are no contacts shown or no mass action is allowed (first action is always there, so comparison with 1 has to be made)
        pnlFooter.Visible = !gridElem.IsEmpty && (drpAction.Items.Count > 1);
        HideUiPartsNotRelevantInLowerEditions();
    }

    #endregion


    #region "Events"

    /// <summary>
    /// UniGrid external databound.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;

        switch (sourceName.ToLowerInvariant())
        {
            // Display delete button
            case "remove":
                btn = sender as CMSGridActionButton;
                if (btn != null)
                {
                    btn.Enabled = mAuthorizedToModifyContactGroups;
                }
                break;

            case "edit":
                btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", objectID);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;
        }

        return null;
    }


    /// <summary>
    /// Unigrid button clicked.
    /// </summary>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        // Perform 'remove' action
        if (actionName == "remove")
        {
            // Delete the object
            int contactId = ValidationHelper.GetInteger(actionArgument, 0);
            ContactInfo contact = ContactInfo.Provider.Get(contactId);
            if (contact != null)
            {
                CheckModifyPermissions();

                // Get the relationship object
                ContactGroupMemberInfo mi = ContactGroupMemberInfo.Provider.Get(cgi.ContactGroupID, contactId, ContactGroupMemberTypeEnum.Contact);
                if (mi != null)
                {
                    ContactGroupMemberInfo.Provider.Delete(mi);
                }
            }
        }
    }


    /// <summary>
    /// Items changed event handler.
    /// </summary>
    protected void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        CheckModifyPermissions();

        // Get new items from selector
        string newValues = ValidationHelper.GetString(contactSelector.Value, null);
        string[] newItems = newValues.Split(new[]
        {
            ';'
        }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string item in newItems)
        {
            // Check if relation already exists
            int itemId = ValidationHelper.GetInteger(item, 0);
            ContactGroupMemberInfo cgmi = ContactGroupMemberInfo.Provider.Get(cgi.ContactGroupID, itemId, ContactGroupMemberTypeEnum.Contact);
            if (cgmi == null)
            {
                ContactGroupMemberInfo.Provider.Add(cgi.ContactGroupID, itemId,
                    ContactGroupMemberTypeEnum.Contact,
                    MemberAddedHowEnum.Manual);
            }
            else if (!cgmi.ContactGroupMemberFromManual)
            {
                cgmi.ContactGroupMemberFromManual = true;
                ContactGroupMemberInfo.Provider.Set(cgmi);
            }
        }

        gridElem.ReloadData();
        pnlUpdate.Update();
        contactSelector.Value = null;
    }


    /// <summary>
    /// Checks permissions for specified action and selected contacts.
    /// </summary>
    /// <param name="action">Type of action</param>
    private void CheckActionPermissions(Action action)
    {
        if (action == Action.ChangeStatus)
        {
            CheckModifyContactPermission();
        }
        else
        {
            CheckModifyPermissions();
        }
    }


    /// <summary>
    /// Checks modify contact permission for current contact group.
    /// If global, current site permission is also checked.
    /// </summary>
    private void CheckModifyContactPermission()
    {
        AuthorizationHelper.AuthorizedModifyContact(true);
    }


    /// <summary>
    /// Checks modify permission for contact group.
    /// </summary>
    private void CheckModifyPermissions()
    {
        if (!CheckPermissions("cms.contactmanagement", "Modify"))
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Modify");
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedValue, 0);
        Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedValue, 0);

        // Check permissions for specified action
        CheckActionPermissions(action);

        // Set constraint for contact relations only
        var where = new WhereCondition()
            .WhereEquals("ContactGroupMemberType", 0)
            .WhereEquals("ContactGroupMemberContactGroupID", cgi.ContactGroupID);

        switch (what)
        {
            case What.All:
                var contactIds = ContactInfo.Provider.Get()
                    .Where(gridElem.WhereClause)
                    .AsIDQuery();
                where.WhereIn("ContactGroupMemberRelatedID", contactIds);
                break;

            case What.Selected:
                where.WhereIn("ContactGroupMemberRelatedID", gridElem.SelectedItems);
                break;
        }

        switch (action)
        {
            case Action.Remove:
                RemoveContacts(what, where);
                break;

            case Action.ChangeStatus:
                ChangeStatus(what);
                break;

            case Action.StartNewProcess:
            {
                var eventArgument = Request.Params.Get("__EVENTARGUMENT");

                if (eventArgument != START_PROCESS_CONFIRMED)
                {
                    ShowStartProcessConfirmation(what);
                    return;
                }

                StartNewProcess(what, where);
                break;
            }

            default:
                return;
        }

        // Reload unigrid
        gridElem.ResetSelection();
        gridElem.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "Methods"

    private void ShowStartProcessConfirmation(What what)
    {
        var confirmationMessage = String.Empty;
        var workflowId = ValidationHelper.GetInteger(hdnIdentifier.Value, 0);
        var selectedAutomationProcess = WorkflowInfo.Provider.Get(workflowId);

        if(selectedAutomationProcess == null || !selectedAutomationProcess.IsAutomation){
            ShowError(ResHelper.GetString("ma.action.initiateprocess.noprocess"));
            return;
        }

        switch (what) {
            case What.All:
            {
                confirmationMessage = ResHelper.GetStringFormat("om.contactgroupmember.confirmstartselectedprocessforall", selectedAutomationProcess.WorkflowDisplayName); 
                break;
            }
            case What.Selected:
            {
                confirmationMessage = ResHelper.GetStringFormat("om.contactgroupmember.confirmstartselectedprocess", selectedAutomationProcess.WorkflowDisplayName);
                break;
            }
        }
        
        var script = $@"if(confirm({ScriptHelper.GetString(confirmationMessage)})){{
{ControlsHelper.GetPostBackEventReference(btnOk, START_PROCESS_CONFIRMED)};
}} else {{
Refresh();
}}";

        ScriptHelper.RegisterStartupScript(btnOk, typeof(string), $"StartProcessConfirmation{Guid.NewGuid()}", script, true);
    }


    private void HideUiPartsNotRelevantInLowerEditions()
    {
        var isFullContactManagementAvailable = IsFullContactManagementAvailable();
        gridElem.NamedColumns["ContactStatusID"].Visible = isFullContactManagementAvailable;
        gridElem.NamedColumns["ContactCountryID"].Visible = isFullContactManagementAvailable;
        if (!isFullContactManagementAvailable)
        {
            gridElem.FilterForm.FieldsToHide.Add("ContactStatusID");
            gridElem.FilterForm.FieldsToHide.Add("ContactCountryID");
        }
    }


    private static bool IsFullContactManagementAvailable()
    {
        return LicenseKeyInfoProvider.IsFeatureAvailable(RequestContext.CurrentDomain, FeatureEnum.FullContactManagement);
    }


    private void RemoveContacts(What what, IWhereCondition where)
    {
        ContactGroupMemberInfoProvider.DeleteContactGroupMembers(where.ToString(true), cgi.ContactGroupID, false, false);

        switch (what)
        {
            case What.All:
                ShowConfirmation(GetString("om.contact.massaction.removedall"));
                break;

            case What.Selected:
                ShowConfirmation(GetString("om.contact.massaction.removed"));
                break;
        }
    }


    private void ChangeStatus(What what)
    {
        int statusId = ValidationHelper.GetInteger(hdnIdentifier.Value, -1);
        string where = null;

        switch (what)
        {
            case What.All:
                {
                    where = SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause);
                    where = "ContactID IN (SELECT ContactGroupMemberRelatedID FROM OM_ContactGroupMember WHERE " + where + ")";
                }
                break;

            case What.Selected:
                where = SqlHelper.GetWhereCondition<int>("ContactID", gridElem.SelectedItems, false);
                break;
        }

        ContactInfoProvider.UpdateContactStatus(statusId, where);

        ShowConfirmation(GetString("om.contact.massaction.statuschanged"));
    }


    private void StartNewProcess(What what, IWhereCondition where)
    {
        try
        {
            string error = String.Empty;
            int processId = ValidationHelper.GetInteger(hdnIdentifier.Value, 0);

            switch (what)
            {
                case What.All:
                    // Get selected IDs based on where condition
                    var contactIdsQuery = ContactGroupMemberInfo.Provider.Get().Where(where).Column("ContactGroupMemberRelatedID");
                    var contactsQuery = ContactInfo.Provider.Get().WhereIn("ContactId", contactIdsQuery);
                    error = ExecuteProcess(processId, contactsQuery);
                    break;

                case What.Selected:
                    var contactIds = gridElem.SelectedItems;
                    var query = ContactInfo.Provider.Get().WhereIn("ContactId", contactIds);
                    error = ExecuteProcess(processId, query);
                    break;
            }

            if (String.IsNullOrEmpty(error))
            {
                string confirmation = GetString(what == What.All ? "ma.process.started" : "ma.process.startedselected");
                ShowConfirmation(confirmation);
            }
            else
            {
                ShowError(GetString("ma.process.error"), error);
            }
        }
        catch (Exception ex)
        {
            LogAndShowError("Automation", "STARTPROCESS", ex);
        }
    }


    private string ExecuteProcess(int processId, ObjectQuery<ContactInfo> query)
    {
        AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
        string error = string.Empty;

        using (new CMSActionContext() { AllowAsyncActions = false })
        {
            query.ForEachPage(contacts =>
            {
                foreach (var contact in contacts)
                {
                    try
                    {
                        manager.StartProcess(contact, processId);
                    }
                    catch (ProcessRecurrenceException ex)
                    {
                        error += "<div>" + ex.Message + "</div>";
                    }
                }
            }, 10000);
        }

        return error;
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs for contact editing
        script.Append(@"
function SelectStatus(queryParameters){
    modalDialog('" + ResolveUrl(CONTACT_STATUS_DIALOG) + @"' + queryParameters, 'selectStatus', '660px', '590px');
}
function StartNewProcess(queryParameters) {
    modalDialog('", ResolveUrl(SELECTION_DIALOG), @"' + queryParameters, 'selectProcess', '750px', '630px');
}
function Refresh() {
    __doPostBack('", pnlUpdate.ClientID, @"', '');
}
function SelectValue_" + ClientID + @"(valueID) {
    document.getElementById('" + hdnIdentifier.ClientID + "').value = valueID;"
    + ControlsHelper.GetPostBackEventReference(btnOk) + @";
}
function US_SelectItems_(valueID) {
    SelectValue_" + ClientID + @"(valueID);
    ShowUpdatePanel();
}
function ShowUpdatePanel(){
     $cmsj('#" + loading.ClientID + @"').css('display', 'inline');
}
function PerformAction(selectionFunction, selectionField, actionId, actionLabel, whatId) {
    var confirmed = true;
    var label = document.getElementById(actionLabel);
    var action = document.getElementById(actionId).value;
    var whatDrp = document.getElementById(whatId).value;
    var selectionFieldElem = document.getElementById(selectionField);
    label.innerHTML = '';
    if (action == '", (int)Action.SelectAction, @"') {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("MassAction.SelectSomeAction"), @"
    }
    else if (eval(selectionFunction) && (whatDrp == '", (int)What.Selected, @"')) {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("om.contact.massaction.select"), @";
    }
    else {
        var param = 'massaction;' + whatDrp;
        if (whatDrp == '", (int)What.Selected, @"') {
            param = param + '#' + selectionFieldElem.value;
        }
        switch(action) {
            case '", (int)Action.Remove, @"':
                if (whatDrp == ", (int)What.Selected, @") {
                    return confirm(", (!string.IsNullOrEmpty(cgi.ContactGroupDynamicCondition)) ? ScriptHelper.GetLocalizedString("om.contactgroupmember.confirmremove") : ScriptHelper.GetLocalizedString("General.ConfirmRemove"), @");
                }
                else {
                    return confirm(", (!string.IsNullOrEmpty(cgi.ContactGroupDynamicCondition)) ? ScriptHelper.GetLocalizedString("om.contactgroupmember.confirmremoveall") : ScriptHelper.GetLocalizedString("General.ConfirmRemoveAll"), @");
                }
                break;
            case '", (int)Action.ChangeStatus, @"':
                dialogParams_", ClientID, " = param + ';", (int)Action.ChangeStatus, "';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectStatus", null), @";
                break;
            case '", (int)Action.StartNewProcess, @"':
                dialogParams_", ClientID, " = param + ';", (int)Action.StartNewProcess, "';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "StartNewProcess", null), @";
                break;
            default:
                break;
        }
    }
    return false;
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MassActions", ScriptHelper.GetScript(script.ToString()));

        btnOk.OnClientClick = "return PerformAction('" + gridElem.GetCheckSelectionScript() + "','" + gridElem.GetSelectionFieldClientID() + "','" + drpAction.ClientID + "','" + lblInfo.ClientID + "','" + drpWhat.ClientID + "');";
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        string queryString = null;

        if (!string.IsNullOrEmpty(CallbackArgument))
        {
            // Prepare parameters...
            mParameters = new Hashtable();

            // ...for mass action
            if (CallbackArgument.StartsWith("massaction;", StringComparison.OrdinalIgnoreCase))
            {
                // Get values of callback argument
                string[] selection = CallbackArgument.Split(new[]
                {
                    ";"
                }, StringSplitOptions.RemoveEmptyEntries);
                if (selection.Length != 3)
                {
                    return null;
                }

                // Get selected actions from DD-list
                Action action = (Action)ValidationHelper.GetInteger(selection[2], 0);
                switch (action)
                {
                    case Action.ChangeStatus:
                        mParameters["allownone"] = true;
                        mParameters["clientid"] = ClientID;
                        break;

                    case Action.StartNewProcess:
                        mParameters["SelectionMode"] = SelectionModeEnum.SingleButton;
                        mParameters["ObjectType"] = WorkflowInfo.OBJECT_TYPE_AUTOMATION;
                        mParameters["WhereCondition"] = "WorkflowEnabled = 1";
                        mParameters["DialogInfoMessage"] = GetString("ma.automationprocess.startprocessinfo");
                        break;

                    default:
                        return null;
                }
            }
            // ...for unigrid action
            else
            {
                mParameters["where"] = SqlHelper.GetWhereCondition<int>("ContactID", new[]
                {
                    CallbackArgument
                }, false);
            }

            WindowHelper.Add(Identifier.ToString(), mParameters);

            queryString = "?params=" + Identifier;
            queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        }

        return queryString;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        CallbackArgument = eventArgument;
    }

    #endregion
}
