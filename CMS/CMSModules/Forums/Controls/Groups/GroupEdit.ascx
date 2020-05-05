<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Groups_GroupEdit"
     Codebehind="GroupEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Panel ID="pnlGroupEdit" runat="server" CssClass="ForumEdit">
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.general"></cms:LocalizedHeading>
    <asp:Panel ID="pnlGeneral" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblGroupDisplayName" EnableViewState="false" ResourceString="Group_General.GroupDisplayNameLabel" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtGroupDisplayName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvGroupDisplayName" runat="server" ControlToValidate="txtGroupDisplayName:cntrlContainer:textbox"
                        ErrorMessage="" ValidationGroup="vgForumGroup" Display="Dynamic"></cms:CMSRequiredFieldValidator>
                </div>
            </div>
            <asp:PlaceHolder ID="plcCodeName" runat="Server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblGroupName" EnableViewState="false" ResourceString="Group_General.GroupNameLabel" ShowRequiredMark="True"/>
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CodeName ID="txtGroupName" runat="server" MaxLength="200" />
                        <cms:CMSRequiredFieldValidator ID="rfvGroupName" Display="Dynamic" runat="server"
                            ErrorMessage="" ControlToValidate="txtGroupName" ValidationGroup="vgForumGroup"></cms:CMSRequiredFieldValidator>
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDescription" EnableViewState="false" ResourceString="general.description"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtGroupDescription" runat="server" TextMode="MultiLine" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcBaseAndUnsubUrl" runat="server" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumBaseUrl" EnableViewState="false" ResourceString="Group_General.ForumBaseUrlLabel" />
                    </div>
                    <div class="editing-form-value-cell">
                        <div class="control-group-inline">
                            <cms:CMSCheckBox runat="server" ID="chkInheritBaseUrl" Checked="true" ResourceString="Forums.InheritBaseUrl" />
                        </div>
                        <div class="control-group-inline">
                            <cms:CMSTextBox ID="txtForumBaseUrl" runat="server" MaxLength="200" />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUnsubscriptionUrl" EnableViewState="false" ResourceString="Group_General.UnsubscriptionUrlLabel" />
                    </div>
                    <div class="editing-form-value-cell">
                        <div class="control-group-inline">
                            <cms:CMSCheckBox runat="server" ID="chkInheritUnsubUrl" Checked="true" ResourceString="Forums.InheritUnsubsUrl" />
                        </div>
                        <div class="control-group-inline">
                            <cms:CMSTextBox ID="txtUnsubscriptionUrl" runat="server" MaxLength="200" />
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:Panel>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="forums.advancedsettings"></cms:LocalizedHeading>
    <asp:Panel ID="pnlAdvanced" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumRequireEmail"
                        EnableViewState="false" ResourceString="Forum_Edit.ForumRequireEmailLabel" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkForumRequireEmail" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumDisplayEmails"
                        EnableViewState="false" ResourceString="Forum_Edit.ForumDisplayEmailsLabel" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkForumDisplayEmails" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" ResourceString="forum.settings.type"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="radio-list-vertical">
                        <cms:CMSRadioButton ID="radTypeChoose" runat="server" GroupName="type" Checked="true"
                            ResourceString="forum.settings.typechoose" />
                        <cms:CMSRadioButton ID="radTypeDiscussion" runat="server" GroupName="type"
                            ResourceString="forum.settings.typediscussion" />
                        <cms:CMSRadioButton ID="radTypeAnswer" runat="server" GroupName="type" ResourceString="forum.settings.typeanswer" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblIsAnswerLimit" EnableViewState="false"
                        ResourceString="forum.settings.isanswerlimit" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtIsAnswerLimit" runat="server" MaxLength="9" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblImageMaxSideSize" EnableViewState="false"
                        ResourceString="forum.settings.maxsidesize" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtImageMaxSideSize" runat="server" MaxLength="9" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblMaxAttachmentSize" EnableViewState="false"
                        ResourceString="forum.settings.maxattachmentsize" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtMaxAttachmentSize" runat="server"
                        MaxLength="9" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcOnline" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblLogActivity" EnableViewState="false" ResourceString="forum.settings.logactivity"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkLogActivity" runat="server" CssClass="CheckBoxMovedLeft" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:Panel>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="forums.securitysettings"></cms:LocalizedHeading>
    <asp:Panel ID="pnlSecurity" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAuthorEdit" EnableViewState="false" ResourceString="forum.settings.authoredit"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkAuthorEdit" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAuthorDelete" EnableViewState="false" ResourceString="forum.settings.authordelete"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkAuthorDelete" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCaptcha" EnableViewState="false"
                        ResourceString="Forum_Edit.useCaptcha" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkCaptcha" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="forums.editorsettings"></cms:LocalizedHeading>
    <asp:Panel ID="pnlEditor" runat="server">
        <div class="form-horizontal">
            <asp:PlaceHolder runat="server" ID="plcUseHtml">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUseHTML" EnableViewState="false"
                            ResourceString="Forum_Edit.UseHtml" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkUseHTML" runat="server" CssClass="CheckBoxMovedLeft" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableUrl" EnableViewState="false" ResourceString="forum.settings.enablesimpleurl"
                        ToolTipResourceString="forum.settings.enablesimpleurl.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="radio-list-horizontal">
                        <cms:CMSRadioButton ID="radUrlNo" runat="server" ResourceString="general.no"
                            ToolTipResourceString="forum.settings.enablesimpleurl.description" GroupName="EnableImage" />
                        <cms:CMSRadioButton ID="radUrlSimple" runat="server" ResourceString="forum.settings.simpledialog"
                            ToolTipResourceString="forum.settings.enablesimpleurl.description" GroupName="EnableImage" />
                        <cms:CMSRadioButton ID="radUrlAdvanced" runat="server" ResourceString="forum.settings.advanceddialog"
                            ToolTipResourceString="forum.settings.enablesimpleurl.description" GroupName="EnableImage" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableImage" EnableViewState="false" ResourceString="forum.settings.enablesimpleimage"
                        ToolTipResourceString="forum.settings.enablesimpleimage.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="radio-list-horizontal">
                        <cms:CMSRadioButton ID="radImageNo" runat="server" ResourceString="general.no"
                            ToolTipResourceString="forum.settings.enablesimpleimage.description" GroupName="EnableURL" />
                        <cms:CMSRadioButton ID="radImageSimple" runat="server" ResourceString="forum.settings.simpledialog"
                            ToolTipResourceString="forum.settings.enablesimpleimage.description" GroupName="EnableURL" />
                        <cms:CMSRadioButton ID="radImageAdvanced" runat="server" ResourceString="forum.settings.advanceddialog"
                            ToolTipResourceString="forum.settings.enablesimpleimage.description" GroupName="EnableURL" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableQuote" EnableViewState="false" ResourceString="forum.settings.enablequote"
                        ToolTipResourceString="forum.settings.enablequote.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableQuote" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enablequote.description" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableCode" EnableViewState="false" ResourceString="forum.settings.enablecode"
                        ToolTipResourceString="forum.settings.enablecode.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableCode" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enablecode.description" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableBold" EnableViewState="false" ResourceString="forum.settings.enablebold"
                        ToolTipResourceString="forum.settings.enablebold.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableBold" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enablebold.description" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableItalic" EnableViewState="false" ResourceString="forum.settings.enableitalic"
                        ToolTipResourceString="forum.settings.enableitalic.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableItalic" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enableitalic.description" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableUnderline" EnableViewState="false"
                        ResourceString="forum.settings.enableunderline" ToolTipResourceString="forum.settings.enableunderline.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableUnderline" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enableunderline.description" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableStrike" EnableViewState="false" ResourceString="forum.settings.enablestrike"
                        ToolTipResourceString="forum.settings.enablestrike.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableStrike" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enablestrike.description" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableColor" EnableViewState="false" ResourceString="forum.settings.enablecolor"
                        ToolTipResourceString="forum.settings.enablecolor.description" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableColor" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enablecolor.description" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.OptIn"></cms:LocalizedHeading>
    <asp:Panel ID="pnlOptIn" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblEnableOptIn" runat="server" EnableViewState="false" ResourceString="general.enableoptin"
                        DisplayColon="true" AssociatedControlID="chkEnableOptIn" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ThreeStateCheckBox ID="chkEnableOptIn" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOptInURL" EnableViewState="false" ResourceString="general.optinurl"
                        DisplayColon="true" AssociatedControlID="txtOptInURL" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                        <cms:CMSCheckBox runat="server" ID="chkInheritOptInURL" Checked="true" ResourceString="Forums.InheritBaseUrl" />
                    </div>
                    <div class="control-group-inline">
                        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" RenderMode="Inline">
                            <ContentTemplate>
                                <cms:SelectPath ID="txtOptInURL" runat="server" CssClass="Inline" MaxLength="450" SinglePathMode="true" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSendOptInConfirmation" runat="server" EnableViewState="false"
                        ResourceString="general.sendoptinconfirmation" DisplayColon="true" AssociatedControlID="chkSendOptInConfirmation" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ThreeStateCheckBox ID="chkSendOptInConfirmation" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click"
                        EnableViewState="false" ValidationGroup="vgForumGroup" ResourceString="General.OK" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Panel>
