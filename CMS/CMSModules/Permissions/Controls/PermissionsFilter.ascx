<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="PermissionsFilter.ascx.cs"
    Inherits="CMSModules_Permissions_Controls_PermissionsFilter" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/SelectModule.ascx" TagName="ModuleSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectClass.ascx" TagName="ClassSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="UserSelector"
    TagPrefix="cms" %>
<asp:Panel ID="PanelOptions" runat="server">
    <div class="form-horizontal form-filter">
        <asp:PlaceHolder ID="plcSite" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" ResourceString="general.site" DisplayColon="true"
                        EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <asp:Panel ID="PanelSites" runat="server">
                        <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                    </asp:Panel>
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPermissionType" runat="server" ResourceString="Administration-Permissions_Header.PermissionType"
                    DisplayColon="false" EnableViewState="false" AssociatedControlID="drpPermissionType" />
            </div>
            <div class="filter-form-condition-cell">
                <cms:CMSUpdatePanel ID="pnlUpdateType" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <cms:CMSDropDownList ID="drpPermissionType" runat="server" OnSelectedIndexChanged="drpPermissionType_SelectedIndexChanged"
                            AutoPostBack="True" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSUpdatePanel ID="pnlUpdateSelectors" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <cms:ModuleSelector runat="server" ID="moduleSelector" IsLiveSite="false" DisplayOnlyWithPermission="true"
                            DisplayOnlyForGivenSite="true" DisplayAllModules="true" FilterMode="true" />
                        <cms:ClassSelector runat="server" ID="docTypeSelector" IsLiveSite="false" OnlyDocumentTypes="true" />
                        <cms:ClassSelector runat="server" ID="customTableSelector" IsLiveSite="false" OnlyCustomTables="true" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
        </div>
        <asp:PlaceHolder ID="plcUser" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUser" runat="server" ResourceString="Administration-Permissions_Header.User"
                        DisplayColon="false" EnableViewState="false" AssociatedControlID="userSelector" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:UserSelector ID="userSelector" runat="server" SelectionMode="SingleDropDownList" />
                    <cms:CMSUpdatePanel ID="pnlUpdateUsers" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:CMSCheckBox ID="chkUserOnly" runat="server" AutoPostBack="true" Enabled="false" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <cms:MessagesPlaceHolder runat="server" ID="plcMess" />
</asp:Panel>
