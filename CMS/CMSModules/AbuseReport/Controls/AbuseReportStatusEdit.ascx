<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AbuseReport/Controls/AbuseReportStatusEdit.ascx.cs"
    Inherits="CMSModules_AbuseReport_Controls_AbuseReportStatusEdit" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTitle" runat="server" ResourceString="general.title" DisplayColon="true"
                EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <asp:Label ID="lblTitleValue" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblUrl" runat="server" ResourceString="general.url" DisplayColon="true"
                EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <asp:HyperLink ID="lnkUrlValue" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblCulture" runat="server" ResourceString="general.culture"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <asp:Label ID="lblCultureValue" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblObjectType" runat="server" ResourceString="abuse.objecttype"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel ID="lblObjectTypeValue" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblObjectName" runat="server" ResourceString="abuse.objectname"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel ID="lblObjectNameValue" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblReportedBy" runat="server" ResourceString="abuse.reportedby"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel ID="lblReportedByValue" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblReportedWhen" runat="server" ResourceString="abuse.reportedwhen"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel ID="lblReportedWhenValue" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" ResourceString="general.site" DisplayColon="true"
                EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <asp:Label ID="lblSiteValue" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblStatus" runat="server" ResourceString="abuse.status" DisplayColon="true"
                EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList ID="drpStatus" runat="server" DataTextField="name" DataValueField="number"
                CssClass="DropDownField" AutoPostBack="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblComment" runat="server" ResourceString="abuse.comment"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextArea ID="txtCommentValue" runat="server" Width="600" Height="300" MaxLength="1000" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="txtCommentValue"
                ValidationGroup="RequiredAbuse" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnOk" runat="server" ValidationGroup="RequiredAbuse" EnableViewState="false" />
            <cms:LocalizedButton ID="btnObjectDetails" runat="server" EnableViewState="false" ResourceString="abuse.details" ButtonStyle="Default" />
        </div>
    </div>
</div>