<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_MyBlogs_MyBlogs_Blogs_List" Title="Blogs - List" Theme="Default"
     Codebehind="MyBlogs_Blogs_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="contentElem" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:UniGrid ID="gridBlogs" runat="server" GridName="~/CMSModules/Blogs/Tools/Blog_List.xml"
        IsLiveSite="false" ExportFileName="cms_blog" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
