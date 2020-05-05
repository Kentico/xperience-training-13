<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Layouts_Flat_Attachments"
     Codebehind="Attachments.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/AttachmentList.ascx" TagName="AttachmentList"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.Forums" Assembly="CMS.Forums" TagPrefix="cms" %>

<div class="Forum">
    <div class="ForumFlat">
        <asp:PlaceHolder runat="server" ID="plcModerationRequired" Visible="false" EnableViewState="false">
            <div class="ForumModerationInfo">
                <div class="ForumModerationInfoText">
                    <asp:Label runat="server" ID="lblModerationInfo"></asp:Label>
                </div>
            </div>
        </asp:PlaceHolder>
        <table class="Table" cellspacing="0" cellpadding="0">
            <tbody>
                <tr class="Info">
                    <td>
                        <span class="ForumName">
                            <%=HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ForumContext.CurrentForum.ForumDisplayName))%>
                        </span><span class="ForumDescription">
                            <%=HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ForumContext.CurrentForum.ForumDescription))%>
                        </span>
                    </td>
                </tr>
                <tr class="Actions">
                    <td>
                        <table cellspacing="0" cellpadding="0" border="0" style="width: 100%;">
                            <tbody>
                                <tr>
                                    <td style="border: medium none; margin: 0px; padding: 0px;">
                                        <span class="ForumBreadCrumbs">&nbsp; </span>
                                    </td>
                                    <td style="border: medium none; margin: 0px; padding: 0px; text-align: right;">
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="ForumAttachments">
                        <div class="Title">
                            <%=ResHelper.GetString("general.attachments")%>
                        </div>
                        <cms:AttachmentList ID="attachmentList" runat="server" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
<div style="clear: both; line-height: 0px; height: 0px;">
</div>
