<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Forums_ForumFavorites"  Codebehind="~/CMSWebParts/Forums/ForumFavorites.ascx.cs" %>
<asp:Literal runat="server" ID="ltlMessage" EnableViewState="false" />
<table class="ForumFavorites">
    <asp:PlaceHolder ID="plcEmptyDSTagBegin" runat="server" Visible="false">
        <tr>
            <td>
    </asp:PlaceHolder>
    <cms:BasicRepeater ID="rptFavorites" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                    <a href="<%#GetFavoriteLink(Eval("PostID"), Eval("ForumID"), Eval("PostForumID"), Eval("PostIdPath"))%>">
                        <%# HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(Eval("FavoriteName"))))%>
                    </a>
                </td>
                <asp:PlaceHolder runat="server" ID="plcEdit" Visible='<%#AllowEditing%>'>
                    <td>
                        <asp:ImageButton EnableViewState="false" Visible='<%#AllowEditing%>' ID="btnDelete"
                            runat="server" OnCommand="btnDelete_OnCommand" CommandArgument='<%#Eval("FavoriteID")%>'
                            CommandName="delete" AlternateText='<%#ResHelper.GetString("general.delete")%>' ImageUrl='<%#mDeleteImageUrl%>' OnClientClick='<%#"if (!confirm(favDelMessage)) { return false; } else { ForumFavoritesSetValue(" + Eval("FavoriteID") + ") }"%>' />
                    </td>
                </asp:PlaceHolder>
            </tr>
        </ItemTemplate>
    </cms:BasicRepeater>
    <asp:PlaceHolder ID="plcEmptyDSTagEnd" runat="server" Visible="false"></td> </tr> </asp:PlaceHolder>
</table>
<asp:HiddenField runat="server" ID="hdnValue" />
