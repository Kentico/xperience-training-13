<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Relateddocs_Add"
    Theme="Default"  Codebehind="Relateddocs_Add.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/Relationships/AddRelatedDocument.ascx"
    TagName="AddRelatedDocument" TagPrefix="cms" %>
<asp:Content runat="server" ID="pnlContent" ContentPlaceHolderID="plcContent">
    <cms:AddRelatedDocument ID="addRelatedDocument" runat="server" IsLiveSite="false" />
</asp:Content>
