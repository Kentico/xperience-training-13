<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartOrderAddresses"
     Codebehind="ShoppingCartOrderAddresses.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/CountrySelector.ascx" TagName="CountrySelector" TagPrefix="cms" %>

<cms:LocalizedHeading runat="server" ID="headTitle" Level="3" ResourceString="ShoppingCart.BillingAddress" EnableViewState="false" />
<div class="BlockContent">
    <cms:LocalizedLabel ID="lblError" runat="server" Visible="false" CssClass="ErrorLabel" EnableViewState="false" />
    <asp:PlaceHolder runat="server" ID="plcBillingAddress">
        <div class="form-horizontal">
            <asp:PlaceHolder ID="plhBillAddr" runat="server" Visible="false">
                <div class="form-group">
                    <%--Billing address--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblBillingAddr" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="drpBillingAddr" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSDropDownList ID="drpBillingAddr" runat="server" CssClass="DropDownField" AutoPostBack="true"
                            DataTextField="AddressName" DataValueField="AddressID" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <%--Billing address name--%>
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblBillingName" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                        AssociatedControlID="txtBillingName" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtBillingName" runat="server" MaxLength="200" />
                </div>
            </div>
            <div class="form-group">
                <%--Billing address lines--%>
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblBillingAddrLine" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                        AssociatedControlID="txtBillingAddr1" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtBillingAddr1" runat="server" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblBillingAddrLine2" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                        AssociatedControlID="txtBillingAddr2" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtBillingAddr2" runat="server" MaxLength="100" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <%--Billing city--%>
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblBillingCity" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                        AssociatedControlID="txtBillingCity" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtBillingCity" runat="server" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <%--Billing ZIP--%>
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblBillingZip" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                        AssociatedControlID="txtBillingZip" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtBillingZip" runat="server" MaxLength="20" />
                </div>
            </div>
            <div class="form-group">
                <%--Billing country and state--%>
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblBillingCountry" runat="server" CssClass="ContentLabel control-label"
                        EnableViewState="false" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSUpdatePanel ID="pnlUpdateCountry1" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:CountrySelector ID="CountrySelector1" runat="server" UseCodeNameForSelection="false"
                                IsLiveSite="false" AddSelectCountryRecord="true" DisplayAllItems="true" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </div>
            <div class="form-group">
                <%--Billing phone--%>
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblBillingPhone" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                        AssociatedControlID="txtBillingPhone" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtBillingPhone" runat="server" MaxLength="26" EnableViewState="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</div>

<asp:PlaceHolder runat="server" ID="plcShippingAddress">
    <div class="BlockContent">
        <cms:CMSCheckBox ID="chkShippingAddr" runat="server" Checked="false" OnCheckedChanged="chkShippingAddr_CheckedChanged"
            AutoPostBack="true" />
    </div>
    <asp:PlaceHolder ID="plhShipping" runat="server" Visible="false">
        <cms:LocalizedHeading ID="lblShippingTitle" runat="server" Level="3" CssClass="BlockTitle" EnableViewState="false" />
        <div class="BlockContent">
            <div class="form-horizontal">
                <asp:PlaceHolder ID="plhShippAddr" runat="server" Visible="false">
                    <div class="form-group">
                        <%--Shipping address--%>
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblShippingAddr" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                                AssociatedControlID="drpShippingAddr" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpShippingAddr" runat="server" CssClass="DropDownField" AutoPostBack="true"
                                DataTextField="AddressName" DataValueField="AddressID" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <%--Shipping address name--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblShippingName" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtShippingName" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtShippingName" runat="server" MaxLength="200" />
                    </div>
                </div>
                <div class="form-group">
                    <%--Shipping address lines--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblShippingAddrLine" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtShippingAddr1" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtShippingAddr1" runat="server" MaxLength="100" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblShippingAddrLine2" runat="server" EnableViewState="false" CssClass="ContentLabel control-label"
                            AssociatedControlID="txtShippingAddr2" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtShippingAddr2" runat="server" MaxLength="100" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <%--Shipping city--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblShippingCity" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtShippingCity" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtShippingCity" runat="server" MaxLength="100" />
                    </div>
                </div>
                <div class="form-group">
                    <%--Shipping ZIP--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblShippingZip" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtShippingZip" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtShippingZip" runat="server" MaxLength="20" />
                    </div>
                </div>
                <div class="form-group">
                    <%--Shipping country--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblShippingCountry" runat="server" CssClass="ContentLabel control-label"
                            EnableViewState="false" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSUpdatePanel ID="CMSUpdatePanel2" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <cms:CountrySelector ID="CountrySelector2" runat="server" UseCodeNameForSelection="false"
                                    IsLiveSite="false" AddSelectCountryRecord="true" DisplayAllItems="true" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </div>
                <div class="form-group">
                    <%--Shipping phone--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblShippingPhone" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtShippingPhone" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtShippingPhone" runat="server" MaxLength="26" />
                    </div>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plcCompanyAll" runat="server">
    <cms:LocalizedLabel ID="lblCompanyAddressTitle" runat="server" CssClass="BlockTitle" EnableViewState="false" />
    <div class="BlockContent">
        <cms:CMSCheckBox ID="chkCompanyAddress" runat="server" Checked="false" OnCheckedChanged="chkCompanyAddress_CheckedChanged"
            AutoPostBack="true" />
    </div>
    <asp:PlaceHolder ID="plcCompanyDetail" runat="server" Visible="false">
        <div class="BlockContent">
            <div class="form-horizontal">
                <asp:PlaceHolder ID="plcCompanyAddress" runat="server" Visible="false">
                    <div class="form-group">
                        <%--Company address--%>
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblCompanyAddress" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                                AssociatedControlID="drpCompanyAddress" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpCompanyAddress" runat="server" CssClass="DropDownField"
                                AutoPostBack="true" DataTextField="AddressName" DataValueField="AddressID" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <%--Company address name--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblCompanyName" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtCompanyName" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtCompanyName" runat="server" MaxLength="200" />
                    </div>
                </div>
                <div class="form-group">
                    <%--Company address lines--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblCompanyLines" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtCompanyLine1" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtCompanyLine1" runat="server" MaxLength="100" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblCompanyLines2" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtCompanyLine2" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtCompanyLine2" runat="server" MaxLength="100" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <%--Company city--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblCompanyCity" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtCompanyCity" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtCompanyCity" runat="server" MaxLength="100" />
                    </div>
                </div>
                <div class="form-group">
                    <%--Company ZIP--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblCompanyZip" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtCompanyZip" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtCompanyZip" runat="server" MaxLength="20" />
                    </div>
                </div>
                <div class="form-group">
                    <%--Company country--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblCompanyCountry" runat="server" CssClass="ContentLabel control-label"
                            EnableViewState="false" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSUpdatePanel ID="CMSUpdatePanel3" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <cms:CountrySelector ID="CountrySelector3" runat="server" UseCodeNameForSelection="false"
                                    IsLiveSite="false" AddSelectCountryRecord="true" DisplayAllItems="true" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </div>
                <div class="form-group">
                    <%--Company phone--%>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblCompanyPhone" runat="server" CssClass="ContentLabel control-label" EnableViewState="false"
                            AssociatedControlID="txtCompanyPhone" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtCompanyPhone" runat="server" MaxLength="100" EnableViewState="false" />
                    </div>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:PlaceHolder>
