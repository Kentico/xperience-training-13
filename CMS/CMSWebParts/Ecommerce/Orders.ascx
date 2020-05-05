<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Ecommerce/Orders.ascx.cs" Inherits="CMSWebParts_Ecommerce_Orders" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:MessagesPlaceHolder ID="plcMessages" runat="server" WrapperControlID="pnlMessages" />
<cms:UniGrid runat="server" ID="gridElem" ObjectType="ecommerce.order" RememberStateByParam="customerId" IsLiveSite="false"
    RememberDefaultState="true" Columns="OrderID,OrderInvoiceNumber,OrderDate,OrderStatusID,OrderCustomerID,OrderCurrencyID,
            OrderTotalPriceInMainCurrency,OrderTotalPrice,OrderPaymentOptionID,OrderIsPaid,OrderShippingOptionID,OrderTrackingNumber,OrderNote,OrderSiteID">
    <GridActions>
        <ug:Action Name="edit" ExternalSourceName="Edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
            Confirmation="$General.ConfirmDelete$" />
        <ug:Action Name="previous" ExternalSourceName="StatusMovePrevious" Caption="$Unigrid.Order.Actions.PreviousStatus$"
            FontIconClass="icon-chevron-left" />
        <ug:Action Name="next" ExternalSourceName="StatusMoveNext" Caption="$Unigrid.Order.Actions.NextStatus$"
            FontIconClass="icon-chevron-right" />
    </GridActions>
    <GridColumns>
        <ug:Column Name="IDAndInvoice" Source="##ALL##" ExternalSourceName="IDAndInvoice"
            Caption="$Unigrid.Order.Columns.OrderID$" Sort="OrderID" Wrap="false" />
        <ug:Column Name="Customer" Source="OrderCustomerID" ExternalSourceName="#transform: ecommerce.customer : {% CustomerInfoName %}" Caption="$Unigrid.Order.Columns.OrderCustomerFullName$"
            AllowSorting="false" Wrap="false">
            <Tooltip Encode="true" Source="OrderCustomerID" ExternalSourceName="#transform: ecommerce.customer : {% CustomerEmail %}" />
        </ug:Column>
        <ug:Column Name="Date" Source="OrderDate" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.Order.Columns.OrderDate$"
            Wrap="false" />
        <ug:Column Name="MainCurrencyPrice" Source="##ALL##" ExternalSourceName="TotalPriceInMainCurrency" Caption="$Unigrid.Order.Columns.OrderTotalPrice$"
            Sort="OrderTotalPriceInMainCurrency" Wrap="false" CssClass="TextRight" />
        <ug:Column Name="OrderPrice" Source="##ALL##" ExternalSourceName="TotalPriceInOrderPrice" Caption="$com.orderlist.ordercurrencycaption$"
            Wrap="false" CssClass="TextRight" />
        <ug:Column Name="OrderStatus" Source="OrderStatusID" ExternalSourceName="OrderStatus"
                    Caption="$Unigrid.Order.Columns.OrderStatusID$" AllowSorting="false" Wrap="false" />
        <ug:Column Name="PaymentOption" Source="OrderPaymentOptionID" ExternalSourceName="#transform: ecommerce.paymentoption : {% PaymentOptionDisplayName %}"
            Caption="$com.orderswidget.paymentmethod$" Wrap="false" AllowSorting="false" />
        <ug:Column Name="IsPaid" Source="OrderIsPaid" ExternalSourceName="#yesno" Caption="$Unigrid.Order.Columns.OrderIsPaid$"
            Wrap="false" />
        <ug:Column Name="ShippingOption" Source="OrderShippingOptionID" ExternalSourceName="#transform: ecommerce.shippingoption : {% ShippingOptionDisplayName %}"
            Caption="$com.orderswidget.shippingoption$" Wrap="false" AllowSorting="false" />
        <ug:Column Name="TrackingNumber" Source="OrderTrackingNumber" Caption="$com.orderswidget.trackingnumber$"
            Wrap="false" />
        <ug:Column Name="Note" Source="OrderNote" ExternalSourceName="Note" Caption="$com.orderlist.notecaption$"
            Wrap="false">
            <Tooltip Encode="true" Source="OrderNote" />
        </ug:Column>
        <ug:Column CssClass="filling-column" />
    </GridColumns>
</cms:UniGrid>
