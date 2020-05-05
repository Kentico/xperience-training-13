<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="OM_ContactGroupSettings.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_Cloning_OM_ContactGroupSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkCloneContacts" CssClass="control-label" runat="server" ID="lblCloneContacts" ResourceString="clonning.settings.contactgroup.contacts"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkCloneContacts" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkCloneAccounts" CssClass="control-label" runat="server" ID="lblCloneAccounts" ResourceString="clonning.settings.contactgroup.accounts"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkCloneAccounts" Checked="true" />
        </div>
    </div>
</div>