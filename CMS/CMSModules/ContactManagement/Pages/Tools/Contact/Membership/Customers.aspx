<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Customers.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact properties" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Customers"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.membershipcustomerlist"
        Columns="MembershipID,RelatedID,ContactFullNameJoined" IsLiveSite="false" ShowObjectMenu="false" ShowActionsMenu="true" OrderBy="RelatedID">
        <GridActions Parameters="MembershipID">
            <ug:Action Name="delete" CommandArgument="MembershipID" Caption="$General.Delete$"
                FontIconClass="icon-bin" FontIconStyle="Critical" ExternalSourceName="delete" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="RelatedID" ExternalSourceName="#transform: ecommerce.customer.customerfirstname"
                Caption="$general.firstname$" Wrap="false" AllowSorting="false" />
            <ug:Column Source="RelatedID" ExternalSourceName="#transform: ecommerce.customer.customerlastname"
                Caption="$general.lastname$" Wrap="false" AllowSorting="false" />
            <ug:Column Source="RelatedID" ExternalSourceName="#transform: ecommerce.customer.customercompany" Caption="$Unigrid.Customers.Columns.CustomerCompanyName$"
                Wrap="false" AllowSorting="false" />
            <ug:Column Source="RelatedID" ExternalSourceName="#transform: ecommerce.customer.customeremail" Caption="$general.email$"
                Wrap="false" AllowSorting="false" />
            <ug:Column Source="ContactFullNameJoined" Caption="$om.contact.name$" Wrap="false"
                Name="contactname" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
    </cms:UniGrid>
</asp:Content>
