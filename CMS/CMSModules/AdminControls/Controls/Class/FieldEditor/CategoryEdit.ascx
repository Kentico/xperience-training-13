<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CategoryEdit.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_CategoryEdit" %>

<cms:LocalizedHeading runat="server" Level="4" ResourceString="templatedesigner.section.category"></cms:LocalizedHeading>
<asp:Panel ID="pnlCategory" runat="server" CssClass="FieldPanel">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCategoryCaption" runat="server" EnableViewState="false"
                                    ResourceString="TemplateDesigner.CategoryName" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtCategoryCaption" runat="server" AllowMacroEditing="true" FormControlName="LocalizableTextBox" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCollapsible" runat="server" EnableViewState="false"
                    ResourceString="categories.collapsible" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl runat="server" ID="chkCollapsible" AllowMacroEditing="true" FormControlName="CheckBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCollapsedByDefault" runat="server" EnableViewState="false"
                    ResourceString="categories.collapsedbydefault" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl runat="server" ID="chkCollapsedByDefault" AllowMacroEditing="true" FormControlName="CheckBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblVisible" runat="server" EnableViewState="false" ResourceString="formengine.visibilitylabel"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="chkVisible" runat="server" AllowMacroEditing="true" FormControlName="CheckBoxControl" />
            </div>
        </div>
    </div>
</asp:Panel>
