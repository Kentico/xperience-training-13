<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Newsletter_ABTestIssue_Send.aspx.cs" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_ABTestIssue_Send" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/SendVariantIssue.ascx" TagPrefix="cms"
    TagName="SendVariant" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms"
    TagName="SmartTip" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlU" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <cms:MessagesPlaceHolder runat="server" ID="plcMessages" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>

    <cms:AlertLabel runat="server" ID="lblUrlWarning" AlertType="Warning" Visible="False" CssClass="hide" EnableViewState="false" />
    <cms:SendVariant ID="ctrSendVariant" runat="server" ShortID="sv" Visible="true" />
    <cms:SmartTip ID="ctrSmartTip" runat="server" EnableViewState="false" Content="{$newsletter.issue.smarttip.content$}"
        ExpandedHeader="{$newsletter.issue.smarttip.header$}" CollapsedHeader="{$newsletter.issue.smarttip.header$}" />
</asp:Content>