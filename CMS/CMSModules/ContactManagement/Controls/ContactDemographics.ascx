<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactDemographics.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_ContactDemographics" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<div class="pattern_tabs tab-content contact-demographics">
    <h3 runat="server" ID="hdrTitle"></h3>
    <div class="btn-group " data-toggle="buttons">
        <a href="#listOfContacts" role="tab" data-toggle="tab" class="btn btn-default active">
            <cms:LocalizedLabel ID="lblListOfContacts" runat="server" ResourceString="om.contact.demographics.listofcontacts"></cms:LocalizedLabel>
        </a>
        <a href="#graphicalRepresentation" role="tab" data-toggle="tab" class="btn btn-default">
            <cms:LocalizedLabel ID="lblGraphicalRepresentation" runat="server" ResourceString="om.contact.demographics.graphicalrepresentation"></cms:LocalizedLabel>
        </a>
    </div>
    <div id="listOfContacts" class="tab-pane active">
         <cms:UniGrid runat="server" ID="gridElem" OrderBy="ContactLastName" RememberState="False" ShowActionsMenu="True"
            Columns="ContactID,ContactLastName,ContactFirstName,ContactEmail,ContactStatusID,ContactCountryID,ContactCreated"
            IsLiveSite="false" HideFilterButton="True" FilterLimit="0">
            <GridActions Parameters="ContactID">
                <ug:Action Name="edit" Caption="$om.contact.viewdetail$" FontIconClass="icon-eye" CommandArgument="ContactID" FontIconStyle="Allow" ExternalSourceName="view"  />
            </GridActions>
            <GridColumns>
                <ug:Column Source="ContactFirstName" Caption="$om.contact.firstname$" Wrap="false" />
                <ug:Column Source="ContactLastName" Caption="$om.contact.lastname$" Wrap="false" />
                <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false" />
                <ug:Column Name="ContactStatusID" Source="ContactStatusID" ExternalSourceName="#transform: om.contactstatus.contactstatusdisplayname" AllowSorting="false" Caption="$om.contactstatus$" Wrap="false" />
                <ug:Column Name="ContactCountryID" Source="ContactCountryID" ExternalSourceName="#transform: cms.country.countrydisplayname" AllowSorting="false" Caption="$general.country$" Wrap="false" />
                <ug:Column Source="ContactCreated" Caption="$general.created$" Wrap="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" />
        </cms:UniGrid>
    </div>
    <div id="graphicalRepresentation" class="tab-pane">
        <h4 class="anchor"><cms:LocalizedLabel ID="lblLocation" runat="server" ResourceString="om.contact.demographics.graphicalrepresentation.location"></cms:LocalizedLabel></h4>
        <div runat="server" ID="mapDiv" class="map"></div>
        
        <h4 class="anchor"><cms:LocalizedLabel ID="lblPersonaAndGender" runat="server" ResourceString="om.contact.demographics.graphicalrepresentation.personaandgender"></cms:LocalizedLabel></h4>
        <div runat="server" ID="personaChartDiv" class="personas-chart chart col-xs-12 col-md-12 col-lg-6"></div>
        <div runat="server" ID="genderChartDiv" class="gender-chart chart  col-xs-12 col-md-12 col-lg-6"></div>
        
        <h4 class="anchor"><cms:LocalizedLabel ID="lblAge" runat="server" ResourceString="om.contact.demographics.graphicalrepresentation.age"></cms:LocalizedLabel></h4>
        <div runat="server" ID="ageChartDiv" class="chart"></div>
    </div>
</div>
