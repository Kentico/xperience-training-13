<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AutomationDesignerToolbar.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Automation_AutomationDesignerToolbar" %>
<%@ Import Namespace="CMS.UIControls.UniMenuConfig" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/AutomationDesignerToolbarStep.ascx" TagName="AutomationStep" TagPrefix="cms" %>

<div class="automation-toolbar-wrapper">
    <div class="nav-search-container">
        <div class="form-search-container">
            <asp:Label AssociatedControlID="txtSearch" runat="server" CssClass="sr-only">
                <%= GetString("general.search") %>
            </asp:Label>
            <cms:CMSTextBox ID="txtSearch" runat="server" MaxLength="200" EnableViewState="false" WatermarkCssClass="WatermarkText"/>
            <i class="icon-magnifier" aria-hidden="true"></i>
        </div>
    </div>
    <cms:CMSUpdatePanel ID="pnlAjax" class="automation-steps-container" runat="server" EnableViewState="false" ChildrenAsTriggers="false" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:LocalizedLabel ID="lblNoResults" runat="server" ResourceString="general.noresults" Visible="false" EnableViewState="false" />
            <asp:Repeater runat="server" ID="groupRepeater">
                <ItemTemplate>
                    <div class="automation-steps-category collapsible-div">
                        <span class="toggle-icon" onclick="ToggleDiv(this); return false;">
                            <i class="ToggleImage cms-icon-80 icon-minus-square"></i>
                        </span>
                        <span class="editing-form-category-caption h4 anchor" onclick="ToggleDiv(this); return false;"><%#Eval("Caption")%></span>
                        <div class="category-steps">
                            <asp:Repeater ID="stepRepeater" DataSource='<%#Eval("Items")%>' OnItemDataBound="stepRepeater_OnItemDataBound" runat="server">
                                <ItemTemplate>
                                    <cms:AutomationStep ID="step" StepItem="<%# Container.DataItem as Item %>" runat="server" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</div>