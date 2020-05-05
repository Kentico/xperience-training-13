<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Controls_ResetPassword"  Codebehind="ResetPassword.ascx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>

<cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
<cms:LocalizedLabel runat="server" ID="lblLogonLink" EnableViewState="false" />
<asp:Panel runat="server" ID="pnlReset">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPassword" runat="server" EnableViewState="false" ResourceString="general.password"
                                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PasswordStrength runat="server" ID="passStrength" ValidationGroup="PasswordReset" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblConfirmPassword" runat="server" EnableViewState="false"
                                    ResourceString="general.confirmpassword" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtConfirmPassword" runat="server" TextMode="Password" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                                               ValidationGroup="PasswordReset" Display="Dynamic" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:LocalizedButton runat="server" ID="btnReset" EnableViewState="false" ButtonStyle="Primary"
                    OnClick="btnReset_Click" ValidationGroup="PasswordReset" ResourceString="general.reset" />
            </div>
        </div>
    </div>
</asp:Panel>