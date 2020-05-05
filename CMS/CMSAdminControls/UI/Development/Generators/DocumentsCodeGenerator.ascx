<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DocumentsCodeGenerator.ascx.cs" Inherits="CMSAdminControls_UI_Development_Generators_DocumentsCodeGenerator" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagName="FileSystemSelector" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="general.site" DisplayColon="True" AssociatedControlID="ucSite" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SiteSelector runat="server" ID="ucSite" AllowAll="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="Classes.Code.SavePath" DisplayColon="True" AssociatedControlID="ucSavePath" />
        </div>
        <div class="editing-form-value-cell">
            <cms:FileSystemSelector runat="server" ID="ucSavePath" ShowFolders="True" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="Classes.Code.IncludeContainerPageTypes" DisplayColon="True" AssociatedControlID="chkIncludeContainerPageTypes" ToolTipResourceString="Classes.Code.IncludeContainerPageTypes.Tooltip" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkIncludeContainerPageTypes" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:LocalizedButton runat="server" ID="btnSaveCode" ButtonStyle="Primary" ResourceString="Classes.Code.SaveCode" />
        </div>
    </div>
</div>
