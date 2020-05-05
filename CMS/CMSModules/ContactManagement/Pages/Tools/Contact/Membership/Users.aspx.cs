using System;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactid")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "ContactMembership.Users")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Users : CMSContactManagementPage
{
    #region "Variables"

    private int contactId;
    private bool modifyAllowed;
    private ContactInfo ci;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckUIElementAccessHierarchical(ModuleName.CONTACTMANAGEMENT, "ContactMembership.Users");
        ci = (ContactInfo)EditedObject;
        if (ci == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }
        
        modifyAllowed = AuthorizationHelper.AuthorizedModifyContact(false);

        contactId = QueryHelper.GetInteger("contactid", 0);

        string where = null;
        
        gridElem.ObjectType = ContactMembershipUserListInfo.OBJECT_TYPE;

        // Query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ContactId", contactId);

        gridElem.WhereCondition = where;
        gridElem.QueryParameters = parameters;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        CurrentMaster.HeaderActionsPlaceHolder.Visible = modifyAllowed;
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
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

    #endregion


    #region "Private methods"

    /// <summary>
    /// Disables delete when user has no modify rights.
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