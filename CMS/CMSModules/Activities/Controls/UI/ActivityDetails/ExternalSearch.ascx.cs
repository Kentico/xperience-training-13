using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.WebAnalytics;

public partial class CMSModules_Activities_Controls_UI_ActivityDetails_ExternalSearch : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if (ai == null)
        {
            return false;
        }

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.EXTERNAL_SEARCH:
            case PredefinedActivityType.INTERNAL_SEARCH:
                break;
            default:
                return false;
        }
        
        ucDetails.AddRow("om.activitydetails.keywords", ai.ActivityValue);
        if (ai.ActivityType == PredefinedActivityType.EXTERNAL_SEARCH)
        {
            string keyword;
            var searchEngine = SearchEngineAnalyzer.GetSearchEngineFromUrl(ai.ActivityURLReferrer, out keyword);

            string provider = searchEngine != null 
                ? searchEngine.SearchEngineDisplayName 
                : ai.ActivityURLReferrer;

            ucDetails.AddRow("om.activitydetails.provider", provider);
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}