using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("Currency_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Currencies_Currency_List : CMSCurrenciesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        var actions = CurrentMaster.HeaderActions;

        // New item action
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("Currency_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("Currency_Edit.aspx?siteId=" + SiteID)
        });

        gridElem.OnAction += gridElem_OnAction;

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(gridElem.ObjectType);

        // Filter records by site
        gridElem.WhereCondition = InitSiteWhereCondition("CurrencySiteID").ToString(true);

        gridElem.RememberStateByParam = GetGridRememberStateParam();
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("Currency_Edit.aspx?currencyid=" + Convert.ToString(actionArgument) + "&siteId=" + SiteID));
        }
        else if (actionName == "delete")
        {
            // Check permissions
            CheckConfigurationModification();

            int currencyId = ValidationHelper.GetInteger(actionArgument, 0);
            var currency = CurrencyInfo.Provider.Get(currencyId);

            if (currency != null)
            {
                if (currency.Generalized.CheckDependencies())
                {
                    // Show error message
                    ShowError(EcommerceUIHelper.GetDependencyMessage(currency));

                    return;
                }

                // An attempt to delete main currency
                if (currency.CurrencyIsMain)
                {
                    // Show error message
                    ShowError(string.Format(GetString("com.deletemaincurrencyerror"), HTMLHelper.HTMLEncode(currency.CurrencyDisplayName)));

                    return;
                }

                // Delete CurrencyInfo object from database
                CurrencyInfo.Provider.Delete(currency);
            }
        }
    }
}
