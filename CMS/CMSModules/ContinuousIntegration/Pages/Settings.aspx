<%@ Page Language="C#" AutoEventWireup="false"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_ContinuousIntegration_Pages_Settings" Theme="Default"  CodeBehind="Settings.aspx.cs" %>
<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:BasicForm runat="server" ID="form"></cms:BasicForm>
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" />
    </asp:Panel>
</asp:Content>
