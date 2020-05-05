<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="Media library - Files" Inherits="CMSModules_Groups_Tools_MediaLibrary_Library_Edit_Files"
    Theme="Default" EnableEventValidation="false"  Codebehind="Library_Edit_Files.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaLibrary.ascx"
    TagName="MediaLibrary" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MediaLibrary ID="libraryElem" runat="server" DisplayFilesCount="true" DisplayMode="Simple"
        IsLiveSite="false" />
    <script type="text/javascript">
            //<![CDATA[
        function Refresh() {
            window.location.replace(window.location);
        }
            //]]>
    </script>
</asp:Content>
