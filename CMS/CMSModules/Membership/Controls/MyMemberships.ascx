<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="MyMemberships.ascx.cs"
    Inherits="CMSModules_Membership_Controls_MyMemberships" %>
<%@ Register TagPrefix="cms" TagName="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>
<%@ Register TagPrefix="ug" Assembly="CMS.UIControls" Namespace="CMS.UIControls.UniGridConfig" %>
<cms:LocalizedButton ID="btnBuyMembership" runat="server" ResourceString="mymemberships.buymembership"
    Visible="false" ButtonStyle="Default" Style="margin-bottom: 15px;" EnableViewState="false" />
<cms:UniGrid ID="membershipsGrid" ShortID="g" runat="server" Columns="MembershipID,MembershipDisplayName,ValidTo"
    OrderBy="MembershipDisplayName" ObjectType="cms.membershiplist" OnOnExternalDataBound="membershipsUniGridElem_OnExternalDataBound">
    <GridColumns>
        <ug:Column Name="MembershipName" Source="MembershipDisplayName" Caption="$mymemberships.membership$"
            Wrap="false" />
        <ug:Column Name="ValidTo" Source="ValidTo" ExternalSourceName="validto" Caption="$mymemberships.validto$"
            Wrap="false" />
        <ug:Column Name="Fill" CssClass="main-column-100" />
    </GridColumns>
</cms:UniGrid>
