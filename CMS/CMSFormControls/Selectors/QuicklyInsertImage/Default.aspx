<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSFormControls_Selectors_QuicklyInsertImage_Default"
     Codebehind="Default.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploaderControl.ascx"
    TagName="DirectFileUploaderControl" TagPrefix="cms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:DirectFileUploaderControl ID="fileUploaderElem" runat="server" />
</asp:Content>