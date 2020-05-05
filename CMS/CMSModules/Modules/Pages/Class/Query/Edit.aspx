<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Class_Query_Edit"
    Title="Class - Edit - Query" Theme="Default"  Codebehind="Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/QueryEdit.ascx" TagName="QueryEdit" TagPrefix="cms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:QueryEdit ID="queryEdit" Visible="true" runat="server" DevelopmentMode="true" RefreshPageURL="Edit.aspx" IsSiteManager="true" />
</asp:Content>
