<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_CustomTables_CustomTable_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Custom Tables List"
     Codebehind="CustomTable_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="uniGrid" OrderBy="ClassDisplayName" IsLiveSite="false"
        Columns="ClassID,ClassDisplayName,ClassName,ClassTableName" ObjectType="cms.customtable">
        <GridActions>
            <ug:Action Name="edit" CommandArgument="ClassID" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" CommandArgument="ClassID" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$general.confirmdelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="ClassDisplayName" Caption="$general.displayname$" Wrap="false"
                Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="ClassName" Caption="$general.codename$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="ClassTableName" Caption="$sysdev.classes.tablename$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="ClassID" Caption="$general.sitename$" Visible="false">
                <Filter Type="site" />
            </ug:Column>
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
