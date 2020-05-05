<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_FormUserControlSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_FormUserControlSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFiles" ResourceString="clonning.settings.formusercontrol.files"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkFiles" ToolTipResourceString="clonning.settings.formusercontrol.files.tooltip" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkFiles" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFileName" ResourceString="clonning.settings.formusercontrol.filename"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtFileName" ToolTipResourceString="clonning.settings.formusercontrol.filename.tooltip" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtFileName" />
        </div>
    </div>
</div>