<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_CustomTables_CustomTable_Edit_Transformation_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Custom table edit - Transformation List"
     Codebehind="CustomTable_Edit_Transformation_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/ClassTransformations.ascx"
    TagName="ClassTransformations" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ClassTransformations ID="classTransformations" runat="server" IsLiveSite="false" />
</asp:Content>
