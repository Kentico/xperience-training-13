<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_WebServices_GridForRESTService"
     Codebehind="~/CMSWebParts/WebServices/GridForRESTService.ascx.cs" %>
<asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
    Visible="false" />
<cms:BasicDataGrid ID="basicDataGrid" runat="server" CssClass="DataGrid" />