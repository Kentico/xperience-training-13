<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DocumentCultureFilter.ascx.cs"
    Inherits="CMSModules_Content_Controls_Filters_DocumentCultureFilter" %>
<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>
<div class="form-condition-cell-generated">
    <cms:CMSDropDownList ID="drpLanguage" runat="server" CssClass="ExtraSmallDropDown" />
</div>
<div class="form-value-cell-generated">
    <cms:SiteCultureSelector runat="server" ID="cultureElem" IsLiveSite="false" />
</div>
