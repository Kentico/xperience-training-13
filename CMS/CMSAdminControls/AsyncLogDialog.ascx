<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="AsyncLogDialog.ascx.cs"
    Inherits="CMSAdminControls_AsyncLogDialog" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>

<asp:Panel ID="pnlAsyncBackground" runat="server" CssClass="async-log-bg">
    &nbsp;
</asp:Panel>
<asp:Panel ID="pnlBody" runat="server" CssClass="async-log-area-wrapper">
    <asp:Panel ID="pnlHeader" runat="server" CssClass="async-log-area-header" EnableViewState="false">
        <cms:PageTitle ID="ctlTitle" runat="server" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel ID="pnlLog" runat="server" CssClass="async-log-area-content" />
    <asp:Panel ID="pnlFooter" runat="server" CssClass="async-log-area-footer dialog-footer control-group-inline">
        <cms:LocalizedButton runat="server" ID="btnCancelAction" ResourceString="General.Cancel"
            ButtonStyle="Primary" />
    </asp:Panel>
</asp:Panel>
