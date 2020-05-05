<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Class_Transformation_Edit"
    ValidateRequest="false" Theme="Default" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="Transformation Edit"  Codebehind="Edit.aspx.cs" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:PreviewHierarchy ID="ucHierarchy" runat="server" CookiesPreviewStateName="t"
        ContentControlPath="~/CMSModules/AdminControls/Controls/Class/TransformationEdit.ascx" />
</asp:Content>
