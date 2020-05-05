<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSAdminControls_UI_UIProfiles_UIElementCheckBoxTree"  Codebehind="UIElementCheckBoxTree.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>
<div class="UIPersonalizationTree">
    <cms:UniTree ID="treeElem" runat="server" Localize="true" />
</div>
<asp:HiddenField runat="server" ID="hdnValue" />
