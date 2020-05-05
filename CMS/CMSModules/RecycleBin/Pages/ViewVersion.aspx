<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_RecycleBin_Pages_ViewVersion"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Page details"  Codebehind="ViewVersion.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/ViewVersion.ascx" TagName="ViewVersion"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ViewVersion ID="viewVersion" runat="server" />
</asp:Content>