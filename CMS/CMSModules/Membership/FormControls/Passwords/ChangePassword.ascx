<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Passwords_ChangePassword"
     Codebehind="ChangePassword.ascx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>

<asp:Panel ID="pnlChangePassword" runat="server" DefaultButton="btnOK" CssClass="PasswordPanel">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblExistingPassword" AssociatedControlID="txtExistingPassword"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtExistingPassword" runat="server" TextMode="Password" MaxLength="100"
                    EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblPassword1" AssociatedControlID="passStrength:txtPassword" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PasswordStrength runat="server" ID="passStrength" ValidationGroup="ChangePassword" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblPassword2" AssociatedControlID="txtPassword2" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtPassword2" runat="server" TextMode="Password" MaxLength="100"
                    EnableViewState="false" />
            </div>
        </div>
        <div class="form-group form-group-submit">
                <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    ValidationGroup="ChangePassword" />
        </div>
    </div>
</asp:Panel>