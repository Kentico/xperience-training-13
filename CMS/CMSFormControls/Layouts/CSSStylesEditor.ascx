<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CSSStylesEditor.ascx.cs"
    Inherits="CMSFormControls_Layouts_CSSStylesEditor" %>
<div class="CssStylesEditorWrapperClass">
    <asp:Panel runat="server" ID="pnlWrapper">
        <asp:PlaceHolder runat="server" ID="plcCssLink">
            <asp:Panel runat="server" ID="pnlLink">
                <cms:LocalizedButton runat="server" ID="btnStyles" EnableViewState="false" ResourceString="general.addcss" ButtonStyle="Default" />
            </asp:Panel>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlEditCSS">
            <cms:FormCategoryHeading runat="server" ID="lblLayoutCSS" EnableViewState="false" ResourceString="templatedesigner.section.styles"
                Level="4" IsAnchor="True" AssociatedControlID="txtLayoutCSS" CssClass="editing-form-category-caption" />
            <cms:ExtendedTextArea ID="txtLayoutCSS" runat="server" EditorMode="Advanced" Language="CSS"
                Width="98%" Height="200px" />
        </asp:Panel>
    </asp:Panel>
</div>
