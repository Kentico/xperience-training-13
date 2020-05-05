<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_View_Listing"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - Listing"
     Codebehind="Listing.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/DocumentList.ascx" TagName="DocumentList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectClass.ascx" TagPrefix="cms" TagName="SelectClass" %>

<asp:Content ID="cntSiteSelector" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblDocType" EnableViewState="false" DisplayColon="true" ResourceString="General.DocumentType" CssClass="control-label" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SelectClass runat="server" ID="SelectClass" OnlyDocumentTypes="true" DisplayAllValue="true"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="upList" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:DocumentList runat="server" ID="docList" ExtenderScopePrefix="DocumentList" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
