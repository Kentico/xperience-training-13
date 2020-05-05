<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TriggerScoreProperties.ascx.cs"
    Inherits="CMSModules_ContactManagement_Controls_UI_Automation_TriggerScoreProperties" %>
<%@ Register Src="~/CMSFormControls/Basic/TextBoxControl.ascx" TagName="TextBoxControl"
    TagPrefix="cms" %>
<div class="form-group">
    <div class="editing-form-label-cell">
        <cms:FormLabel CssClass="control-label" ID="lblScore" runat="server" EnableViewState="false" ResourceString="om.score.scorevalue"
            DisplayColon="true" />
    </div>
    <div class="editing-form-value-cell">
        <cms:TextBoxControl ID="txtScore" FilterType="Numbers, Custom" runat="server" ValidChars="-" />
    </div>
</div>