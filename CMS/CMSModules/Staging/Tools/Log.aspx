<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_Log"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Staging - Synchronization log"  CodeBehind="Log.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid ID="gridLog" runat="server" GridName="SyncLog.xml" OrderBy="SynchronizationLastRun DESC"
        IsLiveSite="false" ShowObjectMenu="false" />
</asp:Content>
