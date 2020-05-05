<%@ Control Language="C#" AutoEventWireup="True" Inherits="CMSModules_ContactManagement_Controls_UI_Account_Subsidiaries"
     Codebehind="Subsidiaries.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/AccountSelector.ascx"
    TagName="AccountSelector" TagPrefix="cms" %>

<%-- Reference to Filter.ascx is here intentionally --%>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/Filter.ascx" TagPrefix="cms" TagName="AccountFilter" %>

<asp:Panel ID="pnlSelector" runat="server" CssClass="cms-edit-menu">
    <cms:AccountSelector ID="accountSelector" runat="server" IsLiveSite="false" />
</asp:Panel>
<asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="gridElem" OrderBy="AccountName" ObjectType="om.accountlist"
                Columns="AccountID,AccountName,AccountStatusID,PrimaryContactFullName,AccountCountryID"
                IsLiveSite="false" WhereCondition="AccountSubsidiaryOfID={%EditedObject.AccountID%}"
                ShowObjectMenu="false" HideFilterButton="true" RememberDefaultState="true">
                <GridActions Parameters="AccountID">
                    <ug:Action Name="edit" Caption="$om.account.viewdetail$" FontIconClass="icon-edit" FontIconStyle="Allow"
                        CommandArgument="AccountID" ModuleName="CMS.ContactManagement" ExternalSourceName="edit" />
                    <ug:Action Name="remove" ExternalSourceName="remove" Caption="$General.Remove$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmRemove$" ModuleName="CMS.ContactManagement" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="AccountName" Caption="$om.account.name$" Wrap="false" />
                    <ug:Column Source="AccountStatusID" AllowSorting="false" ExternalSourceName="#transform: om.accountstatus.accountstatusdisplayname"
                        Caption="$om.accountstatus.name$" Wrap="false" />
                    <ug:Column Source="PrimaryContactFullName" Caption="$om.contact.primary$" Wrap="false" />
                    <ug:Column Source="AccountCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname"
                        Caption="$objecttype.cms_country$" Wrap="false" />
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
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>