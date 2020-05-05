using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_Controls_UI_Account_Edit : CMSAdminEditControl
{
    #region "Variables"

    private AccountInfo accountInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Event that fires after saving the form.
    /// </summary>
    public event EventHandler OnAfterSave
    {
        add
        {
            EditForm.OnAfterSave += value;
        }
        remove
        {
            EditForm.OnAfterSave -= value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
    }


    /// <summary>
    /// OnAfterDataLoad event handler.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        accountInfo = (AccountInfo)EditForm.EditedObject;
        DistributeParams();
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Repairs (none) selector value
        string[] editforms = { "AccountCountryID", "accountprimarycontactid", "accountsecondarycontactid", "accountsubsidiaryofid", "accountstatusid" };
        foreach (string editFormName in editforms)
        {
            if (ValidationHelper.GetInteger(EditForm.Data[editFormName], 0) <= 0)
            {
                EditForm.Data[editFormName] = null;
            }
        }
        
        int subsidiaryID = ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("accountsubsidiaryofid"), -1);

        // When subsidiary account does not exist anymore, reset UI form Subsidiary of field
        if (AccountInfo.Provider.Get(subsidiaryID) == null)
        {
            EditForm.EditedObject.SetValue("accountsubsidiaryofid", null);
            ((UniSelector)EditForm.FieldControls["accountsubsidiaryofid"]).Reload(true);
        }

        AssignContacts();
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        AccountInfo account = (AccountInfo)EditForm.EditedObject;

        // Refresh breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, account.AccountName);
    }


    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        InitHeaderActions();

        // Initialize redirection URL
        string url = UIContextHelper.GetElementUrl(ModuleName.CONTACTMANAGEMENT, "EditAccount", false);
        url = URLHelper.AddParameterToUrl(url, "objectid", "{%EditedObject.ID%}");
        url = URLHelper.AddParameterToUrl(url, "saved", "1");

        EditForm.RedirectUrlAfterCreate = url;

        // Connect role selector and contact selector
        ((UniSelector)EditForm.FieldControls["accountprimarycontactid"]).OnSelectionChanged += (s, ea) => SetContactRoleID((FormEngineUserControl)s, EditForm.FieldControls["accountprimarycontactroleid"]);
        ((UniSelector)EditForm.FieldControls["accountsecondarycontactid"]).OnSelectionChanged += (s, ea) => SetContactRoleID((FormEngineUserControl)s, EditForm.FieldControls["accountsecondarycontactroleid"]);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Reload values in contact role selectors
        SetContactRoleID(EditForm.FieldControls["accountprimarycontactid"], EditForm.FieldControls["accountprimarycontactroleid"]);
        SetContactRoleID(EditForm.FieldControls["accountsecondarycontactid"], EditForm.FieldControls["accountsecondarycontactroleid"]);

        // Hide primary contact field when no contacts are in account. Other fields in Contacts region has visibility condition based on this field.
        if (!((UniSelector)EditForm.FieldControls["accountprimarycontactid"]).HasData)
        {
            EditForm.FieldControls["accountprimarycontactid"].Visible = false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Sets contact role ID to given role selector control based on contact selector value and edited account.
    /// </summary>
    private void SetContactRoleID(FormEngineUserControl contactSelector, FormEngineUserControl roleSelector)
    {
        int contactID = ValidationHelper.GetInteger(contactSelector.Value, 0);
        int accountID = accountInfo.AccountID;
        var accountContactInfo = AccountContactInfo.Provider.Get(accountID, contactID);
        roleSelector.Value = (accountContactInfo != null) ? accountContactInfo.ContactRoleID : UniSelector.US_NONE_RECORD;
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check permission
        AuthorizationHelper.AuthorizedModifyContact(true);

        switch (e.CommandName.ToLowerCSafe())
        {
            // Save account
            case "save":
                EditForm.SaveData(null);
                break;
        }
    }


    /// <summary>
    /// Sets primary and secondary contacts.
    /// </summary>
    private void AssignContacts()
    {
        ContactInfo contact;
        AccountContactInfo accountContact;

        // Assign primary contact to account and/or assign role
        int contactID = ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("accountprimarycontactid"), -1);
        int contactRoleID = ValidationHelper.GetInteger(EditForm.FieldControls["accountprimarycontactroleid"].Value, -1);

        if (contactID > 0)
        {
            contact = ContactInfo.Provider.Get(contactID);
            if (contact != null)
            {
                accountContact = AccountContactInfo.Provider.Get(accountInfo.AccountID, contactID);

                // Update relation
                if (accountContact != null)
                {
                    accountContact.ContactRoleID = contactRoleID;
                    AccountContactInfo.Provider.Set(accountContact);
                }
                else
                {
                    EditForm.EditedObject.SetValue("accountprimarycontactid", null);
                    ((UniSelector)EditForm.FieldControls["accountprimarycontactid"]).Reload(true);
                }
            }
            // Selected contact doesn't exist
            else
            {
                ShowError(GetString("om.contact.primarynotexists"));
                return;
            }
        }

        // Assign secondary contact to account and/or assign role
        contactID = ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("accountsecondarycontactid"), -1);
        contactRoleID = ValidationHelper.GetInteger(EditForm.FieldControls["accountsecondarycontactroleid"].Value, -1);

        // Assign secondary contact to account and/or assign role
        if (contactID > 0)
        {
            contact = ContactInfo.Provider.Get(contactID);
            if (contact != null)
            {
                accountContact = AccountContactInfo.Provider.Get(accountInfo.AccountID, contactID);

                // Update relation
                if (accountContact != null)
                {
                    accountContact.ContactRoleID = contactRoleID;
                    AccountContactInfo.Provider.Set(accountContact);
                }
                else
                {
                    EditForm.EditedObject.SetValue("accountsecondarycontactid", null);
                    ((UniSelector)EditForm.FieldControls["accountsecondarycontactid"]).Reload(true);
                }
            }
            else
            {
                ShowError(GetString("om.contact.secondarynotexists"));
            }
        }
    }


    /// <summary>
    /// Distributes parameters to form controls.
    /// </summary>
    private void DistributeParams(object sender = null, EventArgs eventArgs = null)
    {
        if (EditForm.FieldControls == null)
        {
            // Try to call that later, if controls are not initialized yet
            EditForm.Load += DistributeParams;
            return;
        }

        // UniSelector
        SetControl("accountsubsidiaryofid", ctrl =>
        {
            int accountID = ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("AccountID"), 0);
            ctrl.SetValue("wherecondition", "(AccountID NOT IN (" + "SELECT * FROM Func_OM_Account_GetSubsidiaries(" + accountID + ", 1)))");
        });
        // UserSelector
        SetControl("accountowneruserid", ctrl => ctrl.SetValue("wherecondition", "UserName NOT LIKE N'public'"));
        // UniSelector
        SetControl("accountprimarycontactid", ctrl => ctrl.SetValue("wherecondition", "(ContactID IN (SELECT ContactID FROM OM_AccountContact WHERE AccountID = " + ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("AccountID"), 0) + "))"));
        // UniSelector
        SetControl("accountsecondarycontactid", ctrl => ctrl.SetValue("wherecondition", "(ContactID IN (SELECT ContactID FROM OM_AccountContact WHERE AccountID = " + ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("AccountID"), 0) + "))"));
    }


    /// <summary>
    /// Performs an action on found control.
    /// </summary>
    private void SetControl(string controlName, Action<FormEngineUserControl> action)
    {
        var control = EditForm.FieldControls[controlName];
        if (control != null)
        {
            action(control);
        }
    }

    #endregion
}