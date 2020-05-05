<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_CustomTables_CustomTableDataGrid"  Codebehind="~/CMSWebParts/CustomTables/CustomTableDataGrid.ascx.cs" %>
<cms:QueryDataGrid ID="gridItems" runat="server" DataBindByDefault="false" OnItemDataBound="gridItems_ItemDataBound" />