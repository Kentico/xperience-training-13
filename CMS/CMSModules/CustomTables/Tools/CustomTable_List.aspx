<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_CustomTables_Tools_CustomTable_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Custom Tables List"
     Codebehind="CustomTable_List.aspx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="uniGrid" GridName="CustomTable_List.xml" OrderBy="ClassDisplayName"
        IsLiveSite="false" ShowObjectMenu="false" Columns="ClassDisplayName,ClassID" />
</asp:Content>
