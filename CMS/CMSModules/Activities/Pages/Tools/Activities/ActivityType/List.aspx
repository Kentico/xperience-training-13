<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="List.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Activity type list"
    Inherits="CMSModules_Activities_Pages_Tools_Activities_ActivityType_List" Theme="Default" %>
    
<%@ Register Src="~/CMSModules/Activities/Controls/UI/ActivityType/List.ascx" TagName="ActivityTypeList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDis" Visible="false">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSEnableOnlineMarketing" />
    </asp:Panel>
    <cms:ActivityTypeList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>