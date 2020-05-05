<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EmailProperties.ascx.cs" Inherits="CMSModules_Newsletters_EmailBuilder_EmailProperties"  %>

<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterTemplateSelector.ascx"
    TagPrefix="cms" TagName="NewsletterTemplateSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>
<asp:UpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" class="email-properties">
    <ContentTemplate>
        <div class="header-actions-container">
            <cms:FormSubmitButton ID="btnSubmit" runat="server" ResourceString="general.apply" RegisterHeaderAction="false" OnClick="btnSubmit_Click" />
        </div>
        <div class="email-properties-form scroll-area">
            <cms:AlertLabel runat="server" ID="alErroMsg" AlertType="Error" />
            <div class="form-horizontal">
                <cms:LocalizedHeading runat="server" ID="headGeneral" Level="4" ResourceString="general.general" EnableViewState="false" />
                <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueDisplayName">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayName" runat="server" ResourceString="general.name"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="txtDisplayName" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtDisplayName" runat="server" MaxLength="200" />
                    </div>
                </asp:Panel>
                <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueSubject">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" ResourceString="general.subject"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSubject" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell control-group-inline-forced">
                        <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" />
                    </div>
                </asp:Panel>

                <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueSenderName">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSenderName" runat="server" ResourceString="newsletterissue.sender.name"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSenderName" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtSenderName" runat="server" MaxLength="200" />
                    </div>
                </asp:Panel>
                <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueSenderEmail">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSenderEmail" runat="server" ResourceString="newsletterissue.sender.email"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSenderEmail" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:EmailInput ID="txtSenderEmail" runat="server" />
                    </div>
                </asp:Panel>
                <asp:Panel CssClass="form-group" runat="server" ID="pnlIssuePreheader">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblPreheader" runat="server" ResourceString="newsletterissue.preheader"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="txtPreheader" />
                        <span class="info-icon">
                            <cms:LocalizedLabel runat="server" ID="lblScreenReaderPreheader" CssClass="sr-only" Visible="False"></cms:LocalizedLabel>
                            <cms:CMSIcon ID="iconHelpPreheader" runat="server" CssClass="icon-exclamation-triangle warning-icon" EnableViewState="false" aria-hidden="true" data-html="true" Visible="false" />
                        </span>
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtPreheader" runat="server" TextMode="MultiLine" Rows="4" Columns="50" />
                    </div>
                </asp:Panel>
                <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueTemplate" EnableViewState="False">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblTemplate" runat="server" ResourceString="newsletterissue.template"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="issueTemplate" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:NewsletterTemplateSelector ID="issueTemplate" runat="server" />
                    </div>
                </asp:Panel>
                <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueUseUTM">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblIssueUseUTM" runat="server" ResourceString="newsletterissue.utm.use"
                            DisplayColon="true" EnableViewState="false" AssociatedControlID="chkIssueUseUTM" />
                        <span class="info-icon">
                            <cms:LocalizedLabel runat="server" ID="lblScreenReaderIssueUseUTM" CssClass="sr-only"></cms:LocalizedLabel>
                            <cms:CMSIcon ID="iconHelpIssueUseUTM" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                    </div>
                    <div class="editing-form-value-cell control-group-inline-forced">
                        <cms:CMSCheckBox runat="server" ID="chkIssueUseUTM" CssClass="checkbox-no-label" AutoPostBack="True" OnCheckedChanged="chkIssueUseUTM_CheckedChanged" />
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlUTMParameters">
                    <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueUTMSource">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblIssueUTMSource" runat="server" ResourceString="newsletterissue.utm.source"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="txtIssueUTMSource" ShowRequiredMark="True" />
                            <span class="info-icon">
                                <cms:LocalizedLabel runat="server" ID="lblScreenReaderIssueUTMSource" CssClass="sr-only"></cms:LocalizedLabel>
                                <cms:CMSIcon ID="iconHelpIssueUTMSource" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                            </span>
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtIssueUTMSource" runat="server" MaxLength="200" />
                        </div>
                    </asp:Panel>
                    <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueUTMMedium">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblIssueUTMMedium" runat="server" ResourceString="newsletterissue.utm.medium"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="txtIssueUTMMedium" ShowRequiredMark="True" />
                            <span class="info-icon">
                                <cms:LocalizedLabel runat="server" ID="lblScreenReaderIssueUTMMedium" CssClass="sr-only"></cms:LocalizedLabel>
                                <cms:CMSIcon ID="iconHelpIssueUTMMedium" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                            </span>
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtIssueUTMMedium" runat="server" MaxLength="200" Enabled="False" />
                        </div>
                    </asp:Panel>
                    <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueUTMCampaign">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblIssueUTMCampaign" runat="server" ResourceString="newsletterissue.utm.campaign"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="radUTMCampaignExisting" ShowRequiredMark="True" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSRadioButton runat="server" ID="radUTMCampaignExisting" GroupName="utmCampaign" ResourceString="newsletterissue.utm.campaign.existing" Checked="True" AutoPostBack="True" OnCheckedChanged="radUTMCampaign_OnCheckedChanged" />
                            <div class="selector-subitem">
                                <cms:UniSelector ID="selectorUTMCampaign" runat="server" MaxLength="200" ObjectType="analytics.campaign" ObjectSiteName="#currentsite" AllowEmpty="False" SelectionMode="SingleDropDownList" ReturnColumnName="CampaignUTMCode" />
                            </div>
                            <cms:CMSRadioButton runat="server" ID="radUTMCampaignNew" GroupName="utmCampaign" ResourceString="newsletterissue.utm.campaign.new" AutoPostBack="True" OnCheckedChanged="radUTMCampaign_OnCheckedChanged" />
                            <div class="selector-subitem">
                                <cms:CMSTextBox ID="txtIssueUTMCampaign" runat="server" MaxLength="200" Enabled="False" />
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueUTMCampaignTextBox">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblIssueUTMCampaignTextBox" runat="server" ResourceString="newsletterissue.utm.campaign"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="txtIssueUTMCampaignTextBox" ShowRequiredMark="True" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtIssueUTMCampaignTextBox" runat="server" MaxLength="200" />
                        </div>
                    </asp:Panel>
                </asp:Panel>
                <asp:PlaceHolder runat="server" ID="plcEmailUsage">
                    <asp:Panel CssClass="form-group" runat="server" ID="pnlEmailUsage" EnableViewState="False">
                        <br />
                        <cms:LocalizedHeading runat="server" ID="headAutomation" Level="4" ResourceString="newsletterissue.emailusage.header" CssClass="no-bottom-margin" EnableViewState="false" />
                        <cms:LocalizedLabel runat="server" ID="lblEmailUsageNote" EnableViewState="false" ResourceString="newsletterissue.emailusage.note" />
                    </asp:Panel>
                    <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueForAutomation" EnableViewState="False">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblIssueForAutomation" runat="server" ResourceString="newsletter.issue.forautomation"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="chkIssueForAutomation" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkIssueForAutomation" Checked="false" runat="server" />
                        </div>
                    </asp:Panel>
                </asp:PlaceHolder>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
