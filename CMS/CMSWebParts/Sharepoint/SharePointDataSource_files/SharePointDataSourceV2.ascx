<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/SharePoint/SharePointDatasource_files/SharePointDataSourceV2.ascx.cs" Inherits="CMSWebParts_SharePoint_SharePointDataSource_files_SharePointDatasourceV2" %>
<asp:PlaceHolder runat="server" ID="plcDebug" Visible="false">
    <cms:LocalizedHeading ID="lhDebug" Level="3" Text="SharePoint data source debug" EnableViewState="False" runat="server" />
    <asp:Label ID="lblDebugError" CssClass="ErrorLabel" EnableViewState="False" runat="server" />
    <asp:Label ID="lblDebug" EnableViewState="False" runat="server" />
    <cms:UIDataGrid ID="ugDebug" EnableViewState="False" Visible="False" runat="server"/>
</asp:PlaceHolder>