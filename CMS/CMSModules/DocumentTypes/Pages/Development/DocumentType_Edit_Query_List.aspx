<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Query_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Type Edit - Query List"
     Codebehind="DocumentType_Edit_Query_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/ClassQueries.ascx" TagName="ClassQueries"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ClassQueries ID="classEditQuery" runat="server" IsLiveSite="false" />
</asp:Content>
