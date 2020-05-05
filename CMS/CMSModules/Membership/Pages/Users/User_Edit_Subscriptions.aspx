<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Membership_Pages_Users_User_Edit_Subscriptions" Theme="Default"
     Codebehind="User_Edit_Subscriptions.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/Controls/Subscriptions.ascx" TagName="Subscriptions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntSiteSelect" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSelectSite" runat="server" ResourceString="general.site"
                    CssClass="control-label" DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowEmpty="false" AllowAll="False" ShortID="sl" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel ID="updateContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:Subscriptions ID="elemSubscriptions" runat="server" IsLiveSite="false" ShortID="sb" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>