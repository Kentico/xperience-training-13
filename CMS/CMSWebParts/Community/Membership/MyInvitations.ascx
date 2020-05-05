<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Membership_MyInvitations"  Codebehind="~/CMSWebParts/Community/Membership/MyInvitations.ascx.cs" %>
<asp:Literal runat="server" ID="ltlMessage" EnableViewState="false" />
<asp:Label runat="Server" ID="lblInfo" EnableViewState="false" />
<cms:BasicRepeater ID="rptMyInvitations" runat="server">
    <HeaderTemplate>
        <table class="UniGridGrid" border="1" rules="rows" cellpadding="3" style="border-collapse: collapse;">
            <tr class="UniGridHead">
                <th>
                    <cms:LocalizedLabel ID="lblActions" runat="server" ResourceString="unigrid.actions"
                        EnableViewState="false" />
                </th>
                <th>
                    <cms:LocalizedLabel ID="lblGroupName" runat="server" ResourceString="groupinvitation.group"
                        EnableViewState="false" />
                </th>
                <th>
                    <cms:LocalizedLabel ID="lblInvitedBy" runat="server" ResourceString="groupinvitation.invitedby"
                        EnableViewState="false" />
                </th>
                <th>
                    <cms:LocalizedLabel ID="lblInvitedWhen" runat="server" ResourceString="groupinvitation.invitedwhen"
                        EnableViewState="false" />
                </th>
                <th>
                    <cms:LocalizedLabel ID="lblInvitationComment" runat="server" ResourceString="general.comment"
                        EnableViewState="false" />
                </th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="OddRow">
            <td>
                <asp:ImageButton ID="btnDelete" runat="server" OnCommand="btnDelete_OnCommand" CommandArgument='<%#Eval("InvitationID")%>'
                    CommandName="delete" ImageUrl='<%#mDeleteImageUrl%>' OnClientClick="if (!confirm(deleteMessage)) return false;"
                    ToolTip='<%#mDeleteToolTip%>' AlternateText='<%#mDeleteToolTip%>' EnableViewState="false" />
                <asp:ImageButton ID="btnAccept" runat="server" OnCommand="btnAccept_OnCommand" CommandArgument='<%#Eval("InvitationID")%>'
                    CommandName="accept" ImageUrl='<%#mAcceptImageUrl%>' OnClientClick="if (!confirm(acceptMessage)) return false;"
                    ToolTip='<%#mAcceptToolTip%>' AlternateText='<%#mAcceptToolTip%>' EnableViewState="false" />
            </td>
            <td>
                <asp:Label ID="lblGroupNameValue" runat="server" Text='<%#ResolveText(Eval("GroupDisplayName"))%>'
                    EnableViewState="false" />
            </td>
            <td>
                <asp:Label ID="lblInvitedByValue" runat="server" Text='<%#ResolveText(Functions.GetFormattedUserName(Convert.ToString(Eval("UserName")), true))%>'
                    EnableViewState="false" />
            </td>
            <td>
                <asp:Label ID="lblInvitedWhenValue" runat="server" Text='<%#Eval("InvitationCreated")%>'
                    EnableViewState="false" />
            </td>
            <td>
                <asp:Label ID="lblInvitationCommentValue" runat="server" Text='<%#ResolveText(Eval("InvitationComment"))%>'
                    EnableViewState="false" />
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="EvenRow">
            <td>
                <asp:ImageButton ID="btnDelete" runat="server" OnCommand="btnDelete_OnCommand" CommandArgument='<%#Eval("InvitationID")%>'
                    CommandName="delete" ImageUrl='<%#mDeleteImageUrl%>' OnClientClick="if (!confirm(deleteMessage)) return false;"
                    ToolTip='<%#mDeleteToolTip%>' AlternateText='<%#mDeleteToolTip%>' EnableViewState="false" />
                <asp:ImageButton ID="btnAccept" runat="server" OnCommand="btnAccept_OnCommand" CommandArgument='<%#Eval("InvitationID")%>'
                    CommandName="accept" ImageUrl='<%#mAcceptImageUrl%>' OnClientClick="if (!confirm(acceptMessage)) return false;"
                    ToolTip='<%#mAcceptToolTip%>' AlternateText='<%#mAcceptToolTip%>' EnableViewState="false" />
            </td>
            <td>
                <asp:Label ID="lblGroupNameValue" runat="server" Text='<%#ResolveText(Eval("GroupDisplayName"))%>'
                    EnableViewState="false" />
            </td>
            <td>
                <asp:Label ID="lblInvitedByValue" runat="server" Text='<%#ResolveText(Functions.GetFormattedUserName(Convert.ToString(Eval("UserName")), true))%>'
                    EnableViewState="false" />
            </td>
            <td>
                <asp:Label ID="lblInvitedWhenValue" runat="server" Text='<%#Eval("InvitationCreated")%>'
                    EnableViewState="false" />
            </td>
            <td>
                <asp:Label ID="lblInvitationCommentValue" runat="server" Text='<%#ResolveText(Eval("InvitationComment"))%>'
                    EnableViewState="false" />
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</cms:BasicRepeater>