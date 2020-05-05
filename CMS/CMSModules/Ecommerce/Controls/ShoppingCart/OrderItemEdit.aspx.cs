using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("OrderItemEdit.Title")]
[UIElement(ModuleName.ECOMMERCE, "order.OrderItemProperties")]
public partial class CMSModules_Ecommerce_Controls_ShoppingCart_OrderItemEdit : CMSEcommercePage
{
    #region "Variables"

    private ShoppingCartInfo mShoppingCart;
    private ShoppingCartItemInfo mShoppingCartItem;
    private OptionCategoryInfo mOptionCategory;
    private SKUInfo mSKU;

    #endregion


    #region "Properties"

    /// <summary>
    /// Shopping cart object with order data.
    /// </summary>
    private ShoppingCartInfo ShoppingCart
    {
        get
        {
            if (mShoppingCart == null)
            {
                string cartSessionName = QueryHelper.GetString("cart", String.Empty);
                if (cartSessionName != String.Empty)
                {
                    mShoppingCart = SessionHelper.GetValue(cartSessionName) as ShoppingCartInfo;
                }
            }

            return mShoppingCart;
        }
    }


    /// <summary>
    /// Gets the Order object where edited item belongs.
    /// </summary>
    private OrderInfo Order
    {
        get
        {
            return ShoppingCart.Order;
        }
    }


    /// <summary>
    /// Shopping cart item data.
    /// </summary>
    private ShoppingCartItemInfo ShoppingCartItem
    {
        get
        {
            if (mShoppingCartItem == null)
            {
                if (ShoppingCart != null)
                {
                    Guid cartItemGuid = QueryHelper.GetGuid("itemguid", Guid.Empty);
                    if (cartItemGuid != Guid.Empty)
                    {
                        mShoppingCartItem = ShoppingCartInfoProvider.GetShoppingCartItem(ShoppingCart, cartItemGuid);
                    }
                }
            }

            return mShoppingCartItem;
        }
    }


    /// <summary>
    /// SKU option category data
    /// </summary>
    private OptionCategoryInfo OptionCategory
    {
        get
        {
            if ((mOptionCategory == null) && (SKU != null))
            {
                mOptionCategory = OptionCategoryInfo.Provider.Get(SKU.SKUOptionCategoryID);
            }

            return mOptionCategory;
        }
    }


    /// <summary>
    /// SKU data
    /// </summary>
    private SKUInfo SKU
    {
        get
        {
            if ((mSKU == null) && (ShoppingCartItem != null))
            {
                mSKU = ShoppingCartItem.SKU;
            }

            return mSKU;
        }
    }


    /// <summary>
    /// Indicates if editing of order item is enabled. Reflects EC setting and order is paid flag.
    /// </summary>
    private bool EditingEnabled
    {
        get
        {
            // Order must not be paid and item must be persisted in the OrderItem
            return (Order != null) && !Order.OrderIsPaid && (ShoppingCartItem.OrderItem != null);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditedObject = ShoppingCartItem;
        EditForm.ParentObject = ShoppingCart;

        // Update UIForm before save
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;

        // Validate Order item fields
        EditForm.OnItemValidation += EditForm_OnItemValidate;

        HeaderActions.Visible = false;
        EditForm.SubmitButton.Visible = false;

        if (EditingEnabled)
        {
            btnSave.Click += (s, ea) => EditForm.SaveData(null);
        }
        else
        {
            btnSave.ResourceString = "general.close";
            btnSave.OnClientClick = "CloseDialog();";
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        LoadData();

        RegisterEscScript();
        RegisterModalPageScripts();
    }

    #endregion


    #region "Event handlers"

    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Check if order item modification is allowed
        if (!Order.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
        }
        
        // Reset auto added units (already auto added units in the shopping cart will not be removed)
        ShoppingCartItem.CartItemAutoAddedUnits = 0;

        // Update unit price and clear discounts
        ShoppingCartItem.OrderItem.OrderItemUnitPrice = ValidationHelper.GetDecimal(EditForm.FieldControls["CartItemUnitPrice"].Value, 0m);
        ShoppingCartItem.UnitDiscountSummary = null;

        // Update units
        ShoppingCartItem.CartItemUnits = ValidationHelper.GetInteger(EditForm.FieldControls["SKUUnits"].Value, 0);

        // Update product text
        if (SKU.IsTextAttribute)
        {
            var controlName = (SKU.SKUOptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextArea) ? "CartItemTextArea" : "CartItemTextBox";
            ShoppingCartItem.CartItemText = ValidationHelper.GetString(EditForm.FieldControls[controlName].Value, String.Empty);
        }

        // Update units of the product options
        int skuUnits = ValidationHelper.GetInteger(EditForm.FieldControls["SKUUnits"].Value, 0);
        foreach (var option in ShoppingCartItem.ProductOptions)
        {
            option.CartItemUnits = skuUnits;
        }

        // Evaluate shopping cart content
        ShoppingCart.Evaluate();

        // Close dialog window and refresh parent window
        CloseDialogWindow();

        // Do not save ShoppingCartItem to the database
        EditForm.StopProcessing = true;
    }


    private void EditForm_OnItemValidate(object sender, ref string errorMessage)
    {
        var ctrl = sender as FormEngineUserControl;
        if (ctrl == null)
        {
            return;
        }

        // Check the length of a text product option. Only visible fields are checked
        if (ctrl.FieldInfo.Visible && ((ctrl.FieldInfo.Name == "CartItemTextArea") || (ctrl.FieldInfo.Name == "CartItemTextBox")))
        {
            string text = ValidationHelper.GetString(ctrl.Value, "");

            // Length range set in the product option category
            int minLength = OptionCategory.CategoryTextMinLength;
            int maxLength = OptionCategory.CategoryTextMaxLength;
            // Length of the edited field
            int newLength = text.Length;

            if (((minLength > 0) && (minLength > newLength)) || ((maxLength > 0) && (maxLength < newLength)))
            {
                // New length is not in the range 
                errorMessage = String.Format(GetString("com.orderitemedit.textlengthrange"), minLength, maxLength);
            }
        }
    }

    #endregion


    #region "Help methods"

    /// <summary>
    /// Loads order item data to the form fields.
    /// </summary>
    private void LoadData()
    {
        var sku = SKU;

        // Check if ShoppingCartItem or SKU exist
        if ((ShoppingCartItem == null) || (sku == null))
        {
            RedirectToInformation("general.ObjectNotFound");
            return;
        }

        // Label control must be set even when request is postback 
        if (sku.SKUProductType == SKUProductTypeEnum.EProduct)
        {
            // Check whether the order is paid
            if ((Order != null) && Order.OrderIsPaid)
            {
                // Check whether the e-product is unlimited
                if (ShoppingCartItem.CartItemValidTo.CompareTo(DateTimeHelper.ZERO_TIME) == 0)
                {
                    // Display as unlimited
                    EditForm.FieldControls["CartItemValidToLabel"].Value = GetString("general.unlimited");
                }
                else
                {
                    // Display validity
                    EditForm.FieldControls["CartItemValidToLabel"].Value = TimeZoneHelper.ConvertToUserTimeZone(ShoppingCartItem.CartItemValidTo, true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
                }
            }
            else
            {
                // Display conditions when the validity is displayed 
                EditForm.FieldControls["CartItemValidToLabel"].Value = GetString("com.orderitemedit.validtonotset");
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            // Disable UIForm if editing is disabled
            EditForm.Enabled = EditingEnabled;

            // Disable editing of unit count for product options
            EditForm.FieldControls["SKUUnits"].Enabled = !ShoppingCartItem.IsAccessoryProduct;
            EditForm.FieldControls["CartItemUnitPrice"].Value = ShoppingCartItem.UnitPrice;

            string itemText = ShoppingCartItem.CartItemText;

            // Text product options have text field displayed 
            if (sku.IsProductOption && !String.IsNullOrEmpty(itemText))
            {
                var controlName = (sku.SKUOptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextBox) ? "CartItemTextBox" : "CartItemTextArea";
                EditForm.FieldControls[controlName].Value = itemText;
            }

            // Hide unit price and units count edit for text option (price is already displayed in parent product)
            if (sku.IsTextAttribute)
            {
                EditForm.FieldsToHide.Add("CartItemUnitPrice");
                EditForm.FieldsToHide.Add("SKUUnits");
            }
        }
    }

    /// <summary>
    /// Closes dialog window and refresh parent window
    /// </summary>
    private void CloseDialogWindow()
    {
        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshCart(); CloseDialog();");
    }

    #endregion
}