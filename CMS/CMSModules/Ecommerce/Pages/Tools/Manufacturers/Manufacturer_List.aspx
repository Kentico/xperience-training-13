<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Manufacturers_Manufacturer_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Manufacturers"
     Codebehind="Manufacturer_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="UniGrid" IsLiveSite="false" Columns="ManufacturerID, ManufacturerDisplayName, ManufacturerEnabled, ManufacturerSiteID"
                OrderBy="ManufacturerDisplayName" ObjectType="ecommerce.manufacturer">
                <GridActions>
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="#delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="ManufacturerDisplayName" Caption="$general.name$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="ManufacturerEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                        Wrap="false" />
                    <ug:Column Source="ManufacturerID" Sort="ManufacturerSiteID" ExternalSourceName="#transform: ecommerce.manufacturer: {% (ToInt(ManufacturerSiteID, 0) == 0) ? GetResourceString(&quot;com.globally&quot;) : GetResourceString(&quot;com.onthissiteonly&quot;) %}" Caption="$com.available$"
                        Name="ManufacturerSiteID" Wrap="false" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
