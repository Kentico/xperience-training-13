<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="HierarchicalTransformations_New.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_New"
    Theme="Default" %>

<%@ Register Src="~/CMSFormControls/Filters/DocTypeFilter.ascx" TagName="DocTypeFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/DocumentTypes/Controls/Transformation_Edit.ascx" TagName="HierarchicalTransf"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="plcDocTypeFilter" Visible="false">
        <asp:Panel runat="server" ID="pnlDocTypeFilter" CssClass="header-panel">
            <cms:DocTypeFilter runat="server" ID="ucDocFilter" RenderTableTag="true" EnableViewState="true" IsLiveSite="false" />
        </asp:Panel>
    </asp:PlaceHolder>
    <asp:Panel ID="pnlContainer" runat="server">
        <cms:HierarchicalTransf runat="server" ID="ucTransf" />
    </asp:Panel>
</asp:Content>