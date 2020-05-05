using System;

using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.SiteProvider;

public partial class CMSModules_Activities_Controls_UI_Activity_Filter : CMSAbstractBaseFilterControl
{
    #region "Variables"

    private bool mShowContactSelector = true;

    #endregion


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
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Determines whether contact selector is visible.
    /// </summary>
    public bool ShowContactSelector
    {
        get
        {
            return mShowContactSelector;
        }
        set
        {
            mShowContactSelector = value;
        }
    }


    /// <summary>
    /// Determines whether site selector is visible.
    /// </summary>
    public bool ShowSiteFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Activities with given site id will be filtered.
    /// If <see cref="UniSelector.US_GLOBAL_RECORD"/> is supplied, then only activities without site will be filtered.
    /// By default <see cref=SiteContext.CurrentSiteID/> is used.
    /// </summary>
    /// <remarks>
    /// If value of this property is less than 1 and does not equal to <see cref="UniSelector.US_GLOBAL_RECORD"/>, then activities are not filtered by site.
    /// </remarks>
    public override int SiteID
    {
        get;
        set;
    }

    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        drpType.AdditionalDropDownCSSClass = "DropDownFieldFilter";
        siteSelector.Value = UniSelector.US_ALL_RECORDS;

        SiteID = SiteContext.CurrentSiteID;
        ShowSiteFilter = true;

        base.OnInit(e);
        btnReset.Text = GetString("general.reset");
        btnReset.Click += btnReset_Click;
        btnFilter.Click += btnSearch_Click;
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpType.Value = null;
        fltTimeBetween.Clear();
        fltContactFirstName.ResetFilter();
        fltContactMiddleName.ResetFilter();
        fltContactLastName.ResetFilter();
        fltName.ResetFilter();
        siteSelector.Value = UniSelector.US_ALL_RECORDS;
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("FromTime", fltTimeBetween.ValueFromTime);
        state.AddValue("ToTime", fltTimeBetween.ValueToTime);
        base.StoreFilterState(state);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        fltTimeBetween.ValueFromTime = state.GetDateTime("FromTime");
        fltTimeBetween.ValueToTime = state.GetDateTime("ToTime");
    }

    #endregion


    #region "Search methods - where condition"

    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string whereCond = string.Empty;
        if (!String.IsNullOrEmpty(drpType.SelectedValue))
        {
            whereCond = "ActivityType=N'" + drpType.SelectedValue.Replace("'", "''") + "'";
        }
        
        if (ShowSiteFilter)
        {
            SiteID = ValidationHelper.GetInteger(siteSelector.Value, 0);
        }
        
        if (SiteID > 0)
        {
            whereCond = SqlHelper.AddWhereCondition(whereCond, "ActivitySiteID=" + SiteID);
        }

        if (ShowContactSelector)
        {
            // Filter by contact if contact selector is visible
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltContactFirstName.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltContactMiddleName.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltContactLastName.GetCondition());
        }

        whereCond = SqlHelper.AddWhereCondition(whereCond, fltName.GetCondition());
        whereCond = SqlHelper.AddWhereCondition(whereCond, fltTimeBetween.GetCondition());

        return whereCond;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        plcCon.Visible = ShowContactSelector;
        plcSite.Visible = ShowSiteFilter;

        if (ShowSiteFilter)
        {
            siteSelector.AdditionalDropDownCSSClass = "DropDownFieldFilter";
        }
    }


    /// <summary>
    /// Returns true if <see cref="FilteredControl"/> has it's data filtered by site.
    /// </summary>
    public bool IsUniGridFilteredBySite()
    {
        int siteId = 0;
        if (ShowSiteFilter)
        {
            siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        }

        return siteId != UniSelector.US_ALL_RECORDS;
    }

    #endregion
}