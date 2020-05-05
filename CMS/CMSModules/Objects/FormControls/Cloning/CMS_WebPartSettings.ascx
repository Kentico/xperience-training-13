<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_WebPartSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_WebPartSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWebPartCategory" ResourceString="clonning.settings.webpart.category"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="drpWebPartCategories" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectWebpart runat="server" ID="drpWebPartCategories" ShowWebparts="false" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcFiles">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFiles" ResourceString="clonning.settings.webpart.files"
                    EnableViewState="false" DisplayColon="true" AssociatedControlID="chkFiles" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkFiles" Checked="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFileName" ResourceString="clonning.settings.webpart.filename"
                    EnableViewState="false" DisplayColon="true" AssociatedControlID="txtFileName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtFileName" />
            </div>
        </div>
    </asp:PlaceHolder>
     <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAppThemes" ResourceString="clonning.settings.layouts.appthemesfolder"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkAppThemes" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkAppThemes" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWebpartLayouts" ResourceString="clonning.settings.webpart.layouts"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkWebpartLayouts" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkWebpartLayouts" Checked="true" />
        </div>
    </div>
</div>