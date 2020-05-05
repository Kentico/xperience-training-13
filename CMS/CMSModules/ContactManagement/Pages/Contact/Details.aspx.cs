using System;

using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.Internal;
using CMS.Core;
using CMS.UIControls;
using CMS.ContactManagement;


[EditedObject(ContactInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "ContactProfile", false, true)]
public partial class CMSModules_ContactManagement_Pages_Contact_Details : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ContactInfo contact = (ContactInfo)EditedObject;

        UpdateBreadcrumbs(contact);

        string moduleId = "CMS.ContactManagement/ContactProfile/build";
        var localizationProvider = Service.Resolve<IClientLocalizationProvider>();

        ScriptHelper.RegisterAngularModule(moduleId, new
        {
            Resources = localizationProvider.GetClientLocalization(moduleId),
            PersonaModuleAvailable = ModuleEntryManager.IsModuleLoaded(ModuleName.PERSONAS),
            FormModuleAvailable = ModuleEntryManager.IsModuleLoaded(ModuleName.BIZFORM),
            NewsletterModuleAvailable = ModuleEntryManager.IsModuleLoaded(ModuleName.NEWSLETTER),
            ActivitiesExist = DoActivitiesExistForContact(contact)
        });
    }


    private static bool DoActivitiesExistForContact(ContactInfo contact)
    {
        return ActivityInfo.Provider.Get().WhereEquals("ActivityContactID", contact.ContactID).Count > 0;
    }


    private void UpdateBreadcrumbs(ContactInfo contact)
    {
        ScriptHelper.RefreshTabHeader(Page, contact.ContactDescriptiveName);
    }
}