<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Shortcuts"  Codebehind="~/CMSWebParts/Community/Shortcuts.ascx.cs" %>
<asp:Panel runat="server" ID="pnlProfileLinks" Visible="false" EnableViewState="false"
    CssClass="ShortcutProfileLinks">
    <asp:Panel ID="pnlMyProfile" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkMyProfile" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlEditMyProfile" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkEditMyProfile" runat="server" />
    </asp:Panel>
</asp:Panel>
<asp:Panel runat="server" ID="pnlPersonalLinks" Visible="false" EnableViewState="false"
    CssClass="ShortcutPersonalLinks">
    <asp:Panel ID="pnlJoinCommunity" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkJoinCommunity" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlMyLinks" runat="server" CssClass="ShortcutMyLinks">
        <asp:Panel ID="pnlMyInvitations" runat="server" Visible="false" EnableViewState="false"
            CssClass="ShortcutPanel">
            <cms:LocalizedHyperlink ID="lnkMyInvitations" runat="server" />
        </asp:Panel>      
    </asp:Panel>
</asp:Panel>
<asp:Panel runat="server" ID="pnlGroupLinks" Visible="false" EnableViewState="false"
    CssClass="ShortcutGroupLinks">
    <asp:Panel ID="pnlCreateNewGroup" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkCreateNewGroup" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlJoinGroup" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkJoinGroup" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlLeaveGroup" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkLeaveGroup" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlManageGroup" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkManageGroup" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlInviteToGroup" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkInviteToGroup" runat="server" />
    </asp:Panel>
</asp:Panel>
<asp:Panel runat="server" ID="pnlBlogLinks" Visible="false" EnableViewState="false"
    CssClass="ShortcutBlogLinks">
    <asp:Panel ID="pnlCreateNewBlog" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkCreateNewBlog" runat="server" />
    </asp:Panel>
</asp:Panel>
<asp:Panel runat="server" ID="pnlSignInOut" Visible="false" EnableViewState="false"
    CssClass="ShortcutSignInOutLinks">
    <asp:Panel ID="pnlSignIn" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedHyperlink ID="lnkSignIn" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlSignOut" runat="server" Visible="false" EnableViewState="false"
        CssClass="ShortcutPanel">
        <cms:LocalizedLinkButton ID="btnSignOut" OnClick="btnSignOut_Click" runat="server" />
    </asp:Panel>
</asp:Panel>
