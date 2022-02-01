<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactDataEraserConfiguration.ascx.cs" Inherits="CMSApp.CMSModules.MedioClinic.DataProtection.ContactDataEraserConfiguration" %>

<div class="cms-bootstrap">
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" Level="4" Text="Contact data" />
        <cms:CMSCheckBox ID="chbDeleteContacts" runat="server" Text="Delete contacts" />
        <cms:CMSCheckBox ID="chbDeleteActivities" runat="server" Text="Delete contact activities" />
    </div>
</div>