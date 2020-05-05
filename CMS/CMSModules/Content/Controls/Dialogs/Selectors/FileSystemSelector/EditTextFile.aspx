<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master"
    EnableEventValidation="false" AutoEventWireup="True"  Codebehind="EditTextFile.aspx.cs"
    Inherits="CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_EditTextFile"
    Theme="Default" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:PreviewHierarchy ID="ucHierarchy" runat="server" CookiesPreviewStateName="theme"
        AllowEmptyObject="true" ShowPanelSeparator="false" ContentControlPath="~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/EditFile.ascx" />
</asp:Content>
