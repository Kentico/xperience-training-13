<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Credit_List"
    Theme="Default" Title="Customer credit events"  Codebehind="Customer_Edit_Credit_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="conten" runat="server">
    <cms:LocalizedHeading runat="server" ID="headTotalCredit" Level="4" EnableViewState="false" />
    <cms:UniGrid runat="server" ID="UniGrid" IsLiveSite="false" Columns="EventID,EventDate,EventName,EventCreditChange,EventDescription"
        ObjectType="ecommerce.creditevent">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="EventDate" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.CreditEvent.Columns.EventCreated$"
                Wrap="false" />
            <ug:Column Source="EventName" Caption="$Unigrid.CreditEvent.Columns.EventName$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="EventCreditChange" ExternalSourceName="eventcreditchange" Caption="$Unigrid.CreditEvent.Columns.EventCreditChange$"
                Wrap="false" />
            <ug:Column Source="EventDescription" Caption="$Unigrid.CreditEvent.Columns.EventDescription$" Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
