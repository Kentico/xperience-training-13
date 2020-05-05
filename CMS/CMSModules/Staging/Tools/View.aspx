<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_View"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Staging - Task detail"  Codebehind="View.aspx.cs" %>

<%@ Register Src="~/CMSModules/Staging/Tools/Controls/ViewTask.ascx" TagName="ViewTask"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ViewTask ID="ucViewTask" runat="server" />
</asp:Content>
