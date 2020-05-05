<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_PageElements_Help"
     Codebehind="Help.ascx.cs" EnableViewState="false" %>

<asp:HyperLink runat="server" ID="lnkHelp" CssClass="hidden" Target="_blank">
    <asp:PlaceHolder runat="server" ID="plcIcon">
        <span class="sr-only"><%= GetString("help.tooltip") %></span>
        <i id="iconHelp" runat="server" aria-hidden="true"></i>
    </asp:PlaceHolder>
</asp:HyperLink>