<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DocumentSource.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_DocumentSource" %>

<asp:Panel ID="pnlSourceField" runat="server" Visible="false">
    <div class="content-block-50">
        <cms:LocalizedLabel CssClass="control-label" ID="lblSourceField" runat="server" EnableViewState="false"
            ResourceString="TemplateDesigner.SourceField" AssociatedControlID="drpSourceField" />
        <cms:CMSDropDownList ID="drpSourceField" runat="server" CssClass="SourceFieldDropDown"
            AutoPostBack="true" OnSelectedIndexChanged="drpSourceField_SelectedIndexChanged" />
    </div>
    <div class="content-block-50">
        <cms:LocalizedLabel CssClass="control-label" ID="lblSourceAliasField" runat="server" EnableViewState="false"
            ResourceString="TemplateDesigner.SourceAliasField" AssociatedControlID="drpSourceAliasField" />
        <cms:CMSDropDownList ID="drpSourceAliasField" runat="server" CssClass="SourceFieldDropDown"
            AutoPostBack="true" OnSelectedIndexChanged="drpSourceField_SelectedIndexChanged" />
    </div>
</asp:Panel>