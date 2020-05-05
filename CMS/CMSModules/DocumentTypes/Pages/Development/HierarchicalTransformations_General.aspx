<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="HierarchicalTransformations_General.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Inherits="CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_General" %>

<%@ Register Src="~/CMSModules/DocumentTypes/Controls/Transformation_Edit.ascx"
    TagName="HierarchicalTransf" TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:HierarchicalTransf runat="server" ID="ucTransf" IsLiveSite="false" />
</asp:Content>
