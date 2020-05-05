<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ExchangeRates_ExchangeTable_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Exchange table - properties"
     Codebehind="ExchangeTable_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblExchangeTableDisplayName" EnableViewState="false"
                    ResourceString="general.displayname" DisplayColon="true" AssociatedControlID="txtExchangeTableDisplayName" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtExchangeTableDisplayName" runat="server"
                    MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" Display="Dynamic"
                    ValidationGroup="Exchange" ControlToValidate="txtExchangeTableDisplayName:cntrlContainer:textbox"
                    EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblExchangeTableValidFrom" EnableViewState="false"
                    ResourceString="ExchangeTable_Edit.ExchangeTableValidFromLabel" AssociatedControlID="dtPickerExchangeTableValidFrom" />
            </div>
            <div class="editing-form-value-cell">
                <cms:DateTimePicker ID="dtPickerExchangeTableValidFrom" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblExchangeTableValidTo" EnableViewState="false"
                    ResourceString="ExchangeTable_Edit.ExchangeTableValidToLabel" AssociatedControlID="dtPickerExchangeTableValidTo" />
            </div>
            <div class="editing-form-value-cell">
                <cms:DateTimePicker ID="dtPickerExchangeTableValidTo" runat="server" EnableViewState="false" />
            </div>
        </div>
    </div>

    <div class="form-horizontal">
        <asp:PlaceHolder ID="plcGrid" runat="server">
            <cms:LocalizedHeading Level="4" runat="server" ID="lblRates" EnableViewState="false" ResourceString="ExchangeTable_Edit.ExchangeRates" />
            <asp:PlaceHolder ID="plcRateFromGlobal" runat="server">
                <h5>
                    <cms:LocalizedLabel ID="lblFromGlobalToMain" runat="server" EnableViewState="false" ShowRequiredMark="true" />
                    <span class="info-icon info-icon-heading">
                        <cms:LocalizedLabel runat="server" ResourceString="ExchangeTable_Edit.ExchangeRateHelp" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="iconHelpGlobalExchangeRate" runat="server" CssClass="icon-question-circle" />
                    </span>
                </h5>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <table class="table table-hover table-width-30">
                            <thead>
                                <tr>
                                    <th scope="col">
                                        <cms:LocalizedLabel ID="lblToCurrency" runat="server" ResourceString="ExchangeTable_Edit.ToCurrency" EnableViewState="false" />
                                    </th>
                                    <th scope="col">
                                        <cms:LocalizedLabel ID="lblRateValue" runat="server" ResourceString="ExchangeTable_Edit.RateValue" EnableViewState="false" />
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblSiteMainCurrency" runat="server" />
                                    </td>
                                    <td>
                                        <cms:CMSTextBox ID="txtGlobalExchangeRate" runat="server" MaxLength="19" EnableViewState="false" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcNoCurrency" runat="server" Visible="false">
                <cms:LocalizedHeading Level="4" runat="server" ID="lblNoCurrencies" EnableViewState="false" ResourceString="com.currencies.nosuitablefound" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcSiteRates" runat="server">
                <h5>
                    <cms:LocalizedLabel ID="lblMainToSite" runat="server" EnableViewState="false" />
                    <span class="info-icon info-icon-heading">
                        <cms:LocalizedLabel runat="server" ResourceString="ExchangeTable_Edit.ExchangeRateHelp" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="iconHelpMainExchangeRate" runat="server" CssClass="icon-question-circle" />
                    </span>
                </h5>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:UIGridView ID="editGrid" CssClass="table-width-30" runat="server" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="CurrencyCode"></asp:BoundField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </cms:UIGridView>
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false" ValidationGroup="Exchange" />
            </div>
        </div>
    </div>
</asp:Content>
