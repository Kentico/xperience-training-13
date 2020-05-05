<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSInstall_Controls_StepNavigation"
     Codebehind="StepNavigation.ascx.cs" %>
<%@ Register TagPrefix="cms" TagName="Help" Src="~/CMSAdminControls/UI/PageElements/Help.ascx" %>
<%@ Register TagPrefix="cms" TagName="ActivityBar" Src="~/CMSAdminControls/UI/System/ActivityBar.ascx" %>
<asp:Literal ID="ltlTableBefore" runat="server" Visible="False" />
<div class="install-footer">
    <div id="buttonsDiv" class="footer-actions">
        <cms:LocalizedButton ID="StepPrevButton" Source="file" ButtonStyle="Default" runat="server"
            CommandName="MovePrevious" Text="{$Install.BackStep$}" RenderScript="true"
            UseSubmitBehavior="False" />
        <cms:LocalizedButton UseSubmitBehavior="True" Source="file" ButtonStyle="Primary"
            ID="StepNextButton" runat="server" CommandName="MoveNext" Text="{$Install.NextStep$}"
            RenderScript="true" />
    </div>
    <span class="footer-info">
        <cms:Help ID="hlpContext" runat="server" Visible="True" />
    </span>
    <span id="activity" class="footer-activity" style="display: none;">
        <cms:ActivityBar runat="server" ID="barActivity" Visible="true" />
    </span>
</div>
<asp:Literal ID="ltlTableAfter" runat="server" Visible="False" />