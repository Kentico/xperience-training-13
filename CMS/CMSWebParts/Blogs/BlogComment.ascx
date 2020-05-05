<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Blogs/BlogComment.ascx.cs"
    Inherits="CMSWebParts_Blogs_BlogComment" %>
<%@ Register Src="~/CMSModules/Blogs/Controls/Blogs_Comments.ascx" TagName="BlogComments"
    TagPrefix="cms" %>
<cms:BlogComments runat="server" ID="ucComments" />
