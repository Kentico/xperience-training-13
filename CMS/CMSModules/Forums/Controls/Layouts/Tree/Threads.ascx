<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Layouts_Tree_Threads"
     Codebehind="Threads.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/PostTree.ascx" TagName="PostTree"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AbuseReport/Controls/InlineAbuseReport.ascx" TagName="AbuseReport"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/AttachmentDisplayer.ascx" TagName="AttachmentDisplayer"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/ForumBreadcrumbs.ascx" TagName="ForumBreadcrumbs"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/ThreadMove.ascx" TagName="ThreadMove"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.Forums" Assembly="CMS.Forums" TagPrefix="cms" %>
<cms:CMSUpdatePanel runat="server" ID="pnlSelected">
    <ContentTemplate>
        <asp:HiddenField runat="server" ID="hdnSelected" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<div class="Forum">
    <div class="ForumTree">
        <table class="Table" cellspacing="0" cellpadding="0">
            <asp:PlaceHolder runat="server" ID="plcHeader">
                <tr class="Info">
                    <td>
                        <span class="ForumName">
                            <%=HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ForumContext.CurrentForum.ForumDisplayName))%>
                        </span><span class="ForumDescription">
                            <%=HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ForumContext.CurrentForum.ForumDescription))%>
                        </span>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr class="Actions">
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%;">
                        <tbody>
                            <tr>
                                <td style="border: medium none; margin: 0px; padding: 0px;">
                                    <%=GetLink(null, ResHelper.GetString("Forums.Forum.lnkNewThread"), "ActionLink", ForumActionType.NewThread)%>
                                    <asp:PlaceHolder runat="server" ID="plcActionSeparator"><span>| </span></asp:PlaceHolder>
                                    <%=GetLink(null, ResHelper.GetString("Forums.Forum.SubscribeForum"), "ActionLink", ForumActionType.SubscribeToForum)%>
                                    <asp:PlaceHolder runat="server" ID="plcAddToFavoritesSeparator"><span>| </span></asp:PlaceHolder>
                                    <%=GetLink(ForumContext.CurrentForum, ResHelper.GetString("Forums.Forum.AddForumToFavorites"), "ActionLink", ForumActionType.AddForumToFavorites)%>
                                    <asp:PlaceHolder runat="server" ID="plcBreadcrumbsSeparator"><span>| </span></asp:PlaceHolder>
                                    <span class="ForumBreadCrumbs">
                                        <cms:ForumBreadcrumbs ID="ForumBreadcrumbs1" runat="server" DisplayThread="false" />
                                    </span>
                                </td>
                                <td style="border: medium none; margin: 0px; padding: 0px;">
                                </td>
                            </tr>
                            <asp:PlaceHolder runat="server" ID="plcMoveThread" Visible="false">
                                <tr>
                                    <td class="ThreadMove" colspan="2" style="border-bottom: medium none; border-left: medium none;
                                        border-right: medium none;">
                                        <cms:ThreadMove runat="server" ID="threadMove" ResourceString="forum.thread.movetopic" />
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                        </tbody>
                    </table>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcPostBody">
                <tr>
                    <td class="Posts">
                        <div class="PostDetail">
                            <asp:PlaceHolder runat="server" ID="plcPostPreview" EnableViewState="false" Visible="false">
                                <div class="ForumPost<%=mUnapproved%>">
                                    <table class="PostImage" cellspacing="0" cellpadding="0" border="0" style="border: medium none;
                                        width: 100%;">
                                        <tbody>
                                            <tr>
                                                <td style="border: medium none;" colspan="2">
                                                    <asp:Panel runat="server" ID="pnlManage" CssClass="ForumManage">
                                                        <asp:Literal runat="server" ID="ltlApprove" />
                                                        <asp:Literal runat="server" ID="ltlApproveAll" />
                                                        <asp:Literal runat="server" ID="ltlReject" />
                                                        <asp:Literal runat="server" ID="ltlRejectAll" />
                                                        <asp:Literal runat="server" ID="ltlSplit" />
                                                        <asp:Literal runat="server" ID="ltlMove" />
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr style="border: medium none;">
                                                <td style="border: medium none; padding-right: 5px; vertical-align: top;">
                                                    <asp:Literal runat="server" ID="ltlAvatar" EnableViewState="false" />
                                                    <asp:Literal runat="server" ID="ltlBadge" EnableViewState="false" />
                                                    <asp:Literal runat="server" ID="ltlActions" EnableViewState="false" />
                                                </td>
                                                <td style="border: medium none; width: 100%;">
                                                    <div style="float: left;">
                                                        <asp:Literal runat="server" ID="ltlPostUser" EnableViewState="false" />
                                                        <span class="PostSeparator">-</span> <span class="PostTime">(<asp:Literal runat="server"
                                                            ID="ltlPostTime" />) </span>
                                                    </div>
                                                    <span style="float: left;">&nbsp;
                                                        <asp:Literal runat="server" ID="ltlAddPostToFavorites" />
                                                    </span>
                                                    <div style="float: right;">
                                                        <asp:Literal runat="server" ID="ltlWasHelpful" />
                                                        <asp:Literal runat="server" ID="ltlAnswer" />
                                                        <asp:Literal runat="server" ID="ltlNotAnswer" />
                                                    </div>
                                                    <div style="clear: both">
                                                    </div>
                                                    <span class="PostSubject">
                                                        <asp:Literal runat="server" ID="ltlPostSubject" EnableViewState="false" /></span>
                                                    <div class="PostText">
                                                        <cms:ResolvedLiteral runat="server" ID="ltlPostText" EnableViewState="false" />
                                                    </div>
                                                    <div style="clear: both">
                                                        <cms:AttachmentDisplayer ID="attachmentDisplayer" runat="server" />
                                                    </div>
                                                    <asp:Literal runat="server" ID="ltlSignature" EnableViewState="false" />
                                                    <br />
                                                    <div style="float: left;">
                                                        <asp:Literal runat="server" ID="ltlReply" />
                                                        <asp:PlaceHolder runat="server" ID="plcFirstSeparator"><span class="PostActionSeparator">
                                                            |</span> </asp:PlaceHolder>
                                                        <asp:Literal runat="server" ID="ltlQuote" />
                                                        <asp:PlaceHolder runat="server" ID="plcSecondSeparator"><span class="PostActionSeparator">
                                                            |</span> </asp:PlaceHolder>
                                                        <asp:Literal runat="server" ID="ltlSubscribe" />
                                                    </div>
                                                    <div style="float: right;">
                                                        <asp:Literal runat="server" ID="ltlEdit" />&nbsp;
                                                        <asp:Literal runat="server" ID="ltlDelete" />&nbsp;
                                                        <asp:Literal runat="server" ID="ltlAttachments" />&nbsp;
                                                        <cms:AbuseReport ID="ucAbuse" runat="server" ReportObjectType="forums.forumpost"
                                                            CssClass="ActionLink" />
                                                    </div>
                                                    <div style="clear: both;">
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                        <cms:PostTree ID="postTreeElem" runat="server" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </div>
</div>
