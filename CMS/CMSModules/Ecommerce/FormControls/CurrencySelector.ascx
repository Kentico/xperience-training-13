<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_CurrencySelector"  Codebehind="CurrencySelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" RenderMode="Block">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ShortID="c" DisplayNameFormat="{%CurrencyDisplayName%}" AdditionalColumns="CurrencyID"
            ObjectType="ecommerce.currency" ResourcePrefix="currencyselector" SelectionMode="SingleDropDownList"
            AllowEmpty="false" UseUniSelectorAutocomplete="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
