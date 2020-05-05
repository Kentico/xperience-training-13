<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ExchangeRates_ExchangeTable_List"
    Theme="default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Exchangle table - List"
     Codebehind="ExchangeTable_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" OrderBy="ExchangeTableDisplayName"
        IsLiveSite="false" Columns="ExchangeTableID,ExchangeTableDisplayName,ExchangeTableValidFrom,ExchangeTableValidTo"
        ObjectType="ecommerce.exchangetable">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="ExchangeTableDisplayName" Caption="$general.name$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="ExchangeTableValidFrom" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.ExchangeTable.Columns.ExchangeTableValidFrom$"
                Wrap="false" />
            <ug:Column Source="ExchangeTableValidTo" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.ExchangeTable.Columns.ExchangeTableValidTo$"
                Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
