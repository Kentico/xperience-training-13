<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Layouts_Flat_Thread"  Codebehind="Thread.ascx.cs" %>
<%@ Import Namespace="CMS.Globalization" %>
<%@ Import Namespace="CMS.Membership" %>
<%@ Import Namespace="CMS.SiteProvider" %>
<%@ Import Namespace="CMS.DataEngine" %>
<%@ Register Src="~/CMSModules/Forums/Controls/ForumViewModeSelector.ascx" TagName="ForumViewModeSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/AttachmentDisplayer.ascx" TagName="AttachmentDisplayer" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AbuseReport/Controls/InlineAbuseReport.ascx" TagName="AbuseReport" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/ForumBreadcrumbs.ascx" TagName="ForumBreadcrumbs" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/ThreadMove.ascx" TagName="ThreadMove" TagPrefix="cms" %>
<%@ Register Namespace="CMS.Forums" Assembly="CMS.Forums" TagPrefix="cms" %>


<div class="Forum" id="<%=ClientID%>">
    <div class="ForumFlat">
        <table class="Table" cellspacing="0" cellpadding="0">
            <tbody>
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
                                        <span class="ForumBreadCrumbs">
                                            <cms:forumbreadcrumbs enableviewstate="false" id="ForumBreadcrumbs1" runat="server" />
                                        </span>
                                    </td>
                                    <td style="border: medium none; margin: 0px; padding: 0px; text-align: right;">
                                        <cms:ForumViewModeSelector EnableViewState="false" ID="ForumViewModeSelector1" runat="server" />
                                    </td>
                                </tr>
                                <asp:PlaceHolder runat="server" ID="plcMoveThread" Visible="false">
                                    <tr>
                                        <td colspan="2" style="border: medium none; height: 5px;">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ThreadMove" colspan="2" style="border-bottom: medium none; border-left: medium none;
                                            border-right: medium none;">
                                            <cms:threadmove runat="server" id="threadMove" resourcestring="forum.thread.movetopic" />
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="ForumContent">
                        <div class="Posts">
                            <table class="PostsTable" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;">
                                <tbody>
                                    <cms:uniview runat="server" id="listForums" enableviewstate="false">
                                        <ItemTemplate>
                                            <tr class="Post">
                                                <td>
                                                    <%#GenerateIndentCode(Container.DataItem, "<div class=\"PostIndent\">")%>
                                                    <div class="ForumPost <%#GenerateAnswerCode(Container.DataItem, " AcceptedSolution")%><%#IFCompare(!ValidationHelper.GetBoolean(GetData(Container.DataItem, "PostApproved"), false), " Unapproved", "")%> ">
                                                        <table class='' cellspacing="0" cellpadding="0" border="0" style="border: medium none;
                                                            width: 100%;">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="border: medium none; padding: 0px 5px;" colspan="2">
                                                                        <%-- OnSite management --%>
                                                                        <%#AdministratorCode(Eval("PostForumID"), "<div class=\"ForumManage\">")%>
                                                                        <%#GetLink(Container.DataItem, ResHelper.GetString("general.approve"), "ActionLink", ForumActionType.Appprove)%>
                                                                        <%#GetLink(Container.DataItem, ResHelper.GetString("forums.approveall"), "ActionLink", ForumActionType.ApproveAll)%>
                                                                        <%#GetLink(Container.DataItem, ResHelper.GetString("general.reject"), "ActionLink", ForumActionType.Reject)%>
                                                                        <%#GetLink(Container.DataItem, ResHelper.GetString("forums.rejectall"), "ActionLink", ForumActionType.RejectAll)%>
                                                                        <%#GetLink(Container.DataItem, ResHelper.GetString("forums.splitthread"), "ActionLink", ForumActionType.SplitThread)%>
                                                                        <%#GetLink(Container.DataItem, ResHelper.GetString("forums.movethread"), "ActionLink", ForumActionType.MoveToTheOtherForum)%>
                                                                        <%#AdministratorCode(Eval("PostForumID"), "</div>")%>
                                                                    </td>
                                                                </tr>
                                                                <tr style="border: medium none;">
                                                                    <td style="border: medium none; padding-right: 5px; vertical-align: top;" class="UserAvatar">
                                                                        <%--Avatar--%>
                                                                        <%#AvatarImage(Container.DataItem)%>
                                                                    </td>
                                                                    <td style="border: medium none; padding: 5px; width: 100%" class="Content">
                                                                        <%#GenerateAnswerCode(Container.DataItem, "<table cellspacing=\"0\" cellpadding=\"0\" class=\"AcceptedSolutionArea\"><tr><td class=\"AcceptedSolutionImage\"><img alt=\"" + ResHelper.GetString("General.AcceptedSolution") + "\" style=\"border: 0px;\" src=\"" + GetImageUrl("Design/Forums/AcceptedSolution.gif") + "\" /></td><td class=\"AcceptedSolutionText\">" + ResHelper.GetString("General.AcceptedSolution") + "</td></tr></table>")%>
                                                                        <div style="float: left;">
                                                                            <span class="PostUserName">
                                                                                <%#GetUserName(Container.DataItem)%>
                                                                            </span><span class="PostSeparator">-</span> <span class="PostTime">
                                                                                <%#TimeZoneHelper.ConvertToUserTimeZone(ValidationHelper.GetDateTime(Eval("PostTime"), DateTimeHelper.ZERO_TIME),false, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite)%>
                                                                            </span>
                                                                        </div>
                                                                        <span style="float: left;">&nbsp;
                                                                            <%#GetLink(Container.DataItem, ResHelper.GetString("Forums_WebInterface_ForumPost.AddPostToFavorites"), "ActionLink", ForumActionType.AddPostToFavorites)%>
                                                                        </span>
                                                                        <%#IFCompare(IsAvailable(Container.DataItem, ForumActionType.IsAnswer), "<div style=\"float: right;\">" + ResHelper.GetString("Forums_WebInterface_ForumPost.Washelpful"), "")%>
                                                                        &nbsp;
                                                                        <%#GetLink(Container.DataItem, ResHelper.GetString("general.yes"), "ActionLink", ForumActionType.IsAnswer)%>
                                                                        <%#IFCompare(IsAvailable(Container.DataItem, ForumActionType.IsAnswer), "|", "")%>
                                                                        <%#GetLink(Container.DataItem, ResHelper.GetString("general.no"), "ActionLink", ForumActionType.IsNotAnswer)%>
                                                                        <%#IFCompare(IsAvailable(Container.DataItem, ForumActionType.IsAnswer), "</div>", "")%>
                                                                        <div style="clear: both">
                                                                        </div>
                                                                        <span class="PostSubject">
                                                                            <%#HTMLHelper.HTMLEncode(Eval("PostSubject").ToString())%>
                                                                        </span><div class="PostText">
                                                                            <cms:ResolvedLiteral runat="server" ID="ltlText" EnableViewState="false" AllowedControls='<%#allowedInlineControls%>' Text='<%#ResolvePostText(Eval("PostText").ToString())%>' />
                                                                        </div>
                                                                        <div style="clear: both">
                                                                            <cms:AttachmentDisplayer ID="attachmentDisplayer" runat="server" PostID='<%#ValidationHelper.GetInteger(Eval("PostID"), -1)%>'
                                                                                PostAttachmentCount='<%#ValidationHelper.GetInteger(Eval("PostAttachmentCount"), -1)%>' />
                                                                        </div>
                                                                        <%#GetSignatureArea(Container.DataItem, "<div class=\"SignatureArea\">", "</div>")%>
                                                                        <br />
                                                                        <div style="float: left;">
                                                                            <%#GetLink(Container.DataItem, ResHelper.GetString("Forums_WebInterface_ForumPost.replyLinkText"), "PostActionLink", ForumActionType.Reply)%>
                                                                            <%#Separator(Container.DataItem, "<span class=\"PostActionSeparator\">|</span>", 0)%>
                                                                            <%#GetLink(Container.DataItem, ResHelper.GetString("Forums_WebInterface_ForumPost.quoteLinkText"), "PostActionLink", ForumActionType.Quote)%>
                                                                            <%#Separator(Container.DataItem, "<span class=\"PostActionSeparator\">|</span>", 1)%>
                                                                            <%#GetLink(Container.DataItem, ResHelper.GetString("Forums_WebInterface_ForumPost.Subscribe"), "PostActionLink", ForumActionType.SubscribeToPost)%>
                                                                        </div>
                                                                        <div style="float: right;">
                                                                            <%#GetLink(Container.DataItem, ResHelper.GetString("general.edit"), "ActionLink", ForumActionType.Edit)%>
                                                                            <%#GetLink(Container.DataItem, ResHelper.GetString("general.delete"), "ActionLink", ForumActionType.Delete)%>
                                                                            <%#GetLink(Container.DataItem, ResHelper.GetString("general.attachments"), "ActionLink", ForumActionType.Attachment)%>
                                                                            <cms:AbuseReport ID="ucAbuse" runat="server" ReportObjectType="forums.forumpost"
                                                                                ReportObjectID='<%#Eval("PostID")%>' CMSPanel-Roles='<%#AbuseReportRoles%>'
                                                                                CMSPanel-SecurityAccess='<%#AbuseReportAccess%>' CMSPanel-CommunityGroupID='<%#CommunityGroupID%>'
                                                                                ReportTitle='<%#ResHelper.GetString("Forums_WebInterface_ForumPost.AbuseReport", SettingsKeyInfoProvider.GetValue("CMSDefaultCultureCode", SiteContext.CurrentSiteName)) + Eval("PostText")%>'
                                                                                CssClass="ActionLink" />
                                                                        </div>
                                                                        <div style="clear: both;">
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                    <%#GenerateIndentCode(Container.DataItem, "</div>")%>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </cms:uniview>
                                    <tr class="Pager">
                                        <td colspan="5">
                                            <%-- PAGER --%>
                                            <cms:unipager id="UniPager1" runat="server" querystringkey="tpage" pagecontrol="listForums"
                                                pagermode="Querystring">
                                                <PreviousGroupTemplate>
                                                    <a href="<%#URLCreator("tpage", ValidationHelper.GetString(Eval("PreviousGroup"), "1"), false, null, true)%>">
                                                        ...</a>
                                                </PreviousGroupTemplate>
                                                <PageNumbersTemplate>
                                                    <a href="<%#URLCreator("tpage", ValidationHelper.GetString(Eval("Page"), "1"), false, null, true)%>">
                                                        <%#Eval("Page")%>
                                                    </a>
                                                </PageNumbersTemplate>
                                                <CurrentPageTemplate>
                                                    <span>
                                                        <%#Eval("Page")%>
                                                    </span>
                                                </CurrentPageTemplate>
                                                <NextGroupTemplate>
                                                    <a href="<%#URLCreator("tpage", ValidationHelper.GetString(Eval("NextGroup"), "1"), false, null, true)%>">
                                                        ...</a>
                                                </NextGroupTemplate>
                                            </cms:unipager>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
