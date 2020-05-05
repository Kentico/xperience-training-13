<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CountryFilter.ascx.cs" Inherits="CMSApp.CMSModules.ContactManagement.Filters.CMSModules_ContactManagement_Filters_CountryFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter" TagPrefix="cms" %>

<asp:Panel ID="pnlFilter" runat="server" CssClass="form-generated">
    <cms:TextSimpleFilter ID="fltCountry" runat="server" Column="CountryDisplayName" />
</asp:Panel>