<%@ Page Language="C#" AutoEventWireup="true"
 MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Media library - General"
 Inherits="CMSModules_Groups_Tools_MediaLibrary_Library_Edit_General" Theme="Default"  Codebehind="Library_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/UI/MediaLibraryEdit.ascx" TagName="LibraryEdit"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent" >
    <cms:LibraryEdit ID="elemEdit" runat="server" IsLiveSite="false" />
</asp:Content>