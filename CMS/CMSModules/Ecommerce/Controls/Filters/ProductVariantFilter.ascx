<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_Filters_ProductVariantFilter" 
     Codebehind="ProductVariantFilter.ascx.cs" %>

<asp:Panel ID="pnlContainer" runat="server" DefaultButton="btnFilter" class ="form-horizontal form-filter">
    <asp:Panel ID="pnlFilterOptions" runat="server">
        <asp:Panel ID="pnlVariantNameNumber" runat="server" CssClass="form-group">
            <asp:Panel ID="pnlLblVariantNameNumber" runat="server" CssClass="filter-form-label-cell">
                <asp:Label ID="lblVariantNameNumber" runat="server" CssClass="control-label"/>
            </asp:Panel>
            <asp:Panel ID="pnlTxtVariantNameNumber" runat="server" CssClass="filter-form-value-cell-wide">
                <cms:CMSTextBox ID="txtVariantNameNumber" runat="server" OnTextChanged="btnFilter_Click"/>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="pnlBottom" CssClass="form-group form-group-buttons" runat="server">
        <div class="filter-form-buttons-cell-wide">        
            <cms:CMSButton ID="btnAll" runat="server" ButtonStyle="Default" EnableViewState="false" OnClientClick="Check(true);return false;" />
            <cms:CMSButton ID="btnNone" runat="server" ButtonStyle="Default" EnableViewState="false" OnClientClick="Check(false);return false;" />
            <cms:CMSButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" Visible="false"/>
            <cms:CMSButton ID="btnFilter" runat="server" ButtonStyle="Primary" EnableViewState="false" />
        </div>
    </asp:Panel>
</asp:Panel>