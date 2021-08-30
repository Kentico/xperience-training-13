<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="List.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact list" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_List" EnableEventValidation="false"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/List.ascx" TagName="ContactList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagName="SmartTip" TagPrefix="cms" %>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="control-group-inline header-actions-container">
                <cms:HeaderActions ID="hdrActions" runat="server" IsLiveSite="false" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:SmartTip runat="server" ID="tipHowCMWorks" Visible="true" />
    <cms:ContactList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
