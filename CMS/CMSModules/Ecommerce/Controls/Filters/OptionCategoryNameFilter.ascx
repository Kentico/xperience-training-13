<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="OptionCategoryNameFilter.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_Filters_OptionCategoryNameFilter" %>

<asp:Panel runat="server" ID="pnlFilter">
    <div class="form-condition-cell-generated">
        <cms:CMSDropDownList ID="filter" runat="server" CssClass="ContentDropdown" />
    </div>
    <div class="form-value-cell-generated">
        <cms:CMSTextBox ID="txtName" runat="server" />
    </div>
</asp:Panel>