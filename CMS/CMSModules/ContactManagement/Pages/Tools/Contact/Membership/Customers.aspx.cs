using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactid")]
[UIElement(ModuleName.ECOMMERCE, "ContactMembership.Customers")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Customers : CMSContactManagementPage
{
    #region "Variables"

    private int contactId;
    private bool modifyAllowed;
    private ContactInfo ci;

    #endregion


    #region "Page events"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        ci = (ContactInfo)EditedObject;
        if (ci == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }
        
        modifyAllowed = AuthorizationHelper.AuthorizedModifyContact(false);

        contactId = QueryHelper.GetInteger("contactid", 0);

        gridElem.ObjectType = ContactMembershipCustomerListInfo.OBJECT_TYPE;

        // Query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ContactId", contactId);

        gridElem.QueryParameters = parameters;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        // Hide header actions for global contact or merged contact.
        CurrentMaster.HeaderActionsPlaceHolder.Visible = modifyAllowed;
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "delete":
                DisableDeleteButton(sender);
                break;
        }

        return parameter;
    }


    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int membershipId = ValidationHelper.GetInteger(actionArgument, 0);
                if (membershipId > 0)
                {
                    // Check permissions
                    if (AuthorizationHelper.AuthorizedModifyContact(true))
                    {
                        ContactMembershipInfo.Provider.Delete(ContactMembershipInfo.Provider.Get(membershipId));
                    }
                }
                break;
        }
    }


    /// <summary>
    /// For global, merged contacts or users without permissions is delete button disabled.
    /// </summary>
    private void DisableDeleteButton(object sender)
    {
        if (!modifyAllowed)
        {
            var button = ((CMSGridActionButton)sender);
            button.Enabled = false;
        }
    }

    #endregion
}