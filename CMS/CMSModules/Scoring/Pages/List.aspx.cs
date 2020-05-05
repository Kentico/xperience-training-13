using System;
using System.Web.Services;

using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;


[Action(0, "om.score.new", "New.aspx")]
[Title("om.score.list")]
[UIElement(ModuleName.SCORING, "Scoring")]
public partial class CMSModules_Scoring_Pages_List : CMSScorePage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Scoring", null, "menu");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Check permissions to create new record
        CurrentMaster.HeaderActions.Enabled = CurrentUser.IsAuthorizedPerResource("cms.scoring", "modify", CurrentSiteName);
    }


    /// <summary>
    /// Returns status of score of given ID.
    /// </summary>
    /// <param name="scoreID">ID of score to get status from</param>
    /// <returns>String representation of score's status</returns>
    [WebMethod]
    public static string GetScoreStatus(int scoreID)
    {
        var score = ScoreInfo.Provider.Get(scoreID);
        return score != null ? score.ScoreStatus.ToString() : null;
    }

    #endregion
}