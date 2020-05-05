<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TextSimpleFilter.ascx.cs"
    Inherits="CMSAdminControls_UI_UniGrid_Filters_TextSimpleFilter" %>
<div class="filter-form-condition-cell">
    <cms:CMSDropDownList ID="drpCondition" runat="server" CssClass="ExtraSmallDropDown" />
</div>
<div class="filter-form-value-cell">
    <cms:CMSTextBox ID="txtText" runat="server" />
</div>
