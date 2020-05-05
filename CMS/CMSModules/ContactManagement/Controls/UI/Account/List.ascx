<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Account_List" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/Filter.ascx" TagPrefix="cms" TagName="AccountFilter" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridElem" OrderBy="AccountName" ObjectType="om.accountlist"
            Columns="AccountID,AccountName,PrimaryContactFullName,AccountStatusID,AccountCountryID,AccountPrimaryContactID,AccountCreated"
            IsLiveSite="false" HideFilterButton="true" RememberDefaultState="true" RememberStateByParam="issitemanager" FilterLimit="0">
            <GridActions Parameters="AccountID">
                <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                <ug:Action ExternalSourceName="delete" Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="AccountName" Caption="$om.account.name$" Wrap="false">
                </ug:Column>
                <ug:Column Source="AccountStatusID" AllowSorting="false" ExternalSourceName="#transform: om.accountstatus.accountstatusdisplayname"
                    Caption="$om.accountstatus.name$" Wrap="false">
                </ug:Column>
                <ug:Column Source="##ALL##" Caption="$om.contact.primary$" Sort="PrimaryContactFullName"
                    Wrap="false" ExternalSourceName="primarycontactname" CssClass="unigrid-actions">
                </ug:Column>
                <ug:Column Source="AccountCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname"
                    Caption="$objecttype.cms_country$" Wrap="false">
                </ug:Column>
                <ug:Column Source="AccountCreated" Caption="$general.created$" Wrap="false">
                </ug:Column>
                <ug:Column Source="AccountGUID" Visible="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" ShowSelection="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/Account/Filter.ascx" />
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
        <asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>