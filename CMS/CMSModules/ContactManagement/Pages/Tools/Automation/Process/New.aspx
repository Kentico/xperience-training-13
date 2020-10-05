<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="New automation process" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_New"
    Theme="Default" CodeBehind="New.aspx.cs" ValidateRequest="false" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Repeater runat="server" ID="repeaterElement">
        <HeaderTemplate>
            <div class="ma-template-selector-wrapper">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="ma-template-selector-item <%#Eval("FromScratchCssClass")%>">
                <div class="ma-template-selector-item-envelope" style="overflow: hidden" title="<%#Eval("Tooltip")%>">
                    <cms:CMSMoreOptionsButton ID="btnMoreOptions" runat="server" DropDownItemsAlignment="Right" RenderFirstActionSeparately="false" CssClass="ma-template-selector-item-actions" />
                    <div class="ma-template-selector-item-content" onclick="createWorkflow(<%#Eval("Id")%>)">
                        <div class="ma-template-selector-item-icon">
                            <%#Eval("IconClass")%>
                        </div>
                        <span class="ma-template-selector-item-name h4" title="<%#Eval("NameTooltip")%>">
                            <%#Eval("Name")%>
                        </span>
                        <span class="ma-template-selector-item-description" title="<%#Eval("DescriptionTooltip")%>">
                            <%#Eval("Description")%>
                        </span>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <div style="clear: both">
            </div>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
