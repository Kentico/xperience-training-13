<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Controls_UI_TranslationSubmission_List"
     Codebehind="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:UniGrid runat="server" ID="gridElem" ObjectType="cms.translationsubmission"
    OrderBy="SubmissionDate DESC" Columns="SubmissionID,SubmissionServiceID,SubmissionStatus,SubmissionPrice,SubmissionStatusMessage,SubmissionDate,SubmissionSourceCulture,SubmissionTargetCulture,SubmissionName,SubmissionWordCount,SubmissionCharCount,SubmissionItemCount"
    IsLiveSite="false" EditActionUrl="Edit.aspx?submissionId={0}" ShowActionsMenu="false" ShowObjectMenu="false">
    <GridActions Parameters="SubmissionID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" ExternalSourceName="DeleteAction" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
            ModuleName="CMS.TranslationServices" Permissions="modify" />
        <ug:Action Name="downloadzip" ExternalSourceName="DownloadZipAction" Caption="$translationservice.downloadxliffinzip$" FontIconClass="icon-arrow-right-rect"
            OnClick="window.noProgress = true;" />
        <ug:Action Name="process" ExternalSourceName="ProcessAction" Caption="$translationservice.importtranslationstooltip$"
            FontIconClass="icon-triangle-right" FontIconStyle="Allow" Confirmation="$translationservice.confirmprocesstranslations$" />
        <ug:Action Name="resubmit" ExternalSourceName="ResubmitAction" Caption="$translationservice.resubmittooltip$"
            FontIconClass="icon-rotate-right" />
        <ug:Action Name="cancel" ExternalSourceName="CancelAction" Caption="$translationservice.cancelsubmissiontooltip$"
            FontIconClass="icon-times-circle" FontIconStyle="Critical" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="SubmissionName" Caption="$translationservice.sourcedocuments$"
            Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="SubmissionSourceCulture" Caption="$translationservice.sourceculture$"
            ExternalSourceName="#culturenamewithflag" Wrap="false">
        </ug:Column>
        <ug:Column Source="SubmissionTargetCulture" ExternalSourceName="SubmissionTargetCulture" Caption="$translationservice.targetcultures$" Wrap="false" Width="100%" AllowSorting="false">
        </ug:Column>
        <ug:Column Source="SubmissionDate" Caption="$translationservice.dateofsubmission$"
            Wrap="false" />
        <ug:Column Source="SubmissionStatus" ExternalSourceName="SubmissionStatus" Caption="$translationservice.status$"
            Wrap="false" />
        <ug:Column Source="SubmissionWordCount" Caption="$translationservice.wordcount$"
            Wrap="false">
            <Filter Type="integer" />
        </ug:Column>
        <ug:Column Source="SubmissionCharCount" Caption="$translationservice.charcount$"
            Wrap="false">
            <Filter Type="integer" />
        </ug:Column>
        <ug:Column Source="SubmissionPrice" ExternalSourceName="SubmissionPrice" Caption="$translationservice.submissionprice$"
            Wrap="false" />
        <ug:Column Source="SubmissionServiceID" Caption="$translationservice.translationservice$"
            ExternalSourceName="#transform: cms.translationservice.translationservicedisplayname"
            Wrap="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>
<cms:AsyncControl ID="ctlAsync" runat="server" AttachToRunningThread="True"  />

