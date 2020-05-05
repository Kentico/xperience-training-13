<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Suppliers_Supplier_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Supplier - list"
     Codebehind="Supplier_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="gridElem" OrderBy="SupplierDisplayName"
                IsLiveSite="false" Columns="SupplierID, SupplierDisplayName, SupplierEnabled, SupplierSiteID"
                ObjectType="ecommerce.supplier">
                <GridActions>
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="SupplierDisplayName" Caption="$Unigrid.supplier.Columns.SupplierDisplayName$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="SupplierEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$" Wrap="false" />
                    <ug:Column Source="SupplierID" Name="SupplierSiteID" Sort="SupplierSiteID" ExternalSourceName="#transform: ecommerce.supplier: {% (ToInt(SupplierSiteID, 0) == 0) ? GetResourceString(&quot;com.globally&quot;) : GetResourceString(&quot;com.onthissiteonly&quot;) %}" Caption="$com.available$" Wrap="false" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
