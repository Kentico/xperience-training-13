<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_EmailQueue_SentEmails"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="SentEmails.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="emailqueue.archive.text" CssClass="listing-title" EnableViewState="false" />
    <span class="InfoLabel">
        <cms:CMSIcon runat="server" ID="icShowFilter" CssClass="icon-caret-down cms-icon-30" />
        <cms:LocalizedLinkButton ID="btnShowFilter" runat="server" OnClick="btnShowFilter_Click" />
    </span>
    <asp:PlaceHolder ID="plcFilter" runat="server" Visible="false">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblFrom" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="general.from" />
                </div>
                <cms:TextSimpleFilter ID="fltFrom" runat="server" Column="EmailFrom" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblTo" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="general.toemail" />
                </div>
                <cms:TextSimpleFilter ID="fltTo" runat="server" Column="EmailTo" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="general.subject" />
                </div>
                <cms:TextSimpleFilter ID="fltSubject" runat="server" Column="EmailSubject" />
            </div>
            <asp:Panel ID="pnlBodyFilter" runat="server" class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblBody" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="general.body" />
                </div>
                <cms:TextSimpleFilter ID="fltBody" runat="server" Column="EmailBody" />
            </asp:Panel>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblPriority" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="emailqueue.priority" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSDropDownList ID="drpPriority" runat="server" CssClass="DropDownFieldFilter" />
                </div>
            </div>
            <div class="form-group form-group-buttons">
                <div class="filter-form-buttons-cell-wide">
                    <cms:LocalizedButton ID="btnFilter" runat="server" OnClick="btnFilter_Clicked" ButtonStyle="Primary"
                        EnableViewState="false" ResourceString="general.filter" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <cms:UniGrid ID="gridElem" runat="server" ShortID="g" ObjectType="cms.email" IsLiveSite="false"
        OrderBy="EmailLastSendAttempt DESC"
        Columns="EmailID, EmailSubject, EmailTo, EmailPriority, EmailLastSendAttempt, EmailIsMass">
        <GridActions Parameters="EmailID">
            <ug:Action ExternalSourceName="resend" Name="resend" Caption="$general.resend$" FontIconClass="icon-message" />
            <ug:Action ExternalSourceName="delete" Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
            <ug:Action ExternalSourceName="edit" Name="display" Caption="$general.view$" FontIconClass="icon-eye" FontIconStyle="Allow" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="EmailSubject" ExternalSourceName="subject" Caption="$general.subject$" Wrap="false" CssClass="main-column-100" />
            <ug:Column Source="##ALL##" ExternalSourceName="EmailTo" Caption="$EmailQueue.To$" Wrap="false" />
            <ug:Column Source="EmailPriority" ExternalSourceName="priority" Caption="$EmailQueue.Priority$" Wrap="false" />
            <ug:Column Source="EmailLastSendAttempt" Caption="$EmailQueue.LastSendAttempt$" Wrap="false" />
        </GridColumns>
        <GridOptions DisplayFilter="false" ShowSelection="true" />
    </cms:UniGrid>

    <asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
</asp:Content>