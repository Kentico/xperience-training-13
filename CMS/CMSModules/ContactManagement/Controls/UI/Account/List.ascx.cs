using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_Account_List : CMSAdminListControl, ICallbackEventHandler
{
    #region "Variables"
    protected string mWhereCondition;
    private Hashtable mParameters;
    private bool modifyPermission;
    private CMSModules_ContactManagement_Controls_UI_Account_Filter filter;

    /// <summary>
    /// Available actions in mass action selector.
    /// </summary>
    protected enum Action
    {
        SelectAction = 0,
        AddToGroup = 1,
        ChangeStatus = 2,
        Delete = 3
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
    /// URL of the page for account deletion.
    /// </summary>
    protected const string DELETE_PAGE = "~/CMSModules/ContactManagement/Pages/Tools/Account/Delete.aspx";


    /// <summary>
    /// URL of modal dialog for account status selection.
    /// </summary>
    protected const string ACCOUNT_STATUS_DIALOG = "~/CMSModules/ContactManagement/FormControls/AccountStatusDialog.aspx";


    /// <summary>
    /// URL of modal dialog for contact group selection.
    /// </summary>
    protected const string CONTACT_GROUP_DIALOG = "~/CMSModules/ContactManagement/FormControls/ContactGroupDialog.aspx";

    /// <summary>
    /// URL of modal dialog for contact selection.
    /// </summary>
    protected const string ACCOUNT_SELECT_DIALOG = "~/CMSModules/ContactManagement/FormControls/AccountSelectorDialog.aspx";


    #endregion


    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
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
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Additional WHERE condition to filter data.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
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
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Modal dialog identifier.
    /// </summary>
    private Guid Identifier
    {
        get
        {
            // Try to load data from control View State
            Guid identifier = hdnIdentifier.Value.ToGuid(Guid.NewGuid());
            hdnIdentifier.Value = identifier.ToString();

            return identifier;
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

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        gridElem.OnFilterFieldCreated += gridElem_OnFilterFieldCreated;
        gridElem.LoadGridDefinition();
        base.OnInit(e);
    }


    void gridElem_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        filter = filterDefinition.ValueControl as CMSModules_ContactManagement_Controls_UI_Account_Filter;
        if (filter != null)
        {
            filter.IsLiveSite = IsLiveSite;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        modifyPermission = AuthorizationHelper.AuthorizedModifyContact(false);

        // Edit action URL
        string url = UIContextHelper.GetElementUrl(ModuleName.CONTACTMANAGEMENT, "EditAccount", false);
        url = URLHelper.AddParameterToUrl(url, "objectid", "{0}");

        // Setup UniGrid
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, WhereCondition);
        gridElem.EditActionUrl = url;
        gridElem.ZeroRowsText = GetString("om.account.noaccountsfound");
        gridElem.FilteredZeroRowsText = GetString("om.account.noaccountsfound.filtered");

        // Initialize dropdown lists
        if (!RequestHelper.IsPostBack())
        {
            drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
            if (modifyPermission)
            {
                if (AuthorizationHelper.AuthorizedReadContact(false))
                {
                    drpAction.Items.Add(new ListItem(GetString("om.account." + Action.AddToGroup), Convert.ToInt32(Action.AddToGroup).ToString()));
                }

                drpAction.Items.Add(new ListItem(GetString("general.delete"), Convert.ToInt32(Action.Delete).ToString()));
                drpAction.Items.Add(new ListItem(GetString("om.account." + Action.ChangeStatus), Convert.ToInt32(Action.ChangeStatus).ToString()));
            }
            drpWhat.Items.Add(new ListItem(GetString("om.account." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
            drpWhat.Items.Add(new ListItem(GetString("om.account." + What.All), Convert.ToInt32(What.All).ToString()));
        }
        else
        {
            if (ControlsHelper.CausedPostBack(btnOk))
            {
                // Set delayed reload for UniGrid if mass action is performed
                gridElem.DelayedReload = true;
            }
        }

        // Register JS scripts
        RegisterScripts();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        pnlFooter.Visible = !gridElem.IsEmpty && (drpAction.Items.Count > 1);
    }

    #endregion


    #region "Events"

    /// <summary>
    /// UniGrid data bound.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSAccessibleButton btn;

        DataRowView drv = parameter as DataRowView;
        switch (sourceName.ToLowerCSafe())
        {
            // Display delete button
            case "delete":
                btn = (CMSGridActionButton)sender;
                btn.OnClientClick = string.Format("dialogParams_{0} = '{1}';{2};return false;",
                    ClientID,
                    btn.CommandArgument,
                    Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "Delete", null));

                // Display delete button only for users with appropriate permission
                btn.Enabled = modifyPermission;
                break;

            case "primarycontactname":
                string name = ValidationHelper.GetString(drv["PrimaryContactFullName"], "");
                if (!string.IsNullOrEmpty(name.Trim()))
                {
                    string contactDetailsDialogURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", ValidationHelper.GetInteger(drv["AccountPrimaryContactID"], 0));

                    var placeholder = new PlaceHolder();
                    placeholder.Controls.Add(new Label
                    {
                        Text = HTMLHelper.HTMLEncode(name),
                        CssClass = "contactmanagement-accountlist-primarycontact"
                    });

                    placeholder.Controls.Add(new CMSGridActionButton
                    {
                        IconCssClass = "icon-edit",
                        OnClientClick = ScriptHelper.GetModalDialogScript(contactDetailsDialogURL, "ContactDetail"),
                        ToolTip = GetString("om.contact.viewdetail")
                    });

                    return placeholder;
                }
                return null;
        }
        return null;
    }


    /// <summary>
    /// Mass operation button "OK" click.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Get where condition depending on mass action selection
        string where = null;

        What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedValue, 0);
        switch (what)
        {
            // All items
            case What.All:
                where = SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause);
                break;

            // Selected items
            case What.Selected:
                where = SqlHelper.GetWhereCondition<int>("AccountID", gridElem.SelectedItems, false);
                break;
        }

        Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedItem.Value, 0);
        switch (action)
        {
            // Action 'Change status'
            case Action.ChangeStatus:
                {
                    // Get selected status ID from hidden field
                    int statusId = ValidationHelper.GetInteger(hdnIdentifier.Value, -1);
                    // If status ID is 0, the status will be removed
                    if (statusId >= 0)
                    {
                        AccountInfoProvider.UpdateAccountStatus(statusId, where);
                        ShowConfirmation(GetString("om.account.massaction.statuschanged"));
                    }
                }
                break;

            // Action 'Add to contact group'
            case Action.AddToGroup:
                {
                    // Get contact group ID from hidden field
                    int groupId = ValidationHelper.GetInteger(hdnIdentifier.Value, 0);
                    if (groupId > 0)
                    {
                        IEnumerable<string> accountIds = null;

                        switch (what)
                        {
                            // All items
                            case What.All:
                                // Get selected IDs based on where condition
                                DataSet accounts = AccountInfo.Provider.Get().Where(where).Column("AccountID");
                                if (!DataHelper.DataSourceIsEmpty(accounts))
                                {
                                    // Get array list with IDs
                                    accountIds = DataHelper.GetUniqueValues(accounts.Tables[0], "AccountID", true);
                                }
                                break;
                            // Selected items
                            case What.Selected:
                                // Get selected IDs from UniGrid
                                accountIds = gridElem.SelectedItems;
                                break;
                        }

                        if (accountIds != null)
                        {
                            // Add each selected account to the contact group, skip accounts that are already members of the group
                            foreach (string item in accountIds)
                            {
                                int accountId = ValidationHelper.GetInteger(item, 0);
                                if ((accountId > 0) && (ContactGroupMemberInfo.Provider.Get(groupId, accountId, ContactGroupMemberTypeEnum.Account) == null))
                                {
                                    ContactGroupMemberInfo.Provider.Add(groupId, accountId, ContactGroupMemberTypeEnum.Account, MemberAddedHowEnum.Account);
                                }
                            }
                            // Get contact group to show result message with its display name
                            ContactGroupInfo group = ContactGroupInfo.Provider.Get(groupId);
                            if (group != null)
                            {
                                ShowConfirmation(String.Format(GetString("om.account.massaction.addedtogroup"), ResHelper.LocalizeString(group.ContactGroupDisplayName)));
                            }
                        }
                    }
                }
                break;
            default:
                return;
        }

        // Reload UniGrid
        gridElem.ResetSelection();
        gridElem.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs
        script.Append(@"
function Delete(queryParameters)
{
    document.location.href = '" + ResolveUrl(DELETE_PAGE) + @"' + queryParameters;
}
function SelectStatus(queryParameters)
{
    modalDialog('" + ResolveUrl(ACCOUNT_STATUS_DIALOG) + @"' + queryParameters, 'selectStatus', '660px', '590px');
}
function SelectContactGroup(queryParameters)
{
    modalDialog('" + ResolveUrl(CONTACT_GROUP_DIALOG) + @"' + queryParameters, 'selectGroup', 500, 435);
}
function Refresh()
{
    __doPostBack('" + pnlUpdate.ClientID + @"', '');
}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GridActions", ScriptHelper.GetScript(script.ToString()));

        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "AccountStatus_" + ClientID, ScriptHelper.GetScript("var dialogParams_" + ClientID + " = '';"));

        // Register script for mass actions
        script = new StringBuilder();
        script.Append(@"
function PerformAction(selectionFunction, selectionField, actionId, actionLabel, whatId) {
    var confirmation = null;
    var label = document.getElementById(actionLabel);
    var action = document.getElementById(actionId).value;
    var whatDrp = document.getElementById(whatId);
    var selectionFieldElem = document.getElementById(selectionField);
    label.innerHTML = '';
    if (action == '", (int)Action.SelectAction, @"') {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("MassAction.SelectSomeAction"), @"
    }
    else if (eval(selectionFunction) && (whatDrp.value == '", (int)What.Selected, @"')) {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("om.account.massaction.select"), @";
    }
    else {
        var param = 'massaction;' + whatDrp.value;
        if (whatDrp.value == '", (int)What.Selected, @"') {
            param = param + '#' + selectionFieldElem.value;
        }
        switch(action) {
            case '", (int)Action.Delete, @"':
                dialogParams_", ClientID, @" = param + ';", (int)Action.Delete, @"';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "Delete", null), @";
                break;
            case '", (int)Action.ChangeStatus, @"':
                dialogParams_", ClientID, @" = param + ';", (int)Action.ChangeStatus, @"';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectStatus", null), @";
                break;
            case '", (int)Action.AddToGroup, @"':
                dialogParams_", ClientID, @" = param + ';", (int)Action.AddToGroup, @"';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectContactGroup", null), @";
                break;
            default:
                confirmation = null;
                break;
        }
        if (confirmation != null) {
            return confirm(confirmation)
        }
    }
    return false;
}
function SelectValue_" + ClientID + @"(valueID) {
    document.getElementById('" + hdnIdentifier.ClientID + @"').value = valueID;" + ControlsHelper.GetPostBackEventReference(btnOk, null) + @";
}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MassActions", ScriptHelper.GetScript(script.ToString()));

        // Add action to button
        btnOk.OnClientClick = "return PerformAction('" + gridElem.GetCheckSelectionScript() + "','" + gridElem.GetSelectionFieldClientID() + "','" + drpAction.ClientID + "','" + lblInfo.ClientID + "','" + drpWhat.ClientID + "');";
    }


    /// <summary>
    /// Returns where condition depending on mass action selection.
    /// </summary>
    /// <param name="whatValue">Value of What drop-down list; if the value is 'selected' it also contains selected items</param>
    private string GetWhereCondition(string whatValue)
    {
        string where = string.Empty;

        if (!string.IsNullOrEmpty(whatValue))
        {
            string selectedItems = null;
            string whatAction = null;

            if (whatValue.Contains("#"))
            {
                // Char '#' decides what-value and selected items
                whatAction = whatValue.Substring(0, whatValue.IndexOfCSafe("#"));
                selectedItems = whatValue.Substring(whatValue.IndexOfCSafe("#") + 1);
            }
            else
            {
                whatAction = whatValue;
            }

            What what = (What)ValidationHelper.GetInteger(whatAction, 0);

            switch (what)
            {
                case What.All:
                    // For all items get where condition from grid setting
                    where = SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause);
                    break;
                case What.Selected:
                    // For selected items compose where condition from selected items
                    string[] items = selectedItems.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    where = SqlHelper.GetWhereCondition<int>("AccountID", items, false);
                    break;
            }
        }

        return where;
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        string queryString = string.Empty;

        if (!string.IsNullOrEmpty(CallbackArgument))
        {
            // Prepare parameters...
            mParameters = new Hashtable();
            // ...for mass action
            if (CallbackArgument.StartsWithCSafe("massaction;", true))
            {
                // Get values of callback argument
                string[] selection = CallbackArgument.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (selection.Length != 3)
                {
                    return null;
                }

                // Get selected actions from DD-list
                Action action = (Action)ValidationHelper.GetInteger(selection[2], 0);
                switch (action)
                {
                    case Action.Delete:
                        mParameters["where"] = GetWhereCondition(selection[1]);
                        break;
                    case Action.ChangeStatus:
                        mParameters["allownone"] = true;
                        mParameters["clientid"] = ClientID;
                        break;
                    case Action.AddToGroup:
                        mParameters["clientid"] = ClientID;
                        break;
                    default:
                        return null;
                }
            }
            // ...for UniGrid action
            else
            {
                mParameters["where"] = SqlHelper.GetWhereCondition<int>("AccountID", new [] { CallbackArgument }, false);
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