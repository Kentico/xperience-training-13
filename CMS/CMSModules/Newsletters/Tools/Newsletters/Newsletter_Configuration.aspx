<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Configuration"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter configuration"
    CodeBehind="Newsletter_Configuration.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterTemplateSelector.ascx"
    TagPrefix="cms" TagName="NewsletterTemplateSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/Selectors/ScheduleInterval.ascx" TagPrefix="cms"
    TagName="ScheduleInterval" %>
<%@ Register Src="~/CMSFormControls/System/UrlChecker.ascx" TagPrefix="cms" TagName="UrlChecker" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <%-- General config --%>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.general" />
    <asp:Panel ID="pnlGeneral" runat="server">
        <div class="form-horizontal">
            <%-- Display name --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" runat="server" ID="pnlNewsletterDisplayName">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterDisplayName" EnableViewState="false"
                        ResourceString="general.displayname" DisplayColon="true"
                        AssociatedControlID="txtNewsletterDisplayName" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNewsletterDisplayName" runat="server"
                        MaxLength="250" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterDisplayName" runat="server" ControlToValidate="txtNewsletterDisplayName"
                        Display="dynamic" EnableViewState="false" />
                </div>
            </asp:Panel>
            <%-- Code name --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterName" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterName" EnableViewState="false"
                        ResourceString="general.codename" DisplayColon="true"
                        AssociatedControlID="txtNewsletterName" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CodeName ID="txtNewsletterName" runat="server" MaxLength="250" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterName" runat="server" ControlToValidate="txtNewsletterName"
                        Display="dynamic" EnableViewState="false" />
                </div>
            </asp:Panel>
            <%-- Sender name --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterSenderName" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterSenderName" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterSenderNameLabel" DisplayColon="true" ShowRequiredMark="True"
                        AssociatedControlID="txtNewsletterSenderName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNewsletterSenderName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterSenderName" runat="server" ControlToValidate="txtNewsletterSenderName" EnableViewState="false" />
                </div>
            </asp:Panel>
            <%-- Sender email --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterSenderEmail" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterSenderEmail" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterSenderEmailLabel" DisplayColon="true" ShowRequiredMark="True"
                        AssociatedControlID="txtNewsletterSenderEmail" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="txtNewsletterSenderEmail" runat="server" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterSenderEmail" runat="server" EnableViewState="false" />
                </div>
            </asp:Panel>
            <%-- Subscription template --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterSubscriptionTemplate" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSubscriptionTemplate" EnableViewState="false"
                        ResourceString="Newsletter_Edit.SubscriptionTemplate" DisplayColon="true" AssociatedControlID="subscriptionTemplate" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell control-group-inline-forced">
                    <cms:NewsletterTemplateSelector ID="subscriptionTemplate" runat="server" />
                     <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ID="lblScreenReaderSubscriptionTemplate" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="iconHelpSubscriptionTemplate" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" />
                    </span>
                </div>
            </asp:Panel>
            <%-- Unsubscription template --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterUnsubscriptionTemplate" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUnsubscriptionTemplate" EnableViewState="false"
                        ResourceString="Newsletter_Edit.UnsubscriptionTemplate" DisplayColon="true" AssociatedControlID="unsubscriptionTemplate" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell control-group-inline-forced">
                    <cms:NewsletterTemplateSelector ID="unsubscriptionTemplate" runat="server" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ID="lblScreenReaderUnsubscriptionTemplate" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="iconHelpUnsubscriptionTemplate" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" />
                    </span>
                </div>
            </asp:Panel>
            <%-- Base URL --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterBaseUrl" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterBaseUrl" EnableViewState="false"
                        ResourceString="Newsletter_Configuration.NewsletterBaseUrl" DisplayColon="true"
                        AssociatedControlID="txtNewsletterBaseUrl" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNewsletterBaseUrl" runat="server"
                        MaxLength="500" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ID="lblScreenReaderBaseUrl" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="iconHelpBaseUrl" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" />
                    </span>
                </div>
            </asp:Panel>
            <%-- Unsubscription URL --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterUnsubscriptionUrl" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterUnsubscribeUrl" EnableViewState="false"
                        ResourceString="Newsletter_Configuration.NewsletterUnsubscribeUrl" DisplayColon="true"
                        AssociatedControlID="txtNewsletterUnsubscribeUrl" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNewsletterUnsubscribeUrl" runat="server"
                        MaxLength="1000" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ID="lblScreenReaderUnsubscribeUrl" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="iconHelpUnsubscribeUrl" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" />
                    </span>
                </div>
            </asp:Panel>
            <%-- Draft emails --%>
            <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterDraftEmails" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDraftEmails" runat="server" EnableViewState="false" ResourceString="newsletter.draftemails"
                        DisplayColon="true" AssociatedControlID="txtDraftEmails" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="txtDraftEmails" runat="server" AllowMultipleAddresses="true" />
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>
    <%-- Template based config --%>
    <asp:PlaceHolder ID="plcTemplates" runat="server">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="newsletter_configuration.templatebased" />
        <asp:Panel ID="pnlTemplates" runat="server">
            <div class="form-horizontal">
                <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlUsTemplates" runat="server">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblUsTemplates" runat="server" EnableViewState="false" ResourceString="newsletter_configuration.templatebased"
                            DisplayColon="true" AssociatedControlID="usTemplates" ShowRequiredMark="True"/>
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSUpdatePanel runat="server" ID="pnlAvailability">
                            <ContentTemplate>
                                <cms:UniSelector ID="usTemplates" runat="server" IsLiveSite="false" ObjectType="newsletter.emailtemplate"
                                    SelectionMode="Multiple" ResourcePrefix="templatesselect" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </asp:Panel>
             </div>
        </asp:Panel>
    </asp:PlaceHolder>
    <%-- Dynamic config --%>
    <asp:PlaceHolder ID="plcDynamic" runat="server">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="newsletter_configuration.dynamic" />
        <asp:Panel ID="pnlDynamic" runat="server">
            <div class="form-horizontal">
                <%-- Subject --%>
                <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterDynamicSubject" runat="server">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" ResourceString="general.subject"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSUpdatePanel ID="pnlUpSubject" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="radPageTitle" />
                                <asp:AsyncPostBackTrigger ControlID="radFollowing" />
                            </Triggers>
                            <ContentTemplate>
                                <div class="radio-list-vertical">
                                    <cms:CMSRadioButton ID="radPageTitle" runat="server" GroupName="Subject" ResourceString="Newsletter_Configuration.PageTitleSubject"
                                        AutoPostBack="True" OnCheckedChanged="radSubject_CheckedChanged" />
                                    <cms:CMSRadioButton ID="radFollowing" runat="server" GroupName="Subject" ResourceString="Newsletter_Configuration.PageTitleFollowing"
                                        AutoPostBack="True" OnCheckedChanged="radSubject_CheckedChanged" />
                                    <div class="selector-subitem">
                                        <cms:LocalizableTextBox ID="txtSubject" runat="server" MaxLength="100" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </asp:Panel>
                <%-- Dynamic newsletter URL --%>
                <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterDynamicUrl" runat="server">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterDynamicURL" EnableViewState="false" ShowRequiredMark="True"
                            ResourceString="Newsletter_Edit.SourcePageURL" DisplayColon="true" AssociatedControlID="txtNewsletterDynamicURL" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:UrlChecker runat="server" ID="txtNewsletterDynamicURL" ResourcePrefix="newsletter" />
                        <cms:CMSRequiredFieldValidator ID="rfvNewsletterDynamicURL" runat="server" ControlToValidate="txtNewsletterDynamicURL:txtDomain" Display="Dynamic" />
                    </div>
                </asp:Panel>
                <%-- Scheduler --%>
                <asp:Panel CssClass="form-group" ID="pnlNewsletterDynamicScheduler" runat="server">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSchedule" EnableViewState="false" ResourceString="Newsletter_Edit.Schedule"
                            DisplayColon="true" AssociatedControlID="chkSchedule" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkSchedule" runat="server" Checked="true" AutoPostBack="true"
                            OnCheckedChanged="chkSchedule_CheckedChanged" />
                    </div>
                </asp:Panel>
                <cms:CMSUpdatePanel ID="pnlUpScheduler" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkSchedule" />
                    </Triggers>
                    <ContentTemplate>
                        <cms:ScheduleInterval ID="schedulerInterval" runat="server" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
        </asp:Panel>
    </asp:PlaceHolder>
    <%-- Online marketing config --%>
    <asp:PlaceHolder ID="plcTracking" runat="server">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="onlinemarketing.general" />
        <asp:Panel ID="pnlOM" runat="server">
            <div class="form-horizontal">
                <%-- Track opened emails --%>
                <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterTrackOpenedEmails" runat="server">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblTrackOpenedEmails" runat="server" EnableViewState="false"
                            ResourceString="newsletter.trackopenedemails" DisplayColon="true" AssociatedControlID="chkTrackOpenedEmails" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkTrackOpenedEmails" runat="server" />
                    </div>
                </asp:Panel>
                <%-- Track clicked links --%>
                <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterTrackClickedLinks" runat="server">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblTrackClickedLinks" runat="server" EnableViewState="false"
                            ResourceString="newsletter.trackclickedlinks" DisplayColon="true" AssociatedControlID="chkTrackClickedLinks" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkTrackClickedLinks" runat="server" />
                    </div>
                </asp:Panel>
                <asp:PlaceHolder ID="plcOM" runat="server">
                    <%-- Log activities --%>
                    <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterLogActivities" runat="server">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblLogActivity" runat="server" EnableViewState="false" ResourceString="newsletter.trackactivities"
                                DisplayColon="true" AssociatedControlID="chkLogActivity" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkLogActivity" runat="server" />
                        </div>
                    </asp:Panel>
                </asp:PlaceHolder>
            </div>
        </asp:Panel>
    </asp:PlaceHolder>
    <%-- Double opt-in config --%>
    <cms:LocalizedHeading runat="server" Level="4" ID="hdrDoubleOptIn" ResourceString="newsletter_configuration.optin" />
    <asp:Panel ID="pnlDoubleOptIn" runat="server">
        <div class="form-horizontal">
            <%-- Enable double opt-in --%>
            <asp:Panel CssClass="form-group" ID="pnlNewsletterEnableOptIn" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblEnableOptIn" runat="server" EnableViewState="false" ResourceString="newsletter_configuration.enableoptin"
                        DisplayColon="true" AssociatedControlID="chkEnableOptIn" />
                </div>
                <div class="editing-form-value-cell control-group-inline-forced">
                    <cms:CMSCheckBox ID="chkEnableOptIn" runat="server" AutoPostBack="true" OnCheckedChanged="chkEnableOptIn_CheckedChanged" CssClass="checkbox-no-label" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ID="lblScreenReaderEnableOptIn" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon ID="iconHelpEnableOptIn" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" />
                    </span>
                </div>
            </asp:Panel>
        </div>
        <cms:CMSUpdatePanel ID="pnlUpOptIn" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="chkEnableOptIn" />
            </Triggers>
            <ContentTemplate>
                <asp:PlaceHolder ID="plcOptIn" runat="server" Visible="false">
                    <div class="form-horizontal">
                        <%-- Opt-in template --%>
                        <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterOptInTemplate" runat="server">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOptInTemplate" EnableViewState="false"
                                    ResourceString="newsletter_configuration.optnintemplate" DisplayColon="true"
                                    AssociatedControlID="optInSelector" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:NewsletterTemplateSelector ID="optInSelector" runat="server" />
                            </div>
                        </asp:Panel>
                        <%-- Approval URL --%>
                        <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterOptInApprovalUrl" runat="server">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOptInURL" EnableViewState="false" ResourceString="newsletter_configuration.optinurl"
                                    DisplayColon="true" AssociatedControlID="txtOptInURL" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtOptInURL" runat="server" MaxLength="450" />
                            </div>
                        </asp:Panel>
                        <%-- Send confirmation --%>
                        <asp:Panel CssClass="form-group" EnableViewState="false" ID="pnlNewsletterOptInSendConfirmation" runat="server">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblSendOptInConfirmation" runat="server" EnableViewState="false"
                                    ResourceString="newsletter_configuration.sendoptinconfirmation" DisplayColon="true"
                                    AssociatedControlID="chkSendOptInConfirmation" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSCheckBox ID="chkSendOptInConfirmation" runat="server" />
                            </div>
                        </asp:Panel>
                    </div>
                </asp:PlaceHolder>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>