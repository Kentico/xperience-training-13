<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CustomTableSubmit.ascx.cs" Inherits="CMSModules_Activities_Controls_UI_ActivityDetails_CustomTableSubmit" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblRecord" ResourceString="om.activity.record"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLinkButton runat="server" ID="btnView" ResourceString="om.activitydetails.viewrecord" CssClass="form-control-text"/>
        </div>
    </div>
</div>