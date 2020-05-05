<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Purchase.ascx.cs" Inherits="CMSModules_Activities_Controls_UI_ActivityDetails_Purchase" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblRecord" ResourceString="om.activitydetails.invoice"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLinkButton runat="server" CssClass="form-control-text" ID="btnView" ResourceString="om.activitydetails.viewinvoice" />
        </div>
    </div>
</div>