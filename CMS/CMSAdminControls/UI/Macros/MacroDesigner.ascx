<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_MacroDesigner"
     Codebehind="MacroDesigner.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroRuleDesigner.ascx" TagName="MacroRuleEditor"
    TagPrefix="cms" %>
<cms:UITabs runat="server" ID="tabsElem" />
<asp:Panel runat="server" ID="pnlRuleEditor" CssClass="macro-editor">
    <cms:MacroRuleEditor runat="server" ID="ruleElem" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlEditor" CssClass="MacroDesigner" Visible="false">    
    <cms:MacroEditor runat="server" ID="editorElem" UseAutoComplete="true" MixedMode="false"
        Height="540px" />
</asp:Panel>
<asp:Button runat="server" ID="btnShowCode" CssClass="HiddenButton" EnableViewState="false" OnClick="btnShowCode_Click" />
<asp:Button runat="server" ID="btnShowRuleEditor" CssClass="HiddenButton" EnableViewState="false" OnClick="btnShowRuleEditor_Click" />
<asp:HiddenField runat="server" ID="hdnSelTab" EnableViewState="false" />
<asp:HiddenField runat="server" ID="hdnCondition" />
