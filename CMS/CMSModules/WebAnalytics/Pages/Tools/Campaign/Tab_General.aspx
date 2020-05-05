<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="Campaign properties - General" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_General"
    Theme="Default" CodeBehind="Tab_General.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="cpAfterForm" ng-strict-di>
<asp:Panel runat="server" CssClass="PageContent">
    <cms-campaign></cms-campaign>
</asp:Panel>
</asp:Content>
