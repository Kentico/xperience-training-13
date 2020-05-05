<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_LiveControls_Forum"  Codebehind="Forum.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumEdit.ascx" TagName="ForumEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumModerators.ascx" TagName="ForumModerator"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumSecurity.ascx" TagName="ForumSecurity"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/LiveControls/Subscription.ascx" TagName="Subscription"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/LiveControls/Posts.ascx" TagName="Posts"
    TagPrefix="cms" %>

<cms:BasicTabControl ID="tabElem" runat="server" UseClientScript="true" UsePostback="true" />
<asp:Panel runat="server" ID="pnlContent" CssClass="PageContent">
    <asp:PlaceHolder ID="tabPosts" runat="server">
        <cms:Posts ID="postEdit" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="tabGeneral" runat="server" Visible="false">
        <cms:ForumEdit ID="forumEditElem" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="tabSubscriptions" runat="server" Visible="false">
        <cms:Subscription ID="subscriptionElem" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="tabModerators" runat="server" Visible="false">
        <cms:ForumModerator ID="moderatorEdit" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="tabSecurity" runat="server" Visible="false">
        <cms:ForumSecurity ID="securityElem" runat="server"  />
    </asp:PlaceHolder>
</asp:Panel>
