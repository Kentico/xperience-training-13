using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Ecommerce_Controls_Filters_OptionCategoryNameFilter : CMSAbstractDataFilterControl
{
    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition().ToString(true);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!RequestHelper.IsPostBack())
        {
            Reload();
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    protected void Reload()
    {
        filter.Items.Clear();
        ControlsHelper.FillListWithTextSqlOperators(filter);
    }


    /// <summary>
    /// Generates WHERE condition.
    /// </summary>
    private WhereCondition GenerateWhereCondition()
    {
        var where = new WhereCondition();
        string name = txtName.Text;

        if (String.IsNullOrEmpty(name))
        {
            return where;
        }

        // Get filter operator (LIKE, NOT LIKE, =, !=)
        switch (filter.SelectedValue)
        {
            default:
                where.Where(w => w.WhereContains("CategoryDisplayName", name).Or().WhereContains("CategoryLiveSiteDisplayName", name));
                break;
            case WhereBuilder.EQUAL:
                where.Where(w => w.WhereEquals("CategoryDisplayName", name).Or().WhereEquals("CategoryLiveSiteDisplayName", name));
                break;
            case WhereBuilder.NOT_LIKE:
                where.Where(w => w.WhereNotContains("CategoryDisplayName", name).Or().WhereNull("CategoryDisplayName"))
                     .And()
                     .Where(w => w.WhereNotContains("CategoryLiveSiteDisplayName", name).Or().WhereNull("CategoryLiveSiteDisplayName"));
                break;
            case WhereBuilder.NOT_EQUAL:
                where.Where(w => w.WhereNotEquals("CategoryDisplayName", name).Or().WhereNull("CategoryDisplayName"))
                     .And()
                     .Where(w => w.WhereNotEquals("CategoryLiveSiteDisplayName", name).Or().WhereNull("CategoryLiveSiteDisplayName"));
                break;
        }

        return where;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("condition", filter.SelectedValue);
        state.AddValue("categoryName", txtName.Text);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        filter.SelectedValue = state.GetString("condition");
        txtName.Text = state.GetString("categoryName");
    }


    /// <summary>
    /// Resets the filter settings.
    /// </summary>
    public override void ResetFilter()
    {
        filter.SelectedIndex = 0;
        txtName.Text = String.Empty;
    }

    #endregion

}
