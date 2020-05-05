<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_UI_MediaLibraryEdit"  Codebehind="MediaLibraryEdit.ascx.cs" %>

<asp:Panel ID="pnlBody" runat="server" CssClass="MediaLibraryEdit">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <asp:PlaceHolder ID="plcProperties" runat="server">
        <cms:UIForm runat="server" ID="editElem" ObjectType="media.library" RefreshHeader="true" />
    </asp:PlaceHolder>
</asp:Panel>
