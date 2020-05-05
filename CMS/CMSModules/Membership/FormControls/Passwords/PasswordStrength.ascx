<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="PasswordStrength.ascx.cs"
    Inherits="CMSModules_Membership_FormControls_Passwords_PasswordStrength" %>
<div class="password-strength">
    <cms:CMSTextBox runat="server" ID="txtPassword" TextMode="Password" /><asp:Label ID="lblRequiredFieldMark" runat="server" Text="" Visible="false" />
    <div class="password-strength-text">
        <cms:LocalizedLabel runat="server" ID="lblPasswStregth" CssClass="password-strength-hint"
            ResourceString="Membership.PasswordStrength" />
        <strong runat="server" ID="lblEvaluation" EnableViewState="false" ></strong>
    </div>
    <asp:Panel runat="server" ID="pnlPasswStrengthIndicator" CssClass="passw-strength-indicator">
        <asp:Panel runat="server" ID="pnlPasswIndicator">
            &nbsp;
        </asp:Panel>
    </asp:Panel>
    <cms:CMSRequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
        Display="Dynamic" EnableViewState="false" />
</div>
