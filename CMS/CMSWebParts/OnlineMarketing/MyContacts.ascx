<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/OnlineMarketing/MyContacts.ascx.cs" Inherits="CMSWebParts_OnlineMarketing_MyContacts" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
    Visible="false" />
<cms:UniGrid runat="server" ID="gridElem" ObjectType="om.contactlist" OrderBy="ContactLastName"
    Columns="ContactID,ContactFullNameJoined,ContactEmail,ContactLastModified,ContactMobilePhone,ContactBusinessPhone,ContactBirthday,ContactFullAddressJoined"
    IsLiveSite="false" Visible="false">
    <GridActions>
        <ug:Action Name="edit" ExternalSourceName="edit" Caption="$General.Edit$" FontIconClass="icon-eye" FontIconStyle="Allow"
            CommandArgument="ContactID" ModuleName="CMS.OnlineMarketing" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="ContactFullNameJoined" Caption="$om.contact.name$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false" Name="email">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="ContactFullAddressJoined" Caption="$om.contact.fulladdress$" Wrap="false"
            Name="address">
        </ug:Column>
        <ug:Column Source="ContactMobilePhone" Caption="$om.contact.mobilephone$" Wrap="false"
            Name="mobile">
        </ug:Column>
        <ug:Column Source="ContactBusinessPhone" Caption="$om.contact.businessphone$" Wrap="false"
            Name="business">
        </ug:Column>
        <ug:Column Source="ContactBirthday" Caption="$om.contact.birthday$" Wrap="false"
            Name="birth">
        </ug:Column>
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" ShowSelection="false" FilterLimit="10" />
</cms:UniGrid>
