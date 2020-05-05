<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="CSSsettings.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_CSSsettings" %>
<cms:LocalizedHeading runat="server" Level="4" ResourceString="templatedesigner.section.styles"></cms:LocalizedHeading>
<asp:Panel ID="pnlCss" runat="server" CssClass="FieldPanel">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFieldCssClass" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.FieldCssClass" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtFieldCssClass" runat="server" AllowMacroEditing="true" FormControlName="TextBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCaptionCellCssClass" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.CaptionCellCssClass" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtCaptionCellCssClass" runat="server" AllowMacroEditing="true" FormControlName="TextBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCaptionCssClass" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.CaptionCssClass" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtCaptionCssClass" runat="server" AllowMacroEditing="true" FormControlName="TextBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCaptionStyle" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.CaptionStyle" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtCaptionStyle" runat="server" AllowMacroEditing="true" FormControlName="TextBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblControlCellCssClass" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.ControlCellCssClass" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtControlCellCssClass" runat="server" AllowMacroEditing="true" FormControlName="TextBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblControlCssClass" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.ControlCssClass" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtControlCssClass" runat="server" AllowMacroEditing="true" FormControlName="TextBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblInputStyle" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.InputStyle" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtInputStyle" runat="server" AllowMacroEditing="true" FormControlName="TextBoxControl" />
            </div>
        </div>
    </div>
</asp:Panel>
