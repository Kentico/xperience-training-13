<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MyDesk_WaitingForApproval_WaitingForApproval"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="MyDesk - Pages waiting for my approval"
     Codebehind="WaitingForApproval.aspx.cs" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="WaitingForApproval"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:WaitingForApproval runat="server" ID="ucWaitingForApproval" ListingType="PendingDocuments"
        IsLiveSite="false" />
</asp:Content>
