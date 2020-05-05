<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_AnswerEdit"
     Codebehind="AnswerEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectAlternativeForm.ascx" TagName="AlternativeFormSelector"
    TagPrefix="cms" %>

<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:LocalizedHeading runat="server" ID="pnlGeneralHeading" Level="4" ResourceString="general.general" Visible="False"></cms:LocalizedHeading>
    
    <div class="form-horizontal">
        <asp:Panel ID="pnlGeneral" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAnswerText" ResourceString="Polls_Answer_Edit.AnswerTextLabel" EnableViewState="false" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtAnswerText" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvAnswerText" runat="server" ControlToValidate="txtAnswerText:cntrlContainer:textbox"
                        ValidationGroup="required" EnableViewState="false" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcVotes">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="Polls_Answer_Edit.Votes" ID="lblVotes" EnableViewState="false" ShowRequiredMark="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtVotes" runat="server" MaxLength="9" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAnswerEnabled" EnableViewState="false"
                            AssociatedControlID="chkAnswerEnabled" ResourceString="general.enabled" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkAnswerEnabled" runat="server" Checked="true" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:Panel>

        <asp:PlaceHolder ID="plcOpenAnswer" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAnswerIsOpenEnded" EnableViewState="false"
                        AssociatedControlID="chkAnswerIsOpenEnded" ResourceString="polls.answerisopenended" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkAnswerIsOpenEnded" runat="server" AutoPostBack="true" Checked="false"
                        OnCheckedChanged="chkAnswerIsOpenEnded_CheckedChanged" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder runat="server" ID="plcOpenAnswerSettings" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAnswerHideForm" EnableViewState="false"
                        AssociatedControlID="chkAnswerHideForm" ResourceString="poll.answerhideform" DisplayColon="true"/>
                </div>
                                    <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkAnswerHideForm" runat="server" Checked="false" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>

    <cms:CMSUpdatePanel ID="updPanelForm" runat="server" UpdateMode="Always" Visible="false">
        <ContentTemplate>
            <cms:LocalizedHeading runat="server" Level="4" ResourceString="polls.answerformsettings"></cms:LocalizedHeading>
            <asp:Panel ID="pnlAnswerForm" runat="server">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAnswerForm" EnableViewState="false" ResourceString="polls.answerform"
                                DisplayColon="true" ShowRequiredMark="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:PlaceHolder ID="plcBizFormSelector" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAnswerAlternativeForm" EnableViewState="false"
                                ResourceString="polls.answeraltform" DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:AlternativeFormSelector ID="alternativeFormElem" runat="server" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>

    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    ValidationGroup="required" ResourceString="General.OK" />
            </div>
        </div>
    </div>
</asp:Panel>