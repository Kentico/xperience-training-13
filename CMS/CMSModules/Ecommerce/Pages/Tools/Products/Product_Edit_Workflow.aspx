<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Product_Edit_Workflow.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Workflow" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/Workflow.ascx" TagName="Workflow"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" ShowApprove="true" ShowReject="true" ShowSubmitToApproval="true"
        ShowProperties="false" IsLiveSite="false" ShowSave="false" />
    <cms:CMSDocumentPanel ID="pnlDocInfo" runat="server" Visible="False" />
</asp:Content>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUp" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlContainer" runat="server">
                <cms:Workflow runat="server" ID="workflowElem" IsLiveSite="false" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
