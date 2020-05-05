<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Layouts_Wizard_WizardButtons"
     Codebehind="~/CMSWebParts/Layouts/Wizard/WizardButtons.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniControls/UniButton.ascx" TagName="UniButton"
    TagPrefix="cms" %>
<div class="WizardButtons">
    <cms:UniButton ID="btnBack" runat="server" LinkText="Back" ShowAsButton="true" LinkEvent="Back" LinkCssClass="BackButton" />
    <cms:UniButton ID="btnNext" runat="server" LinkText="Next" ShowAsButton="true" LinkEvent="Next" LinkCssClass="NextButton" />
    <cms:UniButton ID="btnFinish" runat="server" LinkText="Finish" ShowAsButton="true" LinkEvent="Finish" Visible="false" LinkCssClass="FinishButton" />
</div>
