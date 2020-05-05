<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Preview"  Codebehind="Newsletter_Issue_Preview.aspx.cs"
     Title="Newsletter issue - Preview" Theme="Default" %>

<asp:Content ID="ctnPreviewMode" runat="server" ContentPlaceHolderID="plcActions">
    <asp:Panel runat="server" ID="pnlPreviewMode" CssClass="control-group-inline header-actions-container">
        <div class="btn-group">
			<button type="button" id="btnDesignPreview" class="btn btn-default active"><%=ResHelper.GetString("newsletter.issue.preview.design") %></button>
            <button type="button" id="btnCodePreview" class="btn btn-default"><%=ResHelper.GetString("newsletter.issue.preview.code") %></button>
		</div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="cntSubscribers" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSubscriber" runat="server" EnableViewState="false" ResourceString="selectsubscriber.general.itemname" DisplayColon="true" CssClass="control-label" AssociatedControlID="drpSubscribers"/>
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:CMSDropDownList ID="drpSubscribers" runat="server" CssClass="form-control" EnableViewState="false"/>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder runat="server" ID="plcEmailPreview">
        <div id="design-preview" class="row">
            <div class="col-xs-7 col-md-5">
                <div class="form-horizontal">
                    <div class="form-group no-margin">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblFrom" runat="server" EnableViewState="false" ResourceString="general.fromemail" DisplayColon="true" AssociatedControlID="lblSubjectValue" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label CssClass="form-control-text" ID="lblFromValue" runat="server" EnableViewState="false" />
                            <asp:Label CssClass="form-control-text bold-label" ID="lblFromEmailValue" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group no-margin">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" ResourceString="general.subject" DisplayColon="true" AssociatedControlID="lblSubjectValue" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label CssClass="form-control-text" ID="lblSubjectValue" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group margin-bot">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblPreheader" runat="server" EnableViewState="false" ResourceString="newsletterissue.preheader" DisplayColon="true" AssociatedControlID="lblSubjectValue" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label CssClass="form-control-text" ID="lblPreheaderValue" runat="server" EnableViewState="false" />
                            <span class="info-icon">
                                <cms:LocalizedLabel runat="server" ID="lblPreheaderAlert" CssClass="sr-only" Visible="False" />
                                <cms:CMSIcon ID="iconPreheaderAlert" runat="server" CssClass="icon-exclamation-triangle warning-icon" EnableViewState="false" aria-hidden="true" data-html="true" Visible="false" />
                            </span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="mobile-preview-outline">
                            <span class="info-icon preview-alert">
                                <cms:LocalizedLabel runat="server" ID="lblApproximationAlert" CssClass="sr-only" />
                                <cms:CMSIcon ID="iconApproximationAlert" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                            </span>
                            <div class="email-mobile-preview">
                                <iframe id="mobile-preview" class="phone-screen"></iframe>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xs-11 col-sm-5 col-md-7 preview-column">
                <div class="email-preview">
                    <iframe id="preview"></iframe>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="plcEmailSource">
        <div id="code-preview" style="display: none;">
            <cms:MacroEditor runat="server" ID="txtSource" ReadOnly="True"/>
        </div>
    </asp:PlaceHolder>
</asp:Content>