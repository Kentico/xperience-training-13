<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_ResourceSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_ResourceSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblClonePermissions" ResourceString="clonning.settings.resource.permissions"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkClonePermissions" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkClonePermissions" Checked="true" />
        </div>
    </div>
</div>