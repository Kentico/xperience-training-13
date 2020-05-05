<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ProductOptions_ProductOptionSelector"
     Codebehind="ProductOptionSelector.ascx.cs" %>
<cms:LocalizedLabel ID="lblNoProductOptions" runat="server" Visible="false"
    EnableViewState="false" ResourceString="optioncategory_edit.noproductoptions" />
<asp:Panel ID="pnlContainer" runat="server" CssClass="ProductOptionSelectorContainer form-group">
    <asp:Panel ID="plnError" runat="server" Visible="false" CssClass="OptionCategoryErrorContainer">
        <cms:LocalizedLabel ID="lblError" runat="server" CssClass="ErrorLabel OptionCategoryError"
            EnableViewState="false" />
    </asp:Panel>
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel ID="lblCategName" runat="server" CssClass="OptionCategoryName control-label"
            EnableViewState="false" />
    </div>
    <asp:Panel ID="pnlSelector" runat="server" CssClass="ProductOptionSelector editing-form-value-cell" />
    <asp:Label ID="lblCategDescription" runat="server" CssClass="OptionCategoryDescription"
        EnableViewState="false" />
</asp:Panel>
