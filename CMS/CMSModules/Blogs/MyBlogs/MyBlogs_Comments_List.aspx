<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Blogs_MyBlogs_MyBlogs_Comments_List"
    Title="Comments - List" Theme="Default"  Codebehind="MyBlogs_Comments_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Blogs/Controls/Blogs_Comments.ascx" TagName="BlogComments"
    TagPrefix="cms" %>
    
<asp:Content ID="contentElem" ContentPlaceHolderID="plcContent" runat="Server">
     <cms:BlogComments runat="server" ID="ucBlogComments" IsInMydesk="true" />
</asp:Content>
