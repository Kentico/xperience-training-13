using System;
using System.Linq;

using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject("ecommerce.shippingcost", "ShippingCostID")]
[ParentObject(ShippingOptionInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.ECOMMERCE, "Configuration.ShippingOptions.ShippingCosts")]
[Breadcrumbs]
[Breadcrumb(0, "com.ui.shippingcost", "ShippingOption_Edit_ShippingCosts.aspx?objectid={%EditedObjectParent.ID%}", null)]
[Breadcrumb(1, "com.ui.shippingcost.edit", ExistingObject = true)]
[Breadcrumb(1, "com.ui.shippingcost.edit_new", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_ShippingCosts_Edit : CMSShippingOptionsPage
{
    private ShippingOptionInfo mShippingOptionObj;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register item validation
        EditForm.OnItemValidation += EditForm_OnItemValidation;

        mShippingOptionObj = EditedObjectParent as ShippingOptionInfo;

        if (mShippingOptionObj != null)
        {
            // Register check permissions
            EditForm.OnCheckPermissions += (s, args) => CheckConfigurationModification();
        }
        else
        {
            // Invalid shipping option
            EditedObject = null;
            return;
        }

        var shippingCostObj = EditedObject as ShippingCostInfo;
        if (shippingCostObj != null)
        {
            // Check if edited shipping cost is assigned to shipping option
            if ((shippingCostObj.ShippingCostID != 0) && (shippingCostObj.ShippingCostShippingOptionID != mShippingOptionObj.ShippingOptionID))
            {
                EditedObject = null;
            }

            // Check if not editing object from another site
            CheckEditedObjectSiteID(mShippingOptionObj.ShippingOptionSiteID);

            // Check presence of main currency
            CheckMainCurrency(mShippingOptionObj.ShippingOptionSiteID);
        }
    }


    private void EditForm_OnItemValidation(object sender, ref string errorMessage)
    {
        // Look for record with same minimum weight
        var ctrl = sender as FormEngineUserControl;
        if ((ctrl != null) && (ctrl.FieldInfo.Name == "ShippingCostMinWeight"))
        {
            // Check if entered value is double number
            if (!ValidationHelper.IsDouble(ctrl.Value))
            {
                EditForm.StopProcessing = false;

                return;
            }

            ShippingCostInfo editedObj = ctrl.Data as ShippingCostInfo;
            if ((mShippingOptionObj != null) && (editedObj != null))
            {
                double weight = ValidationHelper.GetDouble(ctrl.Value, 0);
                var costs = ShippingCostInfo.Provider.Get()
                                  .TopN(1)
                                  .Columns("ShippingCostID")
                                  .WhereEquals("ShippingCostMinWeight", weight)
                                  .WhereEquals("ShippingCostShippingOptionID", mShippingOptionObj.ShippingOptionID)
                                  .Where("ShippingCostID", QueryOperator.NotEquals, editedObj.ShippingCostID);

                if (costs.Any())
                {
                    errorMessage = GetString("com.ui.shippingcost.edit_costexists");
                    EditForm.StopProcessing = false;
                }
            }
        }
    }
}