<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="UserSubscriptions.ascx.cs"
    Inherits="CMSModules_Reporting_Controls_UserSubscriptions" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:LocalizedLabel ID="lblMessage" runat="server" CssClass="InfoLabel" EnableViewState="false"
    ResourceString="reportsubscripitons.userissubscribed" />
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniGrid ID="reportSubscriptions" runat="server" FilterLimit="10" ObjectType="Reporting.ReportSubscription"
            ShowActionsMenu="true" ShowObjectMenu="false">
            <GridActions>
                <ug:Action Name="ReportUnsubscribe" Caption="Unsubscribe" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$report.confirmunsubscribe$" />
                <ug:Action Name="edit" Caption="Edit" FontIconClass="icon-edit" FontIconStyle="Allow" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="ReportSubscriptionSubject" Caption="$general.subject$" Wrap="false">
                    <Filter Type="text" />
                </ug:Column>
                <ug:Column Source="ReportSubscriptionEmail" Caption="$general.email$" Wrap="false">
                    <Filter Type="text" />
                </ug:Column>
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" />
        </cms:UniGrid>
        <asp:Button runat="server" ID="btnPostback" CssClass="HiddenButton" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
