<%@ Page Title="Email templates - Send draft" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Theme="Default"
    AutoEventWireup="true" CodeBehind="SendDraft.aspx.cs" Inherits="CMSModules_EmailTemplates_Pages_SendDraft" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedLabel ID="lblSendDraft" runat="server" ResourceString="emailtemplates_senddraft.instruction" EnableViewState="false" CssClass="InfoLabel" />
    <div class="form-horizontal">
        <asp:PlaceHolder ID="plcFrom" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblFrom" runat="server" ResourceString="general.fromemail"
                        DisplayColon="true" AssociatedControlID="emlFrom:txtEmailInput" EnableViewState="false" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="emlFrom" runat="server" AllowMultipleAddresses="False" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblRecipients" runat="server" ResourceString="emailtemplates_senddraft.emails"
                    DisplayColon="true" AssociatedControlID="emlRecipients:txtEmailInput" EnableViewState="false" CssClass="control-label" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="emlRecipients" runat="server" AllowMultipleAddresses="True" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnSend" runat="server" ShowAsButton="true" ButtonStyle="Primary" EnableViewState="False"
        OnClick="btnSend_Click" ResourceString="general.send" />
</asp:Content>
