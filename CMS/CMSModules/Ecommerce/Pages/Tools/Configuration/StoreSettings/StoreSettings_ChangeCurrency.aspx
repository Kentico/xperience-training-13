<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_ChangeCurrency"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
     Codebehind="StoreSettings_ChangeCurrency.aspx.cs" %>

<%@ Register Src="~/CMSModules/ECommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal">
        <asp:PlaceHolder runat="server" ID="plcOldCurrency">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblOldMainLabel" runat="server" EnableViewState="false" ResourceString="StoreSettings_ChangeCurrency.OldMainCurrency" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblOldMainCurrency" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblNewMainCurrency" runat="server" EnableViewState="false"
                    ResourceString="StoreSettings_ChangeCurrency.NewMainCurrency" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CurrencySelector runat="server" ID="currencyElem" UseNameForSelection="false" AddAllItemsRecord="false" AddSiteDefaultCurrency="false"
                    ExcludeSiteDefaultCurrency="true" IsLiveSite="false" ShowAllItems="true" EnsureSelectedItem="true" />
            </div>
        </div>
    </div>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOk_Click"
        EnableViewState="false" />
</asp:Content>
