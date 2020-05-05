<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Contacts.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_ContactGroup_Contacts" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx"
    TagName="ContactSelector" TagPrefix="cms" %>

<asp:Panel ID="pnlSelector" runat="server" CssClass="cms-edit-menu">
    <cms:ContactSelector ID="contactSelector" runat="server" IsLiveSite="false" />
</asp:Panel>
<asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" ShowProgress="true">
        <ContentTemplate>
            <cms:CMSUpdateProgress ID="loading" runat="server" HandlePostback="true" DisplayTimeout="100" />
            <cms:UniGrid runat="server" ID="gridElem" OrderBy="ContactLastName" ObjectType="om.contactgroupcontactlist"
                ShowObjectMenu="false" IsLiveSite="false" RememberStateByParam="issitemanager" Columns="ContactID,ContactFirstName,ContactLastName,ContactEmail,ContactStatusID,ContactCountryID,ContactGroupMemberFromCondition,ContactGroupMemberFromAccount,ContactGroupMemberFromManual">
                <GridActions Parameters="ContactID">
                    <ug:Action Name="edit" Caption="$om.contact.viewdetail$" FontIconClass="icon-eye" FontIconStyle="Allow"
                        ExternalSourceName="edit" CommandArgument="ContactID" ModuleName="CMS.ContactManagement" />
                    <ug:Action Name="remove" Caption="$General.Remove$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmRemove$"
                        ModuleName="CMS.ContactManagement" ExternalSourceName="remove" />
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
                    <ug:Column Name="ContactStatusID" Source="ContactStatusID" AllowSorting="false" Caption="$om.contactstatus$" ExternalSourceName="#transform: om.contactstatus.contactstatusdisplayname" Wrap="false">
                        <Filter Type="text" Path="~/CMSModules/ContactManagement/FormControls/ContactStatusSelector.ascx" Size="100" />
                    </ug:Column>
                    <ug:Column Name="ContactCountryID" Source="ContactCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname" Caption="$objecttype.cms_country$" Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/ContactManagement/Filters/CountryFilter.ascx" Size="100" >
                            <CustomFilterParameters>
                                <ug:FilterParameter Name="CountryIDColumnName" Value="ContactCountryID" />
                            </CustomFilterParameters>
                        </Filter>
                    </ug:Column>
                    <ug:Column Source="ContactGroupMemberFromCondition" ExternalSourceName="#yesno" Caption="$om.contactgroupmember.memberfromcondition$"
                        Wrap="false">
                        <Filter Type="bool" Format="{2} = ISNULL(ContactGroupMemberFromCondition, 0)"/>
                    </ug:Column>
                    <ug:Column Source="ContactGroupMemberFromAccount" ExternalSourceName="#yesno" Caption="$om.contactgroupmember.MemberFromAccount$"
                        Wrap="false">
                        <Filter Type="bool" Format="{2} = ISNULL(ContactGroupMemberFromAccount, 0)"/>
                    </ug:Column>
                    <ug:Column Source="ContactGroupMemberFromManual" ExternalSourceName="#yesno" Caption="$om.contactgroupmember.MemberFromManual$"
                        Wrap="false">
                        <Filter Type="bool" Format="{2} = ISNULL(ContactGroupMemberFromManual, 0)"/>
                    </ug:Column>
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" ShowSelection="true" SelectionColumn="ContactID" />
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
</asp:Panel>