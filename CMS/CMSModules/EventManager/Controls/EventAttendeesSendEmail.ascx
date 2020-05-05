<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/EventManager/Controls/EventAttendeesSendEmail.ascx.cs"
    Inherits="CMSModules_EventManager_Controls_EventAttendeesSendEmail" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<cms:LocalizedHeading runat="server" ID="lblTitle" Level="4" ResourceString="Events_SendEmail.lblTitle"
    EnableViewState="false" />
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
    Visible="false" />
<asp:PlaceHolder runat="server" ID="plcSend">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSenderName" runat="server" ResourceString="Events_SendEmail.lblSenderName"
                    EnableViewState="false" ShowRequiredMark="True" AssociatedControlID="txtSenderName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSenderName" runat="server" MaxLength="250" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSenderEmail" runat="server" ResourceString="Events_SendEmail.lblSenderEmail"
                    EnableViewState="false" ShowRequiredMark="True" AssociatedControlID="txtSenderEmail" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtSenderEmail" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" ResourceString="general.subject"
                    DisplayColon="true" EnableViewState="false" ShowRequiredMark="True" AssociatedControlID="txtSubject" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell textarea-full-width">
                <cms:CMSHtmlEditor ID="htmlEmail" runat="server" Height="500" />
            </div>
        </div>
    </div>
</asp:PlaceHolder>