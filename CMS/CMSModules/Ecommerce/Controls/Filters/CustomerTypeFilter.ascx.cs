using System;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_Ecommerce_Controls_Filters_CustomerTypeFilter : CMSAbstractDataFilterControl
{
    protected const string CUSTOMERS_ALL = "all";
    protected const string CUSTOMERS_ANONYMOUS = "ano";
    protected const string CUSTOMERS_REGISTERED = "reg";


    public override string WhereCondition
    {
        get
        {
            return GetWhereCondition();
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Fill customer type selector
        ddlCustomerType.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), CUSTOMERS_ALL));
        ddlCustomerType.Items.Add(new ListItem(ResHelper.GetString("general.yes"), CUSTOMERS_REGISTERED));
        ddlCustomerType.Items.Add(new ListItem(ResHelper.GetString("general.no"), CUSTOMERS_ANONYMOUS));
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }


    public string GetWhereCondition()
    {
        switch (ddlCustomerType.SelectedValue)
        {
            case CUSTOMERS_ALL:
                return String.Empty;

            case CUSTOMERS_ANONYMOUS:
                return "(CustomerUserID IS NULL)";

            case CUSTOMERS_REGISTERED:
                return "(CustomerUserID IS NOT NULL)";
        }

        return "(1=0)";
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        ddlCustomerType.SelectedValue = CUSTOMERS_ALL;
    }
}