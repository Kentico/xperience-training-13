<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSModules_ImportExport_Controls_NewSiteWizard" Codebehind="NewSiteWizard.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/Wizard/Header.ascx" TagName="WizardHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/ImportPanel.ascx" TagName="ImportPanel" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/ImportSiteDetails.ascx" TagName="ImportSiteDetails" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/ActivityBar.ascx" TagName="ActivityBar" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/NewSiteFinish.ascx" TagName="NewSiteFinish" TagPrefix="cms" %>


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

    // Cancel import
    function CancelImport() {
        GetImportState(true);
        return false;
    }
    //]]>
</script>

<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<asp:Panel ID="pnlWrapper" runat="Server">
    <div class="GlobalWizard">
        <table>
            <tr class="Top">
                <td class="Left">&nbsp;
                </td>
                <td class="Center">
                    <cms:WizardHeader ID="ucHeader" runat="server" />
                </td>
                <td class="Right">&nbsp;
                </td>
            </tr>
            <tr class="Middle">
                <td class="Center" colspan="3">
                    <div id="wzdBody">
                        <asp:Wizard ID="wzdImport" runat="server" DisplaySideBar="False" NavigationButtonStyle-Width="100"
                            NavigationStyle-HorizontalAlign="Right" CssClass="Wizard">
                            <StartNavigationTemplate>
                                <div class="WizardProgress">
                                    <div id="actDiv" style="display: none;">
                                        <div class="WizardProgressLabel">
                                            <cms:LocalizedLabel ID="lblActivityInfo" runat="server" Text="{$Export.SelectionInfo$}" />
                                        </div>
                                        <cms:ActivityBar runat="server" ID="barActivity" Visible="true" />
                                    </div>
                                </div>
                                <div id="buttonsDiv" class="WizardButtons">
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepNextButton" runat="server"
                                        CommandName="MoveNext" Text="{$general.next$}" ButtonStyle="Primary" OnClientClick="return NextStepAction();"
                                        RenderScript="true" />
                                </div>
                            </StartNavigationTemplate>
                            <StepNavigationTemplate>
                                <div class="WizardProgress">
                                    <% if (wzdImport.ActiveStepIndex == 2)
                                       { %>
                                    <div id="actDiv">
                                        <div class="WizardProgressLabel">
                                            <cms:LocalizedLabel ID="lblActivityImportInfo" runat="server" Text="{$import.progress$}" />
                                        </div>
                                        <cms:ActivityBar runat="server" ID="barActivity" Visible="true" />
                                    </div>
                                    <% } %>
                                </div>
                                <div id="buttonsDiv" class="WizardButtons control-group-inline">
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepPreviousButton" runat="server"
                                        CommandName="MovePrevious" Text="{$ExportSiteSettings.PreviousStep$}" ButtonStyle="Primary"
                                        CausesValidation="false" RenderScript="true" />
                                    <% if (wzdImport.ActiveStepIndex == 2)
                                       { %>
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepCancelButton" runat="server"
                                        CommandName="Cancel" Text="{$general.cancel$}" ButtonStyle="Primary" CausesValidation="false"
                                        RenderScript="true" />
                                    <% } %>
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepNextButton" runat="server" CommandName="MoveNext" Text="{$general.next$}" ButtonStyle="Primary"
                                        OnClientClick="return NextStepAction();" RenderScript="true" />
                                </div>
                            </StepNavigationTemplate>
                            <FinishNavigationTemplate>
                                <div id="buttonsDiv" class="WizardButtons control-group-inline">
                                    <cms:LocalizedButton UseSubmitBehavior="True" ID="StepFinishButton" runat="server"
                                        CommandName="MoveComplete" ResourceString="general.finish" ButtonStyle="Primary"
                                        RenderScript="true" />
                                </div>
                            </FinishNavigationTemplate>
                            <WizardSteps>
                                <asp:WizardStep ID="wzdStepSiteDetails" runat="server" AllowReturn="False" StepType="Start" EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px">
                                        <cms:ImportSiteDetails ID="siteDetails" runat="server" AllowExisting="false" />
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="wzdStepSelection" runat="server" AllowReturn="False" StepType="Step" EnableViewState="true">
                                    <div class="GlobalWizardStepPanel" style="height: <%=PanelHeight%>px;">
                                        <div class="WizardBorder">
                                            <cms:ImportPanel ID="pnlImport" runat="server" />
                                        </div>
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="wzdStepProgress" runat="server" AllowReturn="False" StepType="Step" EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px">
                                        <cms:AsyncControl ID="ctlAsyncImport" runat="server" LogContextNames="Import" ProvideLogContext="true" PostbackOnError="false" FinishClientCallback="Finished" />
                                    </div>
                                </asp:WizardStep>
                                <asp:WizardStep ID="wzdStepFinished" runat="server" AllowReturn="False" StepType="Finish" EnableViewState="true">
                                    <div class="GlobalWizardStep" style="height: <%=PanelHeight%>px">
                                        <cms:NewSiteFinish ID="finishSite" runat="server" />
                                    </div>
                                </asp:WizardStep>
                            </WizardSteps>
                        </asp:Wizard>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="pnlError" runat="server" CssClass="GlobalWizard">
        <cms:AlertLabel runat="server" ID="lblError" AlertType="Error" Visible="true" />
    </asp:Panel>
    <asp:Panel ID="pnlWarning" runat="server" CssClass="GlobalWizard">
        <cms:AlertLabel runat="server" ID="lblWarning" AlertType="Warning" Visible="true" />
    </asp:Panel>
</asp:Panel>
<br />
<asp:Panel ID="pnlPermissions" runat="server" Visible="false" EnableViewState="false">
    <br />
    <asp:HyperLink ID="lnkPermissions" runat="server" />
</asp:Panel>
<asp:HiddenField ID="hdnState" runat="server" />
<asp:Literal ID="ltlScriptAfter" runat="server" EnableViewState="false" />
<cms:AsyncControl ID="ctrlAsyncSelection" runat="server" />
