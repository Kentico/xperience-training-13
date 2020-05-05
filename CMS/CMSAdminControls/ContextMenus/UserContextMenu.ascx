<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSAdminControls_ContextMenus_UserContextMenu"  Codebehind="UserContextMenu.ascx.cs" %>
<asp:Panel runat="server" ID="pnlUserContextMenu">
    <asp:Repeater runat="server" ID="repItem">
        <ItemTemplate>
            <asp:Panel runat="server" ID="pnlItem" CssClass="Item">
                <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                    <asp:Label runat="server" ID="lblItem" CssClass="Name" EnableViewState="false" Text='<%#DataBinder.Eval(Container.DataItem, "ActionDisplayName")%>' />
                </asp:Panel>
            </asp:Panel>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>