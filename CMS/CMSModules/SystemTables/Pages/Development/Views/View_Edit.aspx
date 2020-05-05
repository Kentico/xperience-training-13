<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_SystemTables_Pages_Development_Views_View_Edit"
    Title="Class - Edit - Query" Theme="Default"  Codebehind="View_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/SystemTables/Controls/Views/SQLEdit.ascx" TagName="SQLEdit" TagPrefix="cms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:SQLEdit ID="editSQL" Visible="true" runat="server" DevelopmentMode="true" IsLiveSite="false" />
</asp:Content>
