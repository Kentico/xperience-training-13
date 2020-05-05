<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_ChangeStylesheetLanguage"  Codebehind="~/CMSFormControls/ChangeStylesheetLanguage.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniControls/UniButton.ascx" TagName="UniButton" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:PlaceHolder ID="plcLanguage" runat="server">
            <span class="form-control-text">
                <asp:Label ID="lblSelectedLang" runat="server" EnableViewState="false" />
                <cms:UniButton ID="btnOpenSelection" runat="server" ShowAsButton="false" CssClass="ChangeStylesheetLanguageLink" EnableViewState="false" />
            </span>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcSelect" runat="server" Visible="false">
            <div class="control-group-inline">
                <cms:CMSDropDownList ID="ddlLanguages" runat="server" CssClass="form-control" AutoPostBack="true" />
                <cms:LocalizedButton ID="btnChange" runat="server" ShowAsButton="true" />
            </div>
        </asp:PlaceHolder>
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btnChange" />
        <asp:AsyncPostBackTrigger ControlID="ddlLanguages" />
        <asp:AsyncPostBackTrigger ControlID="btnOpenSelection" />
    </Triggers>
</cms:CMSUpdatePanel>
