<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="MA_AutomationProcessSettings.ascx.cs"
    Inherits="CMSModules_OnlineMarketing_FormControls_Cloning_MA_AutomationProcessSettings" %>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkWorkflowTrigger" CssClass="control-label" runat="server" ID="lblWorkflowTrigger" ResourceString="clonning.settings.workflow.workflowtrigger"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkWorkflowTrigger" Checked="true" />
        </div>
    </div>
</div>