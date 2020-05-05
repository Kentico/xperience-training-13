<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectContactDeletionImplementation.ascx.cs" EnableViewState="false" Inherits="CMSModules_OnlineMarketing_Controls_Settings_SelectContactDeletionImplementation" %>
<%@ Register Src="~/CMSFormControls/Basic/RadioButtonsControl.ascx" TagName="CMSRadioButtonsControl" TagPrefix="cms" %>

<div>
    <cms:CMSRadioButtonsControl ID="rbList" runat="server" />
    <div class="explanation-text-settings">
        <asp:Literal  runat="server" ID="litExplanationText"></asp:Literal>
    </div>
</div>