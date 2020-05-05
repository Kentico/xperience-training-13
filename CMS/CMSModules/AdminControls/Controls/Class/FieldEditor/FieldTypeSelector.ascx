<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="FieldTypeSelector.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_FieldTypeSelector" %>
<asp:Panel ID="pnlTypeSelector" runat="server" CssClass="FieldPanel" Visible="false">
    <cms:LocalizedHeading ID="LocalizedHeading1" runat="server" Level="4" ResourceString="fieldeditor.fieldtype.fieldtype"></cms:LocalizedHeading>
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblHeight" runat="server" EnableViewState="false" ResourceString="fieldeditor.fieldtype.fieldtype" DisplayColon="true" AssociatedControlID="drpFieldType" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList ID="drpFieldType" runat="server" UseResourceStrings="true" AutoPostBack="true" CssClass="DropDownField" EnableViewState="true" OnSelectedIndexChanged="drpFieldType_SelectedIndexChanged" />
            </div>
        </div>
    </div>
</asp:Panel>