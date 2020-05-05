<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Layout_New"
    Theme="Default"  Codebehind="WebPart_Edit_Layout_New.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    EnableEventValidation="false" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>
<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server">
    <cms:PreviewHierarchy ID="PreviewHierarchy1" runat="server" CookiesPreviewStateName="wpl"
        ContentControlPath="~/CMSModules/PortalEngine/Controls/Layout/General.ascx" ShowPanelSeparator="true" />
</asp:Content>
