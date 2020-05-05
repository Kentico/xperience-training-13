<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ProfilesMenuControl.ascx.cs"
    Inherits="CMSModules_DeviceProfiles_Controls_ProfilesMenuControl" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniMenuButtons.ascx" TagName="UniMenuButtons"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcBigButton" runat="server" Visible="false">
    <div class="ActionButtons">
        <cms:UniMenuButtons ID="buttons" ShortID="b" runat="server" EnableViewState="false"
            AllowSelection="false" />
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcSmallButton" runat="server" Visible="false">
    <cms:CMSSelectableToggleButton runat="server" ID="btnProfilesSelector" EnableViewState="False" ShortID="ps" CssClass="device-profile-menu" DropDownItemsAlignment="Right" />
    <cms:CMSButtonGroup ID="rotationButtons" runat="server" Visible="false" CssClass="device-rotation" />
</asp:PlaceHolder>
