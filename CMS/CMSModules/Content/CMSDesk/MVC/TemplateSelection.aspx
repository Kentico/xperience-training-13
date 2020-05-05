<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" AutoEventWireup="true" CodeBehind="TemplateSelection.aspx.cs" Inherits="CMSModules_Content_CMSDesk_MVC_TemplateSelection" Theme="Default" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">

    <div id="headerActions" class="header-actions-container hidden">
        <div class="header-actions-main">
            <div class="btn-actions">
                <cms:LocalizedButton ID="btnContinue" runat="server" ButtonStyle="Primary" EnableViewState="False" ResourceString="general.continue" ToolTipResourceString="general.continue"
                    OnClientClick="return false;" />
            </div>
        </div>
    </div>

    <div class="selector-messages">
        <cms:AlertLabel runat="server" ID="alError" AlertType="Error" Text="&nbsp;" CssClass="alert-error hidden" EnableViewState="False" />
    </div>

    <div id="defaultTemplates" class="hidden">
        <div class="selector-header">
            <h4><span><%= GetString("pagetemplatesmvc.defaulttemplates")%></span></h4>
        </div>

        <div class="UniFlatContent">
            <div class="UniFlatContentItems">
                <div class="SelectorFlatItems">
                    <div style="clear: both"></div>
                </div>
            </div>
        </div>
    </div>


    <div id="customTemplates" class="hidden">
        <div class="selector-header">
            <h4><span><%= GetString("pagetemplatesmvc.customtemplates")%></span></h4>
        </div>

        <div class="UniFlatContent">
            <div class="UniFlatContentItems">
                <div class="SelectorFlatItems">
                    <div style="clear: both"></div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
