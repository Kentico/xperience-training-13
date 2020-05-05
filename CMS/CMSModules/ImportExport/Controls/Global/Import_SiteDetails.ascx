<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ImportExport_Controls_Global_Import_SiteDetails"  Codebehind="Import_SiteDetails.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblSiteDisplayName" runat="server" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtSiteDisplayName" runat="server" />
            <cms:CMSRequiredFieldValidator ID="rfvSiteDisplayName" runat="server" ControlToValidate="txtSiteDisplayName:cntrlContainer:textbox" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblSiteName" runat="server" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtSiteName" runat="server" />
            <cms:CMSRequiredFieldValidator ID="rfvSiteName" runat="server" ControlToValidate="txtSiteName" />
        </div>
    </div>
</div>