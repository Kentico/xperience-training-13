using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_Filters_ContactStatusFilter : CMSAbstractBaseFilterControl
{
    #region "Properties"
    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return GenerateWhereCondition();
        }
    }

    #endregion


    #region "Page events"


    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to default state.
    /// </summary>
    public override void ResetFilter()
    {
        contactStatusSelector.Value = UniSelector.US_ALL_RECORDS;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string whereCond = String.Empty;

        // Create WHERE condition for basic filter
        int contactStatus = ValidationHelper.GetInteger(contactStatusSelector.Value, -1);
        if (contactStatusSelector.Value == null)
        {
            whereCond = "ContactStatusID IS NULL";
        }
        else if (contactStatus > 0)
        {
            whereCond = "ContactStatusID = " + contactStatus;
        }

        return whereCond;
    }

    #endregion
}