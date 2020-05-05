<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_TimeZones_Pages_TimeZone_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="TimeZone_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="uniGrid" GridName="TimeZone_List.xml" OrderBy="TimeZoneGMT"
        IsLiveSite="false" Columns="TimeZoneID,TimeZoneDisplayName,TimeZoneGMT,TimeZoneDaylight" ExportFileName="cms_timezone" />
</asp:Content>
