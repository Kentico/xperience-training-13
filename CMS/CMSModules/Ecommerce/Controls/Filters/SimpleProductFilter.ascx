<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SimpleProductFilter.ascx.cs" Inherits="CMSModules_Ecommerce_Controls_Filters_SimpleProductFilter" %>
<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnShow">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblProductName" runat="server" EnableViewState="false" ResourceString="com.sku.ProductNameOrNumber" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtProductName" runat="server" MaxLength="450"
                    EnableViewState="false" />
            </div>
        </div>               
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" OnClick="btnReset_Click"/>
                <cms:CMSButton ID="btnShow" runat="server" ButtonStyle="Primary" EnableViewState="false" OnClick="btnShow_Click" />
            </div>
        </div>
    </div>
</asp:Panel>
