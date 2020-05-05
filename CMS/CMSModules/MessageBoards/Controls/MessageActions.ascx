<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_Controls_MessageActions"  Codebehind="MessageActions.ascx.cs" %>
<div class="BoardMessageDetail">
    <div class="ButtonPadding">
        <asp:LinkButton ID="lnkEdit" Visible="false" runat="server" CssClass="CommentAction" EnableViewState="false" />
        <asp:LinkButton ID="lnkDelete" Visible="false" runat="server" CssClass="CommentAction" EnableViewState="false" />
        <asp:LinkButton ID="lnkApprove" Visible="false" runat="server" CssClass="CommentAction" EnableViewState="false" />
        <asp:LinkButton ID="lnkReject" Visible="false" runat="server" CssClass="CommentAction" EnableViewState="false" />
    </div>
</div>
