<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_Filters_MediaLibrarySort"  Codebehind="MediaLibrarySort.ascx.cs" %>
<div class="MediaLibrarySort">
    <cms:LocalizedLabel ID="lblSortBy" runat="server" EnableViewState="false" ResourceString="media.library.sort" />
    <asp:LinkButton ID="lnkName" runat="server" EnableViewState="false" OnClick="lnkName_Click" />
    <asp:LinkButton ID="lnkDate" runat="server" EnableViewState="false" OnClick="lnkDate_Click" />
    <asp:LinkButton ID="lnkSize" runat="server" EnableViewState="false" OnClick="lnkSize_Click" />
</div>
