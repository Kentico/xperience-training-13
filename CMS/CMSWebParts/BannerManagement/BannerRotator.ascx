<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_BannerManagement_BannerRotator"  Codebehind="~/CMSWebParts/BannerManagement/BannerRotator.ascx.cs" %>

<asp:HyperLink runat="server" id="lnkBanner" Visible="false">
    <asp:Literal ID="ltrBanner" runat="server" Visible="false"></asp:Literal>
    <asp:Image ID="imgBanner" runat="server" Visible="false" />
</asp:HyperLink>