<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="OrderFilter.ascx.cs" Inherits="CMSModules_Ecommerce_Controls_Filters_OrderFilter" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/OrderStatusSelector.ascx" TagName="OrderStatusSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/ShippingSelector.ascx" TagName="ShippingSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnFilter">
    <div class="form-horizontal form-filter">
        <%-- Order ID or Invoice number --%>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblOrderID" runat="server" EnableViewState="false" ResourceString="OrderList.OrderIdLabel" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtOrderId" runat="server" MaxLength="20" EnableViewState="false" />
            </div>
        </div>
        <%-- Customer name or company or e-mail --%>
        <asp:PlaceHolder ID="plcCustomerFilter" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCustomerNameOrCompany" runat="server" EnableViewState="false"
                        ResourceString="OrderList.CustomerLastName" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSTextBox ID="txtCustomerNameOrCompanyOrEmail" WatermarkText="{$com.orders.namecompanyoremail$}" MaxLength="254" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <%-- Status --%>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblStatus" runat="server" EnableViewState="false" ResourceString="OrderList.StatusLabel" />
            </div>
            <div class="filter-form-value-cell">
                <cms:OrderStatusSelector runat="server" AddAllItemsRecord="true" ID="statusSelector"
                    UseNameForSelection="false" IsLiveSite="false" DisplayOnlyEnabled="false" />
            </div>
        </div>
        <%-- Order is paid --%>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblIsPaid" runat="server" ResourceString="com.orderlist.orderispaid"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSDropDownList runat="server" ID="drpOrderIsPaid" UseResourceStrings="true"
                    CssClass="DropDownField" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcAdvancedGroup" runat="server">
            <%-- Created from --%>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCreatedFrom" runat="server" ResourceString="com.orderlist.createdfrom"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:DateTimePicker ID="dtpFrom" runat="server" EnableViewState="false" DisplayNow="false" />
                </div>
            </div>
            <%-- Created to --%>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCreatedTo" runat="server" ResourceString="com.orderlist.createdto"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:DateTimePicker ID="dtpCreatedTo" runat="server" DisplayNow="false" />
                </div>
            </div>
            <%-- Currency --%>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCurrency" runat="server" ResourceString="com.orderlist.currency"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CurrencySelector runat="server" ID="CurrencySelector"
                        UseNameForSelection="false" DisplayOnlyEnabled="false" DisplayOnlyWithExchangeRate="false" EnsureSelectedItem="true" />
                </div>
            </div>
            <%-- Payment method --%>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblPaymentMethod" runat="server" ResourceString="com.orderlist.paymentmethod"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:PaymentSelector runat="server" ID="PaymentSelector"
                        DisplayOnlyEnabled="false" UseNameForSelection="false" />
                </div>
            </div>
            <%-- Shipping options --%>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblShippingOptions" runat="server" ResourceString="com.orderlist.shippingoptions"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:ShippingSelector runat="server" ID="ShippingSelector" DisplayOnlyEnabled="false"
                        AddAllItemsRecord="true" UseNameForSelection="false" />
                </div>
            </div>
            <%-- Tracking number --%>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblTrackingNumber" runat="server" ResourceString="com.orderlist.trackingnumber"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSTextBox ID="txtTrackingNumber" runat="server" MaxLength="100" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-label-cell">
                <asp:PlaceHolder ID="plcAdvancedFilter" runat="server">
                    <cms:LocalizedLinkButton ID="btnAdvancedFilter" runat="server" EnableViewState="false" ResourceString="general.displayadvancedfilter"
                        OnClick="btnAdvancedFilter_Click" CssClass="simple-advanced-link" />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcSimpleFilter" runat="server">
                    <cms:LocalizedLinkButton ID="btnSimpleFilter" runat="server" EnableViewState="false" ResourceString="general.displaysimplefilter"
                        OnClick="btnAdvancedFilter_Click" CssClass="simple-advanced-link" />
                </asp:PlaceHolder>
            </div>
            <div class="filter-form-buttons-cell-narrow">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:CMSButton ID="btnFilter" runat="server" ButtonStyle="Primary" EnableViewState="false" />
            </div>
        </div>

    </div>
</asp:Panel>
