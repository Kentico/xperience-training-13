<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_TagGroups_Pages_Development_Tags_Documents" Title="Tag group - Tags pages"
    Theme="Default"  Codebehind="Tags_Documents.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="Documents"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Filters/DocumentFilter.ascx" TagName="DocumentFilter"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="Server">
    <asp:PlaceHolder ID="plcFilter" runat="server">
        <cms:DocumentFilter ID="filterDocuments" runat="server" LoadSites="true" />
    </asp:PlaceHolder>
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="TagGroup_Edit.Documents.used" CssClass="listing-title" EnableViewState="false" DisplayColon="true"/>    
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <cms:Documents ID="docElem" runat="server" ListingType="TagDocuments" IsLiveSite="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
