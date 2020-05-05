<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Macros_ConditionBuilder"
     Codebehind="ConditionBuilder.ascx.cs" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="control-group-inline control-group-inline-wrap condition-builder">
            <cms:CMSPanel ID="pnlConditionBuilder" ShortID="pC" runat="server" CssClass="editing-form-control-nested-control keep-white-space-fixed">
                <cms:MacroEditor runat="server" ID="txtMacro" SingleLineMode="<%# SingleLineMode %>" />
                <asp:Panel runat="server" ID="pnlRule" Visible="false" CssClass="ConditionBuilderRule form-control">
                    <asp:Literal runat="server" ID="ltlMacro" />
                </asp:Panel>
            </cms:CMSPanel>
            <cms:LocalizedButton runat="server" ID="btnEdit" CausesValidation="false" CssClass="btn-first" ButtonStyle="Default" EnableViewState="false" ResourceString="general.edit" />
            <cms:LocalizedButton runat="server" ID="btnClear" CausesValidation="false" CssClass="btn-last" ButtonStyle="Default" EnableViewState="false" ResourceString="general.clear" />
        </div>
        <asp:Button runat="server" ID="btnRefresh" CssClass="HiddenButton" />
        <asp:HiddenField runat="server" ID="hdnValue" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
