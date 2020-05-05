<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_SubscriptionForm"  Codebehind="SubscriptionForm.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>

<asp:Panel runat="server" ID="pnlPadding" CssClass="FormPadding" DefaultButton="btnOK">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                    DisplayColon="true" AssociatedControlID="txtSubscriptionEmail" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtSubscriptionEmail" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvEmailRequired" runat="server" Display="Dynamic" ValidationGroup="NewSubscription" />
            </div>
        </div>
        <div class="form-group form-group-submit">
            <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" ValidationGroup="NewSubscription"
                OnClick="btnOK_Click" />
            <cms:CMSButton ID="btnCancel" runat="server" ButtonStyle="Primary" OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Panel>
