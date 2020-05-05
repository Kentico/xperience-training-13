<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_FormControls_Tags_TagSelector"
     Codebehind="TagSelector.ascx.cs" %>
<asp:Panel ID="pnlTagSelector" runat="server" DefaultButton="btnHidden" CssClass="tag-selector">
    <div class="control-group-inline">
        <cms:CMSTextBox ID="txtTags" runat="server" EnableViewState="true" CssClass="form-control" />
        <cms:CMSButton ID="btnSelect" runat="server" EnableViewState="false" ButtonStyle="Default" />
    </div>

    <cms:LocalizedLabel ID="lblInfoText" runat="server" ResourceString="tags.tagsselector.examples" CssClass="explanation-text"
        EnableViewState="false" />
    <asp:Button ID="btnHidden" runat="server" EnableViewState="false" CssClass="HiddenButton"
        OnClientClick="return false;" />
    <asp:HiddenField runat="server" ID="hdnDialogIdentifier" />
</asp:Panel>
