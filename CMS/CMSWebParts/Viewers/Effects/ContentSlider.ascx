<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Viewers_Effects_ContentSlider"  Codebehind="~/CMSWebParts/Viewers/Effects/ContentSlider.ascx.cs" %>
<asp:Literal runat="server" ID="ltlBefore" EnableViewState="false" />
<cms:CMSRepeater ID="repItems" runat="server" EnableViewState="true" />
<asp:Literal runat="server" ID="ltlAfter" EnableViewState="false" />