<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Type Edit - General"
     Codebehind="DocumentType_Edit_General.aspx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms"
    TagName="SmartTip" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:SmartTip ID="tipUrlPattern" runat="server" EnableViewState="false" ExpandedHeader="{$documenttype.urlpattern.smarttip.header$}" CollapsedHeader="{$documenttype.urlpattern.smarttip.header$}" />
    <cms:UIForm ID="editElem" runat="server" ObjectType="cms.documenttype" AlternativeFormName="documenttype" MarkRequiredFields="false" RefreshHeader="True" />
</asp:Content>
