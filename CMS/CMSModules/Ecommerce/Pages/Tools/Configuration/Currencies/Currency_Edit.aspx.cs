using System;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "Currency_Edit.ItemListLink", "Currency_List.aspx?siteId={?siteId?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "Currency_Edit.NewItemCaption", NewObject = true)]
// Edited object
[EditedObject(CurrencyInfo.OBJECT_TYPE, "currencyid")]
// Title
[Title("Currency_Edit.HeaderCaption", ExistingObject = true)]
[Title("Currency_New.HeaderCaption", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Currencies_Currency_Edit : CMSCurrenciesPage
{
    #region "Properties & Variables"

    private CurrencyInfo mEditedCurrency;

    #endregion


    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        mEditedCurrency = EditedObject as CurrencyInfo;

        if (mEditedCurrency != null)
        {
            // Check if not editing object from another site
            CheckEditedObjectSiteID(mEditedCurrency.CurrencySiteID);
        }

        EditForm.OnItemValidation += EditForm_OnItemValidation;
        EditForm.OnBeforeDataLoad += EditForm_OnBeforeDataLoad;

        // Register check permissions
        EditForm.OnCheckPermissions += (s, args) => CheckConfigurationModification();
    }

    #endregion


    #region "Event Handlers"

    protected void EditForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        EditForm.ObjectSiteID = ConfiguredSiteID;
    }


    protected void EditForm_OnItemValidation(object sender, ref string errorMessage)
    {
        var ctrl = sender as FormEngineUserControl;

        if ((ctrl == null))
        {
            return;
        }

        switch (ctrl.FieldInfo.Name)
        {
            case "CurrencyCode":
                var currencies = CurrencyInfoProvider.GetCurrenciesByCode(SiteID);
                var code = ctrl.Value.ToString();

                // Currency with same code already exists
                if (currencies.ContainsKey(code))
                {
                    // And it is not currently edited one
                    if ((mEditedCurrency == null) || (currencies[code].CurrencyID != mEditedCurrency.CurrencyID))
                    {
                        errorMessage = ResHelper.GetStringFormat("com.currencycodenotunique", code);
                    }
                }
                break;

            case "CurrencyEnabled":
                {
                    var main = CurrencyInfoProvider.GetMainCurrency(ConfiguredSiteID);
                    if ((main != null) && (mEditedCurrency != null) && (main.CurrencyID == mEditedCurrency.CurrencyID)
                        && !ValidationHelper.GetBoolean(ctrl.Value, true))
                    {
                        errorMessage = ResHelper.GetStringFormat("ecommerce.disablemaincurrencyerror", main.CurrencyDisplayName);
                    }
                }
                break;

            case "CurrencyFormatString":
                try
                {
                    // Test for double exception
                    string.Format(ctrl.Value.ToString().Trim(), 1.234);
                    string.Format(ctrl.Value.ToString().Trim(), "12.12");
                }
                catch
                {
                    errorMessage = GetString("Currency_Edit.ErrorCurrencyFormatString");
                }
                break;
        }
    }

    #endregion
}