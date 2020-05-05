<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_EmailBuilder_EmailWidgetProperties" Theme="Default"
    CodeBehind="EmailWidgetProperties.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="widget-properties-sidebar">
        <div class="widget-properties-header">
            <div class="widget-properties-close">
                <button class="widget-properties-close-btn" type="button">
                    <i class="icon-modal-close cms-icon-100"></i>
                </button>
            </div>
            <h1>
                <cms:LocalizedLabel ID="lblWidgetName" runat="server"></cms:LocalizedLabel>
            </h1>
        </div>
        <asp:UpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="widget-properties-content">
                    <div class="header-actions-container">
                        <cms:FormSubmitButton ID="btnSubmit" runat="server" ResourceString="general.apply" RegisterHeaderAction="false" />
                    </div>
                    <div class="widget-properties-form scroll-area">
                        <cms:MessagesPlaceHolder ID="plcMessages" runat="server" IsLiveSite="false"></cms:MessagesPlaceHolder>
                        <cms:BasicForm ID="propertiesForm" ShortID="pf" runat="server" MarkRequiredFields="True"></cms:BasicForm>
                        <asp:HiddenField ID="hdnSaveStatus" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
