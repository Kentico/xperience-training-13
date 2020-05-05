using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Personas;
using CMS.UIControls;


/// <summary>
/// Lists all rules that are connected to persona given in personaid query parameter. Connection is performed via common score object.
/// ID of existing persona has to be set to personaid query parameter.
/// </summary>
[EditedObject(PredefinedObjectType.PERSONA, "personaid")]
[Action(0, "om.score.newrule", "Tab_Rules_Edit.aspx?personaid={?personaid?}")]
[UIElement(ModuleName.PERSONAS, "Personas.Rules")]
public partial class CMSModules_Personas_Pages_Tab_Rules : CMSDeskPage
{
    private PersonaInfo mPersona;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Scoring", null, "menu");

        mPersona = EditedObject as PersonaInfo;
        if (mPersona == null)
        {
            RedirectToInformation(GetString("general.objectnotfound"));
        }

        if (!mPersona.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(mPersona.TypeInfo.ModuleName, "Read");
        }

        listElem.ScoreId = mPersona.GetRelatedScore().ScoreID;
        listElem.EditActionUrl = string.Format("Tab_Rules_Edit.aspx?ruleId={{0}}&personaId={0}", mPersona.PersonaID);
        listElem.ModuleNameForPermissionCheck = PersonaInfo.TYPEINFO.ModuleName;

        listElem.OverrideUITexts(new CMSModules_Scoring_Controls_UI_Rule_List.UITexts
        {
            RuleValueCaptionResourceString = "personas.rule.points",
            ZeroRowsTextResourceString = "personas.rule.nodatafound",
            RecalculationNeededResourceString = "personas.rule.recalculationneeded",
            RecalculationNotNeededResourceString = "personas.rule.recalculationnotrequired",
        });

        listElem.OverrideObjectType("om.personarule");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!mPersona.PersonaEnabled)
        {
            listElem.SetRecalcuateButtonProperties(false, GetString("personas.recalculationdisabled"));
        }
    }
}