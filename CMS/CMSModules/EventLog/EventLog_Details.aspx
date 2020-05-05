<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="EventLog - Details"
    Inherits="CMSModules_EventLog_EventLog_Details" Theme="Default"  Codebehind="EventLog_Details.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEventID" runat="server" ResourceString="EventLogDetails.EventID"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblEventIDValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEventType" runat="server" ResourceString="EventLogDetails.EventType"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblEventTypeValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEventTime" runat="server" ResourceString="EventLogDetails.EventTime"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblEventTimeValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSource" runat="server" ResourceString="EventLogDetails.Source"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblSourceValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEventCode" runat="server" ResourceString="EventLogDetails.EventCode"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblEventCodeValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcUserID" EnableViewState="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUserID" runat="server" ResourceString="EventLogDetails.UserID"
                        EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblUserIDValue" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcUserName" EnableViewState="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" ResourceString="general.username"
                        EnableViewState="false" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblUserNameValue" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblIPAddress" runat="server" ResourceString="EventLogDetails.IPAddress"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblIPAddressValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcNodeID" EnableViewState="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblNodeID" runat="server" ResourceString="EventLogDetails.NodeID"
                        EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblNodeIDValue" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcNodeName" EnableViewState="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblNodeName" runat="server" ResourceString="EventLogDetails.NodeName"
                        EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblNodeNameValue" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEventDescription" runat="server" ResourceString="general.description"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblEventDescriptionValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcSite" EnableViewState="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSiteName" runat="server" ResourceString="general.sitename"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblSiteNameValue" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblMachineName" runat="server" ResourceString="EventLogDetails.MachineName"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblMachineNameValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEventUrl" runat="server" ResourceString="EventLogDetails.EventUrl"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblEventUrlValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblUrlReferrer" runat="server" ResourceString="EventLogDetails.UrlReferrer"
                    EnableViewState="false" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblUrlReferrerValue" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblUserAgent" runat="server" ResourceString="EventLogDetails.UserAgent"
                    EnableViewState="false" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblUserAgentValue" runat="server" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnExport" runat="server" EnableViewState="false" ResourceString="eventlogdetails.export" Visible="false" ButtonStyle="Default" />
    <cms:LocalizedHyperlink ID="btnReportBug" runat="server" EnableViewState="false" ResourceString="eventlogdetails.reportbug" Visible="false" Target="_blank" CssClass="btn btn-default"/>
    <cms:LocalizedButton runat="server" ID="btnPrevious" ButtonStyle="Primary" Enabled="false"
        EnableViewState="false" ResourceString="general.back" />
    <cms:LocalizedButton runat="server" ID="btnNext" ButtonStyle="Primary"
        Enabled="false" EnableViewState="false" ResourceString="general.next" />
</asp:Content>
