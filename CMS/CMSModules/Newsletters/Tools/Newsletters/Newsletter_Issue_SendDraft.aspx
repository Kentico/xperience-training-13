<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Newsletter_Issue_SendDraft.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SendDraft"
    Title="Newsletter issue - Send draft" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedLabel ID="lblSendDraft" runat="server" ResourceString="newsletterissue_send.senddraft"
        EnableViewState="false" CssClass="InfoLabel" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblDraftEmail" runat="server" ResourceString="newsletterissue_send.emails"
                    DisplayColon="true" AssociatedControlID="txtSendDraft" EnableViewState="false" CssClass="control-label" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSendDraft" runat="server" MaxLength="450" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnSend" runat="server" ShowAsButton="true" ButtonStyle="Primary"
        OnClick="btnSend_Click" ResourceString="general.send" />
</asp:Content>