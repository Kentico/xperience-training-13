<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ObjectEditPanel.ascx.cs"
    Inherits="CMSModules_Objects_Controls_Locking_ObjectEditPanel" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditMenu.ascx" TagName="ObjectEditMenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<cms:CMSObjectManager ID="objectManager" runat="server" />
<asp:Panel runat="server" ID="pnlMenu" EnableViewState="false" CssClass="object-edit-panel">
    <cms:PageTitle runat="server" ID="titleElem" Visible="false" EnableViewState="false" />
    <cms:ObjectEditMenu runat="server" ID="editMenuElem" />
</asp:Panel>
<cms:MessagesPlaceHolder OffsetY="10" runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false" />
