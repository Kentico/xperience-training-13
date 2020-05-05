<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Passwords_Password"  Codebehind="Password.ascx.cs" %>
<div>
    <cms:CMSTextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="100" />
    <cms:CMSRequiredFieldValidator ID="rfvPassword" ValidationGroup="ConfirmRegForm" runat="server"
        ControlToValidate="txtPassword" Display="Dynamic" />
</div>
