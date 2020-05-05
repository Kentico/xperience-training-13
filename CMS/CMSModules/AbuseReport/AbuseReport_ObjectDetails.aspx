<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_AbuseReport_AbuseReport_ObjectDetails" Title="Untitled Page"
    ValidateRequest="false" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"  Codebehind="AbuseReport_ObjectDetails.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/ObjectDataViewer.ascx" TagName="ObjectDataViewer"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:ObjectDataViewer ID="ObjectDataViewer" runat="server" />
</asp:Content>
