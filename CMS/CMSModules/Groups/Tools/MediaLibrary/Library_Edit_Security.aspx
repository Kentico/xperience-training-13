<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Media library - Security"
    Inherits="CMSModules_Groups_Tools_MediaLibrary_Library_Edit_Security" Theme="Default"  Codebehind="Library_Edit_Security.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/UI/MediaLibrarySecurity.ascx" TagName="MediaLibrarySecurity"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MediaLibrarySecurity ID="librarySecurity" runat="server" />
</asp:Content>
