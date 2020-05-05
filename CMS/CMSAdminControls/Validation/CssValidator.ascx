<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Validation_CssValidator"
     Codebehind="CssValidator.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlLog" Visible="false">
    <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="CSSValidator" />
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<asp:Panel ID="pnlGrid" runat="server" CssClass="Validation">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
    <cms:LocalizedLabel ID="lblResults" runat="server" EnableViewState="false" CssClass="Results"
        Visible="false" DisplayColon="true" />
    <cms:UniGrid ID="gridValidationResult" runat="server" ExportFileName="CSS_validation_results"
        ShowActionsLabel="false">
        <GridActions>
            <ug:Action Name="view" ExternalSourceName="viewcss" FontIconClass="icon-eye" FontIconStyle="Allow" Caption="$validation.viewcode$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="line" Caption="$validation.css.line$" Wrap="false" />
            <ug:Column Source="context" Caption="$validation.css.context$" AllowSorting="false" />
            <ug:Column Source="message" Caption="$validation.css.message$" AllowSorting="false"
                Width="60%" />
            <ug:Column Source="source" ExternalSourceName="source" Caption="$validation.css.source$"
                Width="30%" />
        </GridColumns>
        <PagerConfig DefaultPageSize="10" />
    </cms:UniGrid>
</asp:Panel>
<asp:HiddenField ID="hdnHTML" runat="server" />
