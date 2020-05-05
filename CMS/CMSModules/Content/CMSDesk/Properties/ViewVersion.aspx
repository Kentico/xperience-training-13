<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_ViewVersion"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Properties - View Version"  Codebehind="ViewVersion.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/ViewVersion.ascx" TagName="ViewVersion"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ViewVersion ID="viewVersion" runat="server" IsLiveSite="false" />
</asp:Content>