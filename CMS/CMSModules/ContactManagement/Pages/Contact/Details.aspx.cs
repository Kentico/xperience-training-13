using System;

using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.Internal;
using CMS.ContactManagement;
using CMS.Core;
using CMS.UIControls;

[EditedObject(ContactInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "ContactProfile", false, true)]
public partial class CMSModules_ContactManagement_Pages_Contact_Details : CMSPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

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
            ActivitiesExist = DoActivitiesExistForContact(contact),
            DisplayGroupMemberships = true,
            DisplayNotes = true,
            DisplayContactInformations = true,
            DisplayEditButton = true
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