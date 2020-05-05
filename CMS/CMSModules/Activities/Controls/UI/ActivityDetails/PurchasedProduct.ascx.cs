using System;

using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;


public partial class CMSModules_Activities_Controls_UI_ActivityDetails_PurchasedProduct : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || !ModuleManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            return false;
        }

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.PURCHASEDPRODUCT:
                break;
            default:
                return false;
        }

        GeneralizedInfo sku = ProviderHelper.GetInfoById(PredefinedObjectType.SKU, ai.ActivityItemID);
        if (sku != null)
        {
            string productName = ValidationHelper.GetString(sku.GetValue("SKUName"), null);
            ucDetails.AddRow("om.activitydetails.product", productName);
            ucDetails.AddRow("om.activitydetails.productunits", ai.ActivityValue);
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}