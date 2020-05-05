<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Details.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Theme="Default"
    Title="Contact details" Inherits="CMSModules_ContactManagement_Pages_Contact_Details" EnableViewState="false" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="cpAfterForm" ng-strict-di>
    <div data-ng-controller="app as $ctrl">
        <cms-contact-profile contact-id="$ctrl.contactId" />
    </div>
</asp:Content>