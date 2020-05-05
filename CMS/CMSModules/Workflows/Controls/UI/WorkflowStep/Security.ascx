<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_Security"
     Codebehind="Security.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdateRoles" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:LocalizedHeading runat="server" ID="headRoles" Level="4" EnableViewState="false" />
        <div class="form-horizontal">
            <div class="form-group radio-list-vertical">
                <cms:CMSRadioButtonList ID="rbRoleType" runat="server" AutoPostBack="true" RepeatDirection="Vertical" />
            </div>
            <div class="form-group">
                <cms:UniSelector ID="usRoles" runat="server" IsLiveSite="false" SelectionMode="Multiple" ResourcePrefix="addroles" />
            </div>            
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>

<cms:CMSUpdatePanel ID="pnlUpdateUsers" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:LocalizedHeading runat="server" ID="headUsers" Level="4" EnableViewState="false" />
        <div class=" form-horizontal">
            <div class="form-group radio-list-vertical">
                <cms:CMSRadioButtonList ID="rbUserType" runat="server" AutoPostBack="true" RepeatDirection="Vertical" />
            </div>
            <div class="form-group">
                <cms:UniSelector ID="usUsers" runat="server" IsLiveSite="false" SelectionMode="Multiple"
                    ResourcePrefix="addusers" DisplayNameFormat="##USERDISPLAYFORMAT##" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>