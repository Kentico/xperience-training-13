<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_InternalStatus_InternalStatus_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Internal status - List"
     Codebehind="InternalStatus_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" OrderBy="InternalStatusDisplayName" IsLiveSite="false"
        Columns="InternalStatusID, InternalStatusDisplayName, InternalStatusEnabled"
        ObjectType="ecommerce.internalstatus">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="InternalStatusDisplayName" Caption="$Unigrid.InternalStatus.Columns.InternalStatusDisplayName$"
                Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="InternalStatusEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
