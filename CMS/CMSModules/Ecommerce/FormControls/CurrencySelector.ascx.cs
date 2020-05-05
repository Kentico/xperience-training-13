using System.Data;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_CurrencySelector : SiteSeparatedObjectSelector
{
    #region "Variables"

    private int mainCurrencyID = -1;

    #endregion


    #region "Properties"

    /// <summary>
    ///  If true, selected value is CurrencyName, if false, selected value is CurrencyID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseCurrencyNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseCurrencyNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Indicates if only currencies with exchange rate will be displayed. Main currency will be included. Default value is false. 
    /// </summary>
    public bool DisplayOnlyWithExchangeRate
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyWithExchangeRate"), false);
        }
        set
        {
            SetValue("DisplayOnlyWithExchangeRate", value);
        }
    }


    /// <summary>
    /// Gets or sets the display name format.
    /// </summary>
    public string DisplayNameFormat
    {
        get
        {
            return uniSelector.DisplayNameFormat;
        }
        set
        {
            uniSelector.DisplayNameFormat = value;
        }
    }


    /// <summary>
    /// Returns inner CMSDropDownList control.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Returns inner UniSelector control.
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Indicates whether to show all items ("more items" is not displayed).
    /// </summary>
    public bool ShowAllItems
    {
        get;
        set;
    }


    /// <summary>
    /// Add (select) record to the drop-down list.
    /// </summary>
    public bool AddSelectRecord
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether to add current site default currency.
    /// </summary>
    public bool AddSiteDefaultCurrency
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether to exclude current site default currency.
    /// </summary>
    public bool ExcludeSiteDefaultCurrency
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether update panel is enabled.
    /// </summary>
    public bool DoFullPostback
    {
        get;
        set;
    }

    /// <summary>
    /// Returns main currency ID.
    /// </summary>
    public int MainCurrencyID
    {
        get
        {
            if (mainCurrencyID < 0)
            {
                var main = CurrencyInfoProvider.GetMainCurrency(SiteID);
                mainCurrencyID = (main != null) ? main.CurrencyID : 0;
                return mainCurrencyID;
            }

            return mainCurrencyID;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Convert given currency name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the currency to be converted.</param>
    /// <param name="siteName">Name of the site of the currency.</param>
    protected override int GetID(string name, string siteName)
    {
        var currency = CurrencyInfo.Provider.Get(name, SiteInfoProvider.GetSiteID(siteName));

        return (currency != null) ? currency.CurrencyID : 0;
    }


    /// <summary>
    /// Selector initialization.
    /// </summary>
    protected override void InitSelector()
    {
        base.InitSelector();

        if (ShowAllItems)
        {
            uniSelector.MaxDisplayedItems = 1000;
        }

        if (DoFullPostback)
        {
            // Disable update panel
            ControlsHelper.RegisterPostbackControl(uniSelector.DropDownSingleSelect);
        }

        if (AddSelectRecord)
        {
            // Add (select) item to the UniSelector
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("currencyselector.select"), Value = "-1" });
        }
    }


    /// <summary>
    /// Exclude main currency and currencies without exchange rate according selector settings.
    /// </summary>
    /// <param name="whereCondition">Where condition.</param>
    protected override string AppendExclusiveWhere(string whereCondition)
    {
        // Prepare where condition
        var where = new WhereCondition(whereCondition);
        
        // Exclude site main currency when required
        if (ExcludeSiteDefaultCurrency && (MainCurrencyID > 0))
        {
            where.WhereNotEquals("CurrencyID", MainCurrencyID);
        }

        // Restrict disabled or site not related currencies
        return base.AppendExclusiveWhere(where.ToString(true));
    }


    /// <summary>
    /// Add site default currency according selector settings.
    /// </summary>
    /// <param name="whereCondition">Where condition.</param>
    /// <returns></returns>
    protected override string AppendInclusiveWhere(string whereCondition)
    {
        // Prepare where condition
        var where = new WhereCondition(whereCondition);

        // Add site main currency when required
        if (AddSiteDefaultCurrency && (MainCurrencyID > 0))
        {
            where.Or().WhereEquals("CurrencyID", MainCurrencyID);
        }

        // Append additional items
        return base.AppendInclusiveWhere(where.ToString(true));
    }


    /// <summary>
    /// Ensures that only currencies with exchange rates are displayed in the selector.
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    protected override DataSet OnAfterRetrieveData(DataSet ds)
    {
        if (!DisplayOnlyWithExchangeRate || DataHelper.DataSourceIsEmpty(ds))
        {
            return ds;
        }

        var currencies = ds.Tables[0].Select();
        foreach (var currencyRow in currencies)
        {
            var currencyId = ValidationHelper.GetInteger(currencyRow["CurrencyID"], 0);

            // Check if we are able to work with this currency in terms of currency conversion
            if (!CurrencyInfoProvider.IsCurrencyWithExchangeRate(currencyId, SiteID))
            {
                ds.Tables[0].Rows.Remove(currencyRow);
            }
        }

        return ds;
    }

    #endregion
}