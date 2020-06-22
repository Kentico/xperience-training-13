<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="Edit.ascx.cs" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_SourcePoint_Edit" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" TagName="ConditionBuilder"
    TagPrefix="cms" %>

<div class="form-horizontal">
    <asp:Panel ID="plcCondition" runat="server" CssClass="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblCondition" runat="server" EnableViewState="false" ResourceString="workflowstep.sourcepoint.condition"
                                CssClass="control-label" AssociatedControlID="cbCondition" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ConditionBuilder ID="cbCondition" runat="server" MaxWidth="720" />
        </div>
    </asp:Panel>
    <asp:Panel ID="plcLabel" runat="server" CssClass="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblLabel" runat="server" EnableViewState="false" ResourceString="workflowstep.sourcepoint.label"
                CssClass="control-label" AssociatedControlID="txtLabel" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtLabel" runat="server" MaxLength="450" />
            <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorLabel" runat="server" EnableViewState="false"
                ControlToValidate="txtLabel:cntrlContainer:textbox" Display="Dynamic" />
        </div>
    </asp:Panel>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblText" runat="server" EnableViewState="false" ResourceString="workflowstep.sourcepoint.text"
                CssClass="control-label" AssociatedControlID="txtText" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtText" runat="server" MaxLength="450" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblTooltip" runat="server" EnableViewState="false" ResourceString="workflowstep.sourcepoint.tooltip"
                CssClass="control-label" AssociatedControlID="txtTooltip" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtTooltip" runat="server" MaxLength="450" TextMode="MultiLine" />
        </div>
    </div>
    <asp:Panel ID="plcStepAllowReject" runat="server" CssClass="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblStepAllowReject" runat="server" EnableViewState="false" ResourceString="WorkflowStep.AllowMoveToPrevious"
                                CssClass="control-label" AssociatedControlID="cbCondition" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkStepAllowReject" runat="server" />
        </div>
    </asp:Panel>
</div>