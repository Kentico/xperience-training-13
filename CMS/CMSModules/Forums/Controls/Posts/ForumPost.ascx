<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Posts_ForumPost"  Codebehind="ForumPost.ascx.cs" %>
<div class="ForumPost">
    <table cellpadding="0" cellspacing="0" border="0" class="PostImage" style="width: 100%;
        border: none;">
        <asp:PlaceHolder runat="server" ID="plcEdit">
            <tr>
                <td colspan="2" style="border:none">
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr style="border: none;">
            <asp:PlaceHolder runat="server" ID="plcImage">
                <td style="padding-right: 5px; border: none; vertical-align: top;">
                    <asp:Literal runat="server" ID="userAvatar" EnableViewState="false" />
                    <asp:Literal runat="server" ID="ltlBadge" EnableViewState="false"  />
                </td>
            </asp:PlaceHolder>
            <td style="width: 100%; border: none;">
                <asp:PlaceHolder runat="server" ID="plcPost">
                    <em><asp:HyperLink ID="lnkUserName" CssClass="PostUser" runat="server" /></em>
                    <asp:Label ID="lblSeparator" CssClass="PostSeparator" runat="server" />
                    <asp:Label ID="lblDate" CssClass="PostTime" runat="server" /><br />
                    <em><asp:Label ID="lblSubject" runat="server" /></em>
                    <cms:ResolvedLiteral ID="ltlText" runat="server" />
                    <asp:PlaceHolder runat="server" ID="plcActions">
                        <asp:HyperLink ID="lnkReply" CssClass="PostActionLink" runat="server" />
                        <asp:Label Visible="false" ID="lblActionSeparator" Text="|" CssClass="PostActionSeparator" runat="server" />
                        <asp:HyperLink ID="lnkQuote" CssClass="PostActionLink" runat="server" />
                        <asp:Label Visible="false" ID="lblActionSeparator2" Text="|" CssClass="PostActionSeparator" runat="server" />
                        <asp:HyperLink ID="lnkSubscribe" CssClass="PostActionLink" runat="server" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="plcSignature" Visible="false">
                        <div class="SignatureArea">
                            <asp:Literal runat="server" ID="ltrSignature" EnableViewState="false" />
                        </div>
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
            </td>
        </tr>
    </table>
</div>
