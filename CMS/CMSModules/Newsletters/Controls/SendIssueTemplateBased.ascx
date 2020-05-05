<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SendIssueTemplateBased.ascx.cs" Inherits="CMSModules_Newsletters_Controls_SendIssueTemplateBased" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblDateTime" runat="server" ResourceString="newsletterissue_send.datetime"
                DisplayColon="true" AssociatedControlID="calendarControl" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker ID="calendarControl" runat="server" Enabled="true" DisplayNow="false" />
        </div>
    </div>
</div>
