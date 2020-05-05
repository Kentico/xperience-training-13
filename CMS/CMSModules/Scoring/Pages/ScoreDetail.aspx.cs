using System;
using System.Data;

using CMS.ContactManagement;
using CMS.ContactManagement.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Core.Internal;


/// <summary>
/// Displays a table of score details.
/// </summary>
[Title("om.score.details.title")]
[Security(Resource = "CMS.Scoring", Permission = "Read")]
[CheckLicence(FeatureEnum.LeadScoring)]
[CheckLicence(FeatureEnum.FullContactManagement)]
public partial class CMSModules_Scoring_Pages_ScoreDetail : CMSModalPage
{
    #region "Variables"

    private int contactId;

    // Default page size 15
    private const int PAGESIZE = 15;

    #endregion


    #region "Methods"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get contact ID
        contactId = QueryHelper.GetInteger("contactId", 0);
        if (contactId == 0)
        {
            RequestHelper.EndResponse();
        }

        // Check contact's existence
        ContactInfo contact = ContactInfo.Provider.Get(contactId);
        EditedObject = contact;

		// Initialize unigrid
		var dateTimeService = Service.Resolve<IDateTimeNowService>();
	    var whereCondition = new WhereCondition().WhereEquals("ContactID", contactId)
	                                             .WhereEquals("ScoreID", QueryHelper.GetInteger("scoreid", 0))
	                                             .WhereNotExpired(dateTimeService.GetDateTimeNow());

	    gridElem.WhereCondition = whereCondition.ToString(true);
        gridElem.Pager.DefaultPageSize = PAGESIZE;
        gridElem.Pager.ShowPageSize = false;
        gridElem.FilterLimit = PAGESIZE;
        gridElem.OnExternalDataBound += UniGrid_OnExternalDataBound;
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = (DataRowView)parameter;

        switch (sourceName)
        {
            case "quantity":
                // Get contact's total value for the rule
                int value = DataHelper.GetIntValue(row.Row, "Value");
                // Get rule value
                int ruleValue = DataHelper.GetIntValue(row.Row, "RuleValue");
                if (value == 0 || ruleValue == 0)
                {
                    return 0;
                }

                // Display number of recurrences of the rule evaluation
                return value / ruleValue;

            default:
                return parameter;
        }
    }

    #endregion
}