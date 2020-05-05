<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Users.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact properties" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Users"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.membershipuserlist" Columns="MembershipID,RelatedID,ContactFullNameJoined" IsLiveSite="false" 
                ShowObjectMenu="false" OrderBy="RelatedID">
                <GridActions Parameters="MembershipID">
                    <ug:Action Name="delete" Caption="$General.Delete$" CommandArgument="MembershipID"
                        FontIconClass="icon-bin" FontIconStyle="Critical" ExternalSourceName="delete" Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: cms.user : firstname #htmlencode" Caption="$general.firstname$" Wrap="false" AllowSorting="false" />
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: cms.user : lastname #htmlencode" Caption="$general.lastname$" Wrap="false" AllowSorting="false" />
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: cms.user : email #htmlencode" Caption="$general.email$" Wrap="false" AllowSorting="false" />
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform:cms.user : username #htmlencode" Caption="$general.username$" Wrap="false" AllowSorting="false" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
