<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Reports.aspx.cs" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Reports"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Campaign reports" EnableViewState="false" Theme="Default" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="cpAfterForm" ng-strict-di>
    <div data-ng-controller="app as $ctrl" class="PageContent">
        <cms-campaign-report campaign-id="$ctrl.campaignId" />
    </div>
</asp:Content>
