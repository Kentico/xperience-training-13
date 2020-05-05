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


public partial class CMSModules_ContactManagement_Controls_UI_Account_Contacts : CMSAdminListControl, ICallbackEventHandler
{
    #region "Variables"

    private AccountInfo ai;
    private Hashtable mParameters;
    private bool modifyAccountContact;


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
            contactSelector.StopProcessing = value;
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

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get edited object
        if (UIContext.EditedObject != null)
        {
            ai = (AccountInfo)UIContext.EditedObject;

            // Setup unigrid
            gridElem.GridOptions.ShowSelection = true;
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "AccountID = " + ai.AccountID);
            gridElem.OnAction += gridElem_OnAction;
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            gridElem.ZeroRowsText = GetString("om.account.nocontacts");

            // Initialize dropdown lists
            if (!RequestHelper.IsPostBack())
            {
                drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
                drpAction.Items.Add(new ListItem(GetString("general.remove"), Convert.ToInt32(Action.Remove).ToString()));
                drpAction.Items.Add(new ListItem(GetString("om.contactrole.selectitem"), Convert.ToInt32(Action.SelectRole).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("om.contact." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("om.contact." + What.All), Convert.ToInt32(What.All).ToString()));
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
            contactSelector.UniSelector.SelectItemPageUrl = "~/CMSModules/ContactManagement/Pages/Tools/Account/Add_Contact_Dialog.aspx";
            contactSelector.UniSelector.SetValue("IsLiveSite", false);
            contactSelector.UniSelector.FilterControl = "~/CMSModules/ContactManagement/Filters/ContactFilter.ascx";
            contactSelector.UniSelector.UseDefaultNameFilter = false;

            modifyAccountContact = AuthorizationHelper.AuthorizedModifyContact(false);

            // Register JS scripts
            RegisterScripts();
        }
        else
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // Hide footer if grid is empty or if the account is merged (is not active)
        pnlFooter.Visible = (!gridElem.IsEmpty) && (gridElem.GridOptions.ShowSelection);

        // Hide controls when account is merged or user doesn't have permission
        if (!modifyAccountContact)
        {
            pnlFooter.Visible = false;
            pnlSelector.Visible = false;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Uni-grid external data bound event handler.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn = sender as CMSGridActionButton;
        if (btn != null)
        {
            switch (sourceName.ToLowerCSafe())
            {
                case "edit":
                    string contactURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", btn.CommandArgument.ToInteger(0));
                    // Add modal dialog script to onClick action
                    btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                    break;

                case "selectrole":
                    if (!modifyAccountContact)
                    {
                        btn.Enabled = false;
                    }
                    else
                    {
                        btn.OnClientClick = string.Format("dialogParams_{0} = '{1}';{2};return false;",
                                                          ClientID,
                                                          btn.CommandArgument,
                                                          Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectRole", null));
                    }
                    break;

                case "remove":
                    if (!modifyAccountContact)
                    {
                        btn.Enabled = false;
                    }
                    break;
            }
        }

        return null;
    }


    /// <summary>
    /// Uni-grid button clicked.
    /// </summary>
    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "remove")
        {
            if (modifyAccountContact)
            {
                int relationId = ValidationHelper.GetInteger(actionArgument, 0);
                AccountContactInfo.Provider.Delete(AccountContactInfo.Provider.Get(relationId));
            }
            else
            {
                CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Modify");
            }
        }
    }


    /// <summary>
    /// Items changed event handler.
    /// </summary>
    private void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        if (AuthorizationHelper.AuthorizedModifyContact(false) || AuthorizationHelper.AuthorizedReadContact(false))
        {
            // Get new items from selector
            string newValues = ValidationHelper.GetString(contactSelector.Value, null);

            if (!String.IsNullOrEmpty(newValues))
            {
                string[] newItems = newValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                int previousStop = 0;
                string where = FetchNextContacts(ref previousStop, newItems, 1000);

                while (!String.IsNullOrEmpty(where))
                {
                    AccountContactInfoProvider.SetContactsIntoAccount(ai.AccountID, "ContactID IN (" + where + ")", ValidationHelper.GetInteger(hdnRoleID.Value, 0));

                    where = FetchNextContacts(ref previousStop, newItems, 1000);
                }

                gridElem.ReloadData();
                pnlUpdate.Update();
                contactSelector.Value = null;
            }
        }
        // No permission modify
        else
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Modify");
        }
    }


    /// <summary>
    /// Button OK click event handler.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (modifyAccountContact)
        {
            Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedItem.Value, 0);
            What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedItem.Value, 0);

            string where = string.Empty;

            switch (what)
            {
                // All items
                case What.All:
                    where = SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause);
                    where = string.Format("AccountID={0} AND ContactID IN (SELECT ContactID FROM View_OM_AccountContact_ContactJoined WHERE {1})", ai.AccountID, where);
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
                    // Reset account's main contact IDs if any of the contacts was set as primary or secondary contact
                    AccountContactInfoProvider.ResetAccountMainContacts(ai.AccountID, 0, where);
                    // Delete the relations between account and contacts
                    AccountContactInfoProvider.DeleteAllAccountContacts(where);
                    // Show result message
                    if (what == What.Selected)
                    {
                        ShowConfirmation(GetString("om.contact.massaction.removed"));
                    }
                    else
                    {
                        ShowConfirmation(GetString("om.contact.massaction.removedall"));
                    }
                    break;
                // Action 'Select role'
                case Action.SelectRole:
                    // Get selected role ID from hidden field
                    int roleId = ValidationHelper.GetInteger(hdnValue.Value, -1);
                    if (roleId >= 0)
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
    /// Returns one thousand of contacts from contacts to be added.
    /// </summary>
    /// <param name="previousStop">Previous position</param>
    /// <param name="newItems">Array of items to be added</param>
    /// <param name="howMuch">How much of records to fetch</param>
    /// <returns>Returns items separated by colon.</returns>
    private string FetchNextContacts(ref int previousStop, string[] newItems, int howMuch)
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

        // Register script to open dialogs for role selection and for contact editing
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
        label.innerHTML = ", ScriptHelper.GetLocalizedString("om.contact.massaction.select"), @";
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
    document.getElementById('", hdnValue.ClientID, @"').value = roleId;", ControlsHelper.GetPostBackEventReference(btnOk, null), @";
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
                mParameters["ismassaction"] = true;
                mParameters["clientid"] = ClientID;
            }
            else
            {
                // for unigrid action
                mParameters["accountcontactid"] = CallbackArgument;
            }
            mParameters["allownone"] = true;

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
