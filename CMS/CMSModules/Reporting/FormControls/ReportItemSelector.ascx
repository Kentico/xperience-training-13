<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_FormControls_ReportItemSelector"
     Codebehind="ReportItemSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:HiddenField ID="hdnGuid" runat="server" />
        <asp:Panel runat="server" ID="pnlReports" CssClass="form-group">
            <cms:UniSelector ID="usReports" runat="server" ObjectType="reporting.report" SelectionMode="SingleDropDownList"
                AllowEditTextBox="false" IsLiveSite="false" DisplayNameFormat="{%ReportDisplayName%}"
                ReturnColumnName="ReportName" AllowEmpty="false" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlItems">
            <cms:UniSelector ID="usItems" runat="server" SelectionMode="SingleDropDownList" AllowEditTextBox="false"
                Enabled="false" AllowEmpty="false" />
        </asp:Panel>
        <asp:Panel CssClass="ReportParametersWebPartPanel form-group" runat="server" ID="pnlParameters" Visible="false">
            <div class="ReportParametersWebPartTable">
                <cms:UniGrid ID="ugParameters" runat="server" IsLiveSite="false" ZeroRowsText="" ShowActionsMenu="false"
                    RememberState="false" ApplyPageSize="false">
                    <GridColumns>
                        <ug:Column runat="server" Source="ParameterName" Caption="$rep.webparrts.parametername$" Localize="true" Wrap="false" />
                        <ug:Column runat="server" Source="ParameterValue" Caption="$rep.webparrts.parametervalue$" Wrap="false" />
                    </GridColumns>
                    <GridOptions AllowSorting="false" DisplayFilter="false" />
                    <PagerConfig DisplayPager="false" />
                </cms:UniGrid>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlParametersButtons" runat="server" Visible="false">
            <cms:LocalizedButton ID="btnSet" runat="server" ResourceString="rep.webparts.setparameters"
                OnClick="btnSet_Click" ButtonStyle="Default" />
            <cms:LocalizedButton ID="btnClear" runat="server" ResourceString="rep.webparts.clearparameters"
                OnClick="btnClear_Click" ButtonStyle="Default" />
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>