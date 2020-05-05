<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Integration_Pages_Administration_View"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Staging - Task detail"  Codebehind="View.aspx.cs" %>

<%@ Register Src="~/CMSModules/Objects/Controls/ViewObjectDataSet.ascx" TagName="ViewDataSet"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ViewDataSet ID="viewDataSet" runat="server" />
</asp:Content>
