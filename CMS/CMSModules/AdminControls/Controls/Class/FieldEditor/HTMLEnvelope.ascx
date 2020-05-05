<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="HTMLEnvelope.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_HTMLEnvelope" %>
<cms:LocalizedHeading runat="server" Level="4" ResourceString="templatedesigner.section.htmlenvelope"></cms:LocalizedHeading>
<asp:Panel ID="pnlHtmlEnvelope" runat="server" Enabled="true" CssClass="FieldPanel">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblContentBefore" runat="server" EnableViewState="false" DisplayColon="true" 
                    ToolTipResourceString="templatedesigner.contentbefore.tooltip" ResourceString="TemplateDesigner.ContentBefore" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtContentBefore" runat="server" AllowMacroEditing="true" FormControlName="LargeTextArea" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblContentAfter" runat="server" EnableViewState="false" DisplayColon="true"
                     ToolTipResourceString="templatedesigner.contentafter.tooltip" ResourceString="TemplateDesigner.ContentAfter" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtContentAfter" runat="server" AllowMacroEditing="true" FormControlName="LargeTextArea" />
            </div>
        </div>
    </div>
</asp:Panel>