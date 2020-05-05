<%@ Page Language="C#" AutoEventWireup="true"
  MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Media library - New library"
  Inherits="CMSModules_MediaLibrary_Tools_Library_New" Theme="Default"  Codebehind="Library_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/UI/MediaLibraryEdit.ascx" TagName="LibraryEdit"
    TagPrefix="cms" %>
      
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LibraryEdit ID="elemEdit" runat="server" IsLiveSite="false" />
</asp:Content>


