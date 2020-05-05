<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Layouts_Tree_Forums"  Codebehind="Forums.ascx.cs" %>
<%@ Register Namespace="CMS.Forums" Assembly="CMS.Forums" TagPrefix="cms" %>
<div class="Forum">
    <div class="ForumGroup">
        <table class="Table" cellspacing="0" cellpadding="0">
            <tr class="Header">
                <td class="ForumName" colspan="2">
                    <span>
                        <%=ResHelper.GetString("ForumGroup.lblForum")%>
                    </span>
                </td>
                <td class="Threads">
                    <span>
                        <%=ResHelper.GetString("ForumGroup.lblThreads")%>
                    </span>
                </td>
                <td class="Posts">
                    <span>
                        <%=ResHelper.GetString("ForumGroup.lblPosts")%>
                    </span>
                </td>
                <td class="LastPost">
                    <span>
                        <%=ResHelper.GetString("ForumGroup.lblLastPost")%>
                    </span>
                </td>
            </tr>
            <%-- UniView Control Begin--%>
            <cms:UniView runat="server" ID="listForums" EnableViewState="false">
                <%-- Header template--%>
                <HeaderTemplate>
                    <tr class="Info">
                        <td colspan="5">
                            <span class="GroupName">
                                <%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(DataBinder.Eval(ForumContext.CurrentGroup, "GroupDisplayName").ToString()))%>
                            </span><span class="GroupDescription">
                                <%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(DataBinder.Eval(ForumContext.CurrentGroup, "GroupDescription").ToString()))%>
                            </span>
                        </td>
                    </tr>
                </HeaderTemplate>
                <%-- Item template--%>
                <ItemTemplate>
                    <tr class="Forum">
                        <td class='<%#"ForumImageDefault " + GetImageClass(Container.DataItem)%>'>&nbsp;
                        </td>
                        <td class="ForumInfo">
                            
                            <div>
                                <span class="ForumName">
                                    <%#GetLink(Container.DataItem, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Eval("ForumDisplayName").ToString())), "ActionLink", ForumActionType.Forum)%>
                                </span>
                                <br />
                                <span class="ForumDescription">
                                    <%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Eval("ForumDescription").ToString()))%>
                                </span>
                            </div>
                            <%#AdministratorCode(Eval("ForumID"), "<div class=\"ForumManage\">", true)%>
                            <%#GetLink(Container.DataItem, ResHelper.GetString("forums.lock"), "ActionLink", ForumActionType.LockForum)%>
                            <%#GetLink(Container.DataItem, ResHelper.GetString("forums.unlock"), "ActionLink", ForumActionType.UnlockForum)%>
                            <%#AdministratorCode(Eval("ForumID"), "</div>", true)%>
                        </td>
                        <td class="Threads">
                            <span>
                                <%#ThreadCount(Container.DataItem)%>
                            </span>
                        </td>
                        <td class="Posts">
                            <span>
                                <%#PostCount(Container.DataItem, false)%>
                            </span>
                        </td>
                        <td class="LastPost">
                            <div class="PostUser">
                                <%#LastUser(Container.DataItem, false)%>
                            </div>
                            <div class="PostTime">
                                (<%#LastTime(Container.DataItem, false)%>)
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <%-- Footer template--%>
            </cms:UniView>
            <%-- UniView Control End--%>
        </table>
    </div>
</div>