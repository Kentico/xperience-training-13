using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("TaxClass_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_List : CMSTaxClassesPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.RememberStateByParam = GetGridRememberStateParam();

        if (UseGlobalObjects && ECommerceContext.IsExchangeRateFromGlobalMainCurrencyMissing)
        {
            ShowError(GetString("com.NeedExchangeRateFromGlobal"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        var actions = CurrentMaster.HeaderActions;

        string newElementName;
        if (IsMultiStoreConfiguration)
        {
            newElementName = "new.configuration.globaltaxclass";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Tools.Ecommerce.TaxClasses");
        }
        else
        {
            newElementName = "new.configuration.taxclass";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.TaxClasses");
        }

        // Prepare the new tax class header element
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("TaxClass_List.NewItemCaption"),
            RedirectUrl = GetRedirectURL(newElementName),
        });

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(UniGrid.ObjectType);

        // Filter records by site
        UniGrid.WhereCondition = InitSiteWhereCondition("TaxClassSiteID").ToString(true);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            var editElementName = IsMultiStoreConfiguration ? "edit.Tools.Ecommerce.TaxProperties" : "edit.Configuration.TaxClasses";
            string url = GetRedirectURL(editElementName);
            url = URLHelper.AddParameterToUrl(url, "objectid", ValidationHelper.GetInteger(actionArgument, 0).ToString("0"));

            URLHelper.Redirect(UrlResolver.ResolveUrl(url));
        }
    }


    /// <summary>
    /// Generates redirection url with query string parameters (siteid working with site selector values and IsMultiStoreConfiguration).
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

        url = URLHelper.AddParameterToUrl(url, "issitemanager", IsMultiStoreConfiguration.ToString());

        return url;
    }
}