<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Edit.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" Inherits="CMSModules_Reporting_Tools_Subscription_Edit" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/SubscriptionEdit.ascx" TagName="SubscriptionEdit"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:SubscriptionEdit runat="server" ID="editElem" />
</asp:Content>
