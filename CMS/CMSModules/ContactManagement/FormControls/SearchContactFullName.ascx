<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SearchContactFullName.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_SearchContactFullName" %>
<asp:Panel CssClass="form-horizontal form-filter" runat="server" ID="pnlSearch" DefaultButton="btnSelect">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblSearch" runat="server" EnableViewState="false" ResourceString="om.contact.name"
                DisplayColon="true" />
        </div>
        <div class="filter-form-condition-cell">
            <cms:CMSDropDownList ID="drpCondition" runat="server" />
        </div>
        <div class="filter-form-value-cell">
            <cms:CMSTextBox ID="txtSearch" runat="server" />
        </div>
    </div>
    <div class="form-group form-group-buttons">
        <div class="filter-form-buttons-cell-wide">
            <cms:LocalizedButton runat="server"
                ID="btnSelect" OnClick="btnSelect_Click" EnableViewState="false" ResourceString="general.search"
                ButtonStyle="Primary" />
        </div>
    </div>
</asp:Panel>
