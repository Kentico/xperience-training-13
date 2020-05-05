<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Campaign properties - Schedule" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Schedule"
    Theme="Default" Codebehind="Tab_Schedule.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel  runat="server" CssClass="control-label" DisplayColon="true" ResourceString="campaign.launchdate" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DateTimePicker runat="server" ID="dtFrom" />
                </div>
            </div>
            
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel runat="server" CssClass="control-label" DisplayColon="true" ResourceString="campaign.finishdate" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DateTimePicker runat="server" ID="dtTo" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
