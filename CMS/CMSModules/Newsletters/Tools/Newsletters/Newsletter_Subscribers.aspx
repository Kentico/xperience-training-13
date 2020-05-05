<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Subscribers"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Newsletters - Subscribers"
    EnableEventValidation="false" CodeBehind="Newsletter_Subscribers.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<asp:Content ID="contentControls" ContentPlaceHolderID="plcActions" runat="server">
    <div class="control-group-inline header-actions-container">
        <cms:UniSelector ID="contactGroupsSelector" runat="server" IsLiveSite="false" DisplayNameFormat="{%ContactGroupDisplayName%}"
            ObjectType="om.contactgroup" ResourcePrefix="contactgroupsubscriber" ReturnColumnName="ContactGroupID" SelectionMode="MultipleButton" />
         <cms:UniSelector ID="contactsSelector" runat="server" IsLiveSite="false" DisplayNameFormat="{%ContactFirstName%} {%ContactLastName%} ({%ContactEmail%})"
            ObjectType="om.contact" ResourcePrefix="contactsubscriber" ReturnColumnName="ContactID" SelectionMode="MultipleButton" DialogWindowHeight="640"/>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <h4>
                <asp:Literal runat="server" ID="ltlTotalRecipientsCount" />
            </h4>
            <cms:UniGrid ID="UniGridSubscribers" runat="server" ShortID="g" OrderBy="SubscriberFullName"
                IsLiveSite="false" ShowObjectMenu="false" ObjectType="newsletter.subscribernewsletterlist" Columns="SubscriberID, NewsletterID, SubscriberFullName, SubscriberEmail, SubscriptionApproved, SubscriberType, SubscriberRelatedID, SubscriberNewsletterID">
                <GridActions>
                    <ug:Action Name="remove" Caption="$newsletter.deletesubscription$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$Unigrid.Subscribers.Actions.RemoveSubscription.Confirmation$" />
                    <ug:Action Name="approve" ExternalSourceName="approve" Caption="$newsletter.approvesubscription$"
                        FontIconClass="icon-check-circle" FontIconStyle="Allow" Confirmation="$subscribers.approvesubscription$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="SubscriberFullName" Caption="$Unigrid.Subscribers.Columns.SubscriberName$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="SubscriberEmail" Caption="$general.emailaddress$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="status" Caption="$emailmarketing.ui.newslettersubscription$"
                        CssClass="TableCell" Wrap="false">
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="ismarketable" Caption="$emailmarketing.ui.marketablebyemail$"
                        Wrap="false">
                    </ug:Column>
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" ShowSelection="true" />
            </cms:UniGrid>
            <asp:Panel ID="pnlActions" runat="server" CssClass="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-label-cell">
                        <cms:LocalizedLabel ID="lblActions" AssociatedControlID="drpActions" ResourceString="general.selecteditems"
                            CssClass="control-label" DisplayColon="true" runat="server" EnableViewState="false" />
                    </div>
                    <div class="mass-action-value-cell">
                        <cms:CMSDropDownList ID="drpActions" runat="server" />
                        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOk_Clicked"
                            ResourceString="general.ok" EnableViewState="false" />
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
