<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Maps_Documents_BingMaps"  Codebehind="~/CMSWebParts/Maps/Documents/BingMaps.ascx.cs" %>
<asp:Literal ID="ltlDesign" runat="server" EnableViewState="false" />
<cms:CMSDocumentsDataSource ID="ucDocumentSource" runat="server" />
<cms:BasicBingMaps ID="ucBingMap" runat="server" />