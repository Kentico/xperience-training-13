using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Newsletters_Controls_OpenedByFilter : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            string whereCond = fltEmail.GetCondition();
             
            if (ShowDateFilter)
            {
                // Get condition from date part of the filter
                whereCond = SqlHelper.AddWhereCondition(whereCond, fltOpenedBetween.GetCondition());
            }
            if (plcVariants.Visible)
            {
                // Get condition from variant part of the filter
                whereCond = SqlHelper.AddWhereCondition(whereCond, fltVariants.GetWhereCondition());
            }

            return whereCond;
        }
    }


    /// <summary>
    /// Indicates if date part of the filter should be visible.
    /// </summary>
    public bool ShowDateFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets column name of email filter.
    /// </summary>
    public string EmailColumn
    {
        get
        {
            return fltEmail.Column;
        }
        set
        {
            fltEmail.Column = value;
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


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Display/hide date part of the filter
        plcDate.Visible = ShowDateFilter;
    }

    #endregion
}