<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Inherits="CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Layout_Edit"
    EnableEventValidation="false" Theme="Default"  Codebehind="WebPart_Edit_Layout_Edit.aspx.cs" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:PreviewHierarchy ID="ucHierarchy" runat="server" CookiesPreviewStateName="wpl"
        ContentControlPath="~/CMSModules/PortalEngine/Controls/Layout/General.ascx" ShowPanelSeparator="true" />
</asp:Content>
