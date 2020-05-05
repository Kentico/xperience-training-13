<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_FormControls_Pages_Development_View"
    Theme="Default" Title="Form User Control View"  Codebehind="View.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headTitle" EnableViewState="false" Level="4" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblLabel" ResourceString="templatedesigner.fieldcaption" EnableViewState="False" AssociatedControlID="ctrlView" />
            </div>
            <div class="editing-form-value-cell">
                <cms:FormControl ID="ctrlView" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:LocalizedButton runat="server" ID="btnSubmit" Text="Submit" ButtonStyle="Primary" />
            </div>
        </div>
    </div>
</asp:Content>