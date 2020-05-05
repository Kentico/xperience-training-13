<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Viewers_Effects_ScrollingText"  Codebehind="~/CMSWebParts/Viewers/Effects/ScrollingText.ascx.cs" %>
<asp:Label ID="lblNoData" runat="server" Visible="false" />
<asp:Literal runat="server" ID="ltlBefore" EnableViewState="false" />
<cms:CMSRepeater ID="repItems" runat="server" EnableViewState="true" OnItemDataBound="repItems_ItemDataBound" />
<asp:Literal runat="server" ID="ltlAfter" EnableViewState="false" />
