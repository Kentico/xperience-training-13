using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("PaymentOption_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_PaymentOptions_PaymentOption_List : CMSPaymentMethodsPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        HandleGridsSiteIDColumn(UniGrid);
        UniGrid.RememberStateByParam = GetGridRememberStateParam();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string newElementName;

        if (IsMultiStoreConfiguration)
        {
            newElementName = "new.Configuration.GlobalPaymentOption";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.GlobalPaymentMethods");
        }
        else
        {
            newElementName = "new.Configuration.PaymentOption";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.PaymentMethods");
        }

        // Header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("PaymentOption_List.NewItemCaption"),
            RedirectUrl = GetRedirectURL(newElementName),
        });

        UniGrid.WhereCondition = InitSiteWhereCondition("PaymentOptionSiteID").ToString(true);
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            var editElementName = IsMultiStoreConfiguration ? "edit.configuration.GlobalPaymentoption" : "edit.configuration.paymentoption";
            URLHelper.Redirect(UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, editElementName, false, ValidationHelper.GetInteger(actionArgument, 0)));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Generates redirection url with query string parameters.
    /// </summary>
    /// <param name="uiElementName">Name of ui element to redirect to.</param>
    private string GetRedirectURL(string uiElementName)
    {
        string url = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, uiElementName, false);
        // Only global object can be created from site manager       
        if (IsMultiStoreConfiguration)
        {
            url = URLHelper.AddParameterToUrl(url, "siteid", SpecialFieldValue.GLOBAL.ToString());
        }

        return url;
    }

    #endregion
}