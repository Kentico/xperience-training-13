<%@ Page Title="Relationship - Select page" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true"  Codebehind="RelatedDocuments.aspx.cs" Inherits="CMSFormControls_Selectors_RelatedDocuments"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/Relationships/AddRelatedDocument.ascx"
    TagName="AddRelatedDocument" TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
    <cms:AddRelatedDocument ID="addRelatedDocument" runat="server" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
