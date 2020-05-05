<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_Currencies_Currency_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Currency - list"
     Codebehind="Currency_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" OrderBy="CurrencyDisplayName" IsLiveSite="false"
        Columns="CurrencyID,CurrencyDisplayName,CurrencyCode,CurrencyEnabled,CurrencyIsMain"
        ObjectType="ecommerce.currency">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="CurrencyDisplayName" Caption="$Unigrid.Currency.Columns.CurrencyDisplayName$"
                Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="CurrencyCode" Caption="$Unigrid.Currency.Columns.CurrencyCode$"
                Wrap="false" />
            <ug:Column Source="CurrencyIsMain" ExternalSourceName="#yes" Caption="$Unigrid.Currency.Columns.CurrencyIsMain$"
                Wrap="false" />
            <ug:Column Source="CurrencyEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
