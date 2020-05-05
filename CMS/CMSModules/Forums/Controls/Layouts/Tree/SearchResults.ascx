<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_Layouts_Tree_SearchResults"  Codebehind="SearchResults.ascx.cs" %>
<%@ Import Namespace="CMS.Globalization.Web.UI" %>
<%@ Register Namespace="CMS.Forums" Assembly="CMS.Forums" TagPrefix="cms" %>
<div class="ForumFlat">
    <div class="Table">
        <asp:Literal runat="server" ID="ltlResultsInfo" EnableViewState="false"></asp:Literal>
        <asp:PlaceHolder runat="server" ID="plcResults">
            <table class="PostsTable ForumSearchResults" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;">
                <cms:UniView ID="uniForumPosts" runat="server" CssClass="PostsTable" EnableViewState="false">
                    <ItemTemplate>
                        <tr class="Post">
                            <td>
                                <div class="Result">
                                    <div class="ResultThread">
                                        <a href="<%#ForumFunctions.GetPostURL(Eval("PostIDPath"), Eval("PostForumID"))%>">
                                            <%=ResHelper.GetString("ForumSearchResults.ViewThread")%>
                                        </a>
                                        <div class="ForumPost">
                                            <table cellpadding="0" cellspacing="0" border="0" class="PostImage" style="width: 100%;
                                                border: none;">
                                                <tr style="border: none;" class="UserAvatar">
                                                    <td style="padding-right: 5px; border: none; vertical-align: top;">
                                                        <%#AvatarImage(Container.DataItem)%>
                                                        <%#GetNotEmpty(Eval("BadgeDisplayName"), "<div class=\"Badge\">" + Encode(Eval("BadgeDisplayName")) + "</div>", "<div class=\"Badge\">" + ResHelper.GetString("Forums.PublicBadge") + "</div>", ForumActionType.Badge)%>
                                                        <%#GetNotEmpty(Eval("BadgeImageURL"), "<div class=\"BadgeImage\"><img alt=\"" + Encode(Eval("BadgeDisplayName")) + "\" src=\"" + ResolveUrl(Encode(Eval("BadgeImageURL"))) + "\" /></div>", "", ForumActionType.Badge)%>
                                                    </td>
                                                    <td style="width: 100%; border: none;" class="Content">
                                                        <asp:PlaceHolder runat="server" ID="plcPost">
                                                            <div>
                                                                <asp:Label Visible="false" runat="server" ID="lblInfo" EnableViewState="false" />
                                                            </div>
                                                            <span class="PostUser">
                                                                <%#GetUserName(Container.DataItem)%>
                                                            </span><span class="PostSeparator">-</span> <span class="PostTime">
                                                                <%# TimeZoneUIMethods.ConvertDateTime(ValidationHelper.GetDateTime(Eval("PostTime"), DateTimeHelper.ZERO_TIME), this)%>
                                                            </span>
                                                            <br />
                                                            <span class="PostSubject">
                                                                <%#HTMLHelper.HTMLEncode(Eval("PostSubject").ToString())%>
                                                            </span><div class="PostText">
                                                                <%#ResolvePostText(Eval("PostText").ToString(), Container.DataItem)%>
                                                            </div></asp:PlaceHolder>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </cms:UniView>
                <tr class="Pager">
                    <td>
                        <cms:UniPager ID="UniPager1" runat="server" PageControl="uniForumPosts">
                            <PageNumbersTemplate>
                                <a href="<%#EvalHtmlAttribute("PageURL")%>">
                                    <%#Eval("Page")%>
                                </a>
                            </PageNumbersTemplate>
                            <CurrentPageTemplate>
                                <span>
                                    <%#Eval("Page")%>
                                </span>
                            </CurrentPageTemplate>
                            <NextGroupTemplate>
                                <a href="<%#EvalHtmlAttribute("NextGroupURL")%>">...</a>
                            </NextGroupTemplate>
                            <PreviousGroupTemplate>
                                <a href="<%#EvalHtmlAttribute("PreviousGroupURL")%>">...</a>
                            </PreviousGroupTemplate>
                        </cms:UniPager>
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <cms:LocalizedLabel ID="lblNoResults" runat="server" EnableViewState="false" Visible="false" />
    </div>
</div>
