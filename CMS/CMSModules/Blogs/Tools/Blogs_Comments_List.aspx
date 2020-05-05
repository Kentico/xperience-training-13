<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_Tools_Blogs_Comments_List" Title="Blogs - Comments list"
    Theme="Default"  Codebehind="Blogs_Comments_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Blogs/Controls/Blogs_Comments.ascx" TagName="BlogComments"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:BlogComments runat="server" ID="ucBlogComments" />
</asp:Content>
