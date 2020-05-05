using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Macros_Controls_UI_MacroRule_CategoryFilter : CMSAbstractBaseFilterControl
{
    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return GetWhere();
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("RuleCategory", drpOptions.SelectedIndex);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        EnsureChildControls();

        base.RestoreFilterState(state);
        drpOptions.SelectedIndex = state.GetInt32("RuleCategory");
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpOptions.SelectedIndex = 0;
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (drpOptions.Items.Count == 0)
        {
            drpOptions.Items.Add(new ListItem(GetString("macros.macrorule.onlycurrentcategory"), "0"));
            drpOptions.Items.Add(new ListItem(GetString("macros.macrorule.currentandglobalcategory"), "1"));
        }
    }


    /// <summary>
    /// Returns correct WHERE condition.
    /// </summary>
    private string GetWhere()
    {
        UniGrid parent = ControlsHelper.GetParentControl(this, typeof(UniGrid)) as UniGrid;
        if (parent != null)
        {
            string resourceName = ValidationHelper.GetString(parent.GetValue("ResourceName"), "");
            if (resourceName == String.Empty)
            {
                var uiContext = UIContextHelper.GetUIContext(this);
                // Check special parameter first
                resourceName = ValidationHelper.GetString(uiContext["MacroRuleResourceName"], String.Empty);
                if (resourceName == String.Empty)
                {
                    // If not set, use element's resource name
                    resourceName = ValidationHelper.GetString(uiContext["resourcename"], String.Empty);
                }
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                string where = "MacroRuleResourceName = N'" + SqlHelper.GetSafeQueryString(resourceName, false) + "'";

                // Show also global rules for global administrator if requested
                if (drpOptions.SelectedIndex == 1)
                {
                    where = SqlHelper.AddWhereCondition(where, "MacroRuleResourceName = '' OR MacroRuleResourceName IS NULL", "OR");
                }
                return where;
            }
        }

        // Only global rules should be displayed
        return "MacroRuleResourceName = '' OR MacroRuleResourceName IS NULL";
    }
}