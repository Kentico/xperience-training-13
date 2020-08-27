<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_LogFiles"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="List of log files"
    MaintainScrollPositionOnPostback="true"  Codebehind="System_LogFiles.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridFiles" IsLiveSite="false" GridName="System_LogFiles.xml" DelayedReload="true" />
</asp:Content>
