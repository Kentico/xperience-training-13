using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_EventLog_Controls_EventFilter : CMSAbstractBaseFilterControl
{
    #region "Properties"

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
    /// Gets button used to toggle filter's advanced mode.
    /// </summary>
    public override IButtonControl ToggleAdvancedModeButton
    {
        get
        {
            return lnkToggleFilter;
        }
    }

    #endregion


    #region "Control methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!RequestHelper.IsPostBack())
        {
            drpEventLogType.Value = QueryHelper.GetString("type", null);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitializeForm();
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
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies the filter.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }

        ShowFilterElements();
    }


    protected override void ToggleAdvancedModeButton_Click(object sender, EventArgs e)
    {
        IsAdvancedMode = !IsAdvancedMode;
        ShowFilterElements();

        base.ToggleAdvancedModeButton_Click(sender, e);
    }

    #endregion


    #region "UI methods"

    /// <summary>
    /// Shows/hides all elements for advanced or simple mode.
    /// </summary>
    private void ShowFilterElements()
    {
        plcAdvancedSearch.Visible = IsAdvancedMode;
    }


    /// <summary>
    /// Initializes the layout of the form.
    /// </summary>
    private void InitializeForm()
    {
        // General UI
        ShowFilterElements();

        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null && grid.RememberState)
        {
            grid.RememberDefaultState = true;
        }
        else
        {
            btnReset.Visible = false;
        }
    }


    private void EnsureCorrectToggleFilterText()
    {
        lnkToggleFilter.Text = IsAdvancedMode ? GetString("general.displaysimplefilter") : GetString("general.displayadvancedfilter");
    }

    #endregion


    #region "Search methods - where condition"

    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string whereCond = "";

        // Create WHERE condition for basic filter
        string eventType = ValidationHelper.GetString(drpEventLogType.Value, null);
        if (!String.IsNullOrEmpty(eventType))
        {
            whereCond = "EventType='" + eventType + "'";
        }

        whereCond = SqlHelper.AddWhereCondition(whereCond, fltSource.GetCondition());
        whereCond = SqlHelper.AddWhereCondition(whereCond, fltEventCode.GetCondition());
        whereCond = SqlHelper.AddWhereCondition(whereCond, fltTimeBetween.GetCondition());

        // Create WHERE condition for advanced filter (id needed)
        if (IsAdvancedMode)
        {
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltUserName.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltIPAddress.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltDocumentName.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltMachineName.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltEventURL.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltEventURLRef.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltDescription.GetCondition());
            whereCond = SqlHelper.AddWhereCondition(whereCond, fltUserAgent.GetCondition());
        }

        return whereCond;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("AdvancedMode", IsAdvancedMode);
        state.AddValue("TimeBetweenFrom", fltTimeBetween.ValueFromTime);
        state.AddValue("TimeBetweenTo", fltTimeBetween.ValueToTime);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        IsAdvancedMode = state.GetBoolean("AdvancedMode");
        ShowFilterElements();

        fltTimeBetween.ValueFromTime = state.GetDateTime("TimeBetweenFrom");
        fltTimeBetween.ValueToTime = state.GetDateTime("TimeBetweenTo");
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpEventLogType.Value = String.Empty;
        fltEventCode.ResetFilter();
        fltSource.ResetFilter();
        fltUserName.ResetFilter();
        fltIPAddress.ResetFilter();
        fltDocumentName.ResetFilter();
        fltMachineName.ResetFilter();
        fltEventURL.ResetFilter();
        fltEventURLRef.ResetFilter();
        fltDescription.ResetFilter();
        fltUserAgent.ResetFilter();
        fltTimeBetween.Clear();
    }

    #endregion
}