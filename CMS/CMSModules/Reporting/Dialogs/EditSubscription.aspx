<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="EditSubscription.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" Inherits="CMSModules_Reporting_Dialogs_EditSubscription" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/SubscriptionEdit.ascx" TagName="SubscriptionEdit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:SubscriptionEdit runat="server" ID="subEdit" SimpleMode="true" />
</asp:Content>
