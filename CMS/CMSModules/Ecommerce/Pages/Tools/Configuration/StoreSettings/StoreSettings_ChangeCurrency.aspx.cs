using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("StoreSettings_ChangeCurrency.ChangeCurrencyTitle")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_ChangeCurrency : CMSEcommerceModalPage
{
    private CurrencyInfo mainCurrency;
    private int editedSiteId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register JQuery
        ScriptHelper.RegisterJQuery(this);

        // Check permissions (only global admin can see this dialog)
        var user = MembershipContext.AuthenticatedUser;
        if (!user.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            RedirectToAccessDenied(GetString("StoreSettings_ChangeCurrency.AccessDenied"));
        }

        int siteId = QueryHelper.GetInteger("siteId", SiteContext.CurrentSiteID);
        if (user.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            editedSiteId = siteId <= 0 ? 0 : siteId;
        }
        else
        {
            editedSiteId = SiteContext.CurrentSiteID;
        }

        mainCurrency = CurrencyInfoProvider.GetMainCurrency(editedSiteId);
        if (mainCurrency != null)
        {
            lblOldMainCurrency.Text = HTMLHelper.HTMLEncode(mainCurrency.CurrencyDisplayName);
        }
        else
        {
            plcOldCurrency.Visible = false;
        }

        // Load the UI
        CurrentMaster.Page.Title = GetString("StoreSettings_ChangeCurrency.ChangeCurrencyTitle");
        btnOk.Text = GetString("General.saveandclose");
        currencyElem.AddSelectRecord = true;
        currencyElem.SiteID = editedSiteId;

        if (!RequestHelper.IsPostBack())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                // Refresh the page
                ltlScript.Text = ScriptHelper.GetScript(@"
var loc = wopener.location;
if(!(/currencysaved=1/.test(loc))) {
    if(!(/\?/.test(loc))) {
        loc += '?currencysaved=1';
    } else {
        loc += '&currencysaved=1';
    }
}
wopener.location.replace(loc); CloseDialog();");

                ShowChangesSaved();
            }
        }
    }


    /// <summary>
    /// Changes the selected prices and other object fields.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        int newCurrencyId = currencyElem.SelectedID;

        // Get the new main currency
        var newCurrency = CurrencyInfo.Provider.Get(newCurrencyId);
        if (newCurrency != null)
        {
            // Only select new main currency when no old main currency specified
            if (mainCurrency == null)
            {
                newCurrency.CurrencyIsMain = true;
                newCurrency.Update();

                // Refresh the page
                URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "saved", "1"));

                return;
            }

            // Change main currency
            CurrencyInfoProvider.ChangeMainCurrency(editedSiteId, newCurrencyId);

            // Refresh the page
            URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "saved", "1"));
        }
        else
        {
            ShowError(GetString("StoreSettings_ChangeCurrency.NoNewMainCurrency"));
        }
    }
}
