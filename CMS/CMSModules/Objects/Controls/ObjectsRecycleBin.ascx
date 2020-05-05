<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Objects_Controls_ObjectsRecycleBin"
     Codebehind="ObjectsRecycleBin.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Panel runat="server" ID="pnlLog" Visible="false">
    <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="RecycleBin" />
</asp:Panel>
<cms:MessagesPlaceholder ID="plcMess" runat="server" />
<div>
    <cms:CMSUpdatePanel ID="pnlGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid ID="ugRecycleBin" runat="server" HideControlForZeroRows="true" ShowObjectMenu="false"
                ObjectType="cms.objectversionhistory" Columns="VersionID, VersionObjectType, VersionObjectID, VersionDeletedWhen, VersionObjectDisplayName, VersionObjectSiteID">
                <GridActions Parameters="VersionID">
                    <ug:Action Name="view" Caption="$general.view$" ExternalSourceName="view" FontIconClass="icon-eye" FontIconStyle="Allow" />
                    <ug:Action Name="restorechilds" CommandArgument="VersionID" Caption="$General.Restore$"
                        FontIconClass="icon-arrow-u-left" Confirmation="$objectversioning.recyclebin.confirmrestore$" />
                    <ug:Action Name="destroy" CommandArgument="VersionID"
                        Caption="$recyclebin.destroyhint$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
                    <ug:Action Name="contextmenu" ExternalSourceName="contextmenu" Caption="$General.OtherActions$"
                        FontIconClass="icon-ellipsis" ContextMenu="~/CMSModules/Objects/Controls/RecycleBinMenu.ascx"
                        MenuParameter="'{0}'" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="VersionObjectDisplayName" ExternalSourceName="objectname" Caption="$general.objectname$"
                        MaxLength="100" Wrap="false" CssClass="main-column-100" Sort="VersionObjectDisplayName">
                        <Tooltip Source="VersionObjectDisplayName" ExternalSourceName="objectname" />
                    </ug:Column>
                    <ug:Column Source="VersionObjectType" ExternalSourceName="versionobjecttype" Caption="$general.objecttype$"
                        Wrap="false" MaxLength="50" Sort="VersionObjectType">
                    </ug:Column>
                    <ug:Column Source="VersionObjectSiteID" Caption="$general.sitename$" ExternalSourceName="#sitenameorglobal" Wrap="false" />
                    <ug:Column Source="VersionDeletedWhen" ExternalSourceName="deletedwhen" Caption="$Unigrid.RecycleBin.Columns.LastModified$"
                        Wrap="false" Sort="VersionDeletedWhen">
                        <Tooltip Source="VersionDeletedWhen" ExternalSourceName="deletedwhentooltip" />
                    </ug:Column>
                    <ug:Column Source="VersionID" Visible="false" />
                </GridColumns>
                <GridOptions ShowSelection="true" SelectionColumn="VersionID" DisplayFilter="true" FilterPath="~/CMSModules/Content/Controls/Filters/ObjectsRecycleBinFilter.ascx" FilterLimit="0" />
            </cms:UniGrid>
            <asp:Panel ID="pnlFooter" runat="server" CssClass="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-value-cell">
                        <cms:CMSDropDownList ID="drpWhat" runat="server" />
                        <cms:CMSDropDownList ID="drpAction" runat="server" />
                        <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
                            EnableViewState="false" OnClick="btnOk_OnClick" />
                    </div>
                    <asp:Label ID="lblValidation" runat="server" CssClass="InfoLabel" EnableViewState="false" />
                </div>
            </asp:Panel>
            <asp:HiddenField ID="hdnValue" runat="server" EnableViewState="false" />
            <asp:Button ID="btnHidden" runat="server" CssClass="HiddenButton" EnableViewState="false"
                OnClick="btnHidden_Click" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</div>