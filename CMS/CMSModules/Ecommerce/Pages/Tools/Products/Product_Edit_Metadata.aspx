<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Product_Edit_Metadata.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Metadata" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" ShowApprove="true" ShowReject="true" ShowSubmitToApproval="true"
        ShowProperties="false" IsLiveSite="false" ShowApplyWorkflow="false" />
</asp:Content>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
            <ContentTemplate>
                <cms:CMSForm runat="server" ID="editForm" AlternativeFormFullName="cms.document.productmetadata" DefaultFieldLayout="Inline" UseColonBehindLabel="False" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
