<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="User_Edit_Membership.aspx.cs"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Membership_Pages_Users_User_Edit_Membership" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntSiteSelect" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <asp:PlaceHolder ID="plcSites" runat="server">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblSelectSite" runat="server" CssClass="control-label" AssociatedControlID="siteSelector"
                        ResourceString="general.site" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <cms:CMSUpdatePanel ID="pnlBasic" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" DisplayColon="true" ResourceString="edit_user.membershipInfo" CssClass="listing-title" EnableViewState="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdnDate" />
                <div style="display: none">
                    <cms:DateTimePicker runat="server" ID="ucCalendar" />
                </div>
                <cms:UniSelector runat="server" ID="usMemberships" IsLiveSite="false" ObjectType="cms.membership"
                    ListingObjectType="cms.membershiplist" SelectionMode="Multiple" ResourcePrefix="membershipselector" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:PlaceHolder>
</asp:Content>