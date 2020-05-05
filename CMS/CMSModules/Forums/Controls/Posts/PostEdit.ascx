<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Posts_PostEdit"  Codebehind="PostEdit.ascx.cs" %>
<%@ Register Src="ForumPost.ascx" TagName="ForumPost" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>
<asp:Panel ID="pnlContent" runat="server" CssClass="ForumNewPost">
    <asp:Label ID="lblHeader" runat="server" EnableViewState="false" CssClass="Title" />
    <asp:Panel runat="server" ID="pnlReplyPost" CssClass="PostReply" Visible="false">
        <div class="ForumFlat">
            <cms:ForumPost ID="ForumPost1" runat="server" />
        </div>
    </asp:Panel>
    <div class="FormPadding">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" ResourceString="general.username"
                        DisplayColon="true" AssociatedControlID="txtUserName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtUserName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName"
                        Display="Dynamic" ValidationGroup="NewPostforum" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                        DisplayColon="true" AssociatedControlID="txtEmail" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="txtEmail" runat="server" />
                    <cms:CMSRequiredFieldValidator ID="rfvEmailRequired" runat="server"
                        Enabled="false" Display="Dynamic" ValidationGroup="NewPostforum" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" ResourceString="general.subject"
                        DisplayColon="true" AssociatedControlID="txtSubject" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" />
                    <cms:CMSRequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject"
                        Display="Dynamic" ValidationGroup="NewPostforum"></cms:CMSRequiredFieldValidator>
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcThreadType" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblThreadType" runat="server" AssociatedControlID="radTypeDiscussion" />
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
                    <cms:LocalizedLabel CssClass="control-label" ID="lblText" runat="server" EnableViewState="false" AssociatedControlID="ucBBEditor" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:BBEditor ID="ucBBEditor" runat="server" EnableViewState="false" />
                    <cms:CMSRequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="ucBBEditor"
                        Display="Dynamic" ValidationGroup="NewPostforum" />
                    <cms:CMSHtmlEditor ID="htmlTemplateBody" runat="server" Width="500px" Height="200px"
                        Visible="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblSignature" runat="server" EnableViewState="false" AssociatedControlID="txtSignature" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextArea ID="txtSignature" runat="server" Rows="3" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcIsAnswer" runat="server" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblPostIsAnswerLabel" runat="server" EnableViewState="false" AssociatedControlID="txtPostIsAnswer" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtPostIsAnswer" runat="server" MaxLength="9" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcIsNotAnswer" runat="server" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblPostIsNotAnswerLabel" runat="server" EnableViewState="false" AssociatedControlID="txtPostIsNotAnswer" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtPostIsNotAnswer" runat="server" MaxLength="9" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcSubscribe" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblSubscribe" runat="server" EnableViewState="false" AssociatedControlID="chkSubscribe" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox runat="server" ID="chkSubscribe" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click" ValidationGroup="NewPostforum" />
                    <cms:CMSButton ID="btnCancel" runat="server" ButtonStyle="Primary" OnClick="btnCancel_Click" />
                    <cms:CMSButton ID="btnPreview" runat="server" ButtonStyle="Primary" OnClick="btnPreview_Click" ValidationGroup="NewPostforum" />
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />