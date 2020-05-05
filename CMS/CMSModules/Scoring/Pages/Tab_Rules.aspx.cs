using System;

using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;


/// <summary>
/// Lists all rules that are connected to score given in scoreid query parameter. 
/// </summary>
[EditedObject(ScoreInfo.OBJECT_TYPE, "scoreid")]
[Action(0, "om.score.newrule", "Tab_Rules_Edit.aspx?ScoreID={?ScoreID?}")]
[UIElement(ModuleName.SCORING, "Scoring.Rules")]
public partial class CMSModules_Scoring_Pages_Tab_Rules : CMSScorePage
{
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Scoring", null, "menu");
        int scoreId = QueryHelper.GetInteger("scoreid", 0);

        if (scoreId == 0)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        listElem.ScoreId = scoreId;
        listElem.EditActionUrl = string.Format("Tab_Rules_Edit.aspx?scoreid={0}&ruleId={{0}}", scoreId);
        listElem.ModuleNameForPermissionCheck = ScoreInfo.TYPEINFO.ModuleName;
    }
}