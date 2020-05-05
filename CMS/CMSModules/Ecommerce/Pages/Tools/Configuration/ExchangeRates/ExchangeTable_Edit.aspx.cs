using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "ExchangeTable_Edit.ItemListLink", "ExchangeTable_List.aspx?siteId={?siteId?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "ExchangeTable_Edit.NewItemCaption", NewObject = true)]
// Edited object
[EditedObject(ExchangeTableInfo.OBJECT_TYPE, "exchangeid")]
// Title
[Title("ExchangeTable_Edit.HeaderCaption", ExistingObject = true)]
[Title("ExchangeTable_New.HeaderCaption", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ExchangeRates_ExchangeTable_Edit : CMSExchangeRatesPage
{
    #region "Constants"

    private const string SAVE = ComponentEvents.SAVE;

    #endregion


    #region "Variables"

    private int mExchangeTableId;
    private readonly Hashtable mTextBoxes = new Hashtable();
    private readonly Hashtable mData = new Hashtable();
    private readonly Dictionary<int, DataRow> mExchangeRates = new Dictionary<int, DataRow>();
    private CurrencyInfo mMainCurrency;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        editGrid.RowDataBound += editGrid_RowDataBound;
        btnOk.Click += (s, args) => Save();

        // Get main currency
        mMainCurrency = CurrencyInfoProvider.GetMainCurrency(ConfiguredSiteID);

        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        // Help image
        iconHelpGlobalExchangeRate.ToolTip = GetString("ExchangeTable_Edit.ExchangeRateHelp");
        iconHelpMainExchangeRate.ToolTip = GetString("ExchangeTable_Edit.ExchangeRateHelp");

        // Use time zones for DateTimePickers
        TimeZoneInfo tzi = TimeZoneHelper.GetTimeZoneInfo(MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
        dtPickerExchangeTableValidFrom.TimeZone = TimeZoneTypeEnum.Custom;
        dtPickerExchangeTableValidFrom.CustomTimeZone = tzi;
        dtPickerExchangeTableValidTo.TimeZone = TimeZoneTypeEnum.Custom;
        dtPickerExchangeTableValidTo.CustomTimeZone = tzi;

        // Get exchangeTable id from query string
        mExchangeTableId = QueryHelper.GetInteger("exchangeid", 0);
        if (mExchangeTableId > 0)
        {
            ExchangeTableInfo exchangeTableObj = EditedObject as ExchangeTableInfo;

            if (exchangeTableObj != null)
            {
                // Check tables site id
                CheckEditedObjectSiteID(exchangeTableObj.ExchangeTableSiteID);

                LoadData(exchangeTableObj);

                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    // Show that the exchangeTable was created or updated successfully
                    if (QueryHelper.GetString("saved", "") == "1")
                    {
                        // Show message
                        ShowChangesSaved();
                    }
                }
            }

            // Check presence of main currency
            plcGrid.Visible = CheckMainCurrency(ConfiguredSiteID);

            plcRateFromGlobal.Visible = IsFromGlobalRateNeeded();
        }
        // Creating a new exchange table
        else
        {
            if (!RequestHelper.IsPostBack())
            {
                // Preset valid from date
                var tableInfo = ExchangeTableInfoProvider.GetLastExchangeTableInfo(ConfiguredSiteID);
                if (tableInfo != null)
                {
                    dtPickerExchangeTableValidFrom.SelectedDateTime = tableInfo.ExchangeTableValidTo;
                }
            }
            // Grids are visible only in edit mode
            plcGrid.Visible = false;
        }

        // Register bootstrap tooltip over help icons
        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");
    }

    #endregion


    #region "Event handlers"

    private void editGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.DataItemIndex >= 0)
        {
            CMSTextBox txt = new CMSTextBox();
            txt.TextChanged += txt_TextChanged;
            txt.MaxLength = 19;

            // Id of the currency displayed in this row
            int curId = ValidationHelper.GetInteger(((DataRowView)e.Row.DataItem)["CurrencyID"], 0);

            // Find exchange rate for this row currency
            string rateTextValue = "";
            if (mExchangeRates.ContainsKey(curId))
            {
                DataRow row = mExchangeRates[curId];
                decimal exchangeRate = ValidationHelper.GetDecimal(row["ExchangeRateValue"], -1m);
                if (exchangeRate > 0m)
                {
                    rateTextValue = exchangeRate.TrimEnd().ToString();
                }
            }

            // Fill and add text box to the "Rate value" column of the grid
            txt.Text = rateTextValue;
            e.Row.Cells[1].Controls.Add(txt);
            mData[txt.ClientID] = e.Row.DataItem;
        }
    }


    private void txt_TextChanged(object sender, EventArgs e)
    {
        mTextBoxes[((TextBox)sender).ClientID] = sender;
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case SAVE:
                // Save rates
                Save();
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Load data of editing exchangeTable.
    /// </summary>
    /// <param name="exchangeTableObj">ExchangeTable object</param>
    protected void LoadData(ExchangeTableInfo exchangeTableObj)
    {
        editGrid.Columns[0].HeaderText = GetString("ExchangeTable_Edit.ToCurrency");
        editGrid.Columns[1].HeaderText = GetString("ExchangeTable_Edit.RateValue");

        // Get exchange rates and fill the dictionary
        DataSet dsExRates = ExchangeRateInfoProvider.GetExchangeRates(mExchangeTableId);
        if (!DataHelper.DataSourceIsEmpty(dsExRates))
        {
            foreach (DataRow dr in dsExRates.Tables[0].Rows)
            {
                int toCurrencyId = ValidationHelper.GetInteger(dr["ExchangeRateToCurrencyID"], -1);
                if (!mExchangeRates.ContainsKey(toCurrencyId))
                {
                    mExchangeRates.Add(toCurrencyId, dr);
                }
            }
        }

        DataSet dsAllCurrencies = CurrencyInfoProvider.GetCurrencies(ConfiguredSiteID);
        // Row index of main currency
        int mainCurrIndex = -1;
        int i = 0;

        if (!DataHelper.DataSourceIsEmpty(dsAllCurrencies))
        {
            // Find main currency in all currencies dataset
            if (mMainCurrency != null)
            {
                // Prepare site main currency unit label
                string siteCode = mMainCurrency.CurrencyCode;
                lblSiteMainCurrency.Text = siteCode;
                lblMainToSite.Text = string.Format(GetString("ExchangeTable_Edit.FromMainToSite"), HTMLHelper.HTMLEncode(siteCode));

                // Prepare global main currency unit label
                string globalCode = CurrencyInfoProvider.GetMainCurrencyCode(0);
                lblFromGlobalToMain.Text = string.Format(GetString("ExchangeTable_Edit.FromGlobalToMain"), HTMLHelper.HTMLEncode(globalCode));

                foreach (DataRow dr in dsAllCurrencies.Tables[0].Rows)
                {
                    if (ValidationHelper.GetInteger(dr["CurrencyID"], -1) == mMainCurrency.CurrencyID)
                    {
                        mainCurrIndex = i;
                    }
                    i++;
                }
            }

            // Remove found main currency
            if (mainCurrIndex != -1)
            {
                dsAllCurrencies.Tables[0].Rows[mainCurrIndex].Delete();
                dsAllCurrencies.AcceptChanges();
            }
        }

        if (DataHelper.DataSourceIsEmpty(dsAllCurrencies))
        {
            // Site exchange rates section is visible only when more currencies exist
            plcSiteRates.Visible = true;
            lblMainToSite.Visible = true;
            plcNoCurrency.Visible = true;
        }

        // Hide rates part when no grid visible
        plcGrid.Visible = plcSiteRates.Visible || plcRateFromGlobal.Visible;

        // Use currencies in grid
        editGrid.DataSource = dsAllCurrencies;
        editGrid.DataBind();

        // Fill editing form
        if (!RequestHelper.IsPostBack())
        {
            dtPickerExchangeTableValidFrom.SelectedDateTime = exchangeTableObj.ExchangeTableValidFrom;
            txtExchangeTableDisplayName.Text = exchangeTableObj.ExchangeTableDisplayName;
            dtPickerExchangeTableValidTo.SelectedDateTime = exchangeTableObj.ExchangeTableValidTo;
            txtGlobalExchangeRate.Text = exchangeTableObj.ExchangeTableRateFromGlobalCurrency.TrimEnd().ToString();
        }
    }


    /// <summary>
    /// Saves exchange rates.
    /// </summary>
    private void Save()
    {
        // Check permissions
        CheckConfigurationModification();

        string errorMessage = new Validator().NotEmpty(txtExchangeTableDisplayName.Text.Trim(), GetString("general.requiresdisplayname")).Result;

        if ((errorMessage == "") && (plcRateFromGlobal.Visible))
        {
            errorMessage = new Validator().NotEmpty(txtGlobalExchangeRate.Text.Trim(), GetString("ExchangeTable_Edit.DecimalFormatRequired")).Result;
        }

        if ((errorMessage == "") && (plcRateFromGlobal.Visible))
        {
            if (!ValidationHelper.IsPositiveNumber(txtGlobalExchangeRate.Text.Trim()) || (ValidationHelper.GetDecimal(txtGlobalExchangeRate.Text.Trim(), 0m) == 0m))
            {
                errorMessage = GetString("ExchangeTable_Edit.errorRate");
            }
        }

        // From/to date validation
        if (errorMessage == "")
        {
            if ((!dtPickerExchangeTableValidFrom.IsValidRange()) || (!dtPickerExchangeTableValidTo.IsValidRange()))
            {
                errorMessage = GetString("general.errorinvaliddatetimerange");
            }

            if ((dtPickerExchangeTableValidFrom.SelectedDateTime != DateTime.MinValue) &&
                (dtPickerExchangeTableValidTo.SelectedDateTime != DateTime.MinValue) &&
                (dtPickerExchangeTableValidFrom.SelectedDateTime >= dtPickerExchangeTableValidTo.SelectedDateTime))
            {
                errorMessage = GetString("General.DateOverlaps");
            }
        }

        // Exchange rates validation
        if (errorMessage == String.Empty)
        {
            foreach (TextBox txt in mTextBoxes.Values)
            {
                string tmp = txt.Text.Trim();
                if (tmp != String.Empty)
                {
                    // Exchange rate must be decimal
                    if (!ValidationHelper.IsDecimal(tmp, precision: 18, scale: 9))
                    {
                        errorMessage = GetString("ExchangeTable_Edit.DecimalFormatRequired");
                        break;
                    }

                    // Exchange rate must be positive
                    decimal rate = ValidationHelper.GetDecimal(tmp, 1m);
                    if (rate <= 0m)
                    {
                        errorMessage = GetString("ExchangeTable_Edit.errorRate");
                    }
                }
            }
        }

        // Save changes if no validation error
        if (errorMessage == "")
        {
            // Truncate display name
            string displayName = txtExchangeTableDisplayName.Text.Trim().Truncate(txtExchangeTableDisplayName.MaxLength);

            ExchangeTableInfo exchangeTableObj = ExchangeTableInfoProvider.GetExchangeTableInfo(displayName, SiteInfoProvider.GetSiteName(ConfiguredSiteID));

            // If exchangeTableName value is unique
            if ((exchangeTableObj == null) || (exchangeTableObj.ExchangeTableID == mExchangeTableId))
            {
                // Get ExchangeTableInfo object by primary key
                exchangeTableObj = ExchangeTableInfo.Provider.Get(mExchangeTableId);
                if (exchangeTableObj == null)
                {
                    // Create new item -> insert
                    exchangeTableObj = new ExchangeTableInfo();
                    exchangeTableObj.ExchangeTableSiteID = ConfiguredSiteID;
                }

                exchangeTableObj.ExchangeTableValidFrom = dtPickerExchangeTableValidFrom.SelectedDateTime;
                exchangeTableObj.ExchangeTableDisplayName = displayName;
                exchangeTableObj.ExchangeTableValidTo = dtPickerExchangeTableValidTo.SelectedDateTime;
                exchangeTableObj.ExchangeTableRateFromGlobalCurrency = 0m;
                if (plcRateFromGlobal.Visible)
                {
                    exchangeTableObj.ExchangeTableRateFromGlobalCurrency = ValidationHelper.GetDecimal(txtGlobalExchangeRate.Text.Trim(), 0m);
                }

                // Save general exchange table information
                ExchangeTableInfo.Provider.Set(exchangeTableObj);

                // Save rates on edit
                if (mExchangeTableId > 0)
                {
                    foreach (TextBox txt in mTextBoxes.Values)
                    {
                        if (mData[txt.ClientID] != null)
                        {
                            int rateCurrencyId = ValidationHelper.GetInteger(((DataRowView)mData[txt.ClientID])["CurrencyID"], 0);
                            bool rateExists = mExchangeRates.ContainsKey(rateCurrencyId);

                            if (rateExists)
                            {
                                ExchangeRateInfo rate = new ExchangeRateInfo(mExchangeRates[rateCurrencyId]);

                                if (txt.Text.Trim() == String.Empty)
                                {
                                    // Remove exchange rate
                                    ExchangeRateInfo.Provider.Delete(rate);
                                }
                                else
                                {
                                    rate.ExchangeRateValue = ValidationHelper.GetDecimal(txt.Text.Trim(), 0m);
                                    // Update rate
                                    ExchangeRateInfo.Provider.Set(rate);
                                }
                            }
                            else
                            {
                                if (txt.Text.Trim() != String.Empty)
                                {
                                    // Insert exchange rate
                                    var rate = new ExchangeRateInfo
                                               {
                                                   ExchangeRateToCurrencyID = rateCurrencyId,
                                                   ExchangeRateValue = ValidationHelper.GetDecimal(txt.Text.Trim(), 0m),
                                                   ExchangeTableID = mExchangeTableId
                                               };

                                    ExchangeRateInfo.Provider.Set(rate);
                                }
                            }
                        }
                    }
                }

                URLHelper.Redirect(UrlResolver.ResolveUrl("ExchangeTable_Edit.aspx?exchangeid=" + exchangeTableObj.ExchangeTableID + "&saved=1&siteId=" + SiteID));
            }
            else
            {
                // Show error message
                ShowError(GetString("ExchangeTable_Edit.CurrencyNameExists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Indicates if exchange rate from global main currency is needed.
    /// </summary>
    protected bool IsFromGlobalRateNeeded()
    {
        var siteId = ConfiguredSiteID;

        if ((siteId == 0) || (ECommerceSettings.UseGlobalCurrencies(siteId)))
        {
            return false;
        }

        string globalMainCode = CurrencyInfoProvider.GetMainCurrencyCode(0);
        string siteMainCode = CurrencyInfoProvider.GetMainCurrencyCode(siteId);

        // Check whether main currencies are defined
        if (string.IsNullOrEmpty(siteMainCode) || string.IsNullOrEmpty(globalMainCode))
        {
            return false;
        }

        // Check whether global and site main currency are the same
        if (string.Equals(globalMainCode, siteMainCode, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        // Check if site has currency with same code as global main -> no need for global rate
        if (CurrencyInfoProvider.GetCurrenciesByCode(siteId).ContainsKey(globalMainCode))
        {
            return false;
        }

        return ECommerceSettings.AllowGlobalProductOptions(siteId) ||
               ECommerceSettings.AllowGlobalProducts(siteId) ||
               ECommerceSettings.UseGlobalCredit(siteId) ||
               ECommerceSettings.UseGlobalTaxClasses(siteId);
    }

    #endregion
}
