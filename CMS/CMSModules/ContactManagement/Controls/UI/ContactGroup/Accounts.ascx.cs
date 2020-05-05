using System;
using System.Data;

using CMS.Base;
using CMS.Helpers;

using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_Controls_UI_ContactGroup_Accounts : CMSAdminListControl
{
    #region "Variables"

    private ContactGroupInfo cgi;
    private bool mModifyPermission;


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
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


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
            accountSelector.StopProcessing = value;
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
            accountSelector.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Get edited object (contact group)
        if (UIContext.EditedObject != null)
        {
            cgi = (ContactGroupInfo)UIContext.EditedObject;

            // Setup unigrid
            gridElem.WhereCondition = GetWhereCondition();
            gridElem.OnAction += gridElem_OnAction;
            gridElem.ZeroRowsText = GetString("om.account.noaccountsfound");
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            gridElem.OnDataReload += gridElem_OnDataReload;

            mModifyPermission = AuthorizationHelper.AuthorizedModifyContact(false);

            // Initialize dropdown lists
            if (!RequestHelper.IsPostBack())
            {
                // Display mass actions
                if (mModifyPermission)
                {
                    drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
                    drpAction.Items.Add(new ListItem(GetString("general.remove"), Convert.ToInt32(Action.Remove).ToString()));
                    drpWhat.Items.Add(new ListItem(GetString("om.account." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
                    drpWhat.Items.Add(new ListItem(GetString("om.account." + What.All), Convert.ToInt32(What.All).ToString()));
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

            // Initialize account selector
            accountSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            accountSelector.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
            accountSelector.UniSelector.DialogButton.ResourceString = "om.account.addaccount";

        }
        else
        {
            StopProcessing = true;
        }
    }


    private DataSet gridElem_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        var query = GetFilteredAccountsFromView()
            .Where(completeWhere)
            .OrderBy(currentOrder)
            .TopN(currentTopN)
            .Columns(columns);

        query.MaxRecords = currentPageSize;
        query.Offset = currentOffset;

        totalRecords = query.TotalRecords;

        return query.Result;
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
        pnlFooter.Visible = !gridElem.IsEmpty && (drpAction.Items.Count > 0);
    }

    #endregion


    #region "Events"

    /// <summary>
    /// UniGrid external databound.
    /// </summary>
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

            // Display delete button
            case "remove":
                btn = (CMSGridActionButton)sender;

                // Display delete button only for users with appropriate permission
                btn.Enabled = mModifyPermission;
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
            int accountId = ValidationHelper.GetInteger(actionArgument, 0);
            AccountInfo account = AccountInfo.Provider.Get(accountId);
            if (account != null)
            {
                CheckModifyPermissions();

                // Get the relationship object
                ContactGroupMemberInfo mi = ContactGroupMemberInfo.Provider.Get(cgi.ContactGroupID, accountId, ContactGroupMemberTypeEnum.Account);
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
        string newValues = ValidationHelper.GetString(accountSelector.Value, null);
        string[] newItems = newValues.Split(new[]
        {
            ';'
        }, StringSplitOptions.RemoveEmptyEntries);

        // Get all selected items
        foreach (string item in newItems)
        {
            // Check if relation already exists
            int itemID = ValidationHelper.GetInteger(item, 0);
            if (ContactGroupMemberInfo.Provider.Get(cgi.ContactGroupID, itemID, ContactGroupMemberTypeEnum.Account) == null)
            {
                ContactGroupMemberInfo.Provider.Add(cgi.ContactGroupID, itemID, ContactGroupMemberTypeEnum.Account, MemberAddedHowEnum.Manual);
            }
        }

        gridElem.ReloadData();
        pnlUpdate.Update();
        accountSelector.Value = null;
    }


    /// <summary>
    /// Checks modify permission for contact group.
    /// </summary>
    private void CheckModifyPermissions()
    {
        // Check modify permission
        if (!CheckPermissions("cms.contactmanagement", "Modify"))
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Modify");
        }
    }


    /// <summary>
    /// Mass action 'ok' button clicked.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        CheckModifyPermissions();

        Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedItem.Value, 0);
        What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedItem.Value, 0);

        var where = new WhereCondition()
            .WhereEquals("ContactGroupMemberContactGroupID", cgi.ContactGroupID)
            // Set constraint for account relations only
            .WhereEquals("ContactGroupMemberType", 1);

        switch (what)
        {
            // All items
            case What.All:
                var accountIds = GetFilteredAccountsFromView().Column("AccountID");

                where.WhereIn("ContactGroupMemberRelatedID", accountIds);
                break;
            // Selected items
            case What.Selected:
                // Convert array to integer values to make sure no sql injection is possible (via string values)
                where.WhereIn("ContactGroupMemberRelatedID", gridElem.SelectedItems);
                break;
            default:
                return;
        }

        switch (action)
        {
            // Action 'Remove'
            case Action.Remove:
                // Delete the relations between contact group and accounts
                ContactGroupMemberInfoProvider.DeleteContactGroupMembers(where.ToString(true), cgi.ContactGroupID, true, true);
                // Show result message
                if (what == What.Selected)
                {
                    ShowConfirmation(GetString("om.account.massaction.removed"));
                }
                else
                {
                    ShowConfirmation(GetString("om.account.massaction.removedall"));
                }
                break;
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

    /// <summary>
    /// Returns query containing accounts filtered by the Unigrid conditions.
    /// </summary>
    private ObjectQuery<AccountInfo> GetFilteredAccountsFromView()
    {
        return new ObjectQuery<AccountInfo>().From("View_OM_ContactGroupMember_AccountJoined")
                                             .Where(gridElem.WhereCondition)
                                             .Where(gridElem.WhereClause);
    }


    /// <summary>
    /// Returns WHERE condition
    /// </summary>
    private string GetWhereCondition()
    {
        if (!AuthorizationHelper.AuthorizedReadContact(true))
        {
            return new WhereCondition().NoResults().ToString();
        }
        
        return new WhereCondition().WhereEquals("ContactGroupMemberContactGroupID", cgi.ContactGroupID).ToString(true);
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs for account editing
        script.Append(@"
function Refresh()
{
    __doPostBack('", pnlUpdate.ClientID, @"', '');
}
function PerformAction(selectionFunction, actionId, actionLabel, whatId) 
{
    var confirmation = null;
    var label = document.getElementById(actionLabel);
    var action = document.getElementById(actionId).value;
    var whatDrp = document.getElementById(whatId).value;
    label.innerHTML = '';
    if (action == '", (int)Action.SelectAction, @"') 
    {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("MassAction.SelectSomeAction"), @"
    }
    else if (eval(selectionFunction) && (whatDrp == '", (int)What.Selected, @"')) 
    {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("om.account.massaction.select"), @";
    }
    else 
    {
        switch(action) 
        {
            case '", (int)Action.Remove, @"':
                if (whatDrp == ", (int)What.Selected, @")
                {
                    confirmation = ", ScriptHelper.GetString(GetString("General.ConfirmRemove")), @";
                }
                else
                {
                    confirmation = ", ScriptHelper.GetString(GetString("General.ConfirmRemoveAll")), @";
                }                
            break;
            default:
                confirmation = null;
                break;
        }
        if (confirmation != null) 
        {
            return confirm(confirmation)
        }
    }
    return false;
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Actions", ScriptHelper.GetScript(script.ToString()));

        // Add action to button
        btnOk.OnClientClick = "return PerformAction('" + gridElem.GetCheckSelectionScript() + "','" + drpAction.ClientID + "','" + lblInfo.ClientID + "','" + drpWhat.ClientID + "');";
    }

    #endregion
}
