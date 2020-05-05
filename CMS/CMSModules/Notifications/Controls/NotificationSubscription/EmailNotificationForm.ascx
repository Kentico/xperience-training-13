<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Notifications_Controls_NotificationSubscription_EmailNotificationForm"  Codebehind="EmailNotificationForm.ascx.cs" %>

<div class="form-group">
    <div class="editing-form-label-cell">
        <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" EnableViewState="false" CssClass="control-label" />
    </div>
    <div class="editing-form-value-cell">
        <cms:CMSTextBox ID="txtEmail" runat="server" CssClass="EmailNotificationForm form-control" EnableViewState="false" />
    </div>
</div>
