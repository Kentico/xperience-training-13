<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_WebPartLayoutSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_WebPartLayoutSettings" %>

<div class="form-horizontal">
     <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAppThemes" ResourceString="clonning.settings.layouts.appthemesfolder"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkAppThemes" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkAppThemes" Checked="true" />
        </div>
    </div>
</div>