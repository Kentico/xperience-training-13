<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_Departments_Department_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="Department_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:UniGrid runat="server" ID="UniGrid" OrderBy="DepartmentDisplayName"
        IsLiveSite="false" Columns="DepartmentID,DepartmentDisplayName,DepartmentSiteID,DepartmentDefaultTaxClassID"
        ObjectType="ecommerce.department">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="DepartmentDisplayName" Caption="$general.name$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="DepartmentDefaultTaxClassID" Caption="$com.department.defaulttax$" ExternalSourceName="#transform: ecommerce.taxclass.taxclassdisplayname" Wrap="false" Localize="true" AllowSorting="false">
            </ug:Column>
            <ug:Column Source="DepartmentID" Name="DepartmentSiteID" Sort="DepartmentSiteID" ExternalSourceName="#transform: ecommerce.department: {% (ToInt(DepartmentSiteID, 0) == 0) ? GetResourceString(&quot;com.globally&quot;) : GetResourceString(&quot;com.onthissiteonly&quot;) %}" Caption="$com.available$"
                Wrap="false">
            </ug:Column>
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
