<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Layouts_Flat_Threads"
     Codebehind="Threads.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/ForumBreadcrumbs.ascx" TagName="ForumBreadcrumbs" TagPrefix="cms" %>
<%@ Register Namespace="CMS.Forums" Assembly="CMS.Forums" TagPrefix="cms" %>
<div class="Forum">
    <div class="ForumFlat">
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
                                        <cms:ForumBreadcrumbs ID="ForumBreadcrumbs1" runat="server" />
                                    </span>
                                </td>
                                <td style="border: medium none; margin: 0px; padding: 0px;">
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="plcContent">
                <tr>
                    <td class="ForumContent">
                        <table class="ThreadTable" cellspacing="0" cellpadding="0" border="0" style="width: 100%;
                            border-collapse: collapse;">
                            <tbody>
                                <tr class="Header">
                                    <td>
                                    </td>
                                    <td class="ThreadName">
                                        <%=ResHelper.GetString("ForumThreadList.ThreadName")%>
                                    </td>
                                    <td class="Author">
                                        <%=ResHelper.GetString("ForumThreadList.CreatedBy")%>
                                    </td>
                                    <td class="Posts">
                                        <%=ResHelper.GetString("ForumThreadList.Posts")%>
                                    </td>
                                    <td class="Views">
                                        <%=ResHelper.GetString("ForumThreadList.Views")%>
                                    </td>
                                    <td class="LastPost">
                                        <%=ResHelper.GetString("ForumThreadList.LastUser")%>
                                    </td>
                                </tr>
                                <cms:UniView runat="server" ID="listForums" EnableViewState="false">
                                    <ItemTemplate>
                                        <tr class='<%#IFCompare(!ValidationHelper.GetBoolean(GetData(Container.DataItem, "PostApproved"), false), "Thread Unapproved", "Thread")%>'>
                                            <td class='<%#"ThreadImageDefault " + GetImageClass(Container.DataItem)%>'>
                                            </td>
                                            <td class="ThreadInfo">
                                                <%#GetLink(Container.DataItem, Eval("PostSubject"), "ThreadName", ForumActionType.Thread)%>
                                                <%#AdministratorCode(ForumContext.CurrentForum.ForumID, "<div class=\"ForumManage\">")%>
                                                <%#GetLink(Container.DataItem, ResHelper.GetString("forums.lock"), "ActionLink", ForumActionType.LockThread)%>
                                                <%#GetLink(Container.DataItem, ResHelper.GetString("forums.unlock"), "ActionLink", ForumActionType.UnlockThread)%>
                                                <%#GetLink(Container.DataItem, ResHelper.GetString("ForumPost_View.IconStick"), "ActionLink", ForumActionType.StickThread)%>
                                                <%#GetLink(Container.DataItem, ResHelper.GetString("ForumPost_View.IconUnStick"), "ActionLink", ForumActionType.UnstickThread)%>
                                                <%#GetLink(Container.DataItem, ResHelper.GetString("general.movedown"), "ActionLink", ForumActionType.MoveStickyThreadDown)%>
                                                <%#GetLink(Container.DataItem, ResHelper.GetString("general.moveup"), "ActionLink", ForumActionType.MoveStickyThreadUp)%>
                                                <%#UnaprovedPosts(Container.DataItem, "<br />" + ResHelper.GetString("Forums.UnapprovedPosts"), true)%>
                                                <%#AdministratorCode(ForumContext.CurrentForum.ForumID, "</div>")%>
                                            </td>
                                            <td class="Author">
                                                <%#HTMLHelper.HTMLEncode(Eval("PostUserName").ToString())%>
                                                <br />
                                            </td>
                                            <td class="Posts">
                                                <%#PostCount(Container.DataItem, true)%>
                                            </td>
                                            <td class="Views">
                                                <%#ValidationHelper.GetInteger(Eval("PostViews"), 0)%>
                                            </td>
                                            <td class="LastPost">
                                                <span>
                                                    <%#LastUser(Container.DataItem, true)%>
                                                </span>
                                                <br />
                                                <span>(<%#LastTime(Container.DataItem, true)%>) </span>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </cms:UniView>
                                <tr class="Pager">
                                    <td colspan="6">
                                        <%-- PAGER --%>
                                        <cms:UniPager ID="UniPager1" runat="server" PageSize="10" QueryStringKey="fpage"
                                            PageControl="listForums">
                                            <PreviousGroupTemplate>
                                                <a href="<%#URLCreator("fpage", ValidationHelper.GetString(Eval("PreviousGroup"), "1"), false, null, true)%>">
                                                    ...</a>
                                            </PreviousGroupTemplate>
                                            <PageNumbersTemplate>
                                                <a href="<%#URLCreator("fpage", ValidationHelper.GetString(Eval("Page"), "1"), false, null, true)%>">
                                                    <%#Eval("Page")%>
                                                </a>
                                            </PageNumbersTemplate>
                                            <CurrentPageTemplate>
                                                <span>
                                                    <%#Eval("Page")%>
                                                </span>
                                            </CurrentPageTemplate>
                                            <NextGroupTemplate>
                                                <a href="<%#URLCreator("fpage", ValidationHelper.GetString(Eval("NextGroup"), "1"), false, null, true)%>">
                                                    ...</a>
                                            </NextGroupTemplate>
                                        </cms:UniPager>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </div>
</div>
