<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CheckedOutObjectsList.ascx.cs"
    Inherits="CMSModules_Objects_Controls_Locking_CheckedOutObjectsList" %>
<%@ Register TagPrefix="cms" TagName="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>
<%@ Register TagPrefix="ug" Assembly="CMS.UIControls" Namespace="CMS.UIControls.UniGridConfig" %>
<%-- Grid --%>
<cms:UniGrid ID="grid" ShortID="g" runat="server" ObjectType="cms.objectsettings"
    ShowObjectMenu="false" OrderBy="ObjectCheckedOutWhen">
    <GridActions>
        <ug:Action Name="checkin" ExternalSourceName="checkin" Caption="$general.checkin$" FontIconClass="icon-lock-unlocked" />
        <ug:Action Name="undocheckout" ExternalSourceName="undocheckout" Caption="$general.undocheckout$" FontIconClass="icon-arrow-u-left" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="##ALL##" ExternalSourceName="#transform: {% ObjectSettingsObjectType %} => {% ObjectSettingsObjectID %} : {% TypeInfo.GetNiceObjectTypeName() %} '{% DisplayName %}'"
            Caption="$general.objectname$" Wrap="false" CssClass="main-column-100" />
        <ug:Column Source="ObjectCheckedOutWhen" Caption="$general.checkouttime$" Wrap="false" />
    </GridColumns>
</cms:UniGrid>
<%-- Check in button --%>
<cms:CMSButton ID="btnCheckIn" ButtonStyle="Primary" runat="server" Visible="false" />
<%-- Undo check out button --%>
<cms:CMSButton ID="btnUndoCheckOut" ButtonStyle="Primary" runat="server" Visible="false" />
<%-- Hidden fields --%>
<asp:HiddenField ID="hdnObjectType" runat="server" />
<asp:HiddenField ID="hdnObjectId" runat="server" />
