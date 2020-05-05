<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_AllowedTypes"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Type Edit - Child Types"
     Codebehind="DocumentType_Edit_AllowedTypes.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" ID="headParents" EnableViewState="false" Level="4" CssClass="listing-title" ResourceString="DocumentType.AllowedParents" />
        <cms:UniSelector ID="selParent" runat="server" IsLiveSite="false" ObjectType="cms.documenttype"
            SelectionMode="Multiple" />
    </div>
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" ID="headChildren" EnableViewState="false" Level="4" CssClass="listing-title" ResourceString="DocumentType.AllowedChildren" />
        <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="cms.documenttype"
            SelectionMode="Multiple" />
    </div>
</asp:Content>
