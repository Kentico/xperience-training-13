<%@ Control Language="C#" AutoEventWireup="True" Inherits="CMSModules_ContactManagement_Controls_UI_Account_Contacts"
    CodeBehind="Contacts.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx"
    TagName="ContactSelector" TagPrefix="cms" %>

<asp:Panel ID="pnlSelector" runat="server" CssClass="cms-edit-menu">
    <cms:ContactSelector ID="contactSelector" runat="server" IsLiveSite="false" />
    <asp:HiddenField ID="hdnRoleID" runat="server" />
    <div class="ClearBoth">
        &nbsp;
    </div>
</asp:Panel>
<asp:Panel ID="pnlBody" runat="server" CssClass="PageContent">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="gridElem" OrderBy="ContactLastName" ObjectType="om.accountcontactlist"
                Columns="AccountContactID,ContactFirstName,ContactLastName,ContactEmail,ContactRoleID,ContactStatusID,ContactCountryID,ContactID"
                IsLiveSite="false">
                <GridActions>
                    <ug:Action ExternalSourceName="edit" Name="edit" Caption="$om.contact.viewdetail$"
                        FontIconClass="icon-eye" FontIconStyle="Allow" ModuleName="CMS.ContactManagement" CommandArgument="ContactID" />
                    <ug:Action ExternalSourceName="selectrole" Name="selectrole" Caption="$om.contactrole.selectitem$"
                        FontIconClass="icon-app-roles" ModuleName="CMS.ContactManagement" />
                    <ug:Action ExternalSourceName="remove" Name="remove" Caption="$General.Remove$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmRemove$" ModuleName="CMS.ContactManagement" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="ContactFirstName" Caption="$om.contact.firstname$" Wrap="false">
                        <Filter Type="text" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactLastName" Caption="$om.contact.lastname$" Wrap="false">
                        <Filter Type="text" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false">
                        <Filter Type="text" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactRoleID" AllowSorting="false" ExternalSourceName="#transform: om.contactrole.contactroledisplayname"
                        Caption="$om.contactrole$" Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/ContactManagement/FormControls/ContactRoleSelector.ascx" Size="100">
                            <CustomFilterParameters>
                                <ug:FilterParameter Name="AllowAllRoles" Value="true" />
                            </CustomFilterParameters>
                        </Filter>
                    </ug:Column>
                    <ug:Column Source="ContactStatusID" AllowSorting="false" ExternalSourceName="#transform: om.contactstatus.contactstatusdisplayname"
                        Caption="$om.contactstatus$" Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/ContactManagement/FormControls/ContactStatusSelector.ascx" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname"
                        Caption="$objecttype.cms_country$" Wrap="false">
                        <Filter Type="text" Format="ContactCountryID IN (SELECT CountryID FROM CMS_Country WHERE {3})"
                            Source="CountryDisplayName" Size="100" />
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