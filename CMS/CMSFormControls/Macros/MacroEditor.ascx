<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="MacroEditor.ascx.cs" Inherits="CMSFormControls_Macros_MacroEditor" %>

<div class="macro-editor-form-control">
    <cms:MacroEditor runat="server" ID="ucEditor" />
    <asp:PlaceHolder runat="server" ID="plcInsertMacro" Visible="False">
        <cms:CMSAccessibleButton runat="server" ID="btnInsertMacro" CausesValidation="false" CssClass="btn-first"
            IconOnly="True" IconCssClass="icon-braces-octothorpe" EnableViewState="false" />
    </asp:PlaceHolder>
</div>
