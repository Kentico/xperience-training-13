<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Widgets_Dialogs_WidgetProperties_Header"
    Theme="default" EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
     Codebehind="WidgetProperties_Header.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>

<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlBody" CssClass="WebpartTabsPageHeader">
        <cms:PageTitle ID="pageTitle" runat="server" IsDialog="true" />
        <asp:Panel runat="server" ID="PanelSeparator" CssClass="HeaderSeparator">
            &nbsp;
        </asp:Panel>
    </asp:Panel>
    <asp:HiddenField ID="hdnSelected" runat="server" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <asp:Button ID="btnHidden" runat="server" EnableViewState="false" CssClass="HiddenButton"
        OnClick="btnHidden_Click" />
</asp:Content>
