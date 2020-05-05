using System;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Personas;


/// <summary>
/// 'Viewer' form contol which displays persona information. Persona's picture and name is displayed. ID of persona should be set to Value.
/// </summary>
public partial class CMSModules_Personas_Controls_PersonaInformation : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get;
        set;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        var personaInfo = PersonaInfo.Provider.Get(Value.ToInteger(0));

        string personaPictureUrl;

        if (personaInfo == null)
        {
            lblPersonaName.Text = GetString("general.empty");
            personaPictureUrl = PersonasFactory.GetPersonaPictureUrlCreator().CreateDefaultPersonaPictureUrl(32);
            imgPersonaImage.AlternateText = GetString("general.empty");
        }
        else
        {
            divPersonaInfoContent.Attributes["title"] = personaInfo.PersonaDescription;
            lblPersonaName.Text = HTMLHelper.HTMLEncode(personaInfo.PersonaDisplayName);
            personaPictureUrl = PersonasFactory.GetPersonaPictureUrlCreator().CreatePersonaPictureUrl(personaInfo, 32);
            imgPersonaImage.AlternateText = HTMLHelper.HTMLEncode(personaInfo.PersonaDisplayName);
        }

        if (personaPictureUrl != null)
        {
            imgPersonaImage.ImageUrl = personaPictureUrl;
        }
        else
        {
            imgPersonaImage.Visible = false;
        }
    }
}