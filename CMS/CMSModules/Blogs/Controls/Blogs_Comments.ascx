<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/Blogs/Controls/Blogs_Comments.ascx.cs"
    Inherits="CMSModules_Blogs_Controls_Blogs_Comments" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Blogs/Controls/CommentFilter.ascx" TagName="CommentFilter"
    TagPrefix="cms" %>
<cms:CommentFilter runat="server" ID="filterElem" />
<cms:UniGrid ID="gridComments" runat="server" GridName="~/CMSModules/Blogs/Tools/Comments_List.xml"
    OrderBy="CommentDate DESC" IsLiveSite="false" ExportFileName="cms_blogcomment" />
<br />
<asp:Panel ID="pnlActions" runat="server" CssClass="control-group-inline">
    <cms:LocalizedLabel ID="lblAction" runat="server" EnableViewState="false" DisplayColon="true" CssClass="form-control-text"
        ResourceString="blog.comments.action" />
    <cms:CMSDropDownList ID="drpAction" runat="server" CssClass="DropDownFieldSmall" />
    <cms:CMSButton ID="btnAction" runat="server" ButtonStyle="Default" OnClick="btnAction_Click"
        EnableViewState="false" />
</asp:Panel>
