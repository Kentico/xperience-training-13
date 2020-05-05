<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_WorkflowSettings.ascx.cs"
    Inherits="CMSModules_Workflows_FormControls_Cloning_CMS_WorkflowSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWorkflowScope" ResourceString="clonning.settings.workflow.workflowscope"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkWorkflowScope" Checked="true" />
        </div>
    </div>
</div>