<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_PaymentOptions_PaymentOption_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="PaymentOption_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="UniGrid" OrderBy="PaymentOptionDisplayName" IsLiveSite="false"
                Columns="PaymentOptionID, PaymentOptionDisplayName, PaymentOptionAllowIfNoShipping, PaymentOptionEnabled, PaymentOptionSiteID"
                ObjectType="ecommerce.paymentoption">
                <GridActions>
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="PaymentOptionDisplayName" Caption="$general.name$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="PaymentOptionAllowIfNoShipping" ExternalSourceName="#yesno" Caption="$com.paymentoption_list.allowedifnoshipping$"
                        Wrap="false" />
                    <ug:Column Source="PaymentOptionEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                        Wrap="false" />
                       <ug:Column Source="PaymentOptionID" Sort="PaymentOptionSiteID" Name="PaymentOptionSiteID" ExternalSourceName="#transform: ecommerce.paymentoption: {% (ToInt(PaymentOptionSiteID, 0) == 0) ? GetResourceString(&quot;com.globally&quot;) : GetResourceString(&quot;com.onthissiteonly&quot;) %}" Caption="$com.available$" Wrap="false" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
