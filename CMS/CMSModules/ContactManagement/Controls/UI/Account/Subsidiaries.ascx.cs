using System;

using CMS.Base;
using CMS.Helpers;

using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.MacroEngine;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_Controls_UI_Account_Subsidiaries : CMSAdminListControl
{
    #region "Variables"

    private AccountInfo accountInfo;
    private bool? mModifyAccountPermission;
    private CMSModules_ContactManagement_Controls_UI_Account_Filter filter;

    /// <summary>
    /// Available actions in mass action selector.
    /// </summary>
    protected enum Action
    {
        SelectAction = 0,
        Remove = 1
    }


    /// <summary>
    /// Selected objects in mass action selector.
    /// </summary>
    protected enum What
    {
        Selected = 0,
        All = 1
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Local placeholder for messages
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Gets permission for modifying an account.
    /// </summary>
    protected bool ModifyAccountPermission
    {
        get
        {
            return (bool)(mModifyAccountPermission ??
                          (mModifyAccountPermission = AuthorizationHelper.AuthorizedModifyContact(false)));
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
            accountSelector.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
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
            accountSelector.IsLiveSite = value;
            gridElem.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events and methods"

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
            filter.DisplayAccountStatus = true;
            filter.IsLiveSite = IsLiveSite;
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        if (UIContext.EditedObject != null)
        {
            accountInfo = (AccountInfo)UIContext.EditedObject;

            // Initialize account selector
            accountSelector.UniSelector.Enabled = AuthorizationHelper.AuthorizedModifyContact(false);
            accountSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            accountSelector.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
            accountSelector.UniSelector.DialogButton.ResourceString = "om.account.addaccount";

            // Setup UniGrid
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            gridElem.OnAction += gridElem_OnAction;
            gridElem.ZeroRowsText = GetString("om.account.noaccountsfound");

            // Initialize dropdown lists
            if (!RequestHelper.IsPostBack())
            {
                drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
                drpAction.Items.Add(new ListItem(GetString("general.remove"), Convert.ToInt32(Action.Remove).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("om.account." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("om.account." + What.All), Convert.ToInt32(What.All).ToString()));
            }

            // Isn't child of someone else
            accountSelector.WhereCondition = "AccountSubsidiaryOfID IS NULL";
            // And isn't recursive parent of edited account
            accountSelector.WhereCondition = SqlHelper.AddWhereCondition(accountSelector.WhereCondition, "AccountID NOT IN (SELECT * FROM Func_OM_Account_GetSubsidiaryOf(" + accountInfo.AccountID + ", 1))");
        }
        else
        {
            StopProcessing = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register JS scripts
        RegisterScripts();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide footer if grid is empty
        pnlFooter.Visible = !gridElem.IsEmpty;
    }

    #endregion


    #region "Events"


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;

        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
                btn = ((CMSGridActionButton)sender);
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Account detail URL
                string accountURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditAccount", objectID);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(accountURL, "AccountDetail");
                break;

            case "remove":
                if (!ModifyAccountPermission)
                {
                    btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                break;
        }

        return null;
    }


    /// <summary>
    /// UniGrid button clicked.
    /// </summary>
    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "remove")
        {
            if (AuthorizationHelper.AuthorizedModifyContact(true))
            {
                int relationId = ValidationHelper.GetInteger(actionArgument, 0);
                AccountInfo editedObject = AccountInfo.Provider.Get(relationId);
                if (editedObject != null)
                {
                    editedObject.AccountSubsidiaryOfID = 0;
                    AccountInfo.Provider.Set(editedObject);
                }
            }
        }
    }


    /// <summary>
    /// Items changed event handler.
    /// </summary>
    private void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        if (AuthorizationHelper.AuthorizedModifyContact(true))
        {
            // Get new items from selector
            var newValues = ValidationHelper.GetString(accountSelector.Value, null);
            if (newValues == null)
            {
                return;
            }

            var newItems = newValues.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            // Set HQ ID of selected accounts to edited account ID
            string where = SqlHelper.GetWhereCondition<int>("AccountID", newItems, false);
            AccountInfoProvider.UpdateAccountHQ(accountInfo.AccountID, @where);

            gridElem.ReloadData();
            pnlUpdate.Update();
            accountSelector.Value = null;
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (AuthorizationHelper.AuthorizedModifyContact(true))
        {
            Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedItem.Value, 0);
            What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedItem.Value, 0);

            string where;

            switch (what)
            {
                // All items
                case What.All:
                    where = MacroResolver.Resolve(gridElem.WhereCondition);
                    break;
                // Selected items
                case What.Selected:
                    where = SqlHelper.GetWhereCondition<int>("AccountID", gridElem.SelectedItems, false);
                    break;
                default:
                    return;
            }

            switch (action)
            {
                // Action 'Remove'
                case Action.Remove:
                    // Clear HQ ID of selected accounts
                    AccountInfoProvider.UpdateAccountHQ(0, where);
                    ShowConfirmation(GetString("om.account.massaction.removed"));
                    break;
                default:
                    return;
            }

            // Reload UniGrid
            gridElem.ResetSelection();
            gridElem.ReloadData();
            pnlUpdate.Update();
        }
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

        // Register script to open dialogs for role selection and for account editing
        script.Append(@"
function Refresh()
{
    __doPostBack('" + pnlUpdate.ClientID + @"', '');
}
");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GridActions", ScriptHelper.GetScript(script.ToString()));

        // Register script for mass actions
        script = new StringBuilder();
        script.Append(
            @"
function PerformAction(selectionFunction, actionId, actionLabel, whatId) {
    var confirmation = null;
    var label = document.getElementById(actionLabel);
    var action = document.getElementById(actionId).value;
    var whatDrp = document.getElementById(whatId);
    label.innerHTML = '';
    if (action == '", (int)Action.SelectAction, @"') {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("MassAction.SelectSomeAction"), @"
    }
    else if (eval(selectionFunction) && (whatDrp.value == '", (int)What.Selected, @"')) {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("om.account.massaction.select"), @";
    }
    else {
        switch(action) {
            case '", (int)Action.Remove, @"':
                confirmation = ", ScriptHelper.GetString(GetString("General.ConfirmRemove")), @";
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
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MassActions", ScriptHelper.GetScript(script.ToString()));

        // Add action to button
        btnOk.OnClientClick = "return PerformAction('" + gridElem.GetCheckSelectionScript() + "','" + drpAction.ClientID + "','" + lblInfo.ClientID + "','" + drpWhat.ClientID + "');";
    }

    #endregion
}
