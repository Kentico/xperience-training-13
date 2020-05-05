<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ImportExport_Controls_NewSiteFinish"  Codebehind="NewSiteFinish.ascx.cs" %>
<div style="padding: 10px">
    <asp:PlaceHolder ID="plcFinish" runat="server">
        <asp:HyperLink ID="lnkWebSite" runat="server" />
        <asp:Label ID="lblSiteStatus" runat="server" />
        <br />
        <br />
    </asp:PlaceHolder>
    <asp:Label ID="lblLogonDetails" runat="server" />
    <br />
    <br />
    <asp:Label ID="lblMediumTrust" runat="server" />
</div>
