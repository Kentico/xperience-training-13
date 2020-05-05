<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Validation_HTMLValidator"
    CodeBehind="HTMLValidator.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Panel ID="pnlGrid" runat="server" CssClass="Validation">
    <cms:CMSUpdateProgress ID="up" runat="server" HandlePostback="true" EnableViewState="false" />
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
    <cms:CMSCheckBox runat="server" ID="chkErrorsOnly" Visible="false" Checked="false" AutoPostBack="true"/>
    <cms:LocalizedLabel ID="lblResults" runat="server" EnableViewState="false" CssClass="Results"
        Visible="false" DisplayColon="true" />
    <cms:UniGrid ID="gridValidationResult" runat="server" ExportFileName="HTML_validation_results"
        ShowActionsLabel="false">
        <GridActions>
            <ug:Action Name="view" ExternalSourceName="view" FontIconClass="icon-eye" FontIconStyle="Allow" Caption="$validation.viewcode$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="type" Caption="$validation.html.type$" Wrap="false" ExternalSourceName="type" />
            <ug:Column Source="line" Caption="$validation.html.line$" Wrap="false" />
            <ug:Column Source="column" Caption="$validation.html.column$" Wrap="false" AllowSorting="false" />
            <ug:Column Source="message" Caption="$validation.html.message$" AllowSorting="false"
                 ExternalSourceName="message" Width="50%"/>
            <ug:Column Source="source" ExternalSourceName="source" Caption="$validation.html.source$"
                Wrap="true" AllowSorting="false" />
        </GridColumns>
        <PagerConfig DefaultPageSize="10" />
    </cms:UniGrid>
</asp:Panel>
<asp:HiddenField ID="hdnHTML" runat="server" EnableViewState="false" />
