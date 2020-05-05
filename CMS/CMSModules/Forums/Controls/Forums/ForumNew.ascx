<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Forums_ForumNew"  Codebehind="ForumNew.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumDisplayName" EnableViewState="false" AssociatedControlID="txtForumDisplayName" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtForumDisplayName" runat="server" MaxLength="200" />
            <cms:CMSRequiredFieldValidator ID="rfvForumDisplayName" runat="server" ErrorMessage="forum_general.emptydisplayname"
                ControlToValidate="txtForumDisplayName:cntrlContainer:textbox" Display="Dynamic" ValidationGroup="vgForum" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcCodeName" runat="Server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumName" EnableViewState="false" AssociatedControlID="txtForumName" ShowRequiredMark="true"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtForumName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator Display="Dynamic" ID="rfvForumName" runat="server" ErrorMessage=""
                    ControlToValidate="txtForumName" ValidationGroup="vgForum" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumDescription" AssociatedControlID="txtForumDescription"
                EnableViewState="false" ResourceString="general.description" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtForumDescription" runat="server" TextMode="MultiLine" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcBaseAndUnsubUrl" runat="server" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblBaseUrl" EnableViewState="false" AssociatedControlID="txtBaseUrl" />
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
                <asp:Label CssClass="control-label" runat="server" ID="lblUnsubscriptionUrl" EnableViewState="false" AssociatedControlID="txtUnsubscriptionUrl" />
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
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" runat="server" ID="lblForumRequireEmail" EnableViewState="false" AssociatedControlID="chkForumRequireEmail" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ThreeStateCheckBox ID="chkForumRequireEmail" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" runat="server" ID="lblForumDisplayEmails" EnableViewState="false" AssociatedControlID="chkForumDisplayEmails" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ThreeStateCheckBox ID="chkForumDisplayEmails" runat="server" />
        </div>
    </div>
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
            <asp:Label CssClass="control-label" runat="server" ID="lblCaptcha" EnableViewState="false" AssociatedControlID="chkCaptcha" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ThreeStateCheckBox ID="chkCaptcha" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" runat="server" ID="lblForumOpen" EnableViewState="false" AssociatedControlID="chkForumOpen" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkForumOpen" runat="server" CssClass="CheckBoxMovedLeft" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" runat="server" ID="lblForumLocked" EnableViewState="false" AssociatedControlID="chkForumLocked" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkForumLocked" runat="server" CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" runat="server" ID="lblForumModerated" EnableViewState="false" AssociatedControlID="chkForumModerated" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkForumModerated" runat="server" CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                ValidationGroup="vgForum" ResourceString="General.OK" />
        </div>
    </div>
</div>
<asp:Literal runat="server" ID="ltrScript" EnableViewState="false" />