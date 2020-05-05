<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_CustomTables_CustomTable_Edit_Query_Edit"
    Title="Untitled Page" Theme="Default"  Codebehind="CustomTable_Edit_Query_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/QueryEdit.ascx" TagName="QueryEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:QueryEdit ID="queryEdit" Visible="true" runat="server" />
</asp:Content>
