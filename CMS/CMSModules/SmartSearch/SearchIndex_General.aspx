<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_SearchIndex_General"
    Title="Search Index - General" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default"  Codebehind="SearchIndex_General.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_General.ascx" TagName="SearchIndexEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/IndexInfo.ascx" TagName="IndexInfo" TagPrefix="cms" %>
<%@ Register Assembly="CMS.Search.Web.UI" Namespace="CMS.Search.Web.UI" TagPrefix="search" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:SearchIndexEdit ID="ucSearchIndexEdit" runat="server" IsLiveSite="false" />
    <cms:LocalizedHeading runat="server" ID="headElem" ResourceString="srch.index.info" Level="4" CssClass="editing-form-category-caption" />
    <search:InfoMessage runat="server" ID="infoMessage" Visible="false" />
    <cms:CMSUpdatePanel runat="server" ID="pnlIndexInfo" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Timer ID="timIndexInfoRefresh" runat="server" Interval="3000" EnableViewState="false" />
            <cms:IndexInfo ID="ucIndexInfo" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
