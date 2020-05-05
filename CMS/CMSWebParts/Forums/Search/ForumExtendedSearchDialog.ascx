<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Forums_Search_ForumExtendedSearchDialog"  Codebehind="~/CMSWebParts/Forums/Search/ForumExtendedSearchDialog.ascx.cs" %>
<asp:Panel ID="pnlForumExtendedSearch" runat="server" DefaultButton="btnSearch">
    <cms:LocalizedLabel ID="lblInfo" EnableViewState="false" runat="server" Visible="false" />
    <table class="ForumExtendedSearch">
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSearchText" ResourceString="ForumExtSearch.Text" DisplayColon="true"
                    EnableViewState="false" AssociatedControlID="txtSearchText" runat="server" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtSearchText"  runat="server" EnableViewState="false" />
            </td>
            <td>
            <asp:Image runat="server" ID="imgTextHint" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSearchUserName" ResourceString="ForumExtSearch.User" DisplayColon="true"
                    EnableViewState="false" AssociatedControlID="txtUserName" runat="server" />
            </td>
            <td colspan="2">
                <cms:CMSTextBox ID="txtUserName"  runat="server" EnableViewState="false" />
            </td>
        </tr>
        <asp:PlaceHolder runat="server" ID="plcForums" Visible="false">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblForums" ResourceString="ForumExtSearch.Forums" DisplayColon="true"
                        EnableViewState="false" AssociatedControlID="listForums" runat="server" />
                </td>
                <td colspan="2">
                    <cms:CMSListBox Rows="8" CssClass="DropDownList" runat="server" ID="listForums" SelectionMode="Multiple" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSearchIn" ResourceString="ForumExtSearch.SearchIn" DisplayColon="true"
                    EnableViewState="false" AssociatedControlID="drpSearchIn" runat="server" />
            </td>
            <td colspan="2">
                <cms:CMSDropDownList ID="drpSearchIn" CssClass="DropDownList" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSearchOrderBy" ResourceString="ForumExtSearch.OrderBy"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="drpSearchOrderBy" runat="server" />
            </td>
            <td>
                <cms:CMSDropDownList ID="drpSearchOrderBy" CssClass="DropDownList" runat="server" />
            </td>
            <td>
                <cms:CMSRadioButtonList ID="rblSearchOrder" runat="server" RepeatDirection="Horizontal" />
            </td>
        </tr>
        <tr>
            <td></td>
            <td colspan="2">
                <cms:CMSButton ID="btnSearch" ButtonStyle="Default" runat="server" EnableViewState="false"
                    OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
