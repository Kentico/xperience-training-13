<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartSelector" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" Title="Select web part"  Codebehind="WebPartSelector.aspx.cs" %>

<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/WebPartSelector.ascx"
    TagName="WebPartSelector" TagPrefix="cms" %>

<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="content">
    <asp:Panel runat="server" ID="pnlSelector">
        <cms:WebPartSelector runat="server" ID="selectElem" IsLiveSite="false" />
        <cms:LocalizedHidden ID="hdnMessage" runat="server" Value="{$PortalEngine-WebPartSelection.NoWebPartSelected$}"
            EnableViewState="false" />
    </asp:Panel>
</asp:Content>