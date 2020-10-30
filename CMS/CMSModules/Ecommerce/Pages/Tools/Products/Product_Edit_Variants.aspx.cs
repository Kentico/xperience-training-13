using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("com.products.variants")]
[UIElement(ModuleName.ECOMMERCE, "Products.Variants")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Variants : CMSProductsPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "product_variants";

    #endregion


    #region "Variables"

    private SKUInfo mProduct;

    // Possible actions
    private enum VariantActionEnum
    {
        Delete = 0,
        ChangePrice = 1,
    }

    // VariantAction scope
    private enum WhatEnum
    {
        SelectedVariants = 0,
        AllVariants = 1
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Local page messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    /// <summary>
    /// Variant parent product.
    /// </summary>
    private SKUInfo Product
    {
        get
        {
            return mProduct ?? (mProduct = SKUInfo.Provider.Get(ProductID));
        }
    }


    /// <summary>
    /// Current Warning.
    /// </summary>
    private string CurrentWarning
    {
        get
        {
            return ctlAsyncLog.ProcessData.Warning;
        }
        set
        {
            ctlAsyncLog.ProcessData.Warning = value;
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        btnOk.Text = GetString("general.ok");

        // Init unigrid
        ugVariants.OnAction += ugVariants_OnAction;
        ugVariants.OnExternalDataBound += ugVariants_OnExternalDataBound;
        ugVariants.WhereCondition = "SKUParentSKUID=" + ProductID;
        ugVariants.RememberStateByParam = "ProductID";
        ugVariants.ZeroRowsText = GetString("com.variant.novariant");

        // EditedObject used in grid filter
        EditedObject = Product;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Product != null)
        {
            CheckEditedObjectSiteID(Product.SKUSiteID);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Show confirmation message after generation of variants on generation page was successful
            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowConfirmation(GetString("com.variants.generated"));
            }
        }

        // Setup help
        object options = new
        {
            helpName = "lnkProductEditHelp",
            helpUrl = DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_LINK)
        };
        ScriptHelper.RegisterModule(this, "CMS/DialogContextHelpChange", options);

        // Setup asynchronous control
        SetupControl();

        // Init action selection
        InitBulkActionDropdownLists();

        string warningMessage = VariantsCanBeGenerated();

        if (string.IsNullOrEmpty(warningMessage))
        {
            string url = "~/CMSModules/Ecommerce/Pages/Tools/Products/Variant_New.aspx";
            url = URLHelper.AddParameterToUrl(url, "ProductID", ProductID.ToString());
            url = URLHelper.AddParameterToUrl(url, "dialog", QueryHelper.GetString("dialog", "0"));

            // Allow user to generate variants
            CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
            {
                Text = GetString("com.products.newvariant"),
                RedirectUrl = ResolveUrl(url)
            });
        }
        else
        {
            ShowWarning(warningMessage);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide bulk action area in case unigrid does not contain any variant
        pnlFooter.Visible = !ugVariants.IsEmpty;

        // Hide SKUAvailableItems column if inventory is not track by variants
        if (Product.SKUTrackInventory != TrackInventoryTypeEnum.ByVariants)
        {
            ugVariants.NamedColumns["SKUAvailableItems"].Visible = false;
        }

        // Action requires confirmation
        btnOk.OnClientClick = "return confirm(" + EnsureConfirmationMessage() + ");";
    }

    #endregion


    #region "Event handlers"

    private void ugVariants_OnAction(string actionName, object actionArgument)
    {
        if (string.IsNullOrEmpty(actionName))
        {
            return;
        }

        int variantID = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerInvariant())
        {
            case "edit":
                string url = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_Variant_Edit.aspx";
                url = URLHelper.AddParameterToUrl(url, "variantid", variantID.ToString());
                url = URLHelper.AddParameterToUrl(url, "productId", ProductID.ToString());
                url = URLHelper.AddParameterToUrl(url, "dialog", QueryHelper.GetString("dialog", "0"));
                URLHelper.Redirect(url);
                break;

            case "delete":

                // Check modify permission for parent product
                CheckProductModifyAndRedirect(Product);

                SKUInfo variantInfo = SKUInfo.Provider.Get(variantID);

                if (variantInfo == null)
                {
                    break;
                }

                // Try to delete variant and display warning in case it was disabled
                if (!DeleteVariant(variantInfo))
                {
                    // Inform user that variant was disabled
                    ShowWarning(GetString("com.product.edit.disableonevariant"));
                }

                break;
        }
    }


    private object ugVariants_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView variantRow = parameter as DataRowView;
        SKUInfo variant;

        if (variantRow != null)
        {
            variant = new SKUInfo(variantRow.Row);
        }
        else
        {
            return parameter;
        }

        if (string.IsNullOrEmpty(sourceName))
        {
            return parameter;
        }

        switch (sourceName.ToLowerInvariant())
        {
            case "skuname":
            {
                // Ensure correct values for unigrid export
                if (sender == null)
                {
                    return HTMLHelper.HTMLEncode(variant.SKUName);
                }

                InlineEditingTextBox inlineSkuName = new InlineEditingTextBox
                {
                    Text = variant.SKUName,
                    AdditionalCssClass = "inline-editing-textbox-text"
                };

                inlineSkuName.Formatting += (s, e) =>
                {
                    // Localize name
                    inlineSkuName.FormattedText = ResHelper.LocalizeString(variant.SKUName);
                };

                inlineSkuName.Update += (s, e) =>
                {
                    CheckProductModifyAndRedirect(Product);

                    // Trim text
                    inlineSkuName.Text = inlineSkuName.Text.Trim();

                    // Update name, if it is not empty
                    if (!string.IsNullOrEmpty(inlineSkuName.Text))
                    {
                        variant.SKUName = inlineSkuName.Text;
                        variant.MakeComplete(true);
                        variant.Update();

                        ugVariants.ReloadData();
                    }
                    else
                    {
                        inlineSkuName.ErrorText = GetString("general.requiresvalue");
                    }
                };

                return inlineSkuName;
            }

            case "skunumber":
            {
                // Ensure correct values for unigrid export
                if (sender == null)
                {
                    return HTMLHelper.HTMLEncode(variant.SKUNumber);
                }

                InlineEditingTextBox inlineSkuNumber = new InlineEditingTextBox
                {
                    Text = variant.SKUNumber,
                    AdditionalCssClass = "inline-editing-textbox-text"
                };

                inlineSkuNumber.Update += (s, e) =>
                {
                    CheckProductModifyAndRedirect(Product);

                    variant.SKUNumber = inlineSkuNumber.Text;
                    variant.MakeComplete(true);
                    variant.Update();

                    ugVariants.ReloadData();
                };

                return inlineSkuNumber;
            }

            case "skuprice":
            {
                var currency = CurrencyInfoProvider.GetMainCurrency(variant.SKUSiteID);
                var formattedValue = CurrencyInfoProvider.GetFormattedValue(variant.SKUPrice, currency);

                // Ensure correct values for unigrid export
                if (sender == null)
                {
                    return formattedValue;
                }

                var inlineSkuPrice = new InlineEditingTextBox
                {
                    Text = formattedValue,
                    FormattedText = CurrencyInfoProvider.GetFormattedPrice(variant.SKUPrice, currency)
                };

                inlineSkuPrice.Update += (s, e) =>
                {
                    CheckProductModifyAndRedirect(Product);

                    // Price must be decimal number
                    var price = ValidationHelper.GetDecimal(inlineSkuPrice.Text, -1);

                    var error = ValidatePrice(price, currency, variant);
                    if (String.IsNullOrEmpty(error))
                    {
                        // Round the price to according to the currency configuration
                        variant.SKUPrice = price;

                        variant.MakeComplete(true);
                        variant.Update();

                        ugVariants.ReloadData();
                    }
                    else
                    {
                        inlineSkuPrice.ErrorText = error;
                    }
                };

                return inlineSkuPrice;
            }

            case "skuavailableitems":
            {
                // Ensure correct values for unigrid export
                if (sender == null)
                {
                    return variant.SKUAvailableItems;
                }
                int availableItems = variant.SKUAvailableItems;

                var inlineSkuAvailableItems = new InlineEditingTextBox
                {
                    Text = availableItems.ToString(),
                    EnableEncode = false
                };

                inlineSkuAvailableItems.Formatting += (s, e) =>
                {
                    var reorderAt = variant.SKUReorderAt;

                    // Emphasize the number when product needs to be reordered
                    if (availableItems <= reorderAt)
                    {
                        // Format message informing about insufficient stock level
                        string reorderMsg = string.Format(GetString("com.sku.reorderatTooltip"), reorderAt);
                        string message = string.Format("<span class=\"alert-status-error\" onclick=\"UnTip()\" onmouseout=\"UnTip()\" onmouseover=\"Tip('{1}')\">{0}</span>", availableItems, reorderMsg);
                        inlineSkuAvailableItems.FormattedText = message;
                    }
                };

                inlineSkuAvailableItems.Update += (s, e) =>
                {
                    CheckProductModifyAndRedirect(Product);

                    var newNumberOfItems = ValidationHelper.GetInteger(inlineSkuAvailableItems.Text, availableItems);

                    // Update available items if new value is valid
                    if (ValidationHelper.IsInteger(inlineSkuAvailableItems.Text) && (-1000000000 <= newNumberOfItems) && (newNumberOfItems <= 1000000000))
                    {
                        variant.SKUAvailableItems = ValidationHelper.GetInteger(inlineSkuAvailableItems.Text, availableItems);
                        variant.MakeComplete(true);
                        variant.Update();

                        ugVariants.ReloadData();
                    }
                    else
                    {
                        inlineSkuAvailableItems.ErrorText = GetString("com.productedit.availableitemsinvalid");
                    }
                };
                return inlineSkuAvailableItems;
            }
        }

        return parameter;
    }


    protected void btnOk_Clicked(object sender, EventArgs e)
    {
        WhatEnum what = (WhatEnum)ValidationHelper.GetInteger(drpWhat.SelectedIndex, -1);

        // Do nothing if any variant is selected
        if ((what == WhatEnum.SelectedVariants) && (ugVariants.SelectedItems.Count == 0))
        {
            ShowWarning(GetString("com.variant.selectvariant"));
        }
        // Execute bulk action in asynchronous control
        else
        {
            // Check modify permission for parent product
            CheckProductModifyAndRedirect(Product);

            // Find selected action
            var action = (VariantActionEnum)ValidationHelper.GetInteger(drpAction.SelectedValue, -1);

            switch (action)
            {
                case VariantActionEnum.Delete:

                    ctlAsyncLog.TitleText = GetString("com.variant.deleting");

                    // Run action in asynchronous control
                    EnsureAsyncLog();
                    RunAsync(Delete);

                    break;

                case VariantActionEnum.ChangePrice:

                    ctlAsyncLog.TitleText = GetString("com.variant.priceupdating");

                    var currency = CurrencyInfoProvider.GetMainCurrency(Product.SKUSiteID);
                    var newPrice = ValidationHelper.GetDecimal(txtNewPrice.Text, -1);
                    var error = ValidatePrice(newPrice, currency, Product);

                    if (!String.IsNullOrEmpty(error))
                    {
                        pnlNewPrice.AddCssClass("has-error");
                        ShowError(error);

                        break;
                    }

                    txtNewPrice.RemoveCssClass("has-error");

                    // Run action in asynchronous control
                    EnsureAsyncLog();
                    RunAsync(p => UpdatePrice(newPrice));

                    break;
            }
        }
    }


    protected void drpAction_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Find selected action
        VariantActionEnum action = (VariantActionEnum)ValidationHelper.GetInteger(drpAction.SelectedValue, -1);

        // Display price input only for ChangePrice action
        pnlNewPrice.Visible = (action == VariantActionEnum.ChangePrice);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks if user is able to generate variants. Returns warning message if validation failed, empty string if it passed.
    /// </summary>
    private string VariantsCanBeGenerated()
    {
        // Inform user about need to change product type to be able to generate variants, it is allowed only for a standard product
        if (Product.SKUProductType != SKUProductTypeEnum.Product)
        {
            return String.Format(GetString("com.variant.notstandardproduct"), GetString("com.producttype.product"));
        }

        // Data set with option categories assigned to the product
        DataSet optionCategories = OptionCategoryInfoProvider.GetProductOptionCategories(ProductID, true, OptionCategoryTypeEnum.Attribute);
        // List of assigned Category IDs
        IList<int> categoryIDs = DataHelper.GetIntegerValues(optionCategories.Tables[0], "CategoryID");

        // Inform that attribute option category does not exist for this product
        if (DataHelper.DataSourceIsEmpty(optionCategories))
        {
            return GetString("com.variant.nooptioncategorycreated");
        }

        bool variantsCannotBeGenerated = true;

        foreach (int categoryId in categoryIDs)
        {
            var enabledAllowedOptions = SKUInfoProvider.GetSKUOptionsForProduct(Product.SKUID, categoryId, true).TopN(1);

            if (!DataHelper.DataSourceIsEmpty(enabledAllowedOptions))
            {
                variantsCannotBeGenerated = false;
                break;
            }
        }

        // Inform that any option is selected,created or enabled in assigned option category
        if (variantsCannotBeGenerated)
        {
            return GetString("com.variant.nooptionselectedforproduct");
        }

        var bundles = SKUInfo.Provider.Get()
                                     .Column("SKUName")
                                     .WhereIn("SKUID", BundleInfo.Provider.Get()
                                                                         .Column("BundleID")
                                                                         .WhereEquals("SKUID", Product.SKUID)
                                             )
                                     .GetListResult<string>();

        if (bundles.Count > 0)
        {
            return String.Format(GetString("com.variant.productusedinbundle"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(string.Join(", ", bundles))));
        }

        return "";
    }


    /// <summary>
    /// Generates confirmation text.
    /// </summary>
    private string EnsureConfirmationMessage()
    {
        string message;

        // Find selected scope
        WhatEnum scope = (WhatEnum)ValidationHelper.GetInteger(drpWhat.SelectedValue, -1);

        // Find selected action
        VariantActionEnum action = (VariantActionEnum)ValidationHelper.GetInteger(drpAction.SelectedValue, -1);

        // Ensure correct confirmation message
        if (action == VariantActionEnum.Delete)
        {
            message = (scope == WhatEnum.AllVariants) ? "com.productvariant.deleteconfirmation" : "com.productvariant.deleteselectedconfirmation";
        }
        else
        {
            message = (scope == WhatEnum.AllVariants) ? "com.productvariant.updateprice" : "com.productvariant.updateselectedprice";
        }

        return ScriptHelper.GetLocalizedString(message);
    }


    /// <summary>
    /// Generates scope and action drop down lists.
    /// </summary>
    private void InitBulkActionDropdownLists()
    {
        // Init actions and subjects
        if (!RequestHelper.IsPostBack())
        {
            drpAction.Items.Add(new ListItem(GetString("general." + VariantActionEnum.Delete), Convert.ToInt32(VariantActionEnum.Delete).ToString()));
            drpAction.Items.Add(new ListItem(GetString("com.variant." + VariantActionEnum.ChangePrice), Convert.ToInt32(VariantActionEnum.ChangePrice).ToString()));

            drpWhat.Items.Add(new ListItem(GetString("com.variant." + WhatEnum.SelectedVariants), Convert.ToInt32(WhatEnum.SelectedVariants).ToString()));
            drpWhat.Items.Add(new ListItem(GetString("com.variant." + WhatEnum.AllVariants), Convert.ToInt32(WhatEnum.AllVariants).ToString()));
        }
    }


    /// <summary>
    /// Deletes or disables variant. Returns true when variant was deleted, false when disabled.
    /// </summary>
    /// <param name="variantInfo"> Variant of product</param>
    private bool DeleteVariant(SKUInfo variantInfo)
    {
        // Check variant dependencies and delete it if possible, otherwise disable it
        if (variantInfo.Generalized.CheckDependencies())
        {
            // Disable variant
            variantInfo.SKUEnabled = false;
            SKUInfo.Provider.Set(variantInfo);

            return false;
        }

        AddLog(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(variantInfo.SKUName)) + GetString("com.variant.isbeingdeleted"));

        // Delete variant product
        SKUInfo.Provider.Delete(variantInfo);

        return true;
    }


    /// <summary>
    /// Updates variant price.
    /// </summary>
    /// <param name="variantInfo">Variant SKU info</param>
    /// <param name="newPrice">New price of variant</param>
    private void UpdateVariant(SKUInfo variantInfo, decimal newPrice)
    {
        AddLog(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(variantInfo.SKUName)) + GetString("com.variant.priceisbeingupdated"));

        // Change price
        variantInfo.SKUPrice = newPrice;
        variantInfo.Update();
    }


    /// <summary>
    /// Prepare asynchronous control
    /// </summary>
    private void SetupControl()
    {
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        ctlAsyncLog.MaxLogLines = 1000;

        // Register full postback, button cannot be in update panel
        ControlsHelper.RegisterPostbackControl(btnOk);

        // Asynchronous content configuration
        if (!RequestHelper.IsCallback())
        {
            // Set visibility of panels
            pnlContent.Visible = true;
            pnlLog.Visible = false;
        }
    }


    /// <summary>
    /// Updates price of variants in asynchronous control.
    /// </summary>
    /// <param name="newPrice">New price of variant</param>
    protected void UpdatePrice(decimal newPrice)
    {
        try
        {
            SKUInfo variantInfo = null;

            // Use special action contexts to turn off unnecessary actions
            using (ECommerceActionContext eCommerceContext = new ECommerceActionContext())
            {
                eCommerceContext.TouchParent = false;
                eCommerceContext.SetLowestPriceToParent = false;

                // Update all variants
                if ((WhatEnum)ValidationHelper.GetInteger(drpWhat.SelectedIndex, -1) == WhatEnum.AllVariants)
                {
                    var variants = VariantHelper.GetVariants(ProductID);

                    foreach (var variant in variants)
                    {
                        variantInfo = variant;
                        UpdateVariant(variantInfo, newPrice);
                    }
                }
                // Update selected variants
                else
                {
                    var variantsToUpdate = ugVariants.SelectedItems;

                    foreach (var variantId in variantsToUpdate)
                    {
                        variantInfo = SKUInfo.Provider.Get(ValidationHelper.GetInteger(variantId, 0));

                        // Do not allow modify variants of other product
                        if ((variantInfo != null) && (variantInfo.SKUParentSKUID != ProductID))
                        {
                            variantInfo = null;
                        }

                        if (variantInfo != null)
                        {
                            UpdateVariant(variantInfo, newPrice);
                        }
                    }
                }
            }

            // Save variant to update parent SKULastModified a SKUPrice properties
            variantInfo?.Generalized.SetObject();
        }
        catch (Exception ex)
        {
            CurrentError = GetString("com.product.updatepriceerror");
            Service.Resolve<IEventLogService>().LogException("Update price bulk action", "UPDATEVARIANT", ex);
        }
    }


    /// <summary>
    /// Deletes variants is asynchronous control.
    /// </summary>
    private void Delete(object parameters)
    {
        bool variantWasDisabled = false;

        try
        {
            // Use special action contexts to turn off unnecessary actions
            using (ECommerceActionContext eCommerceContext = new ECommerceActionContext())
            {
                eCommerceContext.TouchParent = false;
                eCommerceContext.SetLowestPriceToParent = false;

                // Delete all variants
                if ((WhatEnum)ValidationHelper.GetInteger(drpWhat.SelectedIndex, -1) == WhatEnum.AllVariants)
                {
                    var variants = VariantHelper.GetVariants(ProductID);

                    foreach (SKUInfo variant in variants)
                    {
                        // Set flag when variant was disabled due to dependencies
                        variantWasDisabled |= !DeleteVariant(variant);
                    }
                }
                // Delete selected variants
                else
                {
                    var variantsToDelete = ugVariants.SelectedItems;

                    foreach (string variantId in variantsToDelete)
                    {
                        var variantInfo = SKUInfo.Provider.Get(ValidationHelper.GetInteger(variantId, 0));

                        if ((variantInfo != null) && (variantInfo.SKUParentSKUID == ProductID))
                        {
                            // Set flag when variant was disabled due to dependencies
                            variantWasDisabled |= !DeleteVariant(variantInfo);
                        }
                    }
                }
            }

            // Save variant to update parent SKULastModified a SKUPrice properties
            DataSet productVariants = VariantHelper.GetVariants(ProductID);
            if (!DataHelper.DataSourceIsEmpty(productVariants))
            {
                SKUInfo variantInfo = new SKUInfo(productVariants.Tables[0].Rows[0]);
                variantInfo.Generalized.SetObject();
            }
            else
            {
                // If all variants were deleted, update parent
                Product?.Generalized.SetObject();
            }
        }
        catch (Exception ex)
        {
            CurrentError = GetString("com.product.deleteerror");
            Service.Resolve<IEventLogService>().LogException("Variant listing", "DELETEVARIANT", ex);
        }

        ugVariants.ResetSelection();

        if (variantWasDisabled)
        {
            CurrentWarning = GetString("com.product.edit.disablevariant");
        }
    }

    #endregion


    #region "Handling asynchronous thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        ShowError(GetString("com.variant.terminated"));
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(CurrentWarning))
        {
            ShowWarning(CurrentWarning);
        }

        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
    }


    /// <summary>
    /// Adds the log information
    /// </summary>
    /// <param name="newLog">New log information</param>
    private void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Ensures log for asynchronous control
    /// </summary>
    private void EnsureAsyncLog()
    {
        pnlLog.Visible = true;
        pnlContent.Visible = false;
        CurrentWarning = string.Empty;
        CurrentError = string.Empty;
    }


    /// <summary>
    /// Runs asynchronous thread
    /// </summary>
    /// <param name="action">Method to run</param>
    private void RunAsync(AsyncAction action)
    {
        ctlAsyncLog.EnsureLog();
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }

    #endregion
}
