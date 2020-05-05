using System;
using System.Linq;

using CMS.ContactManagement;
using CMS.Personas;
using CMS.SiteProvider;
using CMS.UIControls;


/// <summary>
/// Displays number of contacts in persona and ratio of that to the number of all contacts.
/// Used on Persona Overview (General) and Contacts tabs.
/// </summary>
public partial class CMSModules_Personas_Controls_PersonaContactCounter : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check that the control is included in CMSPage (otherwise an exception is thrown on the Design tab)
        var page = Page as CMSPage;
        if (page == null)
        {
            return;
        }

        var persona = UIContext.EditedObjectParent as PersonaInfo;
        if (persona == null)
        {
            return;
        }

        // Display number of contacts in persona
        int personaContactCount = PersonasFactory.GetPersonaService().GetContactsForPersona(persona).Count;
        lblCount.InnerText = String.Format(GetString("personas.ui.contactcount"), personaContactCount);

        // Display ratio of the number of contacts in persona to the number of all contacts
        int totalContactCount = ContactInfo.Provider.Get().Count;

        double ratio = (totalContactCount == 0) ? 0 : (double)personaContactCount / totalContactCount * 100;
        lblRatio.InnerText = String.Format(GetString("personas.ui.contactratio"), ratio);
    }
}