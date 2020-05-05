<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSFormControls_CountrySelector"  Codebehind="CountrySelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<div class="country-selector">
    <cms:UniSelector ID="uniSelectorCountry" runat="server" DisplayNameFormat="{%CountryDisplayName%}" SelectionMode="SingleDropDownList"
        ObjectType="cms.country" ResourcePrefix="countryselector" AllowAll="false" AllowEmpty="false" OnOnSelectionChanged="uniSelectorCountry_OnSelectionChanged" />
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <asp:PlaceHolder runat="server" ID="plcStates">
                <cms:UniSelector ID="uniSelectorState" runat="server" DisplayNameFormat="{%StateDisplayName%}" SelectionMode="SingleDropDownList"
                    ObjectType="cms.state" ResourcePrefix="stateselector" />
            </asp:PlaceHolder>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="uniSelectorCountry:drpSingleSelect" />
        </Triggers>
    </cms:CMSUpdatePanel>
</div>
    
