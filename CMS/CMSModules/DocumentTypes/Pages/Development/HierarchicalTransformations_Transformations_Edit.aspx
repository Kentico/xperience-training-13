<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="HierarchicalTransformations_Transformations_Edit.aspx.cs"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_Transformations_Edit" %>

<%@ Register Src="~/CMSModules/DocumentTypes/Controls/HierarchicalTransformations_Edit.ascx"
    TagName="HierarchicalTransfNew" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContainer" runat="server">
        <cms:HierarchicalTransfNew runat="server" ID="ucTransf" />
    </asp:Panel>
</asp:Content>