<%@ Control Language="C#" AutoEventWireup="True"  Codebehind="ValidationSettings.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_ValidationSettings" %>
<cms:LocalizedHeading runat="server" Level="4" ResourceString="TemplateDesigner.Section.Validation"></cms:LocalizedHeading>
<asp:Panel ID="pnlSectionValidation" runat="server" CssClass="FieldPanel">
    <div class="form-horizontal">
        <asp:PlaceHolder runat="server" ID="plcSpellCheck" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSpellCheck" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.SpellCheck" AssociatedControlID="chkSpellCheck" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkSpellCheck" runat="server" CssClass="CheckBoxMovedLeft" Checked="true" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblErrorMessage" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.ErrorMessage" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtErrorMessage" runat="server" AllowMacroEditing="true" FormControlName="LocalizableTextBox" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblMacroRule" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.MacroRule" AssociatedControlID="ruleDesigner" />
            </div>
            <div class="editing-form-value-cell">
                <cms:FieldMacroRuleDesigner runat="server" ID="ruleDesigner" />
            </div>
        </div>
    </div>
</asp:Panel>