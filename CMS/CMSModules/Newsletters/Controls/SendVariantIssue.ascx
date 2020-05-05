<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SendVariantIssue.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_SendVariantIssue" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/GroupSizeSlider.ascx" TagPrefix="cms"
    TagName="GroupSlider" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/WinnerOptions.ascx" TagPrefix="cms"
    TagName="WinnerOptions" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/VariantMailout.ascx" TagPrefix="cms"
    TagName="VariantMailout" %>

<cms:LocalizedHeading ID="lhdSizeOfTestGroup" Visible="true" runat="server" Level="4" ResourceString="newsletterissue_send.sizeoftestgroup"></cms:LocalizedHeading>
<asp:Panel runat="server" ID="pnlSlider" CssClass="content-block">
    <cms:GroupSlider ID="ucGroupSlider" runat="server" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlMailout" CssClass="content-block">
    <asp:Label ID="lblAdditionalInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
    <cms:VariantMailout ID="ucMailout" runat="server" />
</asp:Panel>
<div class="content-block clearfix">
    <cms:WinnerOptions ID="ucWO" runat="server" Visible="false" />
</div>
<cms:CMSUpdatePanel runat="server" ID="pnlU2">
    <ContentTemplate>
        <asp:Label runat="server" ID="lblWillBeSent" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
