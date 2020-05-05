using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_Emails : CMSEcommerceStoreSettingsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI element
        var elementName = IsMultiStoreConfiguration ? "Tools.Ecommerce.EmailSettings" : "Configuration.Settings.Emails";
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, elementName);

        // Set up header
        CurrentMaster.HeaderActions.AddAction(new SaveAction());
        CurrentMaster.HeaderActions.ActionPerformed += StoreSettingsActions_ActionPerformed;

        // Assign category, group and site ID
        SettingsGroupViewer.CategoryName = "CMS.ECommerce";
        SettingsGroupViewer.Where = "CategoryName = N'CMS.Ecommerce.Emails'";
        SettingsGroupViewer.SiteID = (IsMultiStoreConfiguration) ? SiteID : SiteContext.CurrentSiteID;
    }


    /// <summary>
    /// Handles saving of configuration.
    /// </summary>
    protected override void SaveChanges()
    {
        SettingsGroupViewer.SaveChanges();
    }
}