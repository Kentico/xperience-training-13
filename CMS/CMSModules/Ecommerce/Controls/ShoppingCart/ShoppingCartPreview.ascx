<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPreview"
    CodeBehind="ShoppingCartPreview.ascx.cs" %>

<cms:LocalizedHeading runat="server" ID="headTitle" Level="3" ResourceString="ShoppingCartPreview.Title" EnableViewState="false" />
<div class="BlockContent">
    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
    <table width="100%">
        <tr runat="server" id="tblAddressPreview">
            <td>
                <asp:Panel ID="pnlBillingAddress" runat="server" EnableViewState="false">
                    <cms:LocalizedHeading runat="server" ID="headBilling" Level="4" ResourceString="Ecommerce.CartPreview.BillingAddressPanel" EnableViewState="false" DisplayColon="true" />
                    <div class="AddressPreview">
                        <asp:Literal ID="lblBill" runat="server" EnableViewState="false" />
                        <asp:PlaceHolder ID="plcIDs" runat="server" Visible="false">
                            <table border="0">
                                <tr>
                                    <td>
                                        <cms:LocalizedLabel ResourceString="OrderPreview.OrganizationID" ID="lblOrganizationID" runat="server" EnableViewState="false" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblOrganizationIDVal" runat="server" EnableViewState="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cms:LocalizedLabel ResourceString="OrderPreview.TaxRegistrationID" ID="lblTaxRegistrationID" runat="server" EnableViewState="false" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTaxRegistrationIDVal" runat="server" EnableViewState="false" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>
                    </div>
                </asp:Panel>
            </td>
            <td runat="server" id="tdShippingAddress">
                <asp:Panel ID="pnlShippingAddress" runat="server" EnableViewState="false">
                    <cms:LocalizedHeading runat="server" ID="headShipping" Level="4" ResourceString="Ecommerce.CartPreview.ShippingAddressPanel" EnableViewState="false" DisplayColon="true" />
                    <div class="AddressPreview">
                        <asp:Literal ID="lblShip" runat="server" EnableViewState="false" />
                    </div>
                </asp:Panel>
            </td>
            <td runat="server" id="tdCompanyAddress">
                <asp:Panel ID="pnlCompanyAddress" runat="server" Height="50%" EnableViewState="false">
                    <cms:LocalizedHeading runat="server" ID="headCompany" Level="4" ResourceString="Ecommerce.CartPreview.CompanyAddressPanel" EnableViewState="false" DisplayColon="false" />
                    <div class="AddressPreview">
                        <asp:Literal ID="lblCompany" runat="server" EnableViewState="false" />
                    </div>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td style="width: 50%;">
                <cms:LocalizedHeading runat="server" ID="LocalizedHeading1" Level="4" ResourceString="Ecommerce.CartContent.PaymentMethod" EnableViewState="false" />
                <cms:LocalizedLabel ID="lblPaymentMethodValue" runat="server" EnableViewState="false" />
            </td>
            <td>
                <asp:PlaceHolder ID="plcShippingOption" runat="server" EnableViewState="false">
                    <cms:LocalizedHeading runat="server" ID="LocalizedHeading2" Level="4" ResourceString="Ecommerce.CartContent.ShippingOption" EnableViewState="false" />
                    <cms:LocalizedLabel ID="lblShippingOptionValue" runat="server" EnableViewState="false" />
                </asp:PlaceHolder>
            </td>
        </tr>
        <tr>
            <td colspan="3">&nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <cms:UIGridView ID="gridData" runat="server" AutoGenerateColumns="false" CssClass="CartContentTable">
                    <Columns>
                        <asp:BoundField DataField="CartItemId" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="lblGuid" runat="server" Text='<%#Eval("CartItemGuid")%>' EnableViewState="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CartItemParentGuid" />
                        <asp:BoundField DataField="SKUID" />
                        <asp:TemplateField>
                            <HeaderStyle Width="100%" />
                            <ItemTemplate>
                                <%#GetSKUName(Eval("SKUName"), Eval("IsProductOption"), Eval("CartItemText"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Units">
                            <HeaderStyle Wrap="false" CssClass="TextRight" />
                            <ItemStyle Wrap="false" CssClass="TextRight" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderStyle-Wrap="false">
                            <ItemStyle Wrap="false" CssClass="TextRight" />
                            <HeaderStyle Wrap="false" CssClass="TextRight" />
                            <ItemTemplate>
                                <asp:Label ID="lblSKUPrice" runat="server" Text='<%#GetFormattedValue(Eval("UnitPrice"))%>'
                                    EnableViewState="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Wrap="false">
                            <ItemStyle Wrap="false" CssClass="TextRight" />
                            <HeaderStyle Wrap="false" CssClass="TextRight" />
                            <ItemTemplate>
                                <asp:Label ID="lblSubtotal" runat="server" Text='<%#GetFormattedValue(Eval("TotalPrice"))%>'
                                    EnableViewState="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </cms:UIGridView>
            </td>
        </tr>
        <tr>
            <td colspan="3" class="TextRight">
                <br />
                <table cellpadding="2" cellspacing="0" border="0" width="100%">
                    <%-- Order discount --%>
                    <asp:PlaceHolder ID="plcOrderDiscounts" runat="server">
                        <tr class="MultiBuyDiscount">
                            <td class="col1">&nbsp;
                            </td>
                            <td style="padding-right: 10px;">
                                <asp:PlaceHolder runat="server" ID="plcOrderDiscountNames" />
                            </td>
                            <td class="TextRight" style="white-space: nowrap; width: 100px;">
                                <asp:PlaceHolder runat="server" ID="plcOrderDiscountValues" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcShipping" runat="server" EnableViewState="false">
                        <tr class="TotalShipping">
                            <td class="col1">&nbsp;
                            </td>
                            <td style="padding-right: 10px;">
                                <cms:LocalizedLabel ResourceString="Ecommerce.CartContent.Shipping" ID="lblShipping" runat="server" EnableViewState="false" />
                            </td>
                            <td class="TextRight" style="white-space: nowrap;">
                                <asp:Label ID="lblShippingValue" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcTax" runat="server">
                        <tr class="TotalTax">
                            <td class="col1">&nbsp;
                            </td>
                            <td style="padding-right: 10px;">
                                <cms:LocalizedLabel ResourceString="ecommerce.shoppingcartcontent.totaltax" ID="tblTotalTax" runat="server" EnableViewState="false" />
                            </td>
                            <td class="TextRight" style="white-space: nowrap; width: 100px;">
                                <asp:Label ID="lblTotalTaxValue" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <%-- Other payments --%>
                    <asp:PlaceHolder ID="plcOtherPayments" runat="server">
                        <tr class="MultiBuyDiscount">
                            <td class="col1">&nbsp;
                            </td>
                            <td style="padding-right: 10px;">
                                <asp:PlaceHolder runat="server" ID="plcOtherPaymentsNames" />
                            </td>
                            <td class="TextRight" style="white-space: nowrap; width: 100px;">
                                <asp:PlaceHolder runat="server" ID="plcOtherPaymentsValues" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr class="TotalPrice">
                        <td class="col1">&nbsp;
                        </td>
                        <td style="padding-right: 10px; white-space: nowrap;">
                            <strong>
                                <cms:LocalizedLabel ResourceString="ecommerce.cartcontent.totalprice" ID="lblTotalPrice" runat="server" EnableViewState="false" />
                            </strong>
                        </td>
                        <td class="TextRight" style="white-space: nowrap;">
                            <strong>
                                <asp:Label ID="lblTotalPriceValue" runat="server" EnableViewState="false" /></strong>
                        </td>
                    </tr>
                </table>
                <br />
                <cms:UIGridView ID="gridTaxSummary" runat="server" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderStyle CssClass="TextLeft" />
                            <ItemStyle CssClass="TextLeft" Width="90%" />
                            <ItemTemplate>
                                <cms:LocalizedLabel ID="txtTaxName" runat="server" Text='<%#HTMLHelper.HTMLEncode(Eval("Name").ToString())%>'
                                    EnableViewState="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderStyle CssClass="TextRight" Width="10%" Wrap="false" />
                            <ItemStyle CssClass="TextRight" />
                            <ItemTemplate>
                                <asp:Label ID="lblTaxSummary" runat="server" Text='<%#GetFormattedValue(Eval("Value"))%>'
                                    EnableViewState="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </cms:UIGridView>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <div class="content-block-50">
                    <strong>
                        <cms:LocalizedLabel ResourceString="ecommerce.cartcontent.notelabel" ID="lblNote" runat="server" AssociatedControlID="txtNote" EnableViewState="false" />
                    </strong>
                    <br />
                    <cms:CMSTextArea ID="txtNote" runat="server" MaxLength="500" Width="100%" />
                </div>
                <cms:CMSCheckBox ID="chkSendEmail" runat="server" Visible="false" />
            </td>
        </tr>
    </table>
</div>
