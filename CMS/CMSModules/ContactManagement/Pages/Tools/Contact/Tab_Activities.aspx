<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Activities.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact properties - Activities" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Activities"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Activities/Controls/UI/Activity/List.ascx"
    TagName="ActivityList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDis" Visible="false">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
    
    <asp:Panel runat="server" ID="pnlJourney">
        <h4 class="anchor"><cms:localizedlabel runat="server" ResourceString="om.contact.journey.header" /></h4>
        <div class="form-horizontal form-filter contact-journey">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <span class="control-label"><cms:localizedlabel runat="server" ResourceString="om.contact.journey.length" />:</span>
                </div>   
                <div class="filter-form-value-cell">
                    <span class="contact-journey-value">
                        <span style="font-weight: bold;">
                            <asp:label runat="server" ID="lblJourneyLenght" />
                        </span> (<asp:label runat="server" ID="lblJourneyStarted" />)
                    </span>
                </div>
            </div>       
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <span class="control-label"><cms:localizedlabel runat="server" ResourceString="om.contact.journey.lastactivity" />:</span>
                </div>   
                <div class="filter-form-value-cell">
                    <span class="contact-journey-value">
                        <span style="font-weight: bold;">
                            <asp:label runat="server" ID="lblLastActivity" />
                        </span> (<asp:label runat="server" ID="lblLastActivityDate" />)
                    </span>
                </div>     
            </div>
        </div>
    </asp:Panel>
    
    <h4 class="anchor" runat="server" ID="hdrActivities"><cms:localizedlabel runat="server" ResourceString="om.activity.list" /></h4>
    <cms:ActivityList runat="server" ID="listElem" ZeroRowsText="om.contact.noactivities" FilteredZeroRowsText="om.contact.noactivities.filtered" ShowRemoveButton="true" ShowContactNameColumn="false" FilterLimit="1" />
</asp:Content>
