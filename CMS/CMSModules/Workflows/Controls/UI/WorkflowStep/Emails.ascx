<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_Emails"
     Codebehind="Emails.ascx.cs" %>
<%@ Register Src="~/CMSModules/EmailTemplates/FormControls/EmailTemplateSelector.ascx"
    TagName="TemplateSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUp" runat="server">
    <ContentTemplate>
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.general"></cms:LocalizedHeading>
        <asp:Panel ID="pnlSettings" runat="server">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSendEmails" runat="server" EnableViewState="false" ResourceString="development-workflow_emails.sendemails"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:ThreeStateCheckBox ID="chkEmails" runat="server" />
                    </div>
                </div>
            </div>
        </asp:Panel>
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="workflow.templates"></cms:LocalizedHeading>
        <asp:Panel ID="pnlTemplates" runat="server">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblReadyApprovalSend" runat="server" EnableViewState="false"
                            ResourceString="workflow.readyapprovaltemplatelabel" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:ThreeStateCheckBox ID="chkReadyApproval" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblReadyApprovalTemplate" runat="server" EnableViewState="false"
                            ResourceString="workflow.customtemplate" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:TemplateSelector ID="ucReadyApproval" runat="server" IsLiveSite="false" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcApprove" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblApproveSend" runat="server" EnableViewState="false" ResourceString="workflow.approvedtemplatelabel"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:ThreeStateCheckBox ID="chkApprove" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblApproveTemplate" runat="server" EnableViewState="false"
                                ResourceString="workflow.customtemplate" DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:TemplateSelector ID="ucApprove" runat="server" IsLiveSite="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblRejectSend" runat="server" EnableViewState="false" ResourceString="workflow.rejecttemplatelabel"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:ThreeStateCheckBox ID="chkReject" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblRejectTemplate" runat="server" EnableViewState="false"
                            ResourceString="workflow.customtemplate" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:TemplateSelector ID="ucReject" runat="server" IsLiveSite="false" />
                    </div>
                </div>
            </div>
            <br />
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
