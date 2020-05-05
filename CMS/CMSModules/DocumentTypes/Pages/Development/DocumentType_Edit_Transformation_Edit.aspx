<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_Edit"
    ValidateRequest="false" Theme="Default" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="Transformation Edit"  Codebehind="DocumentType_Edit_Transformation_Edit.aspx.cs" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PreviewHierarchy ID="ucHierarchy" runat="server" CookiesPreviewStateName="t"
        ContentControlPath="~/CMSModules/AdminControls/Controls/Class/TransformationEdit.ascx" />
</asp:Content>