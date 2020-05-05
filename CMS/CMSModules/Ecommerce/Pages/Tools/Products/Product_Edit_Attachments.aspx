<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Product_Edit_Attachments.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Attachments"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DocumentAttachments/DocumentAttachmentsList.ascx"
    TagName="Attachments" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" ShowApprove="true" ShowReject="true" ShowSubmitToApproval="true"
        ShowProperties="false" ShowSave="false" IsLiveSite="false" ShowApplyWorkflow="false" />
    <cms:CMSDocumentPanel ID="pnlDocInfo" runat="server" Visible="False" />
</asp:Content>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
    <div class="Unsorted">
        <cms:Attachments ID="ucAttachments" runat="server" IsLiveSite="false" InnerDivClass="NewAttachment"
            InnerLoadingDivClass="NewAttachmentLoading" />
        <asp:Button ID="btnRefresh" runat="server" EnableViewState="true" CssClass="HiddenButton"
            OnClick="btnRefresh_Click" UseSubmitBehavior="false" />
    </div>
    <div class="Clear">
    </div>
</asp:Content>
