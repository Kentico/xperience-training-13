<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_Controls_BlogCommentDetail" CodeBehind="BlogCommentDetail.ascx.cs" %>
<%@ Reference Control="~/CMSAdminControls/UI/UserPicture.ascx" %>
<%@ Reference Control="~/CMSModules/AbuseReport/Controls/InlineAbuseReport.ascx" %>
<div class="CommentDetail">
    <table width="100%">
        <tr>
            <td rowspan="4">
                <asp:PlaceHolder runat="server" ID="plcUserPicture"></asp:PlaceHolder>
            </td>
            <td style="width: 100%">
                <asp:Label ID="lblName" runat="server" CssClass="CommentUserName" EnableViewState="false" />
                <asp:HyperLink ID="lnkName" runat="server" CssClass="CommentUserName" EnableViewState="false"
                    Target="_blank" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblText" runat="server" CssClass="CommentText" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDate" runat="server" CssClass="CommentDate" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <div class="buttonpedding">
                    <asp:LinkButton ID="lnkEdit" Visible="false" runat="server" CssClass="CommentAction" EnableViewState="false" />
                    <asp:LinkButton ID="lnkDelete" Visible="false" runat="server" CssClass="CommentAction" EnableViewState="false" />
                    <asp:LinkButton ID="lnkApprove" Visible="false" runat="server" CssClass="CommentAction" EnableViewState="false" />
                    <asp:LinkButton ID="lnkReject" Visible="false" runat="server" CssClass="CommentAction" EnableViewState="false" />
                    <asp:PlaceHolder runat="server" ID="plcInlineAbuseReport"></asp:PlaceHolder>
                </div>
            </td>
        </tr>
    </table>
</div>

<script type="text/javascript">
    //<![CDATA[
    // Opens modal dialog with comment edit page
    function EditComment(editPageUrl) {
        modalDialog(editPageUrl, "CommentEdit", 700, 440);
    }
    //]]>
</script>

