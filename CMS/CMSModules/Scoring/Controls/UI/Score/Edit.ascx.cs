using System;

using CMS.ContactManagement;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Scoring_Controls_UI_Score_Edit : CMSAdminEditControl
{
    #region "Events"

    /// <summary>
    /// OnLoad event handler.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        EditForm.RedirectUrlAfterCreate = URLHelper.AppendQuery(UIContextHelper.GetElementUrl("CMS.Scoring", "ScoringProperties"), "displayTitle=0&tabname=Scoring.General&objectid={%EditedObject.ID%}&saved=1");
    }


    /// <summary>
    /// OnAfterValidate event handler.
    /// </summary>
    protected void EditForm_OnAfterValidate(object sender, EventArgs e)
    {
        // Both e-mail and score should be filled or both should be empty
        bool sendAtScoreEmpty = (string.Empty == ValidationHelper.GetString(EditForm.GetFieldValue("ScoreEmailAtScore"), string.Empty));
        bool emailNotificationEmpty = (string.Empty == ValidationHelper.GetString(EditForm.GetFieldValue("ScoreNotificationEmail"), string.Empty));

        if (sendAtScoreEmpty != emailNotificationEmpty)
        {
            ShowError(GetString("om.score.requiredemailandscore"));
            EditForm.StopProcessing = true;
        }
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        ScoreInfo score = EditForm.EditedObject as ScoreInfo;
        if (score != null)
        {
            if (score.ScoreEnabled)
            {
                // If score got changed from disabled to enabled, notify user that the score is not up to date
                if (!ValidationHelper.GetBoolean(score.GetOriginalValue("ScoreEnabled"), false))
                {
                    // Set its status to recalculation required
                    score.ScoreStatus = ScoreStatusEnum.RecalculationRequired;

                    string recalcRequired = GetString("om.score.recalcrequired");
                    string recalcRequiredDetails = GetString("om.score.recalcrequired.details");

                    ShowWarning(recalcRequired, recalcRequiredDetails, recalcRequired);
                }
            }
            else
            {
                // If score is disabled, score never gets automatically recalculated, so set its status to new - recalculation is always required
                score.ScoreStatus = ScoreStatusEnum.RecalculationRequired;
            }

            if (score.ScoreID == 0)
            {
                score.ScoreStatus = ScoreStatusEnum.Ready;
            }
        }
    }

    #endregion
}