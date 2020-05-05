<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Users_General_User_Reject"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Theme="Default"  Codebehind="User_Reject.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-group">
        <div class="editing-form-label-cell label-full-width">
            <cms:LocalizedLabel ID="lblReason" runat="server" DisplayColon="true" ResourceString="administration.users.reason" EnableViewState="False" AssociatedControlID="txtReason" CssClass="control-label" />
        </div>
        <div class="editing-form-value-cell textarea-full-width">
            <cms:CMSTextArea ID="txtReason" runat="server" Rows="19" MaxLength="1000" EnableViewState="False" />
        </div>
    </div>
    <div class="control-group-inline">
        <cms:CMSCheckBox ID="chkSendEmail" runat="server" Checked="true" ResourceString="administration.users.email" EnableViewState="False" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Default" ResourceString="general.cancel" EnableViewState="False" />
    <cms:LocalizedButton ID="btnReject" runat="server" ButtonStyle="Primary" ResourceString="general.reject" EnableViewState="False" />
</asp:Content>
