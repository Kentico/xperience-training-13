using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(CreditEventInfo.OBJECT_TYPE, "eventid")]
[ParentObject(CustomerInfo.OBJECT_TYPE, "customerid")]
// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "CreditEvent_Edit.ItemListLink", "Customer_Edit_Credit_List.aspx?customerid={%EditedObjectParent.ID%}&siteId={%EditedObjectParent.SiteID%}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "CreditEvent_Edit.NewItemCaption", NewObject = true)]
// Help
[Help("newedit_credit_event", "helpTopic")]
// Security
[UIElement(ModuleName.ECOMMERCE, "Customers.Credit")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Credit_Edit : CMSEcommercePage
{
    private int creditSiteId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        creditSiteId = ECommerceHelper.GetSiteID(SiteContext.CurrentSiteID, ECommerceSettings.USE_GLOBAL_CREDIT);
        
        CreditEventInfo creditEvent = EditedObject as CreditEventInfo;
        // Check site of credit event
        if ((creditEvent != null) && (creditEvent.EventID > 0) && (creditEvent.EventSiteID != creditSiteId))
        {
            EditedObject = null;
        }

        creditEvent = EditForm.Data as CreditEventInfo;
        if ((creditEvent != null) && (creditEvent.EventID == 0))
        {
            creditEvent.EventSiteID = creditSiteId;
        }

        // Check presence of main currency
        CheckMainCurrency(creditSiteId);
        
        // Register check permissions
        EditForm.OnCheckPermissions += (s, args) => CheckPermissions();
    }


    /// <summary>
    /// Check if user is authorized to modify Customer's credit.
    /// </summary>
    private void CheckPermissions()
    {
        // Check modify permission
        if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyCustomers");
        }

        // Check if using global credit
        if (creditSiteId <= 0)
        {
            // Check Ecommerce global modify permission
            if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ECOMMERCE_MODIFYGLOBAL))
            {
                RedirectToAccessDenied(ModuleName.ECOMMERCE, EcommercePermissions.ECOMMERCE_MODIFYGLOBAL);
            }
        }
    }
}