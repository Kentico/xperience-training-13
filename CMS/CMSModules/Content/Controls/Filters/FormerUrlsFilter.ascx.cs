using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Filters_FormerUrlsFilter : CMSAbstractBaseFilterControl
{
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


    /// <summary>
    /// Indicates if filter is in advanced mode.
    /// </summary>
    protected bool IsAdvancedMode
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false);
        }
        set
        {
            ViewState["IsAdvancedMode"] = value;
        }
    }


    /// <summary>
    /// Gets button used to toggle filter's advanced mode.
    /// </summary>
    public override IButtonControl ToggleAdvancedModeButton
    {
        get
        {
            return lnkToggleFilter;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ShowFilterElements();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        EnsureCorrectToggleFilterText();
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        var grid = FilteredControl as UniGrid;
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
        var grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    protected override void ToggleAdvancedModeButton_Click(object sender, EventArgs e)
    {
        IsAdvancedMode = !IsAdvancedMode;
        ShowFilterElements();

        base.ToggleAdvancedModeButton_Click(sender, e);
    }


    private void EnsureCorrectToggleFilterText()
    {
        lnkToggleFilter.Text = IsAdvancedMode ? GetString("general.displaysimplefilter") : GetString("general.displayadvancedfilter");
    }


    /// <summary>
    /// Shows/hides all elements for advanced or simple mode.
    /// </summary>
    private void ShowFilterElements()
    {
        plcAdvancedSearch.Visible = IsAdvancedMode;
    }


    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        var whereCondition = new WhereCondition();

        whereCondition = whereCondition
            .Where(fltFormerUrlPath.GetCondition());

        // Create WHERE condition for advanced filter
        if (IsAdvancedMode)
        {
            whereCondition = whereCondition
                .Where(fltModified.GetCondition())
                .Where(fltPageName.GetCondition())
                .Where(fltType.GetCondition());
        }

        return whereCondition.ToString(true);
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        fltFormerUrlPath.ResetFilter();
        fltModified.Clear();
        fltPageName.ResetFilter();
        fltType.ResetFilter();
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("AdvancedMode", IsAdvancedMode);
        state.AddValue("ToTime", fltModified.ValueToTime);
        state.AddValue("FromTime", fltModified.ValueFromTime);
        base.StoreFilterState(state);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        IsAdvancedMode = state.GetBoolean("AdvancedMode");
        fltModified.ValueFromTime = state.GetDateTime("FromTime");
        fltModified.ValueToTime = state.GetDateTime("ToTime");
    }
}