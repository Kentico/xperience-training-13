<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Forums_ForumEdit"
     Codebehind="ForumEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Panel ID="pnlGroupEdit" runat="server" CssClass="ForumEdit">
    <cms:LocalizedHeading ID="headGeneral" runat="server" Level="4" EnableViewState="false" ResourceString="general.general" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumDisplayName" EnableViewState="false" AssociatedControlID="txtForumDisplayName" ShowRequiredMark="true"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtForumDisplayName" runat="server"
                    MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvForumDisplayName" runat="server" ErrorMessage="forum_general.emptydisplayname"
                    ControlToValidate="txtForumDisplayName" Display="Dynamic" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcCodeName" runat="Server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumName" EnableViewState="false" AssociatedControlID="txtForumName" ShowRequiredMark="true"/>
                </div>
                <div class="editing-form-value-cell">
                    <cms:CodeName ID="txtForumName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator Display="Dynamic" ID="rfvForumName" runat="server"
                        ErrorMessage="" ControlToValidate="txtForumName"></cms:CMSRequiredFieldValidator>
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDescription" EnableViewState="false"
                    ResourceString="general.description" DisplayColon="true" AssociatedControlID="txtForumDescription"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtForumDescription" runat="server" TextMode="MultiLine" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcBaseAndUnsubUrl" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblBaseUrl" EnableViewState="false" AssociatedControlID="txtBaseUrl"/>
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                        <cms:CMSCheckBox runat="server" ID="chkInheritBaseUrl" />
                    </div>
                    <div class="control-group-inline">
                        <cms:CMSTextBox ID="txtBaseUrl" runat="server" MaxLength="200" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblUnsubscriptionUrl" EnableViewState="false" AssociatedControlID="txtUnsubscriptionUrl"/>
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                        <cms:CMSCheckBox runat="server" ID="chkInheritUnsubscribeUrl" />
                    </div>
                    <div class="control-group-inline">
                        <cms:CMSTextBox ID="txtUnsubscriptionUrl" runat="server" MaxLength="200" />
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <cms:LocalizedHeading ID="headAdvanced" runat="server" Level="4" EnableViewState="false" ResourceString="forums.advancedsettings" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblForumOpen" EnableViewState="false" AssociatedControlID="chkForumOpen"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkForumOpen" runat="server" CssClass="CheckBoxMovedLeft" Checked="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblForumLocked" EnableViewState="false" AssociatedControlID="chkForumLocked"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkForumLocked" runat="server" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblForumRequireEmail" EnableViewState="false" AssociatedControlID="chkForumRequireEmail"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:ThreeStateCheckBox ID="chkForumRequireEmail" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblForumDisplayEmails" EnableViewState="false" AssociatedControlID="chkForumDisplayEmails"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:ThreeStateCheckBox ID="chkForumDisplayEmails" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" ResourceString="forum.settings.type" DisplayColon="true" AssociatedControlID="chkInheritType"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkInheritType" ResourceString="forum.settings.inheritfromgroup" />
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton ID="radTypeChoose" runat="server" GroupName="type" ResourceString="forum.settings.typechoose" />
                    <cms:CMSRadioButton ID="radTypeDiscussion" runat="server" GroupName="type" ResourceString="forum.settings.typediscussion" />
                    <cms:CMSRadioButton ID="radTypeAnswer" runat="server" GroupName="type" ResourceString="forum.settings.typeanswer" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblIsAnswerLimit" EnableViewState="false" AssociatedControlID="txtIsAnswerLimit"
                    ResourceString="forum.settings.isanswerlimit" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkInheritIsAnswerLimit" ResourceString="forum.settings.inheritfromgroup" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtIsAnswerLimit" runat="server" MaxLength="9" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblImageMaxSideSize" EnableViewState="false" AssociatedControlID="txtImageMaxSideSize"
                    ResourceString="forum.settings.maxsidesize" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkInheritMaxSideSize" ResourceString="forum.settings.inheritfromgroup" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtImageMaxSideSize" runat="server" MaxLength="9" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblMaxAttachmentSize" EnableViewState="false" AssociatedControlID="txtMaxAttachmentSize"
                    ResourceString="forum.settings.maxattachmentsize" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkInheritMaxAttachmentSize" ResourceString="forum.settings.inheritfromgroup" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtMaxAttachmentSize" runat="server" MaxLength="9" />
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcOnline" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblLogActivity" EnableViewState="false" ResourceString="forum.settings.logactivity" AssociatedControlID="chkLogActivity"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkInheritLogActivity" ResourceString="forum.settings.inheritfromgroup" />
                    <cms:CMSCheckBox ID="chkLogActivity" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <cms:LocalizedHeading ID="headSecurity" runat="server" Level="4" EnableViewState="false" ResourceString="forums.securitysettings" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAuthorEdit" EnableViewState="false" ResourceString="forum.settings.authoredit" AssociatedControlID="chkAuthorEdit"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ThreeStateCheckBox ID="chkAuthorEdit" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAuthorDelete" EnableViewState="false" ResourceString="forum.settings.authordelete" AssociatedControlID="chkAuthorDelete"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ThreeStateCheckBox ID="chkAuthorDelete" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblCaptcha" EnableViewState="false" AssociatedControlID="chkCaptcha" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ThreeStateCheckBox ID="chkCaptcha" runat="server" />
            </div>
        </div>
    </div>
    <cms:LocalizedHeading ID="headEditorSettings" runat="server" Level="4" EnableViewState="false" ResourceString="forums.editorsettings" />
    <div class="form-horizontal">
        <asp:PlaceHolder runat="server" ID="plcUseHtml">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblUseHTML" EnableViewState="false" AssociatedControlID="chkUseHTML" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ThreeStateCheckBox ID="chkUseHTML" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                &nbsp;
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkInheritDiscussion" ResourceString="forum.settings.inheritfromgroup" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableUrl" EnableViewState="false" ResourceString="forum.settings.enablesimpleurl" AssociatedControlID="chkInheritDiscussion"
                    ToolTipResourceString="forum.settings.enablesimpleurl.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton ID="radUrlNo" runat="server" ResourceString="general.no"
                        GroupName="EnableImage" ToolTipResourceString="forum.settings.enablesimpleurl.description" />
                    <cms:CMSRadioButton ID="radUrlSimple" runat="server" ResourceString="forum.settings.simpledialog"
                        GroupName="EnableImage" ToolTipResourceString="forum.settings.enablesimpleurl.description" />
                    <cms:CMSRadioButton ID="radUrlAdvanced" runat="server" ResourceString="forum.settings.advanceddialog"
                        GroupName="EnableImage" ToolTipResourceString="forum.settings.enablesimpleurl.description" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableImage" EnableViewState="false" ResourceString="forum.settings.enablesimpleimage" AssociatedControlID="radImageNo"
                    ToolTipResourceString="forum.settings.enablesimpleimage.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton ID="radImageNo" runat="server" ResourceString="general.no"
                        GroupName="EnableURL" ToolTipResourceString="forum.settings.enablesimpleimage.description" />
                    <cms:CMSRadioButton ID="radImageSimple" runat="server" ResourceString="forum.settings.simpledialog"
                        GroupName="EnableURL" ToolTipResourceString="forum.settings.enablesimpleimage.description" />
                    <cms:CMSRadioButton ID="radImageAdvanced" runat="server" ResourceString="forum.settings.advanceddialog"
                        GroupName="EnableURL" ToolTipResourceString="forum.settings.enablesimpleimage.description" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableQuote" EnableViewState="false" ResourceString="forum.settings.enablequote" AssociatedControlID="chkEnableQuote"
                    ToolTipResourceString="forum.settings.enablequote.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkEnableQuote" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enablequote.description" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnbaleCode" EnableViewState="false" ResourceString="forum.settings.enablecode" AssociatedControlID="chkEnableCode"
                    ToolTipResourceString="forum.settings.enablecode.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkEnableCode" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enablecode.description" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableBold" EnableViewState="false" ResourceString="forum.settings.enableBold" AssociatedControlID="chkEnableBold"
                    ToolTipResourceString="forum.settings.enableBold.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkEnableBold" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enableBold.description" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableItalic" EnableViewState="false" ResourceString="forum.settings.enableItalic" AssociatedControlID="chkEnableItalic"
                    ToolTipResourceString="forum.settings.enableItalic.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkEnableItalic" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enableItalic.description" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableUnderline" EnableViewState="false" AssociatedControlID="chkEnableUnderline"
                    ResourceString="forum.settings.enableUnderline"  ToolTipResourceString="forum.settings.enableUnderline.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkEnableUnderline" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enableUnderline.description" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableStrike" EnableViewState="false" ResourceString="forum.settings.enableStrike" AssociatedControlID="chkEnableStrike"
                    ToolTipResourceString="forum.settings.enableStrike.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkEnableStrike" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enableStrike.description" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableColor" EnableViewState="false" ResourceString="forum.settings.enableColor" AssociatedControlID="chkEnableColor"
                    ToolTipResourceString="forum.settings.enableColor.description" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkEnableColor" runat="server" CssClass="CheckBoxMovedLeft" ToolTipResourceString="forum.settings.enableColor.description" />
            </div>
        </div>
    </div>
    <cms:LocalizedHeading ID="LocalizedHeading1" runat="server" Level="4" EnableViewState="false" ResourceString="general.OptIn" />
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
                    <cms:CMSCheckBox runat="server" ID="chkInheritOptInURL" ResourceString="forum.settings.inheritfromgroup" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" RenderMode="Inline" class="wrap-nowrap">
                        <ContentTemplate>
                            <cms:SelectPath ID="txtOptInURL" runat="server" MaxLength="450" SinglePathMode="true" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSendOptInConfirmation" runat="server" EnableViewState="false" AssociatedControlID="chkSendOptInConfirmation"
                    ResourceString="general.sendoptinconfirmation" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ThreeStateCheckBox ID="chkSendOptInConfirmation" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click"
                    EnableViewState="false" ResourceString="General.OK" />
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Literal runat="server" ID="ltrScript" EnableViewState="false" />