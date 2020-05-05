<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Controls_GroupProfile"  Codebehind="GroupProfile.ascx.cs" %>
<asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
    Visible="false" />
<div class="TabContainer">
    <cms:BasicTabControl ID="tabMenu" runat="server" Visible="true" />
</div>
<asp:Panel ID="pnlContent" runat="server" CssClass="TabBody">
</asp:Panel>
