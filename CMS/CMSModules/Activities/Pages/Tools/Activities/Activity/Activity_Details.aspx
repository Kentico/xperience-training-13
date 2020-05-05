<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Activity - Details" Inherits="CMSModules_Activities_Pages_Tools_Activities_Activity_Activity_Details"
    Theme="Default"  Codebehind="Activity_Details.aspx.cs" %>

<%@ Register Src="~/CMSModules/Activities/Controls/UI/Activity/Details.ascx"
    TagName="Details" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcDetails" runat="server">
        <asp:Panel runat="server" ID="pnlGen" CssClass="ActivityPanel">
            <cms:FormCategoryHeading runat="server" ID="headGeneral" Level="4" EnableViewState="false" IsAnchor="true" ResourceString="om.activity.details.groupgeneral" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" ResourceString="om.activity.type"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label ID="lblTypeVal" CssClass="form-control-text" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel AssociatedControlID="txtTitle" CssClass="control-label" ID="lblTitle" runat="server" ResourceString="om.activity.title"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtTitle" runat="server" ReadOnly="true" CssClass="ActivityCommentBox"
                            MaxLength="250" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblContact" runat="server" ResourceString="om.activity.contactname"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label ID="lblContactVal" CssClass="form-control-text" runat="server" EnableViewState="false" />
                        <cms:CMSAccessibleButton runat="server" ID="btnContact" IconCssClass="icon-edit" IconOnly="true" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblDate" runat="server" ResourceString="om.activity.date"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label ID="lblDateVal" CssClass="form-control-text" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel AssociatedControlID="txtURL" CssClass="control-label" ID="lblURL" runat="server" ResourceString="om.activity.url" DisplayColon="true"
                            EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell control-group-inline">
                        <cms:CMSTextBox ID="txtURL" runat="server" ReadOnly="true" CssClass="ActivityCommentBox" />
                        <asp:HyperLink runat="server" ID="btnView" CssClass="btn-icon" Target="_blank" Visible="false" EnableViewState="false">
                            <i class="icon-eye cms-icon-80" aria-hidden="true"></i>
                            <cms:LocalizedLabel CssClass="sr-only" ID="lblBtnView" runat="server" ResourceString="general.view" EnableViewState="false" />
                        </asp:HyperLink>
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel AssociatedControlID="txtURLRef" CssClass="control-label" ID="lblURLreferrer" runat="server" ResourceString="om.activity.urlreferrer"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtURLRef" runat="server" CssClass="ActivityCommentBox" ReadOnly="true" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" ResourceString="om.activity.site"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label ID="lblSiteVal" CssClass="form-control-text" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcActivityValue" runat="server" Visible="false">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblActivityValue" runat="server" ResourceString="om.activity.value"
                                DisplayColon="true" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcCampaign" Visible="false">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblCampaign" runat="server" ResourceString="om.activity.campaign"
                                DisplayColon="true" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblCampaignVal" CssClass="form-control-text" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </asp:Panel>
        <cms:Details runat="server" ID="cDetails" CssClass="ActivityPanel" />
        <asp:Panel ID="pnlComment" runat="server">
            <cms:FormCategoryHeading runat="server" ID="headComment" Level="4" EnableViewState="false" IsAnchor="true" ResourceString="om.activity.activitycomment" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblComment" runat="server" ResourceString="om.activity.comment"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <div class="control-group-inline">
                            <cms:CMSHtmlEditor ID="txtComment" runat="server" Enabled="False" ToolbarSet="Basic" IsLiveSite="false" />
                        </div>
                        <div class="control-group-inline">
                            <cms:LocalizedButton ID="btnStamp" runat="server" Enabled="False" ResourceString="formcontrol.htmlareacontrol.timestamp"
                                ButtonStyle="Default" EnableViewState="false" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Literal ID="ltlScript" runat="server" />
    </asp:PlaceHolder>
</asp:Content>