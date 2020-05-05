<%@ Page Language="C#" AutoEventWireup="true" Title="Page Type Edit - Query Edit" Theme="Default"
    Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Query_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="DocumentType_Edit_Query_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/QueryEdit.ascx" TagName="QueryEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">    
    <cms:QueryEdit ID="queryEdit" Visible="true" runat="server" />    
</asp:Content>