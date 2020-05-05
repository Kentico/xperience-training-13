<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ContactStatusFilter.ascx.cs"
    Inherits="CMSModules_ContactManagement_Filters_ContactStatusFilter" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactStatusSelector.ascx"
    TagName="ContactStatusSelector" TagPrefix="cms" %>
<asp:Panel CssClass="Filter" runat="server" ID="pnlSearch">
    <cms:ContactStatusSelector ID="contactStatusSelector" runat="server" IsLiveSite="false" />
</asp:Panel>
