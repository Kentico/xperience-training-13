<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_General_pageplaceholder"  Codebehind="~/CMSWebParts/General/pageplaceholder.ascx.cs" %>

<asp:Label ID="lblError" runat="server" EnableViewState="false" Visible="false" CssClass="ErrorLabel" />
<cms:CMSPagePlaceholder ID="partPlaceholder" runat="server" ShortID="p" />