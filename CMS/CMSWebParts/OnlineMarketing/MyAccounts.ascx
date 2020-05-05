<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/OnlineMarketing/MyAccounts.ascx.cs" Inherits="CMSWebParts_OnlineMarketing_MyAccounts" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
    Visible="false" />
<cms:UniGrid runat="server" ID="gridElem" OrderBy="AccountName" ObjectType="om.accountlist"
    Columns="AccountID,AccountName,AccountWebSite,AccountPhone,AccountEmail,AccountFax,AccountPrimaryContactID,AccountFullAddress,PrimaryContactFullName"
    IsLiveSite="false" Visible="false" >
    <GridActions>
        <ug:Action Name="edit" ExternalSourceName="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" CommandArgument="AccountID" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="AccountName" Caption="$om.account.name$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="AccountFulLAddress" Caption="$om.account.fulladdress$" Wrap="false" Name="address">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="AccountWebSite" ExternalSourceName="website" Caption="$om.account.url$" Wrap="false" Name="website">
        </ug:Column>
        <ug:Column Source="AccountPhone" Caption="$om.account.phone$" Wrap="false" Name="phone">
        </ug:Column>
        <ug:Column Source="AccountEmail" Caption="$general.email$" Wrap="false" Name="email">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="AccountFax" Caption="$om.account.fax$" Wrap="false" Name="fax">
        </ug:Column>
        <ug:Column Source="##ALL##" ExternalSourceName="primary" Caption="$om.account.primarycontact$" Wrap="false" Name="contact" Sort="PrimaryContactFullName">
            <Filter Type="text" Source="PrimaryContactFullName" />
        </ug:Column>        
        <ug:Column CssClass="filling-column" />            
    </GridColumns>
    <GridOptions DisplayFilter="true" ShowSelection="false" FilterLimit="10" />
</cms:UniGrid>
    