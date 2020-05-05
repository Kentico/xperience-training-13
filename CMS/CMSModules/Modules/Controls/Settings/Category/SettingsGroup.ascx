<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SettingsGroup.ascx.cs"
    Inherits="CMSModules_Modules_Controls_Settings_Category_SettingsGroup" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UGrid" TagPrefix="cms" %>
<cms:CategoryPanel ID="cpCategory" runat="server" Text="Some Category Panel" AllowCollapsing="false"
    DisplayRightPanel="true">
    <cms:CategoryPanelRow ID="cprRow01" runat="server" IsRequired="true" LabelTitle="" ShowFormLabelCell="false">
    <cms:CMSButton ID="btnNewKey" ButtonStyle="Default" runat="server" EnableViewState="false" />
    </cms:CategoryPanelRow>
    <cms:CategoryPanelRow ID="cprModuleInfoRow" runat="server" IsRequired="true" LabelTitle="" ShowFormLabelCell="false">
        <cms:CMSPanel ID="pnlAnotherModule" runat="server">
            <asp:Label ID="lblAnotherModule" runat="server"/>
        </cms:CMSPanel>
    </cms:CategoryPanelRow>
    <cms:CategoryPanelRow ID="cprRow02" runat="server" IsRequired="true" LabelTitle="" ShowFormLabelCell="false">
        <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
            Visible="false" />
        <cms:UGrid runat="server" ID="gridElem" GridName="~/CMSModules/Modules/Controls/Settings/Category/SettingsGroup_List.xml"
            OrderBy="KeyOrder" IsLiveSite="false" />
    </cms:CategoryPanelRow>
</cms:CategoryPanel>
