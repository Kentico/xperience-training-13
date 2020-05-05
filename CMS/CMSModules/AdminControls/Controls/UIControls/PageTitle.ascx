<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/PageTitle.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_PageTitle" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Panel ID="pnlHeader" runat="server" CssClass="PageHeader SimpleHeader" EnableViewState="false">
    <cms:PageTitle runat="server" ID="ucPageTitle" />
</asp:Panel>
