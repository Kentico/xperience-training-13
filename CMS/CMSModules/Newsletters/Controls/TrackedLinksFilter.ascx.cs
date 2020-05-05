using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Newsletters_Controls_TrackedLinksFilter : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            // Get condition from link part of the filter
            return GetWhereCondition();
        }
    }


    /// <summary>
    /// Gets or sets issue ID of or to variant selector. Used for A/B test issues.
    /// </summary>
    public int IssueId
    {
        get
        {
            return ValidationHelper.GetInteger(fltVariants.Value, 0);
        }
        set
        {
            fltVariants.Value = value;

            // Display variant part of the filter
            plcVariants.Visible = fltVariants.Visible = true;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets complex where condition for link fields.
    /// </summary>
    protected string GetWhereCondition()
    {
        // Get condition from link filter fields
        string linkCondition = fltLink.GetCondition();
        string descriptionCondition = fltDescription.GetCondition();

        // Prepare base condition for link target and description
        return SqlHelper.AddWhereCondition(linkCondition, descriptionCondition);
    }

    #endregion
}