<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Maps_Documents_GoogleMaps"  Codebehind="~/CMSWebParts/Maps/Documents/GoogleMaps.ascx.cs" %>
<asp:Literal ID="ltlDesign" runat="server" EnableViewState="false" />
<cms:CMSDocumentsDataSource ID="ucDocumentSource" runat="server" />
<cms:BasicGoogleMaps ID="ucGoogleMap" runat="server" />