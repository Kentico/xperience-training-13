<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_FormControls_LinkedInCompanyAccessToken"  Codebehind="LinkedInCompanyAccessToken.ascx.cs" %>

<asp:Panel ID="pnlToken" runat="server">
    <asp:HiddenField runat="server" ID="hdnToken" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnTokenExpiration" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnTokenExpirationString" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnTokenAppId" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnCompanyId" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnCompanyName" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnCompanies" EnableViewState="False"/>
    
    <asp:Panel runat="server" ID="pnlCompanyInfo">
        <div class="control-group-inline">
            <strong><asp:Label ID="lblCompanyName" runat="server" CssClass="form-control-text"/></strong>
            <div>
                <asp:Label runat="server" ID="lblExpiration" CssClass="form-control-text" EnableViewState="False"></asp:Label>
            </div>
        </div>
        <div class="control-group-inline">
            <cms:LocalizedButton ID="btnReauthorize" runat="server" ButtonStyle="Default" ResourceString="sm.linkedin.account.reauthorize" OnClick="btnGetToken_OnClick"
            /><asp:Label ID="lblMessageReauthorize" runat="server" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlCompanySelector">
        <asp:DropDownList runat="server" ID="drpCompany" CssClass="form-control hide" EnableViewState="False"/>
        <cms:LocalizedButton ID="btnGetToken" runat="server" ButtonStyle="Default" ResourceString="sm.linkedin.account.selectcompany" OnClick="btnGetToken_OnClick"
        /><asp:Label ID="lblMessageAuthorize" runat="server" EnableViewState="false" CssClass="form-control-text" />
    </asp:Panel>
</asp:Panel>
