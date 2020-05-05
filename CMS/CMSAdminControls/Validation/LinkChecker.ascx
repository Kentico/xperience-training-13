<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Validation_LinkChecker"
     Codebehind="LinkChecker.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Panel runat="server" ID="pnlLog" Visible="false">
    <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="LinkChecker" />
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<asp:Panel ID="pnlGrid" runat="server" CssClass="Validation">
    <cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
    <cms:LocalizedLabel ID="lblResults" runat="server" EnableViewState="false" class="Results"
        Visible="false" DisplayColon="true" />
    <cms:UniGrid ID="gridValidationResult" runat="server" ExportFileName="link_checker_results" ShowActionsLabel="false">
        <GridColumns>
            <ug:Column Source="statuscode" ExternalSourceName="statuscode" Caption="$validation.link.statuscode$"
                Wrap="false" Sort="statuscodevalue"/>
            <ug:Column Source="type" Caption="$validation.link.type$" ExternalSourceName="type" />
            <ug:Column Source="message" ExternalSourceName="message" Caption="$validation.link.message$"
                Width="60%" AllowSorting="false" />
            <ug:Column Source="url" ExternalSourceName="url" Caption="$validation.link.url$"
                Width="30%" AllowSorting="false" />
            <ug:Column Source="time" Caption="$validation.link.time$" ExternalSourceName="time" Sort="timeint" />
        </GridColumns>
        <PagerConfig DefaultPageSize="10" />
    </cms:UniGrid>
    <asp:HiddenField ID="hdnHTML" runat="server" EnableViewState="false" />
</asp:Panel>
