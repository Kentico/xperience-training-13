<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Users.aspx.cs" Inherits="CMSModules_Membership_Pages_Membership_Tab_Users"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel runat="server" ID="pnlBasic" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:LocalizedHeading ID="headTitle" Level="4" runat="server" CssClass="listing-title" DisplayColon="true"
                ResourceString="Membership.assignedusers" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <div style="display: none">
        <cms:DateTimePicker runat="server" ID="ucCalendar" />
    </div>
    <cms:UniSelector ID="usUsers" runat="server" IsLiveSite="false" ListingObjectType="cms.membershiplist"
        ObjectType="cms.user" SelectionMode="Multiple" ResourcePrefix="addusers" DisplayNameFormat="##USERDISPLAYFORMAT##" />
    <asp:HiddenField runat="server" ID="hdnDate" />
    <asp:HiddenField runat="server" ID="hdnSendNotification" />
</asp:Content>