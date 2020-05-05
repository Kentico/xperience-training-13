<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="List.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Activity list"
    Inherits="CMSModules_Activities_Pages_Tools_Activities_Activity_List" Theme="Default" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/Activities/Controls/UI/Activity/List.ascx" TagName="ActivityList" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="HeaderActions" Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="control-group-inline header-actions-container">
                <cms:HeaderActions ID="hdrActions" runat="server" IsLiveSite="false" />
                <cms:LocalizedLabel ID="lblWarnNew" runat="server" ResourceString="om.choosesite"
                    EnableViewState="false" Visible="false" CssClass="button-explanation-text" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDis" Visible="false">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSEnableOnlineMarketing;CMSCMActivitiesEnabled" />
    </asp:Panel>
    <cms:ActivityList ID="listElem" runat="server" IsLiveSite="false" ShowContactNameColumn="true" ShowRemoveButton="true" />
</asp:Content>
