<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="VariantMailout.ascx.cs"
Inherits="CMSModules_Newsletters_Controls_VariantMailout" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel ID="pnlUM" runat="server">
    <ContentTemplate>
        <cms:LocalizedHeading ID="pnlMailoutHeading" runat="server" Level="4"></cms:LocalizedHeading>
        <asp:Panel runat="server" ID="pnlMailout">
            <asp:Panel runat="server" ID="pnlMailoutContent">
                <cms:UniGrid runat="server" ID="grdElem" ShortID="g" IsLiveSite="false" ObjectType="newsletter.issuevariant"
                             Columns="IssueID, IssueIsABTest, IssueVariantOfIssueID, IssueVariantName, IssueScheduledTaskID, IssueMailoutTime, IssueOpenedEmails, IssueStatus, IssueSentEmails, IssueBounces"
                             ShowActionsMenu="false">
                    <GridActions Parameters="IssueID">
                        <ug:Action Name="selwinner" Caption="$newsletterissue_send.selectwinneraction$" FontIconClass="icon-trophy"
                                   OnClick="SelWinner({0}); return false;" />
                    </GridActions>
                    <GridColumns>
                        <ug:Column Source="##ALL##" ExternalSourceName="IssueVariantName" Caption="$newsletterissue_send.variantname$"
                                   Wrap="false">
                        </ug:Column>
                        <ug:Column Source="IssueStatus" ExternalSourceName="IssueStatus" Caption="$newsletterissue_mailout.status$"
                                   Wrap="false" Name="status" />
                        <ug:Column Source="##ALL##" ExternalSourceName="MailoutTime" Caption="$unigrid.newsletter_issue.columns.issuemailouttime$"
                                   Wrap="false" />
                        <ug:Column Source="IssueSentEmails" Caption="$unigrid.newsletter_issue.columns.issuesentemails$"
                                   Wrap="false" ExternalSourceName="IssueSentEmails" Name="sentEmails" CssClass="TableCell"/>
                        
                        <ug:Column Source="##ALL##" Caption="$newsletter.issue.delivered$"
                                   Wrap="false" CssClass="TableCell" ExternalSourceName="Delivered" Name="delivered" />
                        <ug:Column Source="##ALL##" Caption="$newsletters.issuedeliveryrate$"
                                   Wrap="false" CssClass="TableCell" ExternalSourceName="DeliveryRate" Name="deliveryRate" />

                        <ug:Column Source="##ALL##" Caption="$unigrid.newsletter_issue.columns.issueopenedemails$"
                                   Wrap="false" ExternalSourceName="IssueOpenedEmails" Name="opened" Sort="IssueOpenedEmails" CssClass="text-center" />
                        <ug:Column Source="##ALL##" Caption="$unigrid.newsletter_issue.columns.issueopenrate$"
                                   Wrap="false" ExternalSourceName="IssueOpenedEmailsRate" Name="openRate" CssClass="text-center" />

                        <ug:Column Source="##ALL##" Caption="$newsletterissue_mailout.uniqueclicks$"
                                   Wrap="false" ExternalSourceName="UniqueClicks" Name="clicks" CssClass="text-center" />
                        <ug:Column Source="##ALL##" Caption="$unigrid.newsletter_issue.columns.issueclickrate$"
                                   Wrap="false" ExternalSourceName="UniqueClicksRate" Name="clickRate" CssClass="text-center" />

                        <ug:Column CssClass="filling-column" />
                        <ug:Column Source="IssueID" Wrap="false" Name="issueid" Visible="false" />
                    </GridColumns>
                    <GridOptions ShowSelection="true" DisplayFilter="false" />
                </cms:UniGrid>
                <asp:Panel ID="pMOut" runat="server">
                    <div class="form-inline">
                        <div class="control-group-inline">
                            <cms:LocalizedLabel runat="server" ID="lblMailoutTime" ResourceString="newsletterissue_send.mailouttime"
                                                EnableViewState="false" CssClass="form-control-text"/>
                            <cms:CMSDropDownList runat="server" ID="drpAllSelected" CssClass="SmallDropDown" />
                            <cms:LocalizedLabel runat="server" ID="lblTo" ResourceString="newsletterissue_send.to"
                                                EnableViewState="false"  CssClass="form-control-text"/>
                            <cms:DateTimePicker runat="server" ID="dtpMailout" />
                            <cms:LocalizedButton runat="server" ID="btnOk" EnableViewState="false" ButtonStyle="Primary"
                                                 ResourceString="general.ok" OnClick="btnOk_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
