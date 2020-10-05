using CMS.Activities;
using CMS.Activities.Web.UI;

public partial class CMSModules_Activities_Controls_UI_ActivityDetails_PageVisit : ActivityDetail
{
    #region "Methods"

    /// <summary>
    /// Loads activity info data.
    /// </summary>
    /// <param name="ai">ActivityInfo object</param>
    public override bool LoadData(ActivityInfo ai)
    {
        if (ai == null)
        {
            return false;
        }

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.LANDING_PAGE:
            case PredefinedActivityType.PAGE_VISIT:
                break;

            default:
                return false;
        }

        // Loads data to grid
        ucDetails.AddRow("om.activitydetails.abvariant", ai.ActivityABVariantName);
        return ucDetails.IsDataLoaded;
    }

    #endregion
}