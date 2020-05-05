<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Categories_Controls_CategoryEdit"
     Codebehind="CategoryEdit.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>

<cms:HeaderActions ID="headerActions" runat="server" PanelCssClass="cms-edit-menu" />
<asp:Panel ID="pnlEdit" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server"/>
    <cms:UIContextPanel ID="pnlContext" runat="server">
        <cms:UIForm ID="editElem" runat="server" ObjectType="cms.category" EnableViewState="false" RedirectUrlAfterCreate="" />
    </cms:UIContextPanel>
</asp:Panel>
