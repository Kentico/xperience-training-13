using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;


public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_GlobalObjects : CMSEcommerceStoreSettingsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions for CMS Desk -> Ecommerce
        if (!IsGlobalStoreAdmin)
        {
            RedirectToAccessDenied(GetString("security.accesspage.onlyglobaladmin"));
            return;
        }

        // Check UI element
        var elementName = IsMultiStoreConfiguration ? "Tools.Ecommerce.GlobalObjectsSettings" : "Configuration.Settings.GlobalObjects";
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, elementName);

        // Set up header
        CurrentMaster.HeaderActions.AddAction(new SaveAction());
        CurrentMaster.HeaderActions.ActionPerformed += StoreSettingsActions_ActionPerformed;

        // Assign category, group and site ID
        SettingsGroupViewer.CategoryName = "CMS.Ecommerce.GlobalObjects";
        SettingsGroupViewer.SiteID = SiteID;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Show warning when using global exchange rates and site specific currencies
        if (ECommerceSettings.UseGlobalExchangeRates(SiteID) && !ECommerceSettings.UseGlobalCurrencies(SiteID))
        {
            ShowWarning(GetString("com.WrongCurrencyRateCombination"));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles saving of configuration.
    /// </summary>
    protected override void SaveChanges()
    {
        SettingsGroupViewer.SaveChanges();
    }

    #endregion
}