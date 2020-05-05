<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_NewPost"  Codebehind="NewPost.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<asp:Label ID="lblHeader" runat="server" EnableViewState="false" CssClass="Title" />
<asp:Panel runat="server" ID="pnlReplyPost" CssClass="PostReply" Visible="false">
    <div class="PostPreviewSubject">
        <asp:Label runat="server" ID="lblSubjectPreview" />
    </div>
    <br />
    <div>
        <asp:Label runat="server" ID="lblTextPreview" />
    </div>
</asp:Panel>
<div class="FormPadding">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <asp:PlaceHolder runat="server" ID="plcUserName">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" ResourceString="general.username"
                        AssociatedControlID="txtUserName" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtUserName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName"
                        Display="Static" ValidationGroup="NewPostforum" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcNickName">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblNickName" runat="server" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label runat="server" ID="lblNickNameValue" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                    AssociatedControlID="txtEmail" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtEmail" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvEmailRequired" runat="server"
                    Enabled="false" Display="Static" ValidationGroup="NewPostforum" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" ResourceString="general.subject"
                    AssociatedControlID="txtSubject" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" />
                <cms:CMSRequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject"
                    Display="Static" ValidationGroup="NewPostforum" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcThreadType" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblThreadType" runat="server" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSRadioButton ID="radTypeDiscussion" runat="server" GroupName="type"
                        ResourceString="forum.settings.discussionthread" Checked="true" />
                    <cms:CMSRadioButton ID="radTypeQuestion" runat="server" GroupName="type" ResourceString="forum.settings.questionthread" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblText" runat="server" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:BBEditor ID="ucBBEditor" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="ucBBEditor"
                    Display="Dynamic" ValidationGroup="NewPostforum" />
                <cms:CMSHtmlEditor ID="htmlTemplateBody" runat="server" Width="500px" Height="200px" Visible="true" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcSignature">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblSignature" runat="server" EnableViewState="false" AssociatedControlID="txtSignature" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextArea ID="txtSignature" runat="server" Rows="3" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="SubscribeHolder">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblSubscribe" runat="server" AssociatedControlID="chkSubscribe" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkSubscribe" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcAttachFile">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblAttachFile" AssociatedControlID="chkAttachFile" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkAttachFile" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-submit">
            <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                ValidationGroup="NewPostforum" />
            <cms:CMSButton ID="btnCancel"
                runat="server" ButtonStyle="Primary" OnClick="btnCancel_Click" />
            <cms:CMSButton ID="btnPreview" runat="server" ButtonStyle="Primary" OnClick="btnPreview_Click"
                ValidationGroup="NewPostforum" />
        </div>
    </div>
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />