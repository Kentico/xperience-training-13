<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="EmailQueue.ascx.cs" Inherits="CMSModules_EmailQueue_Controls_EmailQueue" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid ID="gridElem" runat="server" ShortID="g" ObjectType="cms.email" IsLiveSite="false"
    OrderBy="EmailPriority DESC, EmailID" 
    Columns="EmailID, EmailSubject, EmailTo, EmailPriority, EmailLastSendResult, EmailLastSendAttempt, EmailStatus, EmailIsMass">
    <GridActions>
        <ug:Action ExternalSourceName="resend" Name="resend" Caption="$general.resend$" FontIconClass="icon-message" />
        <ug:Action ExternalSourceName="delete" Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$general.confirmdelete$" />
        <ug:Action ExternalSourceName="edit" Name="display" Caption="$general.view$" FontIconClass="icon-eye" FontIconStyle="Allow" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="EmailSubject" ExternalSourceName="subject" Caption="$general.subject$" Wrap="false" CssClass="main-column-100">
            <Tooltip Source="EmailSubject" Encode="true" />
        </ug:Column>
        <ug:Column Source="##ALL##" ExternalSourceName="EmailTo" Caption="$EmailQueue.To$" Wrap="false" />
        <ug:Column Source="EmailPriority" ExternalSourceName="priority" Caption="$EmailQueue.Priority$" Wrap="false" />
        <ug:Column Source="EmailLastSendResult" ExternalSourceName="result" IsText="true" Caption="$EmailQueue.LastSendResult$" Wrap="false">
            <Tooltip Source="EmailLastSendResult" Encode="true" />
        </ug:Column>
        <ug:Column Source="EmailLastSendAttempt" Caption="$EmailQueue.LastSendAttempt$" Wrap="false" />
        <ug:Column Source="EmailStatus" ExternalSourceName="status" Caption="$EmailQueue.Status$" Wrap="false" />
    </GridColumns>
    <GridOptions DisplayFilter="false" ShowSelection="true" />    
</cms:UniGrid>
    
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />