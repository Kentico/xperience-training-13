<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartProperties_header"
    Theme="Default"  Codebehind="WebPartProperties_header.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>

<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlBody" CssClass="WebpartTabsPageHeader">
        <asp:Panel runat="server" ID="Panel1">
            <cms:PageTitle ID="pageTitle" runat="server" IsDialog="true" />
        </asp:Panel>
    <asp:Panel runat="server" ID="PanelSeparator" CssClass="HeaderSeparator">
        &nbsp;
    </asp:Panel>
    </asp:Panel>
</asp:Content>
