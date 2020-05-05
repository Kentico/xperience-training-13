<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_WebParts"
    Theme="Default" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Page Template Edit - Web Parts"  Codebehind="PageTemplate_WebParts.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ObjectLockingPanel runat="server" ID="pnlObjectLocking">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell label-full-width">
                    <asp:Label runat="server" ID="lblWPConfig" EnableViewState="false" CssClass="control-label editing-form-label" /><br />
                </div>
                <div class="editing-form-value-cell textarea-full-width">
                    <cms:ExtendedTextArea runat="server" ID="txtWebParts" EnableViewState="false" EditorMode="Advanced"
                        Language="XML" Width="98%" Height="480px" />
                </div>
            </div>
        </div>
    </cms:ObjectLockingPanel>
</asp:Content>
