<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ObjectVersionList.ascx.cs"
    Inherits="CMSModules_Objects_Controls_Versioning_ObjectVersionList" %>

<%@ Register TagPrefix="cms" TagName="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:ObjectLockingPanel runat="server" ID="pnlObjectLocking">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:LocalizedHeading ID="headTitle" Level="4" runat="server" CssClass="listing-title" ResourceString="objectversioning.objecthistory"
        EnableViewState="false" />

    <div class="TextRight content-block-50">
        <asp:Button ID="btnRefresh" runat="server" CssClass="HiddenButton" OnClick="btnRefresh_Click" />
        <cms:LocalizedButton ID="btnMakeMajor" runat="server" ButtonStyle="Default" OnClick="btnMakeMajor_Click"
            ResourceString="objectversioning.makemajor" />
        <cms:LocalizedButton ID="btnDestroy" runat="server" ButtonStyle="Default" OnClick="btnDestroy_Click"
            ResourceString="objectversioning.destroyhistory" />
    </div>

    <cms:UniGrid ID="gridHistory" runat="server" ObjectType="cms.objectversionhistorylist"
        ShowObjectMenu="false" OrderBy="VersionID DESC">
        <GridActions Parameters="VersionID">
            <ug:Action Name="view" ExternalSourceName="view" Caption="$Unigrid.VersionHistory.Actions.View$"
                FontIconClass="icon-eye" FontIconStyle="Allow" />
            <ug:Action Name="rollback" Caption="$Unigrid.VersionHistory.Actions.Rollback$" ExternalSourceName="rollback"
                FontIconClass="icon-arrow-u-left" Confirmation="$Unigrid.ObjectVersionHistory.Actions.Rollback.Confirmation$" />
            <ug:Action Name="destroy" ExternalSourceName="allowdestroy" Caption="$General.Delete$"
                FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
            <ug:Action Name="contextmenu" ExternalSourceName="contextmenu" Caption="$General.OtherActions$"
                FontIconClass="icon-ellipsis" ContextMenu="~/CMSModules/Objects/Controls/Versioning/VersionListMenu.ascx"
                MenuParameter="'{0}'" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="##ALL##" ExternalSourceName="ModifiedBy" Caption="$Unigrid.VersionHistory.Columns.ModifiedByUserFullName$"
                Wrap="false" CssClass="main-column-100" />
            <ug:Column Source="VersionModifiedWhen" ExternalSourceName="#userdatetimegmt" Caption="$general.modified$"
                Wrap="false">
                <Tooltip Source="##ALL##" ExternalSourceName="#usertimezonename"></Tooltip>
            </ug:Column>
            <ug:Column Source="VersionNumber" Caption="$Unigrid.VersionHistory.Columns.VersionNumberLong$"
                Wrap="false" />
        </GridColumns>
        <PagerConfig DefaultPageSize="10" />
    </cms:UniGrid>
    <asp:HiddenField ID="hdnValue" runat="server" EnableViewState="false" />
    <asp:Button ID="btnHidden" runat="server" CssClass="HiddenButton" EnableViewState="false"
        OnClick="btnHidden_Click" />
</cms:ObjectLockingPanel>