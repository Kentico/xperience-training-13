<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Blogs_PostArchive"  Codebehind="~/CMSWebParts/Blogs/PostArchive.ascx.cs" %>
<cms:CMSRepeater ID="rptPostArchive" runat="server" OrderBy="BlogMonthStartingDate DESC"
    ClassNames="cms.blogmonth" OnItemDataBound="rptPostArchive_ItemDataBound" />
