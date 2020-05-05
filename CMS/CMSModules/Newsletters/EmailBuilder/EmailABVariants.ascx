<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EmailABVariants.ascx.cs" Inherits="CMSModules_Newsletters_EmailBuilder_EmailABVariants" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>

<div class="ab-testing-container">
    <cms:LocalizedButton ID="btnNewVariant" runat="server" CssClass="btn-new-variant" EnableViewState="false" ResourceString="emailbuilder.addvariant" ButtonStyle="Primary" />
    <cms:UniGrid runat="server" ID="gridVariants" ObjectType="newsletter.issuevariant" OrderBy="IssueID" Columns="IssueID, IssueVariantName, IssueVariantOfIssueID, IssueIsABTest"
        ShowActionsLabel="false" ShowActionsMenu="false" ShowExportMenu="false" ShowObjectMenu="false" ApplyPageSize="false" RememberState="false">
        <GridActions>
            <ug:Action Name="delete" ExternalSourceName="delete" CommandArgument="IssueID"
                Caption="$newsletterslider.removevariant$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$newslettervariant.deletevariantconfirm$"
                ModuleName="CMS.Newsletter" Permissions="AuthorIssues" />
        </GridActions>
        <GridColumns>
            <ug:Column runat="server" Caption="$emailbuilder.variantlist.variantname$" Source="IssueVariantName" ExternalSourceName="variantName" CssClass="main-column-100"  />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions AllowSorting="false" />
        <PagerConfig DisplayPager="false" />
    </cms:UniGrid>
    <cms:ModalPopupDialog ID="variantDialog" runat="server" BackgroundCssClass="ModalBackground" CssClass="issue-variant-dialog" DefaultButton="btnSaveDialog">
        <asp:UpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:PageTitle ID="ucTitle" runat="server" EnableViewState="false" DisplayMode="Simple" ShowFullScreenButton="false" ShowCloseButton="false" IsDialog="true" />
                <div class="dialog-content">
                    <cms:AlertLabel runat="server" ID="errorMessage" AlertType="Error" EnableViewState="false" />
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayName" runat="server" ResourceString="general.name"
                                    DisplayColon="true" ShowRequiredMark="True" EnableViewState="false" AssociatedControlID="txtVariantName" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtVariantName" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="dialog-footer control-group-inline">
                    <cms:LocalizedButton ID="btnCloseDialog" runat="server" EnableViewState="false" ResourceString="general.cancel" ButtonStyle="Default" />
                    <cms:LocalizedButton ID="btnSaveDialog" runat="server" EnableViewState="false" ResourceString="general.create" ButtonStyle="Primary" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </cms:ModalPopupDialog>
</div>
