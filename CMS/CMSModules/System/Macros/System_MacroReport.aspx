<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_System_Macros_System_MacroReport" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - Macros" CodeBehind="System_MacroReport.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedLabel ID="lblReport" runat="server" ResourceString="macros.report.description" EnableViewState="False" />
    <br />
    <br />
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:CMSUpdateProgress ID="up" runat="server" HandlePostback="True" HandleAsyncPostback="True" />
            <asp:Panel runat="server" ID="pnlSearch" DefaultButton="btnSearch">
                <div class="form-horizontal form-filter">
                    <div class="form-group">
                        <div class="filter-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblObjectType" ResourceString="General.ObjectType"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="selObjectType" />
                        </div>
                        <div class="filter-form-value-cell">
                            <cms:FormControl ID="selObjectType" FormControlName="ObjectTypeSelector" runat="server">
                                <Properties>
                                    <cms:Property Name="ObjectTypeList" Value="ObjectTypes.ObjectTypesWithMacros" />
                                    <cms:Property Name="DisplayAll" Value="true" />
                                </Properties>
                            </cms:FormControl>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="filter-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblMacroType" ResourceString="Macros.Type"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="drpType" />
                        </div>
                        <div class="filter-form-value-cell">
                            <cms:FormControl runat="server" ID="drpType" FormControlName="MacroType" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="filter-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblReportProblems" ResourceString="Macros.InvalidSignature"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="chkReportProblems" />
                        </div>
                        <div class="filter-form-value-cell">
                            <cms:CMSCheckBox ID="chkReportProblems" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="filter-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFilter" ResourceString="Macros.Contains"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="txtTextToSearch" />
                        </div>
                        <div class="filter-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtTextToSearch" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="filter-form-buttons-cell form-group-buttons">
                            <cms:LocalizedButton runat="server" ID="btnSearch" ResourceString="General.search"
                                ButtonStyle="Primary" EnableViewState="false" OnClick="btnSearch_Click" />
                            <asp:Button ID="btnView" runat="server" CssClass="HiddenButton" OnClick="btnView_Click" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel runat="server" ID="plcItems" Visible="false">
                <table class="table table-hover">
                    <thead>
                        <tr>
                            <th class="wrap-normal">
                                <cms:LocalizedLabel runat="server" ID="lblExpression" ResourceString="macros.expression" EnableViewState="false" />
                            </th>
                            <th class="text-center wrap-normal">
                                <cms:LocalizedLabel runat="server" ID="lblSyntaxValid" ResourceString="macros.syntaxvalid" EnableViewState="false" />
                            </th>
                            <th class="text-center wrap-normal">
                                <cms:LocalizedLabel runat="server" ID="lblSignatureValid" ResourceString="macros.signaturevalid" EnableViewState="false" />
                            </th>
                            <th class="text-center wrap-normal">
                                <cms:LocalizedLabel runat="server" ID="lblMethodCall" ResourceString="macros.methodsvalid" EnableViewState="false" />
                            </th>
                            <th>
                                <cms:LocalizedLabel runat="server" ID="lblObject" ResourceString="general.object" EnableViewState="false" />
                            </th>
                            <th>
                                <cms:LocalizedLabel runat="server" ID="lblColumn" ResourceString="general.field" EnableViewState="false" />
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:PlaceHolder ID="plcRows" runat="server" EnableViewState="false" />
                    </tbody>
                </table>
                <cms:UIPager ID="pagerItems" ShortID="p" ShowDirectPageControl="true" runat="server" VisiblePages="5" PagerMode="Postback" />
            </asp:Panel>
            <cms:LocalizedLabel CssClass="InfoLabel" runat="server" ID="lblInfo" ResourceString="General.NoDataFound"
                Visible="false" EnableViewState="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
