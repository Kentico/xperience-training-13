using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject(OrderInfo.OBJECT_TYPE, "orderId")]
[UIElement(ModuleName.ECOMMERCE, "Orders.Shipping")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Shipping : CMSEcommercePage
{
    private ShoppingCartInfo mShoppingCartFromOrder;


    /// <summary>
    /// Shopping cart created from edited order.
    /// </summary>
    private ShoppingCartInfo ShoppingCartFromOrder => mShoppingCartFromOrder ?? (mShoppingCartFromOrder = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID));


    /// <summary>
    /// Editing order object
    /// </summary>
    private OrderInfo Order => orderShippingForm.EditedObject as OrderInfo;


    private bool IsTaxBasedOnShippingAddress => (ECommerceSettings.ApplyTaxesBasedOn(Order.OrderSiteID) == ApplyTaxBasedOnEnum.ShippingAddress);


    private BaseObjectSelector ShippingOptionSelector => orderShippingForm.FieldControls["OrderShippingOptionID"] as BaseObjectSelector;


    private UniSelector ShippingAddressSelector => orderShippingForm.FieldControls["OrderShippingAddressID"] as UniSelector;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        orderShippingForm.OnCheckPermissions += CheckPermissions;
        orderShippingForm.OnBeforeSave += OnBeforeSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if order is not edited from another site
        CheckEditedObjectSiteID(Order.OrderSiteID);

        // Hide Select and Clear button which are visible by default for UniSelector
        if (ShippingAddressSelector != null)
        {
            ShippingAddressSelector.Value = Order?.OrderShippingAddress?.AddressID;

            ShippingAddressSelector.ButtonSelect.Visible = false;
            ShippingAddressSelector.ButtonClear.Visible = false;
        }

        ShippingOptionSelector?.SetValue("ShoppingCart", ShoppingCartFromOrder);
    }


    protected void CheckPermissions(object sender, EventArgs e)
    {
        if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
        }
    }


    protected void OnBeforeSave(object sender, EventArgs e)
    {
        if ((Order == null) || (ShippingAddressSelector == null) || (ShippingOptionSelector == null) || (ShoppingCartFromOrder == null))
        {
            return;
        }

        // Get current values
        var addressID = ValidationHelper.GetInteger(ShippingAddressSelector.Value, 0);
        var shippingOptionID = ValidationHelper.GetInteger(ShippingOptionSelector.Value, 0);

        // Is shipping needed?
        var isShippingNeeded = ShoppingCartFromOrder.IsShippingNeeded;

        // If shipping address is required
        if (isShippingNeeded || IsTaxBasedOnShippingAddress)
        {
            // If shipping address is not set
            if (addressID <= 0)
            {
                // Show error message
                ShowError(GetString("Order_Edit_Shipping.NoAddress"));
                return;
            }
        }

        try
        {
            // Check if original order shipping option was changed, the check cannot be done on edited object because edited object contains current value (not the original one)
            if (ShoppingCartFromOrder.ShoppingCartShippingOptionID != shippingOptionID)
            {
                PaymentOptionInfo payment = PaymentOptionInfo.Provider.Get(Order.OrderPaymentOptionID);

                // Check if payment is allowed with no shipping
                if ((payment != null) && (!payment.PaymentOptionAllowIfNoShipping && shippingOptionID == 0))
                {
                    // Set payment method to none and display warning
                    ShoppingCartFromOrder.ShoppingCartPaymentOptionID = 0;
 
                    var paymentMethodName = ResHelper.LocalizeString(payment.PaymentOptionDisplayName, null, true);
                    var shippingOptionName = HTMLHelper.HTMLEncode(ShippingOptionSelector.ValueDisplayName);

                    ShowWarning(String.Format(GetString("com.shippingoption.paymentsetnone"), paymentMethodName, shippingOptionName));
                }

                // Set order new properties
                ShoppingCartFromOrder.ShoppingCartShippingOptionID = shippingOptionID;

                // Evaluate order data
                ShoppingCartFromOrder.Evaluate();

                // Update order data
                ShoppingCartInfoProvider.SetOrder(ShoppingCartFromOrder, true);
            }

            var newTrackingNumber = ValidationHelper.GetString(orderShippingForm.FieldEditingControls["OrderTrackingNumber"].DataValue, String.Empty).Trim();

            if (!newTrackingNumber.Equals(ShoppingCartFromOrder.Order.OrderTrackingNumber, StringComparison.InvariantCulture))
            {
                // Update on the current order instance
                var order = ShoppingCartFromOrder.Order;
                order.OrderTrackingNumber = newTrackingNumber;
                OrderInfo.Provider.Set(Order);
            }

            // Show message
            ShowChangesSaved();

            // Stop automatic saving action
            orderShippingForm.StopProcessing = true;
        }
        catch (Exception ex)
        {
            // Show error message
            ShowError(ex.Message);
        }
    }
}