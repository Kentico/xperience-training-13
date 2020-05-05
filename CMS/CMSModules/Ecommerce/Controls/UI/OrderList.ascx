<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_OrderList"
     Codebehind="OrderList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridElem" IsLiveSite="false" OrderBy="OrderDate DESC" FilterLimit="0"
            DisplayFilter="true" RememberStateByParam="customerId" RememberDefaultState="true"
            Columns="OrderID,OrderInvoiceNumber,OrderCustomerID,OrderDate,OrderGrandTotal,OrderGrandTotalInMainCurrency,OrderPaymentOptionID,OrderIsPaid,OrderShippingOptionID,
            OrderCurrencyID,OrderTrackingNumber,OrderNote,OrderStatusID"
            ObjectType="ecommerce.order">
            <GridActions>
                <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                    Confirmation="$General.ConfirmDelete$" />
                <ug:Action Name="previous" Caption="$Unigrid.Order.Actions.PreviousStatus$" FontIconClass="icon-chevron-left" />
                <ug:Action Name="next" Caption="$Unigrid.Order.Actions.NextStatus$" FontIconClass="icon-chevron-right" />
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
                <ug:Column Name="MainCurrencyPrice" Source="##ALL##" ExternalSourceName="GrandTotalInMainCurrency"
                    Caption="$Unigrid.Order.Columns.OrderTotalPrice$" Sort="OrderGrandTotalInMainCurrency"
                    Wrap="false" CssClass="TextRight" />
                <ug:Column Name="OrderPrice" Source="##ALL##" ExternalSourceName="GrandTotalInOrderCurrency"
                    Caption="$com.orderlist.ordercurrencycaption$" Wrap="false"
                    CssClass="TextRight" />
                <ug:Column Name="OrderStatus" Source="OrderStatusID" ExternalSourceName="statusName"
                    Caption="$Unigrid.Order.Columns.OrderStatusID$" AllowSorting="false" Wrap="false" />
                <ug:Column Name="IsPaid" Source="OrderIsPaid" ExternalSourceName="#yesno" Caption="$Unigrid.Order.Columns.OrderIsPaid$"
                    Wrap="false" />
                <ug:Column Name="Note" Source="OrderNote" ExternalSourceName="Note" Caption="$com.orderlist.notecaption$"
                    Wrap="false">
                    <Tooltip Encode="true" Source="OrderNote" />
                </ug:Column>
                <ug:Column Source="OrderID" Visible="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" FilterPath="~/CMSModules/Ecommerce/Controls/Filters/OrderFilter.ascx" />
        </cms:UniGrid>
    </ContentTemplate>
</cms:CMSUpdatePanel>
