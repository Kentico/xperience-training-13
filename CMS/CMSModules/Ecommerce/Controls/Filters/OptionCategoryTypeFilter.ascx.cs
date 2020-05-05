using System;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Ecommerce;
using CMS.Helpers;


public partial class CMSModules_Ecommerce_Controls_Filters_OptionCategoryTypeFilter : CMSAbstractDataFilterControl
{
    #region "Constants"

    protected const string FILTER_ALL = "all";
    protected const string FILTER_PRODUCTS = "PRODUCTS";
    protected const string FILTER_ATTRIBUTE = "ATTRIBUTE";
    protected const string FILTER_TEXT = "TEXT";

    #endregion


    #region "Properties"

    public override string WhereCondition
    {
        get
        {
            return GetWhereCondition();
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Fill category type selector
        InitOptions();
    }

    #endregion


    #region "Methods"

    public string GetWhereCondition()
    {
        switch (drpCategoryType.SelectedValue)
        {
            case FILTER_ALL:
                return string.Empty;

            case FILTER_ATTRIBUTE:
            case FILTER_TEXT:
                return "(CategoryType=N'" + drpCategoryType.SelectedValue + "')";

            case FILTER_PRODUCTS:
            default:
                return "((CategoryType=N'" + FILTER_PRODUCTS + "') OR (CategoryType IS NULL))";
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpCategoryType.SelectedValue = FILTER_ALL;
    }

    #endregion


    #region "Initialization"

    /// <summary>
    /// Inits the list of category types plus option ALL.
    /// </summary>
    private void InitOptions()
    {
        // Clear items
        if (drpCategoryType.Items.Count == 0)
        {
            // Add items
            drpCategoryType.Items.Add(new ListItem(GetString("general.selectall"), FILTER_ALL));
            AddItem(OptionCategoryTypeEnum.Products);
            AddItem(OptionCategoryTypeEnum.Attribute);
            AddItem(OptionCategoryTypeEnum.Text);
        }
    }


    /// <summary>
    ///  Adds specific type of category to the list.
    /// </summary>
    /// <param name="categType">Category type</param>
    private void AddItem(OptionCategoryTypeEnum categType)
    {
        drpCategoryType.Items.Add(new ListItem(categType.ToLocalizedString("com.optioncategorytype"), categType.ToStringRepresentation()));
    }

    #endregion
}