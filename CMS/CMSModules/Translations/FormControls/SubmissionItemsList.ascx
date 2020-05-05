<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_FormControls_SubmissionItemsList"
     Codebehind="SubmissionItemsList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridElem" ObjectType="cms.translationsubmissionitem"
            OrderBy="SubmissionItemName" Columns="SubmissionItemID,SubmissionItemObjectType,SubmissionItemObjectID,SubmissionItemName,SubmissionItemTargetCulture,SubmissionItemWordCount,SubmissionItemCharCount,SubmissionItemType, CASE WHEN ([SubmissionItemTargetXLIFF] IS NULL) THEN 0 ELSE 1 END AS SubmissionItemIsTranslated"
            IsLiveSite="false" ShowObjectMenu="false" ShowActionsMenu="false">
            <GridActions Parameters="SubmissionItemID">
                <ug:Action ExternalSourceName="downloadxliff" Name="downloadxliff" Caption="$translationservice.downloadxliff$" FontIconClass="icon-eye" FontIconStyle="Allow" />
                <ug:Action Name="uploadxliff" Caption="$translationservice.uploadxliff$" FontIconClass="icon-arrow-up-line"
                    OnClick="ShowUploadDialog(0, {0});" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="SubmissionItemName" Caption="$translationservice.documentname$" Wrap="false">
                    <Filter Type="Text" />
                </ug:Column>
                <ug:Column Source="SubmissionItemTargetCulture" Caption="$translationservice.targetculture$" ExternalSourceName="#culturenamewithflag" Wrap="false" />
                <ug:Column Source="SubmissionItemWordCount" Caption="$translationservice.wordcount$" Wrap="false" />
                <ug:Column Source="SubmissionItemCharCount" Caption="$translationservice.charcount$" Wrap="false" />
                <ug:Column Source="SubmissionItemIsTranslated" Caption="$transman.Translated$" ExternalSourceName="#yesno" Wrap="false" AllowSorting="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" />
        </cms:UniGrid>
        <br />
        <cms:LocalizedButton runat="server" ID="btnExportToZip" ResourceString="translationservice.downloadxliffinzip" ButtonStyle="Default" />
        <cms:LocalizedButton runat="server" ID="btnImportFromZip" ResourceString="translationservice.importxlifffromzip" ButtonStyle="Default" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
