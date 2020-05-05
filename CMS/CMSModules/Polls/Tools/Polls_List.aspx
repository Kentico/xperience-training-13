<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Polls" Inherits="CMSModules_Polls_Tools_Polls_List" Theme="Default"  Codebehind="Polls_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/PollsList.ascx" TagName="PollsList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Polls/Controls/Filters/SiteSelector.ascx" TagName="SiteFilter"
    TagPrefix="cms" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <cms:SiteFilter ID="fltSite" ShortID="c" runat="server" DisplayAllGlobals="true" />
</asp:Content>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="control-group-inline header-actions-container">
                <cms:HeaderActions ID="hdrActions" runat="server" ShortID="ha" IsLiveSite="false" />
                <div class="header-actions-label">
                    <cms:LocalizedLabel ID="lblWarnNew" runat="server" ResourceString="pollslist.choosegloborsite"
                        EnableViewState="false" Visible="false" CssClass="form-control-text" />
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:PollsList ID="PollsList" runat="server" Visible="true" DelayedReload="false"
                ShortID="l" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
