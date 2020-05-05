<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_UserSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_UserSettings" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEmail" ResourceString="clonning.settings.user.email"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtEmail" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtEmail" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblGeneratePassword" ResourceString="clonning.settings.user.generatepassword"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkGeneratePassword" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkGeneratePassword" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblPassword" ResourceString="clonning.settings.user.password"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtPassword" />
        </div>
        <div class="editing-form-value-cell">
            <cms:PasswordStrength runat="server" ID="txtPassword" AllowEmpty="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblPermissions" ResourceString="clonning.settings.user.permissions"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkPermissions" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkPermissions" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblPersonalization" ResourceString="clonning.settings.user.personalization"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkPersonalization" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkPersonalization" Checked="true" />
        </div>
    </div>
</div>