<%@ Page Title="Relationship - Select page" Language="C#" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true"  Codebehind="RelatedDocuments.aspx.cs" Inherits="CMSFormControls_LiveSelectors_RelatedDocuments"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/Relationships/AddRelatedDocument.ascx"
    TagName="AddRelatedDocument" TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <div class="PageContent">
        <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
        <cms:AddRelatedDocument ID="addRelatedDocument" runat="server" IsLiveSite="true" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnSave" runat="server" ButtonStyle="Primary" ResourceString="general.save"
            EnableViewState="false" />
        <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Primary" ResourceString="general.close"
            EnableViewState="false" />
    </div>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
