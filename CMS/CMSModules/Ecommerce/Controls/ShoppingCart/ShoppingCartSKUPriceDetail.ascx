<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartSKUPriceDetail_Control" CodeBehind="ShoppingCartSKUPriceDetail.ascx.cs" %>

<%--Totals--%>
<table class="table table-hover">
    <thead>
        <tr>
            <th colspan="2">
                <asp:Label ID="lblProductName" runat="server" EnableViewState="false" />
            </th>
        </tr>
    </thead>
    <tbody>
        <tr class="PriceDetailSubtotal">
            <td>
                <cms:LocalizedLabel ID="lblStandardPrice" ResourceString="ProductPriceDetail.StandardPrice" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblStandardPriceValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblTotalDiscount" ResourceString="productpricedetail.catalogdiscount" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblTotalDiscountValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr class="PriceDetailSubtotal">
            <td>
                <cms:LocalizedLabel ID="lblPrice" ResourceString="ProductPriceDetail.Price" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblPriceValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr class="PriceDetailSubtotal">
            <td>
                <cms:LocalizedLabel ResourceString="ProductPriceDetail.ProductUnits" ID="lblProductUnits" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblProductUnitsValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr class="PriceDetailSubtotal">
            <td>
                <cms:LocalizedLabel ResourceString="productpricedetail.itemdiscount" ID="lblItemDiscount" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblItemDiscountValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr class="PriceDetailHeader">
            <td>
                <cms:LocalizedLabel ResourceString="ProductPriceDetail.TotalPrice" ID="lblTotalPrice" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblTotalPriceValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
    </tbody>
</table>


<%--Catalog-level Discounts--%>
<cms:UIGridView ID="gridDiscounts" runat="server" AutoGenerateColumns="false" CssClass="PriceDetailSummaryTable">
    <columns>
            <asp:TemplateField>
                <HeaderStyle />
                <ItemTemplate>
                    <%#GetFormattedName(Eval("Name"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#GetFormattedValue(Eval("Value"))%>
                </ItemTemplate>
            </asp:TemplateField>
        </columns>
</cms:UIGridView>

<%--Item-level Discounts--%>
<cms:UIGridView ID="gridItemDiscounts" runat="server" AutoGenerateColumns="false" CssClass="PriceDetailSummaryTable">
    <columns>
            <asp:TemplateField>
                <HeaderStyle />
                <ItemTemplate>
                    <%#GetFormattedName(Eval("Name"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#GetFormattedValue(Eval("Value"))%>
                </ItemTemplate>
            </asp:TemplateField>
        </columns>
</cms:UIGridView>
