using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_General : CMSEcommerceStoreSettingsPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        GlobalObjectsKeyName = ECommerceSettings.USE_GLOBAL_CURRENCIES;

        // Check UI element
        var elementName = IsMultiStoreConfiguration ? "Tools.Ecommerce.GeneralSettings" : "Configuration.Settings.General";
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, elementName);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeControls();

        // Register scripts for currency change
        ScriptHelper.RegisterDialogScript(this);

        // Append tooltip
        string tooltip = GetString("com.maincurrencytooltip");
        ScriptHelper.AppendTooltip(lblCurrentMainCurrency, tooltip, null);
        icoHelp.ToolTip = tooltip;

        // Display info message when main currency saved
        if (QueryHelper.GetBoolean("currencysaved", false))
        {
            ShowInformation(GetString("com.storesettings.maincurrencychanged"));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets current main currency.
    /// </summary>
    private void GetCurrentMainCurrency()
    {
        int mainCurrencyId = -1;

        CurrencyInfo mainCurrency = CurrencyInfoProvider.GetMainCurrency(ConfiguredSiteID);
        if (mainCurrency != null)
        {
            mainCurrencyId = mainCurrency.CurrencyID;
            lblMainCurrency.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(mainCurrency.CurrencyDisplayName));
        }
        else
        {
            lblMainCurrency.Text = GetString("general.na");
        }

        DataSet ds = CurrencyInfo.Provider.Get()
                                         .TopN(1)
                                         .WhereTrue("CurrencyEnabled")
                                         .Where("CurrencyID", QueryOperator.NotEquals, mainCurrencyId)
                                         .OnSite(ConfiguredSiteID);

        // When there is no choice
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            // Disable "change main currency" button
            btnChangeCurrency.Enabled = false;
        }
    }


    /// <summary>
    /// Initialization of form controls.
    /// </summary>
    private void InitializeControls()
    {
        CurrentMaster.HeaderActions.AddAction(new SaveAction());
        CurrentMaster.HeaderActions.ActionPerformed += StoreSettingsActions_ActionPerformed;

        // Assign category, group and site ID
        SettingsGroupViewer.CategoryName = "CMS.ECommerce";
        SettingsGroupViewer.Where = "CategoryName IN (N'CMS.Ecommerce.UnregisteredCustomers', N'CMS.Ecommerce.Accounts', N'CMS.Ecommerce.Units', N'CMS.Ecommerce.Taxes', N'CMS.Ecommerce.RoundingOptions', N'CMS.Ecommerce.ProductsUI', N'CMS.Ecommerce.ProductProperties', N'CMS.ECommerce.Pages', N'CMS.Ecommerce.Pricing', N'CMS.Ecommerce.Invoice', N'CMS.Ecommerce.ShoppingCart')";
        SettingsGroupViewer.SiteID = (IsMultiStoreConfiguration) ? SiteID : SiteContext.CurrentSiteID;

        GetCurrentMainCurrency();

        // Main currency can be changed only by global administrator
        btnChangeCurrency.Enabled = IsGlobalStoreAdmin;

        string dialogUrl = UrlResolver.ResolveUrl("StoreSettings_ChangeCurrency.aspx");
        dialogUrl = URLHelper.AddParameterToUrl(dialogUrl, "siteid", ConfiguredSiteID.ToString());
        URLHelper.EnsureHashToQueryParameters(dialogUrl);
        
        btnChangeCurrency.OnClientClick = ScriptHelper.GetModalDialogScript(dialogUrl, "ChangeMainCurrency", 800, 250);
    }


    /// <summary>
    /// Handles saving of configuration.
    /// </summary>
    protected override void SaveChanges()
    {
        SettingsGroupViewer.SaveChanges();
    }

    #endregion
}
