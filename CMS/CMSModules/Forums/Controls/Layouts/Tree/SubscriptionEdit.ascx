<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Layouts_Tree_SubscriptionEdit"
     Codebehind="SubscriptionEdit.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/SubscriptionForm.ascx" TagName="SubscriptionEdit"
    TagPrefix="cms" %>
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
                        <table cellspacing="0" cellpadding="0" border="0" style="width: 100%;">
                            <tbody>
                                <tr>
                                    <td style="border: medium none; margin: 0px; padding: 0px;">
                                        <span class="ForumBreadCrumbs"></span>
                                    </td>
                                    <td style="border: medium none; margin: 0px; padding: 0px; text-align: right;">
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
                                                    <td style="border: medium none; padding-right: 5px; vertical-align: top;">
                                                        <asp:Literal runat="server" ID="ltrAvatar" />
                                                    </td>
                                                    <td style="border: medium none; width: 100%;">
                                                        <div>
                                                        </div>
                                                        <a class="PostUser">
                                                            <asp:Literal runat="server" ID="ltrUserName" /></a> <span class="PostSeparator">-</span>
                                                        <span class="PostTime">(<asp:Literal runat="server" ID="ltrTime" />)</span>
                                                        <br />
                                                        <span class="PostSubject">
                                                            <asp:Literal runat="server" ID="ltrSubject" /></span>
                                                        <div class="PostText">
                                                            <asp:Literal runat="server" ID="ltrText" /></div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <cms:SubscriptionEdit runat="server" ID="subscriptionEdit" />
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
<div style="clear: both; line-height: 0px; height: 0px;">
</div>
