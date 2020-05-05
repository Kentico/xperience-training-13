<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Type Edit - Transformation List"
     Codebehind="DocumentType_Edit_Transformation_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/ClassTransformations.ascx"
    TagName="ClassTransformations" TagPrefix="cms" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ClassTransformations ID="classTransformations" runat="server" IsLiveSite="false" />
</asp:Content>
