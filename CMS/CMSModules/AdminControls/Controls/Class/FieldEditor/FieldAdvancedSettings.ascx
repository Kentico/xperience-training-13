<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="FieldAdvancedSettings.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_FieldAdvancedSettings" %>
<%@ Register TagPrefix="cms" TagName="ConditionBuilder" Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" %>
<cms:LocalizedHeading runat="server" Level="4" ResourceString="templatedesigner.section.fieldadvancedsettings"></cms:LocalizedHeading>
<asp:Panel ID="pnlFieldAdvancedSettings" runat="server" CssClass="FieldAdvancedSettings FieldPanel">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblVisibleMacro" runat="server" EnableViewState="false" ResourceString="fieldeditor.visibilitymacro"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ConditionBuilder ID="visibleMacro" runat="server" ShowAutoCompletionAbove="true" MaxWidth="550" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEnabledMacro" runat="server" EnableViewState="false" ResourceString="fieldeditor.enabledmacro"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ConditionBuilder ID="enabledMacro" runat="server" ShowAutoCompletionAbove="true" MaxWidth="550" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcDisplayInSimpleMode" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayInSimpleMode" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="fieldeditor.displayinsimplemode" AssociatedControlID="chkDisplayInSimpleMode" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkDisplayInSimpleMode" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblHasDepending" runat="server" EnableViewState="false"
                    ResourceString="fieldeditor.hasdepending" DisplayColon="true" AssociatedControlID="chkHasDepending" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkHasDepending" runat="server" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblDependsOn" runat="server" EnableViewState="false"
                    ResourceString="fieldeditor.dependson" DisplayColon="true" AssociatedControlID="chkDependsOn" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkDependsOn" runat="server" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
    </div>
</asp:Panel>