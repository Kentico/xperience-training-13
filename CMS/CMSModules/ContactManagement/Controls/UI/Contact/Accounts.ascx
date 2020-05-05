<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Accounts.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_Accounts" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/AccountSelector.ascx"
    TagName="AccountSelector" TagPrefix="cms" %>

<asp:Panel ID="pnlSelector" runat="server" CssClass="cms-edit-menu">
    <cms:AccountSelector ID="accountSelector" ShortID="as" runat="server" IsLiveSite="false" />
    <asp:HiddenField ID="hdnRoleID" runat="server" ClientIDMode="Static" />
</asp:Panel>
<asp:Panel ID="pnlBody" runat="server" CssClass="PageContent">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="gridElem" ShortID="g" OrderBy="AccountName" ObjectType="om.contactaccountlist"
                Columns="AccountContactID,AccountName,ContactRoleID,AccountStatusID,AccountCountryID,AccountID"
                IsLiveSite="false" RememberStateByParam="issitemanager">
                <GridActions>
                    <ug:Action ExternalSourceName="edit" Name="edit" Caption="$om.account.viewdetail$"
                        FontIconClass="icon-edit" FontIconStyle="Allow" ModuleName="CMS.ContactManagement" CommandArgument="AccountID" />
                    <ug:Action ExternalSourceName="selectrole" Name="selectrole" Caption="$om.contactrole.selectitem$"
                        FontIconClass="icon-app-roles" ModuleName="CMS.ContactManagement" />
                    <ug:Action ExternalSourceName="remove" Name="remove" Caption="$General.Remove$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmRemove$" ModuleName="CMS.ContactManagement" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="AccountName" Caption="$om.account.name$" Wrap="false">
                        <Filter Type="text" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactRoleID" AllowSorting="false" ExternalSourceName="#transform: om.contactrole.contactroledisplayname" Caption="$om.contactrole$" Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/ContactManagement/FormControls/ContactRoleSelector.ascx" Size="100">
                            <CustomFilterParameters>
                                <ug:FilterParameter Name="AllowAllRoles" Value="true" />
                            </CustomFilterParameters>
                        </Filter>
                    </ug:Column>
                    <ug:Column Source="AccountStatusID" AllowSorting="false" ExternalSourceName="#transform: om.accountstatus.accountstatusdisplayname" Caption="$om.accountstatus.name$" Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/ContactManagement/FormControls/AccountStatusSelector.ascx" Size="100"/>
                    </ug:Column>
                    <ug:Column Source="AccountCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname" Caption="$objecttype.cms_country$" Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/ContactManagement/Filters/CountryFilter.ascx" Size="100" >
                            <CustomFilterParameters>
                                <ug:FilterParameter Name="CountryIDColumnName" Value="AccountCountryID" />
                            </CustomFilterParameters>
                        </Filter>
                    </ug:Column>
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" ShowSelection="true" SelectionColumn="AccountContactID" />
            </cms:UniGrid>
            <asp:Panel ID="pnlFooter" runat="server" CssClass="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-value-cell">
                        <cms:LocalizedLabel runat="server" AssociatedControlID="drpWhat" CssClass="sr-only" ResourceString="general.scope" />
                        <cms:CMSDropDownList ID="drpWhat" runat="server" />
                        <cms:LocalizedLabel runat="server" AssociatedControlID="drpAction" CssClass="sr-only" ResourceString="general.action" />
                        <cms:CMSDropDownList ID="drpAction" runat="server" />
                        <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
                            EnableViewState="false" OnClick="btnOk_Click" />
                    </div>
                </div>
                <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
            </asp:Panel>
            <asp:HiddenField ID="hdnValue" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>