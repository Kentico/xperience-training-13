<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"  Codebehind="Benchmark.aspx.cs" Inherits="CMSModules_System_Macros_Benchmark" Theme="Default" %>

<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="editorElem" ID="lblExpression" CssClass="control-label" runat="server" ResourceString="macros.benchmark.text" DisplayColon="true" />
            </div>
            <div class="form-field-full-column-width editing-form-value-cell">
                <cms:MacroEditor ID="editorElem" runat="server" Text="<div>{% CMSContext.Current %}</div>" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="txtIterations" ID="lblIterations" CssClass="control-label" runat="server" ResourceString="macros.benchmark.iterations" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtIterations" Text="1000" />

            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:LocalizedButton runat="server" ID="btnRefresh" ResourceString="macros.benchmark.run"
                    OnClick="btnRun_Click" ButtonStyle="Primary" />
            </div>
        </div>

        <cms:LocalizedHeading AssociatedControlID="txtResults" ID="lblResults" runat="server" ResourceString="macros.benchmark.results" Level="4" />
        <div class="form-group">
            <cms:CMSTextArea runat="server" ID="txtResults" ProcessMacroSecurity="False" Rows="10" Width="100%" ReadOnly="True" />
        </div>
        <cms:LocalizedHeading AssociatedControlID="txtOutput" ID="lblOutput" runat="server" ResourceString="macros.console.resultoutput" Level="4" />
        <div class="form-group">
            <cms:CMSTextArea runat="server" ID="txtOutput" Rows="10" Width="100%" ReadOnly="True" />
        </div>
    </div>
</asp:Content>