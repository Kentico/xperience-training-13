<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Reporting_ReportSettings.ascx.cs"
    Inherits="CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSettings" %>
<%@ Register Namespace="CMS.Reporting.Web.UI" TagPrefix="cms" Assembly="CMS.Reporting.Web.UI" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCategory" ResourceString="clonning.settings.report.category"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectReportCategory ID="categorySelector" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCloneSavedReports" ResourceString="clonning.settings.report.savedreports"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkCloneSavedReports" Checked="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSubscriptions" ResourceString="clonning.settings.report.subscriptions"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkSubscriptions" Checked="false" />
        </div>
    </div>
</div>