using System;
using System.Collections;

using CMS.Base;
using CMS.Helpers;

using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_Controls_UI_Contact_Accounts : CMSAdminListControl, ICallbackEventHandler
{
    #region "Variables"

    private ContactInfo mContactInfo;
    private Hashtable mParameters;
    private bool mModifyAccountContact;


    /// <summary>
    /// Available actions in mass action selector.
    /// </summary>
    protected enum Action
    {
        SelectAction = 0,
        Remove = 1,
        SelectRole = 2,
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
    /// Mass action selector parameters.
    /// </summary>
    protected enum Argument
    {
        Action = 0,
        AllSelected = 1,
        Items = 2
    }


    /// <summary>
    /// URL of modal dialog window for contact role selection.
    /// </summary>
    public const string CONTACT_ROLE_DIALOG = "~/CMSModules/ContactManagement/FormControls/ContactRoleDialog.aspx";

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
    /// Dialog control identifier.
    /// </summary>
    private Guid Identifier
    {
        get
        {
            Guid identifier;
            if (!Guid.TryParse(hdnValue.Value, out identifier))
            {
                identifier = Guid.NewGuid();
                hdnValue.Value = identifier.ToString();
            }

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


    /// <summary>
    /// Indicates if control is used on live site.
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
        mContactInfo = (ContactInfo)UIContext.EditedObject;

        if (mContactInfo == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        // Setup unigrid
        gridElem.GridOptions.ShowSelection = true;
        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "(ContactID = " + mContactInfo.ContactID + ")");
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.ZeroRowsText = GetString("om.contact.noaccounts");

        // Initialize dropdown lists
        if (!RequestHelper.IsPostBack())
        {
            drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
            drpAction.Items.Add(new ListItem(GetString("general.remove"), Convert.ToInt32(Action.Remove).ToString()));
            drpAction.Items.Add(new ListItem(GetString("om.contactrole.selectitem"), Convert.ToInt32(Action.SelectRole).ToString()));
            drpWhat.Items.Add(new ListItem(GetString("om.account." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
            drpWhat.Items.Add(new ListItem(GetString("om.account." + What.All), Convert.ToInt32(What.All).ToString()));
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
        accountSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
        accountSelector.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        accountSelector.UniSelector.SelectItemPageUrl = "~/CMSModules/ContactManagement/Pages/Tools/Contact/Add_Account_Dialog.aspx";
        accountSelector.UniSelector.SetValue("IsLiveSite", false);
        accountSelector.UniSelector.DialogButton.ResourceString = "om.account.addaccount";

        mModifyAccountContact = AuthorizationHelper.AuthorizedModifyContact(false);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterScripts();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // Hide footer if grid is empty or if the contact is merged (is not active)
        pnlFooter.Visible = (!gridElem.IsEmpty) && (gridElem.GridOptions.ShowSelection);

        // Hide controls when contact is merged or user doesn't have permission
        if (!mModifyAccountContact)
        {
            pnlFooter.Visible = false;
            pnlSelector.Visible = false;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Unigrid external databoud event handler.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
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

            case "selectrole":
                btn = (CMSGridActionButton)sender;
                if (mModifyAccountContact)
                {
                    btn.OnClientClick = string.Format("dialogParams_{0} = '{1}';{2};return false;",
                        ClientID,
                        btn.CommandArgument,
                        Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectRole", null));
                }
                else
                {
                    btn.Enabled = false;
                }
                break;

            case "remove":
                if (!mModifyAccountContact)
                {
                    btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                break;
        }
        return null;
    }


    /// <summary>
    /// Unigrid button clicked.
    /// </summary>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "remove")
        {
            // User has permission modify
            if (mModifyAccountContact)
            {
                int relationId = ValidationHelper.GetInteger(actionArgument, 0);
                AccountContactInfo relation = AccountContactInfo.Provider.Get(relationId);
                if (relation != null)
                {
                    // We need to invalidate the contact as we might have modified some of its relationships and data in the cache might not be valid
                    AccountContactInfo.Provider.Delete(relation);
                }
            }
            // User doesn't have sufficient permissions
            else
            {
                CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Modify");
            }
        }
    }


    /// <summary>
    /// Items changed event handler.
    /// </summary>
    protected void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        if (mModifyAccountContact)
        {
            // Get new items from selector
            string newValues = ValidationHelper.GetString(accountSelector.Value, null);
            if (!String.IsNullOrEmpty(newValues))
            {
                string[] newItems = newValues.Split(new[]
                {
                    ';'
                }, StringSplitOptions.RemoveEmptyEntries);

                if (newItems != null)
                {
                    int previousStop = 0;
                    string where = FetchNextAccounts(ref previousStop, newItems, 1000);

                    while (!String.IsNullOrEmpty(where))
                    {
                        AccountContactInfoProvider.SetAccountsIntoContact(mContactInfo.ContactID, "AccountID IN (" + where + ")", ValidationHelper.GetInteger(hdnRoleID.Value, 0));

                        where = FetchNextAccounts(ref previousStop, newItems, 1000);
                    }
                }

                gridElem.ReloadData();
                pnlUpdate.Update();
                accountSelector.Value = null;
            }
        }
        // No permission modify
        else
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Modify");
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (mModifyAccountContact)
        {
            Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedItem.Value, 0);
            What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedItem.Value, 0);

            string where = string.Empty;

            switch (what)
            {
                // All items
                case What.All:
                    where = SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause);
                    where = string.Format("ContactID={0} AND AccountID IN (SELECT AccountID FROM View_OM_AccountContact_AccountJoined WHERE {1})", mContactInfo.ContactID, where);
                    break;
                // Selected items
                case What.Selected:
                    where = SqlHelper.GetWhereCondition<int>("AccountContactID", gridElem.SelectedItems, false);
                    break;
                default:
                    return;
            }

            switch (action)
            {
                // Action 'Remove'
                case Action.Remove:
                    // Reset accounts' main contact IDs if the contact was set as primary or secondary contact
                    AccountContactInfoProvider.ResetAccountMainContacts(0, mContactInfo.ContactID, where);
                    // Delete the relations between contact and accounts
                    AccountContactInfoProvider.DeleteAllAccountContacts(where);
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
                // Action 'Select role'
                case Action.SelectRole:
                    // Get selected role ID from hidden field
                    int roleId = ValidationHelper.GetInteger(hdnValue.Value, -1);
                    if (roleId >= 0 && mModifyAccountContact)
                    {
                        AccountContactInfoProvider.UpdateContactRole(roleId, where);
                        ShowConfirmation(GetString("om.contact.massaction.roleassigned"));
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
        // No permission modify
        else
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Modify");
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns limited number of accounts to be added.
    /// </summary>
    /// <param name="previousStop">Previous position</param>
    /// <param name="newItems">Array of items to be added</param>
    /// <param name="howMuch">How much of records to fetch</param>
    /// <returns>Returns items separated by colon.</returns>
    private string FetchNextAccounts(ref int previousStop, string[] newItems, int howMuch)
    {
        StringBuilder whereBuild = new StringBuilder();

        // Get new where
        for (int i = previousStop; (i < (previousStop + howMuch)) && (i < newItems.Length); i++)
        {
            whereBuild.Append(ValidationHelper.GetInteger(newItems[i], 0) + ",");
        }

        // Update last position
        if (previousStop + howMuch > newItems.Length)
        {
            previousStop = newItems.Length;
        }
        else
        {
            previousStop += howMuch;
        }

        // Return WHERE
        String where = whereBuild.ToString();
        if (!String.IsNullOrEmpty(where))
        {
            return where.Remove(where.Length - 1, 1);
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs for role selection and for account editing
        script.Append(@"
function SelectRole(queryParameters)
{
    modalDialog('", ResolveUrl(CONTACT_ROLE_DIALOG), @"' + queryParameters, 'selectRole', '660', '590');
}
function Refresh()
{
    __doPostBack('", pnlUpdate.ClientID, @"', '');
}
function setRole(roleID) 
{
    $cmsj('#", hdnRoleID.ClientID, @"').val(roleID);
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
            case '", (int)Action.SelectRole, @"':
                dialogParams_", ClientID, @" = 'ismassaction';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectRole", null), @";
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
}
function AssignContactRole_", ClientID, @"(roleId) 
{
    document.getElementById('", hdnValue.ClientID, @"').value = roleId;", ControlsHelper.GetPostBackEventReference(btnOk), @";
}
    
var dialogParams_", ClientID, @" = '';");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Actions", ScriptHelper.GetScript(script.ToString()));

        // Add action to button
        btnOk.OnClientClick = "return PerformAction('" + gridElem.GetCheckSelectionScript() + "','" + drpAction.ClientID + "','" + lblInfo.ClientID + "','" + drpWhat.ClientID + "');";
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
            if (CallbackArgument.EqualsCSafe("ismassaction", true))
            {
                // for mass action
                mParameters["ismassaction"] = "1";
                mParameters["clientid"] = ClientID;
            }
            else
            {
                // for unigrid action
                mParameters["accountcontactid"] = CallbackArgument;
            }
            mParameters["allownone"] = "1";

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
