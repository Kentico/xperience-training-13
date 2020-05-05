<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Layouts_Flat_ThreadEdit"
     Codebehind="ThreadEdit.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/NewPost.ascx" TagName="ForumPostEdit" TagPrefix="cms" %>
<%@ Register Namespace="CMS.Forums" Assembly="CMS.Forums" TagPrefix="cms" %>

<div class="Forum">
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
                        <table cellspacing="0" cellpadding="0" border="0" style="border: 0px none; margin: 0px;
                            padding: 0px; background: transparent none repeat scroll 0% 0%; width: 100%;">
                            <tbody>
                                <tr>
                                    <td style="border: medium none; margin: 0px; padding: 0px; background: transparent none repeat scroll 0% 0%;">
                                        <span class="ForumBreadCrumbs">&nbsp;</span>
                                    </td>
                                    <td style="border: medium none; margin: 0px; padding: 0px; background: transparent none repeat scroll 0% 0%;
                                        text-align: right;">
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="ForumContent">
                        <div class="ForumNewPost">
                            <span class="Title">
                                <asp:Literal runat="server" ID="ltrTitle" />
                            </span>
                            <asp:PlaceHolder runat="server" ID="plcPreview" Visible="false">
                                <div class="PostReply">
                                    <div class="ForumPost">
                                        <table class="PostImage" cellspacing="0" cellpadding="0" border="0" style="border: medium none;
                                            width: 100%;">
                                            <tbody>
                                                <tr style="border: medium none;">
                                                    <td style="border: medium none; padding-right: 5px; vertical-align: top;" class="UserAvatar">
                                                        <asp:Literal runat="server" ID="ltrAvatar" />
                                                        <asp:Literal runat="server" ID="ltrBadge" EnableViewState="false" />
                                                    </td>
                                                    <td style="border: medium none; width: 100%;" class="Content">
                                                        <div>
                                                        </div>
                                                        <a class="PostUser">
                                                            <asp:Literal runat="server" ID="ltrUserName" /></a> <span class="PostSeparator">-</span>
                                                        <span class="PostTime">(<asp:Literal runat="server" ID="ltrTime" />)</span>
                                                        <br />
                                                        <span class="PostSubject">
                                                            <asp:Literal runat="server" ID="ltrSubject" /></span>
                                                        <div class="PostText">
                                                            <cms:ResolvedLiteral runat="server" ID="ltrText" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="plcModerationRequired" Visible="false" EnableViewState="false">
                                <div class="ForumModerationInfo">
                                    <div class="ForumModerationInfoText">
                                        <asp:Label runat="server" ID="lblModerationInfo"></asp:Label>
                                    </div>
                                    <cms:CMSButton runat="server" ButtonStyle="Default" ID="btnOk" OnClick="btnOK_Click" />
                                </div>
                            </asp:PlaceHolder>
                            <cms:ForumPostEdit UseExternalPreview="true" runat="server" ID="forumEdit" />
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
<div style="clear: both; line-height: 0px; height: 0px;">
</div>
