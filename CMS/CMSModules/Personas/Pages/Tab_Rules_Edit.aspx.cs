using System;

using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Personas;
using CMS.UIControls;


/// <summary>
/// New/edit rule details page. PersonaId query parameter has to be set in order to the page is able to verify RuleID belongs under Persona score object.
/// If PersonaID and RuleID does not match (RuleScoreID != Persona related score id), page is redirected to Access denied page.
/// </summary>
// Edited object
[EditedObject(RuleInfo.OBJECT_TYPE, "ruleid")]
[ParentObject(PersonaInfo.OBJECT_TYPE, "personaid")]
// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "om.score.rulelist", "~/CMSModules/Personas/Pages/Tab_Rules.aspx?personaid={?personaid?}", null)]
[Breadcrumb(1, "om.score.newrule", NewObject = true)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
// Help
[Help("scoringrule_new", "helptopic")]
[UIElement(ModuleName.PERSONAS, "Personas.Rules")]
public partial class CMSModules_Personas_Pages_Tab_Rules_Edit : CMSDeskPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Request to this page has to have all three following query parameters set
        int personaId = QueryHelper.GetInteger("personaid", 0);
        
        // Persona has to be connected with current score
        var personaInfo = PersonaInfo.Provider.Get(personaId);
        if (personaInfo == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!personaInfo.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(personaInfo.TypeInfo.ModuleName, "Read");
        }
        
        RuleInfo rule = EditedObject as RuleInfo;
        var personaScoreId = personaInfo.GetRelatedScore().ScoreID;

        // Editing existing rule
        if (rule != null)
        {
            // Rule has to be connected with current score
            if ((rule.RuleScoreID != personaScoreId))
            {
                RedirectToAccessDenied(GetString("general.invalidparameters"));
            }
        }

        editElem.ResourceName = ModuleName.PERSONAS;
        editElem.ScoreId = personaScoreId;
        editElem.NewRuleUrl = string.Format("Tab_Rules_Edit.aspx?personaid={0}", personaId);
        editElem.RedirectUrlAfterCreate = string.Format("Tab_Rules_Edit.aspx?ruleid={{%EditedObject.ID%}}&personaid={0}&saved=1", personaId);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.OverrideUITexts(new CMSModules_Scoring_Controls_UI_Rule_Edit.UITexts()
        {
            DisplayNameTooltipResourceString = "personas.rule.displayname.tooltip",
            ScoreValueTooltipResourceString = "personas.rule.scorevaluecodename.tooltip",
            ScoreValueLabelResourceString = "personas.rule.scorevaluecodename.label",
            RecalculationNeededResourceString = "personas.rule.recalculationneeded"
        });
    }
}