<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_ContactGroup_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.contactgroup" OrderBy="ContactGroupDisplayName"
            Columns="ContactGroupID,ContactGroupDisplayName" IsLiveSite="false" RememberStateByParam="issitemanager">
            <GridActions Parameters="ContactGroupID">
                <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                <ug:Action Name="delete" ExternalSourceName="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
                    ModuleName="CMS.ContactManagement" Permissions="Modify" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="ContactGroupDisplayName" Caption="$om.contactgroup.displayname$"
                    Wrap="false" Localize="true">
                    <Filter Type="text" Size="200" />
                </ug:Column>
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" />
        </cms:UniGrid>
    </ContentTemplate>
</cms:CMSUpdatePanel>
