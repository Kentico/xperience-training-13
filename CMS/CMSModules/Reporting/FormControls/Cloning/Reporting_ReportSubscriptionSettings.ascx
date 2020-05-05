<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="Reporting_ReportSubscriptionSettings.ascx.cs"
    Inherits="CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSubscriptionSettings" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEmail" ResourceString="general.email" EnableViewState="false"
                DisplayColon="true" AssociatedControlID="txtEmail" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:EmailInput runat="server" ID="txtEmail" AllowMultipleAddresses="True"/>
            <cms:CMSRequiredFieldValidator runat="server" ID="rfvEmail"
                Display="Dynamic" EnableViewState="false" />
        </div>
    </div>
</div>