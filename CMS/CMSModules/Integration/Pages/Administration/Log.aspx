<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Log.aspx.cs" Inherits="CMSModules_Integration_Pages_Administration_Log"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Integration - Synchronization log" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcControls">
    <asp:Label runat="server" ID="lblInfo" EnableViewState="false" />
    <cms:LocalizedButton runat="server" ID="btnClear" ButtonStyle="Default" OnClick="btnClear_Click"
        EnableViewState="false" ResourceString="Task.LogClear" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid ID="gridLog" runat="server" OrderBy="SyncLogTime DESC" IsLiveSite="false"
        ObjectType="integration.synclog">
        <GridActions>
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="SyncLogTime" Caption="$SyncLog.Columns.Time$" Wrap="false" />
            <ug:Column Source="SyncLogErrorMessage" Caption="$SyncLog.Columns.Error$" IsText="true"
                CssClass="main-column-100" />
        </GridColumns>
    </cms:UniGrid>
</asp:Content>
