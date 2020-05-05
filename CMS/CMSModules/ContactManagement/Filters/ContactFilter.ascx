<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ContactFilter.ascx.cs"
    Inherits="CMSModules_ContactManagement_Filters_ContactFilter" %>
<cms:LocalizedLabel ID="lblContact" runat="server" DisplayColon="true" ResourceString="om.contact.entersearch" />
<cms:CMSTextBox ID="txtContact" runat="server" />
<cms:LocalizedButton ID="btnSearch" runat="server" ResourceString="general.search"
    ButtonStyle="Default" OnClick="btnSearch_Click" />
