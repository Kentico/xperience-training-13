<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_Controls_BlogCommentView"  Codebehind="BlogCommentView.ascx.cs" %>

<%@ Register Src="BlogCommentEdit.ascx" TagName="BlogCommentEdit" TagPrefix="cms" %>
<%@ Register Src="NewSubscription.ascx" TagName="NewSubscription" TagPrefix="cms" %>

<a id="comments"></a>
<asp:Label ID="lblTitle" runat="server" EnableViewState="false" CssClass="BlogCommentsTitle" />
<div>
    <cms:LocalizedLabel ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel" ResourceString="blog.commentview.nocomments" />
</div>
<div>
    <asp:Repeater ID="rptComments" runat="server" />
</div>
<asp:Panel ID="pnlComment" runat="server">
    <cms:LocalizedLabel CssClass="control-label blog-leave-comment" ID="lblLeaveCommentLnk" runat="server" EnableViewState="false"
        ResourceString="blog.commentview.lnkleavecomment" AssociatedControlID="plcBtnSubscribe" />
    <asp:PlaceHolder ID="plcBtnSubscribe" runat="server">
        <cms:LocalizedLinkButton ID="btnSubscribe" runat="server" EnableViewState="false"
            ResourceString="blog.commentview.lnksubscription" />
    </asp:PlaceHolder>
    <cms:BlogCommentEdit ID="ctrlCommentEdit" runat="server" />
</asp:Panel>
<asp:Panel ID="pnlSubscription" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <cms:LocalizedLabel CssClass="control-label blog-subscribe" ID="lblNewSubscription" runat="server" EnableViewState="false"
                ResourceString="blog.commentview.lnksubscription" AssociatedControlID="btnLeaveMessage" />
            <cms:LocalizedLinkButton ID="btnLeaveMessage" runat="server" EnableViewState="false"
                ResourceString="blog.commentview.lnkleavemessage" />
        </div>
    </div>
    <cms:NewSubscription ID="elemSubscription" runat="server" />
</asp:Panel>
<asp:HiddenField ID="hdnSelSubsTab" runat="server" />