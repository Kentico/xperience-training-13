using System;

using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;


/// <summary>
/// New/edit rule details page. 
/// </summary>
// Edited object
[EditedObject(RuleInfo.OBJECT_TYPE, "ruleid")]
[ParentObject(ScoreInfo.OBJECT_TYPE, "scoreId")]
// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "om.score.rulelist", "~/CMSModules/Scoring/Pages/Tab_Rules.aspx?scoreId={?scoreId?}", null)]
[Breadcrumb(1, "om.score.newrule", NewObject = true)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
// Help
[Help("scoringrule_new", "helptopic")]
[UIElement(ModuleName.SCORING, "Scoring.EditRule")]
public partial class CMSModules_Scoring_Pages_Tab_Rules_Edit : CMSScorePage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        int scoreId = QueryHelper.GetInteger("scoreid", 0);
        var rule = EditedObject as RuleInfo;

        // Check if rule is child of score
        // If creating new rule, it is allowed for the rule to be null
        if ((rule != null) && (rule.RuleScoreID != scoreId))
        {
            AccessDenied();
        }

        // Check if score is visible in scoring module
        var score = ScoreInfo.Provider.Get(scoreId);
        if (score != null && score.ScorePersonaID > 0)
        {
            AccessDenied();
        }

        editElem.ScoreId = scoreId;
        editElem.RedirectUrlAfterCreate = string.Format("Tab_Rules_Edit.aspx?ruleid={{%EditedObject.ID%}}&scoreid={0}&saved=1", scoreId);
    }


    /// <summary>
    /// Redirects to access denied page if one of the request query parameter is missing or invalid.
    /// </summary>
    private void AccessDenied()
    {
        RedirectToAccessDenied(GetString("general.invalidparameters"));
    }
}