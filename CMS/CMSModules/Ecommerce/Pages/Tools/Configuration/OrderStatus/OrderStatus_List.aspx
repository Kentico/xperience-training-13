<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_OrderStatus_OrderStatus_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="E-commerce Configuration - Order status"
     Codebehind="OrderStatus_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" OrderBy="StatusOrder" IsLiveSite="false"
        Columns="StatusID,StatusDisplayName,StatusEnabled,StatusColor,StatusSendNotification,StatusOrderIsPaid"
        ObjectType="ecommerce.orderstatus">
        <GridActions>
            <ug:Action Name="#move" Caption="$General.DragMove$" FontIconClass="icon-dots-vertical"/>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="##ALL##" Caption="$general.name$" Wrap="false" ExternalSourceName="statusName">
                <Filter Type="text" Source="StatusDisplayName" />
            </ug:Column>
            <ug:Column Source="StatusEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                Wrap="false" />
            <ug:Column Source="StatusSendNotification" ExternalSourceName="#yesno" Caption="$Unigrid.OrderStatus.Columns.StatusSendNotification$"
                Wrap="false" />
            <ug:Column Source="StatusOrderIsPaid" ExternalSourceName="#yesno" Caption="$unigrid.orderstatus.columns.statusorderispaid$"
                Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
