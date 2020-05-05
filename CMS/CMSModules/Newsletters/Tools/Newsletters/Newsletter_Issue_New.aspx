<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Newsletter_Issue_New.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_New" Theme="Default"
    EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Newsletter - Pick a template" %>

<%@ Register TagPrefix="cms" TagName="ObjectAttachmentSelector" Src="~/CMSModules/ImportExport/Controls/Global/ObjectAttachmentSelector.ascx" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <cms:MessagesPlaceHolder ID="plcMessages" runat="server" UseRelativePlaceHolder="false" IsLiveSite="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="newslettertemplateselect.properties" />
    <div class="form-horizontal">
        <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueDisplayName">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayName" runat="server" ResourceString="general.name"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtDisplayName" ShowRequiredMark="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtDisplayName" runat="server" MaxLength="200" />
            </div>
        </asp:Panel>
        <asp:Panel CssClass="form-group" runat="server" ID="pnlIssueForAutomation">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblIssueForAutomation" runat="server" ResourceString="newsletter.issue.forautomation"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="chbIssueForAutomation" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chbIssueForAutomation" Checked="false" runat="server" />
            </div>
        </asp:Panel>
    </div>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="newslettertemplateselect.selectorheading" />
    <div class="email-template-selector">
        <cms:ObjectAttachmentSelector id="ucTemplateSelector" runat="server" IDColumn="TemplateID" DescriptionColumn="TemplateDescription" 
            DisplayNameColumn="TemplateDisplayName" IconClassColumn="TemplateIconClass" DefaulIconClass="icon-accordion" ThumbnailGUIDColumn="TemplateThumbnailGUID" />
    </div>
</asp:Content>
