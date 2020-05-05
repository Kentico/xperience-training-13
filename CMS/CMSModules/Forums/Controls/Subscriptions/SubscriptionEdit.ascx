<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_Subscriptions_SubscriptionEdit" Codebehind="SubscriptionEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSubscriptionEmail" EnableViewState="false"
                ResourceString="general.email" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:EmailInput ID="txtSubscriptionEmail" runat="server" />
            <cms:CMSRequiredFieldValidator ID="rfvSubscriptionEmail" runat="server" Display="Dynamic" />
        </div>
    </div>
    <asp:Panel runat="server" ID="pnlSendConfirmationEmail" Visible="true">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSendConfirmationEmail" EnableViewState="false"
                ResourceString="forums.forumsubscription.sendemail" DisplayColon="true" AssociatedControlID="chkSendConfirmationEmail" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkSendConfirmationEmail" />
        </div>
    </asp:Panel>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                />
        </div>
    </div>
</div>