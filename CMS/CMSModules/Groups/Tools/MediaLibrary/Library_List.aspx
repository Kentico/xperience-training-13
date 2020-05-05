<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Library list" Inherits="CMSModules_Groups_Tools_MediaLibrary_Library_List"
    Theme="Default"  Codebehind="Library_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/UI/MediaLibraryList.ascx" TagName="LibraryList"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LibraryList ID="elemList" runat="server" IsLiveSite="false" />
</asp:Content>
