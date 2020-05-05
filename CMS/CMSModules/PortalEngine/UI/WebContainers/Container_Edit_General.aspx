<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Inherits="CMSModules_PortalEngine_UI_WebContainers_Container_Edit_General" Theme="Default"
    EnableEventValidation="false" Title="Edit container"  Codebehind="Container_Edit_General.aspx.cs" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:PreviewHierarchy ID="ucHierarchy" runat="server" CookiesPreviewStateName="wpc"
        ContentControlPath="~/CMSModules/PortalEngine/Controls/WebContainers/General.ascx" />
</asp:Content>
