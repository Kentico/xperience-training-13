<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_SearchResults"
     Codebehind="SearchResults.ascx.cs" %>
<asp:Panel runat="server" ID="pnlSearchResults">
    <asp:PlaceHolder runat="server" ID="plcBasicRepeater"></asp:PlaceHolder>
    <cms:UniPager runat="server" ID="pgrSearch" PageControl="repSearchResults" />
    <cms:LocalizedLabel runat="server" ID="lblNoResults" CssClass="ContentLabel" Visible="false"
        EnableViewState="false" />
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
</asp:Panel>
