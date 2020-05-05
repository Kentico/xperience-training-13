<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_UIPersonalization_Controls_UIPersonalization"
     Codebehind="UIPersonalization.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/selectrole.ascx" TagName="RoleSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/SelectModule.ascx" TagName="ModuleSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIElementCheckBoxTree.ascx" TagName="UITree"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:PlaceHolder runat="server" ID="plcContent">
    <cms:CMSPanel ID="pnlHeader" runat="server">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Always">
            <ContentTemplate>
                <asp:Panel ID="pnlActions" runat="server" CssClass="header-panel">
                    <div class="form-horizontal form-filter">
                        <asp:PlaceHolder runat="server" ID="plcSite">
                            <div class="form-group">
                                <div class="filter-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSite" EnableViewState="false" ResourceString="general.site"
                                        DisplayColon="true" />
                                </div>
                                <div class="filter-form-value-cell-wide">
                                    <cms:SiteSelector runat="server" ID="selectSite" AllowAll="false" AllowEmpty="false" OnlyRunningSites="false" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="plcRole">
                            <div class="form-group">
                                <div class="filter-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblRole" EnableViewState="false" ResourceString="objecttype.cms_role"
                                        DisplayColon="true" />
                                </div>
                                <div class="filter-form-value-cell-wide">
                                    <cms:RoleSelector runat="server" ID="selectRole" ShowSiteFilter="false" AddGlobalObjectSuffix="false"
                                        UseCodeNameForSelection="false" AllowEmpty="false" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="plcModule">
                            <div class="form-group">
                                <div class="filter-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblModule" EnableViewState="false" ResourceString="objecttype.cms_resource"
                                        DisplayColon="true" />
                                </div>
                                <div class="filter-form-value-cell-wide">
                                    <cms:ModuleSelector runat="server" ID="selectModule" DisplayOnlyWithPermission="false" AllowAll="true" FilterMode="true" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlAdditionalControls" runat="server" EnableViewState="false" CssClass="cms-edit-menu">
                    <cms:HeaderActions ID="actionsElem" runat="server" IsLiveSite="false" />
                </asp:Panel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </cms:CMSPanel>
    <div class="PageContent">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdateTree" UpdateMode="Always">
            <ContentTemplate>
                <asp:Panel ID="pnlTree" runat="server">
                    <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSPersonalizeUserInterface" InfoText="{$uiprofile.disabled$}" />
                    <div class="ContentTree">
                        <div class="TreeAreaTree">
                            <cms:UITree runat="server" ID="treeElem" />
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</asp:PlaceHolder>

