using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject(OrderInfo.OBJECT_TYPE, "orderid")]
[UIElement(ModuleName.ECOMMERCE, "Orders.Billing")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Billing : CMSEcommercePage
{
    private const string CAPTURE_COMMAND_NAME = "capture";


    #region "Variables"

    private bool mShowMembershipWarning;
    private bool mShowEproductWarning;
    private bool mRecalculateOrder;
    private HeaderAction mCaptureAction;

    #endregion


    #region "Properties"

    /// <summary>
    /// Editing order object
    /// </summary>
    private OrderInfo Order
    {
        get
        {
            return EditedObject as OrderInfo;
        }
    }


    /// <summary>
    /// Represents value which user selected during editing. Property does not represents value stored in database.
    /// </summary>
    private bool OrderIsPaid
    {
        get
        {
            return ValidationHelper.GetBoolean(editOrderBilling.FieldControls["OrderIsPaid"].Value, false);
        }
    }


    /// <summary>
    /// Gets payment gateway provider for edited order.
    /// </summary>
    private IDelayedPaymentGatewayProvider PaymentGatewayProvider
    {
        get
        {
            if (Order == null)
            {
                return null;
            }

            var provider = CMSPaymentGatewayProvider.GetPaymentGatewayProvider<IDelayedPaymentGatewayProvider>(Order.OrderPaymentOptionID);
            if (provider != null)
            {
                provider.OrderId = Order.OrderID;
            }

            return provider;
        }
    }

    #endregion


    #region "Life cycle"

    /// <summary>
    /// Registers custom event handlers.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // Redirect to object does not exist page.
        if (Order == null)
        {
            EditedObject = null;
        }

        base.OnInit(e);

        editOrderBilling.OnCheckPermissions += editOrderBilling_OnCheckPermissions;
        editOrderBilling.OnBeforeSave += editOrderBilling_OnBeforeSave;
        editOrderBilling.OnAfterSave += editOrderBilling_OnAfterSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if order is not edited from another site
        CheckEditedObjectSiteID(Order.OrderSiteID);

        var paymentSelector = editOrderBilling.FieldControls["OrderPaymentOptionID"] as BaseObjectSelector;
        paymentSelector?.SetValue("ShoppingCart", ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID));

        if (IsCaptureVisible())
        {
            // Add button for payment capture
            mCaptureAction = new HeaderAction
            {
                Text = GetString("com.capture.payment"),
                CommandName = CAPTURE_COMMAND_NAME,
                ButtonStyle = ButtonStyle.Default
            };
            HeaderActions.AddAction(mCaptureAction);

            ComponentEvents.RequestEvents.RegisterForEvent(CAPTURE_COMMAND_NAME, ActionPerformed);
        }
    }


    /// <summary>
    /// Generates confirmation message.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!OrderIsPaid && Order.OrderIsPaid)
        {
            string confirmationMessage = GetConfirmationMessage();
            if (!string.IsNullOrEmpty(confirmationMessage))
            {
                editOrderBilling.SubmitButton.OnClientClick = "return confirm('" + confirmationMessage + "')";
            }
        }

        SetAddressValues();
        RefreshCapturePaymentButton();
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Checks modify permission.
    /// </summary>
    protected void editOrderBilling_OnCheckPermissions(object sender, EventArgs e)
    {
        // Check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
        }
    }


    /// <summary>
    /// Saving billing requires recalculation of order. Save action is executed by this custom code.
    /// </summary>
    protected void editOrderBilling_OnBeforeSave(object sender, EventArgs e)
    {
        // Load original data    
        var origPaymentID = OrderInfo.Provider.GetBySite(Order.OrderSiteID).Columns("OrderPaymentOptionID").WhereEquals("OrderID", Order.OrderID)
                                             .GetListResult<int>().FirstOrDefault();

        // Update data only if shopping cart data were changed
        var paymentID = ValidationHelper.GetInteger(editOrderBilling.FieldEditingControls["OrderPaymentOptionID"].DataValue, 0);

        // Check if recalculate order is needed
        mRecalculateOrder = origPaymentID != paymentID;
    }


    private void editOrderBilling_OnAfterSave(object sender, EventArgs e)
    {
        if (mRecalculateOrder)
        {
            var cart = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID);
            // Evaluate order data
            cart.Evaluate();
            // Update order data
            ShoppingCartInfoProvider.SetOrder(cart, true);
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Checks if order contains e-product or membership product. If so generates a confirmation message.
    /// </summary>
    private string GetConfirmationMessage()
    {
        // Get order items for this order
        var orderItems = OrderItemInfoProvider.GetOrderItems(Order.OrderID);

        mShowMembershipWarning = orderItems.Any(i => i.OrderItemSKU.SKUProductType == SKUProductTypeEnum.Membership);
        mShowEproductWarning = orderItems.Any(i => i.OrderItemSKU.SKUProductType == SKUProductTypeEnum.EProduct);

        // If one of the rollback warnings should be shown
        if (mShowEproductWarning || mShowMembershipWarning)
        {
            // Set standard warning message
            string paidUncheckWarning = GetString("order_edit_billing.orderispaiduncheckwarning");

            // Add memberships rollback warning message if required
            if (mShowMembershipWarning)
            {
                paidUncheckWarning += "\\n\\n- " + GetString("order_edit_billing.orderispaiduncheckwarningmemberships");
            }

            // Add e-products rollback warning message if required
            if (mShowEproductWarning)
            {
                paidUncheckWarning += "\\n\\n- " + GetString("order_edit_billing.orderispaiduncheckwarningeproducts");
            }

            return paidUncheckWarning;
        }

        return "";
    }


    private void SetAddressValues()
    {
        // Hide Select and Clear button which are visible by default for UniSelector
        var billingAddressSelector = editOrderBilling.FieldControls["OrderBillingAddressID"] as UniSelector;

        if (billingAddressSelector != null)
        {
            billingAddressSelector.Value = Order?.OrderBillingAddress?.AddressID;

            billingAddressSelector.ButtonSelect.Visible = false;
            billingAddressSelector.ButtonClear.Visible = false;
        }
    }


    /// <summary>
    /// Sets the visibility and enability of capture payment button.
    /// </summary>
    private void RefreshCapturePaymentButton()
    {
        if (mCaptureAction != null)
        {
            mCaptureAction.Enabled = IsCaptureEnabled();
        }
    }


    /// <summary>
    /// Reloads form data after successful capture.
    /// </summary>
    private void RefreshForm()
    {
        editOrderBilling.ReloadData();
    }


    /// <summary>
    /// Returns true when the capture payment button should be visible.
    /// </summary>
    private bool IsCaptureVisible()
    {
        return PaymentGatewayProvider != null && PaymentGatewayProvider.UseDelayedPayment();
    }


    /// <summary>
    /// Returns true when the capture payment button should be enabled.
    /// </summary>
    private bool IsCaptureEnabled()
    {
        return IsCaptureVisible() && PaymentGatewayProvider.IsPaymentAuthorized && Order != null && !Order.OrderIsPaid;
    }


    /// <summary>
    /// Command action handler.
    /// </summary>
    private void ActionPerformed(object sender, EventArgs e)
    {
        CapturePayment();
    }


    /// <summary>
    /// Captures the payment via payment gateway provider.
    /// </summary>
    private void CapturePayment()
    {
        if (PaymentGatewayProvider == null || Order?.OrderPaymentResult == null)
        {
            return;
        }

        var result = PaymentGatewayProvider.CapturePayment();

        ShowCapturePaymentResult(result);
        RefreshForm();
    }


    /// <summary>
    /// Shows result of the capture operation.
    /// </summary>
    private void ShowCapturePaymentResult(PaymentResultInfo result)
    {
        if (result.PaymentIsCompleted)
        {
            ShowConfirmation(result.PaymentDescription);
        }
        else
        {
            ShowError(result.PaymentDescription);
        }
    }

    #endregion
}