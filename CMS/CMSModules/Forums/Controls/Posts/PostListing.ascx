<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Posts_PostListing"
     Codebehind="PostListing.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:UniGrid ID="gridPosts" runat="server" GridName="~/CMSModules/Forums/Controls/Posts/PostListing.xml"
    EnableViewState="true" DelayedReload="false" IsLiveSite="false" OrderBy="PostTime" />
<br />
<asp:Panel ID="pnlFooter" runat="server" Style="clear: both;">
    <%--<cms:CMSDropDownList ID="drpWhat" runat="server" CssClass="DropDownFieldSmall" />--%>
    <cms:LocalizedLabel ID="lblSelectedPosts" ResourceString="forums.listing.selectedposts"
        DisplayColon="true" runat="server" EnableViewState="false" />
    <cms:CMSDropDownList ID="drpAction" runat="server" CssClass="DropDownFieldSmall" />
    <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
        EnableViewState="false" />
    <br />
    <br />
</asp:Panel>