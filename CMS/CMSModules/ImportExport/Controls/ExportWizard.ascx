<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ExportWizard"
     Codebehind="ExportWizard.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/Wizard/Header.ascx" TagName="WizardHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/ExportPanel.ascx" TagName="ExportPanel"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/ExportConfiguration.ascx" TagName="ExportConfiguration"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/ActivityBar.ascx" TagName="ActivityBar"
    TagPrefix="cms" %>


<script type="text/javascript">
    //<![CDATA[      
    var timerSelectionId = 0;
    
    // End timer function
    function StopSelectionTimer() {
        if (timerSelectionId) {
            clearInterval(timerSelectionId);
            timerSelectionId = 0;

            if (window.HideActivity) {
                window.HideActivity();
            }
        }
    }

    // Start timer function
    function StartSelectionTimer() {
        if (window.Activity) {
            timerSelectionId = setInterval("window.Activity()", 500);
        }
    }
    //]]>
</script>

<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<asp:Panel ID="pnlWrapper" runat="Server">
    <cms:LocalizedLabel ID="lblBeta" runat="server" EnableViewState="false" Visible="false"
        CssClass="ErrorLabel" />

    <div class="GlobalWizard">
        <table>
            <tr class="Top">
                <td class="Left">&nbsp;
                </td>
                <td class="Center">
                    <div style="width: 750px;">
                        <cms:WizardHeader ID="ucHeader" runat="server" EnableViewState="false" />
                    </div>
                </td>
                <td class="Right">&nbsp;
                </td>
            </tr>
            <tr class="Middle">
                <td class="Center" colspan="3">
                    <div id="wzdBody">
                        <cms:CMSWizard ID="wzdExport" ShortID="w" runat="server" DisplaySideBar="False" NavigationButtonStyle-Width="100"
                            NavigationStyle-HorizontalAlign="Right" CssClass="Wizard">
                            <StartNavigationTemplate>
                                <div class="WizardProgress">
                                    <div id="actDiv" style="display: none;">
                                        <div class="WizardProgressLabel">
                                            <cms:LocalizedLabel ID="lblActivityInfo" runat="server" ResourceString="Export.SelectionInfo"
                                                EnableViewState="false" />
                                        </div>
                                        <cms:ActivityBar runat="server" ID="barActivity" Visible="true" />
                                    </div>
                                </div>
                                <div id="buttonsDiv" class="WizardButtons">
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepNextButton" runat="server"
                                        CommandName="MoveNext" ResourceString="general.next" ButtonStyle="Primary" OnClientClick="return exNextStepAction();"
                                        EnableViewState="false" RenderScript="true" />
                                </div>
                            </StartNavigationTemplate>
                            <StepNavigationTemplate>
                                <div id="buttonsDiv" class="WizardButtons">
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepPreviousButton" runat="server"
                                        CommandName="MovePrevious" ResourceString="ExportSiteSettings.PreviousStep" ButtonStyle="Primary"
                                        CausesValidation="false" EnableViewState="false" RenderScript="true" /><cms:LocalizedButton
                                            UseSubmitBehavior="True" ID="StepNextButton" runat="server" CommandName="MoveNext"
                                            ResourceString="general.next" ButtonStyle="Primary" OnClientClick="return exNextStepAction();"
                                            EnableViewState="false" RenderScript="true" />
                                </div>
                            </StepNavigationTemplate>
                            <FinishNavigationTemplate>
                                <div class="WizardProgress">
                                    <div id="actDiv">
                                        <div class="WizardProgressLabel">
                                            <cms:LocalizedLabel ID="lblActivityInfo" runat="server" ResourceString="Export.Progress"
                                                EnableViewState="false" />
                                        </div>
                                        <cms:ActivityBar runat="server" ID="barActivity" Visible="true" />
                                    </div>
                                </div>
                                <div id="buttonsDiv" class="WizardButtons">
                                    <cms:LocalizedButton UseSubmitBehavior="False" ID="StepCancelButton" runat="server"
                                        ResourceString="general.cancel" ButtonStyle="Primary" EnableViewState="false" RenderScript="true" /><cms:LocalizedButton
                                            UseSubmitBehavior="True" ID="StepFinishButton" runat="server" Enabled="false"
                                            CommandName="MoveComplete" ResourceString="general.finish" ButtonStyle="Primary"
                                            EnableViewState="false" RenderScript="true" />
                                </div>
                            </FinishNavigationTemplate>
                            <WizardSteps>
                                <asp:WizardStep ID="wzdStepStart" runat="server" AllowReturn="False" StepType="Start"
                                    EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px">
                                        <cms:ExportConfiguration ID="configExport" ShortID="c" runat="server" />
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="wzdStepSelectionPanel" runat="server" AllowReturn="False" StepType="Step"
                                    EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px; padding: 0px;">
                                        <div class="WizardBorder">
                                            <cms:ExportPanel ID="pnlExport" ShortID="p" runat="server" />
                                        </div>
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="wzdStepProgress" runat="server" AllowReturn="False" StepType="Finish"
                                    EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px">
                                        <cms:AsyncControl ID="ctlAsyncExport" runat="server" ProvideLogContext="true" LogContextNames="Export" PostbackOnFinish="false" PostbackOnError="false" FinishClientCallback="Finished" />
                                    </div>
                                </asp:WizardStep>
                            </WizardSteps>
                        </cms:CMSWizard>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="pnlError" runat="server" CssClass="GlobalWizard">
        <cms:AlertLabel runat="server" ID="lblError" AlertType="Error" Visible="True" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel ID="pnlWarning" runat="server" CssClass="GlobalWizard">
        <cms:AlertLabel runat="server" ID="lblWarning" AlertType="Warning" Visible="True" EnableViewState="false" />
    </asp:Panel>
</asp:Panel>
<asp:Panel ID="pnlErrorBlank" runat="server">
    <cms:AlertLabel runat="server" ID="lblErrorBlank" AlertType="Error" EnableViewState="false" />
</asp:Panel>
<asp:Panel ID="pnlPermissions" runat="server" Visible="false" EnableViewState="false">
    <asp:HyperLink ID="lnkPermissions" runat="server" EnableViewState="false" />
</asp:Panel>
<asp:HiddenField ID="hdnState" runat="server" />
<asp:Literal ID="ltlScriptAfter" runat="server" EnableViewState="false" />
<cms:AsyncControl ID="ctrlAsyncSelection" runat="server" />
